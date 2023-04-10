using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraToActiveRegion : MonoBehaviour
{
    public float percentOfDistanceSpeed;
    public float minSpeed;
    public float minDistanceToStopMove;
    public Vector3 target;

    public bool isMove = false;

    private Action onComplete;

    public bool SetTarget(Vector2 target, Action onComplete = null)
    {
        if(!isMove)
        {
            this.target = target;
            this.onComplete = onComplete;
            GetComponent<CameraTransformation>().isDrag = false;
            isMove = true;
            return true;
        }
        return false;
    }

    public void UnsetTarget()
    {
        isMove = false;
        if(onComplete != null)
            onComplete();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(isMove)
        {
            float distance = Vector2.Distance(target, transform.position);
            Vector3 velocity = (target - transform.position) * distance * percentOfDistanceSpeed;
            Vector3 direction = Vector3.Normalize(target - transform.position);
            velocity.z = 0;
            direction.z = 0;
            transform.position += velocity + direction * minSpeed;

            distance = Vector2.Distance(target, transform.position);
            if(distance < minDistanceToStopMove)
            {
                UnsetTarget();
            }
        }
    }
}
