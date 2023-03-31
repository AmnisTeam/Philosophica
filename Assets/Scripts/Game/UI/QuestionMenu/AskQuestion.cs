using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskQuestion : MonoBehaviourPunCallbacks
{
    public Color answerColor;
    public Color correctAnswerColor;

    public float buttonsChangeTime = 0.3f;

    public TextMeshProUGUI timerText;

    public TextMeshProUGUI questionText;

    public QuestionManager.Question question;

    public Button[] answerButtons;
    public TextMeshProUGUI[] answers;
    public Image[] answersBackgrounds;
    public Image[] answersBorders;

    public double timer = 0;

    public PhotonView pv;

    public void Init(QuestionManager.Question question)
    {
        changeButtonClickability(true);

        pv = GetComponent<PhotonView>();
        this.questionText.text = question.question; //todo12345
        this.question = question;

        for (int a = 0; a < answers.Length && a < question.answer.Length; a++)
            answers[a].text = question.answer[a];
        timer = 0;
        ResetAnswersWithoutAnimation();

    }

    public virtual void ShowCorrectAnswer()
    {
        changeButtonClickability(false);

        MarkAnswerAsCorrect(question.idRightAnswer);
    }

    public void HideCorrectAnswer()
    {
        for (int i = 0; i < question.answer.Length; i++)
            MarkAnswerAsDefault(i);
    }

    public void ResetAnswersWithoutAnimation()
    {
        for (int i = 0; i < question.answer.Length; i++)
        {
            answersBorders[i].GetComponent<CanvasGroup>().alpha = 0;
            answersBackgrounds[i].color = answerColor;
        }
    }
    public void MarkAnswerAsCorrect(int answerId)
    {
        LeanTween.color(answersBackgrounds[answerId].rectTransform, correctAnswerColor, buttonsChangeTime);
    }

    public void MarkAnswerAsDefault(int answerId)
    {
        LeanTween.color(answersBackgrounds[answerId].rectTransform, answerColor, buttonsChangeTime);
    }

    public virtual void SelectAnswer(int buttonId)
    {
        for (int i = 0; i < answersBorders.Length; i++)
            if (answersBorders[i].GetComponent<CanvasGroup>().alpha > 0)
                answersBorders[i].GetComponent<CanvasGroup>().LeanAlpha(0, buttonsChangeTime).setEaseOutSine();


        answersBorders[buttonId].GetComponent<CanvasGroup>().LeanAlpha(1, buttonsChangeTime).setEaseOutSine();
    }

    public void changeButtonClickability(bool state)
    {
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].enabled = state;
    } 

}
