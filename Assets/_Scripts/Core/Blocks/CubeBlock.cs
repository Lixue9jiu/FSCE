using UnityEngine;
using System.Collections;

public class CubeBlock : Block
{
	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
		data.TextureSlot = TextureSlot;
		data.Color = Color.white;
	}
}
