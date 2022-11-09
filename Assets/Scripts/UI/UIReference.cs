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
    }
}
