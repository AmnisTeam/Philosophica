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

    private IconsContent iconsContent;

    public void SetRecord(int number, Sprite iconSprite, Color iconColor, string nickname, int points)
    {
        this.number.text = number.ToString();
        icon.sprite = iconSprite;
        icon.color = iconColor;
        this.nickname.text = nickname;
        this.points.text = points.ToString() + " points";
    }

    public void SetRecord(int number, Player person)
    {
        SetRecord(number, iconsContent.icons[person.iconId].sprite, person.color, person.nickname, (int)person.scores);
    }

    private void Awake()
    {
        iconsContent = GameObject.FindWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContent>();
    }

}
