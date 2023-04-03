using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartOfObject
{
    public int minCount;
    public int maxCount;
    public float minForce;
    public float maxForce;
    public GameObject prifab;
}

public class DestroyOnParts : DestoryObject
{
    public PartOfObject[] partPrifabs;
    public float radiusDestroy = 1;

    public override void ToDestroy()
    {
        System.Random random = new System.Random();
        for(int x = 0; x < partPrifabs.Length; x++)
        {
            int count = random.Next(partPrifabs[x].minCount, partPrifabs[x].maxCount);
            for(int y = 0; y < count; y++)
            {
                float force = (float)(partPrifabs[x].minForce + (partPrifabs[x].maxForce - partPrifabs[x].minForce) * random.NextDouble());

                double angle = random.NextDouble() * 2 * Math.PI;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector3 localPosition = direction * (float)(random.NextDouble() + 1) / 2.0f * radiusDestroy;

                GameObject part = Instantiate(partPrifabs[x].prifab, transform.position + localPosition, Quaternion.Euler(0, 0, (float)(random.NextDouble() * 2 * Math.PI)));
                if(part.GetComponent<Rigidbody2D>())
                    part.GetComponent<Rigidbody2D>().velocity = direction * force;
            }
        }

        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
