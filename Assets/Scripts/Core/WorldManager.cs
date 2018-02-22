using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public List<string> Worlds = new List<string> ();
	string WorldsFolder;

	public static ProjectData Project;

//	void Awake ()
//	{
//		WorldsFolder = Path.Combine (Application.persistentDataPath, "Worlds");
//		if (Directory.Exists (WorldsFolder)) {
//			Worlds.AddRange (Directory.GetFiles (WorldsFolder));
//		} else {
//			Directory.CreateDirectory (WorldsFolder);
//		}
//	}

	public void LoadWorld (string path)
	{
		string dir = Path.Combine (WorldsFolder, "World" + Worlds.Count);
		Directory.CreateDirectory (dir);
		ZipUtils.Unzip (path, dir);

		LoadWorldDir (dir);

		BroadcastMessage ("OnWorldLoaded");
	}

	void LoadWorldDir (string dir)
	{
		Project = new ProjectData (XDocument.Load (Path.Combine (dir, "Project.xml")));
		Vector3 playerPosition = Project
			.GetSubsystem ("Players")
			.GetValues ("Players")
			.GetValues ("1")
			.GetValue<Vector3> ("SpawnPosition");
		
	}

}

public class ProjectData {
	public XElement Root;

	public string WorldName {
		get {
			XElement gameinfo = GetSubsystem("GameInfo");
			return XMLUtils.FindValueByName (gameinfo, "WorldName");
		}
	}

	public ProjectData (XDocument doc)
	{
		Root = doc.Root;
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

	public GameInfo(ProjectData project)
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
		XMLUtils.FindValuesByName (e, "Palette").GetValue("Colors", out str);
		Colors = str.Split (';');
	}
}
