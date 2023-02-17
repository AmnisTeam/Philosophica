using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : BaseRaw
{
    public int iconId;
    public UnityEngine.Color color;
    public string nickname;
    public List<Region> claimedRegions = new List<Region>();

    public Player()
    {

    }

    public Player(int id, int iconId, UnityEngine.Color color, string nickname)
    {
        this.id = id;
        this.iconId = iconId;
        this.color = color;
        this.nickname = nickname;
    }

    public void ClaimRegion(Region region)
    {
        claimedRegions.Add(region);
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
    public BaseTable<Player> players = new BaseTable<Player>();
    public BaseTable<PlayerAnswerData> playerAnswerData;
    public ConfigTemp config;
    private TabMenuManager tabMenuManager;
    public ToastShower toastShower;
    
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
        toastShower.showText("Игрок " + player.nickname + " покинул игру.");
        players.list.RemoveAt(id);
        tabMenuManager.disconnectPlayer(id);
    }

    public void disconnect(int id)
    {
        toastShower.showText("Игрок " + players.get(id).nickname + " покинул игру.");
        players.list.RemoveAt(id);
        tabMenuManager.disconnectPlayer(id);
    }

    void Start()
    {
        tabMenuManager = GetComponent<TabMenuManager>();
        playerAnswerData = new BaseTable<PlayerAnswerData>();

        connected(new Player(0, 0, new UnityEngine.Color(255, 0, 0), "SpectreSpect"));
        connected(new Player(1, 1, new UnityEngine.Color(0, 255, 0), "DotaKot"));
        connected(new Player(2, 2, new UnityEngine.Color(0, 0, 255), "ThEnd"));
        connected(config.me);
    }

    void Update()
    {

    }
}
