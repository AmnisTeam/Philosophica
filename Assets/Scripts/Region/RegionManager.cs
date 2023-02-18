using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public PlayersManager playersManager;
    public RegionCreator regionCreator;

    public UnityEngine.Color baseRegionColor;

    private List<Player> players; 
    private List<Region> regions;

    public void SetAllRegionsBaseColor()
    {
        for (int i = 0; i < regions.Count; i++)
            regions[i].SetColor(baseRegionColor);
    }

    public void SetClaimedRegionsPlayersColors()
    {
        for (int i = 0; i < players.Count; i++)
            for (int j = 0; j < players[i].claimedRegions.Count; j++)
                players[i].claimedRegions[j].SetColor(players[i].color);
    }

    public void UpdateRegions()
    {
        SetAllRegionsBaseColor();
        SetClaimedRegionsPlayersColors();
    }

    void Start()
    {
        players = playersManager.players.list;
        //regions = regionCreator.regions;
    }

    void Update()
    {
        //UpdateRegions();
    }
}
