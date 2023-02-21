using UnityEngine;
using Mirror;

public class NetworkFlow : MonoBehaviour {
    public void stopGame() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        } else if (NetworkClient.isConnected) {
            NetworkManager.singleton.StopClient();
        } else if (NetworkServer.active) {
            NetworkManager.singleton.StopServer();
        }
    }
}
