using UnityEngine;
using Mirror;

public class ConnectionFlow : MonoBehaviour {
    public NetworkManager networkManager;

    void Start() {
        if (!Application.isBatchMode) {
            networkManager.StartClient();
        }
    }

    public void JoinClient() {
        networkManager.StartClient();
    }
}
