using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGeneratorCs
{
	class MapConstructor
	{
		// === Fields ===
		private readonly bool ENABLE_DETAILED_LOGGING;
		private ((int x, int y) topLeft, (int x, int y) bottomRight) bounds = ((0, 0), (0, 0));
		private (int width, int height) mapSize => (bounds.bottomRight.x - bounds.topLeft.x + 1,
												   bounds.bottomRight.y - bounds.topLeft.y + 1);
		private (int x, int y) currentPosition;
		private int seed;
		private int length;
		private int thickness;
		private int padding;
		private bool enableTypesInMap;

		public int[,] IntMap2D;

		/// <summary>Canonical node dictionary (world coords). Start/End/Default included.</summary>
		public Dictionary<(int x, int y), TileSpawnType> Nodes
			= new Dictionary<(int x, int y), TileSpawnType>();

		public Dictionary<(int x, int y), TileSpawnType> GetSpecialNodesWithOffset()
		{
			UpdateBoundsFromNodes();
			var (ox, oy) = GetCurrentOffset();

			return Nodes
				.Where(kvp => kvp.Value != TileSpawnType.Default && kvp.Value != TileSpawnType.Empty)
				.ToDictionary(
					kvp => (kvp.Key.x + ox, kvp.Key.y + oy),
					kvp => kvp.Value
				);
		}


		// === Ctor ===
		public MapConstructor(
			int length, int thickness, int collisionRadius,
			(int enemyFactor, int landmarkFactor, int treasureFactor, bool isBoss, bool isQuest) spawnFactors,
			bool enableDetailedLogging = false,
			bool enableTypesInMap = false
		)
		{
			this.enableTypesInMap = enableTypesInMap;
			this.length = length;
			this.seed = Random.Shared.Next();
			this.thickness = thickness;
			this.padding = thickness + 1;
			ENABLE_DETAILED_LOGGING = enableDetailedLogging;

			// Build weighted spawn pool
			var spawns = new List<TileSpawnType>();
			for (int i = 0; i < spawnFactors.enemyFactor; i++) spawns.Add(TileSpawnType.Enemy);
			for (int i = 0; i < spawnFactors.landmarkFactor; i++) spawns.Add(TileSpawnType.Landmark);
			for (int i = 0; i < spawnFactors.treasureFactor; i++) spawns.Add(TileSpawnType.Treasure);
			if (spawnFactors.isBoss) spawns.Add(TileSpawnType.Boss);
			if (spawnFactors.isQuest) spawns.Add(TileSpawnType.Quest);

			GenerateStandardNodes();
			FillNodeTypes(spawns, collisionRadius);
			UpdateIntMap2DFromNodeDictTypeBasic();
		}

		// === Generation ===
		private void GenerateStandardNodes()
		{
			if (ENABLE_DETAILED_LOGGING) Console.WriteLine("[MapConstructor] Generating Nodes...");
			var random = new Random(seed);
			currentPosition = (0, 0);

			while (Nodes.Count < length)
			{
				bool isStart = Nodes.Count == 0;
				bool isEnd = Nodes.Count == length - 1;

				if (!Nodes.ContainsKey(currentPosition))
				{
					if (isStart) Nodes[currentPosition] = TileSpawnType.Start;
					else if (isEnd) Nodes[currentPosition] = TileSpawnType.End;
					else Nodes[currentPosition] = TileSpawnType.Default;

					if (ENABLE_DETAILED_LOGGING)
						Console.WriteLine($"Added node at {currentPosition.x}, {currentPosition.y} ({Nodes.Count}/{length})");
				}

				var directions = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
				var dir = directions[random.Next(directions.Length)];
				currentPosition = (currentPosition.x + dir.x, currentPosition.y + dir.y);
			}
		}

		private void FillNodeTypes(List<TileSpawnType> typesToFill, int collisionRadius)
		{
			if (ENABLE_DETAILED_LOGGING) Console.WriteLine("[MapConstructor] Filling Node Types...");
			var random = new Random(seed);

			var emptyKeys = new List<(int x, int y)>();
			foreach (var kvp in Nodes) if (kvp.Value == TileSpawnType.Default) emptyKeys.Add(kvp.Key);

			// shuffle
			for (int i = 0; i < emptyKeys.Count; i++)
			{
				int j = random.Next(i, emptyKeys.Count);
				(emptyKeys[i], emptyKeys[j]) = (emptyKeys[j], emptyKeys[i]);
			}

			// assign regular types
			for (int e = 0; e < emptyKeys.Count && typesToFill.Count > 0; e++)
			{
				var key = emptyKeys[e];
				if (IsNodesRadiusOccupied(key, collisionRadius)) continue;

				int index = random.Next(typesToFill.Count);
				var typeToAssign = typesToFill[index];

				// ensure single Boss/Quest usage
				if (typeToAssign == TileSpawnType.Boss || typeToAssign == TileSpawnType.Quest)
					typesToFill.RemoveAt(index);

				Nodes[key] = typeToAssign;
			}

			// ensure important types exist
			var important = new List<TileSpawnType> { TileSpawnType.Boss, TileSpawnType.Quest };
			foreach (var kvp in Nodes) if (important.Contains(kvp.Value)) important.Remove(kvp.Value);
			if (important.Count == 0) return;

			var candidates = new List<(int x, int y)>();
			foreach (var kvp in Nodes) if (kvp.Value == TileSpawnType.Default) candidates.Add(kvp.Key);
			if (candidates.Count == 0)
				foreach (var kvp in Nodes) if (kvp.Value != TileSpawnType.Start && kvp.Value != TileSpawnType.End) candidates.Add(kvp.Key);

			// shuffle
			for (int i = 0; i < candidates.Count; i++)
			{
				int j = new Random(seed ^ i).Next(i, candidates.Count);
				(candidates[i], candidates[j]) = (candidates[j], candidates[i]);
			}

			foreach (var type in new List<TileSpawnType>(important))
			{
				bool placed = false;
				foreach (var key in candidates)
				{
				 if (IsNodesRadiusOccupied(key, collisionRadius)) continue;
				 Nodes[key] = type; placed = true; break;
				}
				if (!placed && ENABLE_DETAILED_LOGGING)
					Console.WriteLine($"[MapConstructor] Warning: couldn't place {type} due to collisions.");
			}
		}

		// === Bounds / Offset (pure) ===
		private void UpdateBoundsFromNodes()
		{
			if (ENABLE_DETAILED_LOGGING) Console.WriteLine("[MapConstructor] Updating Bounds...");
			if (Nodes == null || Nodes.Count == 0) throw new InvalidOperationException("No nodes to update bounds.");

			bool first = true;
			foreach (var kvp in Nodes)
			{
				var k = kvp.Key;
				if (first)
				{
					bounds.topLeft = k;
					bounds.bottomRight = k;
					first = false;
					continue;
				}
				if (k.x < bounds.topLeft.x) bounds.topLeft.x = k.x;
				if (k.y < bounds.topLeft.y) bounds.topLeft.y = k.y;
				if (k.x > bounds.bottomRight.x) bounds.bottomRight.x = k.x;
				if (k.y > bounds.bottomRight.y) bounds.bottomRight.y = k.y;
			}
		}

		private (int ox, int oy) GetCurrentOffset()
		{
			UpdateBoundsFromNodes();
			return (-bounds.topLeft.x + padding, -bounds.topLeft.y + padding);
		}

		// === Map conversion (no node reposition) ===
		private void UpdateIntMap2DFromNodeDictTypeBasic() 
		{
			var nodes = Nodes;
			UpdateBoundsFromNodes();
			if (nodes == null || nodes.Count == 0 || length <= 0)
				throw new InvalidOperationException("No nodes to convert to map.");

			// compute bounds from the passed-in nodes (pure; does not touch `bounds` field)
			int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
			foreach (var kvp in nodes)
			{
				var k = kvp.Key;
				if (k.x < minX) minX = k.x;
				if (k.y < minY) minY = k.y;
				if (k.x > maxX) maxX = k.x;
				if (k.y > maxY) maxY = k.y;
			}

			int width = maxX - minX + 1;
			int height = maxY - minY + 1;
			int ox = -minX + padding;
			int oy = -minY + padding;

			int mapW = width + padding * 2;
			int mapH = height + padding * 2;
			int[,] map = new int[mapW, mapH];

			if (ENABLE_DETAILED_LOGGING) Console.WriteLine("[MapConstructor] Converting to Map...");

			// iterate the passed nodes (safe even if caller calls this repeatedly)
			int i = 0, n = nodes.Count;
			foreach (var point in nodes)
			{
				if (ENABLE_DETAILED_LOGGING) Console.WriteLine($"Adding node {++i}/{n} at {point.Key}");

				int mapX = point.Key.x + ox;
				int mapY = point.Key.y + oy;

				// thickness shape (disk)
				for (int dx = -thickness; dx <= thickness; dx++)
				for (int dy = -thickness; dy <= thickness; dy++)
				{
					int distSq = dx * dx + dy * dy;
					if (distSq > thickness * thickness) continue;

					int ix = mapX + dx, iy = mapY + dy;
					if (ix >= 0 && ix < mapW && iy >= 0 && iy < mapH)
						if (map[ix, iy] == (int)TileSpawnType.Empty)
							map[ix, iy] = (int)TileSpawnType.Default;
				}

				// center
				if (mapX >= 0 && mapX < mapW && mapY >= 0 && mapY < mapH)
					map[mapX, mapY] = enableTypesInMap ? (int)point.Value : (int)TileSpawnType.Default;
			}

			IntMap2D = map;
		}

		// === Utilities ===
		private bool IsNodesRadiusOccupied((int x, int y) position, int radius, bool includeTypeNone = false)
		{
			for (int dx = -radius; dx <= radius; dx++)
			for (int dy = -radius; dy <= radius; dy++)
			{
				var p = (position.x + dx, position.y + dy);
				if (Nodes.TryGetValue(p, out var node))
				{
					if (!includeTypeNone && node == TileSpawnType.Default) continue;
					return true;
				}
			}
			return false;
		}

		// === Logging ===
		public void PrintNodesToConsole()
		{
			Console.WriteLine("Nodes:");
			foreach (var node in ConvertNodesToList())
				Console.WriteLine($"Node at ({node.Item1.x}, {node.Item1.y}): {node.Item2}");
		}

		public void PrintMapToConsole()
		{
			Console.WriteLine("Map Layout:");
			for (int y = 0; y < IntMap2D.GetLength(1); y++)
			{
				string line = "";
				for (int x = 0; x < IntMap2D.GetLength(0); x++)
				{
					var val = (IntMap2D[x, y] >= 0) ? IntMap2D[x, y].ToString() : ".";
					line += val + " ";
				}
				Console.WriteLine(line);
			}
		}

		// === Helpers ===
		public List<((int x, int y), TileSpawnType)> ConvertNodesToList(List<TileSpawnType> typesToInclude = null)
		{
			var list = new List<((int x, int y), TileSpawnType)>();
			typesToInclude ??= GetDefaultTypes();
			foreach (var kvp in Nodes) if (typesToInclude.Contains(kvp.Value)) list.Add((kvp.Key, kvp.Value));
			return list;
		}

		private List<TileSpawnType> GetDefaultTypes() => new()
		{
			TileSpawnType.Start, TileSpawnType.End, TileSpawnType.Enemy,
			TileSpawnType.Landmark, TileSpawnType.Treasure, TileSpawnType.Boss, TileSpawnType.Quest
		};

		// === Enums ===
		public enum TileSpawnType
		{
			Empty = 0,
			Default = 1,
			Start = 2,
			End = 3,
			Treasure = 4,
			Enemy = 5,
			Landmark = 6,
			Boss = 7,
			Quest = 8,
		}
	}

	// === Tests ===
	public static class Test
	{
		public static void RunTests()
		{
			Console.WriteLine("Running MapConstructor Tests...");

			var constructor = new MapConstructor(
				length: 20,
				thickness: 1,
				collisionRadius: 2,
				spawnFactors: (enemyFactor: 5, landmarkFactor: 3, treasureFactor: 2, isBoss: true, isQuest: true),
				enableDetailedLogging: true,
				enableTypesInMap: true
			);

			constructor.PrintNodesToConsole();
			constructor.PrintMapToConsole();
		}
	}
}
