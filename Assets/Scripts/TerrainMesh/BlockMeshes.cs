using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshes : MonoBehaviour
{
	public Mesh stair0;
	public Mesh stair1;
	public Mesh stair2;

	public static Mesh[] stairs = new Mesh[24];

	public Mesh slab;
	public static Mesh[] slabs = new Mesh[2];

	void Awake ()
	{
		float y;
		Matrix4x4 m;
		Mesh mesh;
		for (int i = 0; i < 24; i++) {
			y = 0;

			int rotation = FurnitureManager.GetRotation (i);
			y -= rotation * 90;

			m = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0, y, 0), Vector3.one);
			switch ((i >> 3) & 3) {
			case 1:
				mesh = stair0;
				break;
			case 0:
				mesh = stair1;
				break;
			case 2:
				mesh = stair2;
				break;
			default:
				throw new UnityException ("unknown stair module: " + ((i >> 3) & 3));
			}

			stairs [i] = TranslateMesh (mesh, m, (i & 4) != 0);
		}

		slabs [0] = slab;
		slabs [1] = UpsideDownMesh (slab);
	}

	static Vector3 half = new Vector3 (0.5f, 0.5f, 0.5f);

	public static Mesh TranslateMesh (Mesh mesh, Matrix4x4 matrix, bool upsidedown)
	{
		Mesh m = new Mesh ();
		Vector3[] vec = mesh.vertices;
		for (int i = 0; i < mesh.vertexCount; i++) {
			Vector3 v = vec [i];
			v -= half;
			v = matrix.MultiplyPoint (v);
			v += half;
			if (upsidedown) {
				v.y = 1 - v.y;
			}
			vec [i] = v;
		}
		int[] triangles = mesh.triangles;
		if (upsidedown) {
			for (int i = 0; i < triangles.Length; i += 3) {
				int tmp = triangles [i];
				triangles [i] = triangles [i + 1];
				triangles [i + 1] = tmp;
			}
		}
		m.vertices = vec;
		m.triangles = triangles;
		m.uv = mesh.uv;
		return m;
	}

	public static Mesh UpsideDownMesh (Mesh mesh)
	{
		Mesh m = new Mesh ();
		Vector3[] vec = mesh.vertices;
		int[] triangles = mesh.triangles;

		for (int i = 0; i < mesh.vertexCount; i++) {
			Vector3 v = vec [i];
			v.y = 1 - v.y;
			vec [i] = v;
		}
		for (int i = 0; i < triangles.Length; i += 3) {
			int tmp = triangles [i];
			triangles [i] = triangles [i + 1];
			triangles [i + 1] = tmp;
		}

		m.vertices = vec;
		m.triangles = triangles;
		m.uv = mesh.uv;
		return m;
	}
}
