using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Camera _camera;

    private void Awake() => _camera = Camera.main;

    public void HandleInput(PieceController piece)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            piece.SelectOrPlacePiece(worldPosition);
        }
    }

    /// <summary>
    /// Gets the mouse position in world coordinates.
    /// </summary>
    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        return _camera.ScreenToWorldPoint(mousePosition);
    }

}

