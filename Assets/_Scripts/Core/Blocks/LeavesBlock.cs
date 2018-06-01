using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesBlock : Block, INormalBlock
{
    ColorMap map;

    public override void Initialize(string extraData)
    {
        base.Initialize(extraData);
        switch (Index)
        {
            case 12:
                map = new ColorMap(new Color32(96, 161, 123, 255), new Color32(174, 164, 42, 255), new Color32(96, 161, 123, 255), new Color32(30, 191, 1, 255));
                break;
            case 13:
                map = new ColorMap(new Color32(96, 161, 96, 255), new Color32(174, 109, 42, 255), new Color32(96, 161, 96, 255), new Color32(107, 191, 1, 255));
                break;
            case 14:
                map = new ColorMap(new Color32(96, 161, 150, 255), new Color32(129, 174, 42, 255), new Color32(96, 161, 150, 255), new Color32(1, 191, 53, 255));
                break;
            case 225:
                map = new ColorMap(new Color32(90, 141, 160, 255), new Color32(119, 152, 51, 255), new Color32(86, 141, 162, 255), new Color32(1, 158, 65, 255));
                break;
        }
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = new Vector3(x, y, z + 1.0f);
        Vector3 v010 = new Vector3(x, y + 1.0f, z);
        Vector3 v011 = new Vector3(x, y + 1.0f, z + 1.0f);
        Vector3 v100 = new Vector3(x + 1.0f, y, z);
        Vector3 v101 = new Vector3(x + 1.0f, y, z + 1.0f);
        Vector3 v110 = new Vector3(x + 1.0f, y + 1.0f, z);
        Vector3 v111 = new Vector3(x + 1.0f, y + 1.0f, z + 1.0f);

        TerrainMesh terrainMesh = g.AlphaTest;
        int texSlot = TextureSlot;
        Color color = map.Lookup(chunk.GetShiftValue(x, z));

        int content = chunk.GetCellContent(x - 1, y, z);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v001, v011, v010, v000, texSlot, color);
        }

        content = chunk.GetCellContent(x, y - 1, z);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v000, v100, v101, v001, texSlot, color);
        }

        content = chunk.GetCellContent(x, y, z - 1);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v000, v010, v110, v100, texSlot, color);
        }

        content = chunk.GetCellContent(x + 1, y, z);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v100, v110, v111, v101, texSlot, color);
        }

        content = chunk.GetCellContent(x, y + 1, z);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v111, v110, v010, v011, texSlot, color);
        }

        content = chunk.GetCellContent(x, y, z + 1);
        if (content == 0 || content == 18)
        {
            terrainMesh.NormalQuad(v101, v111, v011, v001, texSlot, color);
        }
    }
}
