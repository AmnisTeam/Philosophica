using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SelectionQuestions : MonoBehaviourPunCallbacks
{
    //public Image[] selection0;
    //public Image[] selection1;
    //public Image[] selection2;
    //public Image[] selection3;

    public TMPro.TMP_Text[] buttons;
    public Image[] borders;
    public Image[] backgrounds;

    public QuestionManager questionManager;

    public float speed = 0.01f;
    public float[] opacity = new float[4];
    public int activeSelection = -1;
    public float shadowMinColor = 0.2f;
    public float shadowSpeed = 0.6f;
    private float shadowColor = 1;
    private float shadowChoosButtonColor = 1;
    private float t;

    private bool an = false;

    public void toActiveSelection(int idx)
    {
        if (!questionManager.haveAnswer)
        {
            activeSelection = idx;
            an = false;
        }
/*        else
        {
            activeSelection = -1;
            an = true; // todo �� ����� ��� �� ����  �� ����� false, �� ��� �������� true???
        }*/
    }

    public void setVisibleButtons()
    {
        //shadowColor = 1;
        //for (int x = 0; x < 4; x++)
        //    buttons[x].color = new UnityEngine.Color(255, 255, 255, 1);
        for (int x = 0; x < 4; x++)
        {
            borders[x].GetComponent<CanvasGroup>().alpha = 0;
            backgrounds[x].color = new UnityEngine.Color(0, 0, 0, 1);
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
        else if (activeSelection != -1)
        {
            opacity[activeSelection] += speed * Time.deltaTime;
            if (opacity[activeSelection] > 1)
                opacity[activeSelection] = 1;
        }
        //Debug.Log(activeSelection);
        //for (int x = 0; x < 6; x++)
        //{
        //    if(!(questionManager.endQuestion && 0 == questionManager.rightAnswer))
        //        selection0[x].color = new UnityEngine.Color(255, 255, 255, opacity[0]);
        //    if (!(questionManager.endQuestion && 1 == questionManager.rightAnswer))
        //        selection1[x].color = new UnityEngine.Color(255, 255, 255, opacity[1]);
        //    if (!(questionManager.endQuestion && 2 == questionManager.rightAnswer))
        //        selection2[x].color = new UnityEngine.Color(255, 255, 255, opacity[2]);
        //    if (!(questionManager.endQuestion && 3 == questionManager.rightAnswer))
        //        selection3[x].color = new UnityEngine.Color(255, 255, 255, opacity[3]);
        //}

        if(questionManager.haveAnswer)
        {
            shadowColor -= shadowSpeed * Time.deltaTime;
            if (shadowColor < shadowMinColor)
                shadowColor = shadowMinColor;

            shadowChoosButtonColor -= shadowSpeed * Time.deltaTime;
            if (shadowChoosButtonColor < 0)
                shadowChoosButtonColor = 0;

            for (int x = 0; x < 4; x++)
                //if (x != activeSelection && !(questionManager.endQuestion && x == questionManager.rightAnswer))
                if (x == activeSelection && !(questionManager.endQuestion && x == questionManager.rightAnswer))
                {
                    //buttons[x].color = new UnityEngine.Color(255, 255, 255, shadowColor);
                    borders[x].GetComponent<CanvasGroup>().alpha = 1;
                }
        }

        if(questionManager.endQuestion)
        {
            /*
            buttons[questionManager.rightAnswer].color = new UnityEngine.Color(255, 255, 255, buttons[questionManager.rightAnswer].color.a + shadowSpeed * Time.deltaTime > 1 ? 1 : buttons[questionManager.rightAnswer].color.a + shadowSpeed * Time.deltaTime);
            if (questionManager.rightAnswer == 0)
                for (int x = 0; x < 6; x++)
                    selection0[x].color = new UnityEngine.Color(0, 255, 0, selection0[x].color.a + speed * Time.deltaTime > 1 ? 1 : selection0[x].color.a + speed * Time.deltaTime);
            if (questionManager.rightAnswer == 1)
                for (int x = 0; x < 6; x++)
                    selection1[x].color = new UnityEngine.Color(0, 255, 0, selection1[x].color.a + speed * Time.deltaTime > 1 ? 1 : selection1[x].color.a + speed * Time.deltaTime);
            if (questionManager.rightAnswer == 2)
                for (int x = 0; x < 6; x++)
                    selection2[x].color = new UnityEngine.Color(0, 255, 0, selection2[x].color.a + speed * Time.deltaTime > 1 ? 1 : selection2[x].color.a + speed * Time.deltaTime);
            if (questionManager.rightAnswer == 3)
                for (int x = 0; x < 6; x++)
                    selection3[x].color = new UnityEngine.Color(0, 255, 0, selection3[x].color.a + speed * Time.deltaTime > 1 ? 1 : selection3[x].color.a + speed * Time.deltaTime);
            */

            backgrounds[questionManager.rightAnswer].color = new UnityEngine.Color(0, 0.5f, 0, 1);
        }
    }
}
