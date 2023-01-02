using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Question
{
    public string question;
    public string[] answer;
}

public class QuestionManager : MonoBehaviour
{
    private Question[] questions;
    public GameObject questionsMenu;
    public SelectionQuestions selectionQuestions;
    public TMPro.TMP_Text questionText;
    public TMPro.TMP_Text[] answerText;
    public int selectedAnswer = -1;
    public bool haveAnswer = false;

    public float time = 0.8f;
    private float timer = 0;
    private bool c = false;

    public bool openQuestionMenub;
    private bool t = false;

    public void loadQuestions()
    {
        //Загрузка вопросов с диска
    }

    public void checkSelectAnswer()
    {
        if (!haveAnswer)
        {
            selectedAnswer = selectionQuestions.activeSelection;
            haveAnswer = true;
        }
    }

    public void openQuestionsMenu()
    {
        questionsMenu.SetActive(true);
        questionsMenu.GetComponent<Animator>().SetTrigger("Open");
    }

    public void closeQuestionsMenu()
    {
        questionsMenu.GetComponent<Animator>().SetTrigger("Close");
        timer = time;
        c = true;
    }

    void Start()
    {
        loadQuestions();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        if (c && timer == 0)
        {
            c = false;
            questionsMenu.SetActive(false);
        }

        if (openQuestionMenub)
        {
            if (!t)
            {
                openQuestionsMenu();
                t = true;
            }
        }
        else
        {
            if (t)
            {
                closeQuestionsMenu();
                t = false;
            }
        }
    }
}
