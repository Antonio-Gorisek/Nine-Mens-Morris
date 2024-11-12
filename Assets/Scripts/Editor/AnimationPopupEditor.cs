using UnityEngine;
using UnityEditor;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.58jq7mvme1tl")]
[CustomEditor(typeof(AnimationPopup))]
public class AnimationPopupEditor : Editor
{
    private SerializedProperty _sounds;
    private SerializedProperty _popupCurve;
    private SerializedProperty _hideCurve;

    private SerializedProperty _targetScale;
    private SerializedProperty _speed;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _sounds = serializedObject.FindProperty("_sounds");
        _popupCurve = serializedObject.FindProperty("_popupCurve");
        _hideCurve = serializedObject.FindProperty("_hideCurve");

        _targetScale = serializedObject.FindProperty("_targetScale");
        _speed = serializedObject.FindProperty("_speed");

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
        GUILayout.Label("Sound Effect", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_sounds, new GUIContent("Sound"));
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Animation Cruves", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_popupCurve, new GUIContent("Popup Curve"));
        EditorGUILayout.PropertyField(_hideCurve, new GUIContent("Hide Curve"));
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Object Scaling & Speed", headerStyle);
        EditorGUILayout.PropertyField(_targetScale, new GUIContent("Transform Scale"));
        EditorGUILayout.PropertyField(_speed, new GUIContent("Animation Speed"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
