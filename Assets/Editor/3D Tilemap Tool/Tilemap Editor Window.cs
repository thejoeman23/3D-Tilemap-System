using System;
using UnityEngine;
using UnityEditor;

public class TilemapEditorWindow : EditorWindow
{
    [SerializeField] TilePalette tilePalette;
    Vector2 scrollPosition;
    
    [MenuItem ("Jobs/3D Tilemap Tool")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TilemapEditorWindow));
    }

    void OnGUI()
    {
        tilePalette = (TilePalette)EditorGUILayout.ObjectField("Tile Palette", tilePalette, typeof(TilePalette), true);

        if (tilePalette == null || tilePalette.tiles == null) return;

        if (TilemapContext.tilemap == null)
        {
            if (GUILayout.Button("Create Grid"))
            {
                GameObject grid = new GameObject("Grid3D");
                grid.AddComponent<Tilemap3D>();
                TilemapContext.tilemap = grid.GetComponent<Tilemap3D>();
            }
        }

        float tileSize = 80f;
        float padding = 10f;
        float totalTileSize = tileSize + padding;
        int tilesPerRow = Mathf.FloorToInt((position.width - 20) / totalTileSize);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int row = 0;
        int col = 0;

        for (int i = 0; i < tilePalette.tiles.Count; i++)
        {
            if (col == 0)
                EditorGUILayout.BeginHorizontal();

            var entry = tilePalette.tiles[i];
            Texture2D preview = AssetPreview.GetAssetPreview(entry.prefab);

            GUIStyle style = new GUIStyle(GUI.skin.button);
            if (TilemapContext.currentSelectedTile == entry.prefab)
            {
                style.normal.background = Texture2D.grayTexture;
            }

            if (GUILayout.Button(preview, style, GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
            {
                TilemapContext.currentSelectedTile = entry.prefab;
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
            EditorGUILayout.EndHorizontal(); // End last row if incomplete

        EditorGUILayout.EndScrollView();
    }
}