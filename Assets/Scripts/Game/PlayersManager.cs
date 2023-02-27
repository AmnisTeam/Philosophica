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

    public PlayerAnswerData()
    {
        answerId = -1;
        timeToAnswer = -1;
    }

    public PlayerAnswerData(int answerId, float timeToAnswer)
    {
        this.answerId = answerId;
        this.timeToAnswer = timeToAnswer;
    }
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
        //Debug.Log("Player " + player.nickname + " has been connected!");
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

    private void Awake()
    {
        tabMenuManager = GetComponent<TabMenuManager>();
        playerAnswerData = new BaseTable<PlayerAnswerData>();
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
