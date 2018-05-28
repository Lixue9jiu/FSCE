using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrassBlock : Block, INormalBlock
{
    public static bool GetIsSmall(int data)
    {
        return (data & 8) != 0;
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        int textureSlot = GetIsSmall(BlockTerrain.GetData(value)) ? 84 : 85;
        Color color = GrassBlock.map.Lookup(chunk.GetShiftValue(x, z));

        g.AlphaTest.TwoSidedQuad(new Vector3(x, y, z),
                                 new Vector3(x + 1.0f, y, z + 1.0f),
                                 new Vector3(x + 1.0f, y + 1.0f, z + 1.0f),
                                 new Vector3(x, y + 1.0f, z),
                                 textureSlot, color);

        g.AlphaTest.TwoSidedQuad(new Vector3(x, y, z + 1.0f),
                                 new Vector3(x + 1.0f, y, z),
                                 new Vector3(x + 1.0f, y + 1.0f, z),
                                 new Vector3(x, y + 1.0f, z + 1.0f),
                                 textureSlot, color);
    }
}
