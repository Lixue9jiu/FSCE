using UnityEngine;
using System.IO;

public class WorldPacker
{
#if UNITY_EDITOR
    public const string EXPORT_DIR = "Survivalcraft";
#elif UNITY_ANDROID
    public const string EXPORT_DIR = "/sdcard/Survivalcraft";
#endif

    public static void ExportToSdcard(int index)
    {
        if (!Directory.Exists(EXPORT_DIR))
        {
            Directory.CreateDirectory(EXPORT_DIR);
        }
        var worldName = ProjectData.GetWorldName(WorldManager.Worlds[index]);
        var path = Path.Combine(EXPORT_DIR, worldName + ".scworld");
        if (File.Exists(path))
        {
            int i = 1;
            while (true)
            {
                path = Path.Combine(EXPORT_DIR, string.Format("{0} {1}.scworld", worldName, i));
                if (!File.Exists(path))
                    break;
                i++;
            }
        }
        using (Stream s = File.Create(path))
        {
            WorldManager.ExportWorld(index, s);
        }
    }

    public static void ImportFromSdcard()
    {
        ListWindow.ShowFileBrowser(EXPORT_DIR, (s) =>
        {
            using (Stream stream = File.Open(s, FileMode.Open, FileAccess.ReadWrite))
            {
                WorldManager.LoadWorld(stream);
            }
        });
    }
}
