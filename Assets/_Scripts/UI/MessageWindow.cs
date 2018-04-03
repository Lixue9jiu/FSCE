using UnityEngine.UI;
using UnityEngine;

public class MessageWindow : MonoBehaviour
{

    static MessageWindow main;

    public static MessageWindow instance
    {
        get
        {
            if (main == null)
                main = FindObjectOfType<MessageWindow>();
            return main;
        }
    }

    public GameObject template;

    //private void Start()
    //{
    //    WindowManager.Get<ConsoleWindow>().AssignCommand("message", (args) => { DisplayMessage(args[0]); });
    //}

    public void DisplayStringRes(string id)
    {
        DisplayMessage(LanguageManager.GetString(id));
    }

    public void DisplayMessage(string msg)
    {
        GameObject inst = Instantiate(template, transform);
        inst.GetComponentInChildren<Text>().text = msg;
        inst.SetActive(true);
    }

}
