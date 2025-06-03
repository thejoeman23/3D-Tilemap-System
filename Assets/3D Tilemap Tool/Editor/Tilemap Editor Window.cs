using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Object = System.Object;

public class TilemapEditorWindow : EditorWindow
{
    [SerializeField] TilePalette tilePalette;
    GUIStyle backgroundStyle;
    GUIStyle defaultBackgroundStyle;
    Vector2 scrollPosition;
    
    [MenuItem ("Jobs/3D Tilemap Tool")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TilemapEditorWindow));
    }

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
        
        EditorGUILayout.BeginVertical();
        
        TilemapContext.tileSize = EditorGUILayout.Vector3IntField(
            new GUIContent("Tile Size"),
            TilemapContext.tileSize
        );
        
        EditorGUILayout.EndVertical();
    }

    void DrawButtons()
    {
        EditorGUILayout.BeginHorizontal(backgroundStyle);
        
        Texture2D icon = TilemapIcons.PaintbrushIcon;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.background = Texture2D.grayTexture;

        if (GUILayout.Button(icon, buttonStyle, GUILayout.Width(50), GUILayout.Height(50)))
        {
            TilemapContext.selectedTool = SelectedTool.Paint;
        }

        icon = TilemapIcons.EraserIcon;
        
        if (GUILayout.Button(icon, buttonStyle, GUILayout.Width(50), GUILayout.Height(50)))
        {
            TilemapContext.selectedTool = SelectedTool.Paint;
        }

        icon = TilemapIcons.BoxFillIcon;
        
        if (GUILayout.Button(icon, buttonStyle, GUILayout.Width(50), GUILayout.Height(50)))
        {
            TilemapContext.selectedTool = SelectedTool.Paint;
        }
        
        EditorGUILayout.EndHorizontal();
    }

    void DrawTiles()
    {
        float tileSize = 80f;
        float padding = 10f;
        float totalTileSize = tileSize + padding;
        int tilesPerRow = Mathf.FloorToInt((position.width - 20) / totalTileSize);

        int row = 0;
        int col = 0;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < tilePalette.tiles.Count; i++)
        {
            if (col == 0)
                EditorGUILayout.BeginHorizontal();

            var entry = tilePalette.tiles[i];
            Texture2D preview = AssetPreview.GetAssetPreview(entry.prefab);

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = tileSize,
                fixedHeight = tileSize,
                margin = new RectOffset(2, 2, 2, 2)
            };

            if (TilemapContext.currentSelectedTile == entry.prefab)
            {
                style.normal.background = Texture2D.grayTexture;
            }

            Rect rect = GUILayoutUtility.GetRect(tileSize, tileSize, style);

            if (GUI.Button(rect, GUIContent.none, style))
            {
                TilemapContext.currentSelectedTile = entry.prefab;
            }

            if (preview != null)
            {
                // Draw the image manually over the button
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }

            col++;

            if (col >= tilesPerRow)
            {
                EditorGUILayout.EndHorizontal();
                col = 0;
                row++;
            }
        }

        if (col > 0)
            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void SetupStyles()
    {
        defaultBackgroundStyle = GUI.skin.label;
        defaultBackgroundStyle = GUI.skin.button;
        defaultBackgroundStyle = GUI.skin.textField;
        

        // Create a GUIStyle based on "box" but override background
        backgroundStyle = new GUIStyle(GUI.skin.box);
        backgroundStyle.normal.background = Texture2D.blackTexture;
        backgroundStyle.border = new RectOffset(4, 4, 4, 4); // Optional: helps with padding visuals
    }
}