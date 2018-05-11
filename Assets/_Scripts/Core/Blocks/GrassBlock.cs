using UnityEngine;
using System.Collections;

public class GrassBlock : Block, IStandardCubeBlock
{
    public static ColorMap map = new ColorMap(new Color32(141, 198, 166, 255), new Color32(210, 201, 93, 255), new Color32(141, 198, 166, 255), new Color32(79, 225, 56, 255));
   
	public void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data)
	{
		switch (face)
		{
			case CellFace.TOP:
				data.TextureSlot = 0;
				data.Color = map.Lookup(chunk.GetShiftValue(x, z));
				break;
			case CellFace.BOTTOM:
                data.TextureSlot = 2;
				data.Color = Color.white;
                break;
			default:
                data.TextureSlot = 3;
				data.Color = Color.white;
                break;
		}
	}
}
