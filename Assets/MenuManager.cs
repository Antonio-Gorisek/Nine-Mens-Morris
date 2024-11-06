using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _settings;
    [SerializeField] private Button _quit;
    [Space(20)]
    [SerializeField] private Transform _menuUI;
    [SerializeField] private Transform _canvas;

    private void Awake()
    {
        _startGame.onClick.AddListener(StartGame);
        _settings.onClick.AddListener(Settings);
        _quit.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        _menuUI.gameObject.SetActive(false);
        GameManager.Instance.LoadBoard();
    }
    
    private void Settings()
    {
        Instantiate(Resources.Load<GameObject>("Menu/Popup/Settings"), _canvas);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
