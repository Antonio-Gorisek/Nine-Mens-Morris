using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Camera _camera;
    private bool _removeOpponentPiece;

    private void Awake() => _camera = Camera.main;
    public void OpponentPieceRemoved() => _removeOpponentPiece = false;
    public void RemoveOpponentPiece() => _removeOpponentPiece = true;

    public void HandleInput(PieceController piece)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = GetMouseWorldPosition();

            if (_removeOpponentPiece)
                piece.RemoveOpponentPiece(worldPosition);
            else
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

