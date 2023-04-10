using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePrizeWinner : MonoBehaviour
{
    public Image icon;
    public TMPro.TMP_Text nickname;
    public TMPro.TMP_Text points;
    public bool toWritePoints;

    private IconsContentHolder iconsContent;

    public void SetWinner(Sprite iconSprite, Color iconColor, string nickname, int points)
    {
        icon.sprite = iconSprite;
        icon.color = iconColor;
        this.nickname.text = nickname;
        this.nickname.color = iconColor;
        this.points.text = $"{points}{(toWritePoints ? " очков" : "")}";
    }

    public void SetWinner(Player person)
    {
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
        SetWinner(iconsContent.lobbyIcons[person.iconId],
                  person.color,
                  person.nickname,
                  person.scores);
    }

    private void Awake()
    {
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
    }
}
