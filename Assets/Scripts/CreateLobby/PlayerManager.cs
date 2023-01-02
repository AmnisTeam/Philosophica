using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Player
{
    public bool isConnected;
    public string nickname;
}

public class PlayerManager : MonoBehaviour
{
    public Player[] players = new Player[4];
    public GameObject[] playerObjects = new GameObject[4];
    public Button button;
    
    // Start is called before the first frame update
    void Start()
    {
        players[0].isConnected = true;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {

            if (players[i].isConnected)
            {
                playerObjects[i].GetComponent<Animator>().SetBool("Open", true);

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
