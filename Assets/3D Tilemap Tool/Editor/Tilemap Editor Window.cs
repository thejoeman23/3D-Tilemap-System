using System;
using System.Net.Mime;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Object = System.Object;

public class TilemapEditorWindow : EditorWindow
{
    [SerializeField] TilePalette tilePalette;
    [SerializeField] private Color normalColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color selectedColor;
    
    GUIStyle backgroundStyle;
    
    GUIStyle selectedStyle;
    GUIStyle buttonStyle;
    
    Vector2 scrollPosition;

    [MenuItem ("Jobs/3D Tilemap Tool")]

    public static void ShowWindow() => EditorWindow.GetWindow(typeof(TilemapEditorWindow));

    void OnGUI()
    {
        SetupStyles();
        
        Tilemap3D existingGrid = GameObject.FindObjectOfType<Tilemap3D>();
        if (existingGrid != null) TilemapContext.tilemap = existingGrid;
        
        if (TilemapContext.tilemap == null)
        {
            if (GUILayout.Button("Create Grid"))
            {
                GameObject grid = new GameObject("Grid3D");
                grid.AddComponent<Tilemap3D>();
                TilemapContext.tilemap = grid.GetComponent<Tilemap3D>();
            }
        }
        
        tilePalette = (TilePalette)EditorGUILayout.ObjectField("Tile Palette", tilePalette, typeof(TilePalette), true);

        if (tilePalette == null || tilePalette.tiles == null) return;

        DrawButtons();
        
        DrawTiles();

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

    void DrawButtons()
    {
        EditorGUILayout.BeginHorizontal(backgroundStyle);

        foreach (SelectedTool tool in Enum.GetValues(typeof(SelectedTool)))
        {
            DrawToolButton(tool);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    void DrawToolButton(SelectedTool tool)
    {
        Texture2D icon = TilemapIcons.GetIcon(tool);
        if (icon == null) return;

        GUIStyle style = (TilemapContext.selectedTool == tool) ? selectedStyle : buttonStyle;

        if (GUILayout.Button(icon, style, GUILayout.Width(50), GUILayout.Height(50)))
        {
            // Toggle selection
            TilemapContext.selectedTool = (TilemapContext.selectedTool == tool)
                ? SelectedTool.None
                : tool;
        }
    }

    void DrawTiles()
    {
        float tileSize = 80f;
        float padding = 10f;
        float totalTileSize = tileSize + padding;
        int tilesPerRow = Mathf.FloorToInt((position.width - 20) / totalTileSize);

        int col = 0;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(backgroundStyle); // Always open one

        for (int i = 0; i < tilePalette.tiles.Count; i++)
        {
            var entry = tilePalette.tiles[i];

            // Get preview
            Texture2D preview = AssetPreview.GetAssetPreview(entry.prefab);
            if (preview == null)
                continue;

            // Draw button
            bool isSelected = TilemapContext.currentSelectedTile == entry;
            GUIStyle style = isSelected ? selectedStyle : buttonStyle;

            if (GUILayout.Button(preview, style, GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
            {
                TilemapContext.currentSelectedTile = isSelected ? null : entry;
            }

            col++;

            // When a row is full, close it and open a new one
            if (col >= tilesPerRow)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(backgroundStyle);
                col = 0;
            }
        }

        // Finish any row left open
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }


    private void SetupStyles()
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

    private Texture2D SetTexture(Color color)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point; // if you're going for a crisp pixel look
        tex.Apply();

        return tex;
    }
}