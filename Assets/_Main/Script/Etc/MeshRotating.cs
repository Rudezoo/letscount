using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRotating : MonoBehaviour
{


    public float x = 0;
    public float y = 0;
    public float z = 0;
    void Start()
    {
        //tempPos = transform.position;
        //tempVal = transform.position.y;
    }
   
    void Update()
    {
        Quaternion localRottation = Quaternion.Euler(x, y, z);
        transform.rotation = transform.rotation * localRottation;

    }
}
