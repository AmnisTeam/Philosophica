using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    GameObject obj;
    SoundEffects se;

    private void Start()
    {
        obj = GameObject.FindWithTag("SOUND_EFFECTS_TAG");
        se = obj.GetComponent<SoundEffects>();
    }

    public void SoundPlay(string tag)
    {
        se.PlaySound(tag);
    }

}