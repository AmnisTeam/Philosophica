using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerField : DataBaseField
{
    public int value;

    public IntegerField(DataBaseModel dataBaseModel, string name, bool isPrimaryKey = false) : base("INTEGER", dataBaseModel, name, isPrimaryKey)
    {

    }

    public IntegerField(DataBaseModel dataBaseModel, string name, int defaultValue, bool isPrimaryKey = false) : base("INTEGER", dataBaseModel, name, isPrimaryKey)
    {
        value = defaultValue;
    }

    public override object Get()
    {
        return value;
    }

    public override void Set(object value)
    {
        if(value.GetType() == typeof(long))
            this.value = (int)((long)value);
        if (value.GetType() == typeof(int))
            this.value = (int)value;
    }

    public override string GetAsString()
    {
        return value.ToString();
    }
}
