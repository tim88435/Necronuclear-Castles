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
    private bool[] inputs = new bool[3];
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
        //if client
        Vector2 joystick1 = Vector2.zero;//Dez these are the inputs
        Vector2 joystick2 = Vector2.zero;//Dez these are the inputs
        //inputDirection from joysticks
        //inputs[4] = is blocking

        //if server
        Move(joystick1, joystick2, inputs);
    }
    private void Move(Vector2 inputDirection1, Vector2 inputDirection2, bool[] block)
    {
        Vector2 moveDirection = inputDirection1;
        //block stuff idk @Dez
        _characterController.Move(inputDirection1);
        SendMovement();
    }
    public void SetInput(bool[] inputs, Vector3 forward)
    {
        this.inputs = inputs;
        transform.forward = forward;
    }
    public void SendMovement()
    {
        if (NetworkManager.Singleton.CurrentTick % 2 != 0)
        {
            return;
        }
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.playerPosition);
        message.AddUShort(_player.Identification);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.inputs);
        message.AddBools(inputs, false);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
}
