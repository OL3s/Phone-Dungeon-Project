using Godot;
using System;

public partial class DungeonNode : Node
{
	[Export] public TileMapLayer tileFloor;
	[Export] public Node TileTypeDict;
}
