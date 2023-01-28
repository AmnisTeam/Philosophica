using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RegionCreator : MonoBehaviour
{
    public GameObject regionPrefab;
    //[HideInInspector]
    public List<Shape> shapes = new List<Shape>();
    public List<GameObject> regions = new List<GameObject>();

    [HideInInspector]
    public bool showRegionsList;

    public float handleRadius = .5f;

    public Color nextRegionColor;

    public void UpdateMeshDisplay()
    {
        for (int i = 0; i < shapes.Count; i++)
            shapes[i].UpdateMesh();
    }
}

[System.Serializable]
public class Shape
{
    public GameObject region;
    public int regionId;
    public List<Vector3> points = new List<Vector3>();

    [HideInInspector]
    public bool needDestroyRegion = true;

    public Shape(GameObject region, int regionId)
    {
        this.region = region;
        this.regionId = regionId;
    }
    public void UpdateMesh()
    {
        //region.GetComponent<Renderer>().material.SetColor("_RegionColor", region.GetComponent<Region>().regionColor);
        region.GetComponent<Region>().UpdateMesh(points);
    }
}