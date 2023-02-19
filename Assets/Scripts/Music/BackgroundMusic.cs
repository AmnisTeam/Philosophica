using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusic : MonoBehaviour
{
    //public Slider slider;

    //public string saveVolumeKey;
    //public string sliderTag;
    //public string textVolumeTag;

    public string settingsTag;

    public float volume;

    public AudioClip[] backgroundMusic;
    public AudioSource audioSource;

    void Awake()
    {
        var rnd = new System.Random();
        var n = rnd.Next(0, backgroundMusic.Length);
        audioSource.clip = backgroundMusic[n];

        /*
        if (settingsMusicVolume != null)
        {
            this.volume = settingsMusicVolume.GetComponent<Slider>().value;
            this.audioSource.volume = this.volume;
        }
        else
        {
            this.volume = 0.5f;
        }*/

        if (PlayerPrefs.HasKey(ConfigManager.saveKey))
        {
            var data = SaveManager.Load<SaveData>(ConfigManager.saveKey);

            this.volume = data.soundVolume;
            this.audioSource.volume = this.volume;
        }
        else
        {
            this.volume = 0.5f;
            this.audioSource.volume = this.volume;
        }

        audioSource.Play();


    }

    //private void LateUpdate()
    //{
    //    GameObject settingsMusicVolume = GameObject.FindWithTag(this.settingsTag);
    //    if (settingsMusicVolume != null)
    //    {
    //        this.volume = settingsMusicVolume.GetComponent<Slider>().value;

    //        //this.slider = settings.GetComponent<Slider>();
    //        //this.volume = slider.value;

    //        //if (this.audioSource.volume != this.volume)
    //        //{
    //        //    PlayerPrefs.SetFloat(this.saveVolumeKey, this.volume);
    //        //}
    //    }
    //    this.audioSource.volume = this.volume;
    //}
}
