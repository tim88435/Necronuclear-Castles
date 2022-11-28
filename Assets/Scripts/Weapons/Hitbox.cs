using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Attack script;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("something is here");
        //if sphere hits player and is not current player
        if(other.transform.root.tag == "Player" && other.transform.root != transform.root)
        {
            Debug.Log($"Hit {other.name}, checking if the angle ({Vector3.Angle(other.transform.position - transform.position, transform.root.forward)})is correct...");
            //if the other player is in front of the current player
            if(Vector3.Angle(other.transform.position - transform.position, transform.root.GetChild(0).forward) < 45)
            {
                //send hit message
                Debug.Log("player hit");
                Player otherPlayer = other.GetComponent<Player>();
                script.SendHit(otherPlayer);
                switch (GetComponentInParent<Player>().CurrentPlayerState)
                {
                    case PlayerStateIdentification.Jab:
                        script.KnockBack(otherPlayer);
                        break;
                    case PlayerStateIdentification.Attack:
                        script.DealDamage(otherPlayer);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.Log("player miss");
            }
        }
        //Debug.Log(other.name + Vector3.Angle(other.transform.position - transform.position, transform.root.forward));
    }
}
