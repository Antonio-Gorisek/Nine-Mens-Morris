using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_Text _sliderInfo;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_InputField _p1Name;
    [SerializeField] private TMP_InputField _p2Name;
    [SerializeField] private Button _exitBtn;

    private void Awake()
    {
        _p1Name.onValueChanged.AddListener((value) => SavePlayerName(1, value));
        _p2Name.onValueChanged.AddListener((value) => SavePlayerName(2, value));

        _slider.onValueChanged.AddListener((value) => NumberOfRings((int)value));
        _exitBtn.onClick.AddListener(ExitSettings);
    }

    private void Start()
    {
        _slider.value = PlayerPrefs.GetInt("Rings");
        _p1Name.text = PlayerPrefs.GetString("Player1_Name");
        _p2Name.text = PlayerPrefs.GetString("Player2_Name");
    }

    private void NumberOfRings(int value)
    {
        Debug.Log(value);
        PlayerPrefs.SetInt("Rings", value);
        _sliderInfo.text = $"Rings: {value}";
    }

    private void SavePlayerName(int player, string text)
    {
        PlayerPrefs.SetString($"Player{player}_Name", text);
        PlayerPrefs.SetString($"Player{player}_Name", text);
    }

    private void ExitSettings()
    {
        Destroy(this.gameObject);
    }

}
