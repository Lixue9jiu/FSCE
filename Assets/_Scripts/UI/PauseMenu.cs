using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

	public GameObject menu;
	public PopupWindow window;

	public void Pause ()
	{
		menu.SetActive (true);
	}

	public void OnResumeClicked ()
	{
		CamaraController.SetCursorLocked (true);
		menu.SetActive (false);
	}

	public void OnMainMenuClicked ()
	{
		window.ShowThreeChoices ("要保存世界吗？", delegate {
			GetComponent<TerrainManager> ().SaveAllChunks ();
			SceneManager.LoadScene (0);
		}, delegate {
			SceneManager.LoadScene (0);
		}, delegate {
		}
		);
	}

	public void OnSaveWorldClicked ()
	{
		GetComponent<TerrainManager> ().SaveAllChunks ();
	}

}
