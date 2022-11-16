using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    MainGame,
}
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
    public static GameState CurrentGameState = GameState.MainMenu;
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
    public void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (CurrentGameState == GameState.MainGame)
        {
            if (NetworkManager.IsHost)
            {
                Player.Spawn(0, "Host", UIManager.Singleton.PlayerSkin);
            }
            else
            {
                SpawnPlayer(" ");//name is blank now, name to be set later in dev
            }
        }
    }

    //private void StartGame()
    //{
    //Change name and position
    //Player.Spawn(0, "Host");
    //}
    [MessageHandler((ushort)MessageIdentification.startGame)]
    public static void StartGame(Message message)
    {
        Singleton.ChangeScene(1);
    }
    private void SpawnPlayer(string name)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIdentification.spawn);
        message.AddString(name);
        message.AddString(UIManager.Singleton.PlayerSkin);
        NetworkManager.Singleton.Client.Send(message);
    }

    //finds and returns item with id
    public GameObject FindItem(int ID)
    {
        foreach (GameObject o in ItemPickup.weaponList)
        {
            if (o.GetComponent<ItemPickup>().itemID == ID)
            {
                return o;
            }
        }
        //if item not found
        return null;
    }
}
