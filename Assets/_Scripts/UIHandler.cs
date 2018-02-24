using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
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
		StartCoroutine (GetFromURL (new WWW (input.text)));

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
		SceneManager.LoadScene ("MainScene");
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
		downloadWorld.enabled = true;
		input.enabled = true;
		input.text = string.Empty;
	}

	void RefreshDropdown ()
	{
		dropdown.ClearOptions ();
		dropdown.AddOptions (GetComponent<WorldManager> ().GetWorldNames ());
		enterWorld.enabled = worldManager.SetCurrent (dropdown.value);
		deleteWorld.enabled = enterWorld.enabled;
	}

	IEnumerator GetFromURL (WWW www)
	{
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.error);
			downloadWorld.enabled = true;
			input.enabled = true;
			input.text = string.Empty;
		} else {
			OnURLLoaded (www.bytes);
		}
	}
}
