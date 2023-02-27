using Photon.Pun;
using TMPro;

public class PhotonJoinLobby : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI lobbyIdInput;
    public TextMeshProUGUI lobbyPasswdInput;

    public void JoinLobbyFunc() {
        PhotonNetwork.JoinRoom(lobbyIdInput.text);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel("Lobby(Stars)");
    }
}
