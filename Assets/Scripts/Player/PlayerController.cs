using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private Camera _camera;

    private bool _removeOpponentPiece;

    private GameObject _pauseMenu;
    private AnimationPopup _pauseMenuAnimationPopup;
    private GameOver _pauseMenuGameOver;


    public void OpponentPieceRemoved() => _removeOpponentPiece = false;
    public void RemoveOpponentPiece() => _removeOpponentPiece = true;

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
            if (_pauseMenu != null) {
                _pauseMenuAnimationPopup.HidePopup(true);
                return;
            }

            _pauseMenu = Instantiate(Resources.Load<GameObject>("Menu/Popup/Pause"));

            _pauseMenuAnimationPopup = _pauseMenu.GetComponentInChildren<AnimationPopup>();
            _pauseMenuGameOver = _pauseMenu.GetComponentInChildren<GameOver>();

            _pauseMenuGameOver.SetBackToMenuButtonEvent(() => MenuManager.Instance.LoadMainMenu());
            _pauseMenuGameOver.SetRestartButtonEvent(() => MenuManager.Instance.RestartCurrentLevel());

            _pauseMenuAnimationPopup.ShowPopup();
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
}
