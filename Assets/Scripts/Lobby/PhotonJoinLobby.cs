using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PhotonJoinLobby : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI lobbyIdInput;
    public TextMeshProUGUI lobbyPasswdInput;

    public void JoinLobbyFunc() {
        PhotonNetwork.JoinRoom(lobbyIdInput.text);
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        PhotonNetwork.LoadLevel("Lobby(Stars)");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(returnCode);
        Debug.Log(message);
    }
}
