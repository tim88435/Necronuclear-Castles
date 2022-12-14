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
    [SerializeField] private GameObject weaponModel;

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
        if (player.IsLocal)
        {
            GameObject.Find("Weapon Image").GetComponent<Image>().sprite = weapon.image;
            GameObject.Find("Weapon Name").GetComponent<TextMeshProUGUI>().text = weapon.weaponName;
        }
        _weaponHitbox.radius = weapon.length;
        _weapon = weapon;
        weaponModel.GetComponent<MeshFilter>().mesh = weapon.model;
        weaponModel.GetComponent<MeshRenderer>().material = weapon.skin;
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
    //disables hitbox when time is up, and moves cooldown forward
    private void FixedUpdate()
    {
        if (NetworkManager.IsHost)
        {
            if (player.inputs[2])
            {
                Swing(true);
            }
            else if (player.inputs[1])
            {
                Swing(false);
            }
            _weaponCooldown -= Time.fixedDeltaTime;
            _weaponDuration--;
            if (_weaponDuration <= 0)
            {
                _weaponHitbox.enabled = false;
                if (player.CurrentPlayerState == PlayerStateIdentification.Attack || player.CurrentPlayerState == PlayerStateIdentification.Jab)
                {
                    player.CurrentPlayerState = PlayerStateIdentification.Idle;
                }
            }
        }
    }
    //turns on weapon hitbox for 10 frames
    //jab bool means if attack is jab or not
    public void Swing(bool jab)
    {
        if (jab)
        {
            player.inputs[2] = true;
        }
        else
        {
            player.inputs[1] = true;
        }
        if (!NetworkManager.IsHost)
        {
            return;
        }
        if (_weaponCooldown <= 0)
        {
            _weaponHitbox.enabled = true;
            player.CurrentPlayerState = jab ? PlayerStateIdentification.Jab : PlayerStateIdentification.Attack;
            _weaponDuration = 10;
            _weaponCooldown = _weapon.cooldown + 0.2f;//0.2f is 10 frames of fixedupdate
            player.inputs[1] = false;
            player.inputs[2] = false;
        }
    }
    
    /// <summary>
    /// Send the information to the players (from ther server) that a player has gotten hit by this player
    /// </summary>
    /// <param name="otherPlayer">Player that has gotten hit</param>
    public void SendHit(Player otherPlayer)
    {
        Debug.Log("Attack message sent");
        Message message = Message.Create(MessageSendMode.Reliable, MessageIdentification.damage);
        message.AddUShort(player.Identification);//player who hit
        message.AddUShort(otherPlayer.Identification);//player who got hit
        message.AddUShort((ushort)player.CurrentPlayerState);//if player is attacking or jabbing
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    /// <summary>
    /// Call damage a player (for both server and client)
    /// </summary>
    /// <param name="player">The other player that just got hit</param>
    public void DealDamage(Player player)
    {
        //this instance is the attacker that hit the other player
        //player is the other player that just got hit
        float blockMult = player.CurrentPlayerState == PlayerStateIdentification.Block ? 0.2f : 1;
        player.health -= _weapon.damage * blockMult;
        if (player.health <= 0)
        {
            player.Kill();
        }
    }

    //knockbacks other player in direction
    //player is other player
    public void KnockBack(Player player)
    {
        float blockMult = player.CurrentPlayerState == PlayerStateIdentification.Block ? 0.2f : 1;
        player.GetComponent<CharacterController>().Move(transform.GetChild(0).forward * _weapon.damage * blockMult); //moves other player in player direction
    }
    
    [MessageHandler((ushort)MessageIdentification.pickup)]
    public static void PickupHandler(Message message)
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
