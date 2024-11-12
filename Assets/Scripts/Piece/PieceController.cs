using System.Collections.Generic;
using UnityEngine;

public class PieceController
{
    public delegate void MillDetection(string name);
    public event MillDetection MillDetected;

    private PieceAnimation playerPieceAnimation;
    private GameObject _selectedPiece;
    private int _currentPlayerIndex;

    private readonly Player[] _players;
    private readonly PieceMillDetector _pieceMillDetector;
    private readonly PieceMovement _pieceMovement;

    private readonly List<Vector3> _positionOfSpots = new List<Vector3>();

    private readonly int _numberOfRings;

    /// <summary>
    /// Constructor for PieceController class.
    /// Initializes players and necessary components.
    /// </summary>
    public PieceController(int numberOfRings = 3, List<Vector3> positionOfSpots = null)
    {
        GameManager gameManager = GameManager.Instance;

        // Use settings from GameManager or PlayerPrefs depending on the configuration
        bool useEditorSettings = gameManager._useEditorSettings;
        string player1Name = GetPlayerName(gameManager, useEditorSettings, 1);
        string player2Name = GetPlayerName(gameManager, useEditorSettings, 2);

        GameObject player1Piece = LoadPlayerPiece(1);
        GameObject player2Piece = LoadPlayerPiece(2);

        // Initialize players with their respective pieces and ring counts
        _players = new Player[]
        {
            new Player(player1Name, player1Piece, numberOfRings * 3),
            new Player(player2Name, player2Piece, numberOfRings * 3)
        };

        // Initialize movement and detection components
        _pieceMillDetector = new PieceMillDetector(positionOfSpots, numberOfRings);
        _pieceMovement = new PieceMovement(_pieceMillDetector, positionOfSpots, numberOfRings, _players);

        _positionOfSpots = positionOfSpots;
        _numberOfRings = numberOfRings;
        // Announce the starting player's turn
        Info.Instance.Message($"It's <color=yellow>{_players[_currentPlayerIndex].playerName}'s</color> turn.");
    }

    /// <summary>
    /// Helper method to get the player's name, either from GameManager or PlayerPrefs.
    /// </summary>
    private string GetPlayerName(GameManager gameManager, bool useEditorSettings, int playerIndex)
    {
        return useEditorSettings ?
            (playerIndex == 1 ? gameManager._player1Name : gameManager._player2Name) :
            PlayerPrefs.GetString($"PlayerName_{playerIndex}");
    }

    /// <summary>
    /// Helper method to load the player's piece from resources.
    /// </summary>
    private GameObject LoadPlayerPiece(int playerIndex)
    {
        string playerColor = PlayerPrefs.GetString($"PlayerColor_{playerIndex}");
        return Resources.Load<GameObject>($"Pieces/{playerColor}");
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

            GetAllNeighbors(piece.transform.position);
        }
        else
        {
            Debug.Log("This piece doesn't belong to the current player.");
        }
    }

    private string GetAllNeighbors(Vector3 pos)
    {
        PieceNeighbors pieceNeighbors = new PieceNeighbors(_pieceMillDetector.GetOwners(), _positionOfSpots, _numberOfRings);
        List<Vector3> positions = pieceNeighbors.GetAvailableMoves(pos);
        foreach (var position in positions)
        {
            GameObject spot = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot")).transform.gameObject;
            spot.transform.GetChild(1).transform.gameObject.SetActive(true);
        }
        return null;
    }

    /// <summary>
    /// Deselects the previously selected piece.
    /// </summary>
    private void DeselectPreviousPiece()
    {
        foreach (var position in _positionOfSpots)
        {
            GameObject spot = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot")).transform.gameObject;
            spot.transform.GetChild(1).transform.gameObject.SetActive(false);
        }

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
        // Check if the spot is valid, not occupied, and no piece is currently moving
        if (Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spot")).collider == null) {
            Debug.Log("Invalid position or position is occupied.");
            return;
        }

        // Check if a piece already exists at the target position
        if (Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Piece")).collider != null) {
            Debug.Log("This spot is already occupied by a piece.");
            return;
        }

        // Proceed with moving or placing the piece
        if (_selectedPiece != null)
        {
            _pieceMovement.MoveSelectedPiece(_selectedPiece, position, 
                () => TriggerMillDetected(_players[_currentPlayerIndex].playerName), SwitchPlayer, _players[_currentPlayerIndex]);
            DeselectPreviousPiece();
            _selectedPiece = null;
        }
        else if (_players[_currentPlayerIndex].remainingPieces > 0)
        {
            PlaceNewPiece(position);
        }
    }


    /// <summary>
    /// Places a new piece at the specified position and updates the game state.
    /// </summary>
    private void PlaceNewPiece(Vector3 position)
    {
        if (playerPieceAnimation != null && playerPieceAnimation.isAnimating)
            return;

        // Instantiate a new piece
        GameObject piece = Object.Instantiate(_players[_currentPlayerIndex].piecePrefab, position, Quaternion.identity, GameObject.Find("Board").transform);
        piece.name = _players[_currentPlayerIndex].playerName;

        // Update player stats
        _players[_currentPlayerIndex].remainingPieces--;
        _players[_currentPlayerIndex].piecesOnBoard++;

        // Ensure PieceAnimation is found and triggered
        playerPieceAnimation = piece.GetComponent<PieceAnimation>();
        playerPieceAnimation.StartPlaceAnimation(position);  // Call the animation method

        // Set the piece owner in the line detector
        _pieceMillDetector.SetOwner(position, _players[_currentPlayerIndex].playerName);

        // Switch to the next player
        SwitchPlayer();

        // Check if placing the piece creates a mill
        if (_pieceMillDetector.IsMill(position, piece.name))
        {
            TriggerMillDetected(piece.name); // Trigger mill detected event
        }
    }
    /// <summary>
    /// Removes an opponent's selected piece from the board, ensuring that it is not part of a mill.
    /// </summary>
    public void RemoveOpponentPiece(Vector3 position)
    {
        // Try to find the opponent's piece at the given position
        GameObject piece = GetPieceAtPosition(position);
        if (piece == null) return;

        Player opponentPlayer = _players[_currentPlayerIndex];
        Player currentPlayer = _players[(_currentPlayerIndex + 1) % _players.Length];

        // Check if the piece belongs to the opponent
        if (!IsOpponentPiece(piece, opponentPlayer)) return;

        // Check if the piece is part of a mill
        if (IsPieceInMill(piece, opponentPlayer)) return;

        // Proceed with removing the piece
        RemovePiece(piece, opponentPlayer);

        // Check if the opponent has fewer than 3 pieces remaining
        if (opponentPlayer.piecesOnBoard + opponentPlayer.remainingPieces < 3)
        {
            GameManager.Instance.PlayerWin(currentPlayer.playerName);
        }
    }

    // Method to find the piece at the given position
    private GameObject GetPieceAtPosition(Vector3 position)
    {
        RaycastHit2D hitPiece = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Piece"));
        if (hitPiece.collider == null)
        {
            Debug.Log("No piece found at the selected position to remove.");
            return null;
        }
        return hitPiece.collider.gameObject;
    }

    // Method to check if the piece belongs to the opponent
    private bool IsOpponentPiece(GameObject piece, Player opponentPlayer)
    {
        if (piece.transform.name != opponentPlayer.playerName)
        {
            Debug.Log("Selected piece does not belong to the opponent.");
            return false;
        }
        return true;
    }

    // Method to check if the piece is part of a mill
    private bool IsPieceInMill(GameObject piece, Player opponentPlayer)
    {
        if (_pieceMillDetector.IsMill(piece.transform.position, opponentPlayer.playerName))
        {
            Debug.Log("Cannot remove that piece, it's part of a mill.");
            Info.Instance.Message("<color=red>Cannot remove that piece, it's part of a mill.</color>");
            AudioManager.PlayFromResources(Sounds.Error, 0.5f);
            return true;
        }
        return false;
    }

    // Method to remove the piece and update the game state
    private void RemovePiece(GameObject piece, Player opponentPlayer)
    {
        Object.Destroy(piece);
        opponentPlayer.piecesOnBoard--;
        PlayerController.Instance.OpponentPieceRemoved();
        _pieceMillDetector.RemoveOwner(piece.transform.position);

        Debug.Log($"{opponentPlayer.playerName}'s piece at {piece.transform.position} was removed.");
        Info.Instance.Message($"It's <color=yellow>{_players[_currentPlayerIndex].playerName}'s</color> turn.");
        AudioManager.PlayFromResources(Sounds.PieceRemove, 0.5f);

        string blockedPlayerName = IsAnyPlayerBlocked(_pieceMillDetector);
        if (blockedPlayerName != null)
        {
            GameManager.Instance.PlayerWin(blockedPlayerName);
            return;
        }
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
    /// Switches the current player to the next player.
    /// </summary>
    private void SwitchPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length; // Update player index
        Debug.Log($"It's {_players[_currentPlayerIndex].playerName}'s turn.");
        Info.Instance.Message($"It's <color=yellow>{_players[_currentPlayerIndex].playerName}'s</color> turn.");
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
    /// <param name="name">The name of the player.</param>
    public void TriggerMillDetected(string name)
    {
        MillDetected?.Invoke(name);  // Pass the name when invoking the event
    }
}
