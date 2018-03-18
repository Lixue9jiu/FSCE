using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData {

	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uv;
	public Color[] colors;

	public MeshData (Mesh m)
	{
		vertices = m.vertices;
		triangles = m.triangles;
		uv = m.uv;
		colors = m.colors;
	}

	public void ToMesh (Mesh mesh)
	{
        if (vertices.Length > 65534)
            Debug.LogError("vertices excessed 65534 limit");

		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.RecalculateNormals ();
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
}
