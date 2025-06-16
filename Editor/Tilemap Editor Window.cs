using System.Linq;
using UnityEngine;
using UnityEditor;

public class TilemapEditorWindow : EditorWindow
{
    // Setting varibales visible in editor window
    [SerializeField] TilePalette tilePalette;
    [SerializeField] private Color normalColor = new Color(0.38f, 0.38f, 0.38f, 255);
    [SerializeField] private Color selectedColor = new Color(0.07f, 0.66f, 0.94f, 255);
    [SerializeField] Color hoverColor = new Color(0.58f, 0.58f, 0.58f, 255);
    
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
        GridDrawer existingGrid = GridDrawer.Instance;
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
        EditorGUILayout.BeginHorizontal();
        
        EditorGUIUtility.labelWidth = 90; // or any smaller number
        
        if (LayerManager.Layers.Count == 0)
            LayerManager.AddLayer("Default Layer");
        else
        {
            LayerManager.SetCurrentLayerIndex(
                EditorGUILayout.Popup
                (
                    "Layer",
                    LayerManager.CurrentLayerIndex,
                    LayerManager.GetLayerNames().ToArray()
                )
            );
            
            if (GUILayout.Button("\u274C", GUILayout.Width(30)))
            {
                int currentIndex = LayerManager.GetCurrentLayerIndex();
                if (currentIndex > 0)
                {
                    LayerManager.Layers.Remove(LayerManager.CurrentLayer);
                    LayerManager.SetCurrentLayerIndex(currentIndex - 1);
                }
                else if (currentIndex == 0 && LayerManager.Layers.Count > 0)
                {
                    LayerManager.Layers.Remove(LayerManager.CurrentLayer);
                    LayerManager.SetCurrentLayerIndex(currentIndex);
                }
                else
                {
                    LayerManager.Layers.Remove(LayerManager.CurrentLayer);
                    LayerManager.CurrentLayer = null;
                }
            }  
        }

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
                LayerManager.AddLayer(_newLayerName);

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
            
            // Draw button
            bool isSelected = TilemapContext.currentSelectedTile == entry; // Checks is this tile is currently the selected tile 
            GUIStyle style = isSelected ? selectedStyle : buttonStyle; // If its selected use the selected style if not use the normal style
            
            // Get preview
            Texture2D preview = AssetPreview.GetAssetPreview(entry.prefab); // Gets visual preview of the prefab to display later
            
            if (preview == null)
            {
                GUILayout.Box("No Preview", style, GUILayout.Width(tileSize), GUILayout.Height(tileSize));
            }
            else
            {
                // NOTE: GUILayout.Button acts as a bool and a function. It creates the button then if its pressed (true or false) it runs the if statement
                if (GUILayout.Button(preview, style, GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
                {
                    TilemapContext.currentSelectedTile = isSelected ? null : entry;
                }
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
