using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DestroyOnClick : MonoBehaviour
{
    [Header("������������ ���������� �� ������� �������")]
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
