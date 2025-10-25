using Godot;
using DungeonGenerator;
using System.Collections.Generic;
using System.Linq;

public partial class DungeonNode : Node
{
	TileMapLayer tilemapFloor;
	Godot.Collections.Dictionary<Vector2I, MapConstructor.TileSpawnType> tilemapType;

	// Convert Godot Dictionary to List<(Vector2I, MapConstructor.TileSpawnType)>
	List<(Vector2I, MapConstructor.TileSpawnType)> tilemapTypeList;
	public override void _Ready()
	{
		// Fetch the TileMapLayer node
		tilemapFloor = GetNode<TileMapLayer>("TileMapFloor");

		// Generate the grid using MapConstructor and convert to Godot types
		var map = new MapConstructor(1_000, 1, 2, 1, 1, false, false, 4, true);
		(tilemapFloor, tilemapType) = map.ConvertToGodot();
		map.ConvertToImage("test_image.png");

		// Convert Dictionary to List for easier access later
		tilemapTypeList = [.. tilemapType.Select(kv => (kv.Key, kv.Value))];

		GD.Print("[DungeonNode] Dungeon generated successfully.");
		GD.Print("[DungeonNode] print tostring\n" + this.ToString());
		
	}

	public override string ToString()
	{
		var Return = $"{base.ToString()}";
		foreach (var (position, type) in tilemapTypeList)
		{
			Return += $"\nTile at {position} is of type {type}";
		}
		return Return;
	}
}
