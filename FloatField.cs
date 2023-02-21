using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatField : DataBaseField
{
    public float defaultValue = 0;
    public float value;

    public FloatField(DataBaseModel dataBaseModel, string name) : base("REAL", dataBaseModel, name)
    {

    }

    public FloatField(DataBaseModel dataBaseModel, string name, float defaultValue) : base("REAL", dataBaseModel, name)
    {
        this.defaultValue = defaultValue;
    }

    public override string GetDefault()
    {
        return defaultValue.ToString();
    }

    public override object Get()
    {
        return value;
    }

    public override void Set(object value)
    {
        this.value = (float)value;
    }
}
