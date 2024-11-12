using TMPro;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.3vvlv7its9kr")]
public class Info : Singleton<Info>
{
    [SerializeField] private TMP_Text _txtInfo;

    /// <summary>
    /// Displays the given message on the screen.
    /// </summary>
    /// <param name="msg">The message to display in the info text box.</param>
    public void Message(string msg) => _txtInfo.text = msg;
}
