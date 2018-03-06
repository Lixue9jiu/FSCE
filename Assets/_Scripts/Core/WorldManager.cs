using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public List<string> Worlds = new List<string> ();
	string WorldsFolder;

	public static ProjectData Project;
	public static string ChunkDat;

	static WorldManager()
	{
		Project = new ProjectData ("/Users/user/Library/Application Support/LixueJiu/FSCE/Worlds/World0");
		ChunkDat = "/Users/user/Library/Application Support/LixueJiu/FSCE/Worlds/World0/Chunks32.dat";
	}

	void Awake ()
	{
		WorldsFolder = Path.Combine (Application.persistentDataPath, "Worlds");
		if (Directory.Exists (WorldsFolder)) {
			string[] paths = Directory.GetDirectories (WorldsFolder);
			for (int i = 0; i < paths.Length; i++) {
				string p = paths [i];
				if (Path.GetFileName (p).StartsWith ("World")) {
					if (IsWorldVaild (p)) {
						Worlds.Add (p);
					} else {
						Directory.Delete (p, true);
					}
				}
			}
		} else {
			Directory.CreateDirectory (WorldsFolder);
		}
		Debug.Log (string.Format ("{0}, count: {1}", WorldsFolder, Worlds.Count));
	}

	public bool SetCurrent (int index)
	{
		if (index > -1 && index < Worlds.Count) {
			LoadWorldDir (Worlds [index]);
			Debug.Log (string.Format ("switching world to {0}", Worlds [index]));
			return true;
		}
		return false;
	}

	public List<string> GetWorldNames ()
	{
		string[] strs = new string[Worlds.Count];
		for (int i = 0; i < strs.Length; i++) {
			strs [i] = ProjectData.GetWorldName (Worlds [i]);
		}
		return new List<string> (strs);
	}

	public void LoadWorld (Stream stream)
	{
		string dir = Path.Combine (WorldsFolder, "World");
		int i = 0;
		while (Directory.Exists (dir + i)) {
			i++;
		}
		dir += i;
		Directory.CreateDirectory (dir);
		ZipUtils.Unzip (stream, dir);
		Worlds.Add (dir);

		LoadWorldDir (dir);

		BroadcastMessage ("OnWorldLoaded", SendMessageOptions.DontRequireReceiver);
	}

	public void RemoveWorld (int index)
	{
		if (index > -1 && index < Worlds.Count) {
			Directory.Delete (Worlds [index], true);
			Worlds.RemoveAt (index);
		}
	}

	void LoadWorldDir (string dir)
	{
		Project = new ProjectData (dir);
		ChunkDat = Path.Combine (dir, "Chunks32.dat");
	}

	bool IsWorldVaild (string dir)
	{
		return File.Exists (Path.Combine (dir, "Project.xml")) && File.Exists (Path.Combine (dir, "Chunks32.dat"));
	}

}