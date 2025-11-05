using Godot;
using System;
using MapGeneratorCs;       

public partial class PathFinding : Node
{
    public int[,] gridRaw;
    public AStar2D astar = new AStar2D();

    private int[] GetWalkableTypes()
    {
        return new int[]
        {
            (int)MapConstructor.TileSpawnType.Default,
            (int)MapConstructor.TileSpawnType.Start,
            (int)MapConstructor.TileSpawnType.End,
            (int)MapConstructor.TileSpawnType.Enemy,
        };
    }

    public override void _Ready()
    {

    }

    public void GenerateAStarGrid()
    {
        int[] walkableTypes = GetWalkableTypes();

        // First, add all points
        for (int y = 0; y < gridRaw.GetLength(0); y++)
        {
            for (int x = 0; x < gridRaw.GetLength(1); x++)
            {
                int id = y * gridRaw.GetLength(1) + x;
                astar.AddPoint(id, new Vector2(x, y));  
                
                // Set point as disabled if it's not a walkable type
                if (!Array.Exists(walkableTypes, type => type == gridRaw[y, x]))
                {
                    astar.SetPointDisabled(id, true);
                }
            }
        }

        // Then, connect neighboring points
        for (int y = 0; y < gridRaw.GetLength(0); y++)
        {
            for (int x = 0; x < gridRaw.GetLength(1); x++)
            {
                int id = y * gridRaw.GetLength(1) + x;

                // Connect to neighbors
                Vector2[] neighbors = 
                {
                    // Cardinal directions
                    new Vector2(x + 1, y),      // Right
                    new Vector2(x - 1, y),      // Left
                    new Vector2(x, y + 1),      // Down
                    new Vector2(x, y - 1),      // Up
                    // Diagonal directions
                    new Vector2(x + 1, y + 1),  // Down-Right
                    new Vector2(x + 1, y - 1),  // Up-Right
                    new Vector2(x - 1, y + 1),  // Down-Left
                    new Vector2(x - 1, y - 1)   // Up-Left
                };

                foreach (var neighbor in neighbors)
                {
                    int neighborX = (int)neighbor.X;
                    int neighborY = (int)neighbor.Y;
                    
                    // Check bounds
                    if (neighborX >= 0 && neighborX < gridRaw.GetLength(1) &&   
                        neighborY >= 0 && neighborY < gridRaw.GetLength(0))
                    {
                        // For diagonal neighbors, check if both adjacent cardinal tiles are walkable
                        if (Math.Abs(neighbor.X - x) == 1 && Math.Abs(neighbor.Y - y) == 1)
                        {
                            // This is a diagonal neighbor - check corner cutting
                            bool horizontalWalkable = Array.Exists(walkableTypes, type => type == gridRaw[y, neighborX]);
                            bool verticalWalkable = Array.Exists(walkableTypes, type => type == gridRaw[neighborY, x]);
                            
                            if (!horizontalWalkable || !verticalWalkable)
                                continue; // Skip this diagonal connection to prevent corner cutting
                        }
                        
                        int neighborId = neighborY * gridRaw.GetLength(1) + neighborX;
                        if (astar.HasPoint(neighborId))
                        {
                            astar.ConnectPoints(id, neighborId);
                        }
                    }
                }
            }
        }
    }

    public void SetGrid(int[,] grid)
    {
        gridRaw = grid;
        astar.Clear();
        GenerateAStarGrid();
    }

    public Vector2[] GetPath(Vector2 from, Vector2 to)
    {
        if (gridRaw == null) return new Vector2[0];
        
        int fromId = GetPointId(from);
        int toId = GetPointId(to);
        
        if (fromId == -1 || toId == -1) return new Vector2[0];
        
        var path = astar.GetPointPath(fromId, toId);
        return path;
    }

    private int GetPointId(Vector2 position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        
        if (x < 0 || x >= gridRaw.GetLength(1) || y < 0 || y >= gridRaw.GetLength(0))
            return -1;
            
        return y * gridRaw.GetLength(1) + x;
    }

    public bool IsWalkable(Vector2 position)
    {
        int[] walkableTypes = GetWalkableTypes();
        int x = (int)position.X;
        int y = (int)position.Y;
        
        if (x < 0 || x >= gridRaw.GetLength(1) || y < 0 || y >= gridRaw.GetLength(0))
            return false;
            
        return Array.Exists(walkableTypes, type => type == gridRaw[y, x]);
    }
}
