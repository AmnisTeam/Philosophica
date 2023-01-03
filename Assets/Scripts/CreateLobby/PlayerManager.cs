using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Player
{
    public int id;
    public bool isConnected;
    public string nickname;
}

public class PlayerManager : MonoBehaviour
{
    const int amountPlayers = 4;

    public Player[] players = new Player[amountPlayers];
    public GameObject[] playerObjects = new GameObject[amountPlayers];
    public GameObject[] playerNicknames = new GameObject[amountPlayers];
    public Button button;

    ConfigManager configManager = new ConfigManager();

    // Start is called before the first frame update
    void Start()
    {
        players[0].isConnected = true;
        players[0].nickname = configManager.GetNickname();

        players[1].isConnected = true;
        players[2].isConnected = true;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {

            if (players[i].isConnected)
            {
                playerObjects[i].GetComponent<Animator>().SetBool("Open", true);
                playerNicknames[0].GetComponent<TMP_Text>().SetText(players[0].nickname, true);
            }
            else
                playerObjects[i].GetComponent<Animator>().SetBool("Open", false);
        }
    }

    public void KickPlayer_1()
    {
        players[0].isConnected = false;
    }
    public void KickPlayer_2()
    {
        players[1].isConnected = false;
    }
    public void KickPlayer_3()
    {
        players[2].isConnected = false;
    }
    public void KickPlayer_4()
    {
        players[3].isConnected = false;
    }
}
