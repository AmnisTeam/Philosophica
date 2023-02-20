using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBaseModel
{
    public string tableName;
    public List<DataBaseField> fields = new List<DataBaseField>();

    public DataBaseModel(string tableName)
    {
        this.tableName = tableName;
    }

    public void save()
    {
        SelectArg arg = null;
        for (int x = 0; x < fields.Count; x++)
            if (fields[x].isPrimaryKey)
            {
                arg = new SelectArg(fields[x].name, fields[x].Get());
                break;
            }

        List<DataBaseModel> selectedModel = DataBase.Select(Clone(), arg);
        if(selectedModel.Count > 0)
            DataBase.Update(this, arg); //update
        else
            DataBase.Insert(this); //insert
    }

    public abstract DataBaseModel Clone();

    
}
