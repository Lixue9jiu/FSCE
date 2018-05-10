using UnityEngine;
using System.Collections.Generic;

public class ShaderTest : MonoBehaviour
{
	private void Start()
	{
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 3), new Vector3(3, 0, 3), new Vector3(3, 0, 0) };
        mesh.triangles = new int[] { 0, 1, 3, 1, 2, 3 };
		//mesh.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, -0.1875f), new Vector2(0.1875f, -0.1875f), new Vector2(0.1875f, 0f) };
		//mesh.uv = new Vector2[] { new Vector2(0.9375f, 0.0625f), new Vector2(0.9375f, 0.0625f), new Vector2(0.9375f, 0.0625f), new Vector2(0.9375f, 0.0625f) };
		Vector2 uvPos = new Vector2(0, -0.0625f);
		mesh.uv = new Vector2[] { uvPos, uvPos, uvPos, uvPos };
        mesh.colors = new Color[] { Color.white, Color.white, Color.white, Color.white };

		mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
	}
}
