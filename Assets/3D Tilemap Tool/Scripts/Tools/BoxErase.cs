using UnityEngine;
using System.Collections.Generic;

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
        Vector3Int position = TilemapContext.mouseHoverPos;

        if (_clickCounter == 2)
            GridDrawer.Instance.ClearGridPositions();

        _points.Add(position);
        _clickCounter++;

        if (_clickCounter < 3)
        {
            GridDrawer.Instance.AddGridPosition(position);
        }

        if (_clickCounter == 3)
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

    private void ClearTiles()
    {
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
    }

    bool IsInLayer(Tile tile)
    {
        return LayerManager.Layers.ContainsValue(tile.prefabInstance.transform.parent);
    }
}
