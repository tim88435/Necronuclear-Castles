using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject _playerModel;//this is the player model
    public GameObject PlayerModel
    {
        get => _playerModel;
        set
        {
            _playerModel = value;
            GameManager.Singleton.LocalPlayerPrefab = value;
        }
    }

    private static UIManager _singleton;
    public uint gamesWon;

    public static UIManager Singleton
    {
        get => _singleton;
        set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning($"{typeof(UIManager)} already exists in the current scene!\nRemoving Duplicate");
                DestroyImmediate(value);
            }
        }
    }

    void Awake()
    {
        Singleton = this;//singleton stuff
        DontDestroyOnLoad(gameObject);
        gamesWon = (uint)Random.Range(0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu(GameObject menu)//open menu, used on buttons
    {
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)//close menu, used on buttons
    {
        menu.SetActive(false);
    }
}
