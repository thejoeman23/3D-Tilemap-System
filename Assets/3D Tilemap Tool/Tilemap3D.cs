using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class Tilemap3D : MonoBehaviour
{
    public static Tilemap3D Instance { get; private set; }
    
    void OnEnable()
    {
        Instance = this;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (EditorWindow.mouseOverWindow is not SceneView)
            return;
        
        int gridxSize = TilemapContext.gridSize.x;
        int gridzSize = TilemapContext.gridSize.y;
        float r = Mathf.Pow(Mathf.Max(gridxSize, gridzSize), 2);

        Vector3Int mousePos = TilemapContext.mouseHoverPos;

        for (int x = mousePos.x-gridxSize; x <= mousePos.x+gridxSize; x++)
        {
            for (int z = mousePos.z-gridzSize; z <= mousePos.z+gridzSize; z++)
            {
                float dx = x - mousePos.x;
                float dz = z - mousePos.z;
                float xz = (dx * dx) + (dz * dz);

                float alpha = Mathf.Clamp01(1f - (xz / r));
                
                Color color = Color.cyan;
                color.a = alpha;

                Handles.color = color;
                
                Vector3 pos = new Vector3(x * TilemapContext.tileSize.x, TilemapContext.yValue, z * TilemapContext.tileSize.x);
                Handles.DrawWireCube(pos, new Vector3(TilemapContext.tileSize.x, TilemapContext.yValue, TilemapContext.tileSize.x));
            }
        }

        HandleUtility.Repaint(); // Force SceneView to redraw
    }


    public void PlaceTile(Vector3 position)
    {
        Instantiate(TilemapContext.currentSelectedTile, position, Quaternion.identity);
    }
}