using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Local player may not spawn in the first few frames
        //due to the player getting spawned when the server sends the info where each player is spawning
        if (Player.Local != null)
        {
            hpBar.fillAmount = Player.Local.health / 10f;
        }
    }
}
