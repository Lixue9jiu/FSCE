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

    public MeshData Clone()
    {
        return new MeshData
        {
            vertices = CopyArray(vertices),
            triangles = CopyArray(triangles),
            uv = CopyArray(uv),
            colors = CopyArray(colors)
        };
    }

    public static T[] CopyArray<T>(T[] src)
    {
        T[] dst = new T[src.Length];
        System.Array.Copy(src, dst, src.Length);
        return dst;
    }

    public MeshData Transform(Matrix4x4 matrix)
    {
        MeshData m = Clone();
        for (int i = 0; i < m.vertices.Length; i++)
        {
            m.vertices[i] = matrix.MultiplyPoint3x4(m.vertices[i]);
        }
        return m;
    }

	public static void Transform(MeshData m, Matrix4x4 matrix)
	{
		for (int i = 0; i < m.vertices.Length; i++)
        {
            m.vertices[i] = matrix.MultiplyPoint3x4(m.vertices[i]);
        }
	}
}
