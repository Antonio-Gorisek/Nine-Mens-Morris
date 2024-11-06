using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement
{
    private readonly List<Vector3> positionOfSpots = new List<Vector3>();
    private readonly PieceMillDetector lineDetector;
    private readonly int _numberOfRings;
    private bool _pieceMoving;

    public PieceMovement(PieceMillDetector lineDetector, List<Vector3> positionOfSpots, int _numberOfRings)
    {
        this.lineDetector = lineDetector;
        this.positionOfSpots = positionOfSpots;
        this._numberOfRings = _numberOfRings;
    }

    /// <summary>
    /// Moves the selected piece to a new position if the path is clear.
    /// </summary>
    public void MoveSelectedPiece(GameObject selectedPiece, Vector3 newPosition, HashSet<Vector3> occupiedPositions, System.Action onMillDetected, System.Action onSwitchPlayer, Player player)
    {
        // Check if a piece is already moving, if so, prevent any new movement
        if (_pieceMoving)
            return;

        if (IsPathClear(selectedPiece.transform.position, newPosition, player))
        {
            GameManager.Instance.StartCoroutine(AnimateMovement(selectedPiece, newPosition, occupiedPositions, onMillDetected, onSwitchPlayer));
        }
        else
        {
            Debug.Log("Path is blocked or invalid.");
        }
    }

    /// <summary>
    /// Enables the selection visual for the specified piece, only if no piece is moving.
    /// </summary>
    public void EnablePieceSelection(GameObject piece, Player player)
    {
        // Prevent selection if a piece is already moving
        if (_pieceMoving)
            return;

        piece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        Debug.Log($"{player.playerName} selected their piece.");
    }

    /// <summary>
    /// Checks if the path from start to end is clear for movement.
    /// </summary>
    private bool IsPathClear(Vector3 start, Vector3 end, Player player)
    {
        if (player.piecesOnBoard + player.remainingPieces == 3)
        {
            return true;
        }

        Vector3 direction = end - start; // Calculate movement direction
        float distance = direction.magnitude; // Get the distance

        Debug.Log($"Checking path from {start} to {end}");

        // Check for diagonal movement
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0)
        {
            // Allow diagonal movement only if moving from or to the center
            if ((start == Vector3.zero && positionOfSpots.Contains(end)) || (end == Vector3.zero && positionOfSpots.Contains(start)))
            {
                return true; // Diagonal movement is valid
            }
            else
            {
                Debug.Log("Diagonal movement is not allowed."); // Log if diagonal movement is invalid
                return false;
            }
        }

        // Prevent movement towards the center (0, 0, 0)
        if (_numberOfRings > 1 && IsPlayerCrossingCenter(start, end))
        {
            Debug.Log("Path is blocked or invalid.");
            return false;
        }

        // Check for collisions along the path
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction.normalized, distance);
        int spotCount = 0; // Counter for spots hit

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Spot"))
            {
                spotCount++;
                if (hit.collider.transform.position == end)
                    continue; // Ignore the end position

                if (spotCount > 1) // More than one spot hit indicates a blockage
                {
                    Debug.Log("Path blocked by multiple spots");
                    return false;
                }
            }
        }

        Debug.Log("Path is clear");
        return true; // Path is clear
    }

    public bool IsPlayerCrossingCenter(Vector3 A, Vector3 B)
    {
        // Check if A and B are on opposite sides of the origin (0, 0) on both x and y axes.
        // If the product of their coordinates on either axis is <= 0, the line crosses that axis.
        return (A.x * B.x <= 0) && (A.y * B.y <= 0);
    }

    /// <summary>
    /// Animates the movement of the selected piece to a new position.
    /// </summary>
    private IEnumerator AnimateMovement(GameObject selectedPiece, Vector3 targetPosition, HashSet<Vector3> occupiedPositions, System.Action onMillDetected, System.Action onSwitchPlayer)
    {
        _pieceMoving = true; // Mark the piece as moving
        float speed = 5f;
        float distance = Vector3.Distance(selectedPiece.transform.position, targetPosition); // Distance to move
        float duration = distance / speed;
        float elapsedTime = 0f;

        Vector3 startingPosition = selectedPiece.transform.position; // Starting position of the piece
        occupiedPositions.Remove(startingPosition); // Remove old position from occupied

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time
            float t = elapsedTime / duration; // Calculate interpolation factor
            selectedPiece.transform.position = Vector3.Lerp(startingPosition, targetPosition, t); // Move piece
            yield return null;
        }

        selectedPiece.transform.position = targetPosition; // Snap to target position
        occupiedPositions.Add(targetPosition); // Mark new position as occupied

        // Check if moving to this position creates a mill
        if (lineDetector.IsMill(targetPosition, selectedPiece.name, occupiedPositions))
        {
            onMillDetected.Invoke();
            Debug.Log($"Mill detected after moving {selectedPiece.name} to {targetPosition}.");
        }
        else
        {
            Debug.Log($"No mill detected after moving {selectedPiece.name} to {targetPosition}.");
        }

        DeselectCurrentPiece(selectedPiece);
        onSwitchPlayer.Invoke();
        _pieceMoving = false; // Mark movement as complete
    }

    /// <summary>
    /// Deselects the currently selected piece.
    /// </summary>
    private void DeselectCurrentPiece(GameObject selectedPiece)
    {
        selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
}
