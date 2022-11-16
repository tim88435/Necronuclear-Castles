using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Player localPlayer;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = UIManager.Singleton.localPlayer.health / 10f;
    }
}
