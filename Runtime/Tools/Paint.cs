using UnityEngine;

public class Paint : MonoBehaviour, ITool
{
    public void OnSelected()
    {
        
    }

    public void OnClick()
    {
        // Pulls mousehoverpos
        Vector3Int position = TilemapContext.mouseHoverPos;
        
        // If there is already a tile at mousehoverpos do nothing
        if (TilemapContext.placedTiles.ContainsKey(position) || TilemapContext.currentSelectedTile == null)
            return;
        
        if (LayerManager.CurrentLayer == null)
        { 
            Debug.LogWarning("No Layer Selected");
            return;
        }
        
        // Pull current selected tile and instantiates it into the scene

        string key = LayerManager.CurrentLayer;
                    
        TileEntry entry = TilemapContext.currentSelectedTile;
        GameObject instance = Instantiate(entry.prefab, position, Quaternion.identity);
        instance.transform.SetParent(LayerManager.Layers[key]);
        
        // Creates Tile object and sets up variables
        Tile tile = new Tile(instance, entry.type, entry.label);
        
        // Adds the new tile to dictionary of placed tiles
        TilemapContext.placedTiles.Add(position, tile);
    }

    public void OnDeselected()
    {
        
    }
}