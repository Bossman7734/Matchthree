using System;
using System.Collections.Generic;
using Components;
using Extensions.System;
using UnityEngine;

public static class GridF
{
    private const int MatchOffset = 2;

    public static void GetSpawnableColors(this Tile[,] grid, Vector2Int coord, List<int> results)
    {
            
        int lastPrefabID = -1;
        int lastIDCounter = 0;

        int leftMax = coord.x - MatchOffset;
        int rightMax = coord.x + MatchOffset;

        leftMax = ClampInsideGrid(leftMax, grid.GetLength(0));
        rightMax = ClampInsideGrid(rightMax, grid.GetLength(0));
             
             
             
        for (int x = leftMax ; x <= rightMax ; x++)
        {
            Tile currTile = grid[x, coord.y];
               
            if(currTile== null)
            {
                lastIDCounter = 0;
                lastPrefabID = -1;
                continue;
            }

            if (lastPrefabID == -1)
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1; 
            }

            else if (lastPrefabID == currTile.ID) lastIDCounter++;
            else
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1;
            }

            if (lastIDCounter == MatchOffset) results.Remove(lastPrefabID);
        }
            
        lastPrefabID = -1;  
        lastIDCounter = 0;
            
        int botMax = coord.y - MatchOffset;
        int topMax = coord.y + MatchOffset;
            

        botMax = ClampInsideGrid(botMax, grid.GetLength(1));
        topMax = ClampInsideGrid(topMax, grid.GetLength(1));

        for (int y = botMax; y <= topMax; y++)
        {
            Tile currTile = grid[coord.x, y];

            if (currTile== null)
            {
                lastIDCounter = 0;
                lastPrefabID = -1;
                continue;
            }
            if (lastPrefabID == -1)
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1;
            }
            else if (lastPrefabID == currTile.ID)
            {
                lastIDCounter++;
            }
            else
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1;
            }
            if (lastIDCounter == MatchOffset) results.Remove(lastPrefabID);  
        }
    }
        
    public static List<Tile> GetMatchesX
        (this Tile[,] thisGrid, Tile tile)
        => GetMatchesX(thisGrid,tile.Coords, tile.ID);

    public static List<Tile> GetMatchesX(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            Tile currTile = grid[x, coord.y];

            if (currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if (matches.Contains(thisTile))
            {
                break;
            }
        }
        if (matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    }

    public static List<Tile> GetMatchesY
        (this Tile[,] thisGrid, Tile tile)
        => GetMatchesY(thisGrid,tile.Coords, tile.ID);

    public static List<Tile> GetMatchesY(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        for (int y = 0; y < grid.GetLength(1); y++)
        {
            Tile currTile = grid[coord.x, y];

            if (currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if (matches.Contains(thisTile))
            {
                break;
            }
        }

        if ( matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    } 
        
    private static int ClampInsideGrid(int value, int gridsize)
    {
        return Mathf.Clamp(value, 0, gridsize - 1);
    }

    private static bool IsInsideGrid(this Tile[,] grid, int axis, int axisIndex)
    {
        int min = 0;
        int max = grid.GetLength(axisIndex);

        return axis >= 0 && axis < max;
    }

    public static bool IsInsideGrid(this Tile[,] grid, Vector2Int coord)
    {
        return grid.IsInsideGrid(coord.x, 0) && grid.IsInsideGrid(coord.y, 1);
    }

    public static GridDir GetGridDir(Vector3 input)
    {
        int maxAxis = 0;
        float maxAxisSign = input[0].Sign();
        float lastAxisLengthAbs = input[0].Abs();
            
        for (int axisIndex = 0; axisIndex < 3; axisIndex++)
        {
            float thisAxisLength = input[axisIndex];
            float thisAxisLengthAbs = thisAxisLength.Abs();
                

            if (thisAxisLengthAbs > lastAxisLengthAbs)
            {
                lastAxisLengthAbs = thisAxisLengthAbs;
                maxAxis = axisIndex;
                maxAxisSign = thisAxisLength.Sign();
            }
        }

        return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt());
    }
    public static Vector2Int GetGridDirVector(Vector3 input)
    {
        int maxAxis = 0;
        float maxAxisSign = input[0].Sign();
        float lastAxisLengthAbs = input[0].Abs();
            
        for (int axisIndex = 0; axisIndex < 3; axisIndex++)
        {
            float thisAxisLength = input[axisIndex];
            float thisAxisLengthAbs = thisAxisLength.Abs();
                

            if (thisAxisLengthAbs > lastAxisLengthAbs)
            {
                lastAxisLengthAbs = thisAxisLengthAbs;
                maxAxis = axisIndex;
                maxAxisSign = thisAxisLength.Sign();
            }
        }

        return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt()).ToVector();
    }
        
    /// <summary>
    /// Convert non zero axis indexwith Sign.
    /// </summary>
    /// <param name="axisSignIndex">Should not start from zero.</param>
    /// <returns>Grid Dir</returns>

    public static GridDir GetGridDir(int axisSignIndex)
    {
        return axisSignIndex switch
        {
            1 => GridDir.Right,
            2 => GridDir.Up,
            -1 => GridDir.left,
            -2 => GridDir.Down,
            _ => GridDir.Null
        };
    }

    public static Vector2Int ToVector(this GridDir thisGridDir)
    {
        return thisGridDir switch
        {
            GridDir.Null => Vector2Int.zero,
            GridDir.left => Vector2Int.left,
            GridDir.Right => Vector2Int.right,
            GridDir.Up => Vector2Int.up,
            GridDir.Down => Vector2Int.down,
            _ => throw new ArgumentOutOfRangeException(nameof(thisGridDir), thisGridDir, null)
        };
    }

    public static Tile Get(this Tile[,] thisGrid,Vector2Int coord)
    {
        return thisGrid[coord.x, coord.y];
    }
        
    public static Tile Set(this Tile[,] thisGrid,Tile tileToSet ,Vector2Int coord)
    {
        Tile tileAtCoord = thisGrid.Get(coord);

        thisGrid[coord.x, coord.y] = tileToSet;
        ICoordSet coordSet = tileToSet;
            
        coordSet.SetCoord(coord);

        return tileAtCoord;
    }

    public static void Swap(this Tile[,] thisGrid, Tile fromTile, Vector2Int toCoords)
    {
        Vector2Int fromCoords  = fromTile.Coords;
            
        Tile toTile = thisGrid.Set(fromTile, toCoords);
        thisGrid.Set(toTile, fromCoords);
    }
    public static void Swap(this Tile[,] thisGrid, Tile fromTile, Tile toTile)
    {
        Vector2Int fromCoords = fromTile.Coords;
        Vector2Int toCoords = toTile.Coords;
            
        thisGrid.Set(fromTile, toCoords );
        thisGrid.Set(toTile, fromCoords);
            
    }
        
        
}

public enum GridDir
{
    Null,
    left,
    Right,
    Up,
    Down
}