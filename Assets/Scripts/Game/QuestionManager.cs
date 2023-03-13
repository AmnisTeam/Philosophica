using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static QuestionManager;

public class QuestionManager : MonoBehaviour
{
    public class Question
    {
        public string question;
        public string[] answer = new string[4];
        public int idRightAnswer;
        public float timeToQuestion;
    }

    public QuestionLoader questionLoader;
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
    private bool haveQuestion = false;

    public float time = 0.8f;
    private float timer = 0;

    public Animator questionMenuAnimator;

    public void setQuestion(Question question)
    {
        ShowTable(false);
        OpenQuestionMenu();
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
        haveQuestion = true;
        endQuestion = false;
        selectionQuestions.setVisibleButtons();

    }

    public void setQuestion(int id)
    {
        setQuestion(questionLoader.questions[id % questionLoader.questions.Count()]);
    }

    public void ShowTable(bool toShowTable)
    {
        questionMenuAnimator.SetBool("ShowTable", toShowTable);
        showTable = toShowTable;
    }

    public void OpenQuestionMenu()
    {
        questionMenuAnimator.SetTrigger("Open");
    }

    public void CloseQuestionMenu()
    {
        questionMenuAnimator.SetTrigger("Close");
    }

    public void loadQuestions()
    {
        //Загрузка вопросов с диска
        //questions = new Question[2];
        //questions[0] = new Question();
        //questions[0].question = "Именем какого философа история философии делится на до и после?";
        //questions[0].answer = new string[4];
        //questions[0].answer[0] = "Сократ";
        //questions[0].answer[1] = "Герадот";
        //questions[0].answer[2] = "Архимед";
        //questions[0].answer[3] = "Кант";
        //questions[0].timeToQuestion = 5;
        //questions[0].idRightAnswer = 3;

        //questions[1] = new Question();
        //questions[1].question = "Как называются белые летающие пушистые штуки высоко в верху на улице?";
        //questions[1].answer = new string[4];
        //questions[1].answer[0] = "Коты";
        //questions[1].answer[1] = "Облока";
        //questions[1].answer[2] = "Мед";
        //questions[1].answer[3] = "Зелёнка";
        //questions[1].timeToQuestion = 10;
        //questions[1].idRightAnswer = 1;
        questionLoader.LoadQuestions();
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

    private void Awake()
    {
        questionLoader = new QuestionLoader();
        loadQuestions();
    }

    void Start()
    {   
    }

    void Update()
    {
        if (haveQuestion)
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

            if (timerToShowTable == 0 && showTable)
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
}
