using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[System.Serializable]
public class Region : MonoBehaviour
{

    public void SetColor(UnityEngine.Color color)
    {
        if (gameObject.GetComponent<Renderer>().sharedMaterial != null)
        {
            Material material = gameObject.GetComponent<Renderer>().materials[0];
            material.SetColor("_RegionColor", color);
        }
        else
            Debug.LogError("Region instance has no material component with RegionShader shader", this);
    }

    public UnityEngine.Color GetColor(UnityEngine.Color color)
    {
        if (GetComponent<Renderer>().materials[0] != null)
        {
            Material material = GetComponent<Renderer>().materials[0];
            return material.GetColor("_RegionColor");
        }
        else
            Debug.LogError("Region instance has no material component with RegionShader shader", this);
        return new UnityEngine.Color(255, 0, 255);
    }

    public void UpdateMesh(List<Vector3> points)
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
            GetComponent<MeshFilter>().sharedMesh = new Mesh();

        Triangulator.Triangulate(points, GetComponent<MeshFilter>().sharedMesh);
    }
}
