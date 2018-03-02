using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
	WorldManager worldManager;

	public InputField input;
	public Dropdown dropdown;

	public Button enterWorld;
	public Button deleteWorld;
	public Button downloadWorld;

	void Start ()
	{
		worldManager = GetComponent<WorldManager> ();
		dropdown.AddOptions (worldManager.GetWorldNames ());

		dropdown.onValueChanged.AddListener (new UnityEngine.Events.UnityAction<int> (OnWorldChanged));

		enterWorld.enabled = worldManager.SetCurrent (0);
		deleteWorld.enabled = enterWorld.enabled;
	}

	public void OnWorldChanged (int i)
	{
		enterWorld.enabled = worldManager.SetCurrent (i);
		deleteWorld.enabled = enterWorld.enabled;
	}

	public void OnLoadFileClicked ()
	{
		downloadWorld.enabled = false;
		input.enabled = false;
		string url = input.text;
		if (!url.Contains ("://")) {
			url = "file://" + url.Replace ('\\', '/');
		}
		StartCoroutine (GetFromURL (url));

//		string path = UnityEditor.EditorUtility.OpenFilePanel ("choose a world file", "", "scworld");
//		if (path != string.Empty) {
//			using (Stream s = File.OpenRead (path)) {
//				GetComponent<WorldManager> ().LoadWorld (s);
//			}
//		}
//
//		dropdown.ClearOptions ();
//		dropdown.AddOptions (GetComponent<WorldManager> ().GetWorldNames ());
	}

	public void OnEnterWorldClicked ()
	{
		SceneManager.LoadScene (1);
	}

	public void OnDeleteWorldClicked ()
	{
		worldManager.RemoveWorld (dropdown.value);
		RefreshDropdown ();
	}

	void OnURLLoaded (byte[] bytes)
	{
		using (MemoryStream s = new MemoryStream (bytes)) {
			worldManager.LoadWorld (s);
		}
		RefreshDropdown ();
	}

	void RefreshDropdown ()
	{
		dropdown.ClearOptions ();
		dropdown.AddOptions (GetComponent<WorldManager> ().GetWorldNames ());
		enterWorld.enabled = worldManager.SetCurrent (dropdown.value);
		deleteWorld.enabled = enterWorld.enabled;
	}

	IEnumerator GetFromURL (string url)
	{
		Debug.Log (string.Format ("loading from url : {0}", url));
		try {
			UnityWebRequest web = UnityWebRequest.Get (url);
			yield return web.SendWebRequest ();
			if (web.isHttpError || web.isNetworkError) {
				Debug.Log (web.error);
			} else {
				Debug.Log (string.Format("file loaded, size: {0}, isDone: {1}", web.downloadHandler.data.Length, web.downloadHandler.isDone));
				OnURLLoaded (web.downloadHandler.data);
			}
		} finally {
			downloadWorld.enabled = true;
			input.enabled = true;
			input.text = string.Empty;
		}
	}
}
