using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class QuestionManager : MonoBehaviourPunCallbacks
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

    public GameObject questionMenu;
    public GameObject content;
    public GameObject tableMenu;

    public PhotonView pv;

    public float timeToQuestion;
    public float timerToQuestion;
    public int rightAnswer = 0;
    public int selectedAnswer = -1;
    public bool haveAnswer = false;
    public bool endQuestion = false;

    public float timeToShowTable = 3;
    public float timerToShowTable = 0;
    public bool showTable = false;
    private bool haveQuestion = false;

    public float time = 0.8f;
    private float timer = 0;

    private bool isReset = false;
    public float menusTransitionTime = 0.2f;
    private bool onceShowTable = false;

    public void setQuestion(Question question)
    {
        onceShowTable = false;
        //ShowTable(false);
        content.SetActive(true);
        content.GetComponent<CanvasGroup>().alpha = 1;

        tableMenu.SetActive(false);
        tableMenu.GetComponent<CanvasGroup>().alpha = 0;
        
        OpenQuestionMenu();

        timeToQuestion = question.timeToQuestion;
        timerToQuestion = question.timeToQuestion;
        questionText.text = question.question;

        for (int x = 0; x < 4; x++) {
            answerText[x].text = question.answer[x];
        }
        
        rightAnswer = question.idRightAnswer;
        selectedAnswer = -1;

        foreach (Player player in playersManager.players.list) {
            //if (player.isLocalClient) {
            playersManager.playerAnswerData.find(player.id).answerId = -1;
            playersManager.playerAnswerData.find(player.id).timeToAnswer = timeToQuestion;
                //break;
            //}
        }
        
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
        if(toShowTable)
        {
            content.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setOnComplete(() => { content.SetActive(false); tableMenu.SetActive(true); });
            tableMenu.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setDelay(menusTransitionTime);
        }
        else
        {
            tableMenu.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setOnComplete(() => { tableMenu.SetActive(false); content.SetActive(true); });
            content.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setDelay(menusTransitionTime);
        }

        showTable = toShowTable;
    }

    public void OpenQuestionMenu()
    {
        questionMenu.SetActive(true);
        questionMenu.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        
    }

    public void CloseQuestionMenu()
    {
        questionMenu.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setOnComplete(() => { if(questionMenu) questionMenu.SetActive(false); });
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

            foreach (Player player in playersManager.players.list) {
                if (player.isLocalClient) {
                    playersManager.playerAnswerData.find(player.id).answerId = selectedAnswer;
                    playersManager.playerAnswerData.find(player.id).timeToAnswer = timeToQuestion - timerToQuestion;
                    Debug.Log($">>> checkSelectAnswer() -- {player} answered {selectedAnswer} within {timeToQuestion - timerToQuestion} seconds");
                    break;
                }
            }
        }
    }

    public void Awake()
    {
        questionLoader = new QuestionLoader();
        loadQuestions();

        pv = GetComponent<PhotonView>();
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
                showTable = true;
                /*selectedAnswer = selectionQuestions.activeSelection;
                timerToShowTable = timeToShowTable;*/

                foreach (Player player in playersManager.players.list) {
                    if (player.isLocalClient) {
                        playersManager.playerAnswerData.find(player.id).answerId = selectionQuestions.activeSelection;
                        pv.RPC("RPC_RevealAnswerOfOpponentStageOne", RpcTarget.Others, player.id,
                                                                                       selectionQuestions.activeSelection,
                                                                                       playersManager.playerAnswerData.find(player.id).timeToAnswer);
                        break;
                    }
                }
            }

            if (timerToShowTable == 0 && showTable && !onceShowTable)
            {
                ShowTable(true);
                tableCompiler.compileTheTable();
                isReset = true;
                onceShowTable = true;
            }
            else if (isReset) //вызов только 1 раз для сброса таблицы
            {
                tableCompiler.resetTable();
                isReset = false;
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
        //Debug.Log(selectionQuestions.activeSelection);
    }

    [PunRPC]
    public void RPC_RevealAnswerOfOpponentStageOne(int playerIdx, int answerId, float answerTime) {
        Debug.Log($"{playerIdx} answered {answerId} within {answerTime} seconds");
        playersManager.playerAnswerData.find(playerIdx).answerId = answerId;
        playersManager.playerAnswerData.find(playerIdx).timeToAnswer = answerTime;
    }
}
