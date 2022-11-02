using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _singleton;
    public static GameManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)//if there is already one in the scene:
            {
                Debug.LogWarning($"{nameof(GameManager)} instance already exists\nRemove Duplicate");//warn the user
                Destroy(value);//remove the duplicate
            }
        }
    }
    #endregion
    public enum GameState
    {
        MainMenu,
        MainGame,
        PauseMenu,
        Lobby,
    }
    public static GameState CurrentGameState = GameState.MainMenu;
    [SerializeField] private GameObject _localPlayerPrefab;
    public GameObject LocalPlayerPrefab { get; set; }
    [SerializeField] private GameObject _playerPrefab;
    public GameObject playerPrefab => _playerPrefab;
    public void ChangeScene(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 0://Main Menu
                CurrentGameState = GameState.MainMenu;
                SceneManager.LoadScene(0);
                break;
            case 1://Main Game
                CurrentGameState = GameState.MainGame;
                SceneManager.LoadScene(1);
                break;
            case 2://Test scene
                CurrentGameState = GameState.MainGame;
                NetworkManager.Singleton.StartServer();
                SceneManager.LoadScene(2);
                break;
            default:
                SceneManager.LoadScene(sceneNumber);
                break;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Singleton = this;
        if (CurrentGameState == GameState.MainGame)
        {
            StartGame();
        }
    }
    private void StartGame()
    {
        //Change name and position
        Player.Spawn(0, "Host");
    }
}
