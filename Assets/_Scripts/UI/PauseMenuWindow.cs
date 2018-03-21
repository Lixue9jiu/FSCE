using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuWindow : BaseWindow
{

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

	public override void Show ()
	{
		gameObject.SetActive (true);
		PauseManager.SetAllActive (false);

		isShowing = true;
	}
}
