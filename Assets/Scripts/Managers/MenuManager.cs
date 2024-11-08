using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _settings;
    [SerializeField] private Button _quit;
    [Space(20)]
    [SerializeField] private Transform _gameUI;
    [SerializeField] private Transform _menuUI;
    [SerializeField] private Transform _canvas;

    private GameObject _transitionObj;
    private GameObject _settingsObj;

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
        Destroy(_transitionObj, 2);

        // After a short delay, load the game UI and board
        Invoke(nameof(LoadGame), 0.5f);
    }

    public void LoadMainMenu()
    {
        if (_transitionObj != null)
            return;

        // Instantiate the transition object from Resources and destroy it after 2 seconds
        AudioManager.PlayFromResources(Sounds.Melody, 0.2f, 1, true);

        _transitionObj = Instantiate(Resources.Load<GameObject>("Menu/Transition"));
        Destroy(_transitionObj, 2);

        // After a short delay, load the game UI and board
        Invoke(nameof(LoadMenu), 0.5f);

    }

    // Loads the game UI and board, hides the menu UI
    private void LoadMenu()
    {
        _menuUI.gameObject.SetActive(true);
        _gameUI.gameObject.SetActive(false);
        GameManager.Instance.DestroyBoard();
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
    public void MelodyDisable(bool value)
    {
        GameObject.Find("Melody").GetComponent<AudioSource>().mute = value;
        _melodyDisable = value;
    }

    // Opens the settings menu by instantiating a settings popup
    private void Settings()
    {
        // Prevent multiple instantiations of the settings popup
        if (_settingsObj != null)
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
        // Instantiate the exit game popup
        GameObject obj = Instantiate(Resources.Load<GameObject>("Menu/Popup/ExitGame"), _canvas);
        obj.GetComponent<AnimationPopup>().ShowPopup(); // Show the popup with animation

        // Find the "Btn_Exit" button inside the popup and add a listener to exit the game
        GameObject exitGame = null;
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
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
