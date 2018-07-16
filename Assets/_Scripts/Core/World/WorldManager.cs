using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class WorldManager
{
    public static List<string> Worlds = new List<string>();
    static string WorldsFolder;

    public static ProjectData Project;
    public static string ChunkDat;

	public static string CurrentWorldDir;

	public static string CurrentEmbeddedContent
	{
		get {
			return Path.Combine (CurrentWorldDir, "EmbeddedContent");
		}
	}

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
#if UNITY_EDITOR
        if (Worlds.Count > 0)
        {
            SetCurrent(0);
        }
#endif
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
            //MessageManager.ShowStrRes("load_succeed");
        }
        else
        {
            Directory.Delete(dir, true);
            //MessageManager.ShowStrRes("load_failed");
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

    public static void ExportWorld(int index, Stream stream)
    {
        if (!stream.CanWrite)
            throw new System.Exception("stream cannot write");
        string tmpPath = Path.GetTempFileName();
        ZipUtils.Zip(Worlds[index], tmpPath);
        using (Stream s = File.OpenRead(tmpPath))
        {
            byte[] buffer = new byte[4096];
            int read;
            while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, read);
            }
        }
        File.Delete(tmpPath);
    }

    static void LoadWorldDir(string dir)
    {
		CurrentWorldDir = dir;

        Project = new ProjectData(dir);
        ChunkDat = Path.Combine(dir, "Chunks32.dat");
    }

    static bool IsWorldVaild(string dir)
    {
        return File.Exists(Path.Combine(dir, "Project.xml")) && File.Exists(Path.Combine(dir, "Chunks32.dat"));
    }

}