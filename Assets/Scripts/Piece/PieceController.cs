using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PieceController
{
    public delegate void MillDetection(string name);
    public event MillDetection MillDetected;

    private GameObject _selectedPiece;
    private int _currentPlayerIndex;

    private readonly Player[] _players;
    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private readonly PieceMillDetector _lineDetector;
    private readonly PieceMovement _pieceMovement;

    public GameObject boardParent;
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
        _lineDetector = new PieceMillDetector(positionOfSpots, numberOfRings);
        _pieceMovement = new PieceMovement(_lineDetector, positionOfSpots, numberOfRings);

        boardParent = GameObject.Find("Board");
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
            if (_selectedPiece != null)
            {
                // Move the selected piece
                _pieceMovement.MoveSelectedPiece(
                    _selectedPiece,
                    hit.collider.transform.position,
                    _occupiedPositions,
                    () => TriggerMillDetected(_players[_currentPlayerIndex].playerName),
                    SwitchPlayer,
                    _players[_currentPlayerIndex]
                );
                DeselectPreviousPiece();
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
        GameObject piece = Object.Instantiate(_players[_currentPlayerIndex].piecePrefab, position, Quaternion.identity, boardParent.transform);
        piece.name = _players[_currentPlayerIndex].playerName;

        _occupiedPositions.Add(position); // Mark the position as occupied
        _players[_currentPlayerIndex].remainingPieces--;
        _players[_currentPlayerIndex].piecesOnBoard++;

        PieceAnimation playerPieceAnimation = piece.GetComponent<PieceAnimation>();
         playerPieceAnimation.StartPlaceAnimation(position);

        _lineDetector.SetOwner(position, _players[_currentPlayerIndex].playerName); // Set the piece owner

        SwitchPlayer(); // Switch to the next player
        // Check if placing the piece creates a mill
        if (_lineDetector.IsMill(position, piece.name, _occupiedPositions))
        {
            TriggerMillDetected(piece.name); // Trigger mill detected event
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
            Debug.Log("Cannot remove that piece, it's part of a mill.");
            Info.Instance.Message("<color=red>Cannot remove that piece, it's part of a mill.</color>");
            AudioManager.PlayFromResources(Sounds.Error, 0.5f);
            return;
        }

        // Remove the piece and update game state
        _occupiedPositions.Remove(piece.transform.position);
        Object.Destroy(piece);
        opponentPlayer.piecesOnBoard--;
        PlayerController.Instance.OpponentPieceRemoved();

        Debug.Log($"{opponentPlayer.playerName}'s piece at {position} was removed.");
        Info.Instance.Message($"It's <color=yellow>{_players[_currentPlayerIndex].playerName}'s</color> turn.");
        AudioManager.PlayFromResources(Sounds.PieceRemove, 0.5f);
        // Check for win condition
        if (opponentPlayer.piecesOnBoard + opponentPlayer.remainingPieces < 3)
        {
            PlayerWin(currentPlayer.playerName);
        }

    }

    /// <summary>
    /// Handles the logic when a player wins the game.
    /// Instantiates the "GameOver" popup and sets up the exit button and message.
    /// </summary>
    private void PlayerWin(string name)
    {
        // Instantiate the "GameOver" popup and set its parent to the canvas
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Menu/Popup/GameOver"), GameObject.Find("Canvas").transform);

        // Show the popup animation
        obj.GetComponent<AnimationPopup>().ShowPopup();

        // Setup the exit popup with the player's name
        SetupExitPopup(obj, name);

        // Destroying the game board
        Object.Destroy(boardParent);

        Debug.Log($"{name} wins!");
        AudioManager.PlayFromResources(Sounds.YouWin, 0.7f);
        AudioManager.StopAudioClip("Melody");
    }

    /// <summary>
    /// Sets up the game over popup with the player's name and exit button functionality.
    /// </summary>
    private void SetupExitPopup(GameObject obj, string name)
    {
        GameObject btn_Exit = null;
        GameObject txt_Message = null;

        // Loop through children of the popup and find the exit button and message text
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Btn_Back")
            {
                btn_Exit = child.gameObject;
            }
            if (child.name == "Txt_Message")
            {
                txt_Message = child.gameObject;
            }
        }

        // Set the congratulatory message text for the winner
        txt_Message.GetComponent<TMP_Text>().text = $"CONGRATULATIONS\r\nPLAYER\r\n<color=yellow>{name}</color> WON!";

        // Add listener to the exit button to load the main menu when clicked
        btn_Exit.GetComponent<Button>().onClick.AddListener(() => MenuManager.Instance.LoadMainMenu());
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
