using UnityEngine;
using System.Collections;

public class SlabBlock : Block, INormalBlock, IPaintableBlock
{
    MeshData[] blockMeshes = new MeshData[2];
    MeshData[] paintedBlockMeshes = new MeshData[2];

    public override void Initialize(string extraData)
    {
        base.Initialize(extraData);
        //IsCubic = false;

        Mesh slab = BlockMeshes.FindMesh("Slab");
        Matrix4x4 matrix = Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f));

        blockMeshes[0] = new MeshData(slab);
        blockMeshes[0].Transform(matrix);

        blockMeshes[1] = new MeshData(slab);
        blockMeshes[1].Transform(matrix);
        blockMeshes[1].FlipVertical();

        paintedBlockMeshes[0] = blockMeshes[0].Clone();
        paintedBlockMeshes[1] = blockMeshes[1].Clone();

        blockMeshes[0].WrapInTextureSlotTerrain(TextureSlot);
        blockMeshes[1].WrapInTextureSlotTerrain(TextureSlot);
        paintedBlockMeshes[0].WrapInTextureSlotTerrain(BlocksData.paintedTextures[TextureSlot]);
        paintedBlockMeshes[1].WrapInTextureSlotTerrain(BlocksData.paintedTextures[TextureSlot]);
    }

    public int? GetColor(int data)
    {
        if ((data & 2) != 0)
        {
            return new int?(data >> 2 & 15);
        }
        return null;
    }

    public static bool GetIsTop(int data)
    {
        return (data & 1) != 0;
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        int? color = GetColor(BlockTerrain.GetData(value));
        if (color.HasValue)
            g.Terrain.Mesh(x, y, z, paintedBlockMeshes[GetIsTop(BlockTerrain.GetData(value)) ? 1 : 0], BlocksData.DEFAULT_COLORS[color.Value]);
        else
            g.Terrain.Mesh(x, y, z, blockMeshes[GetIsTop(BlockTerrain.GetData(value)) ? 1 : 0], Color.white);
    }
}
