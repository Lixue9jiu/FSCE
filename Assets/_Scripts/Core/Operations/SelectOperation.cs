using UnityEngine;
using System.Collections;

public class SelectOperation : Operation
{
    //readonly SelectMethod[] selectMethods = { "select_state_two_points", "select_state_addition", "select_state_deletion" };
    SelectMethod[] selectMethods;

    TerrainManager terrainManager;
    TerrainRaycast terrainRaycast;

    TerrainOperations terrainOperations;

    MLineRenderer lineRenderer;

    Point3 minCorner;
    Point3 maxCorner;
    bool isSelecting;

    Point3 tempMin;
    Point3 tempMax;

    MLineRenderer.Cubic cubeInstance = new MLineRenderer.Cubic() { color = Color.blue };

    int selectingMode;

    System.Action selectAction;

    private void Awake()
    {
        terrainManager = GetComponent<TerrainManager>();
        terrainRaycast = GetComponent<TerrainRaycast>();
        lineRenderer = MLineRenderer.main;

        terrainOperations = new TerrainOperations(terrainManager);

        selectMethods = new SelectMethod[]
        {
            new SelectMethod
            {
                Name = "select_state_two_points",
                Action = SelectBy2Points
            },
            new SelectMethod
            {
                Name = "select_state_addition",
                Action = SelectByAddition
            }
        };
        selectAction = selectMethods[0].Action;
    }

	public override void OnSetToCurrent()
	{
        Console.AssignCommand("fill", (args) =>
        {
            terrainOperations.FillWith(minCorner.X, minCorner.Y, minCorner.Z, maxCorner.X, maxCorner.Y, maxCorner.Z, int.Parse(args[0]));
        });
	}

	public override void OnRemoveFromCurrent()
	{
        lineRenderer.activated = false;
        if (WindowManager.instance != null)
        {
            Console.RemoveCommand("fill");
        }
	}

    private void FixedUpdate()
    {
        selectAction();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ApplyTemp();
            isSelecting = true;
            selectCount++;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isSelecting = false;
            selectCount = 0;
            lineRenderer.activated = false;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectingMode = (selectingMode + 1) % selectMethods.Length;
            MessageManager.ShowStrRes(selectMethods[selectingMode].Name);
            selectAction = selectMethods[selectingMode].Action;
        }
    }

    void AddToTempSelection(Point3 p)
    {
        tempMin = Point3.Min(minCorner, p);
        tempMax = Point3.Max(maxCorner, p + Point3.one);
        UpdateLineRenderer();
    }

    void DeleteFromTempSelection(Point3 p)
    {
        if (p.X < tempMin.X || p.X > tempMax.X || p.Y < tempMin.Y || p.Y > tempMax.Y || p.Z < tempMin.Z || p.Z > tempMax.Z)
        {
            tempMin = minCorner;
            tempMax = maxCorner;
        }
        else
        {
            tempMin = Point3.Max(minCorner, p);
            tempMax = Point3.Min(maxCorner, p + Point3.one);
        }

        UpdateLineRenderer();
    }

    void ApplyTemp()
    {
        minCorner = tempMin;
        maxCorner = tempMax;
    }

    void UpdateLineRenderer()
    {
        cubeInstance.start = tempMin.ToVec3();
        cubeInstance.end = tempMax.ToVec3();
        lineRenderer.SetCubic(cubeInstance);
    }

    int selectCount;
    float distance;

    void SelectBy2Points()
    {
        if (selectCount > 1)
            return;

        if (terrainRaycast.LookingAt.HasValue && !isSelecting)
        {
            lineRenderer.activated = true;
            tempMin = terrainRaycast.LookingAt.Value.Position;
            tempMax = tempMin + Point3.one;
            distance = terrainRaycast.LookingAt.Value.Distance;

            UpdateLineRenderer();
        }
        else if (isSelecting)
        {
            AddToTempSelection(terrainRaycast.RayToPosFromCamera(distance));
            UpdateLineRenderer();
        }
        else
        {
            lineRenderer.activated = false;
        }
    }

    void SelectByAddition()
    {
        if (terrainRaycast.LookingAt.HasValue)
        {
            if (isSelecting)
            {
                AddToTempSelection(terrainRaycast.LookingAt.Value.Position);
            }
            else
            {
                lineRenderer.activated = true;
                tempMin = terrainRaycast.LookingAt.Value.Position;
                tempMax = tempMin + Point3.one;
                UpdateLineRenderer();
            }
        }
        else if (isSelecting)
        {
            tempMin = minCorner;
            tempMax = maxCorner;
            UpdateLineRenderer();
        }
        else
        {
            lineRenderer.activated = false;
        }
    }

    struct SelectMethod
    {
        public string Name;
        public System.Action Action;
    }
}
