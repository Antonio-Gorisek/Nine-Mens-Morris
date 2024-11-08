using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _sliderInfo;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_InputField _p1Name;
    [SerializeField] private TMP_InputField _p2Name;
    [SerializeField] private Button _exitBtn;

    private void Awake()
    {
        // Adds listeners for input changes, saving the player names when they change
        _p1Name.onValueChanged.AddListener((value) => SavePlayerName(1, value));
        _p2Name.onValueChanged.AddListener((value) => SavePlayerName(2, value));

        // Adds listener to update the number of rings when the slider value changes
        _slider.onValueChanged.AddListener((value) => NumberOfRings((int)value));

        // Adds listener to the exit button to close the settings menu
        _exitBtn.onClick.AddListener(ExitSettings);
    }

    private void Start()
    {
        // Set initial values based on saved PlayerPrefs
        _slider.value = PlayerPrefs.GetInt("Rings", 3); // Default value of 3 if not found
        _p1Name.text = PlayerPrefs.GetString("Player1_Name", "Player1"); // Default name 'Player1' if not found
        _p2Name.text = PlayerPrefs.GetString("Player2_Name", "Player2"); // Default name 'Player2' if not found
    }

    /// <summary>
    /// Updates the number of rings setting and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="value">The number of rings selected by the slider.</param>
    private void NumberOfRings(int value)
    {
        PlayerPrefs.SetInt("Rings", value);
        _sliderInfo.text = $"Rings: {value}";
    }

    /// <summary>
    /// Saves the player's name to PlayerPrefs when the input field value changes.
    /// </summary>
    /// <param name="player">The player number (1 or 2).</param>
    /// <param name="text">The name of the player entered in the input field.</param>
    private void SavePlayerName(int player, string text) => PlayerPrefs.SetString($"Player{player}_Name", text);

    /// <summary>
    /// Closes the settings menu by destroying this GameObject.
    /// </summary>
    private void ExitSettings() => Destroy(this.gameObject);

}
