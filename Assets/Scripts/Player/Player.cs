using UnityEngine;

[System.Serializable]
public class Player
{
    public GameObject piecePrefab; // The GameObject representing the player's piece (prefab)
    public string playerName; // The name of the player
    public int piecesOnBoard; // The number of pieces the player currently has on the board
    public int remainingPieces; // The number of pieces the player has left to place on the board
    public int maxPieces; // The maximum number of pieces the player can have

    /// <summary>
    /// Constructor for creating a Player instance with a name, piece prefab, and maximum number of pieces.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="prefab">The GameObject that will be used as the player's piece.</param>
    /// <param name="maxPieces">The maximum number of pieces the player can have.</param>
    public Player(string name, GameObject prefab, int maxPieces)
    {
        playerName = name;
        piecePrefab = prefab;
        piecesOnBoard = 0;
        remainingPieces = maxPieces;
        this.maxPieces = maxPieces;
    }
}
