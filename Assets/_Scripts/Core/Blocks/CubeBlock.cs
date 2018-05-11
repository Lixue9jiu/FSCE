using UnityEngine;
using System.Collections;

public class CubeBlock : Block, IStandardCubeBlock
{
	public void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data)
	{
		data.TextureSlot = TextureSlot;
		data.Color = Color.white;
	}
}
