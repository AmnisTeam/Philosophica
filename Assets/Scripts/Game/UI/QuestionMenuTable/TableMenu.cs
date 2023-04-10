using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableMenu : MonoBehaviour
{
    public TMPro.TMP_Text[] nickname;
    public TMPro.TMP_Text[] answer;
    //public TMPro.TMP_Text[] minutes;
    //public TMPro.TMP_Text[] secundes;
    public TMPro.TMP_Text[] time;
    public Image[] icons;
    public CanvasGroup[] tableRaw;
    public Image winnerIcon;
    public TMPro.TMP_Text winnerNickname;
    public TMPro.TMP_Text drawText;
    public TMPro.TMP_Text header;
    public UnityEngine.Color colorRightAnswer;

    public PlayersManager playersManager;
    public GameplayManager gameplayManager;
    //public IconsContent iconsContent;
    public List<Player> table;
    public Player winner;
    [SerializeField] private PlaySound playSound;
    public Icons iconsSprite;

    public bool isHaveRightAnswer = false;

    public void compileTheTable(List<PlayerAnswerData> playerAnswerData, QuestionManager.Question question)
    {
        table = new List<Player>();
        for (int x = 0; x < playersManager.players.count; x++)
            table.Add(playersManager.players.get(x));

        // Проверка на переполнение при клике после окончании таймера
        for (int i = 0; i < playerAnswerData.Count; i++)
        {
            if (playerAnswerData[i].timeToAnswer.Equals(float.NaN)
                || playerAnswerData[i].timeToAnswer < 0 
                || playerAnswerData[i].timeToAnswer > question.timeToQuestion)
            {
                playerAnswerData[i].timeToAnswer = question.timeToQuestion;
            }
        }

        table.Sort((Player x, Player y) => {
            PlayerAnswerData answerDataX = null;
            PlayerAnswerData answerDataY = null;
            for(int i = 0; i < playerAnswerData.Count; i++)
            {
                if(playerAnswerData[i].id == x.id)
                    answerDataX = playerAnswerData[i];

                if (playerAnswerData[i].id == y.id)
                    answerDataY = playerAnswerData[i];
            }

            if (answerDataX.answerId == question.idRightAnswer && answerDataY.answerId != question.idRightAnswer) return -1;
            else if (answerDataX.answerId != question.idRightAnswer && answerDataY.answerId == question.idRightAnswer) return 1;
            else if (answerDataX.timeToAnswer < answerDataY.timeToAnswer) return -1;
            else return 1;
        });

        for (int x = 0; x < playersManager.players.count; x++)
        {
            PlayerAnswerData answerData = null;
            for(int y = 0; y < playerAnswerData.Count; y++)
                if(playerAnswerData[y].id == table[x].id)
                {
                    answerData = playerAnswerData[y];
                    break;
                }

            nickname[x].text = table[x].nickname;
            if (answerData.answerId >= 0)
            {
                answer[x].text = question.answer[answerData.answerId];
                time[x].text = GetTimeStr(answerData.timeToAnswer);
            }
            else
            {
                answer[x].text = "-";
                time[x].text = "-";
            }

            if (answerData.answerId == question.idRightAnswer)
                answer[x].color = colorRightAnswer;
            else
                answer[x].color = Color.white;

            icons[x].sprite = iconsSprite.GetIconsSpriteByID(table[x].iconId);
            icons[x].color = table[x].color;
        }

        for (int x = 0; x < playersManager.MAX_COUNT_PLAYERS; x++)
            if (x < playersManager.players.count)
                tableRaw[x].alpha = 1;
            else
                tableRaw[x].alpha = 0;

        isHaveRightAnswer = false; // Проверка есть ли хотя бы 1 правильный ответ
        foreach (PlayerAnswerData a in playerAnswerData)
        {
            if (a.answerId == question.idRightAnswer)
            {
                isHaveRightAnswer = true; break;
            }
        }

        if (isHaveRightAnswer)
        {
            header.text = "Победил";
            winner = table[0];
            winnerNickname.SetText(winner.nickname);
            drawText.text = "";

            winnerIcon.sprite = iconsSprite.GetIconsSpriteByID(table[0].iconId);
            winnerIcon.color = table[0].color;

            

            if (winner.id == PhotonNetwork.LocalPlayer.ActorNumber - 1) //todo сломается если игрок кикнут в лобби
                playSound.SoundPlay("winner");
            else
                playSound.SoundPlay("loser");
        }
        else
        {
            header.text = "Ничья";
            winner = null;
            winnerNickname.SetText("");
            drawText.text = "Никто не дал верный ответ";

            winnerIcon.sprite = iconsSprite.GetIconsSpriteByID(table[0].iconId);
            winnerIcon.color = new UnityEngine.Color(0, 0, 0, 0); //аватарка типо есть, но она становится прозрачной

            playSound.SoundPlay("loser");
        }
    }

    public void resetTable()
    {
        for (int x = 0; x < playersManager.players.count; x++)
        {
            if (table.Count > 0)
            {
                PlayerAnswerData answerData = playersManager.playerAnswerData.find(table[x].id);
                answerData.answerId = -1;
                answer[x].text = "-";
                time[x].text = "-";
            }
        }
    }

    public string GetTimeStr(double seconds)
    {
        int minutes = (int)seconds / 60;
        int restOfSeconds = (int)seconds % 60;

        string minutesNonsignificantZero = "";
        string secondsNonsignificantZero = "";

        if (Math.Abs(minutes) < 10)
            minutesNonsignificantZero = "0";
        if (Math.Abs(restOfSeconds) < 10)
            secondsNonsignificantZero = "0";

        return $"{minutesNonsignificantZero + minutes}:{secondsNonsignificantZero + restOfSeconds}";
    }
}
