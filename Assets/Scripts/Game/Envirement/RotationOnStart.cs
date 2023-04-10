using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotationOnStart : MonoBehaviour
{
    public float minStartRotation;
    public float maxStartRotation;

    void Start()
    {
        System.Random random = new System.Random();
        GetComponent<Rigidbody2D>().SetRotation((float)(minStartRotation + (maxStartRotation - minStartRotation) * random.NextDouble()));
    }
    void Update()
    {
        
    }
}
