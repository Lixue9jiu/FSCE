using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBlock : Block, INormalBlock, IPaintableBlock
{
    MeshData[] blockMeshes = new MeshData[16];
    MeshData[] paintedBlockMeshes = new MeshData[16];

    bool useAlphaTest;
    Color unpaintedColor;

    public override void Initialize(string extraData)
    {
        base.Initialize(extraData);

        string[] strs = extraData.Split(' ');

        string modelName = strs[0];
        useAlphaTest = bool.Parse(strs[1]);
        bool doubleSidedPlanks = bool.Parse(strs[2]);
        unpaintedColor = new Color32(byte.Parse(strs[3]), byte.Parse(strs[4]), byte.Parse(strs[5]), 255);

        MeshData post = new MeshData(BlockMeshes.FindMesh(modelName + "_Post"));
        post.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)));
        MeshData plank = new MeshData(BlockMeshes.FindMesh(modelName + "_Planks"));

        for (int i = 0; i < 16; i++)
        {
            MeshData data = MeshData.CreateEmpty();

            if ((i & 1) != 0)
            {
                MeshData data1 = plank.Clone();
                data1.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)) * Matrix4x4.Rotate(Quaternion.identity));
                data.Append(data1);
                if (doubleSidedPlanks)
                {
                    data1.FlipWindingOrder();
                    data.Append(data1);
                }
            }
            if ((i & 2) != 0)
            {
                MeshData data1 = plank.Clone();
                data1.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)) * Matrix4x4.Rotate(Quaternion.Euler(0, -180, 0)));
                data.Append(data1);
                if (doubleSidedPlanks)
                {
                    data1.FlipWindingOrder();
                    data.Append(data1);
                }
            }
            if ((i & 4) != 0)
            {
                MeshData data1 = plank.Clone();
                data1.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)) * Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0)));
                data.Append(data1);
                if (doubleSidedPlanks)
                {
                    data1.FlipWindingOrder();
                    data.Append(data1);
                }
            }
            if ((i & 8) != 0)
            {
                MeshData data1 = plank.Clone();
                data1.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)) * Matrix4x4.Rotate(Quaternion.Euler(0, 270, 0)));
                data.Append(data1);
                if (doubleSidedPlanks)
                {
                    data1.FlipWindingOrder();
                    data.Append(data1);
                }
            }

            blockMeshes[i] = post.Clone();
            blockMeshes[i].Append(data);

            paintedBlockMeshes[i] = post.Clone();
            paintedBlockMeshes[i].Append(data);

            if (useAlphaTest)
            {
                blockMeshes[i].WrapInTextureSlot(TextureSlot);
                paintedBlockMeshes[i].WrapInTextureSlot(BlocksData.paintedTextures[TextureSlot]);
            }
            else
            {
                blockMeshes[i].WrapInTextureSlotTerrain(TextureSlot);
                paintedBlockMeshes[i].WrapInTextureSlotTerrain(BlocksData.paintedTextures[TextureSlot]);
            }
        }
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        int data = BlockTerrain.GetData(value);
        int? i = GetColor(data);
        GreedyTerrainMesh terrainMesh = useAlphaTest ? g.AlphaTest : g.Terrain;
        if (i.HasValue)
            terrainMesh.Mesh(x, y, z, paintedBlockMeshes[GetVariant(data)], BlocksData.DEFAULT_COLORS[i.Value]);
        else
            terrainMesh.Mesh(x, y, z, blockMeshes[GetVariant(data)], unpaintedColor);
    }

    public static int GetVariant(int data)
    {
        return data & 0xF;
    }

    public int? GetColor(int data)
    {
        if ((data & 0x10) != 0)
        {
            return data >> 5 & 0xF;
        }
        return null;
    }
}
