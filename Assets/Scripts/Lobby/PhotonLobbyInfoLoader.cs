using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;
using Photon.Realtime;

public class PhotonLobbyInfoLoader : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI lobbyIdText;
    public TextMeshProUGUI lobbyPasswdText;
    public Image lobbyIconImage;
    public string iconsHolderTag;

    void Start() {
        lobbyIdText.text = $"#{PhotonNetwork.CurrentRoom.CustomProperties["lobbyId"]}";
        lobbyNameText.text = PhotonNetwork.CurrentRoom.CustomProperties["lobbyName"].ToString();

        GameObject iconsHolderObject = GameObject.FindGameObjectWithTag(iconsHolderTag);
        IconsContentHolder iconsContentHolderInstance = iconsHolderObject.GetComponent<IconsContentHolder>();
        Sprite[] icons = iconsContentHolderInstance.lobbyIcons;
        lobbyIconImage.sprite = icons[Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["lobbyIconId"])];
    }

    void Update() {
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo roomInfo in roomList) {
            Debug.Log(roomInfo.Name);
        }
    }
}
