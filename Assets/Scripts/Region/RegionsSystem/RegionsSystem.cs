using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RegionsSystem : MonoBehaviour
{
    public GameObject regionPrefab;

    public UnityEngine.Color nextRegionColor;
    public float handleRadius = 0.5f;

    public List<RegionSerd> regionSerds;
}

[System.Serializable]
public class RegionSerd // region ser[ialized] d[ata]
{
    public Region region;
    public List<Vector3> points = new List<Vector3>();

    public RegionSerd(Region region)
    {
        this.region = region;
    }

    public void UpdateMesh()
    {
        region.UpdateMesh(points);
    }
}