using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryOverTime : MonoBehaviour
{
    [Header("Обязательные компоненты на текущем объекте")]
    public DestoryObject destroyObject;

    [Header("Переменные компонента")]
    public float timeToDestroy;
    private float timerToDestroy = 0;

    void Start()
    {
        
    }


    void Update()
    {
        timerToDestroy += Time.deltaTime;
        if(timerToDestroy > timeToDestroy)
        {
            destroyObject.ToDestroy();
            timerToDestroy = float.NaN;
        }
    }
}
