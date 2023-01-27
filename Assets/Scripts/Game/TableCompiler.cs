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

    public PlayersManager playersManager;
    public QuestionManager questionManager;
    private List<Player> players;

    public void compileTheTable()
    {
        players = new List<Player>();
        var e = playersManager.players.GetEnumerator();
        do
        {
            if (e.Current != null)
            {
                Player player = e.Current;
                players.Add(player);
            }
        } while (e.MoveNext());

        players.Sort((Player x, Player y) => {
            if (x.answerId == questionManager.rightAnswer && y.answerId != questionManager.rightAnswer) return 1;
            else if (x.answerId != questionManager.rightAnswer && y.answerId == questionManager.rightAnswer) return -1;
            else if (x.timeToAnswer < y.timeToAnswer) return 1;
            else return -1;
        });

        for(int x = 0; x < players.Count; x++)
        {
            nickname[x].text = players[x].nickname;
            answer[x].text = questionManager.answerText[players[x].answerId].text;

            minutes[x].text = "";
            secundes[x].text = "";

            if ((int)(players[x].timeToAnswer / 60) / 10 == 0)
                minutes[x].text += '0';
            minutes[x].text += ((int)(players[x].timeToAnswer / 60)).ToString();

            if ((int)(players[x].timeToAnswer % 60) / 10 == 0)
                secundes[x].text += '0';
            secundes[x].text += ((int)(players[x].timeToAnswer % 60)).ToString();
        }

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
