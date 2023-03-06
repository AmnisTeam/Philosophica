using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerPlacement : MonoBehaviour {
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

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject remotePlayerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[spawnIndex].position, Quaternion.identity);
            remotePlayerObject.transform.SetParent(spawnPoints[spawnIndex].transform, false);

            TextMeshProUGUI remoteTempText = remotePlayerObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Image remotePlayerIcon = remotePlayerObject.transform.GetChild(2).GetComponent<Image>();

            remoteTempText.text = player.NickName;
            remotePlayerIcon.sprite = icons[(int)player.CustomProperties["playerIconId"]];
            remotePlayerIcon.color = instanceColorHolder.colors[(int)player.CustomProperties["playerColorIndex"]];

            spawnIndex++;
        }
    }

    void Update() {
        
    }
}
