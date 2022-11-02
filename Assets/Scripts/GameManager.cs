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
    public static GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    [SerializeField] private GameObject _localPlayerPrefab;
    public GameObject LocalPlayerPrefab => _localPlayerPrefab;
    [SerializeField] private GameObject _playerPrefab;
    public GameObject playerPrefab => _playerPrefab;
    public void ChangeScene(int sceneNumber)
    {
        CurrentGameState = GameState.MainMenu;
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
    private void OnValidate()
    {
        Singleton = this;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Singleton = this;
    }

}
