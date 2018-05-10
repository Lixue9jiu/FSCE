using UnityEngine;
using System.Collections;

public class NTorchBlock : Block
{
	MeshData[] meshes = new MeshData[5];

	public override void Initialize()
	{
		IsTransparent = true;
		MeshData mesh = new MeshData(BlockMeshes.FindMesh("Torch"));
		meshes[0] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 0, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, -0.05f)));
		meshes[1] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 90, 0)) * Matrix4x4.Translate(new Vector3(-0.05f, 0.15f, 0.5f)));
		meshes[2] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 180, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, 1.05f)));
		meshes[3] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 270, 0)) * Matrix4x4.Translate(new Vector3(1.05f, 0.15f, 0.5f)));
		meshes[4] = mesh.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)));
	}

	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
		terrainMesh.AlphaTest.Mesh(x, y, z, meshes[BlockTerrain.GetData(value)], Color.white);
	}
}
