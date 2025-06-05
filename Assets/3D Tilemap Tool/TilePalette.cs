using System;using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "3D Tilemap/Tile Palette")]
public class TilePalette : ScriptableObject
{
    public List<TileEntry> tiles;
}

[System.Serializable]
public class TileEntry
{
    public string label;
    public GameObject prefab;
    public TileType type;
}

[System.Serializable]
public class Tile
{
    public GameObject prefabInstance;
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
    Floor,
    Wall,
    Corner,
    Event,
    Basic,
} 