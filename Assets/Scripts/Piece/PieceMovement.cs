using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement
{
    public List<Vector3> positionOfSpots = new List<Vector3>();
    private bool _pieceMoving;

    public PieceMovement(List<Vector3> positionOfSpots)
    {
        this.positionOfSpots = positionOfSpots;
    }

    /// <summary>
    /// Moves the selected piece to a new position if the path is clear.
    /// </summary>
    public void MoveSelectedPiece(GameObject selectedPiece, Vector3 newPosition, HashSet<Vector3> occupiedPositions, System.Action onSwitchPlayer)
    {
        if (_pieceMoving == true)
            return;

        if (IsPathClear(selectedPiece.transform.position, newPosition))
        {
            GameManager.Instance.StartCoroutine(AnimateMovement(selectedPiece, newPosition, occupiedPositions, onSwitchPlayer));
        }
        else
        {
            Debug.Log("Path is blocked or invalid.");
        }
    }

    /// <summary>
    /// Enables the selection visual for the specified piece.
    /// </summary>
    public void EnablePieceSelection(GameObject piece, Player player)
    {
        piece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        Debug.Log($"{player.playerName} selected their piece.");
    }

    /// <summary>
    /// Checks if the path from start to end is clear for movement.
    /// </summary>
    private bool IsPathClear(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start; // Calculate movement direction
        float distance = direction.magnitude; // Get the distance

        Debug.Log($"Checking path from {start} to {end}");

        // Check for diagonal movement
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0)
        {
            // Allow diagonal movement only if moving from or to the center
            if ((start == Vector3.zero && positionOfSpots.Contains(end)) || (end == Vector3.zero && positionOfSpots.Contains(start)))
            {
                return true;
            }
            else
            {
                Debug.Log("Diagonal movement is not allowed.");
                return false;
            }
        }

        // Prevent movement between center and side positions
        if ((start == Vector3.zero && IsSidePosition(end)) || (end == Vector3.zero && IsSidePosition(start)))
        {
            Debug.Log("Movement between center and side positions is not allowed.");
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
        return true;
    }

    /// <summary>
    /// Checks if the specified position is a side position.
    /// </summary>
    private bool IsSidePosition(Vector3 position)
    {
        return Mathf.Approximately(Vector3.Distance(Vector3.zero, position), 1f); // Check if distance to center is approximately 1
    }

    /// <summary>
    /// Animates the movement of the selected piece to a new position.
    /// </summary>
    private IEnumerator AnimateMovement(GameObject selectedPiece, Vector3 targetPosition, HashSet<Vector3> occupiedPositions, System.Action onSwitchPlayer)
    {
        _pieceMoving = true;
        float movementSpeed = 5f;
        float distance = Vector3.Distance(selectedPiece.transform.position, targetPosition); // Distance to move
        float duration = distance / movementSpeed; // Duration of movement
        float elapsedTime = 0f; // Time elapsed during movement

        Vector3 startingPosition = selectedPiece.transform.position; // Starting position of the piece
        occupiedPositions.Remove(startingPosition); // Remove old position from occupied

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            selectedPiece.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            yield return null;
        }

        selectedPiece.transform.position = targetPosition; // Snap to target position
        occupiedPositions.Add(targetPosition); // Mark new position as occupied

        DeselectCurrentPiece(selectedPiece); // Deselect piece after movement
        onSwitchPlayer.Invoke(); // Switch to the next player
        _pieceMoving = false;
    }

    /// <summary>
    /// Deselects the currently selected piece.
    /// </summary>
    private void DeselectCurrentPiece(GameObject selectedPiece)
    {
        selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
}