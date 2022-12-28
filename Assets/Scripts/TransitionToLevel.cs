using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToLevel : MonoBehaviour
{
    public string sceneName;
    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

}
