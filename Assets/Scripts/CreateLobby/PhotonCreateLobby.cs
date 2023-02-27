using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonCreateLobby : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI lobbyNameInput;
    public TextMeshProUGUI lobbyPasswdInput;
    public IconScroller lobbyIconScroller;

    public void CreateRoomFunc() {
        RoomOptions roomOptions = new RoomOptions();
        System.Random rand = new System.Random();

        // TODO: use lobby code
        string roomId = rand.Next(100000, 999999).ToString();
        roomOptions.MaxPlayers = 4;
        roomOptions.CustomRoomPropertiesForLobby = new string[]{"lobbyId", "lobbyName", "lobbyPasswd", "lobbyIconId"};
        roomOptions.CustomRoomProperties = new Hashtable{
            {"lobbyId", roomId},
            {"lobbyName", lobbyNameInput.text},
            {"lobbyPasswd", lobbyPasswdInput.text},
            {"lobbyIconId", lobbyIconScroller.selectedId}
        };

        PhotonNetwork.CreateRoom(lobbyNameInput.text, roomOptions);
    }

    public override void OnCreatedRoom() {
        PhotonNetwork.LoadLevel("Lobby(Stars)");
    }
}
