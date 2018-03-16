﻿using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class WorldManager
{
    public static List<string> Worlds = new List<string>();
    static string WorldsFolder;

    public static ProjectData Project;
    public static string ChunkDat;

    static WorldManager()
    {
        WorldsFolder = Path.Combine(Application.persistentDataPath, "Worlds");
        if (Directory.Exists(WorldsFolder))
        {
            string[] paths = Directory.GetDirectories(WorldsFolder);
            for (int i = 0; i < paths.Length; i++)
            {
                string p = paths[i];
                if (Path.GetFileName(p).StartsWith("World", System.StringComparison.Ordinal))
                {
                    if (IsWorldVaild(p))
                    {
                        Worlds.Add(p);
                    }
                    else
                    {
                        Directory.Delete(p, true);
                    }
                }
            }
        }
        else
        {
            Directory.CreateDirectory(WorldsFolder);
        }
        Debug.Log(string.Format("{0}, count: {1}", WorldsFolder, Worlds.Count));
        if (Worlds.Count > 0)
        {
            SetCurrent(0);
        }
    }

    public static bool SetCurrent(int index)
    {
        if (index > -1 && index < Worlds.Count)
        {
            LoadWorldDir(Worlds[index]);
            Debug.Log(string.Format("switching world to {0}", Worlds[index]));
            return true;
        }
        return false;
    }

    public static List<string> GetWorldNames()
    {
        string[] strs = new string[Worlds.Count];
        for (int i = 0; i < strs.Length; i++)
        {
            strs[i] = ProjectData.GetWorldName(Worlds[i]);
        }
        return new List<string>(strs);
    }

    public static void LoadWorld(Stream stream)
    {
        string dir = Path.Combine(WorldsFolder, "World");
        int i = 0;
        while (Directory.Exists(dir + i))
        {
            i++;
        }
        dir += i;
        Directory.CreateDirectory(dir);
        ZipUtils.Unzip(stream, dir);
        if (IsWorldVaild(dir))
        {
            Worlds.Add(dir);
            //			BroadcastMessage ("OnWorldLoaded", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Directory.Delete(dir, true);
        }
    }

    public static void RemoveWorld(int index)
    {
        if (index > -1 && index < Worlds.Count)
        {
            Directory.Delete(Worlds[index], true);
            Worlds.RemoveAt(index);
        }
    }

    static void LoadWorldDir(string dir)
    {
        Project = new ProjectData(dir);
        ChunkDat = Path.Combine(dir, "Chunks32.dat");
    }

    static bool IsWorldVaild(string dir)
    {
        return File.Exists(Path.Combine(dir, "Project.xml")) && File.Exists(Path.Combine(dir, "Chunks32.dat"));
    }

}