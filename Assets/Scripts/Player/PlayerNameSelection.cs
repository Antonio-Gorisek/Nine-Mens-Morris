using UnityEngine;
using TMPro;

/// <summary>
/// Manages player name input and persistence using Unity's PlayerPrefs.
/// Allows the user to input or update player names, which are saved locally.
/// </summary>
[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.bfbvyq88fm3r")]
public class PlayerNameSelection : MonoBehaviour
{
    [SerializeField] private TMP_InputField _player1Name;
    [SerializeField] private TMP_InputField _player2Name;

    private void Start()
    {
        LoadData(); // Load player names from PlayerPrefs at start

        // Add listeners to save data when either input field value changes
        _player1Name.onValueChanged.AddListener(value => SaveData());
        _player2Name.onValueChanged.AddListener(value => SaveData());
    }

    /// <summary>
    /// Loads player names from PlayerPrefs. Sets default values if no names are found.
    /// If the loaded name is "Player1" or "Player2" (default values), clears the input fields.
    /// </summary>
    private void LoadData()
    {
        string p1Name = PlayerPrefs.GetString("PlayerName_1", "Player1");
        string p2Name = PlayerPrefs.GetString("PlayerName_2", "Player2");

        // If names are the default, set them as empty in the input fields
        _player1Name.text = p1Name == "Player1" ? string.Empty : p1Name;
        _player2Name.text = p2Name == "Player2" ? string.Empty : p2Name;
    }

    /// <summary>
    /// Saves the current names from input fields to PlayerPrefs.
    /// If the input field is empty, saves "Player1" or "Player2" as default names.
    /// </summary>
    private void SaveData()
    {
        PlayerPrefs.SetString("PlayerName_1", string.IsNullOrWhiteSpace(_player1Name.text) ? "Player1" : _player1Name.text);
        PlayerPrefs.SetString("PlayerName_2", string.IsNullOrWhiteSpace(_player2Name.text) ? "Player2" : _player2Name.text);
    }
}
