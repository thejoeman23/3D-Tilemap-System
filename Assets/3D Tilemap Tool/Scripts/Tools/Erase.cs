using UnityEngine;

public class Erase : MonoBehaviour, ITool
{
    public void OnSelected()
    {
        
    }

    public void OnClick()
    {
        Vector3Int position = TilemapContext.mouseHoverPos;
        
        if (!TilemapContext.placedTiles.TryGetValue(position, out Tile tile))
            return;
        
        TilemapContext.placedTiles.Remove(position);
        DestroyImmediate(tile.prefabInstance);
    }

    public void OnDeselected()
    {
        
    }
}