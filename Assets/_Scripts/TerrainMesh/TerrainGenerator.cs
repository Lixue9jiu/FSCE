using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

	List<Vector3> vertices = new List<Vector3> ();
	List<int> triangles = new List<int> ();
	List<Vector2> uvs = new List<Vector2> ();
	List<Color> colors = new List<Color> ();

	public void MeshFromChunk (BlockTerrain.Chunk chunk, out MeshData mesh)
	{
		for (int x = 0; x < chunk.sizeX; x++) {
			for (int y = 0; y < chunk.sizeY; y++) {
				for (int z = 0; z < chunk.sizeZ; z++) {
					int value = chunk.GetCellValue (x, y, z);
					int content = BlockTerrain.GetContent (value);
					Block b = BlocksData.GetBlock (content);
                    if (!b.IsTransparent) {
						b.GenerateTerrain (x, y, z, value, chunk, this);
					}
				}
			}
		}

		PushToMesh (out mesh);
	}

	public void MeshFromChunk (BlockTerrain.Chunk chunk, Mesh mesh)
	{
		MeshData data;
		MeshFromChunk (chunk, out data);
		data.ToMesh (mesh);
	}

	public void MeshFromTransparent (BlockTerrain.Chunk chunk, out MeshData mesh)
	{
		for (int x = 0; x < chunk.sizeX; x++) {
			for (int y = 0; y < chunk.sizeY; y++) {
				for (int z = 0; z < chunk.sizeZ; z++) {
					int value = chunk.GetCellValue (x, y, z);
					int content = BlockTerrain.GetContent (value);
					Block b = BlocksData.GetBlock (content);
                    if (b.IsTransparent) {
						b.GenerateTerrain (x, y, z, value, chunk, this);
					}
				}
			}
		}

		PushToMesh (out mesh);
	}

	public void MeshFromTransparent (BlockTerrain.Chunk chunk, Mesh mesh)
	{
		MeshData data;
		MeshFromTransparent (chunk, out data);
		data.ToMesh (mesh);
	}

	public void MeshFromFurniture (FurnitureManager.Furniture furniture, out Mesh mesh)
	{
		int res = furniture.Resolution;
		mesh = new Mesh ();
		for (int x = 0; x < res; x++) {
			for (int y = 0; y < res; y++) {
				for (int z = 0; z < res; z++) {
					int value = furniture.GetCellValue (x, y, z);
					int content = BlockTerrain.GetContent (value);
					if (content != 0) {
						int neighborData = 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x - 1, y, z))).IsTransparent) ? Block.XminusOne : 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x, y - 1, z))).IsTransparent) ? Block.YminusOne : 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x, y, z - 1))).IsTransparent) ? Block.ZminusOne : 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x + 1, y, z))).IsTransparent) ? Block.XplusOne : 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x, y + 1, z))).IsTransparent) ? Block.YplusOne : 0;
						neighborData += (BlocksData.GetBlock (BlockTerrain.GetContent (furniture.GetCellValue (x, y, z + 1))).IsTransparent) ? Block.ZplusOne : 0;
						GenerateFurnitureBlcok (res, x, y, z, content, value, neighborData);
					}
				}
			}
		}

		PushToMesh (mesh);
	}

	void GenerateFurnitureBlcok (int resolution, int x, int y, int z, int content, int value, int neighborData)
	{
		float blockSize = 1 / ((float)resolution);
		Vector3 v000 = new Vector3 (x, y, z);
		Vector3 v001 = new Vector3 (x, y, z + 1.0f);
		Vector3 v010 = new Vector3 (x, y + 1.0f, z);
		Vector3 v011 = new Vector3 (x, y + 1.0f, z + 1.0f);
		Vector3 v100 = new Vector3 (x + 1.0f, y, z);
		Vector3 v101 = new Vector3 (x + 1.0f, y, z + 1.0f);
		Vector3 v110 = new Vector3 (x + 1.0f, y + 1.0f, z);
		Vector3 v111 = new Vector3 (x + 1.0f, y + 1.0f, z + 1.0f);

		v000 *= blockSize;
		v001 *= blockSize;
		v010 *= blockSize;
		v011 *= blockSize;
		v100 *= blockSize;
		v101 *= blockSize;
		v110 *= blockSize;
		v111 *= blockSize;

		blockSize = 0.0625f / resolution;

		Block b = BlocksData.GetBlock (content);
		Color color = BlocksData.GetBlockColor (b, value);

		if ((neighborData & Block.XminusOne) == Block.XminusOne) {
			MeshFromRect (v001, v011, v010, v000);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (value, Block.BACK), z, y, blockSize);
			GenerateBlockColors (color);
		}
		if ((neighborData & Block.YminusOne) == Block.YminusOne) {
			MeshFromRect (v000, v100, v101, v001);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (content, Block.BOTTOM), z, x, blockSize);
			GenerateBlockColors (color);
		}
		if ((neighborData & Block.ZminusOne) == Block.ZminusOne) {
			MeshFromRect (v000, v010, v110, v100);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (content, Block.LEFT), x, y, blockSize);
			GenerateBlockColors (color);
		}
		if ((neighborData & Block.XplusOne) == Block.XplusOne) {
			MeshFromRect (v100, v110, v111, v101);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (content, Block.FRONT), z, y, blockSize);
			GenerateBlockColors (color);
		}
		if ((neighborData & Block.YplusOne) == Block.YplusOne) {
			MeshFromRect (v111, v110, v010, v011);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (content, Block.TOP), z, x, blockSize);
			GenerateBlockColors (color);
		}
		if ((neighborData & Block.ZplusOne) == Block.ZplusOne) {
			MeshFromRect (v101, v111, v011, v001);
			GenerateFurnitureBlockUVs (b.GetTextureSlot (content, Block.RIGHT), x, y, blockSize);
			GenerateBlockColors (color);
		}
	}

	void GenerateFurnitureBlockUVs (int slotIndex, int x, int y, float blockSize)
	{
		float v = (float)(slotIndex % 16) / 16;
		float u = (float)(15 - (slotIndex / 16)) / 16;

		Vector2 v00 = new Vector2 (v, u);
		Vector2 v01 = new Vector2 (v, u + blockSize);
		Vector2 v10 = new Vector2 (v + blockSize, u);
		Vector2 v11 = new Vector2 (v + blockSize, u + blockSize);

		v00.x += x * blockSize;
		v00.y += y * blockSize;
		v01.x += x * blockSize;
		v01.y += y * blockSize;
		v10.x += x * blockSize;
		v10.y += y * blockSize;
		v11.x += x * blockSize;
		v11.y += y * blockSize;

		uvs.Add (v00);
		uvs.Add (v01);
		uvs.Add (v11);
		uvs.Add (v10);
	}

	public void PushToMesh (Mesh mesh)
	{
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.uv = uvs.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.RecalculateNormals ();

		vertices.Clear ();
		triangles.Clear ();
		uvs.Clear ();
		colors.Clear ();
	}

	public void PushToMesh (out MeshData mesh)
	{
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.uv = uvs.ToArray ();
		mesh.colors = colors.ToArray ();

		vertices.Clear ();
		triangles.Clear ();
		uvs.Clear ();
		colors.Clear ();
	}

	public void MeshFromMeshRaw (int x, int y, int z, MeshData mesh)
	{
		MeshFromMesh (x, y, z, mesh);
		uvs.AddRange (mesh.uv);
		colors.AddRange (mesh.colors);
	}

	public void MeshFromMesh (int x, int y, int z, MeshData mesh)
	{
		Vector3 tran = new Vector3 (x, y, z);
		int count = vertices.Count;
		for (int i = 0; i < mesh.vertices.Length; i++) {
			vertices.Add (mesh.vertices [i] + tran);
		}
		for (int i = 0; i < mesh.triangles.Length; i++) {
			triangles.Add (mesh.triangles [i] + count);
		}
	}

	public void MeshFromRect (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		int count = vertices.Count;
		vertices.Add (a);
		vertices.Add (b);
		vertices.Add (c);
		vertices.Add (d);

		triangles.Add (count);
		triangles.Add (count + 1);
		triangles.Add (count + 2);
		triangles.Add (count + 2);
		triangles.Add (count + 3);
		triangles.Add (count);
	}

	public void GenerateBlockUVs (int slotIndex)
	{
		float v = slotIndex % 16;
		float u = 15 - (slotIndex / 16);

		Vector2 v00 = new Vector2 (v / 16, u / 16);
		Vector2 v01 = new Vector2 (v / 16, u / 16 + 0.0625f);
		Vector2 v10 = new Vector2 (v / 16 + 0.0625f, u / 16);
		Vector2 v11 = new Vector2 (v / 16 + 0.0625f, u / 16 + 0.0625f);

		uvs.Add (v00);
		uvs.Add (v01);
		uvs.Add (v11);
		uvs.Add (v10);
	}

	public void GenerateBlockColors (Color color)
	{
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
	}

	public void GenerateTextureForMesh (MeshData mesh, int textureSlot, Color color)
	{
		float v = (float)(textureSlot % 16) / 16;
		float u = (float)(15 - (textureSlot / 16)) / 16;

		Vector2 vec = new Vector2 (v, u);
		Vector2[] uv = mesh.uv;

		for (int i = 0; i < uv.Length; i++) {
			uvs.Add (vec + uv [i]);
			colors.Add (color);
		}
	}

}
