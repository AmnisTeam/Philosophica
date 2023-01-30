using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionsSystem : MonoBehaviour
{
    public GameObject regionPrefab;
    public List<RegionSerd> regionSerds;

    //public int regionToDestroyId = -1;

    //public int regionIdForPointToDestroy = -1;
    //public int pointToDestroyId = -1;
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