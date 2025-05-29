using UnityEngine;

public static class TilemapContext
{
    public static GameObject currentSelectedTile;
    public static Tilemap3D currentTilemap;
    public static Vector3Int tileSize = Vector3Int.one;
    public static int yValue;
    public static Vector2Int gridSize = new Vector2Int(5, 5);
    public static Tilemap3D tilemap;
}
