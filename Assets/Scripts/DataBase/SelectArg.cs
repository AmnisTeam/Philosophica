using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SelectArg
{
    public string name;
    public string value;

    public SelectArg(string name, object value)
    {
        this.name = name;
        if (value.GetType() == typeof(int))
            this.value = ((int)value).ToString();
        if (value.GetType() == typeof(float))
            this.value = ((float)value).ToString();
        if (value.GetType() == typeof(string))
            this.value = (string)value;
    }
}