using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LeaveButton : MonoBehaviour {
    void Start() {

    }

    void Update() {
        
    }

    private void Awake() {
        Button btn = GameObject.FindGameObjectWithTag("GAME_LEAVE_TO_LOBBY").GetComponent<Button>();

        /*btn.onClick.AddListener(() => {            
            if (NetworkServer.active && NetworkClient.isConnected) {
                NetworkManager.singleton.StopHost();
            } else if (NetworkClient.isConnected) {
                NetworkManager.singleton.StopClient();
            } else if (NetworkServer.active) {
                NetworkManager.singleton.StopServer();
            }

            //RoomManager.instance.ServerChangeScene("MainScene(Stars)");
        });*/


    }
}
