﻿using UnityEngine;
using System.Collections.Generic;

public class PieceController
{
    private readonly Player[] _players;
    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private readonly PieceMovement _pieceMovement;

    private int _currentPlayerIndex;
    private GameObject _selectedPiece;

    public PieceController(int numberOfRings = 3, List<Vector3> positionOfSpots = null)
    {
        _players = new Player[]
        {
            new Player("Player 1", Resources.Load<GameObject>("P1"), numberOfRings * 3),
            new Player("Player 2", Resources.Load<GameObject>("P2"), numberOfRings * 3)
        };

        _pieceMovement = new PieceMovement(positionOfSpots); // Initialize movement helper
    }

    /// <summary>
    /// Selects or places a piece based on the mouse position.
    /// </summary>
    private void SelectPiece(GameObject piece)
    {
        Player currentPlayer = _players[_currentPlayerIndex]; // Get the current player

        // Check if the piece belongs to the current player
        if (piece.transform.name == currentPlayer.playerName)
        {
            DeselectPreviousPiece(); // Deselect the previous piece
            _pieceMovement.EnablePieceSelection(piece, currentPlayer); // Enable selection in movement helper
            _selectedPiece = piece; // Set the selected piece
        }
        else
        {
            Debug.Log("This piece doesn't belong to the current player."); // Log error
        }
    }

    /// <summary>
    /// Deselects the previously selected piece.
    /// </summary>
    private void DeselectPreviousPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false; // Disable piece visibility
        }
    }


    /// <summary>
    /// Places a piece based on the mouse position.
    /// </summary>
    public void SelectOrPlacePiece(Vector3 position)
    {
        RaycastHit2D hitPiece = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Piece"));
        if (hitPiece.collider != null && !AnyPiecesLeft()) // If no pieces are left
        {
            SelectPiece(hitPiece.collider.gameObject); // Select the piece
        }
        else
        {
            RaycastHit2D hitSpot = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot"));
            if (hitSpot.collider != null)
            {
                PlacePiece(hitSpot.collider.transform.position); // Place the piece
            }
            else
            {
                Debug.Log("Invalid selection.");
            }
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
                _pieceMovement.MoveSelectedPiece(_selectedPiece, hit.collider.transform.position, _occupiedPositions, SwitchPlayer);
            }
            else if (_players[_currentPlayerIndex].remainingPieces > 0)
            {
                PlaceNewPiece(hit.collider.transform.position); // Place a new piece
            }
        }
        else
        {
            Debug.Log("Invalid position or position is occupied."); // Log if the position is invalid
        }
    }

    /// <summary>
    /// Places a new piece at the specified position and updates the game state.
    /// </summary>
    private void PlaceNewPiece(Vector3 position)
    {
        GameObject piece = Object.Instantiate(_players[_currentPlayerIndex].piecePrefab, position, Quaternion.identity);
        piece.name = _players[_currentPlayerIndex].playerName; // Set the piece name

        _occupiedPositions.Add(position); // Mark the position as occupied
        _players[_currentPlayerIndex].remainingPieces--;
        _players[_currentPlayerIndex].piecesOnBoard++;

        SwitchPlayer();
    }

    /// <summary>
    /// Switches the current player to the next player.
    /// </summary>
    private void SwitchPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length; // Update player index

        if (!AnyPiecesLeft())
        {
            Debug.Log("Both players have no pieces left.");
            return;
        }

        Debug.Log($"It's {_players[_currentPlayerIndex].playerName}'s turn.");
    }

    /// <summary>
    /// Checks if any pieces are left to place for both players.
    /// </summary>
    private bool AnyPiecesLeft()
    {
        return _players[0].remainingPieces > 0 || _players[1].remainingPieces > 0;
    }
}