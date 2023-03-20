using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameLetterTile : MonoBehaviour
{
    public TMPro.TMP_Text letterText;
    public TMPro.TMP_Text leftDownNumber;
    public TMPro.TMP_Text rightTopNumber;
    public char letter;
    private char oldLetter;

    public void SetLetter(char letter, int number)
    {
        this.letter = letter;
        leftDownNumber.text = number.ToString();
        rightTopNumber.text = number.ToString();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(oldLetter != letter)
        {
            letterText.text = letter.ToString();
            oldLetter = letter;
        }
    }
}
