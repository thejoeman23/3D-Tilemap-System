using UnityEngine;
using System.Collections.Generic;

public static class TilemapContext
{
    public static ITool selectedTool;

    public static List<ITool> tools = new()
    {
        new Paint(),
        new Erase(),
        new BoxFill()
    };
    
    public static Dictionary<Vector3Int, Tile> placedTiles = new Dictionary<Vector3Int, Tile>();
    public static TileEntry currentSelectedTile;
    public static GridDrawer tilemap;
    
    public static Vector3Int tileSize = Vector3Int.one;
    public static Vector2Int gridSize = new Vector2Int(5, 5);
    
    // Placement Variables
    public static int yValue;
    public static Vector3Int mouseHoverPos;
}

