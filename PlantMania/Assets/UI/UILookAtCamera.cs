using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
   private Transform cp;


    void Update()
    {
        cp = GameObject.FindWithTag("CameraPosition").transform;

        Vector3 v = cp.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cp.position - v);
        transform.Rotate(0, 180, 0);
    }
}
