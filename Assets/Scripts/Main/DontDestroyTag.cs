using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyTag : MonoBehaviour {
    public string objectTag;

    private void Awake() {
        GameObject obj = GameObject.FindWithTag(objectTag);

        if (obj != null) {
            Destroy(gameObject);
        } else {
            gameObject.tag = objectTag;
            DontDestroyOnLoad(gameObject);
        }
    }
}
