using UnityEngine;

[System.Serializable]
public class Player
{
    public GameObject piecePrefab;
    public string playerName;
    public int piecesOnBoard;
    public int remainingPieces;
    public int maxPieces;

    public Player(string name, GameObject prefab, int maxPieces)
    {
        playerName = name;
        piecePrefab = prefab;
        piecesOnBoard = 0;
        this.remainingPieces = maxPieces;
        this.maxPieces = maxPieces;
    }
}