using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ToolManager
{
    static ITool _selectedTool;

    static ToolManager()
    {
        SceneView.duringSceneGui += MyUpdate;
    }

    static void MyUpdate(SceneView sceneView)
    {
        UpdateTool();

        if (_selectedTool == null)
            return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Debug.Log($"Clicked with tool: {_selectedTool.GetType().Name}");
            _selectedTool.OnClick();
            e.Use();
        }
    }

    static void UpdateTool()
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
    void OnSelected();
    void OnClick();
    void OnDeselected();
}
