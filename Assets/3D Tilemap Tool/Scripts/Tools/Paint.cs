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
        
        // Pull current selected tile and instantiates it into the scene
        TileEntry entry = TilemapContext.currentSelectedTile;
        GameObject prefabInstance = Instantiate(entry.prefab, position, Quaternion.identity);
        
        // Creates Tile object and sets up variables
        Tile tile = new Tile(prefabInstance, entry.type, entry.label);
        
        // Adds the new tile to dictionary of placed tiles
        TilemapContext.placedTiles.Add(position, tile);
    }

    public void OnDeselected()
    {
        
    }
}