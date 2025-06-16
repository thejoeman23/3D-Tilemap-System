using System;using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "3D Tilemap/Tile Palette")]
public class TilePalette : ScriptableObject
{
    // List of tiles for tilemap
    public List<TileEntry> tiles;
}

[System.Serializable]
public class TileEntry
{
    // An entry into the tilepalette. Will be displayed as selectable button in TilemapEditorWindow.cs
    public string label;
    public GameObject prefab;
    public TileType type;
}

[System.Serializable]
public class Tile
{
    // Very different from TileEntry, this represents the actual placed tile in the world instead of just an option
    public GameObject prefabInstance; // Reference to the object in the scene
    public TileType type;

    public Tile(GameObject prefabInstance, TileType type, string label)
    {
        this.prefabInstance = prefabInstance;
        this.prefabInstance.name = label;
        this.type = type;
    }
}

[System.Serializable]
public enum TileType
{
    // Will use maybe later
    Floor,
    Wall,
    Corner,
    Event,
    Basic,
} 