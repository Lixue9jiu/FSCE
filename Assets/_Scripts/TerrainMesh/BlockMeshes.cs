using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshes : MonoBehaviour
{
	public Mesh stair0;
	public Mesh stair1;
	public Mesh stair2;

	public Mesh slab;

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

	public static Mesh TranslateMeshRaw (Mesh mesh, Matrix4x4 matrix)
	{
		Mesh m = new Mesh ();
		Vector3[] vec = mesh.vertices;
		for (int i = 0; i < mesh.vertexCount; i++) {
			Vector3 v = vec [i];
			v -= half;
			v = matrix.MultiplyPoint (v);
			v += half;
			vec [i] = v;
		}
		m.vertices = vec;
		m.triangles = mesh.triangles;
		m.uv = mesh.uv;
		m.colors = mesh.colors;
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
