using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.q3cz605yh88g")]
public class GameManager : Singleton<GameManager>
{
    public BoardGenerate board;
    public PieceController piece;

    public bool _useEditorSettings = false;
    [Range(1, 10)] [SerializeField] private int _numberOfRings = 1;

    public string _player1Name = "Player1";
    public string _player2Name = "Player2";

    [HideInInspector] public GameObject _gameOverObj;

    private void Start()
    {
        AudioManager.PlayFromResources(Sound.Melody, 0.3f, 1, true);
        Application.targetFrameRate = 60;

        SaveDefaultData();
    }

    private void SaveDefaultData()
    {
        if (!PlayerPrefs.HasKey("PlayerName_1"))
            PlayerPrefs.SetString("PlayerName_1", _player1Name);

        if (!PlayerPrefs.HasKey("PlayerName_2"))
            PlayerPrefs.SetString("PlayerName_2", _player2Name);

        if (!PlayerPrefs.HasKey("PlayerColor_1"))
            PlayerPrefs.SetString("PlayerColor_1", "Blue");

        if (!PlayerPrefs.HasKey("PlayerColor_2"))
            PlayerPrefs.SetString("PlayerColor_2", "Gray");

        if (!PlayerPrefs.HasKey("Rings"))
            PlayerPrefs.SetInt("Rings", 3);
    }

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
        if(piece == null || board == null) 
            return;

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

        AudioManager.PlayFromResources(Sound.Mill, 0.5f);
    } 

    /// <summary>
    /// Handles the logic when a player wins the game.
    /// Instantiates the "GameOver" popup and sets up the exit button and message.
    /// </summary>
    public void PlayerWin(string name)
    {
        if (_gameOverObj != null)
            return;

        _gameOverObj = Instantiate(Resources.Load<GameObject>("Menu/Popup/GameOver"), GameObject.Find("Canvas").transform);

        _gameOverObj.GetComponent<AnimationPopup>().ShowPopup();

        // Setup the exit popup window
        if (_gameOverObj.TryGetComponent<GameOver>(out var gameOver))
        {
            gameOver.SetBackToMenuButtonEvent(() => MenuManager.Instance.LoadMainMenu());
            gameOver.SetRestartButtonEvent(() => MenuManager.Instance.RestartCurrentLevel());
            gameOver.SetWinnerText(name);
        }

        // Destroying the game board and pieces
        Destroy(GameObject.Find("Board"));

        Debug.Log($"{name} wins!");
        AudioManager.PlayFromResources(Sound.YouWin, 0.7f);
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
