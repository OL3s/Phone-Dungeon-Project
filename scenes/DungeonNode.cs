using Godot;
using MapGeneratorCs;
using System.Collections.Generic;
using static MapGeneratorCs.MapConstructor;

public partial class DungeonNode : Node
{
	TileMapLayer tilemapFloor;
	List<((int x, int y), TileSpawnType)> tileMapNodes = new List<((int x, int y), TileSpawnType)>();

	MapConstructor mapConstructor = new MapConstructor(1_000, 1, 5, (2, 1, 1, true, false), false);

	public override void _Ready()
	{
		tilemapFloor = GetNode<TileMapLayer>("TileMapFloor");

		if (tilemapFloor == null)
			throw new System.Exception("TileMapFloor node not found!");

		ConvertIntMapToTileMapLayer();
		tileMapNodes = mapConstructor.ConvertNodesToList();
		mapConstructor.PrintMapToGDPrint();
		mapConstructor.PrintNodesToGDPrint();
		PrintTileMapLayerToGDPrint();
	}

	private void ConvertIntMapToTileMapLayer()
	{
		GD.Print("[DungeonNode] Converting int map to TileMapLayer...");
		var rawMap = mapConstructor.IntMap2D;

		// Write directly to the scene's TileMapLayer node
		for (int x = 0; x < rawMap.GetLength(0); x++)
		{
			for (int y = 0; y < rawMap.GetLength(1); y++)
			{
				int tileType = rawMap[x, y];

				// Only set cells for valid tile indices (negative = empty)
				if (tileType < 0)
					continue;

				// Use SetCell so the TileMap registers the cell as "used"
				tilemapFloor.SetCell(new Vector2I(x, y), tileType);
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
			GD.Print($"Cell ({cell.X}, {cell.Y}): Tile Type {tileType}");
		}
	}
}
