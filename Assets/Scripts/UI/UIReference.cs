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
        UIManager.Singleton.PausePanel = GameObject.Find("Pause Panel");
        UIManager.Singleton.PausePanel.SetActive(false);
    }
    
    //more listener event testing
    public void PickUpButton()
    {
        UIManager.Singleton.SendMessage("Pickup", SendMessageOptions.DontRequireReceiver);
    }

    public void AttackButton()
    {
        UIManager.Singleton.SendMessage("Attack", SendMessageOptions.DontRequireReceiver);
    }

    public void JabButton()
    {
        UIManager.Singleton.SendMessage("Jab", SendMessageOptions.DontRequireReceiver);
    }

    public void PauseButton()
    {
        UIManager.Singleton.SendMessage("Pause", SendMessageOptions.DontRequireReceiver);
    }

    public void ContinueButton()
    {
        UIManager.Singleton.SendMessage("Unpause", SendMessageOptions.DontRequireReceiver);
    }

    public void QuitButton()
    {
        UIManager.Singleton.SendMessage("QuitGame", SendMessageOptions.DontRequireReceiver);
    }

    public void MainMenuButton()
    {
        UIManager.Singleton.SendMessage("ChangeScene", 0, SendMessageOptions.DontRequireReceiver);
    }
}