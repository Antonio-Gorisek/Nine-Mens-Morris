using TMPro;
using UnityEngine;

public class Info : Singleton<Info>
{
    [SerializeField] private TMP_Text _txtInfo;

    /// <summary>
    /// Displays the given message on the screen.
    /// </summary>
    /// <param name="msg">The message to display in the info text box.</param>
    public void Message(string msg) => _txtInfo.text = msg;
}
