using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData
{

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;
    public Color[] colors;

    public static MeshData CreateEmpty()
    {
        return new MeshData
        {
            vertices = new Vector3[0],
            triangles = new int[0],
            uv = new Vector2[0],
            colors = new Color[0],
        };
    }

    public MeshData(Mesh m)
    {
        vertices = m.vertices;
        triangles = m.triangles;
        uv = m.uv;
        colors = m.colors;
    }

    public void ToMesh(Mesh mesh)
    {
        //if (vertices.Length > 65534)
        //Debug.LogError("vertices excessed 65534 limit");

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void Append(MeshData data)
    {
        int vCount = vertices.Length;
        vertices = MergeArray(vertices, data.vertices);

        int[] tri = new int[triangles.Length + data.triangles.Length];
        int start = triangles.Length;
        int count = data.triangles.Length;
        System.Array.Copy(triangles, tri, triangles.Length);
        for (int i = 0; i < count; i++)
        {
            tri[start + i] = data.triangles[i] + vCount;
        }
        triangles = tri;

        uv = MergeArray(uv, data.uv);
        colors = MergeArray(colors, data.colors);
    }

    public void FlipWindingOrder()
    {
        Debug.Log("filping winding order");
        int[] tri = new int[triangles.Length];
        for (int i = 0; i < tri.Length; i += 3)
        {
            tri[i] = triangles[i];
            tri[i + 1] = triangles[i + 2];
            tri[i + 2] = triangles[i + 1];
        }
        triangles = tri;
    }

    public MeshData Clone()
    {
        return new MeshData
        {
            vertices = CloneArray(vertices),
            triangles = CloneArray(triangles),
            uv = CloneArray(uv),
            colors = CloneArray(colors)
        };
    }

    public static T[] CloneArray<T>(T[] src)
    {
        T[] dst = new T[src.Length];
        System.Array.Copy(src, dst, src.Length);
        return dst;
    }

    public static T[] MergeArray<T>(T[] a1, T[] a2)
    {
        T[] dst = new T[a1.Length + a2.Length];
        System.Array.Copy(a1, 0, dst, 0, a1.Length);
        System.Array.Copy(a2, 0, dst, a1.Length, a2.Length);
        return dst;
    }

    public void Transform(Matrix4x4 matrix)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
        }
    }

    public void WrapInTextureSlot(int texSlot)
    {
        Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4)) / 16f);

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += uvPos;
        }
    }

    public void WrapInTextureSlotTerrain(int texSlot)
    {
        Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4) + 1) / 16f);

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = uvPos;
        }
    }

    public static void Transform(MeshData m, Matrix4x4 matrix)
    {
        for (int i = 0; i < m.vertices.Length; i++)
        {
            m.vertices[i] = matrix.MultiplyPoint3x4(m.vertices[i]);
        }
    }

    public void FlipVertical()
    {
        Vector3[] vec = vertices;
        int[] tri = triangles;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vec[i];
            v.y = 1 - v.y;
            vec[i] = v;
        }
        for (int i = 0; i < tri.Length; i += 3)
        {
            int tmp = tri[i];
            tri[i] = tri[i + 1];
            tri[i + 1] = tmp;
        }
    }
}
