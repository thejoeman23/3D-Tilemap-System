using UnityEngine;

public static class TilemapContext
{
    public static SelectedTool selectedTool;
    public static TilePrefabEntry currentSelectedTile;
    public static Tilemap3D currentTilemap;
    public static Vector3Int tileSize = Vector3Int.one;
    public static int yValue;
    public static Vector2Int gridSize = new Vector2Int(5, 5);
    public static Tilemap3D tilemap;
    public static Vector3Int mouseHoverPos;
}

public enum SelectedTool
{
    None,
    Paint,
    Erase,
    BoxFill,
}
