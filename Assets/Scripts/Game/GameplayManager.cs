using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private PlayersManager playersManager;
    public RegionsSystem regionSystem;

    public void Awake()
    {
        playersManager = GetComponent<PlayersManager>();
    }

    public void Start()
    {
        playersManager.connected(playersManager.config.me);
        playersManager.connected(new Player(0, 0, new Color(255, 0, 0), "SpectreSpect"));
        playersManager.connected(new Player(1, 1, new Color(0, 255, 0), "DotaKot"));
        playersManager.connected(new Player(2, 2, new Color(0, 0, 255), "ThEnd"));

        GrantPlayersStartingRegions();
    }

    public void Update()
    {
        UpdateRegionColors();
    }

    private void GrantPlayersStartingRegions()
    {
        //int[] claimedRegions = new int[playersManager.players.count];
        //System.Random rnd = new System.Random();   
        //for (int i = 0; i < playersManager.players.count; i++)
        //{
        //    int randomValue = 0;
        //    bool found = true;
        //    while(found)
        //    {
        //        randomValue = rnd.Next(0, regionSystem.regionSerds.Count);
        //        for (int j = 0; j < i; j++)
        //            if (randomValue == claimedRegions[j])
        //                found = true;
        //    }
        //    playersManager.players.get(i).ClaimRegion()
        //}

        for (int i = 0; i < playersManager.players.count; i++)
            playersManager.players.get(i).ClaimRegion(regionSystem.regionSerds[i].region);
    }

    private void UpdateRegionColors()
    {
        for (int i = 0; i < playersManager.players.count; i++)
        {
            for(int j = 0; j < playersManager.players.get(i).claimedRegions.Count; j++)
            {
                playersManager.players.get(i).claimedRegions[j].SetColor(playersManager.players.get(i).color);
            }
        }
    }
}
