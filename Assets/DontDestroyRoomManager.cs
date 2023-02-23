using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyRoomManager : MonoBehaviour {
    void Start() {
        
    }

    void Update() {
        
    }

    private void Awake() {
       /* GameObject obj = GameObject.FindGameObjectWithTag("ROOM_MANAGER_TAG");

        if (obj != null) {
            Destroy(gameObject);
        } else {
            gameObject.tag = "ROOM_MANAGER_TAG";
            DontDestroyOnLoad(gameObject);
        }*/
    }
}
