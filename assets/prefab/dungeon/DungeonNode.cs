using Godot;
using MapGeneratorCs;
using System;
using System.Collections.Generic;
using static MapGeneratorCs.MapConstructor;

public partial class DungeonNode : Node
{
	[Export] public bool GenerateStartNode = true;
	TileMapLayer tilemapFloor;
	List<((int x, int y), TileSpawnType)> tileMapNodes = new List<((int x, int y), TileSpawnType)>();

	MapConstructor mapConstructor = new MapConstructor(
		length: 3_000,
		thickness: 2,
		collisionRadius: 5,
		spawnFactors: (2, 1, 1, true, false),
		enableDetailedLogging: false,
		enableTypesInMap: false
	);

	public override void _Ready()
	{
		tilemapFloor = GetNode<TileMapLayer>("TileMapFloor");

		if (tilemapFloor == null)
			throw new System.Exception("TileMapFloor node not found!");

		ApplyIntMapToTileMapLayer();
		GenerateNodesToObjectGroups();
	}

	private void ApplyIntMapToTileMapLayer()
	{
		GD.Print("[DungeonNode] Converting int map to TileMapLayer...");
		var rawMap = mapConstructor.IntMap2D;
		var tileSet = tilemapFloor.TileSet;

		if (tileSet == null)
		{
			GD.PushError("[DungeonNode] TileSet not assigned to TileMapLayer!");
			return;
		}

		// use first atlas source (the texture you added)
		int sourceId = tileSet.GetSourceId(0);

		for (int x = 0; x < rawMap.GetLength(0); x++)
		{
			for (int y = 0; y < rawMap.GetLength(1); y++)
			{
				int tileType = rawMap[x, y];
				var atlasCoords = new Vector2I(0, 0);
				var atlasType = tileType == 0 ? AtlasType.None : AtlasType.Default;

				// skip empty tiles
				if (tileType < 0)
					continue;


				if (tileType == 0 && y < rawMap.GetLength(1) - 1 && rawMap[x, y + 1] != 0)
					atlasType = AtlasType.TopWall;

				if (tileType > 0 && y != 0 && rawMap[x, y - 1] == 0)
					atlasType = AtlasType.TopTile;

				if (tileType > 0 && (Random.Shared.Next(0, 2) == 0) && CountNeighbourOfIndex(x, y, 6, 0) < 6)
					atlasType = AtlasType.Centered;

				// atlas coords: (tileType, 0)
				tilemapFloor.SetCell(
					new Vector2I(x, y),      // cell position
					sourceId,                // atlas source
					GetAtlasTileCoords(atlasType) // atlas coordinates
				);
			}
		}
	}

	private void PrintTileMapLayerToGDPrint()
	{
		GD.Print("[DungeonNode] Printing TileMapLayer:");
		var usedCells = tilemapFloor.GetUsedCells();
		if (usedCells == null || usedCells.Count == 0)
		{
			GD.Print("[DungeonNode] No used cells found on tilemapFloor.");
			return;
		}

		foreach (var cell in usedCells)
		{
			int tileType = tilemapFloor.GetCellSourceId(cell);
			GD.Print($"[DungeonNode] Cell at {cell} has tile type {tileType}");
		}
	}

	private void GenerateNodesToObjectGroups()
	{
		GD.Print("[DungeonNode] Generating nodes to object groups...");
		foreach (var kvp in mapConstructor.Nodes)
		{
			var position = kvp.Key;
			var type = kvp.Value;

			switch (type)
			{
				case TileSpawnType.Start:
					if (GenerateStartNode)
						GenerateStartNodeObject(new Vector2I(position.x, position.y));
					break;
				// Add more cases for different TileSpawnTypes as needed
				default:
					break;
			}
		}
	}

	private void GenerateStartNodeObject(Vector2I location)
	{
		var tileSize = 16;
		location.X *= tileSize;
		location.Y *= tileSize;
		GD.Print("[DungeonNode] Generating start node object at " + location);

		// load resource
		var res = GD.Load<PackedScene>("res://assets/prefab/player/player-default.tscn");
		if (res == null)
			throw new System.Exception("Failed to load player-default.tscn!");
		var instance = res.Instantiate<CharacterBody2D>();

		// get main
		var main = GetTree().Root.GetNodeOrNull("Main");
		if (main == null)
		{
			GD.PushError("[DungeonNode] 'Main' node not found in root.");
			return;
		}

		// defer the add_child call to avoid "Parent node is busy setting up children" error
		main.CallDeferred("add_child", instance);

		// set position
		instance.Position = new Vector2(location.X, location.Y);
	}

	private Vector2I GetAtlasTileCoords(AtlasType atlasType)
	{
		return atlasType switch
		{
			AtlasType.None => new Vector2I(0, 0),
			AtlasType.Default => new Vector2I(Random.Shared.Next(1, 4), 0),
			AtlasType.TopWall => new Vector2I(0, 1),
			AtlasType.TopTile => new Vector2I(Random.Shared.Next(1, 4), 1),
			AtlasType.Centered => new Vector2I(Random.Shared.Next(4, 7), 0),
			_ => new Vector2I(0, 0),
		};
	}

	private int CountNeighbourOfIndex(int x, int y, int radius, int targetIndex)
	{
		var rawMap = mapConstructor.IntMap2D;
		var count = 0;
		for (int offsetX = -radius; offsetX <= radius; offsetX++)
			for (int offsetY = -radius; offsetY <= radius; offsetY++)
			{
				if (offsetX == 0 && offsetY == 0)
					continue;
				int checkX = x + offsetX;
				int checkY = y + offsetY;
				if (checkX >= 0 && checkX < rawMap.GetLength(0) &&
					checkY >= 0 && checkY < rawMap.GetLength(1))
				{
					if (rawMap[checkX, checkY] == targetIndex)
						count++;
				}
			}
		return count;
	}

	private enum AtlasType
	{
		Default,
		None,
		TopWall,
		TopTile,
		Centered
	}
}
