using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Button _btnRestart;
    [SerializeField] private Button _btnBackToMenu;
    [SerializeField] private TMP_Text _txtWinner;

    public void SetRestartButtonEvent(Action onClickAction)
         => _btnRestart.onClick.AddListener(() => onClickAction?.Invoke());

    public void SetBackToMenuButtonEvent(Action onClickAction)
        => _btnBackToMenu.onClick.AddListener(() => onClickAction?.Invoke());

    public void SetWinnerText(string winnerName)
        => _txtWinner.text = $"CONGRATULATIONS\r\nPLAYER\r\n<color=yellow>{winnerName}</color> WON!";
}
