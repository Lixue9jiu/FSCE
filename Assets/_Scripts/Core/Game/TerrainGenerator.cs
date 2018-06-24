using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public void GenerateSuperFlatTerrain(BlockTerrain.Chunk chunk)
    {
        GameInfo info = WorldManager.Project.GameInfo;
        int startx = chunk.chunkx << 4;
        int startz = chunk.chunkx << 4;

        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 128; y++)
                {
                    if (y < info.TerrainLevel)
                    {
                        if (y <= 1)
                        {
                            chunk.SetCellValue(x, y, z, 1);
                        }
                        else if (startx + x >= 200 || startz + z >= 200)
                        {
                            chunk.SetCellValue(x, y, z, info.TerrainOceanBlockIndex);
                        }
                        else
                        {
                            chunk.SetCellValue(x, y, z, info.TerrainBlockIndex);
                        }
                    }
                }
            }
        }
    }
}
