using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Client
{
    public int id;
    public bool isConnected;
    public string nickname;
}

public class PlayerManager : MonoBehaviour
{
    const int amountPlayers = 4;
    int id = 0;



    public List<Client> clients = new List<Client>();
    public GameObject[] playerObjects = new GameObject[amountPlayers];
    public GameObject[] playerNicknames = new GameObject[amountPlayers];

    ConfigManager configManager = new ConfigManager();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerObjects.Length; i++)
            playerObjects[i].SetActive(false);

        //configManager = GetComponent<ConfigManager>();
        Client p = new Client();
        p.id = id;
        p.isConnected = true;
        p.nickname = configManager.GetNickname();
        AddPlayer(p);
        id++;
        playerObjects[0].SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < clients.Count; i++)
        {

            if (clients[i].isConnected)
            {
                //playerObjects[i].GetComponent<Animator>().SetBool("Open", true);

                ////для отладки
                string s = clients[i].nickname;
                s += ' ';
                s += clients[i].id;
                ////для отладки

                playerNicknames[i].GetComponent<TMP_Text>().SetText(s, true);
            }
            //else
                //playerObjects[i].GetComponent<Animator>().SetBool("Open", false);
        }
    }

    public void AddPlayer(Client player)
    {
        if (player != null)
            clients.Add(player);
        else
        {
            Client p = new Client();
            p.id = id;
            p.isConnected = true;
            p.nickname = "test";
            id++;
            clients.Add(p);
        }
    }
    public void AddTestPlayer() //для отладки
    {
        if (clients.Count < 4)
        {
            Client p = new Client();
            p.id = id;
            p.isConnected = true;

            var rnd = new System.Random();  // для отладки
            var n = rnd.Next(0, 26);        // для отладки

            p.nickname = NumberToAZ(n).ToString();
            clients.Add(p);
            playerObjects[id].SetActive(true);
            id++;
        }
    }


    public void updateSequence()
    {
        //playerObjects[clients.Count - 1].GetComponent<Animator>().SetBool("Open", false);
        playerObjects[clients.Count].SetActive(false);
        for (int i = 0; i < clients.Count; i++)
            clients[i].id = i;
        id--;
    }

    public void KickPlayer(int idx)
    {
        clients.RemoveAt(idx);
        updateSequence();
    }



    char NumberToAZ(int num) // для отладки
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[num];
    }

}
