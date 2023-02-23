using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public string createdTag;

    public void PlayTapOnButtonClick()
    {
        GameObject obj = GameObject.FindWithTag(this.createdTag);
        SoundEffects se = obj.GetComponent<SoundEffects>();
        se.PlayTapOnButtonClick();
    }
}
