using UnityEngine;
using System.Collections;

public class MegmaBlock : FluidBlock
{
    public MegmaBlock() : base(4)
    {
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        GenerateFluidTerrain(x, y, z, value, chunk, g.Terrain, Color.white, Color.white);
    }
}
