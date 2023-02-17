using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulator : MonoBehaviour
{
    private static float Cross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private static T GetItem<T>(T[] array, int index)
    {
        if (index >= array.Length)
            return array[index % array.Length];
        else if (index < 0)
            return array[index % array.Length + array.Length];
        else
            return array[index];
    }

    private static T GetItem<T>(List<T> list, int index)
    {
        if (index >= list.Count)
            return list[index % list.Count];
        else if (index < 0)
            return list[index % list.Count + list.Count];
        else
            return list[index];
    }

    private static bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
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

    
    // It will throw you an error if you try to pass points in a counterclock-wise order
    public static void Triangulate(List<Vector3> points, Mesh mesh)
    {
        mesh.Clear();
        if (points.Count >= 3)
        {
            Vector3[] vertices = new Vector3[points.Count];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = points[i];

            int[] indices = new int[(vertices.Length - 2) * 3];
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

                    if (isEar)
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

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
        }
    }
}
