using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBaseField
{
    public string name;
    public string typeFiled;
    public bool isPrimaryKey = false;
    public bool isNotAddToTable = false;
    public DataBaseField(string typeFiled, DataBaseModel dataBaseModel, string name, bool isPrimaryKey)
    {
        this.name = name;
        this.isPrimaryKey = isPrimaryKey;
        dataBaseModel.fields.Add(this);
    }

    public abstract string GetAsString();
    public abstract object Get();
    public abstract void Set(object value);
}
