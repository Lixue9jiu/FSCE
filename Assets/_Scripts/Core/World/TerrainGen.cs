using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour {

	enum TerrainGenMode
	{
		Flat,
		Other
	}

	TerrainGenMode m_mode;

	void Load (ProjectData data)
	{
		GameInfo info = data.GetGameInfo ();
		m_mode = info.TerrainGenerationMode == "Flat" ? TerrainGenMode.Flat : TerrainGenMode.Other;
	}

	bool GenerateTerrain (int chunkx, int chunky, BlockTerrain terrain, out BlockTerrain.Chunk chunk)
	{
		if (m_mode == TerrainGenMode.Flat) {
			chunk = terrain.CreateChunk (chunkx, chunky);



			return true;
		}
		chunk = null;
		return false;
	}
}
