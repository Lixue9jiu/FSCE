using UnityEngine;

public class FPSDisplay : MonoBehaviour
{

    bool showing = true;
	float deltaTime = 0.0f;

	void Update()
	{
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showing = !showing;
        }
        if (!showing)
            return;

		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
        if (showing)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 9 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 3 / 100;
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)\ncurrent chunk: {2}\n按ESC键暂停\n按P键截图", msec, fps, TerrainManager.CurrentChunk());
            RaycastResult? r = GetComponent<TerrainRaycast>().LookingAt;
            if (r.HasValue)
            {
                //text += string.Format ("\nlooking at {0}", BlocksData.GetBlock(BlockTerrain.GetContent(r.Value.BlockValue)).Name);
				text += string.Format("\nlooking at {0}", BlocksData.Blocks[BlockTerrain.GetContent(r.Value.BlockValue)].ToString(r.Value.BlockValue));
            }
            GUI.Label(rect, text, style);
        }
	}
}
