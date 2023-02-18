using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestSystem : MonoBehaviour
{
    public List<TestObject> testObjects = new List<TestObject>();
}

[System.Serializable]
public class TestObject
{
    public List<Vector3> points = new List<Vector3>();
}
