using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyDataLoader : MonoBehaviour
{
    public TextMeshProUGUI lobbyIcon;
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyCode;
    public TextMeshProUGUI lobbyPassword;

    void Awake()
    {
        //lobbyIcon.text = CreateLobbyDataHolder.lobbyIcon;
        lobbyName.text = CreateLobbyDataHolder.lobbyName;
        lobbyCode.text = "#123456";
        lobbyPassword.text = CreateLobbyDataHolder.lobbyPassword;
    }
}
