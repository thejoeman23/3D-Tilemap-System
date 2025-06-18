#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public static class TilemapContext
{
    // This one is quite confusing. The gist of what is does is:
    // Update and pull variables from a ScriptableObject. These variables all scripts need to access and update. Its like a global mailbox all scripts can see.
    // Anyways it would reset every time the scripts recompiled so i back them up into my TilemapContextSaveData object :)
    
    private static TilemapContextSaveData _data; // The scriptable object containing all save data
    private static Dictionary<Vector3Int, Tile> _placedTiles = new();

    private static double _lastSaveTime;

    static TilemapContext()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += LoadData;
#endif
    }

    private static void LoadData() // Updates _data
    {
#if UNITY_EDITOR
        if (_data != null) return;

        
        string[] guids = AssetDatabase.FindAssets("TilemapContext t:Script");
        if (guids.Length == 0)
        {
            Debug.LogError("Could not find TilemapContext.cs script!");
            return;
        }

        string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Invalid script path found!");
            return;
        }
        
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        string assetPath = Path.Combine(scriptDirectory, "TilemapContextData.asset").Replace("\\", "/");

        _data = AssetDatabase.LoadAssetAtPath<TilemapContextSaveData>(assetPath);
        if (_data == null) // If the save data is lost create new data
        {
            _data = ScriptableObject.CreateInstance<TilemapContextSaveData>();
            AssetDatabase.CreateAsset(_data, assetPath);
            Debug.Log("Created new TilemapContextData at: " + assetPath);
        }

        LoadPlacedTiles(); // Loads all placed tiles into _placedTiles from _data
#endif
    }

    // Loads all placed tiles into _placedTiles from _data
    private static void LoadPlacedTiles()
    {
        // Takes the two lists of keys and values and stitches them together
        _placedTiles.Clear();
        for (int i = 0; i < _data.placedTilesValues.Count; i++)
        {
            _placedTiles[_data.placedTilesKeys[i]] = _data.placedTilesValues[i];
        }
    }

    public static void UploadPlacedTiles() // Uploads all placed tiles to _data
    {
        // Splits up dictionary _placedTiles into two lists and stores them in the savedata
        
        _data.placedTilesKeys.Clear();
        _data.placedTilesValues.Clear();

        foreach (var kvp in _placedTiles)
        {
            _data.placedTilesKeys.Add(kvp.Key);
            _data.placedTilesValues.Add(kvp.Value);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(_data);
        DebouncedSaveAssets();
#endif
    }

    private static void MarkDirty() // Let unity know it needs to be updated
    {
#if UNITY_EDITOR
        if (_data == null) return;
        EditorUtility.SetDirty(_data);
        DebouncedSaveAssets();
#endif
    }

    private static void DebouncedSaveAssets()
    {
#if UNITY_EDITOR
        if (EditorApplication.timeSinceStartup - _lastSaveTime > 2.0)
        {
            AssetDatabase.SaveAssets();
            _lastSaveTime = EditorApplication.timeSinceStartup;
        }
#endif
    }

    // === Delegated Fields with Change Detection ===

    public static Vector3Int tileSize
    {
        get => _data != null ? _data.tileSize : Vector3Int.one;
        set
        {
            if (_data != null && _data.tileSize != value)
            {
                _data.tileSize = value;
                MarkDirty();
            }
        }
    }

    public static Vector2Int gridSize
    {
        get => _data != null ? _data.gridSize : new Vector2Int(10, 10);
        set
        {
            if (_data != null && _data.gridSize != value)
            {
                _data.gridSize = value;
                MarkDirty();
            }
        }
    }

    public static int yValue
    {
        get => _data != null ? _data.yValue : 0;
        set
        {
            if (_data != null && _data.yValue != value)
            {
                _data.yValue = value;
                MarkDirty();
            }
        }
    }

    public static Vector3Int mouseHoverPos
    {
        get => _data != null ? _data.mouseHoverPos : Vector3Int.zero;
        set
        {
            if (_data != null && _data.mouseHoverPos != value)
            {
                _data.mouseHoverPos = value;
                MarkDirty();
            }
        }
    }

    public static TileEntry currentSelectedTile
    {
        get => _data != null ? _data.currentSelectedTile : null;
        set
        {
            if (_data != null && _data.currentSelectedTile != value)
            {
                _data.currentSelectedTile = value;
                MarkDirty();
            }
        }
    }

    // A list of placed tiles
    public static Dictionary<Vector3Int, Tile> placedTiles
    {
        get => _placedTiles;
    }

    // Runtime only. Current selected tool
    public static ITool selectedTool;

    // A list of implemented tools. Used by the window script to generate the buttons
    public static List<ITool> tools = new()
    {
        new Paint(),
        new Erase(),
        new BoxFill(),
        new BoxErase()
    };

#if UNITY_EDITOR
    private class TilemapContextProxy : ScriptableObject { }
#endif
}
