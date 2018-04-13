using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
    public InputField input;
    public Dropdown dropdown;

    public Button enterWorld;
    public Button deleteWorld;
    public Button downloadWorld;

    void Start()
    {
        dropdown.AddOptions(WorldManager.GetWorldNames());

        dropdown.onValueChanged.AddListener(OnWorldChanged);

        enterWorld.enabled = WorldManager.Worlds.Count > 0;
        deleteWorld.enabled = enterWorld.enabled;
    }

	public void OnInputFieldChanged(string str)
	{
		if (str.Length != 0 && str[str.Length - 1] == '\n') {
			OnLoadFileClicked ();
		}
	}

    public void OnWorldChanged(int i)
    {
        enterWorld.enabled = WorldManager.SetCurrent(i);
        deleteWorld.enabled = enterWorld.enabled;
    }

    public void OnLoadFileClicked()
    {
        downloadWorld.enabled = false;
        input.enabled = false;
        string url = input.text;
        if (!url.Contains("://"))
        {
            url = "file://" + url.Replace('\\', '/');
        }
        StartCoroutine(GetFromURL(url));

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

    public void OnEnterWorldClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnDeleteWorldClicked()
    {
        WorldManager.RemoveWorld(dropdown.value);
        RefreshDropdown();
    }

	public void OnSettingButtonClicked()
	{
        WindowManager.Get<SettingWindow>().Show();
    }

	void OnURLLoaded(byte[] bytes)
    {
        using (Stream s = new MemoryStream(bytes))
        {
            WorldManager.LoadWorld(s);
        }
        RefreshDropdown();
    }

    void RefreshDropdown()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(WorldManager.GetWorldNames());
        enterWorld.enabled = WorldManager.SetCurrent(dropdown.value);
        deleteWorld.enabled = enterWorld.enabled;
    }

    IEnumerator GetFromURL(string url)
    {
        Debug.Log(string.Format("loading from url : {0}", url));
        try
        {
            UnityWebRequest web = UnityWebRequest.Get(url);
            yield return web.SendWebRequest();
            if (web.isHttpError || web.isNetworkError)
            {
                Debug.Log(web.error);
            }
            else
            {
                Debug.Log(string.Format("file loaded, size: {0}, isDone: {1}", web.downloadHandler.data.Length, web.downloadHandler.isDone));
                OnURLLoaded(web.downloadHandler.data);
            }
        }
        finally
        {
            downloadWorld.enabled = true;
            input.enabled = true;
            input.text = string.Empty;
        }
    }
}
