using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[Serializable]
public class Player : BaseRaw
{
    public int iconId;
    public int colorId;
    public UnityEngine.Color color;
    public string nickname;
    public List<Region> claimedRegions = new List<Region>();
    public int scores = 0;
    public bool isLocalClient;
    public bool isLose = false;

    public Player()
    {

    }

    public Player(int id, int iconId, int colorId, UnityEngine.Color color, string nickname, bool isLocalPlayer)
    {
        this.id = id;
        this.iconId = iconId;
        this.colorId = colorId;
        this.color = color;
        this.nickname = nickname;
        this.isLocalClient = isLocalPlayer;
    }

    public void ClaimRegion(Region region)
    {
        claimedRegions.Add(region);
    }

    public void LoseRegion(Region region)
    {
        claimedRegions.Remove(region);
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
    public ScoreTableManager scoreTableManager;
    public ToastShower toastShower;
    public Queue<Player> leavedPlayersQueue;
    private bool someoneLeaved = false;
    
    public bool IsHaveNotLose()
    {
        bool haveNotLose = false;
        foreach (Player leavedPlayer in leavedPlayersQueue)
        {
            if (!leavedPlayer.isLose)
            {
                haveNotLose = true;
                break;
            }
        }
        return haveNotLose;
    }

    public void connected(Player player)
    {
        players.add(player);
        //oldPlayersCount++;
        playerAnswerData.addwid(new PlayerAnswerData(), player);
        //Debug.Log("Player " + player.nickname + " has been connected!");
        //tabMenuManager.updateTabMenu();
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
        toastShower.showText($"Игрок <color=#{player.color.ToHexString()}>{player.nickname}</color> покинул игру.");
        leavedPlayersQueue.Enqueue(player);
        players.list.RemoveAt(id);
        scoreTableManager.RemovePlayer(id);
        //tabMenuManager.disconnectPlayer(id);
    }

    public void disconnect(int id)
    {
        toastShower.showText($"Игрок <color=#{players.get(id).color.ToHexString()}>{players.get(id).nickname}</color> покинул игру.");
        leavedPlayersQueue.Enqueue(players.get(id));
        players.list.RemoveAt(id);
        scoreTableManager.RemovePlayer(id);
        someoneLeaved = true;
        //tabMenuManager.disconnectPlayer(id);
    }

    public Photon.Realtime.Player GetPhotonPlayerByPlayer(Player player)
    {
        Photon.Realtime.Player photonPlayer = null;
        for (int x = 0; x < PhotonNetwork.PlayerList.Length; x++)
            if (player.id == PhotonNetwork.PlayerList[x].ActorNumber - 1)
            {
                photonPlayer = PhotonNetwork.PlayerList[x];
                break;
            }
        return photonPlayer;
    }

    public Photon.Realtime.Player GetRoomPlayerByColorId(int colorId) {
        Photon.Realtime.Player roomPlayer = null;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++) {
            if ((int)PhotonNetwork.CurrentRoom.Players[i].CustomProperties["playerColorIndex"] == colorId) {
                roomPlayer = PhotonNetwork.CurrentRoom.Players[i];
                break;
            }
        }

        return roomPlayer;
    }

    public bool DidSomeoneLeave()
    {
        return someoneLeaved;
    }

    public void RefreshSomeoneLeaveState()
    {
        someoneLeaved = false;
    }

    //public void UpdateOldPlayersCount()
    //{
    //    oldPlayersCount = players.count;
    //}

    //public bool DidAnyPlayerLeave()
    //{
    //    bool playerLeaved = players.count < oldPlayersCount;

    //    return players.count < oldPlayersCount;
    //}

    private void Awake()
    {
        leavedPlayersQueue = new Queue<Player>();
        //tabMenuManager = GetComponent<TabMenuManager>();
        playerAnswerData = new BaseTable<PlayerAnswerData>();
       /* scoreTableManager = GameObject.FindGameObjectWithTag("SCORE_TABLE_TAG").GetComponent<ScoreTableManager>();
        toastShower = GameObject.FindGameObjectWithTag("TOAST_MANAGER_TAG").GetComponent<ToastShower>();*/
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
