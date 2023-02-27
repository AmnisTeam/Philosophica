using System;

[Serializable]
public class SaveData
{
    public float generalVolume;
    public float soundVolume;
    public float musicVolume;

    public string playerUuid;
    public string nickname;

    public int iconID;

    public SaveData()
    {
        // небытие
    }
}
