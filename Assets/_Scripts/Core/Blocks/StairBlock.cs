using UnityEngine;
using System.Collections;

public class StairBlock : Block, INormalBlock, IPaintableBlock
{
    MeshData[] blockMeshes = new MeshData[24];
    MeshData[] paintedBlockMeshes = new MeshData[24];

	public override void Initialize(string extraData)
	{
		base.Initialize(extraData);
        //IsCubic = false;

        Matrix4x4 matrix = Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f));

		float y;
		Matrix4x4 m;
		Mesh mesh;
		for (int i = 0; i < 24; i++)
		{
			y = 0;

			y -= GetRotation(i) * 90;

            m = Matrix4x4.Rotate(Quaternion.Euler(0, y, 0));
			switch ((i >> 3) & 3)
			{
				case 1:
					mesh = BlockMeshes.FindMesh("Stair0");
					break;
				case 0:
					mesh = BlockMeshes.FindMesh("Stair1");
					break;
				case 2:
					mesh = BlockMeshes.FindMesh("Stair2");
					break;
				default:
					throw new System.Exception("unknown stair module: " + ((i >> 3) & 3));
			}

            blockMeshes[i] = new MeshData(mesh);
            blockMeshes[i].Transform(matrix * m);
            if ((i & 4) != 0)
            {
                blockMeshes[i].FlipVertical();
            }
            paintedBlockMeshes[i] = blockMeshes[i].Clone();

            blockMeshes[i].WrapInTextureSlotTerrain(TextureSlot);
            paintedBlockMeshes[i].WrapInTextureSlotTerrain(BlocksData.paintedTextures[TextureSlot]);
		}
	}

	public static int GetRotation(int data)
	{
		return data & 3;
	}

	public int? GetColor(int data)
	{
		if ((data & 32) != 0)
		{
			return new int?(data >> 6 & 15);
		}
		return null;
	}

	public static int GetVariant(int data)
	{
		return data & 31;
	}

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
	{
		int? color = GetColor(BlockTerrain.GetData(value));
        if (color.HasValue)
            g.Terrain.Mesh(x, y, z, paintedBlockMeshes[GetVariant(BlockTerrain.GetData(value))], BlocksData.DEFAULT_COLORS[color.Value]);
        else
            g.Terrain.Mesh(x, y, z, blockMeshes[GetVariant(BlockTerrain.GetData(value))], Color.white);
	}
}
