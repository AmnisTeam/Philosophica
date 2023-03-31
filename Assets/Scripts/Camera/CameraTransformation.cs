using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransformation : MonoBehaviour
{
    //public float speed;
    //public float scalingSpeed = 1;
    //public float minScale = 1;
    //public float maxScale = 20;

    //private Camera cam;
    //private Vector3 dragOrigin;

    //private void Start()
    //{
    //    cam = gameObject.GetComponent<Camera>();
    //}

    //private void cameraMovementByMouse()
    //{
    //    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

    //    if (Input.GetMouseButtonDown(0))
    //        dragOrigin = mouseWorldPos;

    //    if (Input.GetMouseButton(0))
    //    {
    //        Vector3 difference = dragOrigin - mouseWorldPos;
    //        cam.transform.position += difference;
    //    }
    //}

    //private void cameraScalingByMouse()
    //{
    //    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

    //    float scaling = Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * scalingSpeed;

    //    Vector2 cameraSize0 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
    //    Vector2 cameraLeftBottom0 = cam.transform.position.ToXY() - cameraSize0 / 2.0f;
    //    Vector2 precents = (mouseWorldPos.ToXY() - cameraLeftBottom0) / cameraSize0;

    //    cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * scalingSpeed;
    //    cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minScale, maxScale);

    //    if (Input.GetAxis("Mouse ScrollWheel") != 0)
    //    {
    //        Vector2 cameraSize1 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
    //        Vector2 cameraLeftBottom1 = mouseWorldPos.ToXY() - precents * cameraSize1;
    //        Vector2 cameraCenter = cameraLeftBottom1 + cameraSize1 / 2.0f;

    //        cam.transform.position = new Vector3(cameraCenter.x, cameraCenter.y, cam.transform.position.z);
    //    }
    //}

    //private void HandleCameraTransformationMouse()
    //{
    //    cameraMovementByMouse();
    //    cameraScalingByMouse();
    //}

    //private void HandleCameraTransformationTouch() // Most likely it doesn't work yet
    //{
    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
    //    {
    //        Vector2 TouchDeltaPosition = Input.GetTouch(0).deltaPosition;
    //        transform.Translate(TouchDeltaPosition.x * speed, TouchDeltaPosition.y * speed, 0);
    //    }
    //}
    //private void Update()
    //{
    //    HandleCameraTransformationMouse();
    //    HandleCameraTransformationTouch();
    //}

    public class WorkDetector
    {
        public Dictionary<string, bool> lokers;

        public WorkDetector()
        {
            lokers = new Dictionary<string, bool>();
        }

        public void AddLoker(string tag)
        {
            lokers[tag] = true;
        }

        public void RemoveLocker(string tag)
        {
            lokers.Remove(tag);
        }

        public bool getPass()
        {
            return lokers.Count == 0;
        }
    }

    public float speed;
    public float scalingSpeed = 1;
    public float scalingSpeedTouch = 0.1f;
    public float minScale = 1;
    public float maxScale = 20;
    public Vector2 clampingSize;
    public WorkDetector workDetector;
    public Canvas canvas;
    private bool oldIsWord = false;
    public bool isDrag = false;

    private Camera cam;
    private Vector3 dragOrigin;

    public bool isLocked = false;

    private Vector2 f0Start, f1Start;
    private float oldTouchScaling = 0;
    private int oldCountTouch;

    private void Awake()
    {
        workDetector = new WorkDetector();
    }

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    private void cameraMovementByMouse()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(null);
        eventData.position = Input.mousePosition;
        List<UnityEngine.EventSystems.RaycastResult> resultAppendList = new List<UnityEngine.EventSystems.RaycastResult>();
        graphicRaycaster.Raycast(eventData, resultAppendList);

        if (Input.GetMouseButtonDown(0) && resultAppendList.Count == 0)
        {
            dragOrigin = mouseWorldPos;
            isDrag = true;
            GetComponent<MoveCameraToActiveRegion>().UnsetTarget();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }

        if (Input.GetMouseButton(0) && isDrag)
        {
            Vector3 difference = dragOrigin - mouseWorldPos;
            cam.transform.position += difference;
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -clampingSize.x, clampingSize.x),
                                                 Mathf.Clamp(cam.transform.position.y, -clampingSize.y, clampingSize.y),
                                                 cam.transform.position.z);
        }
    }

    private void cameraScalingByMouse()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        float scaling = Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * scalingSpeed;

        if (Input.touchCount == 2)
        {
            if (oldCountTouch != Input.touchCount)
            {
                f0Start = Input.GetTouch(0).position;
                f1Start = Input.GetTouch(1).position;
            }

            Vector2 f0Position = Input.GetTouch(0).position;
            Vector2 f1Position = Input.GetTouch(1).position;

            mouseWorldPos = cam.ScreenToWorldPoint((f0Position + f1Position) / 2);

            float deltaScaling = Vector2.Distance(f0Position, f1Position) - Vector2.Distance(f0Start, f1Start);
            if (oldCountTouch != Input.touchCount)
            {
                oldTouchScaling = scaling;
            }
            scaling = (deltaScaling - oldTouchScaling) * scalingSpeedTouch;
            oldTouchScaling = deltaScaling;
        }

        Vector2 cameraSize0 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
        Vector2 cameraLeftBottom0 = cam.transform.position.ToXY() - cameraSize0 / 2.0f;
        Vector2 precents = (mouseWorldPos.ToXY() - cameraLeftBottom0) / cameraSize0;

        cam.orthographicSize -= scaling;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minScale, maxScale);

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Vector2 cameraSize1 = new Vector2(2.0f * cam.orthographicSize * cam.aspect, 2.0f * cam.orthographicSize);
            Vector2 cameraLeftBottom1 = mouseWorldPos.ToXY() - precents * cameraSize1;
            Vector2 cameraCenter = cameraLeftBottom1 + cameraSize1 / 2.0f;

            cam.transform.position = new Vector3(cameraCenter.x, cameraCenter.y, cam.transform.position.z);
        }

        oldCountTouch = Input.touchCount;
    }

    private void HandleCameraTransformationMouse()
    {
        if (workDetector.getPass())
            if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.Android && Input.touchCount == 1))
                cameraMovementByMouse();

        cameraScalingByMouse();
    }

    public void UpdateIsWork()
    {
        if (oldIsWord != workDetector.getPass())
        {
            if (workDetector.getPass())
                if (Input.GetMouseButton(0))
                {
                    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    dragOrigin = mouseWorldPos;
                }
            oldIsWord = workDetector.getPass();
        }
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
        UpdateIsWork();
        if (!isLocked)
        {
            HandleCameraTransformationMouse();
            //HandleCameraTransformationTouch();
        }
    }
}
