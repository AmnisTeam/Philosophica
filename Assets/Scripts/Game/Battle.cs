using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    public Opponent opponent1;
    public Opponent opponent2;
    public List<QuestionManager.Question> questions = new List<QuestionManager.Question>();

    public Battle(Opponent opponent1, Opponent opponent2)
    {
        this.opponent1 = opponent1;
        this.opponent2 = opponent2;
    }
}
