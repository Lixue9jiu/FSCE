using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRaycast : MonoBehaviour
{
    public GameObject Cube;

    public MLineRenderer line;
    BlockTerrain terrain;
    TerrainManager terrainManager;

    public RaycastResult? LookingAt;

    void Awake()
    {
        terrain = GetComponent<BlockTerrain>();
        terrainManager = GetComponent<TerrainManager>();
    }

    //	void Update ()
    //	{
    //	}

    void FixedUpdate()
    {
        LookingAt = AlaphaRaycast(Camera.main.transform.position, Camera.main.transform.forward);
        //Cube.SetActive(LookingAt.HasValue);
        line.activated = LookingAt.HasValue;
        if (LookingAt.HasValue)
        {
            line.SetLocation (LookingAt.Value.Position);
            //Cube.transform.position = LookingAt.Value.Position.ToVec3();
        }

        if (!isBreaking && LookingAt.HasValue)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Point3 p = LookingAt.Value.Position;
                terrainManager.ChangeCell(p.X, p.Y, p.Z, 0);
                StartCoroutine(Delay(0.5f));
            }
            else if (Input.GetKey(KeyCode.Mouse1))
            {
                Point3 p = LookingAt.Value.LastPosition;
                terrainManager.ChangeCell(p.X, p.Y, p.Z, 2);
                StartCoroutine(Delay(0.5f));
            }
        }
    }

    bool isBreaking;

    IEnumerator Delay(float seconds)
    {
        isBreaking = true;
        yield return new WaitForSeconds(seconds);
        isBreaking = false;
    }

    public RaycastResult? AlaphaRaycast(Vector3 position, Vector3 direction, float distance = 20)
    {
        Vector3 increase = Vector3.Normalize(direction) * 0.05f;

        Point3 last = new Point3();
        Point3 result;

        int count = (int)(distance / 0.05f);
        for (int i = 0; i < count; i++)
        {
            result = new Point3(ToCell(position.x), ToCell(position.y), ToCell(position.z));
            if (!result.Equals(last))
            {
                int value = terrain.GetCellValue(result.X, result.Y, result.Z);
                if (BlockTerrain.GetContent(value) != 0)
                {
                    return new RaycastResult
                    {
                        Position = result,
                        LastPosition = last,
                        BlockValue = value
                    };
                }
                last = result;
            }
            position += increase;
        }

        return null;
    }

    //	public RaycastResult? Raycast(Vector3 position, Vector3 direction, int distance = 20)
    //	{
    //		int xoff = direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0);
    //		int yoff = direction.y > 0 ? 1 : (direction.y < 0 ? -1 : 0);
    //		int zoff = direction.z > 0 ? 1 : (direction.z < 0 ? -1 : 0);
    //
    //		float startx = xoff > 0 ? Mathf.Floor(position.x) : Mathf.Ceil(position.x);
    //		float starty = yoff > 0 ? Mathf.Floor(position.y) : Mathf.Ceil(position.y);
    //		float startz = zoff > 0 ? Mathf.Floor(position.z) : Mathf.Ceil(position.z);
    //		float ydx = (direction.y / direction.x) * xoff;
    //		float zdx = (direction.z / direction.x) * xoff;
    //		float zdy = (direction.z / direction.y) * yoff;
    //
    //		int yperx = Mathf.Floor (ydx);
    //		int zpery = Mathf.Floor (zdy);
    //		int zperx = Mathf.Floor (zdx);
    //
    ////		Debug.Log (string.Format ("{0}, {1}, {2}, {3}, {4}, {5}", xdy, xdz, ydx, ydz, zdx, zdy));
    //
    //		int i = 0;
    //		int value;
    //		while (true) {
    //			for (int iy = 0; iy < yperx; iy++) {
    //				int tmpz = startz;
    //
    //				int x2 = startx;
    //				int y2 = starty;
    //				int z2 = startz;
    //				for (int iz = 0; iz < zpery; iz++) {
    //					
    //
    //					if (i >= distance)
    //						return null;
    //					i++;
    //				}
    //
    //
    //				if (i >= distance)
    //					return null;
    //				i++;
    //			}
    //
    //			value = terrain.GetCellValue (startx, starty, startz);
    //			if (value != 0) {
    //				return new RaycastResult {
    //					BlockValue = value,
    //					Position = new Point3(startx, starty, startz)
    //				};
    //			}
    //			startx += xoff;
    //			starty += ydx;
    //			startz += zdx;
    //
    //			if (i >= distance)
    //				return null;
    //			i++;
    //		}
    //
    //		return null;
    //	}

    public static int ToCell(float f)
    {
        return Mathf.FloorToInt(f);
    }

    public static Point3 ToCell(Vector3 vec)
    {
        return new Point3(ToCell(vec.x), ToCell(vec.y), ToCell(vec.z));
    }

    public struct RaycastResult
    {
        public Point3 Position;
        public int BlockValue;

        public Point3 LastPosition;
    }
}
