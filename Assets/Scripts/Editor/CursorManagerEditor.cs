using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CursorManager))]
public class CursorManagerEditor : Editor
{
    private SerializedProperty _arrowOffset;
    private SerializedProperty _handOffset;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _arrowOffset = serializedObject.FindProperty("_arrowOffset");
        _handOffset = serializedObject.FindProperty("_handOffset");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/CursorManager"); // Adjust the path as necessary
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw logo
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Add flexible space on the left
        if (logoTexture != null)
        {
            GUILayout.Label(logoTexture, GUILayout.Width(200), GUILayout.Height(50)); // Set the size of the logo
        }
        else
        {
            GUILayout.Label("Logo not found!"); // Fallback if logo is not found
        }

        GUILayout.FlexibleSpace(); // Add flexible space on the right
        GUILayout.EndHorizontal();

        // Custom inspector layout
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.BeginVertical("box");
        GUILayout.Label("Custom Mouse", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_arrowOffset, new GUIContent("Arrow Offset"));
        EditorGUILayout.PropertyField(_handOffset, new GUIContent("Hand Offset"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
