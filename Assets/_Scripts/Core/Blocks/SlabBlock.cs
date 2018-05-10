using UnityEngine;
using System.Collections;

public class SlabBlock : Block
{
	MeshData[] slabs = new MeshData[2];

    public override void Initialize()
    {
        base.Initialize();
        IsTransparent = true;
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

	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
		int? color = GetColor(BlockTerrain.GetData(value));
		terrainMesh.AlphaTest.Mesh(x, y, z, 
		                           slabs[GetIsTop(BlockTerrain.GetData(value)) ? 1 : 0],
		                           color.HasValue ? BlocksData.paintedTextures[TextureSlot] : TextureSlot,
		                           BlocksData.ColorFromInt(color));
	}
}
