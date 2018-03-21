using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public GameObject[] windowPrefabs;

    Dictionary<System.Type, BaseWindow> windowInstances = new Dictionary<System.Type, BaseWindow>();

    public static BaseWindow activeWindow;

    public bool inGame { get; private set; }

    private void Awake()
    {
        SettingWindow setting;
        foreach (GameObject obj in windowPrefabs)
        {
            setting = obj.GetComponent<SettingWindow>();
            if (setting != null)
            {
                setting.Initialize();
            }
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        // windowPrefabs = new Dictionary<System.Type, GameObject>()
        // {
        //     {typeof(PopupWindow), popupWindow},
        //     {typeof(SettingWindow), settingWindow}
        // };
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ConsoleWindow console = Get<ConsoleWindow>();
            if (!console.isShowing)
            {
                console.Show();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!HideActiveWindow() && inGame)
            {
				PauseManager.instance.TuggleEsc();
            }
        }
    }

    public bool HideActiveWindow()
    {
        if (activeWindow != null && activeWindow.isShowing)
        {
            activeWindow.Hide();
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
                behaviour = Instantiate(b, FindObjectOfType<Canvas>().transform).GetComponent<T>();
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
		if (inGame) {
			windowInstances [typeof(PauseMenuWindow)] = FindObjectOfType<PauseMenuWindow> ();
		} else {
			windowInstances.Remove(typeof(PauseMenuWindow));
		}
    }
}