using System;using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "3D Tilemap/Tile Palette")]
public class TilePalette : ScriptableObject
{
    public List<TilePrefabEntry> tiles;
}

[System.Serializable]
public class TilePrefabEntry
{
    public string label;
    public GameObject prefab;
    public TileType type; // enum: Floor, Wall, Corner, Event, etc.
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