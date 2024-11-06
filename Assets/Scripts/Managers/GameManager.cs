using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;
    public PieceController piece;

    [Range(1, 10)]
    [SerializeField] private int _numberOfRings;
    public void Start()
    {
        board = new BoardGenerate(_numberOfRings);
        piece = new PieceController(_numberOfRings, board.ListOfSpots);
        piece.MillDetected += Piece_millDetected;
    }

    private void Piece_millDetected()
    {
        PlayerController.Instance.RemoveOpponentPiece();
        Debug.Log("Mill Detected");
    }

    private void Update()
    {
        PlayerController.Instance.HandleInput(piece);
    }
}
