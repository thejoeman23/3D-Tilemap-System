using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class GridDrawer : MonoBehaviour
{
    // Makes sure all scripts can access this if needed
    public static GridDrawer Instance { get; private set; }

    private List<Vector3Int> _boxFillPositions = new List<Vector3Int>();
    
    void OnEnable()
    {
        Instance = this;
        SceneView.duringSceneGui += OnSceneGUI;
        
        _boxFillPositions.Clear();
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

        Debug.Log(_boxFillPositions.Count);
        
        DrawGrid(TilemapContext.mouseHoverPos);
        if (_boxFillPositions.Count != 0)
            DrawBoxFillVisuals();

        HandleUtility.Repaint(); // Force SceneView to redraw
    }

   private void DrawBoxFillVisuals()
{
    Vector3Int mousePos = TilemapContext.mouseHoverPos;

    if (_boxFillPositions.Count == 1)
    {
        Vector3Int point = _boxFillPositions[0];

        int xMin = Mathf.Min(point.x, mousePos.x);
        int xMax = Mathf.Max(point.x, mousePos.x);

        int zMin = Mathf.Min(point.z, mousePos.z);
        int zMax = Mathf.Max(point.z, mousePos.z);

        float xSize = xMax - xMin + 1; // +1 if you want inclusive bounds
        float zSize = zMax - zMin + 1;

        float xCenter = (xMax + xMin) / 2f;
        float zCenter = (zMax + zMin) / 2f;

        Vector3 center = new Vector3(
            xCenter * TilemapContext.tileSize.x,
            TilemapContext.yValue,
            zCenter * TilemapContext.tileSize.x);

        Vector3 size = new Vector3(
            xSize * TilemapContext.tileSize.x,
            0f, // small height for 2D visual
            zSize * TilemapContext.tileSize.x);

        Handles.color = Color.red;
        Handles.DrawWireCube(center, size);
    }
    else if (_boxFillPositions.Count == 2)
    {
        Vector3Int p1 = _boxFillPositions[0];
        Vector3Int p2 = _boxFillPositions[1];

        int xMin = Mathf.Min(p1.x, p2.x);
        int xMax = Mathf.Max(p1.x, p2.x);

        int yMin = Mathf.Min(p1.y, mousePos.y);
        int yMax = Mathf.Max(p1.y, mousePos.y);

        int zMin = Mathf.Min(p1.z, p2.z);
        int zMax = Mathf.Max(p1.z, p2.z);

        float xSize = xMax - xMin + 1;
        float ySize = yMax - yMin + 1;
        float zSize = zMax - zMin + 1;

        float xCenter = (xMax + xMin) / 2f;
        float yCenter = (yMax + yMin) / 2f;
        float zCenter = (zMax + zMin) / 2f;

        Vector3 center = new Vector3(
            xCenter * TilemapContext.tileSize.x,
            yCenter,
            zCenter * TilemapContext.tileSize.x);

        Vector3 size = new Vector3(
            xSize * TilemapContext.tileSize.x,
            ySize,
            zSize * TilemapContext.tileSize.x);

        Handles.color = Color.red;
        Handles.DrawWireCube(center, size);
    }
}

    
    private void DrawGrid(Vector3Int gridPosition)
    {
        // Pulls variables from TilemapContext
        int gridxSize = TilemapContext.gridSize.x;
        int gridzSize = TilemapContext.gridSize.y;
        float r = Mathf.Pow(Mathf.Max(gridxSize, gridzSize), 2);
        
        // Draws grid
        for (int x = gridPosition.x-gridxSize; x <= gridPosition.x+gridxSize; x++)
        {
            for (int z = gridPosition.z-gridzSize; z <= gridPosition.z+gridzSize; z++)
            {
                // Calculations for fading transparency at edges of grid. Purely for asthetic purposes only.

                #region Color Calculations
                float dx = x - gridPosition.x;
                float dz = z - gridPosition.z;
                float xz = (dx * dx) + (dz * dz);

                float alpha = Mathf.Clamp01(.5f - (xz / r));

                Color color = IsMiddle(x, z, gridPosition) ? 
                    Color.red : // If its the middle of the grid (where the object will be placed) color it red for user clarity
                    Color.cyan; // else color it normal
                
                color.a = alpha;

                Handles.color = color;
                #endregion
                
                // Calculates position of square then draws it
                Vector3 pos = new Vector3(x * TilemapContext.tileSize.x, TilemapContext.yValue, z * TilemapContext.tileSize.x);
                Handles.DrawWireCube(pos, new Vector3(TilemapContext.tileSize.x, 0, TilemapContext.tileSize.x));
            }
        }
    }

    // Checks if pos is the middle of the grid
    private bool IsMiddle(int x, int z, Vector3Int point)
    {
        return x == point.x && z == point.z ? true : false;
    }

    public void AddGridPosition(Vector3Int gridPosition)
    {
        if (_boxFillPositions.Count < 2)
            _boxFillPositions.Add(gridPosition);
        else
            return;
    }

    public void ClearGridPositions() => _boxFillPositions.Clear();
}