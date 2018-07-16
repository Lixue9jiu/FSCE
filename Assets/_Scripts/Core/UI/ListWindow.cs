using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ListWindow : BaseWindow
{
    public GameObject content;
    public RectTransform mainPanel;
    public GameObject itemTemplate;
    public int MaxHeight;

    public string[] items;

    public UnityEngine.Events.UnityAction<string> OnItemSelected;

    bool IsFileBrowserMode;
    string currentPath;
    System.Action<string> fileBrowserHandler;

    public void ShowAsFileBrowser(string startUpPath, System.Action<string> handler)
    {
        IsFileBrowserMode = true;
        currentPath = startUpPath;
        fileBrowserHandler = handler;
        string[] paths = Directory.GetFileSystemEntries(startUpPath);
        items = new string[paths.Length + 1];
        items[0] = "..";
        for (int i = 1; i < items.Length; i++)
        {
            items[i] = Path.GetFileName(paths[i - 1]);
        }
        Show();
    }

    public override void Show()
    {
        if (items.Length == 0)
        {
            Debug.LogError("item list is empty");
        }
        var grid = content.GetComponent<VerticalLayoutGroup>();
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        grid.transform.DetachChildren();

        foreach (string s in items)
        {
            var obj = Instantiate(itemTemplate, grid.transform);
            obj.name = s;
            obj.GetComponentInChildren<Text>().text = s;
            if (IsFileBrowserMode)
            {
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var filePath = Path.Combine(currentPath, s);
                    if (Directory.Exists(filePath))
                    {
                        ShowAsFileBrowser(filePath, fileBrowserHandler);
                    }
                    else
                    {
                        Hide();
                        fileBrowserHandler(filePath);
                    }
                });
            }
            else
            {
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Hide();
                    OnItemSelected.Invoke(s);
                });
            }
        }

        float height = items.Length * itemTemplate.GetComponent<RectTransform>().sizeDelta.y
                              + content.transform.childCount * grid.spacing
                              + grid.padding.top + grid.padding.bottom;

        mainPanel.sizeDelta = new Vector2(mainPanel.sizeDelta.x, Mathf.Min(height, MaxHeight));
        base.Show();
    }

    public override void Hide()
    {
        IsFileBrowserMode = false;
        base.Hide();
    }

    public static void ShowListWindow(string[] list, UnityEngine.Events.UnityAction<string> handler)
    {
        var window = WindowManager.Get<ListWindow>();
        window.items = list;
        window.OnItemSelected = handler;
        window.Show();
    }

    public static void ShowFileBrowser(string startUpPath, System.Action<string> handler)
    {
        if (Directory.Exists(startUpPath))
            WindowManager.Get<ListWindow>().ShowAsFileBrowser(startUpPath, handler);
        //MessageManager.Show("cannot find path: " + startUpPath);
    }
}
