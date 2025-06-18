using UnityEngine;

public class Erase : MonoBehaviour, ITool
{
    public void OnSelected()
    {
        
    }

    public void OnClick()
    {
        // Pulls mouse hover pos
        Vector3Int position = TilemapContext.mouseHoverPos;
        
        // If theres no tile to erase at the mouse hover pos do nothing
        if (!TilemapContext.placedTiles.TryGetValue(position, out Tile tile))
            return;
        
        if (LayerManager.CurrentLayer == null)
        { 
            Debug.LogWarning("No Layer Selected");
            return;
        }
        
        if (!IsInLayer(tile))
            return;
        
        // Remove tile from dictionary and destroy the object from sceneview
        TilemapContext.placedTiles.Remove(position);
        TilemapContext.UploadPlacedTiles();
        DestroyImmediate(tile.prefabInstance);
    }

    bool IsInLayer(Tile tile) // checks if tile is in the current layer
    {
        LayerManager.Layers.TryGetValue(LayerManager.CurrentLayer, out Transform layer);
        
        return tile.prefabInstance.transform.parent == layer;
    }
    
    public void OnDeselected()
    {
        
    }
    
    
}