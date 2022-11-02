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
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Interpolator _interpolator;
    public static void Spawn(ushort identification, string username, Vector3 position)
    {
        Player Player;
        if (identification == NetworkManager.Singleton.Client.Id)
        {
            Player = Instantiate(GameManager.Singleton.LocalPlayerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
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
    private void OnDestroy()
    {
        listOfPlayers.Remove(Identification);
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
            _cameraTransform.forward = forward;
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
}
