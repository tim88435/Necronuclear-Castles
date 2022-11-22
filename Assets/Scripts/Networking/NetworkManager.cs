using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;
using UnityEngine.SceneManagement;
public enum MessageIdentification : ushort
{
    sync = 1,
    playerSpawned,
    playerPosition,
    startGame,
    playerState,
    spawn,
    inputs,
    damage,
    itemSpawn,
    tryPickup,
    pickup,
}
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;//Guys you know this
    public static NetworkManager Singleton//Guys you know this
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
    private static bool _isHost;
    public static bool IsHost { get; private set; }//is this computer hosting the server
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
            InterpolationTick = (ushort)(value - TicksBetweenPositionUpdates);//(ushort)(ServerTick - value);// updates the interpolation tick
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
    public ushort ServerPort
    {
        get => s_port;
        set
        {
            s_port = value;
        }
    }
    [SerializeField] private string s_InternetProtocol;//Client
    public string ServerInternetProtocol
    {
        get => s_InternetProtocol;
        set
        {
            s_InternetProtocol = value;
        }
    }
    [SerializeField] private ushort tickDivergenceTolerance = 1;//Client
    [SerializeField] private ushort s_maxClientCount;//Server
    private void Awake()
    {
        //when this script is made, set the instance ot this
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// Call this when you start a lobby that players can connect to
    /// </summary>
    public void StartServer()
    {
        //create new server at port XXXX with X amount of clients
        Server = new Server();
        Server.Start(s_port, s_maxClientCount);
        //when a client leaves the server, run the PlayerLeftFunction
        Server.ClientDisconnected += PlayerLeft;
        Server.ClientConnected += Connected;
        IsHost = true;
    }
    /// <summary>
    /// Call when you want to close the server lobby
    /// </summary>
    public void CloseServer()
    {
        Server.Stop();
        IsHost = false;
    }
    public void StartClient()
    {
        CheckClient();
    }
    private void CheckClient()
    {
        if (IsHost)
        {
            return;
        }
        if (Client == null)
        {
            Client = new Client();
            //Connect
            //Client.Connected += Connected;
            //ConnectionFailed
            //Client.ConnectionFailed += Failed;
            //Disconnect
            Client.Disconnected += Disconnected;
            Client.ClientDisconnected += PlayerLeft;
            ServerTick = 2;
        }
    }
    /*private void Failed(object sender, ConnectionFailedEventArgs e)//Client for when connection failed to establish
    {
        //Bring back to main Menu
    }*/

    private void Connected(object sender, EventArgs eventArgs)//Server for when connection established
    {
        if (Server.ClientCount == Server.MaxClientCount)
        {
            GameManager.Singleton.ChangeScene(1);
            StartGame();//tell all the clients that the server has started the game
        }
        //UIManager.UIManagerInstance.SendName();
    }
    /// <summary>
    /// Called when the server is entering the main game, an tells the clients to do the same
    /// </summary>
    private void StartGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageIdentification.startGame);
        message.AddVector3(Vector3.zero);//add the host player's starting position
        Server.SendToAll(message);
    }
    private void Disconnected(object sender, DisconnectedEventArgs e)//Client when disconnected after connection established
    {
        ServerTick = 2;
        //load the main menu
        GameManager.Singleton.ChangeScene(0);
    }

    //Checking Server Activity
    public void FixedUpdate()
    {
        if (IsHost)//Server
        {
            Server.Update();
            if (CurrentTick % 250 == 0)
            {
                SendTick();
            }
            CurrentTick++;
        }
        //client
        else if (Client != null)
        {
            Client.Update();
            ServerTick++;
        }
    }
    /// <summary>
    /// Called when a player has left
    /// </summary>
    public void PlayerLeft(object sender, ClientDisconnectedEventArgs eventArgs)//Client
    {
        //when the player leaves the server destroy the player object and remove from list
        if (Player.listOfPlayers.TryGetValue(eventArgs.Id, out Player player))
        {
            GameObject.Destroy(player.gameObject);
        }
    }
    /// <summary>
    /// Called when a player has left
    /// </summary>
    public void PlayerLeft(object sender, ServerDisconnectedEventArgs eventArgs)//Server
    {
        //when the player leaves the server destroy the player object and remove from list
        if (Player.listOfPlayers.TryGetValue(eventArgs.Client.Id, out Player player))
        {
            GameObject.Destroy(player.gameObject);
        }
        if (Server.ClientCount <= 0)
        {
            Server.Stop();
            GameManager.Singleton.ChangeScene(0);
        }
    }
    /// <summary>
    /// Send the current tick of ther server to all the clients
    /// </summary>
    private void SendTick()//Server
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageIdentification.sync);
        message.AddUShort(CurrentTick);
        Server.SendToAll(message);
    }
    /// <summary>
    /// Client listener to set the current tick
    /// </summary>
    /// <param name="serverTick"></param>
    private void SetTick(ushort serverTick)//Client
    {
        if (Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance)
        {
            Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }
    /// <summary>
    /// Client listener to revieve the current tick
    /// </summary>
    [MessageHandler((ushort)MessageIdentification.sync)]
    public static void Sync(Message message)//Client
    {
        Singleton.SetTick(message.GetUShort());
    }
    /// <summary>
    /// Call to connect to a server
    /// </summary>
    public void Connect()//Client
    {
        CheckClient();
        Client.Connect($"{s_InternetProtocol}:{s_port}");
    }
    private void OnDestroy()
    {
        if (Server != null)
        {
            Server.Stop();
        }
    }
    private void OnApplicationQuit()
    {
        if (Server != null)
        {
            Server.Stop();
        }
    }
}
