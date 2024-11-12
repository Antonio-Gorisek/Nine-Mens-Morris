using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class PieceMovement
{
    private readonly List<Vector3> _positionOfSpots = new List<Vector3>();
    private readonly PieceMillDetector _pieceMillDetector;
    private readonly Player[] _players;
    private readonly int _numberOfRings;

    private GameObject _selectedPiece;
    public bool _pieceMoving;

    public PieceMovement(PieceMillDetector lineDetector, List<Vector3> positionOfSpots, int _numberOfRings, Player[] players)
    {
        this._pieceMillDetector = lineDetector;
        this._positionOfSpots = positionOfSpots;
        this._numberOfRings = _numberOfRings;
        this._players = players;
    }

    /// <summary>
    /// Moves the selected piece to a new position if the path is clear.
    /// </summary>
    public void MoveSelectedPiece(GameObject selectedPiece, Vector3 newPosition, System.Action onMillDetected, System.Action onSwitchPlayer, Player player)
    { 
        if (_pieceMoving)
            return;

        if (IsPathClear(selectedPiece.transform.position, newPosition, player))
        {
            GameManager.Instance.StartCoroutine(AnimateMovement(selectedPiece, newPosition, onMillDetected, onSwitchPlayer, _pieceMillDetector));
        }
        else
        {
            Debug.Log("Path is blocked or invalid.");
            AudioManager.PlayFromResources(Sounds.Error, 0.5f);
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

        if (_selectedPiece != null)
        { DeselectCurrentPiece(_selectedPiece); }


        _selectedPiece = piece;
        piece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        Debug.Log($"{player.playerName} selected their piece.");
    }

    /// <summary>
    /// Checks if the path from start to end is clear for movement.
    /// </summary>
    private bool IsPathClear(Vector3 start, Vector3 end, Player player)
    {
        if (player.piecesOnBoard + player.remainingPieces == 3 && _numberOfRings >= 3)
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
            if ((start == Vector3.zero && _positionOfSpots.Contains(end)) || (end == Vector3.zero && _positionOfSpots.Contains(start)))
            {
                return true; // Diagonal movement is valid
            }
            else
            {
                Debug.Log("Diagonal movement is not allowed."); // Log if diagonal movement is invalid
                AudioManager.PlayFromResources(Sounds.Error, 0.5f);
                return false;
            }
        }

        // Prevent movement towards the center (0, 0, 0)
        if (_numberOfRings > 1 && IsPlayerCrossingCenter(start, end))
        {
            Debug.Log("Path is blocked or invalid.");
            AudioManager.PlayFromResources(Sounds.Error, 0.5f);
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
                    AudioManager.PlayFromResources(Sounds.Error, 0.5f);
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
    private IEnumerator AnimateMovement(GameObject selectedPiece, Vector3 targetPosition, System.Action onMillDetected, System.Action onSwitchPlayer, PieceMillDetector lineDetector)
    {
        if (_pieceMoving) yield break; // Ensures no animation starts if a piece is moving already

        _pieceMoving = true;

        float animationDuration = 0.5f; // Duration of the movement animation
        AudioManager.PlayFromResources(Sounds.PieceMove, 0.15f, 1.2f);

        Vector3 startingPosition = selectedPiece.transform.position;
        lineDetector.RemoveOwner(startingPosition);

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            selectedPiece.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            yield return null;
        }

        selectedPiece.transform.position = targetPosition; // Final position snap
        lineDetector.SetOwner(targetPosition, selectedPiece.name);

        onSwitchPlayer.Invoke();

        string blockedPlayerName = IsAnyPlayerBlocked(lineDetector);
        if (blockedPlayerName != null)
        {
            GameManager.Instance.PlayerWin(blockedPlayerName);
            yield break;
        }

        // Check for a mill after the move
        if (_pieceMillDetector.IsMill(targetPosition, selectedPiece.name))
        {
            onMillDetected.Invoke();
            Info.Instance.Message($"<color=yellow>{selectedPiece.name}</color> formed a mill! <color=red>Remove a piece.</color>");
            Debug.Log($"Mill detected after moving {selectedPiece.name} to {targetPosition}.");
        }
        else
        {
            Debug.Log($"No mill detected after moving {selectedPiece.name} to {targetPosition}.");
        }



        DeselectCurrentPiece(selectedPiece);
        _pieceMoving = false; // Reset after animation ends
    }

    private string IsAnyPlayerBlocked(PieceMillDetector lineDetector)
    {
        PieceNeighbors pieceNeighbors = new PieceNeighbors(lineDetector.GetOwners(), _positionOfSpots, _numberOfRings);
        foreach (var player in _players)
        {
            if (pieceNeighbors.AreAllPiecesBlocked(player.playerName))
            {
                return player.playerName;
            }
        }
        return null;
    }

    /// <summary>
    /// Deselects the currently selected piece.
    /// </summary>
    private void DeselectCurrentPiece(GameObject selectedPiece)
    {
        selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        _selectedPiece = null;
    }
}
