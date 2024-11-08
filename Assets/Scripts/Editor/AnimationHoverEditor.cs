using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationHover))]
public class AnimationHoverEditor : Editor
{
    private SerializedProperty _hoverSound;
    private SerializedProperty _zoomFactor;
    private SerializedProperty _zoomDuration;
    private SerializedProperty _rotationAngle;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _hoverSound = serializedObject.FindProperty("_hoverSound");
        _zoomFactor = serializedObject.FindProperty("_zoomFactor");
        _zoomDuration = serializedObject.FindProperty("_zoomDuration");
        _rotationAngle = serializedObject.FindProperty("_rotationAngle");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/AnimationHover"); // Adjust the path as necessary
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
        GUILayout.Label("Settings", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_hoverSound, new GUIContent("Hover Sound Effect"));
        EditorGUILayout.PropertyField(_zoomFactor, new GUIContent("Zoom Factor"));
        EditorGUILayout.PropertyField(_zoomDuration, new GUIContent("Zoom Duration"));
        EditorGUILayout.PropertyField(_rotationAngle, new GUIContent("Rotation Angle"));
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }


}
