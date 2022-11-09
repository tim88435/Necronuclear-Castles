using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;//god i hate tmp
using Riptide;

public class Attack : MonoBehaviour
{
    private SphereCollider _weaponHitbox;
    [SerializeField] private Weapon _weapon;//default is fist
    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<Player>();
        //grab and disable weapon hitbox
        _weaponHitbox = GetComponentInChildren<SphereCollider>();
        _weaponHitbox.enabled = false;
        //set default weapon as fist
        SetWeapon(_weapon);
    }

    public void SetWeapon(Weapon weapon)//changes UI and hitbox to match weapon
    {
        GameObject.Find("Weapon Image").GetComponent<Image>().sprite = weapon.image;
        GameObject.Find("Weapon Name").GetComponent<TextMeshProUGUI>().text = weapon.weaponName;
        _weaponHitbox.radius = weapon.length;
        GameObject model = GameObject.Find("Weapon Model");
        model.GetComponent<MeshFilter>().mesh = weapon.model;
        model.GetComponent<MeshRenderer>().material = weapon.skin;
    }

    private void Update()
    {
        if (player.isLocal)
        {
            player.inputs[0] = UIManager.Singleton.BlockButton.buttonHeld;
        }
        if (player.inputs[0] == true)//if block button held
        {
            player.CurrentPlayerStateIdentification = PlayerStateIdentification.Block;
        }
        else if (player.CurrentPlayerStateIdentification == PlayerStateIdentification.Block)
        {
            player.CurrentPlayerStateIdentification = PlayerStateIdentification.Idle;
        }
    }
    public void Swing()
    {
        
    }
}
