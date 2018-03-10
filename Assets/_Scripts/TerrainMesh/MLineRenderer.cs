using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLineRenderer : MonoBehaviour
{
	// When added to an object, draws colored rays from the
	// transform position.
	Vector3 start;
	Vector3 end;

	Vector3[] linePoints = new Vector3[24];

	public float lineWidth = 5;
	public Color lineColor = Color.black;

	Camera cam;

	public bool activated;

	static Material lineMaterial;

	static void CreateLineMaterial ()
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
		cam = GetComponent<Camera> ();
		CreateLineMaterial ();
	}

	public void SetLocation (Point3 p)
	{
//		start = p.ToVec3 ();
//		end = start + Vector3.one;
		Vector3 start = p.ToVec3 ();
		Vector3 end = start + Vector3.one;
		linePoints [0] = start;
		linePoints [1] = new Vector3 (start.x, start.y, end.z);
		linePoints [2] = new Vector3 (end.x, start.y, start.z);
		linePoints [3] = new Vector3 (end.x, start.y, end.z);
		linePoints [4] = new Vector3 (start.x, end.y, start.z);
		linePoints [5] = new Vector3 (start.x, end.y, end.z);
		linePoints [6] = new Vector3 (end.x, end.y, start.z);
		linePoints [7] = new Vector3 (end.x, end.y, end.z);

		linePoints [8] = start;
		linePoints [9] = new Vector3 (start.x, end.y, start.z);
		linePoints [10] = new Vector3 (start.x, end.y, end.z);
		linePoints [11] = new Vector3 (start.x, start.y, end.z);
		linePoints [12] = new Vector3 (end.x, end.y, start.z);
		linePoints [13] = new Vector3 (end.x, start.y, start.z);
		linePoints [14] = new Vector3 (end.x, end.y, end.z);
		linePoints [15] = new Vector3 (end.x, start.y, end.z);

		linePoints [16] = start;
		linePoints [17] = new Vector3 (end.x, start.y, start.z);
		linePoints [18] = new Vector3 (start.x, start.y, end.z);
		linePoints [19] = new Vector3 (end.x, start.y, end.z);
		linePoints [20] = new Vector3 (start.x, end.y, start.z);
		linePoints [21] = new Vector3 (end.x, end.y, start.z);
		linePoints [22] = new Vector3 (start.x, end.y, end.z);
		linePoints [23] = new Vector3 (end.x, end.y, end.z);
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

	void OnPostRender ()
	{
		if (!activated)
			return;

		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = linePoints.Length - 1;
		float thisWidth = 1f / Screen.width * lineWidth * 0.5f;

		lineMaterial.SetPass (0);
		GL.Color (lineColor);

		if (lineWidth == 1) {
			GL.Begin (GL.LINES);
			for (int i = 0; i < end; ++i) {
				GL.Vertex (cam.ViewportToWorldPoint (new Vector3 (linePoints [i].x, linePoints [i].y, nearClip)));
				GL.Vertex (cam.ViewportToWorldPoint (new Vector3 (linePoints [i + 1].x, linePoints [i + 1].y, nearClip)));
			}
		} else {
			GL.Begin (GL.QUADS);
			for (int i = 0; i < end; ++i) {
				Vector3 perpendicular = (new Vector3 (linePoints [i + 1].y, linePoints [i].x, nearClip) -
				                        new Vector3 (linePoints [i].y, linePoints [i + 1].x, nearClip)).normalized * thisWidth;
				Vector3 v1 = new Vector3 (linePoints [i].x, linePoints [i].y, nearClip);
				Vector3 v2 = new Vector3 (linePoints [i + 1].x, linePoints [i + 1].y, nearClip);
				GL.Vertex (cam.ViewportToWorldPoint (v1 - perpendicular));
				GL.Vertex (cam.ViewportToWorldPoint (v1 + perpendicular));
				GL.Vertex (cam.ViewportToWorldPoint (v2 + perpendicular));
				GL.Vertex (cam.ViewportToWorldPoint (v2 - perpendicular));
			}
		}
		GL.End ();
	}
}
