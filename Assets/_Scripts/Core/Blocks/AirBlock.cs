using UnityEngine;
using System.Collections;

public class AirBlock : Block
{
	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		IsTransparent = true;
	}
}
