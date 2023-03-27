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

public class AskQuestionInBattle : AskQuestion
{
    public IconsContentHolder iconsContent;
    public Image opponent1Avatar;
    public TextMeshProUGUI opponent1Name;
    public Slider opponent1HealthBar;
    public TextMeshProUGUI opponent1HealthPointsText;

    public Image opponent2Avatar;
    public TextMeshProUGUI opponent2Name;
    public Slider opponent2HealthBar;
    public TextMeshProUGUI opponent2HealthPointsText;

    public PlayerAnswerData[] opponentsAnswersData;
    public Opponent[] opponents;

    public void Init(Opponent opponent1, Opponent opponent2, QuestionManager.Question question)
    {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();

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

        Init(question);
    }

    public override void SelectAnswer(int buttonId)
    {
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
            base.SelectAnswer(buttonId);

            Debug.Log("buttonId: " + buttonId + ", " + "timeToAnsnwer: " + timer);

            int lastElementId = opponents[clientNumber].playerAnswerData.Count - 1;
            opponents[clientNumber].playerAnswerData[lastElementId].answerId = buttonId;
            opponents[clientNumber].playerAnswerData[lastElementId].timeToAnswer = (float)timer;

            opponentsAnswersData[clientNumber].answerId = buttonId;
            opponentsAnswersData[clientNumber].timeToAnswer = (float)timer;
        }
    }

    public override void ShowCorrectAnswer()
    {
        int idx = 0;
        foreach (var opponent in opponents) {
            if (opponent.player.isLocalClient) {
                pv.RPC("RPC_RevealAnswerOfOpponent", RpcTarget.Others, idx, opponent.playerAnswerData[opponents[idx].playerAnswerData.Count-1].answerId,
                                                                        opponent.playerAnswerData[opponents[idx].playerAnswerData.Count-1].timeToAnswer);
            }

            idx++;
        }

        base.ShowCorrectAnswer();
    }

    [PunRPC]
    public void RPC_RevealAnswerOfOpponent(int playerIdx, int answerId, float answerTime) {
        opponentsAnswersData[playerIdx].answerId = answerId;
        opponentsAnswersData[playerIdx].timeToAnswer = answerTime;
        opponents[playerIdx].playerAnswerData[opponents[playerIdx].playerAnswerData.Count-1].answerId = answerId;
        opponents[playerIdx].playerAnswerData[opponents[playerIdx].playerAnswerData.Count-1].timeToAnswer = answerTime;
    }

    public static string GetHealthStr(double health, double maxHealth, Color healthColor) {
        return $"<color=#{healthColor.ToHexString()}>{(int)health}</color> / {maxHealth}";
    }
}
