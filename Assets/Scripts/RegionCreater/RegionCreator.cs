using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionCreator : MonoBehaviour
{
    [HideInInspector]
    public List<Shape> shapes = new List<Shape>();

    [HideInInspector]
    public bool showRegionsList;

    public float handleRadius = .5f;
}

[System.Serializable]
public class Shape
{
    public List<Vector3> points = new List<Vector3>();
}
