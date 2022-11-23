using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private string _playerSkin;//this is the player colour
    public string PlayerSkin { get => _playerSkin; set { _playerSkin = value; } }
    public GameObject currentUI;//this is the current UI that the player is seeing

    private static UIManager _singleton;
    public int gamesWon;

    #region Variables

    private ButtonHold _blockButton;
    public ButtonHold BlockButton { get => _blockButton; set { _blockButton = value; } }

    private GameObject _pickupButton;
    public GameObject PickupButton { get => _pickupButton; set { _pickupButton = value; } }

    private GameObject _pausePanel;
    public GameObject PausePanel { get => _pausePanel; set { _pausePanel = value; } }

    #endregion
    
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
                DestroyImmediate(value.gameObject);
            }
        }
    }

    void Awake()
    {
        Singleton = this;//singleton stuff
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //load games won regkey, if no key then generate one
        if (PlayerPrefs.HasKey("gamesWon"))
        {
            gamesWon = PlayerPrefs.GetInt("gamesWon");
        }
        else
        {
            PlayerPrefs.SetInt("gamesWon", 0);
        }
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

    public void EndGame(bool won)//use this when game ends, increases games won if the player wins
    {
        if (won)
        {
            PlayerPrefs.SetInt("gamesWon", gamesWon + 1);
        }
    }
    #region Events
    //for button & testing, player picks up weapon
    public void Pickup()
    {
        Player.Local.GetComponentInChildren<Attack>().Pickup();
    }
    
    //for button & testing, player attacks
    public void Attack()
    {
        Player.Local.GetComponent<Attack>().Swing();
    }

    public void Jab()
    {
        localPlayer.GetComponent<Attack>().Swing();
    }

    //open pause panel

    public void Pause()
    {
        PausePanel.SetActive(true);
    }

    public void Unpause()
    {
        PausePanel.SetActive(false);
    }

    #endregion
}
