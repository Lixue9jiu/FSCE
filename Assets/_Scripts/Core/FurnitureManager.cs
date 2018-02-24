﻿using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{

	Dictionary<int, Mesh> furnitures = new Dictionary<int, Mesh> ();
	TerrainGenerator terrainGenerator;

	public GameObject furniturePrefab;

	void Start ()
	{
		AlaphaTest4 ();
	}

	//	void AlaphaTest ()
	//	{
	//		LoadMash (new Furniture {
	//			index = 0,
	//			Resolution = 2,
	//			data = new int[] { 1, 0, 1, 0, 1, 1, 1, 1 }
	//		});
	//		InstantiateFurniture (0, Vector3.zero, 0);
	//		InstantiateFurniture (0, new Vector3 (1, 0, 0), 0);
	//	}
	//
	//	void AlaphaTest2 ()
	//	{
	//		Furniture f = LoadFurniture (XElement.Parse ("<Values Name=\"2\">\n          <Value Name=\"TerrainUseCount\" Type=\"int\" Value=\"1\" />\n          <Value Name=\"Resolution\" Type=\"int\" Value=\"16\" />\n          <Value Name=\"InteractionMode\" Type=\"Game.FurnitureInteractionMode\" Value=\"None\" />\n          <Value Name=\"Values\" Type=\"string\" Value=\"17*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,8*0,6*180296,2*245832,8*0,6*180296,2*245832,8*0,1*180296,4*442440,1*180296,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,3*147528,3*475208,2*0,1*180296,4*0,1*180296,2*245832,3*147528,3*475208,2*0,1*180296,4*0,1*180296,2*245832,3*16456,3*475208,2*0,1*180296,4*442440,1*180296,2*245832,3*16456,3*213064,8*0,2*245832,3*147528,3*475208,8*0,2*245832,3*0,3*16456,8*0,2*245832,3*147528,11*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,1*475208,1*0,1*475208,1*147528,1*0,1*475208,2*0,1*180296,4*0,1*180296,2*245832,1*475208,1*0,1*475208,1*147528,1*0,1*475208,2*0,1*180296,4*0,1*180296,2*245832,1*475208,1*0,1*475208,1*147528,1*0,1*475208,2*0,1*180296,4*442440,1*180296,2*245832,1*475208,1*0,1*213064,1*147528,1*0,1*213064,8*0,2*245832,1*147528,1*0,1*147528,3*475208,8*0,2*245832,1*0,1*16456,1*0,3*16456,8*0,2*245832,3*147528,11*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,3*147528,3*475208,2*0,6*180296,2*245832,3*147528,3*475208,2*0,6*180296,2*245832,3*16456,3*475208,2*0,1*180296,4*442440,1*180296,2*245832,3*16456,3*213064,8*0,2*245832,3*147528,3*475208,8*0,2*245832,3*0,3*16456,8*0,2*245832,3*147528,11*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*16456,5*0,2*245832,5*0,4*16456,5*0,2*245832,5*0,4*49224,5*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*16456,2*0,1*16456,5*0,2*245832,5*0,1*16456,2*0,1*16456,5*0,2*245832,5*0,4*49224,5*0,2*245832,6*0,2*245832,6*0,2*245832,6*0,2*245832,6*0,2*245832,14*0,2*245832,14*0,34*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*49224,2*0,1*49224,5*0,2*245832,5*0,1*16456,2*0,1*16456,5*0,2*245832,5*0,1*16456,2*0,1*16456,5*0,2*245832,5*0,4*49224,5*0,2*245832,6*0,2*245832,6*0,2*245832,6*0,2*245832,6*0,2*245832,14*0,2*245832,14*0,34*245832,6*0,3*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*49224,5*0,2*245832,5*0,4*16456,5*0,2*245832,5*0,4*16456,5*0,2*245832,5*0,4*49224,5*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,34*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,2*245832,14*0,273*245832,\" />\n        </Values>"));
	//		InstantiateFurniture (f.index, Vector3.zero, 0);
	//	}

	//	void AlaphaTest3()
	//	{
	//		BlockTerrain terrain = GetComponent<BlockTerrain> ();
	//		BlockTerrain.Chunk chunk = terrain.CreateChunk (0, 0);
	//		int data = 0;
	//		SetDesignIndex (data, 0, 0, false);
	//		SetRotation (data, 0);
	//		chunk.SetCellValue (0, 0, 0, BlockTerrain.MakeBlockValue(227, 0, 1));
	//		GetComponent<TerrainManager> ().InstantiateChunk (chunk);
	//	}

	void AlaphaTest4 ()
	{
//		WorldManager.ChunkDat = "/Users/user/Library/Application Support/DefaultCompany/FSCE/Worlds/World1/Chunks32.dat";
//		WorldManager.Project = new ProjectData ("/Users/user/Library/Application Support/DefaultCompany/FSCE/Worlds/World1");
		terrainGenerator = GetComponent<TerrainGenerator> ();
		Debug.Log ("loading furnitures");
		Load (WorldManager.Project);

		GetComponent<TerrainManager> ().AlaphaTest6 ();
	}

	public void InstantiateFurniture (int index, Vector3 position, int rotation)
	{
		GameObject obj = Instantiate (furniturePrefab, position, Quaternion.AngleAxis (rotation * -90, Vector3.up));
		MeshFilter filter = obj.GetComponent<MeshFilter> ();
	
		filter.mesh = furnitures [index];
	}

	public Mesh GetFurniture (int index, int rotation)
	{
		return BlockMeshes.TranslateMeshRaw (furnitures [index], Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0, -90 * rotation, 0), Vector3.one));
	}

	public void Load (ProjectData project)
	{
		XElement designs = project.GetSubsystem ("FurnitureBlockBehavior").GetValues ("FurnitureDesigns");
		foreach (XElement elem in designs.Elements("Values")) {
			LoadFurniture (elem);
		}
	}

	Furniture LoadFurniture (XElement furniture)
	{
		int resolution;
		furniture.GetValue ("Resolution", out resolution);
		if (furniture.GetValue<int> ("TerrainUseCount") > 0) {
			Furniture f = new Furniture {
				index = int.Parse (furniture.Attribute ("Name").Value),
				Resolution = resolution,
				data = ParseData (XMLUtils.FindValueByName (furniture, "Values"), resolution),
			};
			LoadMash (f);
			return f;
		}
		return new Furniture ();
	}

	void LoadMash (Furniture furniture)
	{
		Mesh mesh;
		terrainGenerator.MeshFromFurniture (furniture, out mesh);
		furnitures[furniture.index] = mesh;
	}

	int[] ParseData (string str, int resolution)
	{
		List<int> data = new List<int> (resolution * resolution);
		string[] strs = str.Split (new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < strs.Length; i++) {
			string[] s = strs [i].Split ('*');
			int count = int.Parse (s [0]);
			int value = int.Parse (s [1]);
			for (int k = 0; k < count; k++) {
				data.Add (value);
			}
		}
		return data.ToArray ();
	}

	public struct Furniture
	{
		public int index;
		public int Resolution;
		public int[] data;

		public int GetCellValue (int x, int y, int z)
		{
			if (x >= 0 && x < Resolution && y >= 0 && y < Resolution && z >= 0 && z < Resolution) {
				return data [x + y * Resolution + z * Resolution * Resolution];
			}
			return 0;
		}
	}

	public static int SetDesignIndex (int data, int designIndex, int shadowStrengthFactor, bool isLightEmitter)
	{
		data = ((data & -4093) | (designIndex & 1023) << 2);
		data = ((data & -12289) | (shadowStrengthFactor & 3) << 12);
		data = ((data & -16385) | (isLightEmitter ? 1 : 0) << 14);
		return data;
	}

	public static int SetRotation (int data, int rotation)
	{
		return (data & -4) | (rotation & 3);
	}

	public static int GetVariant (int data)
	{
		return data & 31;
	}

}
