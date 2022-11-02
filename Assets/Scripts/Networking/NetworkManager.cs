using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;
public enum ServerToClientId : ushort
{
    sync = 1,
    playerSpawned,
    playerPosition,
}
public enum ClientToServerId : ushort//this will contain all the ids for messages that we send from the client to the server
{
    name = 1,
    inputs,
}
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        //Property Read is public by default and reads the instance
        get => _singleton;
        private set
        {
            //Property private write sets instance to the value if the instance is null
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)//if there is already one in the scene:
            {
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists\nRemove Duplicate");//warn the user
                //throw new System.Exception($"{nameof(NetworkManager)} instance already exists!");
                Destroy(value);//remove the duplicate
            }
        }
    }
    public Server Server { get; private set; }//Sever
    public Client Client { get; private set; }//Client
    public ushort CurrentTick { get; private set; } = 0;//Server
    private ushort _serverTick;//Client
    public ushort ServerTick//Client
    {
        get => _serverTick;
        private set
        {
            _serverTick = value;
            InterpolationTick = value;//(ushort)(value - TicksBetweenPositionUpdates);//updates the interpolation tick
        }
    }
    public ushort InterpolationTick { get; private set; }//Client
    private ushort _ticksBetweenPositionUpdates = 2;//Client
    public ushort TicksBetweenPositionUpdates//Client
    {
        get => _ticksBetweenPositionUpdates;
        /*private set
        {
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(_serverTick - value);
        }*/
    }
    [SerializeField] private ushort s_port;//Both
    [SerializeField] private string s_InternetProtocol;//Client
    [SerializeField] private ushort tickDivergenceTolerance = 1;//Client
    [SerializeField] private ushort s_maxClientCount;//Server
    private void Awake()
    {
        //when this script is made, set the instance ot this
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        //Logs what the network is doing
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        CheckClient();
    }
    public void StartServer()
    {
        //create new server at port XXXX with X amount of clients
        Server = new Server();
        Server.Start(s_port, s_maxClientCount);
        //when a client leaves the server, run the PlayerLeftFunction
        Server.ClientDisconnected += PlayerLeft;
    }
    public void StartClient()
    {

    }
    private void CheckClient()
    {
        if (Client == null)
        {
            Client = new Client();
            //Connect
            Client.Connected += Connected;
            //ConnectionFailed
            Client.ConnectionFailed += Failed;
            //Disconnect
            Client.Disconnected += Disconnected;
            Client.ClientDisconnected += PlayerLeft;
            ServerTick = 2;
        }
    }
    private void Failed(object sender, ConnectionFailedEventArgs e)//Client for when connection failed to establish
    {
        //Bring back to main Menu
    }

    private void Connected(object sender, EventArgs eventArgs)//Client for when connection established
    {
        CheckClient();
        //UIManager.UIManagerInstance.SendName();
    }

    private void Disconnected(object sender, DisconnectedEventArgs e)//Client when disconnected after connection established
    {
        ServerTick = 2;
    }

    //Checking Server Activity
    public void ServerFixedUpdate()//Server
    {
        Server.Update();
        if (CurrentTick % 200 == 0)
        {
            SendTick();
        }
        CurrentTick++;
    }
    public void PlayerLeft(object sender, ClientDisconnectedEventArgs eventArgs)//Client
    {
        //when the player leaves the server destroy the player object and remove from list
        if (Player.listOfPlayers.TryGetValue(eventArgs.Id, out Player player))
        {
            GameObject.Destroy(player.gameObject);
        }
    }
    public void PlayerLeft(object sender, ServerDisconnectedEventArgs eventArgs)//Server
    {
        //when the player leaves the server destroy the player object and remove from list
        if (Player.listOfPlayers.TryGetValue(eventArgs.Client.Id, out Player player))
        {
            GameObject.Destroy(player.gameObject);
        }
    }
    private void SendTick()//Server
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.sync);
        message.AddUShort(CurrentTick);
        Server.SendToAll(message);
    }
    private void SetTick(ushort serverTick)//Client
    {
        if (Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance)
        {
            Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }
    [MessageHandler((ushort)ServerToClientId.sync)]
    public static void Sync(Message message)//Client
    {
        Singleton.SetTick(message.GetUShort());
    }
    public void Connect()//Server
    {
        Client.Connect($"{s_InternetProtocol}:{s_port}");
    }
}
