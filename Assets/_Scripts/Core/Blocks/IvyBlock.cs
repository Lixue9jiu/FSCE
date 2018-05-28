using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyBlock : Block, INormalBlock
{
    private static ColorMap map = new ColorMap(new Color32(96, 161, 123, 255), new Color32(174, 164, 42, 255), new Color32(96, 161, 123, 255), new Color32(30, 191, 1, 255));

    public static int GetFace(int data)
    {
        return data & 3;
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        switch (GetFace(BlockTerrain.GetData(value)))
        {
            case 1:
                g.AlphaTest.TwoSidedQuad(
                    new Vector3(x, y, z),
                    new Vector3(x, y + 1.0f, z),
                    new Vector3(x, y + 1.0f, z + 1.0f),
                    new Vector3(x, y, z + 1.0f),
                    TextureSlot, map.Lookup(chunk.GetShiftValue(x, z))
                );
                break;
            case 0:
                g.AlphaTest.TwoSidedQuad(
                    new Vector3(x, y, z + 1.0f),
                    new Vector3(x, y + 1.0f, z + 1.0f),
                    new Vector3(x + 1.0f, y + 1.0f, z + 1.0f),
                    new Vector3(x + 1.0f, y, z + 1.0f),
                    TextureSlot, map.Lookup(chunk.GetShiftValue(x, z))
                );
                break;
            case 3:
                g.AlphaTest.TwoSidedQuad(
                    new Vector3(x + 1.0f, y, z + 1.0f),
                    new Vector3(x + 1.0f, y + 1.0f, z + 1.0f),
                    new Vector3(x + 1.0f, y + 1.0f, z),
                    new Vector3(x + 1.0f, y, z),
                    TextureSlot, map.Lookup(chunk.GetShiftValue(x, z))
                );
                break;
            case 2:
                g.AlphaTest.TwoSidedQuad(
                    new Vector3(x + 1.0f, y, z),
                    new Vector3(x + 1.0f, y + 1.0f, z),
                    new Vector3(x, y + 1.0f, z),
                    new Vector3(x, y, z),
                    TextureSlot, map.Lookup(chunk.GetShiftValue(x, z))
                );
                break;
            default:
                throw new UnityException("undefined face: " + GetFace(BlockTerrain.GetData(value)));
        }

    }
}
