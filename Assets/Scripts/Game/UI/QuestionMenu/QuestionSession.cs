using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionSession
{
    public QuestionLoader questionLoader;
    //public int currentQuestion = 0;
    public PlayersManager playersManager;
    public List<PlayerAnswerData> playerAnswerData;
    private System.Random randomSequence;

    public QuestionSession(PlayersManager playersManager)
    {      
        questionLoader = new QuestionLoader();
        questionLoader.LoadQuestions();

        this.playersManager = playersManager;

        playerAnswerData = new List<PlayerAnswerData>();
        UpdateCountPlayerAnswerData();
    }

    public void UpdateCountPlayerAnswerData()
    {
        playerAnswerData = new List<PlayerAnswerData>();
        for (int x = 0; x < playersManager.players.count; x++)
        {
            PlayerAnswerData answerData = new PlayerAnswerData(-1, -1);
            answerData.id = playersManager.players.get(x).id;
            playerAnswerData.Add(answerData);
        }
    }

    public void InitQuestionRandom(int questionsSeed)
    {
        randomSequence = new System.Random(questionsSeed);
    }

    /*
     * ������������� �������� ������ ������. 
     * ������������ � RPC ������� ��� ������������� �������� ����� ��������
     */
    public void SetAnswerData(int playerId, int answerId, float timeToAnswer)
    {
        int playerAnswerId = -1;
        for(int x = 0; x < playerAnswerData.Count; x++)
            if(playerId == playerAnswerData[x].id)
            {
                playerAnswerId = x;
                break;
            }

        PlayerAnswerData answerData = playerAnswerData[playerAnswerId];
        answerData.answerId = answerId;
        answerData.timeToAnswer = timeToAnswer;
    }

    public void NextQuestion()
    {
        int randQuestionId = randomSequence.Next() % questionLoader.GetQuestionsSize();
        //currentQuestion = (currentQuestion + 1) % questionLoader.questions.Count;
        //currentQuestion = randQuestionId;
        UpdateCountPlayerAnswerData();
    }

    public QuestionManager.Question GetCurrentQuestion()
    {
        return questionLoader.currentQuestion;
    }
}
