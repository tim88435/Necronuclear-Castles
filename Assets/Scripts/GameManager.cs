using Riptide;
//using System;
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
    public GameObject[] spawnableWeapons;
    //not static method so that it could be called by unity events in the scene
    public void ChangeScene(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 0://Main Menu
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.CloseServer();
                    if (NetworkManager.Singleton.Client != null)
                    {
                        if (NetworkManager.Singleton.Client.IsConnected)
                        {
                            NetworkManager.Singleton.Client.Disconnect();
                        }
                    }
                }
                if (CurrentGameState == GameState.MainMenu)
                {
                    break;
                }
                CurrentGameState = GameState.MainMenu;
                //Removes the gameobject with the managers
                //(there is already one in the main menu scene)
                GameObject.Destroy(gameObject);
                SceneManager.sceneLoaded -= SceneLoaded;
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
    //not static method so that it could be called by unity events in the scene
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
                SpawnPlayer(" ", UIManager.Singleton.PlayerSkin);//name is blank now, name to be set later in dev
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
    private void SpawnPlayer(string name, string colour)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIdentification.spawn);
        message.AddString(name);
        message.AddString(colour);
        NetworkManager.Singleton.Client.Send(message);
    }
    public static GameObject GetWeapon(ushort index)
    {
        if (index < 0)
        {
            Debug.Log($"Index {index} is negative");
            return null;
        }
        if (Singleton.spawnableWeapons.Length <= index)
        {
            Debug.Log($"Set weapon at index {index} not found");
            return null;
        }
        return Singleton.spawnableWeapons[index];
    }
    public static ushort GetWeapon(Weapon weapon)
    {
        for (int i = 0; i < Singleton.spawnableWeapons.Length; i++)
        {
            if (Singleton.spawnableWeapons[i].GetComponent<ItemPickup>().weapon == weapon)
            {
                return (ushort)i;
            }
        }
        Debug.Log($"No saved weapon called {weapon.weaponName} exists");
        return 0;
    }
    public IEnumerator EndGame()
    {
        if (!NetworkManager.IsHost)
        {
            StopCoroutine(EndGame());
        }
        while (NetworkManager.Singleton.Server.ClientCount >= 1)
        {
            yield return null;
        }
        Singleton.ChangeScene(0);
    }
}
