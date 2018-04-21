using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindow : BaseWindow
{
    public InputField input;
    public Text textDisplay;

    int currentHistory = 0;

<<<<<<< HEAD
	private void OnEnable()
=======
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
>>>>>>> b63ce56b5cda98e10e93d15b0b6f72ae85fb4c08
	{
        currentHistory = Console.history.Count;
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
            input.text = Console.history[currentHistory];
            input.caretPosition = input.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentHistory < Console.history.Count - 1)
        {
            currentHistory++;
            input.text = Console.history[currentHistory];
            input.caretPosition = input.text.Length;
        }
    }

    public void OnInputValueChanged(string str)
    {
        if (str.Length == 0 || str[str.Length - 1] != '\n')
            return;

        currentHistory++;

        Console.Execute(str);

        input.text = "";
    }

    public override void Show()
    {
        base.Show();
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(input.gameObject);
    }

    public override void Hide()
    {
        base.Hide();
        input.text = "";
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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

<<<<<<< HEAD
    public static ILogHandler logHandler;

	static ConsoleLog()
	{
        logHandler = new Handler ();
=======
	static ConsoleLog()
	{
		new Handler ();
>>>>>>> b63ce56b5cda98e10e93d15b0b6f72ae85fb4c08
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
