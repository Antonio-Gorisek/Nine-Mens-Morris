using UnityEngine;
using UnityEditor;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.58jq7mvme1tl")]
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    private SerializedProperty _camera;
    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _camera = serializedObject.FindProperty("_camera");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/PlayerController"); // Adjust the path as necessary
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
        GUILayout.Label("Main Camera", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_camera, new GUIContent("Main Camera"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
