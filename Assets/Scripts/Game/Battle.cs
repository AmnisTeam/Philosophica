using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    public Opponent[] opponents;
    public int winnerId = -1;
    public List<QuestionManager.Question> questions = new List<QuestionManager.Question>();
    public int currentQuestion = 0;

    public Battle(Opponent opponent1, Opponent opponent2)
    {
        opponents = new Opponent[] { opponent1 , opponent2 };
    }
}
