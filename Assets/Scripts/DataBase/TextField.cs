using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextField : DataBaseField
{
    public string value;

    public TextField(DataBaseModel dataBaseModel, string name, bool isPrimaryKey = false) : base("TEXT", dataBaseModel, name, isPrimaryKey)
    {

    }

    public TextField(DataBaseModel dataBaseModel, string name, string defaultValue, bool isPrimaryKey = false) : base("TEXT", dataBaseModel, name, isPrimaryKey)
    {
        value = defaultValue;
    }


    public override object Get()
    {
        return value;
    }

    public override void Set(object value)
    {
        this.value = (string)value;
    }

    public override string GetAsString()
    {
        return "\""+value+"\"";
    }
}
