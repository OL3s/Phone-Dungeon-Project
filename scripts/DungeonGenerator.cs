using Godot;
using System;
using System.Collections.Generic;

/*
	HOW TO USE IN A _START() EVENT
	var DungeonData = new DungeonGenerator.MapConstructor(
		length: 50,
		thickness: 2,
		enemyFactor: 5,
		landmarkFactor: 3,
		treasureFactor: 4,
		isBoss: true,
		isQuest: true
	).ConvertToGodot();
*/

namespace DungeonGenerator
{
	class MapConstructor
	{
		// Internal state
		private static bool ENABLE_DETAILED_LOGGING = false;
		private ((int x, int y) topLeft, (int x, int y) bottomRight) bounds = ((0, 0), (0, 0));
		private (int width, int height) mapSize => (bounds.bottomRight.x - bounds.topLeft.x + 1,
												  bounds.bottomRight.y - bounds.topLeft.y + 1);
		private (int x, int y) currentPosition;
		private int seed;
		private int length;
		private int thickness;
		private int padding;
		public Dictionary<(int x, int y), Node> Nodes
			= new Dictionary<(int x, int y), Node>();

		// Constructor with initialization
		public MapConstructor(
			int length, int thickness,
			int enemyFactor, int landmarkFactor, int treasureFactor,
			bool isBoss, bool isQuest)
		{
			// Initialize parameters
			this.length = length;
			this.seed = Random.Shared.Next();
			this.thickness = thickness;
			this.padding = thickness + 1;

			// Add spawn types based on factors
			var spawns = new List<TileSpawnType>();
			for (int i = 0; i < enemyFactor; i++) spawns.Add(TileSpawnType.EnemySpawn);
			for (int i = 0; i < landmarkFactor; i++) spawns.Add(TileSpawnType.Landmark);
			for (int i = 0; i < treasureFactor; i++) spawns.Add(TileSpawnType.Treasure);
			if (isBoss) spawns.Add(TileSpawnType.BossSpawn);
			if (isQuest) spawns.Add(TileSpawnType.Quest);

			// Generate the map
			GenerateDefaultNodes();
			FillNodeTypes(spawns, 4);
		}

		// Generates nodes in a random walk fashion (start, end and none types)
		private void GenerateDefaultNodes()
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
					if (isStart) Nodes[currentPosition] = new Node { position = currentPosition, spawnType = TileSpawnType.Start };
					else if (isEnd) Nodes[currentPosition] = new Node { position = currentPosition, spawnType = TileSpawnType.End };
					else Nodes[currentPosition] = new Node { position = currentPosition, spawnType = TileSpawnType.Road };

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

		// Updates the bounds of the map based on the current node positions
		private void UpdateBounds()
		{
			Console.WriteLine("Updating Bounds...");

			if (Nodes.Count == 0 || Nodes == null)
				throw new InvalidOperationException("No nodes to update bounds.");

			(int i, int n) counter = (0, Nodes.Count);
			foreach (var node in Nodes.Values)
			{
				counter.i++;
				if (ENABLE_DETAILED_LOGGING)
					Console.WriteLine($"Updating bounds with node at {node.position} ({counter.i}/{counter.n})");
				if (node.position.x < bounds.topLeft.x) bounds.topLeft.x = node.position.x;
				if (node.position.y < bounds.topLeft.y) bounds.topLeft.y = node.position.y;
				if (node.position.x > bounds.bottomRight.x) bounds.bottomRight.x = node.position.x;
				if (node.position.y > bounds.bottomRight.y) bounds.bottomRight.y = node.position.y;
			}
		}

		// Converts the nodes to a 2D map array
		private int[,] ConvertToMap(Dictionary<(int, int), Node> Nodes)
		{
			if (Nodes.Count == 0 || length <= 0 || Nodes == null)
				throw new InvalidOperationException("No nodes to convert to map.");

			UpdateBounds();
			int[,] map = new int[mapSize.width + padding * 2, mapSize.height + padding * 2];
			Console.WriteLine("Converting to Map...");
			(int i, int n) counter = (0, Nodes.Count);
			foreach (var point in Nodes)
			{
				counter.i++;
				if (ENABLE_DETAILED_LOGGING)
					Console.WriteLine($"Adding node at {point.Key} to map ({counter.i}/{counter.n})");
				// Add thickness around node
				var (x, y) = point.Key;
				for (int dx = -thickness; dx <= thickness; dx++)
				{
					for (int dy = -thickness; dy <= thickness; dy++)
					{
						int distSq = dx * dx + dy * dy;
						if (distSq <= thickness * thickness)
						{
							if (map[bounds.bottomRight.x - x + padding + dx, bounds.bottomRight.y - y + padding + dy] == (int)TileSpawnType.Empty)
								map[bounds.bottomRight.x - x + padding + dx, bounds.bottomRight.y - y + padding + dy] = (int)TileSpawnType.Default;
						}
					}
				}

				// Add main node
				map[bounds.bottomRight.x - x + padding, bounds.bottomRight.y - y + padding] = (int)point.Value.spawnType;
			}

			return map;
		}


		// === IMAGE CREATION, FOR DEBUGGING DUNGEON VISUALIZATION ONLY ===
		public void ConvertToImage(string filePath)
		{
			int[,] map = ConvertToMap(Nodes);

			Console.WriteLine("Converting to Image...");
			int width = map.GetLength(0);
			int height = map.GetLength(1);

			var image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					// map[x,y] -> color
					image.SetPixel(x, y, GetColorForTileType(map[x, y]));
				}
			}

			var err = image.SavePng(filePath);
			if (err != Error.Ok)
				Console.WriteLine($"Failed to save image: {err}");
		}

		private Color GetColorForTileType(int t)
		{
			switch ((TileSpawnType)t)
			{
				case TileSpawnType.Empty: return new Color(0f, 0f, 0f);        // black
				case TileSpawnType.Start: return new Color(0f, 1f, 0f);        // green
				case TileSpawnType.End: return new Color(1f, 0f, 0f);        // red
				case TileSpawnType.Treasure: return new Color(1f, 0.84f, 0f);     // gold
				case TileSpawnType.EnemySpawn: return new Color(1f, 0f, 1f);       // magenta
				case TileSpawnType.Landmark: return new Color(0f, 0f, 1f);        // blue
				case TileSpawnType.BossSpawn: return new Color(0.5f, 0f, 0.5f);    // purple
				case TileSpawnType.Quest: return new Color(0f, 1f, 1f);        // cyan
				case TileSpawnType.Default: return new Color(0.8f, 0.8f, 0.8f);  // light gray
				default: return new Color(1f, 0f, 1f);        // magenta for debug
			}
		}
		// === END IMAGE CREATION ===


		// Convert tile dictionary to Godot TileMap
		public (TileMapLayer tilemapFloor, Godot.Collections.Dictionary<Vector2I, TileSpawnType> tilemapType) ConvertToGodot(int[,] map = null)
		{
			if (map == null)
				map = ConvertToMap(Nodes);

			TileMapLayer tilemapFloor = new TileMapLayer();
			Godot.Collections.Dictionary<Vector2I, TileSpawnType> tilemapType = new Godot.Collections.Dictionary<Vector2I, TileSpawnType>();

			for (int x = 0; x < map.GetLength(0); x++)
			{
				for (int y = 0; y < map.GetLength(1); y++)
				{
					// Here you would set the cell value in the TileMapLayer
					tilemapFloor.SetCell(new Vector2I(x, y), (int)TileSpawnType.Default); // Example: set floor type
					tilemapType[new Vector2I(x, y)] = (TileSpawnType)map[x, y];
				}
			}

			return (tilemapFloor, tilemapType);
		}

		// Gets all nodes within a certain radius of a position
		private Dictionary<(int x, int y), Node> GetNodesInRadius((int x, int y) center, int radius)
		{
			var nodesInRadius = new Dictionary<(int x, int y), Node>();

			// Instead of scanning all Nodes, probe the neighborhood using TryGetValue.
			for (int dx = -radius; dx <= radius; dx++)
			{
				for (int dy = -radius; dy <= radius; dy++)
				{
					var p = (center.x + dx, center.y + dy);
					if (Nodes.TryGetValue(p, out var node))
					{
						nodesInRadius.Add(p, node);
					}
				}
			}

			return nodesInRadius;
		}

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
						if (!includeTypeNone && node.spawnType == TileSpawnType.Default)
							continue;
						return true;
					}
				}
			}
			return false;
		}

		// Fills node types based on available types and collision radius
		private void FillNodeTypes(List<TileSpawnType> typesToFill, int collisionRadius)
		{
			Console.WriteLine("Filling Node Types...");
			var random = new Random(seed);

			var keysToUpdate = new List<(int x, int y)>();

			// Build list of empty node keys to consider once and shuffle it
			var emptyKeys = new List<(int x, int y)>();
			foreach (var kvp in Nodes)
				if (kvp.Value.spawnType == TileSpawnType.Default)
					emptyKeys.Add(kvp.Key);

			// shuffle
			for (int i = 0; i < emptyKeys.Count; i++)
			{
				int j = random.Next(i, emptyKeys.Count);
				var tmp = emptyKeys[i]; emptyKeys[i] = emptyKeys[j]; emptyKeys[j] = tmp;
			}

			// Assign regular types, respecting collision radius
			foreach (var key in emptyKeys)
			{
				if (typesToFill.Count == 0) break;
				var node = Nodes[key];

				if (IsNodesRadiusOccupied(node.position, collisionRadius))
					continue;

				// pick a random type
				int index = random.Next(typesToFill.Count);
				TileSpawnType typeToAssign = typesToFill[index];

				// if it's special, remove it from the pool (we still place it now)
				if (typeToAssign == TileSpawnType.BossSpawn || typeToAssign == TileSpawnType.Quest)
					typesToFill.RemoveAt(index);

				keysToUpdate.Add(key);
				Nodes[key] = new Node { position = node.position, spawnType = typeToAssign };
			}

			// --- Ensure important types are assigned ---
			var importantTypes = new List<TileSpawnType> { TileSpawnType.BossSpawn, TileSpawnType.Quest };

			// Remove already assigned important types
			foreach (var kvp in Nodes)
				if (importantTypes.Contains(kvp.Value.spawnType))
					importantTypes.Remove(kvp.Value.spawnType);

			if (importantTypes.Count == 0) return;

			// Try to place remaining important types: prefer empty, then any non Start/End
			var candidateKeys = new List<(int x, int y)>();
			foreach (var kvp in Nodes)
			{
				if (kvp.Value.spawnType == TileSpawnType.Default)
					candidateKeys.Add(kvp.Key);
			}
			if (candidateKeys.Count == 0)
			{
				foreach (var kvp in Nodes)
				{
					if (kvp.Value.spawnType != TileSpawnType.Start && kvp.Value.spawnType != TileSpawnType.End)
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
					var node = Nodes[key];
					if (IsNodesRadiusOccupied(node.position, collisionRadius)) continue;

					Nodes[key] = new Node { position = node.position, spawnType = type };
					placed = true;
					break;
				}
				if (!placed)
					Console.WriteLine($"Warning: couldn't place important type {type} due to collisions.");
			}
		}

		public class PrefabGenerator
		{

		}

		public enum TileSpawnType
		{
			Empty,
			Start,
			End,
			Treasure,
			EnemySpawn,
			Landmark,
			BossSpawn,
			Quest,
			Road,
			Default
		}

		enum TerrainType
		{
			Wall,
			Path,
			Floor,
			Water,
			Nature,
			Lava,
			Bridge,
		}
		public struct Node
		{
			public (int x, int y) position;
			public TileSpawnType spawnType;
		}
	}
}
