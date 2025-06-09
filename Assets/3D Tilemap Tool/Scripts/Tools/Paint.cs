using UnityEngine;

public class Paint : MonoBehaviour, ITool
{
    public void OnSelected()
    {
        
    }

    public void OnClick()
    {
        Vector3Int position = TilemapContext.mouseHoverPos;
        
        if (TilemapContext.placedTiles.ContainsKey(position) || TilemapContext.currentSelectedTile == null)
            return;
        
        TileEntry entry = TilemapContext.currentSelectedTile;
        GameObject prefabInstance = Instantiate(entry.prefab, position, Quaternion.identity);

        Tile tile = new Tile(prefabInstance, entry.type, entry.label);
        TilemapContext.placedTiles.Add(position, tile);
    }

    public void OnDeselected()
    {
        
    }
}