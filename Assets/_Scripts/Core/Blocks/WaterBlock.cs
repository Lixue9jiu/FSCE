using UnityEngine;
using System.Collections;

public class WaterBlock : FluidBlock
{
    static ColorMap map = new ColorMap(new Color32(0, 0, 128, 255), new Color32(0, 80, 100, 255), new Color32(0, 45, 85, 255), new Color32(0, 113, 97, 255));

    public WaterBlock() : base(7)
    {
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        Color color = map.Lookup(chunk.GetShiftValue(x, z));
        GenerateFluidTerrain(x, y, z, value, chunk, g.AlphaTest, color, color);
    }
}
