using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBackground : MonoBehaviour
{
    public Camera cam;
    void Start()
    {
        
    }

    void Update()
    {
        GetComponent<Renderer>().material.SetVector("_Offset", cam.transform.position);
    }
}
