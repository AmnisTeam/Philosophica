using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraToActiveRegion : MonoBehaviour
{
    public double percentOfDistanceSpeed;
    public double minDistanceToStopMove;
    public GameObject target;

    public bool isMove = false;

    private Action onComplete;

    public bool SetTarget(GameObject target, Action onComplete)
    {
        if(!isMove)
        {
            this.target = target;
            this.onComplete = onComplete;
            isMove = true;
            GetComponent<CameraTransformation>().isLocked = true;
            return true;
        }
        return false;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(isMove)
        {
            float distance = Vector2.Distance(target.transform.position, transform.position);
            transform.position += (target.transform.position - transform.position) * distance;

            distance = Vector2.Distance(target.transform.position, transform.position);
            if(distance < minDistanceToStopMove)
            {
                isMove = false;
                target = null;
                GetComponent<CameraTransformation>().isLocked = false;
                onComplete();
            }
        }
    }
}
