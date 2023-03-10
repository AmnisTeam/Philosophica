using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Photon.Pun;

public class AskQuestionInBattle : MonoBehaviourPunCallbacks
{
    public Color answerColor;
    public Color correctAnswerColor;

    public float buttonsChangeTime = 0.3f;

    public IconsContentHolder iconsContent;
    public Image opponent1Avatar;
    public TextMeshProUGUI opponent1Name;
    public Slider opponent1HealthBar;
    public TextMeshProUGUI opponent1HealthPointsText;

    public Image opponent2Avatar;
    public TextMeshProUGUI opponent2Name;
    public Slider opponent2HealthBar;
    public TextMeshProUGUI opponent2HealthPointsText;

    public TextMeshProUGUI timerText;

    public TextMeshProUGUI questionText;

    public QuestionManager.Question question;

    public TextMeshProUGUI[] answers;
    public Image[] answersBackgrounds;
    public Image[] answersBorders;

    public PlayerAnswerData[] opponentsAnswersData;
    public Opponent[] opponents;

    public double timer = 0;

    public PhotonView pv;

    public void Init(Opponent opponent1, Opponent opponent2, QuestionManager.Question question)
    {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
        pv = GetComponent<PhotonView>();

        opponentsAnswersData = new PlayerAnswerData[2];
        for (int i = 0; i < opponentsAnswersData.Length; i++)
        {
            opponentsAnswersData[i] = new PlayerAnswerData();
            opponentsAnswersData[i].answerId = -1;
            opponentsAnswersData[i].timeToAnswer = 0;
        }

        opponents = new Opponent[2];
        opponents[0] = opponent1;
        opponents[1] = opponent2;

        opponents[0].playerAnswerData.Add(new PlayerAnswerData());
        opponents[1].playerAnswerData.Add(new PlayerAnswerData());

        opponent1Name.text = opponent1.player.nickname;
        opponent1Name.color = opponent1.player.color;

        opponent1Avatar.sprite = iconsContent.lobbyIcons[opponent1.player.iconId];
        opponent1Avatar.color = opponent1.player.color;

        opponent1HealthBar.value = (float)(opponent1.health / opponent1.maxHealh);
        //opponent1HealthPointsText.text = "<color=#FF4F4F>" + opponent1.health + "</color> / " + opponent1.maxHealh;
        opponent1HealthPointsText.text = GetHealthStr(opponent1.health, opponent1.maxHealh, GlobalVariables.healthColor);


        opponent2Name.text = opponent2.player.nickname;
        opponent2Name.color = opponent2.player.color;

        opponent2Avatar.sprite = iconsContent.lobbyIcons[opponent2.player.iconId];
        opponent2Avatar.color = opponent2.player.color;

        opponent2HealthBar.value = (float)(opponent2.health / opponent2.maxHealh);
        //opponent2HealthPointsText.text = "<color=#FF4F4F>" + opponent2.health + "</color> / " + opponent2.maxHealh;
        opponent2HealthPointsText.text = GetHealthStr(opponent2.health, opponent2.maxHealh, GlobalVariables.healthColor);

        this.questionText.text = question.question;
        this.question = question;

        for (int a = 0; a < answers.Length && a < question.answer.Length; a++)
            answers[a].text = question.answer[a];
        timer = 0;
        ResetAnswersWithoutAnimation();
    }

    public void SelectAnswer(int buttonId)
    {
        Debug.Log("SelectAnswer");

        int clientNumber = -1;
        bool isClient = false;
        for (int i = 0; i < opponents.Length; i++)
            if (opponents[i].player.isLocalClient)
            {
                clientNumber = i;
                isClient = true;
            }
                

        if (isClient)
        {
            for (int i = 0; i < answersBorders.Length; i++)
                if (answersBorders[i].GetComponent<CanvasGroup>().alpha > 0)
                    answersBorders[i].GetComponent<CanvasGroup>().LeanAlpha(0, buttonsChangeTime).setEaseOutSine();
                    

            answersBorders[buttonId].GetComponent<CanvasGroup>().LeanAlpha(1, buttonsChangeTime).setEaseOutSine();
            //MarkAnswerAsCorrect(buttonId);

            Debug.Log("buttonId: " + buttonId + ", " + "timeToAnsnwer: " + timer);

            int lastElementId = opponents[clientNumber].playerAnswerData.Count - 1;
            opponents[clientNumber].playerAnswerData[lastElementId].answerId = buttonId;
            opponents[clientNumber].playerAnswerData[lastElementId].timeToAnswer = (float)timer;

            opponentsAnswersData[clientNumber].answerId = buttonId;
            opponentsAnswersData[clientNumber].timeToAnswer = (float)timer;
        }
    }

    public void ShowCorrectAnswer()
    {
        //for (int i = 0; i < question.answer.Length; i++)
        //    if (i != question.idRightAnswer)
        //        MarkAnswerAsDefault(i);
        int idx = 0;

        foreach (var opponent in opponents) {
            if (opponent.player.isLocalClient) {
                pv.RPC("RPC_RevealAnswerOfOpponent", RpcTarget.Others, idx, opponent.playerAnswerData[opponents[idx].playerAnswerData.Count-1].answerId,
                                                                        opponent.playerAnswerData[opponents[idx].playerAnswerData.Count-1].timeToAnswer);
            }

            idx++;
        }

        MarkAnswerAsCorrect(question.idRightAnswer);
    }

    [PunRPC]
    public void RPC_RevealAnswerOfOpponent(int playerIdx, int answerId, float answerTime) {
        opponentsAnswersData[playerIdx].answerId = answerId;
        opponentsAnswersData[playerIdx].timeToAnswer = answerTime;
        opponents[playerIdx].playerAnswerData[opponents[playerIdx].playerAnswerData.Count-1].answerId = answerId;
        opponents[playerIdx].playerAnswerData[opponents[playerIdx].playerAnswerData.Count-1].timeToAnswer = answerTime;
    }

    public void HideCorrectAnswer()
    {
        for (int i = 0; i < question.answer.Length; i++)
            MarkAnswerAsDefault(i);
    }

    public void ResetAnswersWithoutAnimation()
    {
        for (int i = 0; i < question.answer.Length; i++)
        {
            answersBorders[i].GetComponent<CanvasGroup>().alpha = 0;
            answersBackgrounds[i].color = answerColor;
        }
    }

    public void MarkAnswerAsCorrect(int answerId)
    {
        LeanTween.color(answersBackgrounds[answerId].rectTransform, correctAnswerColor, buttonsChangeTime);
    }

    public void MarkAnswerAsDefault(int answerId)
    {
        LeanTween.color(answersBackgrounds[answerId].rectTransform, answerColor, buttonsChangeTime);
    }

    public static string GetHealthStr(double health, double maxHealth, Color healthColor)
    {
        string hex = healthColor.ToHexString();
        return "<color=#" + hex + ">" + (int)health + "</color> / " + maxHealth;
    }
}
