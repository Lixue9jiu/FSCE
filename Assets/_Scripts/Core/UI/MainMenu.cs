using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{

    public void OnImportWorldClicked()
    {
#if UNITY_ANDROID
        ListWindow.ShowListWindow(new string[] { "sdcard", "external_link" }, HandleImportWorldList);
#else
        ListWindow.ShowListWindow(new string[] { "external_link" }, HandleImportWorldList);
#endif
    }

    void HandleImportWorldList(string button)
    {
        switch (button)
        {
            case "sdcard":
                WorldPacker.ImportFromSdcard();
                break;
            case "external_link":
                TextBoxWindow.ShowTextBox("enter_world_link", (url) =>
                {
                    if (!url.Contains("://"))
                    {
                        url = "file://" + url.Replace('\\', '/');
                    }
                    StartCoroutine(GetFromURL(url));
                });
                break;
        }
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
    }

    public void OnStartGameClicked()
    {
        SceneManager.LoadScene("SelectWorld");
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

    }

    ProgressWindow progressWindow;

    IEnumerator GetFromURL(string url)
    {
        Debug.Log(string.Format("loading from url : {0}", url));
        progressWindow = WindowManager.Get<ProgressWindow>();
        try
        {
            UnityWebRequest web = UnityWebRequest.Get(url);
            var operation = web.SendWebRequest();
            progressWindow.SetOperation(operation);
            progressWindow.Show();
            yield return operation;
            if (web.isHttpError || web.isNetworkError)
            {
                Debug.LogError(web.error);
                MessageManager.Show(web.error);
            }
            else
            {
                Debug.Log(string.Format("file loaded, size: {0}, isDone: {1}", web.downloadHandler.data.Length, web.downloadHandler.isDone));
                OnURLLoaded(web.downloadHandler.data);
            }
        }
        finally
        {
            progressWindow.Hide();
            progressWindow = null;
        }
    }
}
