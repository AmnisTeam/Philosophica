using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public class Question
    {
        public string question;
        public string[] answer;
        public int idRightAnswer;
        public float timeToQuestion;
    }

    private Question[] questions;
    public GameObject questionsMenu;
    public SelectionQuestions selectionQuestions;
    public PlayersManager playersManager;
    public ConfigTemp config;
    public TMPro.TMP_Text questionText;
    public TMPro.TMP_Text[] answerText;
    public TMPro.TMP_Text minutes;
    public TMPro.TMP_Text secundes;
    public TableCompiler tableCompiler;
    public Question activeQuestion;

    public float timeToQuestion;
    public float timerToQuestion;
    public int rightAnswer = 0;
    public int selectedAnswer = 0;
    public bool haveAnswer = false;
    public bool endQuestion = false;

    public float timeToShowTable = 3;
    public float timerToShowTable = 0;
    public bool showTable = false;

    public float time = 0.8f;
    private float timer = 0;

    public void setQuestion(Question question)
    {
        timeToQuestion = question.timeToQuestion;
        timerToQuestion = question.timeToQuestion;
        questionText.text = question.question;
        for (int x = 0; x < 4; x++)
            answerText[x].text = question.answer[x];
        rightAnswer = question.idRightAnswer;
        selectedAnswer = 0;
        playersManager.playerAnswerData.find(config.me.id).answerId = 0;
        playersManager.playerAnswerData.find(config.me.id).timeToAnswer = timeToQuestion;
        haveAnswer = false;
        activeQuestion = question;
    }

    public void loadQuestions()
    {
        //Загрузка вопросов с диска
        questions = new Question[1];
        questions[0] = new Question();
        questions[0].question = "Именем какого философа история философии делится на до и после?";
        questions[0].answer = new string[4];
        questions[0].answer[0] = "Сократ";
        questions[0].answer[1] = "Герадот";
        questions[0].answer[2] = "Архимед";
        questions[0].answer[3] = "Кант";
        questions[0].timeToQuestion = 5;
        questions[0].idRightAnswer = 3;
    }

    public void checkSelectAnswer()
    {
        if (!haveAnswer)
        {
            selectedAnswer = selectionQuestions.activeSelection;
            haveAnswer = true;
            playersManager.playerAnswerData.find(config.me.id).answerId = selectedAnswer;
            playersManager.playerAnswerData.find(config.me.id).timeToAnswer = timeToQuestion - timerToQuestion;
        }
    }

    void Start()
    {
        loadQuestions();
        setQuestion(questions[0]);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        timerToQuestion -= Time.deltaTime;
        if (timerToQuestion < 0) timerToQuestion = 0;

        if (showTable)
        {
            timerToShowTable -= Time.deltaTime;
            if (timerToShowTable < 0) timerToShowTable = 0;
        }
        else if (timerToQuestion == 0)
        {
            endQuestion = true;
            haveAnswer = true;
            selectedAnswer = selectionQuestions.activeSelection;
            playersManager.playerAnswerData.find(config.me.id).answerId = selectionQuestions.activeSelection;
            timerToShowTable = timeToShowTable;
            showTable = true;
        }

        if(timerToShowTable == 0 && showTable)
        {
            questionsMenu.GetComponent<Animator>().SetBool("ShowTable", true);
            tableCompiler.compileTheTable();
        }

        minutes.text = "";
        secundes.text = "";


        if ((int)(timerToQuestion / 60) / 10 == 0)
            minutes.text += '0';
        minutes.text += ((int)(timerToQuestion / 60)).ToString();

        if ((int)(timerToQuestion % 60) / 10 == 0)
            secundes.text += '0';
        secundes.text += ((int)(timerToQuestion % 60)).ToString();
    }
}
