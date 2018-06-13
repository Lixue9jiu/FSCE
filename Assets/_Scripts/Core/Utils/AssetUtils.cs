using System.IO;
using UnityEngine;

public static class AssetUtils {

	public static TextReader LoadText(string path)
	{
		if (path.Contains("://"))
		{
			WWW reader = new WWW(path);         
            while (!reader.isDone)
            {
            }
            return new StringReader(reader.text);
		}
		return new StreamReader (path);
	}
    
}
