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

}
