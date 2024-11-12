using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.ml1nz46racuq")]
public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _settings;
    [SerializeField] private Button _quit;
    [Space(20)]
    [SerializeField] private Transform _gameUI;
    [SerializeField] private Transform _menuUI;
    [SerializeField] private Transform _canvas;

    [HideInInspector] public GameObject _transitionObj { get; private set; }
    private GameObject _settingsObj;
    private GameObject _quitObj;

    [HideInInspector] public bool _melodyDisable;
    [HideInInspector] public bool _sfxDisable;


    private void Awake()
    {
        _startGame.onClick.AddListener(StartGame);
        _settings.onClick.AddListener(Settings);
        _quit.onClick.AddListener(QuitGamePopup);
    }

    // Starts the game by triggering a transition animation and loading the game scene
    private void StartGame()
    {
        // Prevent multiple instantiations of the transition object
        if (_transitionObj != null)
            return;

        // Instantiate the transition object from Resources and destroy it after 2 seconds
        _transitionObj = Instantiate(Resources.Load<GameObject>("Menu/Transition"));
        _transitionObj.name = "Transition";
        Destroy(_transitionObj, 1);

        // After a short delay, load the game UI and board
        Invoke(nameof(LoadGame), 0.5f);
    }

    public void LoadMainMenu()
    {
        if (_quitObj || _settingsObj || _transitionObj)
            return;

        PlayerController.Instance.OpponentPieceRemoved();

        _transitionObj = Instantiate(Resources.Load<GameObject>("Menu/Transition"));
        _transitionObj.name = "Transition";
        Destroy(_transitionObj, 1);

        // After a short delay, load the game UI and board
        Invoke(nameof(LoadMenu), 0.5f);

    }


    public void RestartCurrentLevel()
    {
        GameManager.Instance.DestroyBoard();

        PlayerController.Instance.OpponentPieceRemoved();

        Invoke(nameof(LoadBoardWithDelay), 0.5f);
    }

    /// <summary>
    /// The delay is set to prevent the board's line renderer and spots from overlapping the UI.
    /// </summary>
    private void LoadBoardWithDelay() => GameManager.Instance.LoadBoard();


    // Loads the game UI and board, hides the menu UI
    private void LoadMenu()
    {
        _menuUI.gameObject.SetActive(true);
        _gameUI.gameObject.SetActive(false);
        GameManager.Instance.DestroyBoard();
        MelodyMute(_melodyDisable);
    }


    // Loads the game UI and board, hides the menu UI
    private void LoadGame()
    {
        _menuUI.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(true);
        GameManager.Instance.LoadBoard();
    }

    /// <summary>
    /// Toggles the SFX state based on the button interaction in the SFX_Option object.
    /// </summary>
    /// <param name="value">The state of the sound effects (true = disabled, false = enabled)</param>
    public void SFXDisable(bool value) => _sfxDisable = value;

    /// <summary>
    /// Toggles the background melody state based on the button interaction in the Melody_Option object.
    /// </summary>
    /// <param name="value">The state of the background melody (true = muted, false = unmuted)</param>
    public void MelodyMute(bool mute)
    {
        _melodyDisable = mute;
        GameObject melody = GameObject.Find("Melody");
        if (melody != null)
        {
            melody.GetComponent<AudioSource>().mute = mute;
        }
        else
        {
            if(mute == false)
            {
                AudioManager.PlayFromResources(Sound.Melody, 0.3f, 1, true);
            }
        }
    }

    // Opens the settings menu by instantiating a settings popup
    private void Settings()
    {
        // Prevent multiple instantiations of the settings popup
        if (_quitObj || _settingsObj || _transitionObj)
            return;

        _settingsObj = Instantiate(Resources.Load<GameObject>("Menu/Popup/Settings"), _canvas);
        _settingsObj.GetComponent<AnimationPopup>().ShowPopup();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stop play mode in Unity editor
        EditorApplication.isPlaying = false;
#else
        // Quit the game in a built application
        Application.Quit();
#endif
    }

    // Shows a confirmation popup to quit the game
    public void QuitGamePopup()
    {
        if (_quitObj || _settingsObj || _transitionObj)
            return;

        // Instantiate the exit game popup
        _quitObj = Instantiate(Resources.Load<GameObject>("Menu/Popup/ExitGame"), _canvas);
        _quitObj.GetComponent<AnimationPopup>().ShowPopup(); // Show the popup with animation

        // Find the "Btn_Exit" button inside the popup and add a listener to exit the game
        GameObject exitGame = null;
        foreach (Transform child in _quitObj.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Btn_Exit")
            {
                exitGame = child.gameObject;
                break;
            }
        }
        exitGame.GetComponent<Button>().onClick.AddListener(QuitGame); // Attach the quit function to the exit button
    }
}
