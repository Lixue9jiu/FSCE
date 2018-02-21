using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

	TerrainReader129 terrainReader = new TerrainReader129 ();

	public GameObject TerrainChunk;

	void Start ()
	{
		AlaphaTest4 ();
	}

	public void AlaphaTest4()
	{
		LoadTerrain ("Assets/Resources/Chunks32.dat");

		TerrainGenerator g = GetComponent<TerrainGenerator> ();
		BlockTerrain.Chunk[] c = terrainReader.AlaphaTest (GetComponent<BlockTerrain> ());
		for (int i = 0; i < c.Length; i++) {
			BlockTerrain.Chunk chunk = c [i];
			GameObject obj = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity) as GameObject;
			Mesh mesh = obj.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			g.MeshFromChunk (chunk, ref mesh);
		}
		Camera.main.transform.position = new Vector3 (c [0].chunkx << 4, 0, c [0].chunky << 4);
	}

//	public void InstantiateChunk (BlockTerrain.Chunk chunk)
//	{
//		GameObject obj = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity) as GameObject;
//		obj.GetComponent<TerrainGenerator> ().MeshFromChunk (chunk);
//		obj.transform.localScale = Vector3.one;
//	}

	void OnDestory ()
	{
		terrainReader.Destory ();
	}

	void LoadTerrain (string path)
	{
		Stream stream = File.OpenRead (path);
		terrainReader.Load (stream);
	}
}
