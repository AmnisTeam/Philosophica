using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    public GameObject generalVolume;
    public GameObject soundVolume;
    public GameObject musicVolume;
    public GameObject nicknameTextField;

    private const string saveKey = "mainSave";



    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private SaveData GetSaveSnapshot()
    {
        var data = new SaveData()
        {
            generalVolume = this.generalVolume.GetComponent<Slider>().value,
            soundVolume = this.soundVolume.GetComponent<Slider>().value,
            musicVolume = this.musicVolume.GetComponent<Slider>().value,

            nickname = this.nicknameTextField.GetComponent<TMP_InputField>().text,
        };
        return data;
    }

    public void SaveSettings()
    {
        /*
         
        string s = nicknameTextField.GetComponent<TMP_InputField>().text;
        s += generalVolume.GetComponent<Slider>().value;
        s += soundVolume.GetComponent<Slider>().value;
        s += musicVolume.GetComponent<Slider>().value;
        Debug.Log(s);
        
         */

        Save();
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData>(saveKey);

        this.generalVolume.GetComponent<Slider>().value = (data.generalVolume);
        this.soundVolume.GetComponent<Slider>().value = (data.soundVolume);
        this.musicVolume.GetComponent<Slider>().value = (data.musicVolume);

        this.nicknameTextField.GetComponent<TMP_InputField>().text = (data.nickname);
    }
    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }



}
