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

    public static void AddLayer(string layerName) // Adds a layer
    {
        if (GridDrawer.Instance == null)
            return;
        
        // Just gotta check if the name isnt a repeat of something yk ;)
        string verifiedOriginalName = VerifiedLayerName(layerName);

        // Create layer gameobject and whatnot
        GameObject newLayer = new GameObject(verifiedOriginalName);
        _layers.Add(verifiedOriginalName, newLayer.transform);
        newLayer.transform.SetParent(GridDrawer.Instance.transform);
        _currentLayer = verifiedOriginalName;
    }
    
    public static void AddLayerByTransform(Transform layerTransform) // Adds an already existing layer to the list
    {
        _layers.Add(layerTransform.gameObject.name, layerTransform);
    }

    public static void RemoveCurrentLayer() // self explanitory
    {
        if (GridDrawer.Instance == null)
            return;
       
        int currentIndex = GetCurrentLayerIndex();
        if (currentIndex > 0)
        {
            // If theres more than 0 layers, movecurrent layer index down 1
            GridDrawer.Instance.DestroyLayerTransform(Layers.TryGetValue(CurrentLayer, out Transform layerTransform) ? layerTransform : null);
            Layers.Remove(CurrentLayer);
            SetCurrentLayerIndex(currentIndex - 1);
        }
        else if (currentIndex == 0 && LayerManager.Layers.Count > 0)
        {
            // If its set to 0 and theres more layers above it set it to the layer above it
            GridDrawer.Instance.DestroyLayerTransform(Layers.TryGetValue(CurrentLayer, out Transform layerTransform) ? layerTransform : null);
            Layers.Remove(CurrentLayer);
            SetCurrentLayerIndex(currentIndex);
        }
        else
        {
            // If its 0 and theres no more layers set it to null (dont worry tho, TilemapEditorWindow.cs will realize its null and add a a default layer for us :) )
            GridDrawer.Instance.DestroyLayerTransform(Layers.TryGetValue(CurrentLayer, out Transform layerTransform) ? layerTransform : null);
            Layers.Remove(CurrentLayer);
            CurrentLayer = null;
        }
    }

    // Our classic getters and setters
    // |
    // V
    
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

    static string VerifiedLayerName(string baseName) // Makes sure the name isnt already in the layer list
    {
        return GenerateUniqueName(baseName, 1);
    }

    // Recursive! Crowd: "oooo, aaaa!"
    static string GenerateUniqueName(string baseName, int n)
    {
        // If the layer name isnt already a layer
        if (!_layers.ContainsKey(baseName))
            return baseName;
        
        string newName = baseName + " (" +  n + ")";
        return _layers.ContainsKey(newName)
            ? GenerateUniqueName(baseName, n + 1)
            : newName;
    }

    public static void SetCurrentLayerIndex(int i) // Convinient for some cases. Lets you set _currenLayer with an int instead
    {
        var keys = GetLayerNames(); // safer and more consistent
        if (i >= 0 && i < keys.Count)
        {
            _currentLayer = keys[i];
        }
    }

    public static int GetCurrentLayerIndex() // The biproduct of the function above
    {
        var keys = GetLayerNames();
        return keys.IndexOf(CurrentLayer);
    }
}
