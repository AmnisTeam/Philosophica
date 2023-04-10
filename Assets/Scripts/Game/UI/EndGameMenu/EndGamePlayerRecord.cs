using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePlayerRecord : MonoBehaviour
{
    public TMPro.TMP_Text number;
    public Image icon;
    public TMPro.TMP_Text nickname;
    public TMPro.TMP_Text points;

    private IconsContentHolder iconsContent;

    public void SetRecord(int number, Sprite iconSprite, Color iconColor, string nickname, int points)
    {
        this.number.text = number.ToString();
        icon.sprite = iconSprite;
        icon.color = iconColor;
        this.nickname.text = nickname;
        this.points.text = $"{points} очков";
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
    }

    public void SetRecord(int number, Player person)
    {
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
        SetRecord(number, iconsContent.lobbyIcons[person.iconId], person.color, person.nickname, (int)person.scores);
    }

    private void Awake()
    {
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
    }

}
