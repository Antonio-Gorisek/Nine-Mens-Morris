using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private SerializedProperty _useEditorSettings;
    private SerializedProperty _numberOfRings;
    private SerializedProperty _player1Name;
    private SerializedProperty _player2Name;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _useEditorSettings = serializedObject.FindProperty("_useEditorSettings");
        _numberOfRings = serializedObject.FindProperty("_numberOfRings");
        _player1Name = serializedObject.FindProperty("_player1Name");
        _player2Name = serializedObject.FindProperty("_player2Name");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/GameManager"); // Adjust the path as necessary
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
        GUILayout.Label("Editor Settings", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_useEditorSettings, new GUIContent("Use editor settings"));
        EditorGUILayout.PropertyField(_numberOfRings, new GUIContent("Map Size (Rings Number)"));
        EditorGUILayout.PropertyField(_player1Name, new GUIContent("Name of Player 1"));
        EditorGUILayout.PropertyField(_player2Name, new GUIContent("Name of Player 2"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
