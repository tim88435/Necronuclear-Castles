using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> listOfPlayers = new Dictionary<ushort, Player>();
    public ushort Identification { get; private set; }
    private string username;
    public bool isLocal { get; private set; }
    public Transform cameraTransform;
    [SerializeField] private Interpolator _interpolator;
    public Vector2 joystick1;//last input from client
    public Vector2 joystick2;//last input from client
    public bool[] inputs = new bool[3];//last inputs from client
    public static void Spawn(ushort identification, string username, Vector3 position)//for each client when a new client joins
    {
        Player Player;
        if (identification == NetworkManager.Singleton.Client.Id)
        {
            Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
            Player.isLocal = true;
        }
        else
        {
            Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
            Player.isLocal = false;
        }
        Player.name = $"Player {identification}({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        Player.username = username;
        Player.Identification = identification;
        listOfPlayers.Add(identification, Player);
    }
    public static void Spawn(ushort identification, string username)//server adding a new client
    {
        foreach (Player otherPlayer in listOfPlayers.Values)
        {//send info on all other players to the new player
            otherPlayer.SendSpawned(identification);
        }
        Player Player = Instantiate(GameManager.Singleton.playerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
        Player.name = $"Player {identification}({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        Player.Identification = identification;
        Player.username = string.IsNullOrEmpty(username) ? "Guest" : username;
        Player.SendSpawned();//send info on new player to all other players
        listOfPlayers.Add(identification, Player);//ony then add the new player
    }
    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.playerSpawned)));
    }
    private void SendSpawned(ushort toClientID)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.playerSpawned)), toClientID);
    }
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Identification);
        message.AddString(username);
        message.AddVector3(transform.position);
        return message;
    }
    private void OnDestroy()
    {
        listOfPlayers.Remove(Identification);
    }
    [MessageHandler((ushort)ClientToServerId.spawn)]
    private static void SpawnNewPlayer(ushort fromClientIdentification, Message message)
    {
        if (!listOfPlayers.ContainsKey(fromClientIdentification))
        {
            Spawn(fromClientIdentification, message.GetString());
        }
    }
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    private void Move(ushort tick, Vector3 newPosition, Vector3 forward)
    {
        //transform.position = newPosition;
        _interpolator.NewUpdate(tick, newPosition);
        if (!isLocal)
        {
            cameraTransform.forward = forward;
        }
    }
    [MessageHandler((ushort)ServerToClientId.playerPosition)]
    private static void UpdatePosition(Message message)
    {
        if (listOfPlayers.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetUShort(), message.GetVector3(), message.GetVector3());
        }
    }
    public void SetInputs(bool[] inputs, Vector3 joystick1, Vector3 joystick2)
    {
        this.inputs = inputs;
        this.joystick1 = joystick1;
        this.joystick2 = joystick2;
    }
}
