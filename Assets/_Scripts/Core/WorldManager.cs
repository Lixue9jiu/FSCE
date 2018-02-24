using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public List<string> Worlds = new List<string> ();
	string WorldsFolder;

	public static ProjectData Project;
	public static string ChunkDat;

//	static WorldManager()
//	{
//		Project = new ProjectData ("/Users/user/Library/Application Support/DefaultCompany/FSCE/Worlds/World1");
//		ChunkDat = "/Users/user/Library/Application Support/DefaultCompany/FSCE/Worlds/World1/Chunks32.dat";
//	}

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

public class ProjectData
{
	public XElement Root;

	public static string GetWorldName (string path)
	{
		ProjectData p = new ProjectData (path);
		return p.GetSubsystem ("GameInfo").GetValue<string> ("WorldName");
	}

	public Vector3 PlayerPosition {
		get {
			Vector3 v = GetSubsystem ("Players")
				.GetValues ("Players")
				.GetValues ("1")
				.GetValue<Vector3> ("SpawnPosition");
			v.x = -v.x;
			return v;
		}
	}

	public ProjectData (XDocument doc)
	{
		Root = doc.Root;
	}

	public ProjectData (string worldPath)
	{
		Root = XDocument.Load (Path.Combine (worldPath, "Project.xml")).Root;
	}

	public GameInfo GetGameInfo ()
	{
		return new GameInfo (this);
	}

	public XElement GetSubsystem (string name)
	{
		return XMLUtils.FindValuesByName (Root.Element ("Subsystems"), name);
	}
}

public struct GameInfo
{
	public string WorldName;
	public string WorldSeed;
	public int TerrainLevel;
	public int TerrainBlockIndex;
	public int TerrainOceanBlockIndex;
	public int TemperatureOffset;
	public int HumidityOffset;
	public int SeaLevelOffset;
	public int BiomeSize;
	public string BlockTextureName;
	public string[] Colors;

	public GameInfo (ProjectData project)
	{
		XElement e = project.GetSubsystem ("GameInfo");
		e.GetValue ("WorldName", out WorldName);
		e.GetValue ("WorldSeedString", out WorldSeed);
		e.GetValue ("TerrainLevel", out TerrainLevel);
		e.GetValue ("TerrainBlockIndex", out TerrainBlockIndex);
		e.GetValue ("TerrainOceanBlockIndex", out TerrainOceanBlockIndex);
		e.GetValue ("TemperatureOffset", out TemperatureOffset);
		e.GetValue ("HumidityOffset", out HumidityOffset);
		e.GetValue ("SeaLevelOffset", out SeaLevelOffset);
		e.GetValue ("BiomeSize", out BiomeSize);
		e.GetValue ("BlockTextureName", out BlockTextureName);

		string str;
		XMLUtils.FindValuesByName (e, "Palette").GetValue ("Colors", out str);
		Colors = str.Split (';');
	}
}
