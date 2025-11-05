using Godot;
using MapGeneratorCs;
using System;
using System.Collections.Generic;
using static MapGeneratorCs.MapConstructor;

public partial class DungeonNode : Node
{
	TileMapLayer tilemapFloor; // TileMapLayer for floor tiles
	MapConstructor mapConstructor; // MapConstructor instance

	public override void _Ready()
	{
		mapConstructor = new MapConstructor(
			length: 3_000,
			thickness: 2,
			collisionRadius: 5,
			spawnFactors: (2, 1, 1, true, false),
			enableDetailedLogging: false,
			enableTypesInMap: true
		);

		// Get TileMapLayer node
		tilemapFloor = GetNode<TileMapLayer>("TileMapFloor");
		if (tilemapFloor == null)
			throw new Exception("TileMapFloor node not found!");

		// Convert mapConstructor int[,] to GodotEngine
		ConvertIntMap2DToAtlas();
	}

	/// <summary>
	/// Converts the integer map int[,] from MapConstructor to the TileMapLayer using atlas tiles.
	/// </summary>
	private void ConvertIntMap2DToAtlas()
	{
		GD.Print("[DungeonNode] Converting int map to TileMapLayer...");
		var rawMap = mapConstructor.IntMap2D;
		var tileSet = tilemapFloor.TileSet;

		if (tileSet == null)
		{
			GD.PushError("[DungeonNode] TileSet not assigned to TileMapLayer!");
			return;
		}

		// Add atlas (background) source to tileset based on int[,] data
		int sourceId = tileSet.GetSourceId(0);

		for (int x = 0; x < rawMap.GetLength(0); x++)
		{
			for (int y = 0; y < rawMap.GetLength(1); y++)
			{
				// get tile type
				int tileRawType = rawMap[x, y];

				// init default atlas type
				var atlasType = (tileRawType <= 0) ? AtlasType.None : AtlasType.Default;

				if (tileRawType == 0 && y < rawMap.GetLength(1) - 1 && rawMap[x, y + 1] != 0)
					atlasType = AtlasType.TopWall;

				if (tileRawType > 0 && y != 0 && rawMap[x, y - 1] == 0)
					atlasType = AtlasType.TopTile;

				if (tileRawType > 0 && (Random.Shared.Next(0, 2) == 0) && CountNeighbourInIntMap2D(x, y, 6, 0) < 6)
					atlasType = AtlasType.Centered;

				// atlas coords: (tileType, 0)
				tilemapFloor.SetCell(
					new Vector2I(x, y),      // cell position
					sourceId,                // atlas source
					GetAtlasTileCoordsFromType(atlasType) // atlas coordinates
				);
			}
		}
	}

	/// <summary>
	/// Generates objects based on the integer map.
	/// </summary>
	private void GenerateIntMap2DToObjects()
	{
		// TODO: implement object generation based on int map
		throw new NotImplementedException();
	}
	
	/// <summary>
	/// Gets the atlas tile coordinates based on the specified atlas type.
	/// </summary>
	/// <param name="atlasType"></param>
	/// <returns></returns>
	private Vector2I GetAtlasTileCoordsFromType(AtlasType atlasType)
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

	/// <summary>
	/// Counts the number of neighboring tiles of a specific index around a given position.
	/// </summary>
	/// <param name="x">x</param>
	/// <param name="y">y</param>
	/// <param name="radius">radius</param>
	/// <param name="targetIndex">The index of the tile to count neighbors for. -1 means count all indexes except '0'.</param>
	/// <param name="isCircular">Whether to count neighbors in a circular or square manner.</param>
	/// <returns></returns>
	private int CountNeighbourInIntMap2D(int x, int y, int radius, int targetIndex, bool isCircular = true)
	{
		var rawMap = mapConstructor.IntMap2D;
		var count = 0;
		int r2 = radius * radius;

		for (int offsetX = -radius; offsetX <= radius; offsetX++)
		{
			for (int offsetY = -radius; offsetY <= radius; offsetY++)
			{
				if (offsetX == 0 && offsetY == 0)
					continue;

				// circular check when requested
				if (isCircular)
				{
					int dx = offsetX;
					int dy = offsetY;
					if (dx * dx + dy * dy > r2)
						continue;
				}

				int checkX = x + offsetX;
				int checkY = y + offsetY;
				if (checkX >= 0 && checkX < rawMap.GetLength(0) &&
					checkY >= 0 && checkY < rawMap.GetLength(1))
				{
					if (targetIndex == -1 ? rawMap[checkX, checkY] != 0 : rawMap[checkX, checkY] == targetIndex)
						count++;
				}
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
