using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent
{
    public Player player;
    public double health;
    public double maxHealh;
    public int roundsWon;

    public List<PlayerAnswerData> playerAnswerData;

    public Opponent(Player player, double health, double maxHealh, int roundsWon)
    {
        playerAnswerData = new List<PlayerAnswerData>();
        this.player = player;
        this.health = health;
        this.maxHealh = maxHealh;
        this.roundsWon = roundsWon;
    }
}
