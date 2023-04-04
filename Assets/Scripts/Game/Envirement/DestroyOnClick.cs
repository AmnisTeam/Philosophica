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
        GameObject obj = GameObject.FindWithTag("SOUND_EFFECTS_TAG");
        if (obj != null )
        {
            SoundEffects se = obj.GetComponent<SoundEffects>();
            se.PlaySound("break_meteorite");
        }

        destroyObject.ToDestroy();
    }

}
