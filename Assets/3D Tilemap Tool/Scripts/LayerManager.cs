using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class LayerManager
{
    static Dictionary<string, Transform> _layers = new Dictionary<string, Transform>(); // A record of all the layers
    static string _currentLayer;

    // NOTE: _grid was only set from TilemapContext.tilemap.transform — removed, unused.
    // If you need it, access TilemapContext.tilemap.transform directly.

    public static Dictionary<string, Transform> Layers
    {
        get { return _layers; }
    }

    public static void AddLayer(string layerName)
    {
        string verifiedOriginalName = VerifiedLayerName(layerName);

        GameObject newLayer = new GameObject(verifiedOriginalName);
        _layers.Add(verifiedOriginalName, newLayer.transform);
        newLayer.transform.SetParent(TilemapContext.tilemap.transform);
        _currentLayer = verifiedOriginalName;
    }

    public static string CurrentLayer
    {
        get { return _currentLayer; }
        set { _currentLayer = value; }
    }

    public static int CurrentLayerIndex
    {
        get { return _layers.Keys.ToList().IndexOf(_currentLayer); }
    }

    public static List<string> GetLayerNames()
    {
        return _layers.Keys.ToList();
    }

    static string VerifiedLayerName(string baseName)
    {
        return GenerateUniqueName(baseName, 1);
    }

    static string GenerateUniqueName(string baseName, int n)
    {
        if (!_layers.ContainsKey(baseName))
            return baseName;

        string newName = baseName + " (" +  n + ")";
        return _layers.ContainsKey(newName)
            ? GenerateUniqueName(baseName, n + 1)
            : newName;
    }

    public static void SetCurrentLayerIndex(int i)
    {
        var keys = GetLayerNames(); // safer and more consistent
        if (i >= 0 && i < keys.Count)
        {
            _currentLayer = keys[i];
        }
    }

    public static int GetCurrentLayerIndex()
    {
        var keys = GetLayerNames();
        return keys.IndexOf(CurrentLayer);
    }
}
