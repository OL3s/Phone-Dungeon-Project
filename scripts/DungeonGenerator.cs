using System;
using System.Collections.Generic;
using Godot;
using System.Diagnostics.Metrics;

namespace MapGeneratorCs
{
	class MapConstructor
	{
		// === Fields ===
		private static bool ENABLE_DETAILED_LOGGING = false;
		private ((int x, int y) topLeft, (int x, int y) bottomRight) bounds = ((0, 0), (0, 0));
		private (int width, int height) mapSize => (bounds.bottomRight.x - bounds.topLeft.x + 1,
												  bounds.bottomRight.y - bounds.topLeft.y + 1);
		private (int x, int y) currentPosition;
		private int seed;
		private int length;
		private int thickness;
		private int padding;
		public int[,] IntMap2D;
		private Dictionary<(int x, int y), TileSpawnType> Nodes
			= new Dictionary<(int x, int y), TileSpawnType>();

		// === Constructors ===
		public MapConstructor(
			int length, int thickness, int collisionRadius,
			(int enemyFactor, int landmarkFactor, int treasureFactor,
			bool isBoss, bool isQuest) spawnFactors, bool enableDetailedLogging = true)
		{
			// Initialize parameters
			this.length = length;
			this.seed = Random.Shared.Next();
			this.thickness = thickness;
			this.padding = thickness + 1;
			ENABLE_DETAILED_LOGGING = enableDetailedLogging;

			// Add spawn types based on factors
			var spawns = new List<TileSpawnType>();
			for (int i = 0; i < spawnFactors.enemyFactor; i++) spawns.Add(TileSpawnType.EnemySpawn);
			for (int i = 0; i < spawnFactors.landmarkFactor; i++) spawns.Add(TileSpawnType.Landmark);
			for (int i = 0; i < spawnFactors.treasureFactor; i++) spawns.Add(TileSpawnType.Treasure);
			if (spawnFactors.isBoss) spawns.Add(TileSpawnType.BossSpawn);
			if (spawnFactors.isQuest) spawns.Add(TileSpawnType.Quest);

			// Generate the map
			GenerateStandardNodes();
			FillNodeTypes(spawns, collisionRadius);
			IntMap2D = ConvertToMap(Nodes, includeTypes: true);

		}



		// === GENERATION METHODS ===
		// Generates nodes in a random walk fashion, with types: Start, End, Default
		private void GenerateStandardNodes()
		{
			Console.WriteLine("Generating Nodes...");
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
						Console.WriteLine($"Added node at {currentPosition.x}, {currentPosition.y} (total {Nodes.Count}/{length})");
				}

				var directions = new List<(int x, int y)> { (1, 0), (-1, 0), (0, 1), (0, -1) };
				var dir = directions[random.Next(directions.Count)];


				currentPosition = (
					currentPosition.x + dir.x,
					currentPosition.y + dir.y
				);

			}
		}

		// Fills node types based on available types and collision radius
		private void FillNodeTypes(List<TileSpawnType> typesToFill, int collisionRadius)
		{
			Console.WriteLine("Filling Node Types...");
			var random = new Random(seed);

			// Build list of empty node keys to consider once and shuffle it
			var emptyKeys = new List<(int x, int y)>();
			foreach (var kvp in Nodes)
				if (kvp.Value == TileSpawnType.Default)
					emptyKeys.Add(kvp.Key);

			// Fisher–Yates shuffle
			for (int i = 0; i < emptyKeys.Count; i++)
			{
				int j = random.Next(i, emptyKeys.Count);
				var tmp = emptyKeys[i]; emptyKeys[i] = emptyKeys[j]; emptyKeys[j] = tmp;
			}

			// Assign regular types, respecting collision radius
			for (int e = 0; e < emptyKeys.Count && typesToFill.Count > 0; e++)
			{
				var key = emptyKeys[e];

				if (IsNodesRadiusOccupied(key, collisionRadius))
					continue;

				int index = random.Next(typesToFill.Count);
				var typeToAssign = typesToFill[index];

				// if it's special, remove it from the pool so it isn't reused
				if (typeToAssign == TileSpawnType.BossSpawn || typeToAssign == TileSpawnType.Quest)
					typesToFill.RemoveAt(index);

				Nodes[key] = typeToAssign;
			}

			// --- Ensure important types are assigned ---
			var importantTypes = new List<TileSpawnType> { TileSpawnType.BossSpawn, TileSpawnType.Quest };

			// Remove already assigned important types
			foreach (var kvp in Nodes)
				if (importantTypes.Contains(kvp.Value))
					importantTypes.Remove(kvp.Value);

			if (importantTypes.Count == 0) return;

			// Try to place remaining important types: prefer empty, then any non Start/End
			var candidateKeys = new List<(int x, int y)>();
			foreach (var kvp in Nodes)
				if (kvp.Value == TileSpawnType.Default)
					candidateKeys.Add(kvp.Key);

			if (candidateKeys.Count == 0)
			{
				foreach (var kvp in Nodes)
				{
					if (kvp.Value != TileSpawnType.Start && kvp.Value != TileSpawnType.End)
						candidateKeys.Add(kvp.Key);
				}
			}

			// shuffle candidates
			for (int i = 0; i < candidateKeys.Count; i++)
			{
				int j = random.Next(i, candidateKeys.Count);
				var tmp = candidateKeys[i]; candidateKeys[i] = candidateKeys[j]; candidateKeys[j] = tmp;
			}

			foreach (var type in new List<TileSpawnType>(importantTypes))
			{
				bool placed = false;
				foreach (var key in candidateKeys)
				{
					if (IsNodesRadiusOccupied(key, collisionRadius)) continue;

					Nodes[key] = type;
					placed = true;
					break;
				}
				if (!placed)
					Console.WriteLine($"Warning: couldn't place important type {type} due to collisions.");
			}
		}

		// Updates the bounds of the map based on the current node positions
		private void UpdateBounds()
		{
			Console.WriteLine("Updating Bounds...");

			if (Nodes == null || Nodes.Count == 0)
				throw new InvalidOperationException("No nodes to update bounds.");

			bool first = true;
			(int i, int n) counter = (0, Nodes.Count);
			foreach (var kvp in Nodes)
			{
				var key = kvp.Key; // (int x, int y)
				if (first)
				{
					bounds.topLeft = key;
					bounds.bottomRight = key;
					first = false;
					continue;
				}

				if (key.x < bounds.topLeft.x) bounds.topLeft.x = key.x;
				if (key.y < bounds.topLeft.y) bounds.topLeft.y = key.y;
				if (key.x > bounds.bottomRight.x) bounds.bottomRight.x = key.x;
				if (key.y > bounds.bottomRight.y) bounds.bottomRight.y = key.y;

				counter.i++;
				if (ENABLE_DETAILED_LOGGING)
					Console.WriteLine($"Processing node at {key} ({counter.i}/{counter.n})");
			}
		}

		// === CONVERSION METHODS ===
		// Converts the nodes to a 2D map array
		private int[,] ConvertToMap(Dictionary<(int, int), TileSpawnType> Nodes, bool includeTypes = false)
		{
			if (Nodes == null || Nodes.Count == 0 || length <= 0)
				throw new InvalidOperationException("No nodes to convert to map.");

			UpdateBounds();
			int[,] map = new int[mapSize.width + padding * 2, mapSize.height + padding * 2];

			// Fill new map with Empty
			for (int x = 0; x < map.GetLength(0); x++)
				for (int y = 0; y < map.GetLength(1); y++)
					map[x, y] = (int)TileSpawnType.Empty;

			Console.WriteLine("Converting to Map...");
			(int i, int n) counter = (0, Nodes.Count);
			foreach (var point in Nodes)
			{
				counter.i++;
				if (ENABLE_DETAILED_LOGGING)
					Console.WriteLine($"Adding node at {point.Key} to map ({counter.i}/{counter.n})");

				// Compute map-local coordinates (use topLeft as origin)
				var (x, y) = point.Key;
				int mapX = x - bounds.topLeft.x + padding;
				int mapY = y - bounds.topLeft.y + padding;

				// Add thickness around node (clamp to map bounds)
				for (int dx = -thickness; dx <= thickness; dx++)
				{
					for (int dy = -thickness; dy <= thickness; dy++)
					{
						int distSq = dx * dx + dy * dy;
						if (distSq <= thickness * thickness)
						{
							int ix = mapX + dx;
							int iy = mapY + dy;
							if (ix >= 0 && ix < map.GetLength(0) && iy >= 0 && iy < map.GetLength(1))
							{
								if (map[ix, iy] == (int)TileSpawnType.Empty)
									map[ix, iy] = (int)TileSpawnType.Default;
							}
						}
					}
				}

				// Add main node (respect includeTypes)
				if (mapX >= 0 && mapX < map.GetLength(0) && mapY >= 0 && mapY < map.GetLength(1))
					map[mapX, mapY] = includeTypes ? (int)point.Value : (int)TileSpawnType.Default;
			}
			return map;
		}

		// Converts nodes to a list of positions and types, filtering by typesToInclude
		public List<((int x, int y), TileSpawnType)> ConvertNodesToList(List<TileSpawnType> typesToInclude = null)
		{
			var nodes = new List<((int x, int y), TileSpawnType)>();
			if (typesToInclude == null)
				typesToInclude = GetDefaultTypes();

			foreach (var kvp in Nodes)
			{
				if (typesToInclude.Contains(kvp.Value))
					nodes.Add((kvp.Key, kvp.Value));
			}
			return nodes;
		}

		private List<TileSpawnType> GetDefaultTypes()
		{
			return new List<TileSpawnType>
			{
				TileSpawnType.Start,
				TileSpawnType.End,
				TileSpawnType.EnemySpawn,
				TileSpawnType.Landmark,
				TileSpawnType.Treasure,
				TileSpawnType.BossSpawn,
				TileSpawnType.Quest,
			};
		}

		// === PRINTING / LOGGING METHODS ===
		public void PrintNodesToGDPrint()
		{
			GD.Print("Nodes:");
			var nodes = ConvertNodesToList();
			foreach (var node in nodes)
			{
				GD.Print($"Node at ({node.Item1.x}, {node.Item1.y}): {node.Item2}");
			}
		}

		public void PrintMapToGDPrint()
		{
			GD.Print("Map Layout:");
			for (int y = 0; y < IntMap2D.GetLength(1); y++)
			{
				string line = "";
				for (int x = 0; x < IntMap2D.GetLength(0); x++)
				{
					var val = (IntMap2D[x, y] >= 0) ? IntMap2D[x, y].ToString() : ".";
					line += val + " ";
				}
				GD.Print(line);
			}
		}

		// === UTILITY METHODS ===

		// Checks if any nodes within a radius are occupied (not None)
		private bool IsNodesRadiusOccupied((int x, int y) position, int radius, bool includeTypeNone = false)
		{
			// Probe only the local neighborhood -> O((2r+1)^2) work instead of O(N)
			for (int dx = -radius; dx <= radius; dx++)
			{
				for (int dy = -radius; dy <= radius; dy++)
				{
					var p = (position.x + dx, position.y + dy);
					if (Nodes.TryGetValue(p, out var node))
					{
						if (!includeTypeNone && node == TileSpawnType.Default)
							continue;
						return true;
					}
				}
			}
			return false;
		}

		// === ENUMS ===
		public enum TileSpawnType
		{
			Empty = -1,
			Default = 0,
			Start = 1,
			End = 2,
			Treasure = 3,
			EnemySpawn = 4,
			Landmark = 5,
			BossSpawn = 6,
			Quest = 7,
		}
	}
}
