using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingSceneTextUpdate : MonoBehaviour
{
    private TMP_Text text;
    private float time = 0f;
    private float timer = 0.15f;
    private int points = 0;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > timer)
        {
            time = 0f;
            text.text += ".";

            if (points >= 3)
            {
                text.text = "Загрузка";
                points = 0;
            }
            else
                points++;
        }
    }
}
