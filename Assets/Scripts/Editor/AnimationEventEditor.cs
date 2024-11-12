using UnityEngine;
using UnityEditor;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.58jq7mvme1tl")]
[CustomEditor(typeof(AnimationEvent))]
public class AnimationEventEditor : Editor
{
    private SerializedProperty _sound;
    private SerializedProperty _audioVolume;
    private SerializedProperty _minPitch;
    private SerializedProperty _maxPitch;


    private SerializedProperty _animEvent;

    private Texture2D logoTexture;

    private void OnEnable()
    {
        // Fetch the properties
        _sound = serializedObject.FindProperty("_sound");
        _audioVolume = serializedObject.FindProperty("_audioVolume");
        _minPitch = serializedObject.FindProperty("_minPitch");

        _maxPitch = serializedObject.FindProperty("_maxPitch");
        _animEvent = serializedObject.FindProperty("_animEvent");

        // Load the logo texture from the Resources folder
        logoTexture = Resources.Load<Texture2D>("Editor/Images/AnimationEvent"); // Adjust the path as necessary
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
        GUILayout.Label("Animation Settings", headerStyle);
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(_sound, new GUIContent("Sound Effect"));
        EditorGUILayout.PropertyField(_audioVolume, new GUIContent("Audio Volume"));
        EditorGUILayout.PropertyField(_minPitch, new GUIContent("Minimum pitch"));
        EditorGUILayout.PropertyField(_maxPitch, new GUIContent("Maximum pitch"));
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Event", headerStyle);
        EditorGUILayout.PropertyField(_animEvent, new GUIContent("Animation Event"));
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}
