using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;
    public PieceController piece;
    
    public void LoadBoard()
    {
        board = new BoardGenerate(PlayerPrefs.GetInt("Rings"));
        piece = new PieceController(PlayerPrefs.GetInt("Rings"), board.ListOfSpots);
        piece.MillDetected += Piece_millDetected;
    }

    private void Piece_millDetected()
    {
        PlayerController.Instance.RemoveOpponentPiece();
        Debug.Log("Mill Detected");
    }

    private void Update()
    {
        if (board == null || piece == null)
            return;

        PlayerController.Instance.HandleInput(piece);
    }
}
