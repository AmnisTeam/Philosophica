using System;
using UnityEngine;

public class BackgroundTask : MonoBehaviour {
    private const string saveKey = "mainSave";

    // Start is called before the first frame update
    void Start() {
        if (!PlayerPrefs.HasKey("player_uuid")) {
            string playerUuid = Guid.NewGuid().ToString();
            string playerName = ServerUtils.getNickname(playerUuid);
            PlayerPrefs.SetString("player_uuid", playerUuid);
            PlayerPrefs.SetString("nickname", playerName);

            var toSave = new SaveData() {
                generalVolume = 0,
                soundVolume = 0,
                musicVolume = 0,
                playerUuid = playerUuid,
                nickname = playerName
            };

            SaveManager.Save(saveKey, toSave);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
