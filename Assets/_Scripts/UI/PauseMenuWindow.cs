using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuWindow : MonoBehaviour
{
	public static bool paused;

	public void OnResumeClicked ()
	{
		PauseManager.instance.Resume ();
	}

	public void OnMainMenuClicked ()
	{
		//		GetComponent<TerrainManager> ().SaveAllChunks ();
		SceneManager.LoadScene (0);
	}

	public void OnSettingsClicked ()
	{
		WindowManager.Get<SettingWindow> ().Show ();
	}

	public void Show ()
	{
		gameObject.SetActive (true);
		PauseManager.SetAllActive (false, true);

		paused = true;
	}

	public void Hide()
	{
		gameObject.SetActive (false);
		PauseManager.SetAllActive (true, true);

		paused = false;
	}
}
