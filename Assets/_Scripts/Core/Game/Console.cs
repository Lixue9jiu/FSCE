using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Console {

    static Dictionary<string, System.Action<string[]>> commands = new Dictionary<string, System.Action<string[]>>();

    public static readonly List<string> history = new List<string>(20);

    static Console()
    {
        AssignCommand("help", (args) =>
        {
            ConsoleLog.Log("可以使用的命令：");
            foreach (string s in commands.Keys)
            {
                ConsoleLog.Log(s);
            }
        });
    }

    public static void AssignCommand(string name, System.Action<string[]> action)
    {
        commands[name] = action;
    }

    public static void RemoveCommand(string name)
    {
        commands.Remove(name);
    }

    public static void Execute(string str)
    {
        str = str.TrimEnd('\n');
        history.Add(str);

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
    }

    static void RunCommand(string name, string[] args)
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

}
