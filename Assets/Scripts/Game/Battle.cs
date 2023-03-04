using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    public Opponent[] opponents;
    public Region region;
    public int winnerId = -1;
    public List<QuestionManager.Question> questions = new List<QuestionManager.Question>();
    public int currentQuestion = 0;

    public Opponent GetWinner()
    {
        foreach (var o in opponents)
        {
            if (o.health > 0)
                return o;
        }
        return null;
    }

    public Opponent GetLoser()
    {
        foreach (var o in opponents)
        {
            if (o.health <= 0)
                return o;
        }
        return null;
    }

    public Opponent GetDefender()
    {
        foreach (var o in opponents)
        {
            foreach (var cr in o.player.claimedRegions)
            {
                if (cr == region)
                    return o;
            }
        }
        return null;
    }

    public int GetDeadCount()
    {
        int count = 0;
        foreach (var o in opponents)
        {
            if (o.health <= 0)
                count++;
        }
        return count;
    }

    public Battle(Opponent opponent1, Opponent opponent2, Region region)
    {
        opponents = new Opponent[] { opponent1 , opponent2 };
        this.region = region;
    }
}
