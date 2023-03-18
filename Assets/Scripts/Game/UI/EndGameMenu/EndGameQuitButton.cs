using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EndGameQuitButton : MonoBehaviourPunCallbacks
{

    public void LeaveTheGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel("MainScene(Stars)");
    }

}
