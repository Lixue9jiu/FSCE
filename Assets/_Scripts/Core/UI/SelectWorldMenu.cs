using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectWorldMenu : MonoBehaviour
{
    [SerializeField]
    ListPanel listPanel;

    [SerializeField]
    Button enterWorld;

    [SerializeField]
    Button deleteWorld;

    [SerializeField]
    Button exportWorld;

    void Start()
    {
        foreach (string s in WorldManager.GetWorldNames())
        {
            listPanel.AddItem().GetComponentInChildren<Text>().text = s;
        }
        enterWorld.interactable = false;
        deleteWorld.interactable = false;
        exportWorld.interactable = false;
    }

    public void OnListPanelItemSelected(int i)
    {
        enterWorld.interactable = WorldManager.SetCurrent(i);
        deleteWorld.interactable = enterWorld.enabled;
        exportWorld.interactable = enterWorld.enabled;
    }

    public void OnEnterWorldClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnDeleteWorldClicked()
    {
        WorldManager.RemoveWorld(listPanel.SelectedItem);
        RefreshList();
    }

    public void OnExportWorldClicked()
    {
        ListWindow.ShowListWindow(new string[] { "sdcard" }, HandleUnityAction);
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnBackButtonClicked();
        }
    }

    void HandleUnityAction(string str)
    {
        switch (str)
        {
            case "sdcard":
                WorldPacker.ExportToSdcard(listPanel.SelectedItem);
                break;
        }
    }

    void RefreshList()
    {
        listPanel.ClearItems();
        enterWorld.interactable = false;
        deleteWorld.interactable = false;
        exportWorld.interactable = false;
        foreach (string s in WorldManager.GetWorldNames())
        {
            listPanel.AddItem().GetComponentInChildren<Text>().text = s;
        }
    }
}
