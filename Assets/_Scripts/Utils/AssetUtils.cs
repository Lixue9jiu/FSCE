using System.IO;
using UnityEngine;

public static class AssetUtils {

	public static TextReader LoadText(string path)
	{
		#if UNITY_EDITOR || !UNITY_ANDROID
		return new StreamReader (path);

		#else
		WWW reader = new WWW (path);

		while (!reader.isDone) {
		}

		return new StringReader (reader.text);
		#endif
	}

}
