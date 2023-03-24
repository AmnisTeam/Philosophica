using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableCompiler : MonoBehaviour
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
    public QuestionManager questionManager;
    public IconsContentHolder iconsContent;
    public List<Player> table;
    public Player winner;

    public bool isHaveRightAnswer = false;

    public void Awake() {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
    }

    public void compileTheTable()
    {
        table = new List<Player>();
        for (int x = 0; x < questionManager.playersManager.players.count; x++)
            table.Add(questionManager.playersManager.players.get(x));

        table.Sort((Player x, Player y) => {
            if (playersManager.playerAnswerData.find(x.id).answerId == questionManager.rightAnswer && playersManager.playerAnswerData.find(y.id).answerId != questionManager.rightAnswer) return -1;
            else if (playersManager.playerAnswerData.find(x.id).answerId != questionManager.rightAnswer && playersManager.playerAnswerData.find(y.id).answerId == questionManager.rightAnswer) return 1;
            else if (playersManager.playerAnswerData.find(x.id).timeToAnswer < playersManager.playerAnswerData.find(y.id).timeToAnswer) return -1;
            else return 1;
        });

        for(int x = 0; x < questionManager.playersManager.players.count; x++)
        {
            PlayerAnswerData answerData = playersManager.playerAnswerData.find(table[x].id);
            
            // REMOVE КОСТЫЛЬ
            if (answerData.answerId == -1)
                answerData.answerId = 0;


            nickname[x].text = table[x].nickname;
            answer[x].text = questionManager.activeQuestion.answer[answerData.answerId];
            if (answerData.answerId == questionManager.rightAnswer)
                answer[x].color = colorRightAnswer;
            else
                answer[x].color = Color.white;
            icons[x].sprite = iconsContent.lobbyIcons[table[x].iconId];
            icons[x].color = table[x].color;

            //minutes[x].text = "";
            //secundes[x].text = "";
            //time[x].text = "";

            //if ((int)(answerData.timeToAnswer / 60) / 10 == 0)
            //    minutes[x].text += '0';
            //minutes[x].text += ((int)(answerData.timeToAnswer / 60)).ToString();

            //if ((int)(answerData.timeToAnswer % 60) / 10 == 0)
            //    secundes[x].text += '0';
            //secundes[x].text += ((int)(answerData.timeToAnswer % 60)).ToString();
            time[x].text = GetTimeStr(answerData.timeToAnswer);
        }

        for (int x = 0; x < questionManager.playersManager.MAX_COUNT_PLAYERS; x++)
            if(x < questionManager.playersManager.players.count)
                tableRaw[x].alpha = 1;
            else
                tableRaw[x].alpha = 0;

        isHaveRightAnswer = false; // Проверка есть ли хотя бы 1 правильный ответ
        foreach (Player p in table)
        {
            if (playersManager.playerAnswerData.find(p.id).answerId == questionManager.rightAnswer)
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

            winnerIcon.sprite = iconsContent.lobbyIcons[table[0].iconId];
            winnerIcon.color = table[0].color;
        }
        else
        {
            header.text = "Ничья";
            winner = null;
            winnerNickname.SetText("");
            drawText.text = "Никто не дал верный ответ";

            winnerIcon.sprite = iconsContent.lobbyIcons[table[0].iconId];
            winnerIcon.color = new UnityEngine.Color(0, 0, 0, 0); //аватарка типо есть, но она становится прозрачной
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
