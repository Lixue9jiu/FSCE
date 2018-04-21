using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    static MessageWindow main;

    public GameObject messageWindowPrefab;

    static MessageWindow GetWindow()
    {
        if (main == null)
            main = FindObjectOfType<MessageWindow>();
        return main;
    }

    public static void ShowStrRes(string id)
    {
        GetWindow().DisplayStringRes(id);
    }

    public static void Show(string str)
    {
        GetWindow().DisplayMessage(str);
    }

	private void Start()
	{
        SceneManager.sceneLoaded += OnSceneLoaded;
		OnSceneLoaded (new Scene(), LoadSceneMode.Single);
	}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        main = Instantiate(messageWindowPrefab).GetComponent<MessageWindow>();
    }
}
