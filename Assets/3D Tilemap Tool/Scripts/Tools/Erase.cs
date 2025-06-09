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
        
        // Remove tile from dictionary and destroy the object from sceneview
        TilemapContext.placedTiles.Remove(position);
        DestroyImmediate(tile.prefabInstance);
    }

    public void OnDeselected()
    {
        
    }
}