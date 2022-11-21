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

    [SerializeField] private Collider[] nearbyPickup;

    [SerializeField] private int _weaponDuration;//how long the weapon hitbox stays active in fixedupdate frames
    [SerializeField] private float _weaponCooldown;//how long the weapon cools down for before being usable again
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

    private void SetWeapon(Weapon weapon)//changes UI and hitbox to match weapon
    {
        GameObject.Find("Weapon Image").GetComponent<Image>().sprite = weapon.image;
        GameObject.Find("Weapon Name").GetComponent<TextMeshProUGUI>().text = weapon.weaponName;
        _weaponHitbox.radius = weapon.length;
        GameObject model = GameObject.Find("Weapon Model");
        model.GetComponent<MeshFilter>().mesh = weapon.model;
        model.GetComponent<MeshRenderer>().material = weapon.skin;
    }

    //can be used for both player and enemy
    public void Pickup()
    {
        if (nearbyPickup.Length <= 0)//in case player picks up a weapon before the enemy
        {
            return;
        }
        if (NetworkManager.IsHost)
        {
            ItemPickup pickup = nearbyPickup[0].GetComponent<ItemPickup>();
            //set up weapon in ui
            SetWeapon(pickup.weapon);
            //tell the clients that this player has picked up the weapon
            pickup.SendPickedUp(player.Identification);
            //remove weapon from world
            Destroy(pickup.gameObject);
        }
        else
        {
            //tell the server that this client wants to pick up the weapon
            nearbyPickup[0].GetComponent<ItemPickup>().TryPickUp();
        }
        
    }

    private void Update()
    {
        //get list of all nearby pickups
        nearbyPickup = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Pickups"), QueryTriggerInteraction.Collide);//gets all pickups in radius 2

        if (player.IsLocal)
        {
            player.inputs[0] = UIManager.Singleton.BlockButton.buttonHeld;
            if(nearbyPickup.Length > 0)//if there are nearby pickups
                UIManager.Singleton.PickupButton.SetActive(true);
            else
                UIManager.Singleton.PickupButton.SetActive(false);
        }
        if (NetworkManager.IsHost || player.IsLocal)
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
        _weaponCooldown -= Time.deltaTime;
        _weaponDuration--;
        if(_weaponDuration == 0)
            _weaponHitbox.enabled = false;
    }
    //turns on weapon hitbox for 10 frames
    public void Swing()
    {
        if (_weaponCooldown <= 0)
        {
            _weaponHitbox.enabled = true;
            _weaponDuration = 10;
            _weaponCooldown = _weapon.cooldown + 0.2f;//0.2f is 10 frames of fixedupdate
        }
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
    /// Call damage a player
    /// </summary>
    /// <param name="player">The other player that just got hit</param>
    public void DealDamage(Player player)
    {
        //this instance is the attacker that hit the other player
        //player is the other player that just got hit
    }
    [MessageHandler((ushort)MessageIdentification.pickup)]
    public static void PickuoHandler(Message message)
    {
        if (Player.listOfPlayers.TryGetValue(message.GetUShort(), out Player player))
        {
            player.GetComponent<Attack>().WeaponPickedUp(message.GetUShort());
        }
    }
    private void WeaponPickedUp(ushort weaponIdentification)
    {
        ItemPickup itemPickup = ItemPickup.FindItem(weaponIdentification).GetComponent<ItemPickup>();
        SetWeapon(itemPickup.weapon);
        Destroy(itemPickup.gameObject);
    }
}
