using UnityEditor;
using UnityEngine;

[InitializeOnLoad] // Automatically creates the object and calls MousePosUpdater()
public static class MousePosUpdater
{
    // Pretty self explanitory name
    
    static MousePosUpdater()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView) // Called every frame
    {
        if (TilemapContext.selectedTool == null) 
            return;
        
        if (EditorWindow.mouseOverWindow is not SceneView)
            return;
        
        Event e = Event.current;
        if (e == null) return;
        
        UpdateMouseHoverPos(e, sceneView);
    }

    // Finds the point along a line (from start to end) where the Y coordinate equals targetY
    private static Vector3 FindPointAtY(Vector3 start, Vector3 end, float targetY)
    {
        Vector3 direction = end - start;

        // Avoid division by zero if direction is flat in Y
        if (Mathf.Approximately(direction.y, 0f)) return Vector3.zero;

        // Solve for t in the line equation: start + direction * t
        float t = (targetY - start.y) / direction.y;

        // Return the interpolated point at the target Y height
        return start + direction * t;
    }

    // Updates the mouse hover tile position in the scene view based on mouse position and camera raycasting
    private static void UpdateMouseHoverPos(Event e, SceneView sceneView)
    {
        // Get the scene view camera used for editor navigation
        Camera cam = sceneView.camera;
        if (!cam) return;

        // Get mouse position from the event
        Vector2 mousePosition = e.mousePosition;

        // Invert Y because GUI coordinates start at top-left, screen coords start at bottom-left
        mousePosition.y = cam.pixelHeight - mousePosition.y;

        // Convert the screen-space mouse position into a world-space ray
        Ray ray = cam.ScreenPointToRay(mousePosition);

        // The Y-plane we want to intersect with (e.g. the tilemap floor height)
        float targetY = TilemapContext.yValue;

        // Find where the ray intersects that Y-plane
        Vector3 intersection = FindPointAtY(ray.origin, ray.origin + ray.direction * 1000f, targetY);

        // Convert the intersection point into tile coordinates by rounding and dividing by tile size
        int roundedX = Mathf.RoundToInt(intersection.x / TilemapContext.tileSize.x);
        int roundedZ = Mathf.RoundToInt(intersection.z / TilemapContext.tileSize.x);
        int roundedY = Mathf.RoundToInt(targetY);

        // Store the hover position in the context
        TilemapContext.mouseHoverPos = new Vector3Int(roundedX, roundedY, roundedZ);

        // Ask Unity to repaint the scene view so the hover effect is visible
        HandleUtility.Repaint();
    }
}