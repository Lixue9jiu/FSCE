using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

	public PauseMenuWindow pauseWindow;

    public GameObject operations;

	private static PauseManager main;

	public static PauseManager instance
	{
		get {
			if (main == null)
				main = FindObjectOfType<PauseManager> ();
			return main;
		}
	}

    public static void SetAllActive(bool active)
    {
		if (instance != null)
			instance.SetActiveForAll(active);
    }

    private void Start()
    {
		Resume();
    }

	public void TuggleEsc()
	{
		if (pauseWindow.isShowing) {
			Resume ();
		} else {
			Pause ();
		}
	}

    public void Pause()
    {
		pauseWindow.Show ();
        operations.SetActive(false);

        SetActiveForAll(false);
    }

    public void Resume()
    {
		pauseWindow.Hide ();
        operations.SetActive(true);

        SetActiveForAll(true);
    }

    public void SetActiveForAll(bool active)
    {
		if (pauseWindow.isShowing && (WindowManager.activeWindow != pauseWindow))
			return;
        CameraController.SetCursorLocked(active);
        GetComponent<CameraController>().enabled = active;
        GetComponent<TerrainRaycast>().enabled = active;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            ScreenshotManager.Screenshot();
        }
    }

}
