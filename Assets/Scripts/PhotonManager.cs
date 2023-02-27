using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PhotonManager : MonoBehaviourPunCallbacks {
    void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update() {
        
    }

    public override void OnConnectedToMaster() {
        Debug.Log(PhotonNetwork.CloudRegion);

        var data = SaveManager.Load<SaveData>("mainSave");
        PhotonNetwork.NickName = data.nickname;

        SceneManager.LoadScene("MainScene(Stars)");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.EnableCloseConnection = true;
    }
}
