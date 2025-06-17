using UnityEngine;
using System.Collections.Generic;

public class BoxFill : MonoBehaviour, ITool
{
    private int _clickCounter = 0;
    private readonly List<Vector3Int> _points = new List<Vector3Int>();

    public void OnSelected()
    {
        _clickCounter = 0;
        _points.Clear();
    }

    public void OnClick()
    {
        if (LayerManager.CurrentLayer == null)
        { 
            Debug.LogWarning("No Layer Selected");
            return;
        }
        
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
            SpawnTiles();
            _clickCounter = 0;
            _points.Clear();
        }
    }


    public void OnDeselected()
    {
        _clickCounter = 0;
        _points.Clear();
    }

    private void SpawnTiles()
    {
        if (_points.Count < 3 || TilemapContext.currentSelectedTile == null)
            return;

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

                    if (TilemapContext.placedTiles.ContainsKey(pos))
                        continue; // skip already placed tiles

                    string key = LayerManager.CurrentLayer;
                    
                    TileEntry entry = TilemapContext.currentSelectedTile;
                    GameObject instance = Instantiate(entry.prefab, pos, Quaternion.identity);
                    instance.transform.SetParent(LayerManager.Layers[key]);
                    
                    Tile tile = new(instance, entry.type, entry.label);
                    TilemapContext.placedTiles.Add(pos, tile);
                }
            }
        }
        
        TilemapContext.UploadPlacedTiles();
    }
}
