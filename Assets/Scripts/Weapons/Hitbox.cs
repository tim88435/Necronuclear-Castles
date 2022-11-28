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
            //if the other player is in front of the current player
            if(Vector3.Angle(other.transform.position - transform.position, transform.root.forward) < 45)
            {
                //send hit message
                Debug.Log("player hit");
                script.SendHit(other.GetComponent<Player>());
            }
            else
            {
                Debug.Log("player miss");
            }
        }
        //Debug.Log(other.name + Vector3.Angle(other.transform.position - transform.position, transform.root.forward));
    }
}
