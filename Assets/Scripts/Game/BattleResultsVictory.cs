using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultsVictory : MonoBehaviour
{
    public IconsContentHolder iconsContent;
    public Image winnersAvatar;
    
    public TextMeshProUGUI winnersNameText;

    public TextMeshProUGUI notification;

    public Image houseIcon;
    public Image shieldIcon;
    
    public void Init(Player winner, Player loser, string type = "offence")
    {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();

        winnersAvatar.sprite = iconsContent.lobbyIcons[winner.iconId];
        winnersAvatar.color = winner.color;

        winnersNameText.text = winner.nickname;
        winnersNameText.color = winner.color;

        string winnerName = $"<color=#{winner.color.ToHexString()}>{winner.nickname}</color>";
        string loserName = $"<color=#{loser.color.ToHexString()}>{loser.nickname}</color>";


        houseIcon.gameObject.SetActive(false);
        shieldIcon.gameObject.SetActive(false);
        if (type == "offence") {
            notification.text = $"Поэтому {winnerName} получает территорию {loserName}";
            houseIcon.gameObject.SetActive(true);
        } else if (type == "defense") {
            notification.text = $"Игрок {winnerName} успешно защитил свои территории от нападения";
            shieldIcon.gameObject.SetActive(true);
        }
    }
}
