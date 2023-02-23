using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderTransition : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene("MainScene(Stars)");
    }

    void Update() {
        
    }
}
