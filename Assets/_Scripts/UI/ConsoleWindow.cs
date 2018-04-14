using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindow : BaseWindow
{
    public InputField input;
    public Text textDisplay;

    Dictionary<string, System.Action<string[]>> commands = new Dictionary<string, System.Action<string[]>>();

    List<string> history = new List<string>(20);
    int currentHistory = 0;

    private void Start()
    {
        currentHistory = history.Count;
    }

    private void Awake()
    {
        AssignCommand("help", (args) =>
        {
            foreach (string s in commands.Keys)
            {
                ConsoleLog.Log(s);
            }
        });
    }

    public void AssignCommand(string name, System.Action<string[]> action)
    {
        commands.Add(name, action);
    }

	public void RemoveCommand(string name)
	{
		commands.Remove (name);
	}

    private void Update()
    {
        if (ConsoleLog.needFresh)
        {
            textDisplay.text = ConsoleLog.text;
            ConsoleLog.needFresh = false;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentHistory > 0)
        {
            currentHistory--;
            input.text = history[currentHistory];
            input.caretPosition = input.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentHistory < history.Count - 1)
        {
            currentHistory++;
            input.text = history[currentHistory];
            input.caretPosition = input.text.Length;
        }
    }

    public void OnInputValueChanged(string str)
    {
        if (str.Length == 0 || str[str.Length - 1] != '\n')
            return;

        str = str.TrimEnd('\n');
        history.Add(str);
        currentHistory = history.Count;

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
        try
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
        catch (System.Exception e)
        {
            ConsoleLog.Log(e.Message);
            ConsoleLog.Log(e.StackTrace);
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

	static ConsoleLog()
	{
		new Handler ();
	}

    public static void Log(string str)
    {
        strBuilder.AppendLine(str);
        needFresh = true;
    }

    public static void LogFormat(string format, params object[] param)
    {
        Log(string.Format(format, param));
    }

	class Handler : ILogHandler
	{
		ILogHandler defaultLogHandler;

		public Handler()
		{
			defaultLogHandler = Debug.unityLogger.logHandler;
			Debug.unityLogger.logHandler = this;
		}

		public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			ConsoleLog.LogFormat (format, args);
			defaultLogHandler.LogFormat(logType, context, format, args);
		}

		public void LogException(System.Exception exception, UnityEngine.Object context)
		{
			ConsoleLog.Log (exception.Message);
			ConsoleLog.Log (exception.StackTrace);
			defaultLogHandler.LogException(exception, context);
		}
	}
}
