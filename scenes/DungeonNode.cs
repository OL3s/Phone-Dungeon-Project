using Godot;
using DungeonGenerator;
using System.Collections.Generic;
using System.Linq;

public partial class DungeonNode : Node
{
	TileMapLayer tilemapFloor;
	Godot.Collections.Dictionary<Vector2I, MapConstructor.TileSpawnType> tilemapType;

	// Convert Godot Dictionary to List<(Vector2I, MapConstructor.TileSpawnType)>
	List<(Vector2I, MapConstructor.TileSpawnType)> tilemapTypeList =>
		new List<(Vector2I, MapConstructor.TileSpawnType)>(
			tilemapType.Select(kv => (kv.Key, kv.Value))
		);
	public override void _Ready()
	{
		tilemapFloor = GetNode<TileMapLayer>("TileMapFloor");
		(tilemapFloor, tilemapType) = new MapConstructor(1_000, 1, 2, 1, 1, false, false, 4, true).ConvertToGodot();
		foreach (var (position, type) in tilemapTypeList)
		{
			tilemapFloor.SetCell(position, (int)type > 0 ? 1 : -1);
		}
		GD.Print("[DungeonNode] Dungeon generated successfully.");
	}
}
