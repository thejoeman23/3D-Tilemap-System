using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TilePlacer : MonoBehaviour
{
    private void Update()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (!sceneView) return;

        Camera cam = sceneView.camera;
        Vector3 camPosition = cam.transform.position;

        // Get mouse position in scene view (from GUI space to world)
        Vector2 mousePosition = Event.current != null ? Event.current.mousePosition : Vector2.zero;
        mousePosition.y = sceneView.position.height - mousePosition.y;

        // Get world position under the mouse (just a point on the near clip plane)
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));

        // Draw a line from camera to mouseWorld, and extend it to y = TilemapContext.yValue
        Vector3 intersection = FindPointAtY(camPosition, mouseWorld, TilemapContext.yValue);

        // Snap to grid
        int roundedX = Mathf.RoundToInt(intersection.x);
        int roundedZ = Mathf.RoundToInt(intersection.z);
        int roundedY = Mathf.RoundToInt(TilemapContext.yValue);
        TilemapContext.mouseHoverPos = new Vector3Int(roundedX, roundedY, roundedZ);
    }

    Vector3 FindPointAtY(Vector3 start, Vector3 end, float targetY)
    {
        Vector3 direction = end - start;
        if (Mathf.Approximately(direction.y, 0f))
        {
            Debug.LogWarning("Direction is parallel to plane.");
            return Vector3.zero;
        }

        float t = (targetY - start.y) / direction.y;
        return start + direction * t;
    }
}