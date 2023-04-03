using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnIntersect : MonoBehaviour
{
    [Header("������������ ���������� �� ������� �������")]
    public DestoryObject destroyObject;

    [Header("���������� ����������")]
    public Collider2D[] colliders;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int x = 0; x < colliders.Length; x++)
            if (collision == colliders[x])
            {
                destroyObject.ToDestroy();
                break;
            }
    }
}
