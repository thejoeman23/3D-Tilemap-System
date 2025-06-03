#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TilePalette))]
public class TilePaletteEditor : Editor
{
    private SerializedProperty tilesProp;

    ReorderableList tilesList;

    void OnEnable()
    {
        tilesProp = serializedObject.FindProperty("tiles");

        tilesList = new ReorderableList(serializedObject, tilesProp, true, false, true, true);
        tilesList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            EditorGUI.PropertyField(rect, tilesProp.GetArrayElementAtIndex(index), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        float previewWidth = EditorGUIUtility.currentViewWidth / 4;

        EditorGUILayout.LabelField("Tiles", EditorStyles.boldLabel);

        int size = tilesProp.arraySize;
        if (tilesProp.arraySize != size) tilesProp.arraySize = size;

        // Draw lists side by side
        for (int i = 0; i < size; i++)
        {
            SerializedProperty tilePrefabEntry = tilesList.serializedProperty.GetArrayElementAtIndex(i);
            SerializedProperty tileLabel = tilePrefabEntry.FindPropertyRelative("label");
            SerializedProperty tilePrefab = tilePrefabEntry.FindPropertyRelative("prefab");
            SerializedProperty tileType = tilePrefabEntry.FindPropertyRelative("type");
            
            EditorGUILayout.BeginHorizontal();

            #region TilePreview
            EditorGUILayout.BeginVertical();
            
            Texture2D preview = AssetPreview.GetAssetPreview(tilePrefab.objectReferenceValue as UnityEngine.Object);
            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(previewWidth), GUILayout.Height(previewWidth));
            }
            else
            {
                EditorGUILayout.LabelField("Loading preview...");
                Repaint();
            }
            
            EditorGUILayout.EndVertical();
            #endregion

            #region TileVariables
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField(tilePrefab, GUIContent.none);
            EditorGUILayout.PropertyField(tileLabel, GUIContent.none);
            EditorGUILayout.PropertyField(tileType, GUIContent.none);
            
            #endregion

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.25f, 0.15f); // vivid red
            
            if (GUILayout.Button("Remove Tile"))
            {
                tilesProp.DeleteArrayElementAtIndex(i);
                break;
            }
            
            GUI.backgroundColor = oldColor;
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("+"))
        {
            tilesProp.InsertArrayElementAtIndex(tilesProp.arraySize);
        }

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif