using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.4ef175agafus")]
public class PlayerController : Singleton<PlayerController>
{
    private GameObject _pauseMenu;
    private AnimationPopup _pauseMenuAnimationPopup;
    private GameOver _pauseMenuGameOver;

    [SerializeField] private Camera _camera;

    [SerializeField] private Image[] _playersPieces;
    [SerializeField] private TMP_Text[] _playersPiecesCount;


    private bool _removeOpponentPiece;

    public void OpponentPieceRemoved() => _removeOpponentPiece = false;
    public void RemoveOpponentPiece() => _removeOpponentPiece = true;

    public void SetPlayerUIStats(Player[] players)
    {
        _playersPieces[0].sprite = players[0].piecePrefab.GetComponent<SpriteRenderer>().sprite;
        _playersPieces[1].sprite = players[1].piecePrefab.GetComponent<SpriteRenderer>().sprite;

        _playersPiecesCount[0].text = "x" + players[0].remainingPieces;
        _playersPiecesCount[1].text = "x" + players[1].remainingPieces;
    }

    // Handles player input when clicking the mouse
    public void HandleInput(PieceController piece)
    {
        if (Input.GetMouseButtonDown(0) && _pauseMenu == null)
        {
            // Get the world position corresponding to the mouse position on screen
            Vector3 worldPosition = GetMouseWorldPosition();

            // If the _removeOpponentPiece is true, remove the opponent's piece at the mouse position
            if (_removeOpponentPiece)
                piece.RemoveOpponentPiece(worldPosition);
            else
                // Otherwise, select or place a piece at the mouse position
                piece.SelectOrPlacePiece(worldPosition);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
        {
            TogglePauseMenu();
        }
    }

    /// <summary>
    /// Gets the mouse position in world coordinates.
    /// </summary>
    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        return _camera.ScreenToWorldPoint(mousePosition);
    }

    /// <summary>
    /// Toggles the pause menu visibility.
    /// </summary>
    private void TogglePauseMenu()
    {
        if (MenuManager.Instance._transitionObj || GameManager.Instance._gameOverObj != null)
            return;

        if (_pauseMenu != null)
        {
            _pauseMenuAnimationPopup.HidePopup(true);
            return;
        }

        _pauseMenu = Instantiate(Resources.Load<GameObject>("Menu/Popup/Pause"));

        _pauseMenuAnimationPopup = _pauseMenu.GetComponentInChildren<AnimationPopup>();
        _pauseMenuGameOver = _pauseMenu.GetComponentInChildren<GameOver>();

        _pauseMenu.GetComponent<Canvas>().worldCamera = _camera;

        _pauseMenuGameOver.SetBackToMenuButtonEvent(() => MenuManager.Instance.LoadMainMenu());
        _pauseMenuGameOver.SetRestartButtonEvent(() => MenuManager.Instance.RestartCurrentLevel());

        _pauseMenuAnimationPopup.ShowPopup();
    }
}
