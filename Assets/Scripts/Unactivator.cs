using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unactivator : MonoBehaviour
{
    public bool unactive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unactive)
            gameObject.SetActive(false);
    }
}
