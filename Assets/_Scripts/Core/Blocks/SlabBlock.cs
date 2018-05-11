using UnityEngine;
using System.Collections;

public class SlabBlock : Block, INormalBlock, IPaintableBlock
{
	MeshData[] slabs = new MeshData[2];

    public override void Initialize()
    {
        base.Initialize();
        //IsCubic = false;

        Mesh slab = BlockMeshes.FindMesh("Slab");

        slabs[0] = new MeshData(slab);
        slabs[1] = new MeshData(BlockMeshes.UpsideDownMesh(slab));
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

	public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainMesh terrainMesh)
	{
		int? color = GetColor(BlockTerrain.GetData(value));
		terrainMesh.Mesh(x, y, z, 
		                           slabs[GetIsTop(BlockTerrain.GetData(value)) ? 1 : 0],
		                           color.HasValue ? BlocksData.paintedTextures[TextureSlot] : TextureSlot,
		                           BlocksData.ColorFromInt(color));
	}
}
