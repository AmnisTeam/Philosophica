using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;

    private Camera cam;
    private Vector3 dragOrigin;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void HandleCameraMovementMouse()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }
    }

    void HandleCameraMovementTouch() // Most likely it doesn't work yet
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 TouchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(TouchDeltaPosition.x * speed, TouchDeltaPosition.y * speed, 0);
        }
    }
    void Update()
    {
        HandleCameraMovementMouse();
        HandleCameraMovementTouch();
    }
}
