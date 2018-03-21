using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindow : BaseWindow
{
    public InputField input;
    public Text textDisplay;

    Dictionary<string, System.Action<string[]>> commands = new Dictionary<string, System.Action<string[]>>();

    public void AssignCommand(string name, System.Action<string[]> action)
    {
        commands.Add(name, action);
    }

    private void Update()
    {
        if (ConsoleLog.needFresh)
        {
            textDisplay.text = ConsoleLog.text;
            ConsoleLog.needFresh = false;
        }
    }

    public void OnInputValueChanged(string str)
    {
        if (str.Length == 0 || str[str.Length - 1] != '\n')
            return;

        str = str.Remove(str.Length - 1);
        ConsoleLog.Log("--> " + str);
        int index = str.IndexOf(' ');
        if (index == -1)
        {
            RunCommand(str, null);
        }
        else
        {
            RunCommand(str.Remove(index), str.Substring(index + 1).Split(' '));
        }

        input.text = "";
    }

    public void RunCommand(string name, string[] args)
    {
        if (commands.ContainsKey(name))
        {
            commands[name](args);
        }
        else
        {
            ConsoleLog.LogFormat("command named \"{0}\" does not exist", name);
        }
    }

    public override void Show()
    {
        base.Show();
        FocusTo(input.gameObject);
    }

    public override void Hide()
    {
        base.Hide();
        input.text = "";
        FocusTo(gameObject);
    }

    static void FocusTo(GameObject obj)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(obj);
    }
}

public static class ConsoleLog
{
    public static string text
    {
        get
        {
            return strBuilder.ToString();
        }
    }
    static StringBuilder strBuilder = new StringBuilder();
    public static bool needFresh = false;

    public static void Log(string str)
    {
        strBuilder.AppendLine(str);
        needFresh = true;
    }

    public static void LogFormat(string format, params object[] param)
    {
        Log(string.Format(format, param));
    }
}