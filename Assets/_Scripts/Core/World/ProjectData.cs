﻿using System.IO;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectData
{
	readonly float version;

	public XElement Root;

    public readonly GameInfo GameInfo;

	public static string GetWorldName (string path)
	{
		ProjectData p = new ProjectData (path);
		return p.GetSubsystem ("GameInfo").GetValue<string> ("WorldName");
	}

	public Vector3 PlayerPosition {
		get {
			if (version >= 2.1f) {
				Vector3 v = GetSubsystem ("Players")
				.GetValues ("Players")
				.GetValues ("1")
				.GetValue<Vector3> ("SpawnPosition");
				v.x = -v.x;
				return v + new Vector3(0, 1.7f, 0);
			} else {
				Vector3 v = GetSubsystem ("Player").GetValue<Vector3> ("SpawnPosition");
				v.x = -v.x;
				return v + new Vector3(0, 1.7f, 0);
			}
		}
	}

	public ProjectData (XDocument doc)
	{
		Root = doc.Root;
		version = float.Parse (Root.Attribute ("Version").Value);
        GameInfo = new GameInfo(this);
	}

    public ProjectData(string worldPath) : this (XDocument.Load(Path.Combine(worldPath, "Project.xml")))
    {
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
	public string TerrainGenerationMode;
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
		e.GetValue ("TerrainGenerationMode", out TerrainGenerationMode);
		e.GetValue ("TerrainLevel", out TerrainLevel);
		e.GetValue ("TerrainBlockIndex", out TerrainBlockIndex);
		e.GetValue ("TerrainOceanBlockIndex", out TerrainOceanBlockIndex);
		e.GetValue ("TemperatureOffset", out TemperatureOffset);
		e.GetValue ("HumidityOffset", out HumidityOffset);
		e.GetValue ("SeaLevelOffset", out SeaLevelOffset);
		e.GetValueOrDefault ("BiomeSize", out BiomeSize, 0);
		e.GetValueOrDefault ("BlockTextureName", out BlockTextureName, "");

		string str;
		XElement palette = XMLUtils.FindValuesByName (e, "Palette");
		if (palette != null) {
			palette.GetValue ("Colors", out str);
			Colors = str.Split (';');
		} else {
			Colors = new string[16];
		}
	}
}
