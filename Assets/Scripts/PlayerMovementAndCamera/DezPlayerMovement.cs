using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DezPlayerMovement : MonoBehaviour
{

    private Vector3 _inputMovement, _inputRotation;
    private Vector3 _moveDirection = Vector3.zero;

    public float MoveSpeed;
    public float BlockSpeed;


    //
    private CharacterController _controller; //NEEDED!
    public Joystick TranslationJoystick, RotationalJoystick; //this is for the joystick(s) input
    public Transform PlayerBody;

    private void Awake()
    {
        if ( GetComponent<CharacterController>() == null)
        {
            Debug.LogError( "There is no character controller on the player object" );
        }
        else
        {
            _controller = GetComponent<CharacterController>(); 
        }
        //_joystick = FindObjectOfType<Joystick>();
    }
    private void Update()
    {
        _inputMovement = new Vector3( TranslationJoystick.input.x, 0, TranslationJoystick.input.y );
        if ( RotationalJoystick.InputUpdate )
        {
            _inputRotation = new Vector3( RotationalJoystick.input.x, 0, RotationalJoystick.input.y );
        }
        

        singleStickMovement();
    }
    public void singleStickMovement()
    {
        _moveDirection = _inputMovement * MoveSpeed * Time.deltaTime;
        _controller.Move( _moveDirection );
        //_controller.transform.rotation = Quaternion.LookRotation( _inputRotation );
        PlayerBody.rotation = Quaternion.LookRotation( _inputRotation );

    }
}
