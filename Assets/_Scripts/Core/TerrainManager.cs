using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

	public int chunkLoadDistance = 8;
	public int chunkDiscardDistance = 9;

	TerrainReader129 terrainReader = new TerrainReader129 ();

	LinkedList<BlockTerrain.Chunk> dirty = new LinkedList<BlockTerrain.Chunk> ();

	public GameObject TerrainChunk;
	public BlockTerrain Terrain;

	TerrainGenerator terrainGenerator;

	Thread thread;
	bool threadRunning = true;

	void Awake ()
	{
		terrainGenerator = GetComponent<TerrainGenerator> ();
	}

	void Start ()
	{
//		AlaphaTest4 ();
//		AlaphaTest6 ();
		GetComponent <FurnitureManager> ().AlaphaTest4 ();
//		AlaphaTest9 ();

		thread = new Thread (new ThreadStart (delegate {
			while (threadRunning) {
				if (needTerrainUpdate) {
					Debug.Log (string.Format ("start: {0}", centerChunk));
					try {
						UpdateTerrain (centerChunk.X, centerChunk.Y);
					} catch (System.Exception e) {
						Debug.Log ("error updating terrain " + e.Message);
					}
					Debug.Log ("done");

					needTerrainUpdate = false;
				}
			}
			Debug.Log ("chunk loading thread closed");
		}));
		thread.Start ();
	}

	void OnDisable ()
	{
		threadRunning = false;
		thread.Join ();
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

	public void AlaphaTest7 ()
	{
		Point3 p = TerrainRaycast.ToCell (Camera.main.transform.position);
		UpdateTerrain (p.X >> 4, p.Z >> 4);
	}

	public void AlaphaTest8 ()
	{
		Point2 p = CurrentChunk ();
//		Terrain.chunkStats.Get (p.X, p.Y).Loaded = true;
		DiscardChunk (p.X, p.Y);
	}

	public void AlaphaTest9 ()
	{
		int x = 9;
		int y = 10;
		Terrain.chunkStats.Get (x, y).Loaded = true;
		Terrain.chunkStats.GetOriginal (ref x, ref y);
		Debug.Log (Terrain.chunkStats.Get (x, y).Loaded);
	}

	public void Load ()
	{
		LoadTerrain (WorldManager.ChunkDat);
		Camera.main.transform.position = WorldManager.Project.PlayerPosition;

		Point3 p = TerrainRaycast.ToCell (WorldManager.Project.PlayerPosition);
		int startx = (p.X >> 4) - 4;
		int starty = (p.Z >> 4) - 4;
		Terrain.chunkStats.SetOffset (startx, starty);

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				LoadChunk (startx + x, starty + y);
			}
		}

		centerChunk = CurrentChunk ();
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

	Point2 centerChunk;
	bool needTerrainUpdate;

	void FixedUpdate ()
	{
		LinkedList<BlockTerrain.Chunk> copy;
		lock (dirty) {
			copy = new LinkedList<BlockTerrain.Chunk> (dirty);
		}
		foreach (BlockTerrain.Chunk c in copy) {
			UpdateChunk (c);
		}
		lock (dirty) {
			for (int i = 0; i < copy.Count; i++) {
				dirty.RemoveLast ();
			}
		}

		Point2 p = CurrentChunk ();
		if (!p.Equals (centerChunk)) {
			Debug.Log ("need terrain update: " + needTerrainUpdate);
			centerChunk = p;
			needTerrainUpdate = true;
		}
	}

	void LoadChunk (int x, int y)
	{
		if (terrainReader.ChunkExist (x, y)) {
			Debug.Log (string.Format ("loading chunk at {0}, {1}", x, y));
			BlockTerrain.Chunk c = terrainReader.ReadChunk (x, y, Terrain);
			terrainGenerator.MeshFromChunk (c, out c.mesh);
			terrainGenerator.MeshFromTransparent (c, out c.mesh2);
			c.state = 4;
			lock (dirty) {
				dirty.AddFirst (c);
			}
		}
	}

	void DiscardChunk (int x, int y)
	{
		Debug.Log (string.Format ("discarding chunk at {0}, {1}", x, y));
		BlockTerrain.Chunk c = Terrain.DiscardChunk (x, y);
		if (c != null) {
			terrainReader.SaveChunk (c);
			c.state = 5;
			lock (dirty) {
				dirty.AddFirst (c);
			}
		}
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
//			Mesh m = new Mesh ();
//			terrainGenerator.MeshFromTransparent (chunk, m);
			obj2 = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity);
			obj2.transform.parent = obj.transform;
			chunk.instance2 = obj2;

			chunk.mesh2.ToMesh (obj2.GetComponent<MeshFilter> ().mesh);
		} else {
			obj = chunk.instance;
			obj.transform.position = new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4);
			chunk.mesh2.ToMesh (chunk.instance2.GetComponent <MeshFilter> ().mesh);
//			terrainGenerator.MeshFromTransparent (chunk, obj2.GetComponent<MeshFilter> ().mesh);
		}
//		terrainGenerator.MeshFromChunk (chunk, obj.GetComponent<MeshFilter> ().mesh);
		chunk.mesh.ToMesh (obj.GetComponent<MeshFilter> ().mesh);

	}

	public void UpdateChunk (BlockTerrain.Chunk chunk)
	{
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
		case 4:
			InstantiateChunk (chunk);
			break;
		case 5:
//			GameObject.Destroy (chunk.instance);
//			GameObject.Destroy (chunk.instance2);
//
//			chunk.instance = null;
//			chunk.instance2 = null;
			break;
		}
		chunk.state = 0;
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
		centerChunkX -= 4;
		centerChunkY -= 4;

		BlockTerrain.ChunkStats chunkStats = Terrain.chunkStats;

		int startx;
		int starty;
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				startx = centerChunkX + x;
				starty = centerChunkY + y;

				if (!chunkStats.IsValid (startx, starty)) {
					if (chunkStats.Get (startx, starty).Loaded) {
						chunkStats.GetOriginal (ref startx, ref starty);
						DiscardChunk (startx, starty);
//						Debug.Log ("is loaded " + Terrain.chunkStats.Get (startx, starty).Loaded);
					}
//					Debug.Log ("is loaded " + Terrain.chunkStats.Get (centerChunkX + x, centerChunkY + y).Loaded);
					LoadChunk (centerChunkX + x, centerChunkY + y);
				}
			}
		}

		chunkStats.SetOffset (centerChunkX, centerChunkY);
	}

	public static Point2 CurrentChunk ()
	{
		Point3 p = TerrainRaycast.ToCell (Camera.main.transform.position);
		return ToChunk (p.X, p.Z);
	}

	public static Point2 ToChunk (int x, int z)
	{
		return new Point2 (x >> 4, z >> 4);
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

	//	void OnDrawGizmos ()
	//	{
	//		Gizmos.color = Color.black;
	////		Point3 p = TerrainRaycast.ToCell (Camera.main.transform.position);
	//		BlockTerrain.ChunkStats stats = Terrain.chunkStats;
	//		int startx = stats.offsetX;
	//		int starty = stats.offsetY;
	////		Terrain.chunkStats.SetOffset (startx, starty);
	//
	//		int cx;
	//		int cy;
	//		for (int x = 0; x < 8; x++) {
	//			for (int y = 0; y < 8; y++) {
	//				cx = startx + x;
	//				cy = starty + y;
	//
	//				if (stats.Get (cx, cy).Loaded) {
	//					DrawChunk (cx, cy);
	//				}
	//			}
	//		}
	//	}
	//
	//	void DrawChunk (int x, int y)
	//	{
	//		Gizmos.DrawCube (new Vector3 ((x << 4) + 8, 0, (y << 4) + 8), Vector3.one * 16f);
	//	}
}
