using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoundResults : MonoBehaviour
{
    public IconsContent iconsContent;

    public Image opponent1Avatar;
    public TextMeshProUGUI opponent1Name;
    public Slider opponent1HealthBar;
    public TextMeshProUGUI opponent1HealthPointsText;

    public Image opponent2Avatar;
    public TextMeshProUGUI opponent2Name;
    public Slider opponent2HealthBar;
    public TextMeshProUGUI opponent2HealthPointsText;

    public GameObject playerRowPrefab;
    public GameObject tableContent;

    Opponent[] opponents;
    public void Init(Opponent opponent1, Opponent opponent2)
    {
        opponents = new Opponent[2];
        opponents[0] = opponent1;
        opponents[1] = opponent2;

        Array.Sort(opponents, (Opponent opponent1, Opponent oppoent2) =>
        {
            int lastId1 = opponent1.playerAnswerData.Count - 1;
            int lastId2 = opponent2.playerAnswerData.Count - 1;
            if (opponent1.playerAnswerData[lastId1].timeToAnswer > 
                    opponent2.playerAnswerData[lastId2].timeToAnswer)
                return 1;
            else
                return 0;
        });

        opponent1Avatar.sprite = iconsContent.icons[opponent1.player.iconId].sprite;
        opponent1Avatar.color = opponent1.player.color;

        opponent1Name.text = opponent1.player.nickname;
        opponent1Name.color = opponent1.player.color;

        opponent1HealthBar.value = (float)(opponent1.health / opponent1.maxHealh);
        opponent1HealthPointsText.text = "<color=#FF4F4F>" + opponent1.health + "</color> / " + opponent1.maxHealh;


        opponent2Avatar.sprite = iconsContent.icons[opponent2.player.iconId].sprite;
        opponent2Avatar.color = opponent2.player.color;

        opponent2Name.text = opponent2.player.nickname;
        opponent2Name.color = opponent2.player.color;

        opponent2HealthBar.value = (float)(opponent2.health / opponent2.maxHealh);
        opponent2HealthPointsText.text = "<color=#FF4F4F>" + opponent1.health + "</color> / " + opponent1.maxHealh;

        for (int i = 0; i < opponents.Count(); i++)
            AddPlayerRow(opponents[i]);
    }

    public void AddPlayerRow(Opponent opponent)
    {
        PlayerRow playerRow = Instantiate(playerRowPrefab, tableContent.transform).GetComponent<PlayerRow>();
        playerRow.Init(
            iconsContent.icons[opponent.player.iconId].sprite,
            opponent.player.nickname,
            "õç",
            GameplayManager.GetTimeStr(opponent.playerAnswerData[opponent.playerAnswerData.Count - 1].timeToAnswer)
            );
    }
}
