using UnityEngine;
using System.Collections.Generic;

public static class TilemapContext
{
    public static SelectedTool selectedTool;
    
    public static Dictionary<Vector3Int, Tile> placedTiles = new Dictionary<Vector3Int, Tile>();
    public static TileEntry currentSelectedTile;
    public static Tilemap3D tilemap;
    
    public static Vector3Int tileSize = Vector3Int.one;
    public static Vector2Int gridSize = new Vector2Int(5, 5);
    
    // Placement Variables
    public static int yValue;
    public static Vector3Int mouseHoverPos;
}

public enum SelectedTool
{
    None,
    Paint,
    Erase,
    BoxFill,
}
