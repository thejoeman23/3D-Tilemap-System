using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ToolManager
{
    // Current selected tool. Updates with Tilemap's CurrentSelectedTool.
    static ITool _selectedTool;

    static ToolManager()
    {
        SceneView.duringSceneGui += MyUpdate;
    }

    static void MyUpdate(SceneView sceneView) // My Update() (runs every frame)
    {
        SyncTool();

        if (_selectedTool == null)
            return;

        Event e = Event.current;

        // If left click is pressed (e.button == 0 is left click) then run OnClick for selected tool
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Debug.Log($"Clicked with tool: {_selectedTool.GetType().Name}");
            _selectedTool.OnClick();
            e.Use(); // Makes sure nothing else uses that click
        }
    }

    // Syncs our tool varibale with TilemapContext.cs's variable
    static void SyncTool()
    {
        var newTool = TilemapContext.selectedTool;

        if (_selectedTool == newTool)
            return;

        _selectedTool?.OnDeselected();
        _selectedTool = newTool;
        _selectedTool?.OnSelected();
    }
}


public interface ITool
{
    // Interface for all tools
    
    void OnSelected();
    void OnClick();
    void OnDeselected();
}
