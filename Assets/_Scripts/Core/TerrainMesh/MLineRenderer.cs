using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLineRenderer : MonoBehaviour
{
	Cubic mainCube = new Cubic ();

	public float lineWidth = 5;
	public Color lineColor = Color.black;

	public bool activated;

	public Material lineMaterial;

	public static MLineRenderer main {
		get {
			return Camera.main.GetComponent<MLineRenderer> ();
		}
	}

	void CreateLineMaterial ()
	{
		if (!lineMaterial) {
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt ("_ZWrite", 0);
		}
	}

	void Awake ()
	{
		CreateLineMaterial ();
	}

	public void SetCubic (Cubic cubic)
	{
		mainCube = cubic;
	}

	// Will be called after all regular rendering is done
	//	public void OnRenderObject ()
	//	{
	//		if (activated) {
	//			CreateLineMaterial ();
	//			// Apply the line material
	//			lineMaterial.SetPass (0);
	//
	//			GL.PushMatrix ();
	//
	//			// Draw lines
	//			GL.Begin (GL.QUADS);
	//			GL.Color (Color.black);
	//			GL.Vertex (start);
	//			GL.Vertex3 (start.x, start.y, end.z);
	//			GL.Vertex3 (end.x, start.y, start.z);
	//			GL.Vertex3 (end.x, start.y, end.z);
	//			GL.Vertex3 (start.x, end.y, start.z);
	//			GL.Vertex3 (start.x, end.y, end.z);
	//			GL.Vertex3 (end.x, end.y, start.z);
	//			GL.Vertex3 (end.x, end.y, end.z);
	//
	//			GL.Vertex (start);
	//			GL.Vertex3 (start.x, end.y, start.z);
	//			GL.Vertex3 (start.x, end.y, end.z);
	//			GL.Vertex3 (start.x, start.y, end.z);
	//			GL.Vertex3 (end.x, end.y, start.z);
	//			GL.Vertex3 (end.x, start.y, start.z);
	//			GL.Vertex3 (end.x, end.y, end.z);
	//			GL.Vertex3 (end.x, start.y, end.z);
	//
	//			GL.Vertex (start);
	//			GL.Vertex3 (end.x, start.y, start.z);
	//			GL.Vertex3 (start.x, start.y, end.z);
	//			GL.Vertex3 (end.x, start.y, end.z);
	//			GL.Vertex3 (start.x, end.y, start.z);
	//			GL.Vertex3 (end.x, end.y, start.z);
	//			GL.Vertex3 (start.x, end.y, end.z);
	//			GL.Vertex3 (end.x, end.y, end.z);
	//
	//			GL.End ();
	//			GL.PopMatrix ();
	//		}
	//	}

	//void GenerateCuboid(Vector3 start, Vector3 end)
	//{
	//    Vector3 v000 = start;
	//    Vector3 v001 = new Vector3(start.x, start.y, end.z);
	//    Vector3 v010 = new Vector3(start.x, end.y, start.z);
	//    Vector3 v011 = new Vector3(start.x, end.y, end.z);
	//    Vector3 v100 = new Vector3(end.x, start.y, start.z);
	//    Vector3 v101 = new Vector3(end.x, start.y, end.z);
	//    Vector3 v110 = new Vector3(end.x, end.y, start.z);
	//    Vector3 v111 = end;

	//    GenerateFace(v001, v011, v010, v000);
	//    GenerateFace(v000, v100, v101, v001);
	//    GenerateFace(v000, v010, v110, v100);
	//    GenerateFace(v100, v110, v111, v101);
	//    GenerateFace(v111, v110, v010, v011);
	//    GenerateFace(v101, v111, v011, v001);
	//}

	//void GenerateFace(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
	//{
	//    //		Vector3 center = (v0 + v2) / 2;
	//    //		float scale = Vector3.Distance (v0, v1);
	//    //		scale = (scale - lineWidth) / scale;
	//    //		Vector3 sv0 = (v0 - center) * scale + center;
	//    //		Vector3 sv1 = (v1 - center) * scale + center;
	//    //		Vector3 sv2 = (v2 - center) * scale + center;
	//    //		Vector3 sv3 = (v3 - center) * scale + center;
	//    //
	//    //		GenerateQuard (v0, v1, sv1, sv0);
	//    //		GenerateQuard (v0, sv0, sv3, v3);
	//    //		GenerateQuard (v3, sv3, sv2, v2);
	//    //		GenerateQuard (v2, sv2, sv1, v1);

	//    GenerateQuard(v0, v1, v2, v3);
	//}

	//void GenerateQuard(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	//{
	//    vertices.Add(a);
	//    vertices.Add(b);
	//    vertices.Add(c);
	//    vertices.Add(d);
	//}

	void OnPostRender ()
	{
		//		if (!activated || verts == null)
		//			return;
		//
		//		lineMaterial.SetPass (0);
		//		GL.PushMatrix ();
		//
		//		GL.Color (lineColor);
		//
		//		GL.Begin (GL.TRIANGLES);
		////		for (int i = 0; i < verts.Length; i++) {
		////			GL.Vertex (verts [i]);
		////		}
		//		GL.Vertex (start);
		//		GL.Vertex3 (start.x, start.y, end.z);
		//		GL.Vertex (end);
		////		GL.Vertex3 (end.x, end.y, start.z);
		//
		//		GL.End ();
		//		GL.PopMatrix ();

		if (activated) {
			lineMaterial.SetPass (0);

			DrawCubic (mainCube);
		}
	}

	void DrawCubic (Cubic cubic)
	{
		DrawCubic (cubic.start, cubic.end, cubic.color);
	}

	void DrawCubic (Vector3 start, Vector3 end, Color color)
	{
		GL.Begin (GL.LINES);

		GL.Color (color);
		GL.Vertex (start);
		GL.Vertex3 (start.x, start.y, end.z);
		GL.Vertex3 (end.x, start.y, start.z);
		GL.Vertex3 (end.x, start.y, end.z);
		GL.Vertex3 (start.x, end.y, start.z);
		GL.Vertex3 (start.x, end.y, end.z);
		GL.Vertex3 (end.x, end.y, start.z);
		GL.Vertex3 (end.x, end.y, end.z);

		GL.Vertex (start);
		GL.Vertex3 (start.x, end.y, start.z);
		GL.Vertex3 (start.x, end.y, end.z);
		GL.Vertex3 (start.x, start.y, end.z);
		GL.Vertex3 (end.x, end.y, start.z);
		GL.Vertex3 (end.x, start.y, start.z);
		GL.Vertex3 (end.x, end.y, end.z);
		GL.Vertex3 (end.x, start.y, end.z);

		GL.Vertex (start);
		GL.Vertex3 (end.x, start.y, start.z);
		GL.Vertex3 (start.x, start.y, end.z);
		GL.Vertex3 (end.x, start.y, end.z);
		GL.Vertex3 (start.x, end.y, start.z);
		GL.Vertex3 (end.x, end.y, start.z);
		GL.Vertex3 (start.x, end.y, end.z);
		GL.Vertex3 (end.x, end.y, end.z);

		GL.End ();
	}

	public struct Cubic
	{
		public Vector3 start;
		public Vector3 end;
		public Color color;
	}
}
