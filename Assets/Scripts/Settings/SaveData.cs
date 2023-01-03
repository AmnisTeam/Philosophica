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
        nickname = "123"; //Сделать рандомный никнейм с сервера
    }
}
