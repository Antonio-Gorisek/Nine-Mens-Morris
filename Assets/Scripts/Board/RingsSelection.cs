using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RingsSelection : MonoBehaviour
{
    [SerializeField] private TMP_Text _sliderInfo;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        // Initialize and load saved data for the number of rings
        SaveDefaultData();
        LoadRingsData();

        // Add listener to slider to handle changes in value
        _slider.onValueChanged.AddListener((value) => {
            UpdateInfo(); // Update the displayed information when the slider value changes
            SaveRingsData(); // Save the updated number of rings to PlayerPrefs
            AudioManager.PlayFromResources(Sounds.pop, 0.2f, 1.4f);
        });
    }

    /// <summary>
    /// Saves default data for the number of rings if it hasn't been saved yet.
    /// </summary>
    private void SaveDefaultData()
    {
        if (!PlayerPrefs.HasKey("Rings"))
            PlayerPrefs.SetInt("Rings", 3);
    }

    /// <summary>
    /// Loads the saved number of rings from PlayerPrefs and updates the slider.
    /// </summary>
    private void LoadRingsData()
    {
        _slider.value = PlayerPrefs.GetInt("Rings");
        UpdateInfo(); // Update the displayed text to reflect the current slider value
    }

    /// <summary>
    /// Updates the displayed information showing the number of rings selected.
    /// </summary>
    private void UpdateInfo() => _sliderInfo.text = string.Format("RINGS: {0}", _slider.value);

    /// <summary>
    /// Saves the current number of rings to PlayerPrefs.
    /// </summary>
    private void SaveRingsData() => PlayerPrefs.SetInt("Rings", (int)_slider.value);
}