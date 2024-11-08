using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MouseHover))]
public class MouseHoverEditor : Editor
{
    private SerializedProperty OnMouseEnterEvent;
    private SerializedProperty OnMouseExitEvent;
    private SerializedProperty OnMouseClickEvent;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        OnMouseEnterEvent = serializedObject.FindProperty("OnMouseEnterEvent");
        OnMouseExitEvent = serializedObject.FindProperty("OnMouseExitEvent");
        OnMouseClickEvent = serializedObject.FindProperty("OnMouseClickEvent");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/MouseHover"); // Adjust the path as necessary
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
        GUILayout.Label("On Mouse Enter", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(OnMouseEnterEvent, new GUIContent("Event"));
        GUILayout.EndVertical();


        GUILayout.Space(20);
        GUILayout.BeginVertical("box");
        GUILayout.Label("On Mouse Exit", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(OnMouseExitEvent, new GUIContent("Event"));
        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.BeginVertical("box");
        GUILayout.Label("On Mouse Click", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(OnMouseClickEvent, new GUIContent("Event"));
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }


}