using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private Camera _camera;

    private bool _removeOpponentPiece;

    public void OpponentPieceRemoved() => _removeOpponentPiece = false;
    public void RemoveOpponentPiece() => _removeOpponentPiece = true;

    // Handles player input when clicking the mouse
    public void HandleInput(PieceController piece)
    {
        if (Input.GetMouseButtonDown(0))
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
