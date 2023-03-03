using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultsState : GameState
{
    public GameObject BattleResultsVictory;
    public Battle battle;
    public BattleResultsState(GameObject BattleResultsVictory, Battle battle)
    {
        this.BattleResultsVictory = BattleResultsVictory;
        this.battle = battle;
    }

    public override void Start()
    {
        BattleResultsVictory.SetActive(true);
        BattleResultsVictory.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseOutSine();
        Player winner = battle.GetWinner().player;
        Player loser = battle.GetLoser().player;
        BattleResultsVictory.GetComponent<BattleResultsVictory>().Init(winner, loser);
    }

    public override void Update()
    {

    }
}
