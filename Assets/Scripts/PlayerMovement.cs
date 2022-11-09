using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _blockSpeed;
    private void OnValidate()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }
    }
    private void FixedUpdate()
    {
        if (NetworkManager.IsHost)
        {
            Move(_player.joystick1, _player.joystick2, _player.inputs);
        }
        else
        {
            SendInputs();
        }
    }
    private void Move(Vector2 inputDirection1, Vector2 inputDirection2, bool[] inputs)
    {
        Vector2 moveDirection = inputDirection1 + inputDirection2;
        //block stuff idk @Dez
        _characterController.Move(moveDirection);
        SendMovement();
    }
    public void SetInput(bool[] inputs, Vector3 forward)
    {
        _player.inputs = inputs;
        transform.forward = forward;
    }
    public void SendMovement()
    {
        if (NetworkManager.Singleton.CurrentTick % 2 != 0)
        {
            return;
        }
        Message message = Message.Create(MessageSendMode.Unreliable, MessageIdentification.playerPosition);
        message.AddUShort(_player.Identification);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageIdentification.inputs);
        message.AddBools(_player.inputs, false);//buttons
        message.AddVector3(Vector3.zero);//joystick 1
        message.AddVector3(Vector3.zero);//joystick 2
        NetworkManager.Singleton.Client.Send(message);
    }
    [MessageHandler((ushort)MessageIdentification.inputs)]
    private static void GetInputs(ushort fromClientIdentification, Message message)
    {
        if (Player.listOfPlayers.TryGetValue(fromClientIdentification, out Player player))
        {
            player.SetInputs(message.GetBools(3), message.GetVector3(), message.GetVector3());
        }
    }
}
