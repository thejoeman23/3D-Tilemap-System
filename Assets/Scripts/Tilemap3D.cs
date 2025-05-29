using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class Tilemap3D : MonoBehaviour
{
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeGameObject != gameObject) return;
        
        Handles.color = Color.cyan;

        int gridxSize = TilemapContext.gridSize.x;
        int gridzSize = TilemapContext.gridSize.y;

        for (int x = -gridxSize; x <= gridxSize; x++)
        {
            for (int z = -gridzSize; z <= gridzSize; z++)
            {
                Vector3 pos = new Vector3(x * TilemapContext.tileSize.x, TilemapContext.yValue, z * TilemapContext.tileSize.x);
                Handles.DrawWireCube(pos, new Vector3(TilemapContext.tileSize.x, TilemapContext.yValue, TilemapContext.tileSize.x));
            }
        }

        HandleUtility.Repaint(); // Force SceneView to redraw
    }


    public void PlaceTile(Vector3 position)
    {
        // place Tile
    }
}