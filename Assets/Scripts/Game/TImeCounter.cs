using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TImeCounter : MonoBehaviour
{
    public TMPro.TMP_Text minutesText;
    public TMPro.TMP_Text secundesText;
    public float time;

    void Start()
    {
        
    }

    void Update()
    {
        time += Time.deltaTime;

        minutesText.text = "";
        secundesText.text = "";


        if ((int)(time / 60) / 10 == 0)
            minutesText.text += '0';
        minutesText.text += ((int)(time / 60)).ToString();

        if ((int)(time % 60) / 10 == 0)
            secundesText.text += '0';
        secundesText.text += ((int)(time % 60)).ToString();
    }
}
