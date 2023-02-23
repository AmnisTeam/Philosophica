using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    [Header("Genaral")]
    public float generalVolume;

    [Header("Sound")]
    public AudioSource soundSource;
    public AudioClip tap;

    public float soundVolume;

    public void PlayTapOnButtonClick()
    {
        soundSource.clip = tap;
        soundSource.Play();
    }

    private void Awake()
    {
        PlayTapOnButtonClick();
    }

    private void UpdateBackgroundVolume()
    {
        if (PlayerPrefs.HasKey(ConfigManager.saveKey))
        {
            var data = SaveManager.Load<SaveData>(ConfigManager.saveKey);

            this.generalVolume = data.generalVolume;

            this.soundVolume = data.soundVolume;
            this.soundSource.volume = this.soundVolume * generalVolume;
        }
        else
        {
            this.soundVolume = 0.5f;
            this.soundSource.volume = this.soundVolume;
        }
    }

    private void LateUpdate()
    {
        UpdateBackgroundVolume();
    }
}
