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
    public Client(int id, string nickname/*, LocalColor playerColor*/)
    {
        this.id = id;
        this.isConnected = true;
        this.nickname = nickname;
        this.playerColor = new LocalColor(UnityEngine.Color.white);
    }

    public int id;
    public bool isConnected;
    public string nickname;
    public LocalColor playerColor;
}

[Serializable]
public class LocalColor
{
    public LocalColor(Color32 color, bool isBusy)
    {
        this.color = color;
        this.isBusy = isBusy;
    }
    public LocalColor(Color32 color)
    {
        this.color = color;
        this.isBusy = false;
    }

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
    public GameObject[] playerIcon = new GameObject[amountPlayers];
    public List<LocalColor> colors = new List<LocalColor>();
    public Sprite[] icons;

    ConfigManager configManager = new ConfigManager();

    void AddColors()
    {
        colors.Add(new LocalColor(UnityEngine.Color.red));
        colors.Add(new LocalColor(UnityEngine.Color.green));
        colors.Add(new LocalColor(UnityEngine.Color.blue));
        colors.Add(new LocalColor(UnityEngine.Color.yellow));
        colors.Add(new LocalColor(UnityEngine.Color.magenta));
    }

    void Start()
    {
        AddColors();

        for (int i = 0; i < playerObjects.Length; i++)
            playerObjects[i].SetActive(false);

        AddPlayer(new Client(id, configManager.GetNickname()));

        
        AddPlayer(new Client(id, "123"));
    }


    void Update()
    {
        for (int i = 0; i < clients.Count; i++)
        {

            if (clients[i].isConnected)
            {
                ////для отладки
                string s = clients[i].nickname;
                s += ' ';
                s += clients[i].id;
                ////для отладки

                playerNicknames[i].GetComponent<TMP_Text>().SetText(s, true);
            }
        }
    }

    public void AddPlayer(Client player)
    {
        if (clients.Count < amountPlayers)
        {
            LocalColor c = RandomColor();

            playerColor[id].GetComponent<Image>().color = c.color;
            playerIcon[id].GetComponent<Image>().sprite = icons[Randomizer(0, icons.Length)];
            playerIcon[id].GetComponent<Image>().color = c.color;

            player.playerColor = c;

            clients.Add(player);
            playerObjects[id].SetActive(true);
            id++;
        }
    }
    public void AddTestPlayer() //для отладки
    {
        if (clients.Count < amountPlayers)
        {
            AddPlayer(new Client(id, NumberToAZ(Randomizer(0, 26))));
        }
    }


    public void updateSequence(int idx)
    {
        //playerObjects[clients.Count - 1].GetComponent<Animator>().SetBool("Open", false);
        playerObjects[clients.Count].SetActive(false);

        for (int i = idx; i < clients.Count; i++)
        {
            clients[i].id = i;
            playerColor[i].GetComponent<Image>().color = playerColor[i + 1].GetComponent<Image>().color;
            playerIcon[i].GetComponent<Image>().sprite = playerIcon[i + 1].GetComponent<Image>().sprite;
            playerIcon[i].GetComponent<Image>().color = playerIcon[i + 1].GetComponent<Image>().color;
        }

        id--;
    }

    public void KickPlayer(int idx)
    {
        clients[idx].playerColor.isBusy = false;
        clients.RemoveAt(idx);
        updateSequence(idx);
    }



    string NumberToAZ(int num) // для отладки
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[num].ToString();
    }

    int Randomizer(int f, int l)
    {
        var rnd = new System.Random();
        var n = rnd.Next(f, l);
        return n;
    }

    LocalColor RandomColor()
    {
        LocalColor c = colors[Randomizer(0, colors.Count)];

        // Если цвета закончились
        int count_colors = 0;
        for (int i = 0; i < colors.Count; i++)
            if (colors[i].isBusy)
                count_colors++;

        if (count_colors == colors.Count)
            return new LocalColor(UnityEngine.Color.black, true);
        // Если цвета закончились

        while (c.isBusy)
            c = colors[Randomizer(0, colors.Count)];

        c.isBusy = true;
        return c;
    }
}
