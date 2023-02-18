using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateLobbySaveData : MonoBehaviour
{
    public IconScroller iconScroller;
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyPassword;
    public void SaveData()
    {
        CreateLobbyDataHolder.lobbyIconID = iconScroller.selectedId;
        CreateLobbyDataHolder.icons = iconScroller.sprites;
        CreateLobbyDataHolder.lobbyName = lobbyName.text;
        CreateLobbyDataHolder.lobbyPassword = lobbyPassword.text;
    }
}
