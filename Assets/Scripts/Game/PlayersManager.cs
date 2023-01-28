using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : BaseRaw
{
    public int iconId;
    public Color iconColor;
    public string nickname;

    public Player()
    {

    }

    public Player(int id, int iconId, Color iconColor, string nickname)
    {
        this.id = id;
        this.iconId = iconId;
        this.iconColor = iconColor;
        this.nickname = nickname;
    }
}

public class PlayerAnswerData : BaseRaw
{
    public int answerId;
    public float timeToAnswer;
}
public class PlayersManager : MonoBehaviour
{
    public int MAX_COUNT_PLAYERS = 4;
    public BaseTable<Player> players;
    public BaseTable<PlayerAnswerData> playerAnswerData;
    public ConfigTemp config;
    private TabMenuManager tabMenuManager;
    
    public void connected(Player player)
    {
        players.add(player);
        playerAnswerData.addwid(new PlayerAnswerData(), player);
        Debug.Log("Player " + player.nickname + " has been connected!");
        tabMenuManager.updateTabMenu();
    }

    public void disconnect(Player player)
    {
        int id = 0;
        for(int x = 0; x < players.count; x++)
            if(players.get(x) == player)
            {
                id = x;
                break;
            }
        players.list.RemoveAt(id);
        tabMenuManager.disconnectPlayer(id);
    }

    public void disconnect(int id)
    {
        players.list.RemoveAt(id);
        tabMenuManager.disconnectPlayer(id);
    }

    void Start()
    {
        tabMenuManager = GetComponent<TabMenuManager>();
        players = new BaseTable<Player>();
        playerAnswerData = new BaseTable<PlayerAnswerData>();

        connected(new Player(0, 0, new Color(255, 0, 0), "SpectreSpect"));
        connected(new Player(1, 1, new Color(0, 255, 0), "DotaKot"));
        connected(new Player(2, 2, new Color(0, 0, 255), "ThEnd"));
        connected(config.me);
    }

    void Update()
    {

    }
}
