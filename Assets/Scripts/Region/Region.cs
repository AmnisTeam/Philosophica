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
    public Color regionColor;
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] indices;

    public static float Cross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static T GetItem<T>(T[] array, int index)
    {
        if (index >= array.Length)
            return array[index % array.Length];
        else if (index < 0)
            return array[index % array.Length + array.Length];
        else
            return array[index];
    }

    public static T GetItem<T>(List<T> list, int index)
    {
        if (index >= list.Count)
            return list[index % list.Count];
        else if (index < 0)
            return list[index % list.Count + list.Count];
        else
            return list[index];
    }

    public static bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 bc = c - b;
        Vector3 ca = a - c;

        Vector3 ap = p - a;
        Vector3 bp = p - b;
        Vector3 cp = p - c;

        float cross1 = Cross(ab, ap);
        float cross2 = Cross(bc, bp);
        float cross3 = Cross(ca, cp);

        return cross1 <= 0 && cross2 <= 0 && cross3 <= 0;
    }

    void Triangulate(List<Vector3> points)
    {
        vertices = new Vector3[points.Count];
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = points[i];

        indices = new int[(vertices.Length - 2) * 3];
        int currentIndicesCount = 0;

        List<int> indexList = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
            indexList.Add(i);

        while (indexList.Count > 3)
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                int a = indexList[i];
                int b = GetItem<int>(indexList, i - 1);
                int c = indexList[i + 1];

                Vector3 va = vertices[a];
                Vector3 vb = vertices[b];
                Vector3 vc = vertices[c];

                Vector3 vab = vb - va;
                Vector3 vac = vc - va;

                if (Cross(vab, vac) < 0)
                    continue;

                bool isEar = true;

                for (int j = 0; j < vertices.Length; j++)
                {
                    if (j == a || j == b || j == c)
                        continue;

                    Vector3 p = vertices[j];
                    if (IsPointInTriangle(p, vb, va, vc))
                    {
                        isEar = false;
                        break;
                    }
                }

                if(isEar)
                {
                    indices[currentIndicesCount++] = b;
                    indices[currentIndicesCount++] = a;
                    indices[currentIndicesCount++] = c;
                    indexList.RemoveAt(i);
                    break;
                }
            }
        }

        indices[currentIndicesCount++] = indexList[0];
        indices[currentIndicesCount++] = indexList[1];
        indices[currentIndicesCount++] = indexList[2];
    }

    void CreateShapeOld(List<Vector3> points)
    {
        Vector3 midPoint = new Vector3(0, 0, 0);
        for (int i = 0; i < points.Count; i++)
            midPoint += points[i];
        midPoint /= (float)points.Count;


        vertices = new Vector3[points.Count + 1];
        for(int i = 0; i < vertices.Length; i++)
        {
            if (i < points.Count)
                vertices[i] = points[i];
            else
                vertices[i] = midPoint;
        }


        int indicesCount = points.Count * 3;
        indices = new int[indicesCount];

        int nTriangles = indices.Length / 3;
        for (int i = 0; i < nTriangles; i++)
        {
            int iTimes3 = i * 3;
            if (i < nTriangles - 1)
            {
                indices[iTimes3] = i;
                indices[iTimes3 + 1] = i + 1;
                indices[iTimes3 + 2] = vertices.Length - 1;
            }
            else
            {
                indices[iTimes3] = i;
                indices[iTimes3 + 1] = 0;
                indices[iTimes3 + 2] = vertices.Length - 1;
            }
        }
    }

    void CreateShape(List<Vector3> points)
    {
        Triangulate(points);

        Color shadowColor = regionColor * 0.7f;
    }

    public void UpdateMesh(List<Vector3> points)
    {
        if (mesh == null)
            mesh = new Mesh();
        mesh.Clear();

        if (points.Count >= 3)
        {
            CreateShape(points);

            mesh.vertices = vertices;
            mesh.triangles = indices;

            mesh.RecalculateNormals();
        }

        GetComponent<MeshFilter>().mesh = mesh;
    }
}