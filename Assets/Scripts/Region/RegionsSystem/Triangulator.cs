using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    
    public static Vector2[] GetUv(Vector3[] vertices, out float aspect)
    {
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3 min = vertices[0];
        Vector3 max = vertices[0];

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x < min.x)
                min.x = vertices[i].x;

            if (vertices[i].y < min.y)
                min.y = vertices[i].y;

            if (vertices[i].x > max.x)
                max.x = vertices[i].x;

            if (vertices[i].y > max.y)
                max.y = vertices[i].y;
        }

        Vector2 size = new Vector2(max.x - min.x, max.y - min.y);

        for (int i = 0; i < vertices.Length; i++)
            uv[i] = (vertices[i] - min) / size;

        aspect = size.y / size.x;

        return uv;
    }

    // It will throw you an error if you try to pass points in a counterclock-wise order
    public static float Triangulate(List<Vector3> points, Mesh mesh)
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

            float aspect = 0;

            mesh.vertices = vertices;
            mesh.uv = GetUv(vertices, out aspect);
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            return aspect;
        }
        return 0;
    }
}
