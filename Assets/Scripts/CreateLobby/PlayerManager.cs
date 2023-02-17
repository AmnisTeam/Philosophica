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
    public Color playerColor;
}

[Serializable]
public class Color
{
    public Color32 color;
    public bool isBusy;
}

public class PlayerManager : MonoBehaviour
{
    const int amountPlayers = 4;
    int id = 0;



    public List<Client> clients = new List<Client>();
    public GameObject[] playerObjects = new GameObject[amountPlayers];
    public GameObject[] playerNicknames = new GameObject[amountPlayers];
    public GameObject[] playerColor = new GameObject[amountPlayers];
    public List<Color> colors = new List<Color>();

    ConfigManager configManager = new ConfigManager();

    void AddColors()
    {
        Color c = new Color();
        c.color = UnityEngine.Color.red;
        colors.Add(c);

        c = new Color();
        c.color = UnityEngine.Color.green;
        colors.Add(c);

        c = new Color();
        c.color = UnityEngine.Color.blue;
        colors.Add(c);

        c = new Color();
        c.color = UnityEngine.Color.yellow;
        colors.Add(c);

    }

    void Start()
    {
        AddColors();

        for (int i = 0; i < playerObjects.Length; i++)
            playerObjects[i].SetActive(false);

        //configManager = GetComponent<ConfigManager>();
        Client p = new Client();
        p.id = id;
        p.isConnected = true;
        p.nickname = configManager.GetNickname();

        Color c = colors[Randomizer(0, colors.Count)];
        c.isBusy = true;
        playerColor[0].GetComponent<Image>().color = c.color;

        AddPlayer(p);
        id++;
        playerObjects[0].SetActive(true);

        AddTestPlayer();
        AddTestPlayer();
        AddTestPlayer();
    }


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
            p.nickname = NumberToAZ(Randomizer(0, 26)).ToString();

            int qwe = 0;
            Color c = colors[Randomizer(0, colors.Count)];
            while (c.isBusy)
            {
                c = colors[Randomizer(0, colors.Count)];
                qwe++;
                if (qwe > 100)
                    break;
            }
            c.isBusy = true;
            playerColor[id].GetComponent<Image>().color = c.color;



            clients.Add(p);
            playerObjects[id].SetActive(true);
            id++;
        }
    }


    public void updateSequence(int idx)
    {
        //playerObjects[clients.Count - 1].GetComponent<Animator>().SetBool("Open", false);
        playerObjects[clients.Count].SetActive(false);
        colors[idx].isBusy = false;
        for (int i = idx; i < clients.Count; i++)
        {
            clients[i].id = i;
            playerColor[i].GetComponent<Image>().color = playerColor[i + 1].GetComponent<Image>().color;
        }
        id--;
    }

    public void KickPlayer(int idx)
    {
        clients.RemoveAt(idx);
        updateSequence(idx);
    }



    char NumberToAZ(int num) // для отладки
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[num];
    }

    int Randomizer(int f, int l)
    {
        var rnd = new System.Random();
        var n = rnd.Next(f, l);
        return n;
    }
}
