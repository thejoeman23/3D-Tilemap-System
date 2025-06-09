using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MousePosUpdater
{
    static MousePosUpdater()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (TilemapContext.selectedTool == null) 
            return;
        
        if (EditorWindow.mouseOverWindow is not SceneView)
            return;
        
        Event e = Event.current;
        if (e == null) return;
        
        UpdateMouseHoverPos(e, sceneView);
    }

    private static Vector3 FindPointAtY(Vector3 start, Vector3 end, float targetY)
    {
        Vector3 direction = end - start;
        if (Mathf.Approximately(direction.y, 0f)) return Vector3.zero;

        float t = (targetY - start.y) / direction.y;
        return start + direction * t;
    }

    private static void UpdateMouseHoverPos(Event e, SceneView sceneView)
    {
        Camera cam = sceneView.camera;
        if (!cam) return;

        Vector2 mousePosition = e.mousePosition;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;

        Ray ray = cam.ScreenPointToRay(mousePosition);
        float targetY = TilemapContext.yValue;

        Vector3 intersection = FindPointAtY(ray.origin, ray.origin + ray.direction * 1000f, targetY);

        int roundedX = Mathf.RoundToInt(intersection.x / TilemapContext.tileSize.x);
        int roundedZ = Mathf.RoundToInt(intersection.z / TilemapContext.tileSize.x);
        int roundedY = Mathf.RoundToInt(targetY);

        TilemapContext.mouseHoverPos = new Vector3Int(roundedX, roundedY, roundedZ);
        HandleUtility.Repaint(); // force redraw
    }
}