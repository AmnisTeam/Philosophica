using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour {
    public static LobbyManager instance;

    public LobbyManager() {
        instance = this;
    }

    public void OnLobbyLeave() {
        Debug.Log("OnLobbyLeave");
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
            Debug.Log("Host stopped!");
        } else if (NetworkClient.isConnected) {
            NetworkManager.singleton.StopClient();
        } else if (NetworkServer.active) {
            NetworkManager.singleton.StopServer();
        }
    }
}
