using UnityEngine;

public class PauseManager : MonoBehaviour
{

	public PauseMenuWindow pauseWindow;

    OperationManager operation;

	private static PauseManager main;

	public static PauseManager instance
	{
		get {
			if (main == null)
				main = FindObjectOfType<PauseManager> ();
			return main;
		}
	}

	public static void SetAllActive(bool active, bool calledFromPauseMenu = false)
    {
		if (instance != null)
			instance.SetActiveForAll(active, calledFromPauseMenu);
    }

	private void Awake()
	{
        operation = GetComponent<OperationManager>();
	}

	private void Start()
    {
		Resume();
    }

	public void TuggleEsc()
	{
		if (PauseMenuWindow.paused) {
			Resume ();
		} else {
			Pause ();
		}
	}

    public void Pause()
    {
        pauseWindow.Show();
    }

    public void Resume()
    {
		pauseWindow.Hide ();
    }

	public void SetActiveForAll(bool active, bool calledFromPauseMenu)
    {
		if (PauseMenuWindow.paused && !calledFromPauseMenu)
			return;
        operation.enabled = active;
        CameraController.SetCursorLocked(active);
        GetComponent<CameraController>().enabled = active;
        GetComponent<TerrainRaycast>().enabled = active;
        OperationManager.instance.SetCurrentOpEnabled(active);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            ScreenshotManager.Screenshot();
        }
    }

}
