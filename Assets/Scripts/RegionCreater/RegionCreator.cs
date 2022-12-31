using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionCreator : MonoBehaviour
{
    public GameObject regionPrefab;
    [HideInInspector]
    public List<GameObject> shapes = new List<GameObject>();

    [HideInInspector]
    public bool showRegionsList;

    public float handleRadius = .5f;

    public void UpdateMeshDisplay()
    {
        for (int i = 0; i < shapes.Count; i++)
            shapes[i].GetComponent<Region>().UpdateMesh();
    }
}

[System.Serializable]
public class Shape
{
    public List<Vector3> points = new List<Vector3>();
}
