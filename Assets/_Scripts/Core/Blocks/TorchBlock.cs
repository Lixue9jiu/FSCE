using UnityEngine;
using System.Collections;

public class TorchBlock : Block, INormalBlock
{
	MeshData[] meshes = new MeshData[5];
	MeshData test;

	public override void Initialize(string extraData)
	{
		MeshData mesh = new MeshData(BlockMeshes.FindMesh("Torch"));
        meshes[0] = mesh.Clone();
        meshes[0].Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 0, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, -0.05f)));
        meshes[1] = mesh.Clone();
        meshes[1].Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 90, 0)) * Matrix4x4.Translate(new Vector3(-0.05f, 0.15f, 0.5f)));
        meshes[2] = mesh.Clone();
        meshes[2].Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 180, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, 1.05f)));
        meshes[3] = mesh.Clone();
        meshes[3].Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 270, 0)) * Matrix4x4.Translate(new Vector3(1.05f, 0.15f, 0.5f)));
        meshes[4] = mesh.Clone();
        meshes[4].Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)));
	}
    
    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
	{
        g.AlphaTest.Mesh(x, y, z, meshes[BlockTerrain.GetData(value)], Color.white);
	}
}
