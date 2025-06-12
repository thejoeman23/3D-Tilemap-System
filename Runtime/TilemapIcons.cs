using UnityEditor;
using UnityEngine;

public static class TilemapIcons
{
    // A static class of all tilemap icons any script can call on
    
    private static Texture2D PaintbrushIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/Paintbrush.png");
    private static Texture2D EraserIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/Eraser.png");
    private static Texture2D BoxFillIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/BoxPaint.png");
    private static Texture2D BoxEraseIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/BoxErase.png");

    // Returns icon for specific tool
    public static Texture2D GetIcon(ITool tool)
    {
        return tool switch
        {
            Paint => PaintbrushIcon,
            Erase => EraserIcon,
            BoxFill => BoxFillIcon,
            BoxErase => BoxEraseIcon,
            _ => null
        };
    }
}
