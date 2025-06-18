using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TilemapContextData", menuName = "Tilemap/Context Save Data")]
public class TilemapContextSaveData : ScriptableObject
{
    public Vector3Int tileSize; // Size of the squares in the grid
    public Vector2Int gridSize = new Vector2Int(5, 5); // Size of the grid thats drawn to the screen
    public int yValue; // Height of the cursor
    public Vector3Int mouseHoverPos; // World location of the cursor
    public TileEntry currentSelectedTile; // The selected tile to place

    // You cant keep a dictionary in here so i split up the keys and values into 2 seperate lists
    public List<Vector3Int> placedTilesKeys = new();
    public List<Tile> placedTilesValues = new();
}