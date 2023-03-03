using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultsVictory : MonoBehaviour
{
    public IconsContent iconsContent;
    public Image winnersAvatar;
    public TextMeshProUGUI winnersNameText;

    public TextMeshProUGUI notification;

    public void Init(Player winner, Player loser)
    {
        winnersAvatar.sprite = iconsContent.icons[winner.iconId].sprite;
        winnersAvatar.color = winner.color;

        winnersNameText.text = winner.nickname;
        winnersNameText.color = winner.color;

        string winnerName = "<color=#" + winner.color.ToHexString() + ">" + winner.nickname + "</color>";
        string loserName = "<color=#" + loser.color.ToHexString() + ">" + loser.nickname + "</color>";

        notification.text = "Поэтому " + winnerName + " получает территорию " + loserName + " ";
    }
}
