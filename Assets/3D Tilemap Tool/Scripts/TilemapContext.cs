using UnityEngine;
using System.Collections.Generic;

public static class TilemapContext
{
    // All of the most used variables that must be global
    
    public static ITool selectedTool; // Selected tool

    public static List<ITool> tools = new() // List of all available tools
    {
        new Paint(),
        new Erase(),
        new BoxFill(),
        new BoxErase()
    };
    
    public static Dictionary<Vector3Int, Tile> placedTiles = new Dictionary<Vector3Int, Tile>(); // A record of all tiles placed on the grid
    
    public static TileEntry currentSelectedTile; // The current selected tile that will be placed
    public static GridDrawer tilemap; // The grid 
    
    public static Vector3Int tileSize = Vector3Int.one; // Size of the squares on the grid (perhaps a bit misleading)
    public static Vector2Int gridSize = new Vector2Int(5, 5); // Size of visual grid drawn is scene view
    
    // Placement Variables
    public static int yValue; // The height of the object to be placed
    public static Vector3Int mouseHoverPos; // The position of the mouse in the world
}

