using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoreTableRow : MonoBehaviour, IPointerDownHandler
{
    public GameObject backgroundField;
    public CanvasGroup backgroundRow;
    public GameObject iconContanier;
    public Image icon;
    public CanvasGroup content;
    public TMPro.TMP_Text nickname;
    public TMPro.TMP_Text countLands;
    public TMPro.TMP_Text countScores;

    public ScoreTableManager scoreTableManager;

    public float timeToDisappearanceContentAndStars;
    public float timeToHiddenBackgroundToIcon;

    public bool isOpen = false;

    public void FillRow(Sprite iconSprite, string nickname, int countLands, int maxCountLands, int countScores, Color color)
    {
        icon.sprite = iconSprite;
        this.nickname.text = nickname;
        this.countLands.text = countLands.ToString() + "/" + maxCountLands;
        this.countScores.text = countScores.ToString();
        icon.color = color;
    }

    public void CloseRow()
    {
        content.LeanAlpha(0, timeToDisappearanceContentAndStars).setOnComplete(() => { content.gameObject.SetActive(false); });
        backgroundRow.LeanAlpha(0, timeToDisappearanceContentAndStars).setOnComplete(() => { backgroundRow.gameObject.SetActive(false); });

        float iconContainerWidth = iconContanier.GetComponent<RectTransform>().rect.width;
        float scoreTableMaxWidth = GetComponent<RectTransform>().rect.width;
        LeanTween.value(0.0f, 1.0f, timeToHiddenBackgroundToIcon).setDelay(timeToDisappearanceContentAndStars).setEaseInOutQuint().setOnUpdate((float value) => 
        {
            RectTransform rectTransform = backgroundField.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(scoreTableMaxWidth, iconContainerWidth, value));
        });
    }

    public void OpenRow()
    {
        float iconContainerWidth = iconContanier.GetComponent<RectTransform>().rect.width;
        float scoreTableMaxWidth = GetComponent<RectTransform>().rect.width;

        LeanTween.value(0.0f, 1.0f, timeToHiddenBackgroundToIcon).setEaseInOutQuint().setOnUpdate((float value) =>
        {
            RectTransform rectTransform = backgroundField.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(iconContainerWidth,  scoreTableMaxWidth, value));
        });

        content.gameObject.SetActive(true);
        backgroundRow.gameObject.SetActive(true);
        content.LeanAlpha(1, timeToDisappearanceContentAndStars).setDelay(timeToHiddenBackgroundToIcon);
        backgroundRow.LeanAlpha(1, timeToDisappearanceContentAndStars).setDelay(timeToHiddenBackgroundToIcon);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scoreTableManager.ChangeStateOfScoreTable(gameObject);
    }

    private void Update()
    {   

    }
}
