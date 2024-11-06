﻿using System.Collections.Generic;
using UnityEngine;

public class PieceController
{
    public delegate void MillDetection();
    public event MillDetection MillDetected; // Event to notify when a mill is detected

    private GameObject _selectedPiece;
    private int _currentPlayerIndex;

    private readonly Player[] _players;
    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private readonly PieceMillDetector _lineDetector;
    private readonly PieceMovement _pieceMovement;

    /// <summary>
    /// Constructor for PiecePlacement class.
    /// Initializes players and necessary components.
    /// </summary>
    public PieceController(int numberOfRings = 3, List<Vector3> positionOfSpots = null)
    {
        _players = new Player[]
        {
            new Player("Player 1", Resources.Load<GameObject>("P1"), numberOfRings * 3),
            new Player("Player 2", Resources.Load<GameObject>("P2"), numberOfRings * 3)
        };

        _lineDetector = new PieceMillDetector(positionOfSpots, numberOfRings);
        _pieceMovement = new PieceMovement(_lineDetector, positionOfSpots, numberOfRings);
    }

    /// <summary>
    /// Selects or places a piece based on the mouse position.
    /// </summary>
    public void SelectOrPlacePiece(Vector3 position)
    {
        var hitPiece = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Piece"));
        if (hitPiece.collider != null && !AnyPiecesLeft()) // If no pieces are left
        {
            SelectPiece(hitPiece.collider.gameObject); // Select the piece
            return;
        }

        var hitSpot = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot"));
        if (hitSpot.collider != null)
        {
            PlacePiece(hitSpot.collider.transform.position); // Place the piece
        }
        else
        {
            Debug.Log("Invalid selection.");
        }
    }

    /// <summary>
    /// Selects the specified piece if it belongs to the current player.
    /// </summary>
    private void SelectPiece(GameObject piece)
    {
        Player currentPlayer = _players[_currentPlayerIndex];

        // Check if the piece belongs to the current player
        if (piece.transform.name == currentPlayer.playerName)
        {
            DeselectPreviousPiece();
            _pieceMovement.EnablePieceSelection(piece, currentPlayer);
            _selectedPiece = piece;
        }
        else
        {
            Debug.Log("This piece doesn't belong to the current player.");
        }
    }

    /// <summary>
    /// Deselects the previously selected piece.
    /// </summary>
    private void DeselectPreviousPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Places a piece at the specified position.
    /// </summary>
    private void PlacePiece(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot"));

        // Check if the hit spot is valid and not occupied
        if (hit.collider != null && hit.collider.CompareTag("Spot") && !_occupiedPositions.Contains(hit.transform.position))
        {
            Debug.Log($"Hit {hit.collider.name} at {hit.collider.transform.position}");
            if (_selectedPiece != null)
            {
                // Move the selected piece
                _pieceMovement.MoveSelectedPiece(_selectedPiece, hit.collider.transform.position, _occupiedPositions, TriggerMillDetected, SwitchPlayer, _players[_currentPlayerIndex]);

                _selectedPiece = null;
            }
            else if (_players[_currentPlayerIndex].remainingPieces > 0)
            {
                PlaceNewPiece(hit.collider.transform.position);
            }
        }
        else
        {
            Debug.Log("Invalid position or position is occupied.");
        }
    }

    /// <summary>
    /// Places a new piece at the specified position and updates the game state.
    /// </summary>
    private void PlaceNewPiece(Vector3 position)
    {
        GameObject piece = Object.Instantiate(_players[_currentPlayerIndex].piecePrefab, position, Quaternion.identity);
        piece.name = _players[_currentPlayerIndex].playerName;

        _occupiedPositions.Add(position); // Mark the position as occupied
        _players[_currentPlayerIndex].remainingPieces--;
        _players[_currentPlayerIndex].piecesOnBoard++;

        _lineDetector.SetOwner(position, _players[_currentPlayerIndex].playerName); // Set the piece owner

        SwitchPlayer(); // Switch to the next player

        // Check if placing the piece creates a mill
        if (_lineDetector.IsMill(position, piece.name, _occupiedPositions))
        {
            TriggerMillDetected(); // Trigger mill detected event
        }
    }

    /// <summary>
    /// Removes an opponent's selected piece from the board, ensuring that it is not part of a mill.
    /// </summary>
    public void RemoveOpponentPiece(Vector3 position)
    {
        RaycastHit2D hitPiece = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Piece"));

        if (hitPiece.collider == null)
        {
            Debug.Log("No piece found at the selected position to remove.");
            return;
        }

        GameObject piece = hitPiece.collider.gameObject;
        Player opponentPlayer = _players[_currentPlayerIndex];
        Player currentPlayer = _players[(_currentPlayerIndex + 1) % _players.Length];

        if (piece.transform.name != opponentPlayer.playerName)
        {
            Debug.Log("Selected piece does not belong to the opponent.");
            return;
        }

        // Check if the piece is part of a mill and prevent removal if it is
        if (_lineDetector.IsMill(piece.transform.position, opponentPlayer.playerName, _occupiedPositions))
        {
            Debug.Log("Cannot remove this piece as it is part of a mill.");
            return;
        }

        // Remove the piece and update game state
        _occupiedPositions.Remove(piece.transform.position);
        Object.Destroy(piece);
        opponentPlayer.piecesOnBoard--;
        PlayerController.Instance.OpponentPieceRemoved();

        Debug.Log($"{opponentPlayer.playerName}'s piece at {position} was removed.");

        // Check for win condition
        if (opponentPlayer.piecesOnBoard + opponentPlayer.remainingPieces < 3)
        {
            Debug.Log($"{currentPlayer.playerName} wins!");
        }
    }


    /// <summary>
    /// Switches the current player to the next player.
    /// </summary>
    private void SwitchPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length; // Update player index
        Debug.Log($"It's {_players[_currentPlayerIndex].playerName}'s turn.");
    }

    /// <summary>
    /// Checks if any pieces are left to place for both players.
    /// </summary>
    private bool AnyPiecesLeft()
    {
        return _players[0].remainingPieces > 0 || _players[1].remainingPieces > 0;
    }

    /// <summary>
    /// Triggers the mill detected event.
    /// </summary>
    public void TriggerMillDetected()
    {
        MillDetected?.Invoke();
    }
}
