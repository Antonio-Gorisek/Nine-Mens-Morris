
public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;
    public PieceController piece;

    public void Start()
    {
        board = new BoardGenerate(3);
        piece = new PieceController(3, board.ListOfSpots);
    }

    private void Update()
    {
        PlayerController.Instance.HandleInput(piece);
    }
}
