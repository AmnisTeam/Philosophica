using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableCompiler : MonoBehaviour
{
    public TMPro.TMP_Text[] nickname;
    public TMPro.TMP_Text[] answer;
    public TMPro.TMP_Text[] minutes;
    public TMPro.TMP_Text[] secundes;
    public Image[] icons;
    public CanvasGroup[] tableRaw;
    public Image winnerIcon;
    public TMPro.TMP_Text winnerNickname;
    public UnityEngine.Color colorRightAnswer;

    public PlayersManager playersManager;
    public QuestionManager questionManager;
    public IconsContent iconsContent;
    public List<Player> table;
    public Player winner;

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
            nickname[x].text = table[x].nickname;
            answer[x].text = questionManager.activeQuestion.answer[answerData.answerId];
            if (answerData.answerId == questionManager.rightAnswer)
                answer[x].color = colorRightAnswer;
            icons[x].sprite = iconsContent.icons[table[x].iconId].sprite;
            icons[x].color = table[x].color;

            minutes[x].text = "";
            secundes[x].text = "";

            if ((int)(answerData.timeToAnswer / 60) / 10 == 0)
                minutes[x].text += '0';
            minutes[x].text += ((int)(answerData.timeToAnswer / 60)).ToString();

            if ((int)(answerData.timeToAnswer % 60) / 10 == 0)
                secundes[x].text += '0';
            secundes[x].text += ((int)(answerData.timeToAnswer % 60)).ToString();
        }

        for (int x = 0; x < questionManager.playersManager.MAX_COUNT_PLAYERS; x++)
            if(x < questionManager.playersManager.players.count)
                tableRaw[x].alpha = 1;
            else
                tableRaw[x].alpha = 0;

        winner = table[0];
        winnerNickname.SetText(winner.nickname);
        winnerIcon.sprite = iconsContent.icons[table[0].iconId].sprite;
        winnerIcon.color = table[0].color;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
