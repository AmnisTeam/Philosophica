using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class PhotonCreateLobby : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI lobbyNameInput;
    public TextMeshProUGUI lobbyPasswdInput;
    public IconScroller lobbyIconScroller;

    public void CreateRoomFunc() {
        RoomOptions roomOptions = new RoomOptions();
        System.Random rand = new System.Random();

        string roomId = rand.Next(100000, 999999).ToString();
        roomOptions.MaxPlayers = 4;
        roomOptions.CustomRoomPropertiesForLobby = new string[]{"lobbyId", "lobbyName", "lobbyPasswd", "lobbyIconId"};
        roomOptions.CustomRoomProperties = new Hashtable{{"lobbyId", roomId}, {"lobbyName", lobbyNameInput.text}, {"lobbyPasswd", lobbyPasswdInput.text}, {"lobbyIconId", lobbyIconScroller.selectedId}};

        PhotonNetwork.CreateRoom(lobbyNameInput.text, roomOptions);
    }

    public override void OnCreatedRoom() {
        base.OnCreatedRoom();

        Debug.Log("Joined room "+PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Lobby(Stars)");

        //GameObject tempListing = Instantiate(playerPrefab, spawnPoints[spawnIndex]);
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();
        Debug.Log("Here");
    }
}
