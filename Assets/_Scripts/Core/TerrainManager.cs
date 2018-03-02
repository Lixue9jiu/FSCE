using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

	TerrainReader129 terrainReader = new TerrainReader129 ();

	LinkedList<BlockTerrain.Chunk> dirty = new LinkedList<BlockTerrain.Chunk> ();

	public GameObject TerrainChunk;
	public BlockTerrain Terrain;

	TerrainGenerator terrainGenerator;

	void Awake ()
	{
		terrainGenerator = GetComponent<TerrainGenerator> ();
	}

	void Start ()
	{
//		AlaphaTest4 ();
//		AlaphaTest6 ();
	}

	public void AlaphaTest4 ()
	{
		LoadTerrain ("Assets/Resources/Chunks32.dat");

		BlockTerrain.Chunk[] c = terrainReader.AlaphaTest (GetComponent<BlockTerrain> ());
		for (int i = 0; i < c.Length; i++) {
			InstantiateChunk (c [i]);
		}
		Camera.main.transform.position = new Vector3 (c [0].chunkx << 4, 0, c [0].chunky << 4);
	}

	//	public void AlaphaTest5 ()
	//	{
	//		LoadTerrain ("Assets/Resources/Chunks32.dat");
	//
	//		TerrainGenerator g = GetComponent<TerrainGenerator> ();
	//		BlockTerrain.Chunk[] c = terrainReader.AlaphaTest (GetComponent<BlockTerrain> ());
	//
	//		BlockTerrain.Chunk chunk = c [0];
	//		GameObject obj = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity) as GameObject;
	//		Mesh mesh = obj.GetComponent<MeshFilter> ().mesh;
	//		g.MeshFromChunk (chunk, mesh, obj.GetComponentInChildren<MeshFilter> ().mesh);
	//
	//		long total = 0;
	//
	//		for (int i = 0; i < c.Length; i++) {
	//			System.Diagnostics.Stopwatch time = System.Diagnostics.Stopwatch.StartNew ();
	//			mesh = obj.GetComponent<MeshFilter> ().mesh;
	//			mesh.Clear ();
	//			g.MeshFromChunk (c [i], mesh, obj.GetComponentInChildren<MeshFilter> ().mesh);
	//			long t = time.ElapsedMilliseconds;
	//			total += t;
	//			Debug.Log (t);
	//			time.Stop ();
	//		}
	//
	//		Camera.main.transform.position = new Vector3 (c [0].chunkx << 4, 0, c [0].chunky << 4);
	//	}

	public void AlaphaTest6 ()
	{
		LoadTerrain (WorldManager.ChunkDat);

		BlockTerrain.Chunk[] c = terrainReader.AlaphaTest (GetComponent<BlockTerrain> ());
		for (int i = 0; i < c.Length; i++) {
			InstantiateChunk (c [i]);
		}
		Camera.main.transform.position = WorldManager.Project.PlayerPosition;
	}

	public void ChangeCell (int x, int y, int z, int newValue)
	{
		BlockTerrain.Chunk c = Terrain.GetChunk (x >> 4, z >> 4);
		if (c != null) {
			int cx = x & 15;
			int cz = z & 15;

			int content = c.GetCellContent (cx, y, cz);
			if (content != 0) {
				Block b = BlocksData.GetBlock (content);
				if (b.IsTransparent) {
					c.state += 2;
				} else {
					c.state += 1;
				}
			}
			content = newValue;
			if (content != 0) {
				Block b = BlocksData.GetBlock (content);
				if (b.IsTransparent) {
					c.state += 2;
				} else {
					c.state += 1;
				}
			}
			c.SetCellValue (cx, y, cz, newValue);
			if (cx == 0 && c.XminusOne != null) {
				c.XminusOne.state = 1;
				dirty.AddFirst (c.XminusOne);
			} else if (cx == 15 && c.XplusOne != null) {
				c.XplusOne.state = 1;
				dirty.AddFirst (c.XplusOne);
			}
			if (cz == 0 && c.YminusOne != null) {
				c.YminusOne.state = 1;
				dirty.AddFirst (c.YminusOne);
			} else if (cz == 15 && c.YplusOne != null) {
				c.YplusOne.state = 1;
				dirty.AddFirst (c.YplusOne);
			}
			dirty.AddFirst (c);
		}
	}

	void FixedUpdate ()
	{
		foreach (BlockTerrain.Chunk c in dirty) {
			UpdateChunk (c);
		}
		dirty.Clear ();
	}

	void LoadChunk (int x, int y)
	{

	}

	public void SaveAllChunks ()
	{
		terrainReader.SaveChunkEntries ();
		foreach (BlockTerrain.Chunk c in Terrain.Chunks) {
			terrainReader.SaveChunk (c);
		}
	}

	public void InstantiateChunk (BlockTerrain.Chunk chunk)
	{
		GameObject obj;
		GameObject obj2;
		if (chunk.instance == null) {
			obj = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity) as GameObject;
			chunk.instance = obj;
			Mesh m = new Mesh ();
			terrainGenerator.MeshFromTransparent (chunk, m);
			if (m.vertexCount != 0) {
				obj2 = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity);
				obj2.transform.parent = obj.transform;
				chunk.instance2 = obj2;

				obj2.GetComponent<MeshFilter> ().mesh = m;
			}
		} else {
			obj = chunk.instance;
			obj.transform.position = new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4);
			obj2 = chunk.instance2;
			if (obj2 != null) {
				terrainGenerator.MeshFromTransparent (chunk, obj2.GetComponent<MeshFilter> ().mesh);
			}
		}
		terrainGenerator.MeshFromChunk (chunk, obj.GetComponent<MeshFilter> ().mesh);

	}

	public void UpdateChunk (BlockTerrain.Chunk chunk)
	{
		if (chunk.instance != null) {
			switch (chunk.state) {
			case 3:
				terrainGenerator.MeshFromChunk (chunk, chunk.instance.GetComponent<MeshFilter> ().mesh);
				terrainGenerator.MeshFromTransparent (chunk, chunk.instance2.GetComponent<MeshFilter> ().mesh);
				break;
			case 1:
				terrainGenerator.MeshFromChunk (chunk, chunk.instance.GetComponent<MeshFilter> ().mesh);
				break;
			case 2:
				terrainGenerator.MeshFromTransparent (chunk, chunk.instance2.GetComponent<MeshFilter> ().mesh);
				break;
			}
			chunk.state = 0;
		}
	}

	void OnDestory ()
	{
		terrainReader.Destory ();
	}

	void LoadTerrain (string path)
	{
		Stream stream = File.Open (path, FileMode.Open, FileAccess.ReadWrite);
		terrainReader.Load (stream);
	}

	void UpdateTerrain (int centerChunkX, int centerChunkY)
	{

	}

	//	class Chunk
	//	{
	//		public GameObject obj;
	//		public ChunkState chunkState;
	//	}
	//
	//	enum ChunkState
	//	{
	//		Undefined,
	//		Dirty,
	//		Good
	//	}
}
