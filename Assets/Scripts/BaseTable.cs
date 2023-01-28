using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTable<T> where T : BaseRaw
{
    public List<T> list;
    public int count
    {
        get { return list.Count; }
    }

    public BaseTable()
    {
        list = new List<T>();
    }

    public void add(T baseRaw)
    {
        list.Add(baseRaw);
    }

    //ƒобавл€ет в таблицу строку baseRaw и делает id такое же, как и у строки rid
    public void addwid(T baseRaw, BaseRaw rid)
    {
        list.Add(baseRaw);
        baseRaw.id = rid.id;
    }
    
    public T find(int id)
    {
        T baseRaw = null;
        for (int x = 0; x < list.Count; x++)
        {
            if (list[x].id == id)
            {
                baseRaw = list[x];
                break;
            }
        }

        return baseRaw;
    }

    //id - индкес элемента в массиве list
    public T get(int id)
    {
        return list[id];
    }
}
