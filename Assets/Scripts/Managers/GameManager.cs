
public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;

    public void Start()
    {
        board = new BoardGenerate(3, 1);
    }
}
