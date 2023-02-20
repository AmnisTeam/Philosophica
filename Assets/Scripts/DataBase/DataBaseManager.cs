using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    void Awake()
    {
        DataBase.Init();
        DataBase.RegisterModel(new ClientModel());
    }

    void Start()
    {
        ClientModel clients = new ClientModel();
        clients.name.value = "Some name";
        clients.iconId.value = 4;
        clients.isConnected.value = 0;
        DataBase.Insert(clients);

        //List<DataBaseModel> selectedModels = DataBase.Select(new ClientModel());
        //(selectedModels[1] as ClientModel).name.value = "asd";
        //selectedModels[1].save();
        //for (int x = 0; x < selectedModels.Count; x++)
        //{
        //    string str = "";
        //    for(int y = 0; y < selectedModels[x].fields.Count; y++)
        //        str += selectedModels[x].fields[y].name + " = " + selectedModels[x].fields[y].Get() + " ";
        //    Debug.Log(str);
        //}
    }

    void Update()
    {
        
    }
}
