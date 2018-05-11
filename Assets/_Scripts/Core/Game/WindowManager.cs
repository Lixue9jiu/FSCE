using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public GameObject[] windowPrefabs;

    Dictionary<System.Type, BaseWindow> windowInstances = new Dictionary<System.Type, BaseWindow>();

    public static BaseWindow activeWindow;

    public bool inGame { get; private set; }

    private static GameObject mainCanvas;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
		inGame = SceneManager.GetActiveScene().name == "MainScene";
    }

    private void Update()
    {
		if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null) {
			if (Input.GetKeyDown(KeyCode.T))
			{
				ConsoleWindow console = Get<ConsoleWindow>();
				if (!console.isShowing)
				{
					console.Show();
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!HideActiveWindow () && inGame) {
				PauseManager.instance.TuggleEsc ();
			}
		} else if (Input.GetKeyDown (KeyCode.F10)) {
			Screen.fullScreen = !Screen.fullScreen;
		}
    }

    public bool HideActiveWindow()
    {
        if (activeWindow != null && activeWindow.isShowing)
        {
            activeWindow.Hide();
			activeWindow = null;
            return true;
        }
        return false;
    }

    static WindowManager main;

    public static WindowManager instance
    {
        get
        {
            if (main != null)
                return main;
            main = FindObjectOfType<WindowManager>();
            return main;
        }
    }

    public static T Get<T>() where T : BaseWindow
    {
        BaseWindow behaviour;
        if (instance.windowInstances.TryGetValue(typeof(T), out behaviour))
        {
            return (T)behaviour;
        }
        foreach (GameObject b in instance.windowPrefabs)
        {
            if (b.GetComponent<T>() != null)
            {
                behaviour = Instantiate(b, LanguageManager.mainCanvas.transform).GetComponent<T>();
                instance.windowInstances.Add(typeof(T), behaviour);
                //				Debug.LogFormat ("creating window: {0} on {1}", typeof(T), FindObjectOfType<Canvas> ().name);
            }
        }
        return (T)behaviour;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        windowInstances.Clear();
        inGame = scene.name == "MainScene";
    }
}