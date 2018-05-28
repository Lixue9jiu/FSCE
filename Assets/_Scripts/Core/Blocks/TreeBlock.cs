using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBlock : Block, IStandardCubeBlock
{
    public void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data)
    {
        data.TextureSlot = face == CellFace.TOP || face == CellFace.BOTTOM ? 21 : TextureSlot;
        data.Color = Color.white;
    }
}
