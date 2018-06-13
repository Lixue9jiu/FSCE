using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlock : Block, INormalBlock
{
    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = new Vector3(x, y, z + 1.0f);
        Vector3 v010 = new Vector3(x, y + 0.125f, z);
        Vector3 v011 = new Vector3(x, y + 0.125f, z + 1.0f);
        Vector3 v100 = new Vector3(x + 1.0f, y, z);
        Vector3 v101 = new Vector3(x + 1.0f, y, z + 1.0f);
        Vector3 v110 = new Vector3(x + 1.0f, y + 0.125f, z);
        Vector3 v111 = new Vector3(x + 1.0f, y + 0.125f, z + 1.0f);

        GreedyTerrainMesh terrainMesh = g.AlphaTest;

        int content = chunk.GetCellContent(x - 1, y, z);
        if (content != BlockTerrain.NULL_BLOCK_CONTENT && content != Index && BlocksData.IsTransparent[content])
        {
            terrainMesh.NormalQuad(v001, v011, v010, v000, TextureSlot, Color.white);
        }

        content = chunk.GetCellContent(x, y, z - 1);
        if (content != BlockTerrain.NULL_BLOCK_CONTENT && content != Index && BlocksData.IsTransparent[content])
        {
            terrainMesh.NormalQuad(v000, v010, v110, v100, TextureSlot, Color.white);
        }

        content = chunk.GetCellContent(x + 1, y, z);
        if (content != BlockTerrain.NULL_BLOCK_CONTENT && content != Index && BlocksData.IsTransparent[content])
        {
            terrainMesh.NormalQuad(v100, v110, v111, v101, TextureSlot, Color.white);
        }

        content = chunk.GetCellContent(x, y + 1, z);
        if (content != BlockTerrain.NULL_BLOCK_CONTENT && content != Index && BlocksData.IsTransparent[content])
        {
            terrainMesh.NormalQuad(v111, v110, v010, v011, TextureSlot, Color.white);
        }

        content = chunk.GetCellContent(x, y, z + 1);
        if (content != BlockTerrain.NULL_BLOCK_CONTENT && content != Index && BlocksData.IsTransparent[content])
        {
            terrainMesh.NormalQuad(v101, v111, v011, v001, TextureSlot, Color.white);
        }
    }
}
