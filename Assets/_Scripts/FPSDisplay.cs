using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{

	float deltaTime = 0.0f;

	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 9 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 3 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)\ncurrent chunk: {2}\n按ESC键暂停", msec, fps, TerrainManager.CurrentChunk ());
		TerrainRaycast.RaycastResult? r = GetComponent<TerrainRaycast> ().LookingAt;
		if (r.HasValue) {
            //text += string.Format ("\nlooking at {0}", BlocksData.GetBlock(BlockTerrain.GetContent(r.Value.BlockValue)).Name);
            text += string.Format("\nlooking at {0}", BlockTerrain.GetContent(r.Value.BlockValue));
		}
		GUI.Label (rect, text, style);
	}
}