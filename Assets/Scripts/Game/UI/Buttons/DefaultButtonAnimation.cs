using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DefaultButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Color idleColor;
    public Color pressColor;
    public Image coloredObject;

    public float timeTransition;

    public void OnPointerDown(PointerEventData eventData)
    {

        LeanTween.value(0, 1, timeTransition).setOnUpdate((float value) =>
        {
            if(coloredObject)
                coloredObject.color = idleColor + (pressColor - idleColor) * value;
        });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LeanTween.value(1, 0, timeTransition).setOnUpdate((float value) =>
        {
            if (coloredObject)
                coloredObject.color = idleColor + (pressColor - idleColor) * value;
        });
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
