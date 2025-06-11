using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Object = System.Object;

public class TilemapEditorWindow : EditorWindow
{
    // Setting varibales visible in editor window
    [SerializeField] TilePalette tilePalette;
    [SerializeField] private Color normalColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color selectedColor;
    
    // Private varibales
    GUIStyle backgroundStyle;
    
    GUIStyle selectedStyle;
    GUIStyle buttonStyle;
    
    Vector2 scrollPosition;

    string _newLayerName = "New Layer Name";

    
    [MenuItem ("Jobs/3D Tilemap Tool")] // Creates Editor Window in Jobs dropdown
    public static void ShowWindow() => EditorWindow.GetWindow(typeof(TilemapEditorWindow));

    void OnGUI() // The Update() of editor windwos
    {
        SetupStyles();
        
        // Search for existing grid
        GridDrawer existingGrid = GameObject.FindObjectOfType<GridDrawer>();
        if (existingGrid != null) TilemapContext.tilemap = existingGrid;
        
        // If no grid exists add option to create a grid
        if (TilemapContext.tilemap == null)
        {
            if (GUILayout.Button("Create Grid"))
            {
                GameObject grid = new GameObject("Grid3D");
                grid.AddComponent<GridDrawer>();
                TilemapContext.tilemap = grid.GetComponent<GridDrawer>();
            }
        }

        EditorGUILayout.BeginVertical(backgroundStyle);
        
        DrawLayerSelection();
        
        // Draw input for tilepalette
        tilePalette = (TilePalette)EditorGUILayout.ObjectField("Tile Palette", tilePalette, typeof(TilePalette), true);
        
        EditorGUILayout.EndVertical();
        
        
        // If no tilepalette is selected dont draw anything else
        if (tilePalette == null || tilePalette.tiles == null) return;

        // Draw tool buttons for user to select
        DrawToolButtons();
        
        // Draw tiles from tilepalette for user to select
        DrawTiles();

        // Draws variables for editor window like selected color and whatnot
        #region Variables
        EditorGUILayout.BeginVertical();
        
        TilemapContext.tileSize = EditorGUILayout.Vector3IntField(
            new GUIContent("Tile Size"),
            TilemapContext.tileSize
        );
        
        normalColor = EditorGUILayout.ColorField(
            new GUIContent("Normal Color"),
            normalColor
        );
        
        selectedColor = EditorGUILayout.ColorField(
            new GUIContent("Selected Color"),
            selectedColor
        );
        
        hoverColor = EditorGUILayout.ColorField(
            new GUIContent("Hover Color"),
            hoverColor
        );
        
        EditorGUILayout.EndVertical();
        #endregion
        
        Repaint();
    }

    bool _isNamingLayer = false;
    
    void DrawLayerSelection()
    {
        TilemapContext.keys = TilemapContext.layers.Keys.ToList();

        EditorGUILayout.BeginHorizontal();
        
        EditorGUIUtility.labelWidth = 90; // or any smaller number
        
        if (TilemapContext.layers.Count == 0)
            EditorGUILayout.HelpBox("No Layers Available", MessageType.Info);
        else
            TilemapContext.currentLayerIndex = EditorGUILayout.Popup("Layer", TilemapContext.currentLayerIndex, TilemapContext.keys.ToArray());

        if (!_isNamingLayer)
        {
            if (GUILayout.Button("Add Layer", GUILayout.Width(80)))
            {
                _isNamingLayer = true;
            }   
        }
        else
        {
            _newLayerName = EditorGUILayout.TextField("", _newLayerName);

            if (GUILayout.Button("\u2713", GUILayout.Width(30)))
            {
                string verifiedOriginalName = VerifiedLayerName(_newLayerName);
                
                GameObject newLayer = new GameObject(verifiedOriginalName);
                TilemapContext.layers.Add(verifiedOriginalName, newLayer.transform);
                newLayer.transform.SetParent(TilemapContext.tilemap.transform);

                _isNamingLayer = false;
                _newLayerName = "New Layer Name";
            }

            if (GUILayout.Button("\u274C", GUILayout.Width(30)))
            {
                _isNamingLayer = false;
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    string VerifiedLayerName(string baseName)
    {
        return GenerateUniqueName(baseName, 1);
    }

    string GenerateUniqueName(string baseName, int n)
    {
        if (!TilemapContext.layers.ContainsKey(baseName))
            return baseName;

        string newName = baseName + n;
        if (!TilemapContext.layers.ContainsKey(newName))
            return newName;
        else
            return GenerateUniqueName(baseName, n + 1);
    }



    void DrawToolButtons() // Draws buttons for tools
    {
        EditorGUILayout.BeginHorizontal(backgroundStyle);

        // Loop through each tool and draw the button for said tool
        foreach (ITool tool in TilemapContext.tools)
        {
            DrawToolButton(tool);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    void DrawToolButton(ITool tool) // Draws a button for a specific tool
    {
        Texture2D icon = TilemapIcons.GetIcon(tool); // Gets tool icon
        if (icon == null) return; // Catch

        GUIStyle style = (TilemapContext.selectedTool == tool) ? 
            selectedStyle :  // If tool is selected show selected style
            buttonStyle;     // If tool isnt then dont

        // NOTE: GUILayout.Button acts as a bool and a function. It creates the button then if its pressed (true or false) it runs the if statement
        if (GUILayout.Button(icon, style, GUILayout.Width(50), GUILayout.Height(50)))
        {
            // Toggle selection
            TilemapContext.selectedTool = (TilemapContext.selectedTool == tool)
                ? null  // If already selected -> Desellect
                : tool; // Else select
        }
    }

    void DrawTiles() // Draws the buttons for the tiles from the tile palette
    {
        // Setting varibales
        float tileSize = 80f;
        float padding = 10f;
        float totalTileSize = tileSize + padding;
        int tilesPerRow = Mathf.FloorToInt((position.width - 20) / totalTileSize);

        int col = 0;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // Start scroll view

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal(backgroundStyle);

        for (int i = 0; i < tilePalette.tiles.Count; i++)
        {
            var entry = tilePalette.tiles[i]; // Gets tile from tilepallette

            // Get preview
            Texture2D preview = AssetPreview.GetAssetPreview(entry.prefab); // Gets visual preview of the prefab to display later
            if (preview == null)
                continue;

            // Draw button
            bool isSelected = TilemapContext.currentSelectedTile == entry; // Checks is this tile is currently the selected tile 
            GUIStyle style = isSelected ? selectedStyle : buttonStyle; // If its selected use the selected style if not use the normal style

            // NOTE: GUILayout.Button acts as a bool and a function. It creates the button then if its pressed (true or false) it runs the if statement
            if (GUILayout.Button(preview, style, GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
            {
                TilemapContext.currentSelectedTile = isSelected ? 
                    null :  // Deselect if already selected
                    entry;  // Select if not already selected
            }

            col++;

            // When a row is full, close the row and reset the collumn count
            if (col >= tilesPerRow)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(backgroundStyle);
                col = 0;
            }
        }
        
        EditorGUILayout.EndHorizontal(); // End row
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView(); 
    }

    private void SetupStyles() // Sets up variables for button styles and so on
    {
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.background = SetTexture(normalColor);
        buttonStyle.hover.background = SetTexture(hoverColor);
        
        selectedStyle = new GUIStyle(GUI.skin.button);
        selectedStyle.normal.background = SetTexture(selectedColor);

        // Create a GUIStyle based on "box" but override background
        backgroundStyle = new GUIStyle(GUI.skin.box);
        backgroundStyle.normal.background = Texture2D.blackTexture;
        backgroundStyle.border = new RectOffset(4, 4, 4, 4); // Optional: helps with padding visuals
    }

    private Texture2D SetTexture(Color color) // Used to create textures with given color to use in button styles
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point; // if you're going for a crisp pixel look
        tex.Apply();

        return tex;
    }
}