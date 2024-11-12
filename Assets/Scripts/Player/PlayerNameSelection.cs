using UnityEngine;
using TMPro;

/// <summary>
/// Manages player name input and persistence using Unity's PlayerPrefs.
/// Allows players to input or update their names, and saves them locally for future use.
/// </summary>
[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.bfbvyq88fm3r")]
public class PlayerNameSelection : MonoBehaviour
{
    [SerializeField] private TMP_InputField _player1Name;
    [SerializeField] private TMP_InputField _player2Name;

    private const string DefaultName1 = "Player1";
    private const string DefaultName2 = "Player2";

    private void Start()
    {
        LoadPlayerNames();
        AddInputListeners();
    }

    /// <summary>
    /// Loads player names from PlayerPrefs, or sets default names if no saved data is found.
    /// If both players have the same name, adjusts them to avoid conflict.
    /// </summary>
    private void LoadPlayerNames()
    {
        string player1Name = PlayerPrefs.GetString("PlayerName_1", DefaultName1);
        string player2Name = PlayerPrefs.GetString("PlayerName_2", DefaultName2);

        // Prevent the same name for both players
        if (player1Name == player2Name)
        {
            player2Name += "2";
            PlayerPrefs.SetString("PlayerName_2", player2Name);
        }

        // Set the names in the input fields
        _player1Name.text = player1Name;
        _player2Name.text = player2Name;
    }

    /// <summary>
    /// Saves the current player names from input fields to PlayerPrefs.
    /// If names are the same, appends a suffix to ensure uniqueness.
    /// </summary>
    private void SavePlayerNames()
    {
        string player1Name = _player1Name.text.Trim();
        string player2Name = _player2Name.text.Trim();

        // If player name is empty or contains only spaces, set default name
        if (string.IsNullOrEmpty(player1Name))
            player1Name = DefaultName1;

        if (string.IsNullOrEmpty(player2Name))
            player2Name = DefaultName2;

        // Ensure unique names for both players
        if (player1Name == player2Name)
        {
            player2Name += "2";
            _player2Name.text = player2Name;
        }

        PlayerPrefs.SetString("PlayerName_1", player1Name);
        PlayerPrefs.SetString("PlayerName_2", player2Name);
    }

    /// <summary>
    /// Adds listeners to input fields to automatically save data when the user changes the input.
    /// </summary>
    private void AddInputListeners()
    {
        _player1Name.onValueChanged.AddListener(value => SavePlayerNames());
        _player2Name.onValueChanged.AddListener(value => SavePlayerNames());
    }
}
