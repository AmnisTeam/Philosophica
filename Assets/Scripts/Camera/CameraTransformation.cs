using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraTransformation : MonoBehaviour
{
    public float speed;
    public float scalingSpeed = 1;
    public float minScale = 1;
    public float maxScale = 20;

    private Camera cam;
    private Vector3 dragOrigin;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    private void cameraMovementByMouse()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
            dragOrigin = mouseWorldPos;

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - mouseWorldPos;
            cam.transform.position += difference;
        }
    }

    private void cameraScalingByMouse()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        float scaling = Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * scalingSpeed;

        Vector2 cameraSize0 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
        Vector2 cameraLeftBottom0 = cam.transform.position.ToXY() - cameraSize0 / 2.0f;
        Vector2 precents = (mouseWorldPos.ToXY() - cameraLeftBottom0) / cameraSize0;

        cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * scalingSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minScale, maxScale);

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Vector2 cameraSize1 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
            Vector2 cameraLeftBottom1 = mouseWorldPos.ToXY() - precents * cameraSize1;
            Vector2 cameraCenter = cameraLeftBottom1 + cameraSize1 / 2.0f;

            cam.transform.position = new Vector3(cameraCenter.x, cameraCenter.y, cam.transform.position.z);
        }
    }

    private void HandleCameraTransformationMouse()
    {
        cameraMovementByMouse();
        cameraScalingByMouse();
    }

    private void HandleCameraTransformationTouch() // Most likely it doesn't work yet
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 TouchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(TouchDeltaPosition.x * speed, TouchDeltaPosition.y * speed, 0);
        }
    }
    private void Update()
    {
        HandleCameraTransformationMouse();
        HandleCameraTransformationTouch();
    }
}
