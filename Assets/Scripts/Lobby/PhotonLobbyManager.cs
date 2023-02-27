using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using UnityEngine.UI;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks {
    public string mainSceneName;
    public GameObject playerCointainer;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    private int spawnIndex = 0;

    GameObject avatarSprites;
    IconsContentHolder iconsContent;
    Sprite[] icons;
    GameObject colorsHolder;
    ColorsHolder instanceColorHolder;

    void Start() {
        avatarSprites = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG");
        iconsContent = avatarSprites.GetComponent<IconsContentHolder>();
        icons = iconsContent.lobbyIcons;

        colorsHolder = GameObject.FindGameObjectWithTag("COLOR_CONTENT_TAG");
        instanceColorHolder = colorsHolder.GetComponent<ColorsHolder>();

        instanceColorHolder.refillFreeIndicies();

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            if (!player.IsLocal) {
                instanceColorHolder.freeIndicies.Remove((int)player.CustomProperties["playerColorIndex"]);
            } else {
                var data = SaveManager.Load<SaveData>(ConfigManager.saveKey);
                var colorIndex = instanceColorHolder.getRandomIndex();
                var color = instanceColorHolder.freeIndicies[colorIndex];
                instanceColorHolder.freeIndicies.Remove(color);
                
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable{
                    {"playerColorIndex", color},
                    {"playerIconId", data.iconID}
                });
            }
        }
    }

    void Update() {
        
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log("»грок " + newPlayer.NickName + " зашЄл");
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        Debug.Log(">>> LobbyManager.OnPlayerPropertiesUpdate");
        UpdateList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log("»грок " + otherPlayer.NickName + " вышел");
        UpdateList();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel(mainSceneName);
    }

    public void LeaveLobbyFunc() {
        PhotonNetwork.LeaveRoom();
    }

    public void UpdateList() {
        foreach (Transform spawn in spawnPoints) {
            for (var i = spawn.childCount-1; i >= 0; i--) {
                PhotonNetwork.Destroy(spawn.GetChild(i).gameObject);
                spawnIndex--;
            }
        }

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject remotePlayerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[spawnIndex].position, Quaternion.identity);
            remotePlayerObject.transform.SetParent(spawnPoints[spawnIndex].transform, false);

            TextMeshProUGUI remoteTempText = remotePlayerObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Image remotePlayerColor = remotePlayerObject.transform.GetChild(3).GetComponent<Image>();
            Image remotePlayerIcon = remotePlayerObject.transform.GetChild(1).GetComponent<Image>();
            Button remotePlayerButton = remotePlayerObject.transform.GetChild(4).GetComponent<Button>();

            remoteTempText.text = player.NickName;
            remotePlayerColor.color = instanceColorHolder.colors[(int)player.CustomProperties["playerColorIndex"]];
            remotePlayerIcon.sprite = icons[(int)player.CustomProperties["playerIconId"]];
            remotePlayerIcon.color = instanceColorHolder.colors[(int)player.CustomProperties["playerColorIndex"]];

            if (player.IsMasterClient && player.IsLocal) {
                remotePlayerButton.gameObject.SetActive(false);
            } else if (player.IsMasterClient && !player.IsLocal) {
                remotePlayerButton.gameObject.SetActive(false);
            } else if (player.IsLocal) {
                remotePlayerButton.gameObject.SetActive(false);
            }

            spawnIndex++;
        }
    }
}
