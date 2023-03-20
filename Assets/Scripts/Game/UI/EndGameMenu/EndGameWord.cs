using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LetterWithPoints
{
    public char letter;
    public int points;

    public LetterWithPoints(char letter, int points)
    {
        this.letter = letter;
        this.points = points;
    }
}

public class TilesWordInfo
{
    public List<LetterWithPoints> tiles;

    public TilesWordInfo()
    {
        tiles = new List<LetterWithPoints>();
    }
}

public class EndGameWord : MonoBehaviour
{
    public GameObject letterPrifab;
    public float offset;
    private GameObject[] letters;

    public void SetWord(string word, int[] points = null)
    {
        if(letters != null)
        {
            for(int x = 0; x < letters.Length; x++)
                Destroy(letters[x]);
        }

        letters = new GameObject[word.Length];

        float letterSizeX = letterPrifab.GetComponent<RectTransform>().rect.width;
        float localOffsetX = offset;
        float sizeWordX = (letterSizeX + localOffsetX) * word.Length;
        float maxWordSizeX = GetComponent<RectTransform>().rect.width;
        if (sizeWordX > maxWordSizeX)
        {
            float relatedSizeX = maxWordSizeX / sizeWordX;
            localOffsetX *= relatedSizeX;
            letterSizeX *= relatedSizeX;
        }

        for (int x = 0; x < letters.Length; x++)
        {
            GameObject letter = Instantiate(letterPrifab, transform);
            letter.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, letterSizeX); 
            letter.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, letterSizeX);
            letter.transform.localPosition = new Vector3((letterSizeX + localOffsetX) * x, -GetComponent<RectTransform>().rect.height / 2 + letterSizeX / 2f);
            letter.GetComponent<EndGameLetterTile>().SetLetter(word[x], points != null ? points[x] : 1);
            letters[x] = letter;
        }
    }

    public void SetWord(TilesWordInfo tilesWordInfo)
    {
        char[] charsWord = new char[tilesWordInfo.tiles.Count];
        int[] points = new int[tilesWordInfo.tiles.Count];
        for(int x = 0; x < tilesWordInfo.tiles.Count; x++)
        {
            charsWord[x] = tilesWordInfo.tiles[x].letter;
            points[x] = tilesWordInfo.tiles[x].points;
        }

        string word = new string(charsWord);
        SetWord(word, points);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }
}
