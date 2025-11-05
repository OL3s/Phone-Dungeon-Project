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
- Create contract pointer at last wave to select which gem (which is close to current biome)
	- Make fitting icons for each gem to be next to contract pointer

- REWORK the dungeon generator, have the whole mapgenerator generate the entire map, including nodes, then have the DungeonNode.cs just read from that data and fill in the TileMapLayer and ObjectGroups. This will make it way easier to manage the data flow.

# Roadmap
1. Create dungeon generator v2.
2. Create AStar pathfinding system.
3. Create working mobs with basic AI.
4. Create working player with touch controls.
5. Create basic combat system.
6. Create basic item system with equippable items.
7. Create basic UI for inventory, health, etc.
8. Create basic sound effects and music.
9. Polish and optimize the game for mobile platforms.