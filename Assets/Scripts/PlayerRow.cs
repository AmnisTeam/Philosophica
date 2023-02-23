using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    public Image avatar;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI timeText;

    public void Init(Sprite avatar, string playerName, string answer, string time)
    {
        this.avatar.sprite = avatar;
        playerNameText.text = playerName;
        answerText.text = answer;
        timeText.text = time;
    }
}
