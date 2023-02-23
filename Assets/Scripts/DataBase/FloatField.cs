using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatField : DataBaseField
{
    public float value;

    public FloatField(DataBaseModel dataBaseModel, string name, bool isPrimaryKey = false) : base("REAL", dataBaseModel, name, isPrimaryKey)
    {

    }

    public FloatField(DataBaseModel dataBaseModel, string name, float defaultValue, bool isPrimaryKey = false) : base("REAL", dataBaseModel, name, isPrimaryKey)
    {
        value = defaultValue;
    }

    public override object Get()
    {
        return value;
    }

    public override string GetAsString()
    {
        return value.ToString();
    }

    public override void Set(object value)
    {
        this.value = (float)value;
    }
}
