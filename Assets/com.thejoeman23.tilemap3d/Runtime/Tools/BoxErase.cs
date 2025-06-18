using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class BoxErase : MonoBehaviour, ITool
{
    private int _clickCounter = 0;
    private readonly List<Vector3Int> _points = new();

    public void OnSelected()
    {
        _clickCounter = 0;
        _points.Clear();
    }

    public void OnClick()
    {
        // If there is no layer return
        if (LayerManager.CurrentLayer == null)
        { 
            Debug.LogWarning("No Layer Selected");
            return;
        }
        
        // This here counts the number of clicks. (the first click is click 0)
        
        Vector3Int position = TilemapContext.mouseHoverPos;

        if (_clickCounter == 2) // If the player has clicked 3 times clear the visuals since its gonna erase the tiles there now
            GridDrawer.Instance.ClearGridPositions();

        _points.Add(position);
        _clickCounter++;

        if (_clickCounter < 3) // If 3 clicks havent occured update the visuals
        {
            GridDrawer.Instance.AddGridPosition(position);
        }

        if (_clickCounter == 3) // If its the 3rd click clear the tiles
        {
            ClearTiles();
            _clickCounter = 0;
            _points.Clear();
        }
    }


    public void OnDeselected()
    {
        _clickCounter = 0;
        _points.Clear();
    }

    private void ClearTiles() // Clears all the tiles within the specified boundaries
    {
        // Catch
        if (_points.Count < 3 || TilemapContext.currentSelectedTile == null)
            return;

        TileEntry entry = TilemapContext.currentSelectedTile;

        Vector3Int p1 = _points[0];
        Vector3Int p2 = _points[1];
        Vector3Int p3 = _points[2];

        // Calculate bounds using min/max to handle dragging in any direction
        int xMin = Mathf.Min(p1.x, p2.x);
        int xMax = Mathf.Max(p1.x, p2.x);

        int zMin = Mathf.Min(p1.z, p2.z);
        int zMax = Mathf.Max(p1.z, p2.z);

        int yMin = Mathf.Min(p1.y, p3.y);
        int yMax = Mathf.Max(p1.y, p3.y);

        // Begin to loop through all xyz and search for objects to clear
        for (int x = xMin; x <= xMax; x++)
        {
            for (int z = zMin; z <= zMax; z++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    Vector3Int pos = new(x, y, z);

                    if (!TilemapContext.placedTiles.TryGetValue(pos, out Tile tile))
                        continue; // skip already placed tiles

                    if (!IsInLayer(tile))
                        continue; // skip if its not in the current selected layer
                    
                    GameObject instance = tile.prefabInstance;
                    DestroyImmediate(instance);
                    
                    TilemapContext.placedTiles.Remove(pos);
                }
            }
        }
        
        TilemapContext.UploadPlacedTiles();
    }

    bool IsInLayer(Tile tile) // checks if tile is in the current layer
    {
        LayerManager.Layers.TryGetValue(LayerManager.CurrentLayer, out Transform layer);
        
        return tile.prefabInstance.transform.parent == layer;
    }
}
