using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateLobbySaveData : MonoBehaviour
{
    public TextMeshProUGUI lobbyIcon;
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyPassword;
    public void SaveData()
    {
        CreateLobbyDataHolder.lobbyIcon = 1;
        CreateLobbyDataHolder.lobbyName = lobbyName.text;
        CreateLobbyDataHolder.lobbyPassword = lobbyPassword.text;
    }
}
