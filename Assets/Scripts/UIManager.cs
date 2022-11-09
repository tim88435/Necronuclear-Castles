using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private string _playerSkin;//this is the player colour
    public string PlayerSkin { get; set; }
    public GameObject currentUI;//this is the current UI that the player is seeing
    
    private static UIManager _singleton;
    public uint gamesWon;

    public ButtonHold _blockButton;
    public ButtonHold BlockButton { get => _blockButton; set { _blockButton = value; } }

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
