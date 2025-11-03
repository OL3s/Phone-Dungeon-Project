# TODO
- Create accept dialog `NoItemPopup.cs` file for starting without anything equipped.
- Create a working player.
	- Fix _Input() for touch handling in `PlayerInput.cs`
	- Fix xscale based on aimDirection
- Create a working mob.
- Create a working dungeon.
	- Implement DungeonGenerator.cs to an object
	- Make tile background for layers
	- implement tiletype code for init() spawns
	- implement DungeonNode.cs : Node
	- !! Fix the filling of TileMapLayer being empty on generation function from int[,] -> TileTypeLayer[Vector2I]

- REWORK the dungeon generator, have the whole mapgenerator generate the entire map, including nodes, then have the DungeonNode.cs just read from that data and fill in the TileMapLayer and ObjectGroups. This will make it way easier to manage the data flow.