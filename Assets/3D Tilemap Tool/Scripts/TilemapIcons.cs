using UnityEditor;
using UnityEngine;

public static class TilemapIcons
{
    // A static class of all tilemap icons any script can call on
    
    public static Texture2D PaintbrushIcon { get; private set; } = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/Paintbrush.png");
    public static Texture2D EraserIcon { get; private set; } = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/Eraser.png");
    public static Texture2D BoxFillIcon { get; private set; } = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3D Tilemap Tool/Icons/BoxFill.png");

    // Returns icon for specific tool
    public static Texture2D GetIcon(ITool tool)
    {
        return tool switch
        {
            Paint => PaintbrushIcon,
            Erase => EraserIcon,
            BoxFill => BoxFillIcon,
            _ => null
        };
    }
}
