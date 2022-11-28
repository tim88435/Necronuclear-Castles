using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
public class PlayerMovement : MonoBehaviour
{
    private Vector3 _inputMovement, _inputRotation;
    private Vector3 _moveDirection = Vector3.zero;


    [SerializeField] private float _movementSpeed = 6f;
    [SerializeField] private float _blockSpeed = 3f;
    [SerializeField] private Joystick TranslationJoystick;
    [SerializeField] private Joystick RotationalJoystick; 

    public Transform PlayerBody;
    private Player _player;
    private CharacterController _controller;
    private DezCamera _camera;
    //[SerializeField] private Joystick joystick2;


    private void OnEnable()
    {
        if ( GetComponent<CharacterController>() == null )
        {
            Debug.LogError( "There is no character controller on the player object" );
        }
        else
        {
            _controller = GetComponent<CharacterController>();
        }
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }
        if (TranslationJoystick == null)
        {
            TranslationJoystick = GameObject.Find("Joystick Panel").GetComponent<Joystick>();
        }
        if ( RotationalJoystick == null )
        {
            RotationalJoystick = GameObject.Find( "Joystick Panel" ).GetComponent<Joystick>();
        }
    }
    private void FixedUpdate()
    {
        if (_player.IsLocal)
        {
            _camera = Camera.main.GetComponent<DezCamera>();
            if ( _camera != null )
            {
                _camera.PlayerBody = PlayerBody;
            }
            SetInput(RotationalJoystick.input, TranslationJoystick.input);
        }
        if (NetworkManager.IsHost)
        {
            //Move(_player.joystick1, _player.inputs);//, _player.joystick2
            JoystickMovement();
        }
        else
        {
            SendInputs();
            _player.inputs[1] = false;
            _player.inputs[2] = false;
        }
    }
    private void JoystickMovement()
    {
        _moveDirection = _inputMovement * _movementSpeed * Time.fixedDeltaTime;
        _controller.Move( _moveDirection );
        //_controller.transform.rotation = Quaternion.LookRotation( _inputRotation );
        PlayerBody.rotation = Quaternion.LookRotation(_inputRotation);
        SendMovement();
    }
    private void Move(Vector2 inputDirection1, bool[] inputs)//, Vector2 inputDirection2
    {
        //Debug.Log(inputDirection1);
        Vector2 moveDirection = inputDirection1;// + inputDirection2;
        //block stuff idk @Dez
        _controller.Move(moveDirection);
        SendMovement();
    }
    public void SetInput(Vector2 RotationInput, Vector2 TranslationInput)
    {
        //_player.joystick1 = input;
        _inputMovement = new Vector3( TranslationJoystick.input.x, 0, TranslationJoystick.input.y );
        if ( RotationalJoystick.InputUpdate )
        {
            _inputRotation = new Vector3( RotationalJoystick.input.x, 0, RotationalJoystick.input.y );
        }

    }
    /*public void SetInput(Vector2 input, Vector2 input2)
    {
        _player.joystick1 = input;
        _player.joystick2 = input2;
    }*/
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
        message.AddVector3(PlayerBody.forward);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageIdentification.inputs);
        message.AddBools(_player.inputs, false);//buttons
        message.AddVector3(TranslationJoystick.input);//joystick 1
        //message.AddVector3(_player.joystick2);//joystick 2
        NetworkManager.Singleton.Client.Send(message);
    }
    [MessageHandler((ushort)MessageIdentification.inputs)]
    public static void GetInputs(ushort fromClientIdentification, Message message)
    {
        if (Player.listOfPlayers.TryGetValue(fromClientIdentification, out Player player))
        {
            player.SetInputs(message.GetBools(3));//, message.GetVector3(), message.GetVector3()
            Vector3 input = message.GetVector3();
            player.playerMovement._inputMovement = new Vector3(input.x, 0, input.y);
            if (input != Vector3.zero)
            {//if there isn't any ipduts, don't update the rotation
                player.playerMovement._inputRotation = player.playerMovement._inputMovement;//input movement is rotation for now
            }
        }
    }
}
