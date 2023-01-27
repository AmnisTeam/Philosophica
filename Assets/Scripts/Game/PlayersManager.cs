using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public int id;
    public int iconId;
    public string nickname;
    public int answerId;
    public float timeToAnswer;
}
public class PlayersManager : MonoBehaviour
{

    public LinkedList<Player> players;
    public ConfigTemp config;
    
    public void connected(Player player)
    {
        players.AddLast(player);
        Debug.Log("Player " + player.nickname + " has been connected!");
    }

    void Start()
    {
        players = new LinkedList<Player>();
        Player[] bots = new Player[4];
        for (int x = 0; x < 3; x++)
            bots[x] = new Player();

        bots[0].id = 0;
        bots[0].iconId = 0;
        bots[0].nickname = "SpectreSpect";
        bots[0].answerId = 0;

        bots[1].id = 1;
        bots[1].iconId = 1;
        bots[1].nickname = "DotaKot";
        bots[1].answerId = 1;

        bots[2].id = 2;
        bots[2].iconId = 2;
        bots[2].nickname = "ThEnd";
        bots[2].answerId = 2;

        for (int x = 0; x < 3; x++)
            connected(bots[x]);

        connected(config.me);
 
    }

    void Update()
    {

    }
}
