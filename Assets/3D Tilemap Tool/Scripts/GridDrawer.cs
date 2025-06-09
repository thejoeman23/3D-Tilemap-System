using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class GridDrawer : MonoBehaviour
{
    // Makes sure all scripts can access this if needed
    public static GridDrawer Instance { get; private set; } 
    
    void OnEnable()
    {
        Instance = this;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnSceneGUI(SceneView sceneView) // Called Every Frame
    {
        if (TilemapContext.selectedTool == null) 
            return;
        
        if (EditorWindow.mouseOverWindow is not SceneView)
            return;
        
        // Pulls variables from TilemapContext
        int gridxSize = TilemapContext.gridSize.x;
        int gridzSize = TilemapContext.gridSize.y;
        float r = Mathf.Pow(Mathf.Max(gridxSize, gridzSize), 2);

        Vector3Int mousePos = TilemapContext.mouseHoverPos;
        
        // Draws grid
        for (int x = mousePos.x-gridxSize; x <= mousePos.x+gridxSize; x++)
        {
            for (int z = mousePos.z-gridzSize; z <= mousePos.z+gridzSize; z++)
            {
                // Calculations for fading transparency at edges of grid. Purely for asthetic purposes only.

                #region Color Calculations
                float dx = x - mousePos.x;
                float dz = z - mousePos.z;
                float xz = (dx * dx) + (dz * dz);

                float alpha = Mathf.Clamp01(1f - (xz / r));

                Color color = IsMiddle(x, z, mousePos) ? 
                    Color.red : // If its the middle of the grid (where the object will be placed) color it red for user clarity
                    Color.cyan; // else color it normal
                
                color.a = alpha;

                Handles.color = color;
                #endregion
                
                // Calculates position of square then draws it
                Vector3 pos = new Vector3(x * TilemapContext.tileSize.x, TilemapContext.yValue, z * TilemapContext.tileSize.x);
                Handles.DrawWireCube(pos, new Vector3(TilemapContext.tileSize.x, TilemapContext.yValue, TilemapContext.tileSize.x));
            }
        }

        HandleUtility.Repaint(); // Force SceneView to redraw
    }

    // Checks if pos is the middle of the grid
    private bool IsMiddle(int x, int z, Vector3Int point)
    {
        if (x == point.x && z == point.z)
            return true;
        else
            return false;
    }
}