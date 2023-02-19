using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusic : MonoBehaviour
{
    [Header("Genaral")]
    public float generalVolume;

    [Header("Music")]
    public AudioClip[] backgroundMusic;
    public AudioSource backgroundMusicSource;
    public float backgroundVolume;

    private void UpdateBackgroundVolume()
    {
        if (PlayerPrefs.HasKey(ConfigManager.saveKey))
        {
            var data = SaveManager.Load<SaveData>(ConfigManager.saveKey);

            this.generalVolume = data.generalVolume;

            this.backgroundVolume = data.musicVolume;
            this.backgroundMusicSource.volume = this.backgroundVolume * generalVolume;
        }
        else
        {
            this.backgroundVolume = 0.5f;
            this.backgroundMusicSource.volume = this.backgroundVolume;
        }
    }

    void Awake()
    {
        var rnd = new System.Random();
        var n = rnd.Next(0, backgroundMusic.Length);
        backgroundMusicSource.clip = backgroundMusic[n];

        UpdateBackgroundVolume();

        backgroundMusicSource.Play();

    }

    private void LateUpdate()
    {
        UpdateBackgroundVolume();
    }
}
