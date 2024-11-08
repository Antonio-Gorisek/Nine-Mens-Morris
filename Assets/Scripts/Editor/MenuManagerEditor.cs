using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuManager))]
public class MenuManagerEditor : Editor
{
    private SerializedProperty _startGame;
    private SerializedProperty _settings;
    private SerializedProperty _quit;

    private SerializedProperty _gameUI;
    private SerializedProperty _menuUI;
    private SerializedProperty _canvas;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _startGame = serializedObject.FindProperty("_startGame");
        _settings = serializedObject.FindProperty("_settings");
        _quit = serializedObject.FindProperty("_quit");

        _gameUI = serializedObject.FindProperty("_gameUI");
        _menuUI = serializedObject.FindProperty("_menuUI");
        _canvas = serializedObject.FindProperty("_canvas");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/MenuManager"); // Adjust the path as necessary
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
        GUILayout.Label("Main Menu", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_startGame, new GUIContent("Play Button"));
        EditorGUILayout.PropertyField(_settings, new GUIContent("Settings Button"));
        EditorGUILayout.PropertyField(_quit, new GUIContent("Quit Button"));
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Transform", headerStyle);
        EditorGUILayout.PropertyField(_gameUI, new GUIContent("Game Transform"));
        EditorGUILayout.PropertyField(_menuUI, new GUIContent("Menu Transform"));
        EditorGUILayout.PropertyField(_canvas, new GUIContent("Canvas"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
