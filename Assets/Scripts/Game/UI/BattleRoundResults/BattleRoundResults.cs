using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoundResults : MonoBehaviour
{
    public Color correctAnswerColor;

    public IconsContent iconsContent;

    public Image opponent1Avatar;
    public TextMeshProUGUI opponent1Name;
    public Slider opponent1HealthBar;
    public TextMeshProUGUI opponent1HealthPointsText;

    public Image opponent2Avatar;
    public TextMeshProUGUI opponent2Name;
    public Slider opponent2HealthBar;
    public TextMeshProUGUI opponent2HealthPointsText;

    public TextMeshProUGUI notification;

    public GameObject playerRowPrefab;
    public GameObject tableContent;

    private Battle battle;
    //private Opponent[] opponents;
    public void Init(Battle battle)
    {
        this.battle = battle;
        InitOpponents();
        InitNotification();
        for (int i = 0; i < battle.opponents.Count(); i++)
            AddPlayerRow(battle.opponents[i]);
    }

    private void InitOpponents()
    {
        int lastId1 = battle.opponents[0].playerAnswerData.Count - 1;
        int lastId2 = battle.opponents[1].playerAnswerData.Count - 1;

        if (battle.opponents[0].playerAnswerData[lastId1].timeToAnswer == -1 ||
            battle.opponents[0].playerAnswerData[lastId1].answerId == -1)
        {
            battle.winnerId = 1; // If the answer or the time is incorrect
        }      
        else if (battle.opponents[1].playerAnswerData[lastId2].timeToAnswer == -1 ||
            battle.opponents[1].playerAnswerData[lastId2].answerId == -1)
        {
            battle.winnerId = 0; // If the answer or the time is incorrect
        }        
        else if (battle.opponents[0].playerAnswerData[lastId1].answerId ==
            battle.questions[battle.currentQuestion].idRightAnswer &&
            battle.opponents[1].playerAnswerData[lastId2].answerId !=
            battle.questions[battle.currentQuestion].idRightAnswer)
        {
            battle.winnerId = 0; // If the first opponent answered correctly whereas the second not
        }        
        else if (battle.opponents[1].playerAnswerData[lastId2].answerId ==
            battle.questions[battle.currentQuestion].idRightAnswer &&
            battle.opponents[0].playerAnswerData[lastId1].answerId !=
            battle.questions[battle.currentQuestion].idRightAnswer)
        {
            battle.winnerId = 1; // If the second opponent answered correctly whereas the first not
        }       
        else if (battle.opponents[0].playerAnswerData[lastId1].timeToAnswer >=
                battle.opponents[1].playerAnswerData[lastId2].timeToAnswer)
        {
            battle.winnerId = 0; // If first opponent answerd faster
        }         
        else
        {
            battle.winnerId = 1; // If second opponent answerd faster
        }
         

        //Array.Sort(opponents, (Opponent opponent1, Opponent oppoent2) =>
        //{
        //    int lastId1 = battle.opponents[0].playerAnswerData.Count - 1;
        //    int lastId2 = battle.opponents[1].playerAnswerData.Count - 1;

        //    if (battle.opponents[0].playerAnswerData[lastId1].timeToAnswer == -1 ||
        //        battle.opponents[0].playerAnswerData[lastId1].answerId == -1)
        //        return 1; // If the answer or the time is incorrect

        //    if (battle.opponents[1].playerAnswerData[lastId2].timeToAnswer == -1 ||
        //        battle.opponents[1].playerAnswerData[lastId2].answerId == -1)
        //        return 0; // If the answer or the time is incorrect

        //    if (battle.opponents[0].playerAnswerData[lastId1].answerId ==
        //        battle.questions[battle.currentQuestion].idRightAnswer &&
        //        battle.opponents[1].playerAnswerData[lastId2].answerId !=
        //        battle.questions[battle.currentQuestion].idRightAnswer)
        //        return 0; // If the first opponent answered correctly whereas the second not

        //    if (battle.opponents[1].playerAnswerData[lastId2].answerId ==
        //        battle.questions[battle.currentQuestion].idRightAnswer &&
        //        battle.opponents[0].playerAnswerData[lastId1].answerId !=
        //        battle.questions[battle.currentQuestion].idRightAnswer)
        //        return 1; // If the second opponent answered correctly whereas the first not


        //    if (battle.opponents[0].playerAnswerData[lastId1].timeToAnswer >=
        //            battle.opponents[1].playerAnswerData[lastId2].timeToAnswer)
        //        return 1; // If first opponent answerd faster
        //    else
        //        return 0; // If second opponent answerd faster
        //});

        opponent1Avatar.sprite = iconsContent.icons[battle.opponents[0].player.iconId].sprite;
        opponent1Avatar.color = battle.opponents[0].player.color;

        opponent1Name.text = battle.opponents[0].player.nickname;
        opponent1Name.color = battle.opponents[0].player.color;

        opponent1HealthBar.value = (float)(battle.opponents[0].health / battle.opponents[0].maxHealh);
        opponent1HealthPointsText.text = "<color=#FF4F4F>" + battle.opponents[0].health + "</color> / " + battle.opponents[0].maxHealh;


        opponent2Avatar.sprite = iconsContent.icons[battle.opponents[1].player.iconId].sprite;
        opponent2Avatar.color = battle.opponents[1].player.color;

        opponent2Name.text = battle.opponents[1].player.nickname;
        opponent2Name.color = battle.opponents[1].player.color;

        opponent2HealthBar.value = (float)(battle.opponents[1].health / battle.opponents[1].maxHealh);
        opponent2HealthPointsText.text = "<color=#FF4F4F>" + battle.opponents[0].health + "</color> / " + battle.opponents[0].maxHealh;
    }

    public void InitNotification()
    {
        int correctAnswersCount = 0;
        bool everyoneAnsweredCorrectly = false;
        bool isCorrectAnswer = false;
        for (int i = 0; i < battle.opponents.Length; i++)
        {
            int lastRoundId = battle.opponents[i].playerAnswerData.Count - 1;
            int opponentAnswerId = battle.opponents[i].playerAnswerData[lastRoundId].answerId;
            int correctAnswerId = battle.questions[battle.currentQuestion].idRightAnswer;

            if (opponentAnswerId == correctAnswerId)
            {
                correctAnswersCount++;
                isCorrectAnswer = true;
            }                
        }

        if (correctAnswersCount == battle.opponents.Length)
            everyoneAnsweredCorrectly = true;

        if (!isCorrectAnswer)
            notification.text = "����� �� ������� �� ��� ���������� �����, ������� ����� �� �������� ����.";

        Opponent opponent0 = battle.opponents[battle.winnerId];
        Opponent opponent1 = battle.opponents[1 - battle.winnerId];

        string hex0 = "#" + opponent0.player.color.ToHexString();
        string hex1 = "#" + opponent1.player.color.ToHexString();

        string winningOpponentName = "<color=" + hex0 + ">" + opponent0.player.nickname + "</color>";
        string losingOpponentName = "<color=" + hex1 + ">" + opponent1.player.nickname + "</color>";

        string hex = GlobalVariables.healthColor.ToHexString();
        string damageSeverity = "���� � ������� <color=#" + hex + ">" + (int)GetWinnersDamage() + " ������.</color>";

        if (everyoneAnsweredCorrectly)
        {
            notification.text = "����� " + losingOpponentName + " ������� ���������, ��� ����� " + winningOpponentName +
                ", ������� �� �������� " + damageSeverity;
        }
        else
        {
            notification.text = "����� " + losingOpponentName + " ������� �� ���������, ������� �� �������� " + damageSeverity;
        }
            
    }

    public double GetWinnersDamage()
    {
        int lastRoundId = battle.opponents[0].playerAnswerData.Count - 1;
        double damage = (int)((10 - battle.opponents[0].playerAnswerData[lastRoundId].timeToAnswer) * 10);
        if (damage < 0)
            damage = 10;
        return damage;
    }

    public void InflictDamageOnLoser()
    {
        int loserId = 1 - battle.winnerId;
        battle.opponents[loserId].health -= GetWinnersDamage();
    }

    public void UpdateOpponentsHealthGradudally(float time)
    {      
        LeanTween.value((float)(opponent1HealthBar.value * battle.opponents[0].maxHealh), 
            (float)(battle.opponents[0].health), 
            time).setOnUpdate((float val) => 
            {
                opponent1HealthBar.value = val / (float)battle.opponents[0].maxHealh;
                double currentHealth = val;
                double maxHealth = battle.opponents[0].maxHealh;
                opponent1HealthPointsText.text = AskQuestionInBattle.GetHealthStr(currentHealth, 
                    maxHealth, GlobalVariables.healthColor);
            }).setEaseOutSine();

        LeanTween.value((float)(opponent2HealthBar.value * battle.opponents[1].maxHealh),
            (float)(battle.opponents[1].health),
            time).setOnUpdate((float val) =>
            {
                opponent2HealthBar.value = val / (float)battle.opponents[1].maxHealh;
                double currentHealth = val;
                double maxHealth = battle.opponents[1].maxHealh;
                opponent2HealthPointsText.text = AskQuestionInBattle.GetHealthStr(currentHealth,
                    maxHealth, GlobalVariables.healthColor);
            }).setEaseOutSine();
    }



    public void AddPlayerRow(Opponent opponent, string unfilledFieldPlaceholder = "-")
    {
        PlayerRow playerRow = Instantiate(playerRowPrefab, tableContent.transform).GetComponent<PlayerRow>();
        int opponentLastAnswerData = opponent.playerAnswerData.Count - 1;
        int opponentAnswerId = opponent.playerAnswerData[opponentLastAnswerData].answerId;

        string answer = unfilledFieldPlaceholder;
        string time = unfilledFieldPlaceholder;
        if (opponentAnswerId >= 0)
        {
            answer = battle.questions[battle.currentQuestion].answer[opponentAnswerId];
            time = GameplayManager.GetTimeStr(opponent.playerAnswerData[opponentLastAnswerData].timeToAnswer);
        }          

        playerRow.Init(
            iconsContent.icons[opponent.player.iconId].sprite,
            opponent.player.nickname,
            answer,
            time);
        playerRow.avatar.color = opponent.player.color;

        if (opponentAnswerId == battle.questions[battle.currentQuestion].idRightAnswer)
            playerRow.answerText.color = correctAnswerColor;
    }
}
