using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.6le53bu273f9")]
public class PlayerColorSelection : MonoBehaviour
{
    [SerializeField] private Transform _Player1; // Reference to the transform that holds Player 1 color options
    [SerializeField] private Transform _Player2; // Reference to the transform that holds Player 2 color options

    // List of toggle buttons for color selection for both players
    private List<Toggle> _colorsPlayer1 = new List<Toggle>();
    private List<Toggle> _colorsPlayer2 = new List<Toggle>();

    // Flag to prevent multiple audio effects when loading colors
    bool disableAudioEffect;

    void Start()
    {
        SaveDefaultColors(); // Save default color selections if not already saved
        GetAllChildColors(); // Collect all color toggles for both players
        AddToggleEvents(); // Add listeners to each color toggle
        LoadSavedColors(); // Load saved color preferences from PlayerPrefs
    }

    // Collects all the color toggle GameObjects for Player 1 and Player 2
    private void GetAllChildColors()
    {
        // Find all color toggle buttons under Player 1
        foreach (Transform child in _Player1)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null) _colorsPlayer1.Add(toggle); // Add to the list if it's a Toggle
        }

        // Find all color toggle buttons under Player 2
        foreach (Transform child in _Player2)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null) _colorsPlayer2.Add(toggle); // Add to the list if it's a Toggle
        }
    }

    /// <summary>
    /// Saves the default color selections for both players to PlayerPrefs.
    /// </summary>
    private void SaveDefaultColors()
    {
        // Save default color for Player 1 if not already saved
        if (!PlayerPrefs.HasKey("PlayerColor_1"))
            PlayerPrefs.SetString("PlayerColor_1", _colorsPlayer1[0].name);

        // Save default color for Player 2 if not already saved
        if (!PlayerPrefs.HasKey("PlayerColor_2"))
            PlayerPrefs.SetString("PlayerColor_2", _colorsPlayer2[1].name);
    }

    /// <summary>
    /// Loads the saved color selections for both players from PlayerPrefs and sets the corresponding toggles to "on".
    /// </summary>
    private void LoadSavedColors()
    {
        // Load the saved color for Player 1, use default if not found
        string playerColor1 = PlayerPrefs.GetString("PlayerColor_1", _colorsPlayer1[0].name);

        // Load the saved color for Player 2, use default if not found
        string playerColor2 = PlayerPrefs.GetString("PlayerColor_2", _colorsPlayer2[1].name);

        // Set the toggle for Player 1 based on saved color
        for (int i = 0; i < _colorsPlayer1.Count; i++)
        {
            if (_colorsPlayer1[i].name == playerColor1)
            {
                disableAudioEffect = true; // Disable audio effect while loading
                _colorsPlayer1[i].isOn = true; // Set the toggle to "on"
                _colorsPlayer1[i].onValueChanged.Invoke(true); // Trigger the toggle event (this plays the audio effect)
                break;
            }
        }

        // Set the toggle for Player 2 based on saved color
        for (int i = 0; i < _colorsPlayer2.Count; i++)
        {
            if (_colorsPlayer2[i].name == playerColor2)
            {
                disableAudioEffect = true; // Disable audio effect while loading
                _colorsPlayer2[i].isOn = true; // Set the toggle to "on"
                _colorsPlayer2[i].onValueChanged.Invoke(true); // Trigger the toggle event (this plays the audio effect)
                break;
            }
        }

        disableAudioEffect = false; // Re-enable audio effects after loading
    }

    // Adds listeners to each toggle for color selection and updates PlayerPrefs when the color is changed
    private void AddToggleEvents()
    {        for (int i = 0; i < _colorsPlayer1.Count; i++)
        {
            int index = i;
            _colorsPlayer1[i].onValueChanged.AddListener((value) =>
            {
                // Set outline and disable toggles of Player 2 when a color is selected for Player 1
                SetOutlineAndDisable(_colorsPlayer1, _colorsPlayer2, index, value);
                PlayerPrefs.SetString("PlayerColor_1", _colorsPlayer1[index].name); // Save the selected color
            });

            // Add listener for Player 2's color selection toggles
            _colorsPlayer2[i].onValueChanged.AddListener((value) =>
            {
                // Set outline and disable toggles of Player 1 when a color is selected for Player 2
                SetOutlineAndDisable(_colorsPlayer2, _colorsPlayer1, index, value);
                PlayerPrefs.SetString("PlayerColor_2", _colorsPlayer2[index].name); // Save the selected color
            });
        }
    }

    // Sets the outline for the selected color and disables the corresponding toggle for the other player
    private void SetOutlineAndDisable(List<Toggle> activePlayerColors, List<Toggle> otherPlayerColors, int index, bool isActive)
    {
        Toggle selectedToggle = activePlayerColors[index]; // Get the selected toggle
        Outline outline = selectedToggle.GetComponent<Outline>() ?? selectedToggle.gameObject.AddComponent<Outline>();
        outline.enabled = isActive; // Enable or disable outline based on selection

        // Disable the corresponding toggle for the other player
        otherPlayerColors[index].interactable = !isActive;

        PlayAudioEffect(isActive);
    }

    // Plays an audio effect if a color toggle is selected
    private void PlayAudioEffect(bool isActive)
    {
        // Avoid playing the sound if the effect is disabled or the toggle is not active
        if (!isActive || disableAudioEffect) return;

        // Play a sound effect with random pitch variation
        AudioManager.PlayFromResources(Sound.pop, 0.3f, Random.Range(0.8f, 1.2f));
    }
}
