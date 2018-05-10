using UnityEngine;
using System.Collections;

public class NStairBlock : Block
{
	MeshData[] stairs = new MeshData[24];

    public override void Initialize()
    {
        base.Initialize();
        IsTransparent = true;
        //IsCubic = false;

        float y;
        Matrix4x4 m;
        Mesh mesh;
        for (int i = 0; i < 24; i++)
        {
            y = 0;

            y -= GetRotation(i) * 90;

            m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, y, 0), Vector3.one);
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

            stairs[i] = new MeshData(BlockMeshes.TranslateMesh(mesh, m, (i & 4) != 0));
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

	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
		int? color = GetColor(BlockTerrain.GetData(value));
		terrainMesh.AlphaTest.Mesh(x, y, z,
		                           stairs[GetVariant(BlockTerrain.GetData(value))],
                                   color.HasValue ? BlocksData.paintedTextures[TextureSlot] : TextureSlot,
                                   BlocksData.ColorFromInt(color));
	}
}
