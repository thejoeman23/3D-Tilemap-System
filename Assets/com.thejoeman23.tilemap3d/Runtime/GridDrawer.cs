using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class GridDrawer : MonoBehaviour
{
    // Makes sure all scripts can access this if needed
    public static GridDrawer Instance { get; private set; }

    readonly List<Vector3Int> _boxFillPositions = new List<Vector3Int>();
    
    private static Mesh cubeMesh;
    private static Material previewMaterial;

    void OnEnable()
    {
        Instance = this;
        SceneView.duringSceneGui += OnSceneGUI;

        // Load Unity's built-in cube mesh
        if (cubeMesh == null)
        {
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tempCube);
        }

        // Use basic built-in transparent material
        if (previewMaterial == null)
        {
            Shader shader = Shader.Find("Unlit/Color");
            previewMaterial = new Material(shader);
            previewMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        _boxFillPositions.Clear();
    }


    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnSceneGUI(SceneView sceneView) // Called Every Frame
    {
        CheckForExcessLayers();
        
        if (TilemapContext.selectedTool == null) 
            return;
        
        if (EditorWindow.mouseOverWindow is not SceneView)
            return;

        Vector3Int mousePos = TilemapContext.mouseHoverPos;
        
        // Draw Grid at MousePos
        DrawGrid(mousePos, TilemapContext.yValue, Color.cyan, Color.red);

        // Draw Grid at y=0 and draw a line between them to tell how high up you are
        if (TilemapContext.yValue != 0)
        {
            DrawGrid(TilemapContext.mouseHoverPos, 0, Color.gray, Color.gray);
            
            Handles.color = TilemapContext.yValue > 0 ? Color.cyan : Color.red;
            Handles.DrawDottedLine(new Vector3(mousePos.x, TilemapContext.yValue, mousePos.z), new Vector3(mousePos.x, 0, mousePos.z), 5);
        }
        
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
    
    private void DrawGrid(Vector3Int gridPosition, int height, Color normalColor, Color middleColor)
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
                    middleColor : // If its the middle of the grid (where the object will be placed) color it red for user clarity
                    normalColor; // else color it normal
                
                color.a = alpha;

                Handles.color = color;
                #endregion
                
                // Calculates position of square then draws it
                Vector3 pos = new Vector3(x * TilemapContext.tileSize.x, height, z * TilemapContext.tileSize.x);
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

    public void DestroyLayerTransform(Transform layerTransform) => DestroyImmediate(layerTransform.gameObject);
    
    void CheckForExcessLayers()
    {
        foreach (Transform child in transform)
        {
            if (!LayerManager.Layers.ContainsValue(child))
                LayerManager.AddLayerByTransform(child);
        }
    }
}