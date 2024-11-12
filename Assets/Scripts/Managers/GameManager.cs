using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;
    public PieceController piece;

    public bool _useEditorSettings = false;
    [Range(1, 10)] [SerializeField] private int _numberOfRings = 1;

    public string _player1Name = "Player1";
    public string _player2Name = "Player2";


    // Play the background melody on loop with specified volume and pitch
    private void Awake() => AudioManager.PlayFromResources(Sounds.Melody, 0.3f, 1, true);


    // Loads the game board and pieces
    public void LoadBoard()
    {
        // Ensure that the board and pieces are only loaded once
        if (board != null || piece != null)
            return;

        // Generates the game board based on the number of "Rings"
        board = new BoardGenerate(_useEditorSettings ? _numberOfRings : PlayerPrefs.GetInt("Rings"));

        // Stores the piece data based on the number of rings (size of the board)
        piece = new PieceController(_useEditorSettings ? _numberOfRings : PlayerPrefs.GetInt("Rings"), board.ListOfSpots);

        // Subscribe to the event that is triggered when a mill (three pieces in a row) is formed
        piece.MillDetected += Piece_millDetected;
    }

    public void DestroyBoard()
    {
        Destroy(GameObject.Find("Board"));
        piece.MillDetected -= Piece_millDetected;
        board = null; piece = null;
    }
    /// <summary>
    /// This method is called when a mill is detected (three pieces in a row).
    /// </summary>
    /// <param name="name">The name of the player who formed the mill.</param>
    private void Piece_millDetected(string name)
    {
        // Grants the player permission to remove one of the opponent's pieces
        PlayerController.Instance.RemoveOpponentPiece();

        // Display a message
        Info.Instance.Message($"<color=yellow>{name}</color> formed a mill! <color=red>Remove a piece.</color>");
        Debug.Log($"{name} formed a mill! Remove a piece.");

        AudioManager.PlayFromResources(Sounds.Mill, 0.5f);
    }

    /// <summary>
    /// Handles the logic when a player wins the game.
    /// Instantiates the "GameOver" popup and sets up the exit button and message.
    /// </summary>
    public void PlayerWin(string name)
    {
        // Instantiate the "GameOver" popup and set its parent to the canvas
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Menu/Popup/GameOver"), GameObject.Find("Canvas").transform);

        // Show the popup animation
        obj.GetComponent<AnimationPopup>().ShowPopup();

        // Setup the exit popup window
        if (obj.TryGetComponent<GameOver>(out var gameOver))
        {
            gameOver.SetBackToMenuButtonEvent(() => MenuManager.Instance.LoadMainMenu());
            gameOver.SetRestartButtonEvent(() => MenuManager.Instance.RestartCurrentLevel());
            gameOver.SetWinnerText(name);
        }

        // Destroying the game board
        Object.Destroy(GameObject.Find("Board"));

        Debug.Log($"{name} wins!");
        AudioManager.PlayFromResources(Sounds.YouWin, 0.7f);
        AudioManager.StopAudioClip("Melody");
    }

    private void Update()
    {
        // If the board or piece is null, skip the input handling
        if (board == null || piece == null)
            return;

        // Handle player input to select or place pieces on the board
        PlayerController.Instance.HandleInput(piece);
    }
}
