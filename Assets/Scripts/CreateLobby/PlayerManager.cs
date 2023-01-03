using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    public int id;
    public bool isConnected;
    public string nickname;
}

public class PlayerManager : MonoBehaviour
{
    const int amountPlayers = 4;
    int id = 0;

    public List<Player> players = new List<Player>();
    public GameObject[] playerObjects = new GameObject[amountPlayers];
    public GameObject[] playerNicknames = new GameObject[amountPlayers];
    public Button button;

    ConfigManager configManager = new ConfigManager();

    // Start is called before the first frame update
    void Start()
    {
        Player p = new Player();
        p.id = id;
        p.isConnected = true;
        p.nickname = configManager.GetNickname();
        AddPlayer(p);
        id++;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {

            if (players[i].isConnected)
            {
                playerObjects[i].GetComponent<Animator>().SetBool("Open", true);

                //для отладки
                string s = players[i].nickname;
                s += ' ';
                s += players[i].id;
                //для отладки

                playerNicknames[i].GetComponent<TMP_Text>().SetText(s, true);
            }
            else
                playerObjects[i].GetComponent<Animator>().SetBool("Open", false);
        }
    }

    public void AddPlayer(Player player)
    {
        if (player != null)
            players.Add(player);
        else
        {
            Player p = new Player();
            p.id = id;
            p.isConnected = true;
            p.nickname = "test";
            id++;
            players.Add(p);
        }
    }
    public void AddTestPlayer()
    {
        Player p = new Player();
        p.id = id;
        p.isConnected = true;
        p.nickname = "test";
        id++;
        players.Add(p);
    }


    public void updateSequence()
    {
        playerObjects[players.Count - 1].GetComponent<Animator>().SetBool("Open", false);
    }

    public void KickPlayer_1()
    {
        updateSequence();
        players.RemoveAt(0);
    }
    public void KickPlayer_2()
    {
        updateSequence();
        players.RemoveAt(1);
    }
    public void KickPlayer_3()
    {
        updateSequence();
        players.RemoveAt(2);
    }
    public void KickPlayer_4()
    {
        updateSequence();
        players.RemoveAt(3);
    }
}
