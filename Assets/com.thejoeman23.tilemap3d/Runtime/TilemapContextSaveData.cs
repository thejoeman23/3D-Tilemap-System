using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TilemapContextData", menuName = "Tilemap/Context Save Data")]
public class TilemapContextSaveData : ScriptableObject
{
    public Vector3Int tileSize;
    public Vector2Int gridSize = new Vector2Int(5, 5);
    public int yValue;
    public Vector3Int mouseHoverPos;
    public TileEntry currentSelectedTile;

    public List<Vector3Int> placedTilesKeys = new();
    public List<Tile> placedTilesValues = new();
}