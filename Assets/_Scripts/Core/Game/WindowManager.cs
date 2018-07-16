using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public GameObject[] windowPrefabs;

    public GameObject baseCanvasPrefab;
    public GameObject baseCanvas;

    public static BaseWindow ActiveWindow { get; private set; }

    public static bool isShowingWindow
    {
        get
        {
            return ActiveWindow != null && ActiveWindow.isShowing;
        }
    }

    public bool inGame { get; private set; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        inGame = SceneManager.GetActiveScene().name == "MainScene";
    }

    private void Update()
    {
        if (!isShowingWindow && Input.GetKeyDown(KeyCode.T))
        {
            ConsoleWindow console = Get<ConsoleWindow>();
            if (!console.isShowing)
            {
                console.Show();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!HideActiveWindow() && inGame)
            {
                PauseManager.instance.TuggleEsc();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public static bool SetAsActiveWindow(BaseWindow window)
    {
        if (window.isShowing || isShowingWindow)
            return false;

        ActiveWindow = window;

        if (instance.baseCanvas == null)
            instance.baseCanvas = Instantiate(instance.baseCanvasPrefab);
        window.transform.SetParent(instance.baseCanvas.transform, false);
        return true;
    }

    public static bool HideActiveWindow()
    {
        if (isShowingWindow)
        {
            ActiveWindow.Hide();
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
        foreach (GameObject b in instance.windowPrefabs)
        {
            if (b.GetComponent<T>() != null)
            {
                behaviour = Instantiate(b).GetComponent<T>();
                return (T)behaviour;
                //				Debug.LogFormat ("creating window: {0} on {1}", typeof(T), FindObjectOfType<Canvas> ().name);
            }
        }
        throw new System.Exception("unknown window: " + typeof(T));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        baseCanvas = null;
        inGame = scene.name == "MainScene";
    }
}