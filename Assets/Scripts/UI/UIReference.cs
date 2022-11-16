using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReference : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        UIManager.Singleton.currentUI = gameObject;
        UIManager.Singleton.BlockButton = GameObject.Find("Block Button").GetComponent<ButtonHold>();
        UIManager.Singleton.PickupButton = GameObject.Find("Pickup Weapon");
        UIManager.Singleton.JabButton = GameObject.Find("Jab Button");
        UIManager.Singleton.AttackButton = GameObject.Find("Attack Button");
        UIManager.Singleton.PauseButton = GameObject.Find("Pause Button");
        UIManager.Singleton.ContinueButton = GameObject.Find("Continue Button");
        UIManager.Singleton.QuitButton = GameObject.Find("Quit Button");
        UIManager.Singleton.PausePanel = GameObject.Find("Pause Panel");
        UIManager.Singleton.PausePanel.SetActive(false);
    }

    private void Start()
    {
        //button tests
        UIManager.Singleton.PickupButton.GetComponent<Button>().onClick.AddListener(UIManager.Singleton.Pickup);
        UIManager.Singleton.JabButton.GetComponent<Button>().onClick.AddListener(UIManager.Singleton.Attack);
        UIManager.Singleton.AttackButton.GetComponent<Button>().onClick.AddListener(UIManager.Singleton.Attack);
        UIManager.Singleton.PauseButton.GetComponent<Button>().onClick.AddListener(UIManager.Singleton.Pause);
        UIManager.Singleton.ContinueButton.GetComponent<Button>().onClick.AddListener(UIManager.Singleton.Unpause);
        UIManager.Singleton.QuitButton.GetComponent<Button>().onClick.AddListener(GameManager.Singleton.QuitGame);
    }
}