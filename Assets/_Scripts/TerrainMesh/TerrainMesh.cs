using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh {

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
	List<Color> colors = new List<Color>();



	public void PushToMesh(out MeshData data)
	{
		data = new MeshData {
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			uv = uvs.ToArray(),
			colors = colors.ToArray()
		};

		vertices.Clear ();
		triangles.Clear ();
		uvs.Clear ();
		colors.Clear ();
	}

	private void VerticesFromRect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		int count = vertices.Count;
		vertices.Add(a);
		vertices.Add(b);
		vertices.Add(c);
		vertices.Add(d);

		triangles.Add(count);
		triangles.Add(count + 1);
		triangles.Add(count + 2);
		triangles.Add(count + 2);
		triangles.Add(count + 3);
		triangles.Add(count);
	}
}

public struct CellFace
{
    public const int TOP = 0;
    public const int FRONT = 1;
    public const int RIGHT = 2;
    public const int BOTTOM = 3;
    public const int BACK = 4;
    public const int LEFT = 5;

    public int Face;
    public int x;
    public int y;
    public int z;


}