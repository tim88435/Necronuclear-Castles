using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DezCamera : MonoBehaviour
{
    public Transform PlayerBody;

    public float SmoothSpeed = 0.125f;
    public Vector3 Offset;
    public float Distance;

    public Vector3 debugTarget;
    private void Awake()
    {
        Offset = transform.position; 
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.IsHost)
        {
            return;
        }
        Vector3 target = PlayerBody.position + (PlayerBody.forward * Distance); //target is right infront of the player * distance
        //target.y = PlayerBody.position.y; //this just makes it so the target is always at the players level, really not needed but i feel like i need to put it here anyway
        //target = transform.TransformVector(target);

        debugTarget = target;

        Vector3 CameraPosition = target + Offset;
        Vector3 SmoothedPosition;
        SmoothedPosition = Vector3.Lerp(transform.position, CameraPosition, SmoothSpeed * Time.fixedDeltaTime);
        transform.position = SmoothedPosition;
    }
    private void LateUpdate()
    {
        if (NetworkManager.IsHost)
        {
            return;
        }
        Vector3 target = PlayerBody.position + (PlayerBody.forward * Distance); //target is right infront of the player * distance
        //target.y = PlayerBody.position.y; //this just makes it so the target is always at the players level, really not needed but i feel like i need to put it here anyway
        //target = transform.TransformVector(target);

        debugTarget = target;

        Vector3 CameraPosition = target + Offset;
        Vector3 SmoothedPosition;
        SmoothedPosition = Vector3.Lerp(transform.position, CameraPosition, SmoothSpeed * Time.deltaTime);
        transform.position = SmoothedPosition;
    }
}
