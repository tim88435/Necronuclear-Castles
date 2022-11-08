using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    public Transform target;
    public Vector3 targetDir;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetDir = target.position - transform.position;
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
    }
}
