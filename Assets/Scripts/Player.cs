using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public enum PlayerStateIdentification
{
    Idle = 1,
    Block,
    Jab,
    Attack,
    Pickup,
}
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> listOfPlayers = new Dictionary<ushort, Player>();
    [SerializeField] private PlayerStateIdentification _currentPlayerStateIdentification = PlayerStateIdentification.Idle;
    public PlayerStateIdentification CurrentPlayerState
    {
        get => _currentPlayerStateIdentification;
        set
        {//if current is idle or you are switching to idle, switch
            if (_currentPlayerStateIdentification == PlayerStateIdentification.Idle || value == PlayerStateIdentification.Idle)
            {
                _currentPlayerStateIdentification = value;
            }//otherwise ignore it
        }
    }
    public ushort Identification { get; private set; }
    private string username;
    [SerializeField] private Color playerColour;
    private bool isLocal;
    public bool IsLocal
    {
        get
        {
            return isLocal;
        }
        private set
        {
            isLocal = value;
            if (isLocal)
            {
                Local = this;
            }
        }
    }
    public static Player Local { get; private set; }
    //public Transform cameraTransform;
    private Interpolator _interpolator;
    private Attack attackScript;
    public float health = 10;//10 is maximum health
    //public Vector2 joystick1;//last input from client
    //public Vector2 joystick2;//last input from client
    public bool[] inputs = new bool[3];//last inputs from client
    public PlayerMovement playerMovement;
    public static void Spawn(ushort identification, string username, Vector3 position)//for each client when a new client joins
    {
        if (listOfPlayers.ContainsKey(identification))
        {
            return;
        }
        Player Player;
        if (identification == NetworkManager.Singleton.Client.Id)
        {
            Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
            Player.IsLocal = true;
            Local = Player;
            //get main camera
        }
        else
        {
            Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
            Player.IsLocal = false;
        }
        Player.name = $"Player {identification}({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        Player.username = username;
        Player._interpolator = Player.GetComponent<Interpolator>();
        Player.attackScript = Player.GetComponent<Attack>();
        Player.playerMovement = Player.GetComponent<PlayerMovement>();
        Player.Identification = identification;
        listOfPlayers.Add(identification, Player);
    }
    public static void Spawn(ushort identification, string username, string skin)//server adding a new client
    {
        foreach (Player otherPlayer in listOfPlayers.Values)
        {//send info on all other players to the new player
            otherPlayer.SendSpawned(identification);
        }
        Player Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
        Player.name = $"Player {identification}({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        Player.Identification = identification;
        Player.IsLocal = identification == 0;
        Player.playerMovement = Player.GetComponent<PlayerMovement>();
        Player.username = string.IsNullOrEmpty(username) ? "Guest" : username;
        if (ColorUtility.TryParseHtmlString(skin, out Color colour))
        {
            //Player.playerColour = colour;
            Player.GetComponentInChildren<Renderer>().material.color = colour;
        }
        Player.SendSpawned();//send info on new player to all other players
        listOfPlayers.Add(identification, Player);//only then add the new player
    }
    private void SendSpawned()//send info on this instance of player to ALL players
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, (ushort)MessageIdentification.playerSpawned)));
    }
    private void SendSpawned(ushort toClientID)//send into on this instance of player to the client ID player
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, (ushort)MessageIdentification.playerSpawned)), toClientID);
    }
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Identification);
        message.AddString(username);
        message.AddVector3(transform.position);
        message.AddString(ColorUtility.ToHtmlStringRGB(playerColour));
        return message;
    }
    private void OnDestroy()
    {
        listOfPlayers.Remove(Identification);
    }
    [MessageHandler((ushort)MessageIdentification.spawn)]
    public static void SpawnNewPlayer(ushort fromClientIdentification, Message message)
    {
        if (!listOfPlayers.ContainsKey(fromClientIdentification))
        {
            Spawn(fromClientIdentification, message.GetString(), message.GetString());
        }
    }
    [MessageHandler((ushort)MessageIdentification.playerSpawned)]
    public static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    private void Move(ushort tick, Vector3 newPosition, Vector3 forward)
    {
        //transform.position = newPosition;
        _interpolator.NewUpdate(tick, newPosition);
        playerMovement.PlayerBody.forward = forward;
        /*if (!isLocal)
        {
            cameraTransform.forward = forward;
        }*/
    }
    [MessageHandler((ushort)MessageIdentification.playerPosition)]
    public static void UpdatePosition(Message message)
    {
        if (listOfPlayers.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetUShort(), message.GetVector3(), message.GetVector3());
        }
    }
    public void SetInputs(bool[] inputs)//, Vector3 joystick1, Vector3 joystick2
    {
        this.inputs = inputs;
        //this.joystick1 = joystick1;
        //this.joystick2 = joystick2;
    }
    /// <summary>
    /// The server sends the message of each player (e.g. who is attacking and blocking)
    /// </summary>
    private void SendState()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageIdentification.playerState);
        message.AddUShort(Identification);
        message.AddUShort((ushort)CurrentPlayerState);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    //message handler for client to handle player states
    [MessageHandler((ushort)MessageIdentification.playerState)]
    public static void ReceivePlayerStates(Message message)
    {
        if (listOfPlayers.TryGetValue(message.GetUShort(), out Player player))
        {
            if (!player.IsLocal)
            {
                player.HandlePlayerStates(message.GetUShort());
            }
        }
    }
    /// <summary>
    /// Client insance of player handler of player states
    /// </summary>
    private void HandlePlayerStates(int stateIdentification)
    {
        CurrentPlayerState = (PlayerStateIdentification)stateIdentification;
    }
    private void FixedUpdate()
    {
        if (NetworkManager.IsHost)
        {
            SendState();//172.27.14.102
        }
    }
    /// <summary>
    /// Client message hanlder to show the player that they took damage
    /// </summary>
    [MessageHandler((ushort)MessageIdentification.damage)]
    private static void GetDamage(Message message)
    {
        if (listOfPlayers.TryGetValue(message.GetUShort(), out Player playerAttacked))
        {
            if (listOfPlayers.TryGetValue(message.GetUShort(), out Player playerHit))
            {
                playerAttacked.attackScript.DealDamage(playerHit);
            }
        }
    }
}
