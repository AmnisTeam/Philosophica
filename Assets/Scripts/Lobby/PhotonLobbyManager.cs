using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks {
    public string mainSceneName;
    public GameObject playerCointainer;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    private int spawnIndex = 0;

    void Start() {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerPrefab, spawnPoints[spawnIndex]);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
            spawnIndex++;
        }
    }

    void Update() {
        
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log("»грок " + newPlayer.NickName + " зашЄл");

        Transform spawnPoint = spawnPoints[spawnIndex];
        //GameObject playerSpawned = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        GameObject tempListing = Instantiate(playerPrefab, spawnPoints[spawnIndex]);
        
        //GameObject tempListing = Instantiate(playerPrefab, playerCointainer.transform);
        //TextMeshProUGUI tempText = playerSpawned.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tempText = tempListing.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tempText.text = newPlayer.NickName;
        spawnIndex++;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log("»грок " + otherPlayer.NickName + " вышел");

        foreach (Transform spawn in spawnPoints) {
            for (var i = spawn.childCount-1; i >= 0; i--) {
                Object.Destroy(spawn.GetChild(i).gameObject);
                spawnIndex--;
            }
        }

         foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerPrefab, spawnPoints[spawnIndex]);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
            spawnIndex++;
        }
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel(mainSceneName);
    }

    public void LeaveLobbyFunc() {
        PhotonNetwork.LeaveRoom();
    }
}
