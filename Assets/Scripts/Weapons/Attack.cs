using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;//god i hate tmp
using Riptide;

public class Attack : MonoBehaviour
{
    [SerializeField] private SphereCollider _weaponHitbox;
    [SerializeField] private Weapon _weapon;//default is fist
    private Player player;

    [SerializeField] private GameObject nearbyPickup;

    [SerializeField] private int _weaponDuration;//how long the weapon hitbox stays active in fixedupdate frames
    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<Player>();
        //grab and disable weapon hitbox
        //_weaponHitbox = GetComponentInChildren<SphereCollider>();
        //_weaponHitbox.enabled = false;
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
            if(nearbyPickup == null)
                UIManager.Singleton.PickupButton.SetActive(false);
            else
                UIManager.Singleton.PickupButton.SetActive(true);
        }
        if (NetworkManager.IsHost || player.isLocal)
        {
            if (player.inputs[0])//if block button held
            {
                player.CurrentPlayerState = PlayerStateIdentification.Block;
            }
            else if (player.CurrentPlayerState == PlayerStateIdentification.Block)
            {
                player.CurrentPlayerState = PlayerStateIdentification.Idle;
            }
        }
    }

    private void FixedUpdate()
    {
        _weaponDuration--;
        if(_weaponDuration == 0)
            _weaponHitbox.enabled = false;
    }
    //turns on weapon hitbox for 10 frames
    public void Swing()
    {
        _weaponHitbox.enabled = true;
        _weaponDuration = 10;
    }
    /// <summary>
    /// Send the information to the players (from ther server) that a player has gotten hit by this player
    /// </summary>
    /// <param name="otherPlayer">Player that has gotten hit</param>
    private void SendHit(Player otherPlayer)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageIdentification.damage);
        message.AddUShort(player.Identification);//player who hit
        message.AddUShort(otherPlayer.Identification);//player who got hit
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    /// <summary>
    /// Call to damage a player
    /// </summary>
    /// <param name="player">The other player that just got hit</param>
    public void DealDamage(Player player)
    {
        //this instance is the attacker that hit the other player
        //player is the other player that just got hit
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Weapon")
            nearbyPickup = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Weapon")
            nearbyPickup = null;
    }
}
