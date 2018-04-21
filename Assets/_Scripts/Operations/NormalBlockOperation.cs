using UnityEngine;
using System.Collections;

public class NormalBlockOperation : Operation
{
	TerrainManager terrainManager;
	TerrainRaycast terrainRaycast;

	MLineRenderer lineRenderer;

    int placeBlockValue = 2;

	private void Awake ()
	{
		terrainManager = GetComponent<TerrainManager> ();
		terrainRaycast = GetComponent<TerrainRaycast> ();
		lineRenderer = Camera.main.GetComponent<MLineRenderer> ();
	}

	private void FixedUpdate ()
	{
		RaycastResult? result = terrainRaycast.LookingAt;

		lineRenderer.activated = result.HasValue;
		if (result.HasValue) {
			SetLocation (result.Value.Position);
		} else {
			return;
		}

		if (!isBreaking) {
			if (Input.GetKey (KeyCode.Mouse0)) {
				Point3 p = result.Value.Position;
				terrainManager.ChangeCell (p.X, p.Y, p.Z, 0);
				StartCoroutine (Delay (0.1f));
			} else if (Input.GetKey (KeyCode.Mouse1)) {
				Point3 p = result.Value.LastPosition;
                terrainManager.ChangeCell (p.X, p.Y, p.Z, placeBlockValue);
				StartCoroutine (Delay (0.1f));
			}
		}
	}

	void SetLocation (Point3 position)
	{
		cubicInstance.start = position.ToVec3 ();
		cubicInstance.end = cubicInstance.start + Vector3.one;
		lineRenderer.SetCubic (cubicInstance);
	}

	MLineRenderer.Cubic cubicInstance = new MLineRenderer.Cubic () { color = Color.black };

	public override void OnSetToCurrent()
	{
        Console.AssignCommand("setData", delegate (string[] args)
        {
            if (terrainRaycast.LookingAt.HasValue)
            {
                Point3 p = terrainRaycast.LookingAt.Value.Position;
                terrainManager.ChangeCell(p.X, p.Y, p.Z, BlockTerrain.ReplaceData(terrainManager.Terrain.GetCellValue(p.X, p.Y, p.Z), int.Parse(args[0])));
            }
        });
        Console.AssignCommand("setContent", delegate (string[] args)
        {
            placeBlockValue = int.Parse(args[0]);
        });
	}

	public override void OnRemoveFromCurrent()
	{
        lineRenderer.activated = false;
        if (WindowManager.instance != null)
        {
            Console.RemoveCommand("setData");
            Console.RemoveCommand("setContent");
        }
	}

	bool isBreaking;

	IEnumerator Delay (float seconds)
	{
		isBreaking = true;
		yield return new WaitForSeconds (seconds);
		isBreaking = false;
	}
}
