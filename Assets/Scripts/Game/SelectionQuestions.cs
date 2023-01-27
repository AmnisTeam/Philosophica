using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionQuestions : MonoBehaviour
{
    public Image[] selection0;
    public Image[] selection1;
    public Image[] selection2;
    public Image[] selection3;

    public TMPro.TMP_Text[] buttons;
    public CanvasGroup cavasGroup;

    public QuestionManager questionManager;

    public float speed = 0.01f;
    public float[] opacity;
    public int activeSelection = 0;
    public float shadowMinColor = 0.2f;
    public float shadowSpeed = 0.6f;
    private float shadowColor = 1;
    private float shadowChoosButtonColor = 1;

    public bool an = false;

    public void toActiveSelection0()
    {
        if (!questionManager.haveAnswer)
        {
            activeSelection = 0;
            an = false;
        }
    }

    public void toActiveSelection1()
    {
        if (!questionManager.haveAnswer)
        {
            activeSelection = 1;
            an = false;
        }
    }

    public void toActiveSelection2()
    {
        if (!questionManager.haveAnswer)
        {
            activeSelection = 2;
            an = false;
        }
    }

    public void toActiveSelection3()
    {
        if (!questionManager.haveAnswer)
        {
            activeSelection = 3;
            an = false;
        }
    }

    void Start()
    {
        opacity = new float[4];
        opacity[0] = 1;
        opacity[1] = 0;
        opacity[2] = 0;
        opacity[3] = 0;
    }

    void Update()
    {
        if (!an)
        {
            for (int x = 0; x < 4; x++)
            {
                opacity[x] -= speed * Time.deltaTime;

                if (opacity[x] < 0) opacity[x] = 0;
            }

            for (int x = 0; x < 4; x++)
            {
                an = opacity[x] == 0;
                if (!an) break;
            }
        }
        else
        {
            opacity[activeSelection] += speed * Time.deltaTime;
            if (opacity[activeSelection] > 1)
                opacity[activeSelection] = 1;
        }

        for (int x = 0; x < 6; x++)
        {
            selection0[x].color = new Color(255, 255, 255, opacity[0]);
            selection1[x].color = new Color(255, 255, 255, opacity[1]);
            selection2[x].color = new Color(255, 255, 255, opacity[2]);
            selection3[x].color = new Color(255, 255, 255, opacity[3]);
        }

        if(questionManager.haveAnswer)
        {
            shadowColor -= shadowSpeed * Time.deltaTime;
            if (shadowColor < shadowMinColor)
                shadowColor = shadowMinColor;

            shadowChoosButtonColor -= shadowSpeed * Time.deltaTime;
            if (shadowChoosButtonColor < 0)
                shadowChoosButtonColor = 0;

            for (int x = 0; x < 4; x++)
                if (x != activeSelection)
                    buttons[x].color = new Color(255, 255, 255, shadowColor);

            cavasGroup.alpha = shadowChoosButtonColor;
        }
    }
}
