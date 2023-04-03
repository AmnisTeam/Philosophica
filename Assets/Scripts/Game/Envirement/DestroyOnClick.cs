using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DestroyOnClick : MonoBehaviour
{
    [Header("Обязательные компоненты на текущем объекте")]
    public DestoryObject destroyObject;

    void Start()
    {
        
    }


    void Update()
    {

    }

    void OnMouseDown()
    {
        destroyObject.ToDestroy();
    }

}
