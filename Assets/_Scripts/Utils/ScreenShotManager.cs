using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotManager
{
    public readonly static string ScreenshotFolder;

    static ScreenshotManager()
    {
        ScreenshotFolder = Path.Combine(Application.persistentDataPath, "Screenshots");
        if (!Directory.Exists(ScreenshotFolder))
        {
            Directory.CreateDirectory(ScreenshotFolder);
        }
    }

    public static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot(Path.Combine(ScreenshotFolder, CurrentTime() + ".png"));
    }

    static string CurrentTime()
    {
        return System.DateTime.Now.ToString().Replace('/', '-').Replace(':', '.');
    }
}
