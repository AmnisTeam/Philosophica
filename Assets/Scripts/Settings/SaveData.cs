using System;

[Serializable]
public class SaveData
{
    public float generalVolume;
    public float soundVolume;
    public float musicVolume;

    public string nickname;

    public SaveData()
    {
        nickname = "test"; //Сделать рандомный никнейм с сервера
    }
}
