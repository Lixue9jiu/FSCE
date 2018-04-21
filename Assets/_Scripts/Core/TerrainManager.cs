using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    const int updateMili = 200;

    public int chunkLoadDistance = 8;
    public int chunkDiscardDistance = 9;

    TerrainReader129 terrainReader = new TerrainReader129();

    ChunkQueue chunkQueue = new ChunkQueue();

    public GameObject TerrainChunk;
    public BlockTerrain Terrain;
    public ChunkRenderer chunkRenderer;

    public TerrainGenerator terrainGenerator;

    Thread thread;
    bool threadRunning = true;

    void Start()
    {
        //AlaphaTest4 ();
        //AlaphaTest6 ();
        Load();
        //GetComponent<FurnitureManager>().AlaphaTest4();
        //		AlaphaTest9 ();

        StartChunkUpdateThread();
    }

    //System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

    void StartChunkUpdateThread()
    {
        thread = new Thread(new ThreadStart(ChunkUpdateWork));
        thread.Start();
    }

    void ChunkUpdateWork()
    {
        Debug.Log("starting terrain updating thread");
        while (threadRunning)
        {
            try
            {
                //Stopwatch.Reset();
                //Stopwatch.Start();
                if (chunkQueue.TerrainCount != 0)
                {
                    int count = chunkQueue.TerrainCount;
                    for (int i = 0; i < count; i++)
                    {
                        int index = chunkQueue.PopTerrain();
                        BlockTerrain.Chunk chunk = Terrain.GetChunk(index);
                        if (chunk == null)
                            continue;
                        lock (chunk)
                        {
                            switch (Terrain.chunkStats.Get(index).state)
                            {
                                case 3:
                                    terrainGenerator.MeshFromChunk(chunk, out chunk.mesh[0]);
                                    terrainGenerator.MeshFromTransparent(chunk, out chunk.mesh[1]);
                                    break;
                                case 1:
                                    terrainGenerator.MeshFromChunk(chunk, out chunk.mesh[0]);
                                    break;
                                case 2:
                                    terrainGenerator.MeshFromTransparent(chunk, out chunk.mesh[1]);
                                    break;
                            }
                        }
                        chunkQueue.AddMain(index);
                    }
                }
                if (needTerrainUpdate)
                {
                    UpdateTerrain(centerChunk.X, centerChunk.Y);

                    needTerrainUpdate = false;
                }
                //if (Stopwatch.ElapsedMilliseconds < updateMili)
                //{
                //    Thread.Sleep(updateMili - (int)Stopwatch.ElapsedMilliseconds);
                //}
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        Debug.Log("chunk loading thread closed");
    }

    void OnDisable()
    {
        try
        {
            threadRunning = false;
            thread.Join();
        }
        finally
        {
            SaveAllChunks();
            terrainReader.Destory();
        }
    }

    public void AlaphaTest4()
    {
        LoadTerrain("Assets/Resources/Chunks32.dat");

        BlockTerrain.Chunk[] c = terrainReader.AlaphaTest(GetComponent<BlockTerrain>());
        for (int i = 0; i < c.Length; i++)
        {
            InstantiateChunk(c[i]);
        }
        Camera.main.transform.position = new Vector3(c[0].chunkx << 4, 0, c[0].chunky << 4);
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

    public void AlaphaTest6()
    {
        LoadTerrain(WorldManager.ChunkDat);

        BlockTerrain.Chunk[] c = terrainReader.AlaphaTest(GetComponent<BlockTerrain>());
        for (int i = 0; i < c.Length; i++)
        {
            InstantiateChunk(c[i]);
        }
        Camera.main.transform.position = WorldManager.Project.PlayerPosition;
    }

    public void AlaphaTest7()
    {
        Point3 p = TerrainRaycast.ToCell(Camera.main.transform.position);
        UpdateTerrain(p.X >> 4, p.Z >> 4);
    }

    public void AlaphaTest8()
    {
        Point2 p = CurrentChunk();
        //		Terrain.chunkStats.Get (p.X, p.Y).Loaded = true;
        DiscardChunk(p.X, p.Y);
    }

    public void AlaphaTest9()
    {
        int x = 9;
        int y = 10;
        Terrain.chunkStats.Get(x, y).Loaded = true;
        Terrain.chunkStats.GetOriginal(ref x, ref y);
        Debug.Log(Terrain.chunkStats.Get(x, y).Loaded);
    }

    //public void AlaphaTest10()
    //{
    //    int chunkx = 1;
    //    int chunky = 1;
    //}

    public void Load()
    {
        LoadTerrain(WorldManager.ChunkDat);
        Camera.main.transform.position = WorldManager.Project.PlayerPosition;

        Point3 p = TerrainRaycast.ToCell(WorldManager.Project.PlayerPosition);
        int startx = (p.X >> 4) - Terrain.chunkStats.halfSize;
        int starty = (p.Z >> 4) - Terrain.chunkStats.halfSize;
        Terrain.chunkStats.SetOffset(startx, starty);

        int size = Terrain.chunkStats.size;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                LoadChunk(startx + x, starty + y);
                Terrain.chunkStats.Get(startx + x, starty + y).state = 0;
            }
        }

        centerChunk = CurrentChunk();

        needTerrainUpdate = true;
    }

    public void ChangeCell(int x, int y, int z, int newValue)
    {
        int index = Terrain.GetChunkIndex(x >> 4, z >> 4);
        BlockTerrain.Chunk c = Terrain.GetChunk(index);
        BlockTerrain.ChunkStatics state = Terrain.chunkStats.Get(index);
        if (c != null)
        {
            int cx = x & 15;
            int cz = z & 15;

            int content = c.GetCellContent(cx, y, cz);
            if (content != 0)
            {
                Block b = BlocksData.GetBlock(content);
                if (b.IsTransparent)
                {
                    state.state |= 2;
                }
                else
                {
                    state.state |= 1;
                }
            }
            content = newValue;
            if (content != 0)
            {
                Block b = BlocksData.GetBlock(content);
                if (b.IsTransparent)
                {
                    state.state |= 2;
                }
                else
                {
                    state.state |= 1;
                }
            }
            c.SetCellValue(cx, y, cz, newValue);
            if (cx == 0 && c.XminusOne != null)
            {
                QuqueChunkUpdate(c.XminusOne.index, 3);
            }
            else if (cx == 15 && c.XplusOne != null)
            {
                QuqueChunkUpdate(c.XplusOne.index, 3);
            }
            if (cz == 0 && c.YminusOne != null)
            {
                QuqueChunkUpdate(c.YminusOne.index, 3);
            }
            else if (cz == 15 && c.YplusOne != null)
            {
                QuqueChunkUpdate(c.YplusOne.index, 3);
            }
            QuqueChunkUpdate(c.index);
        }
    }

    Point2 centerChunk;
    bool needTerrainUpdate;

    void FixedUpdate()
    {
        int count = chunkQueue.MainCount;
        for (int i = 0; i < count; i++)
        {
            UpdateChunk(chunkQueue.PopMain());
        }

        Point2 p = CurrentChunk();
        if (!p.Equals(centerChunk))
        {
            centerChunk = p;
            needTerrainUpdate = true;
        }
    }

    void LoadChunk(int x, int y)
    {
        BlockTerrain.Chunk c;
        try
        {
            if (terrainReader.ChunkExist(x, y))
            {
                //			Debug.Log (string.Format ("loading chunk at {0}, {1}", x, y));
                c = terrainReader.ReadChunk(x, y, Terrain);
                terrainGenerator.MeshFromChunk(c, out c.mesh[0]);
                terrainGenerator.MeshFromTransparent(c, out c.mesh[1]);
                Terrain.chunkStats.Get(c.index).updateState = BlockTerrain.ChunkUpdateState.NeedsToBeCreated;
                chunkQueue.AddMain(c.index);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("error loading chunk: " + e.Message + e.StackTrace);
            chunkQueue.PopMain();
            Terrain.DiscardChunk(x, y);
            //			chunkQueue.Clean ();
        }
    }

    void DiscardChunk(int x, int y)
    {
        //Debug.Log(string.Format("discarding chunk at {0}, {1}", x, y));
        BlockTerrain.Chunk c;
        c = Terrain.DiscardChunk(x, y);
        if (c != null)
        {
            terrainReader.SaveChunk(c);
            Terrain.chunkStats.Get(c.index).updateState = BlockTerrain.ChunkUpdateState.NeedsToBeDestoryed;
            chunkQueue.AddMain(c.index);
        }
    }

    public void SaveAllChunks()
    {
        terrainReader.SaveChunkEntries();
        foreach (BlockTerrain.Chunk c in Terrain.Chunks)
        {
            if (c != null)
                terrainReader.SaveChunk(c);
        }
    }

    public void InstantiateChunk(BlockTerrain.Chunk c)
    {
        int index = Terrain.GetChunkIndex(c.chunkx, c.chunky);
        ChunkInstance instance = Terrain.chunkInstances[index];
        if (instance == null)
        {
            instance = new ChunkInstance();
            Terrain.chunkInstances[index] = instance;
        }
        instance.UpdateAll(c);
        instance.UpdateMesh(0);
        instance.UpdateMesh(1);
        chunkRenderer.AddChunk(index);

        //GameObject obj;
        //GameObject obj2;
        //if (chunk.instance == null) {
        //	obj = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity) as GameObject;
        //	chunk.instance = obj;
        //	obj2 = Instantiate (TerrainChunk, new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4), Quaternion.identity);
        //	obj2.transform.parent = obj.transform;
        //	chunk.instance2 = obj2;
        //} else {
        //	obj = chunk.instance;
        //	obj.transform.position = new Vector3 (chunk.chunkx << 4, 0, chunk.chunky << 4);
        //          obj2 = chunk.instance2;
        //	chunk.instance.SetActive (true);
        //	chunk.instance2.SetActive (true);
        //}
        //chunk.mesh.ToMesh (obj.GetComponent<MeshFilter> ().mesh);
        //chunk.mesh2.ToMesh(obj2.GetComponent<MeshFilter>().mesh);
    }

    public void UpdateChunk(int index)
    {
        BlockTerrain.ChunkStatics statics = Terrain.chunkStats.Get(index);

        switch (statics.updateState)
        {
            case BlockTerrain.ChunkUpdateState.NeedsToBeCreated:
                BlockTerrain.Chunk chunk = Terrain.GetChunk(index);
                InstantiateChunk(chunk);
                if (chunk.XminusOne != null && chunk.XminusOne.Statics(Terrain).IsNormal)
                {
                    QuqueChunkUpdate(chunk.XminusOne.index, 3);
                }
                if (chunk.XplusOne != null && chunk.XplusOne.Statics(Terrain).IsNormal)
                {
                    QuqueChunkUpdate(chunk.XplusOne.index, 3);
                }
                if (chunk.YminusOne != null && chunk.YminusOne.Statics(Terrain).IsNormal)
                {
                    QuqueChunkUpdate(chunk.YminusOne.index, 3);
                }
                if (chunk.YplusOne != null && chunk.YplusOne.Statics(Terrain).IsNormal)
                {
                    QuqueChunkUpdate(chunk.YplusOne.index, 3);
                }
                break;
            case BlockTerrain.ChunkUpdateState.NeedsToBeDestoryed:
                chunkRenderer.RemoveChunk(index);
                //Terrain.chunkInstances[index] = null;
                break;
        }
        statics.updateState = BlockTerrain.ChunkUpdateState.None;

        ChunkInstance instance;
        switch (statics.state)
        {
            case 3:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(0);
                instance.UpdateMesh(1);
                break;
            case 1:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(0);
                break;
            case 2:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(1);
                break;
        }

        statics.state = 0;
    }

    void LoadTerrain(string path)
    {
        Stream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
        terrainReader.Load(stream);
    }

    void QuqueChunkUpdate(int index)
    {
        chunkQueue.AddTerrain(index);
        needTerrainUpdate = true;
    }

    public void QuqueChunkUpdate(int index, int state)
    {
        Terrain.chunkStats.Get(index).state |= state;
        QuqueChunkUpdate(index);
    }

    void UpdateTerrain(int centerChunkX, int centerChunkY)
    {
        BlockTerrain.ChunkStatus chunkStats = Terrain.chunkStats;

        centerChunkX -= chunkStats.halfSize;
        centerChunkY -= chunkStats.halfSize;

        int size = chunkStats.size;
        int startx;
        int starty;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                startx = centerChunkX + x;
                starty = centerChunkY + y;

                if (!chunkStats.IsValid(startx, starty))
                {
                    if (chunkStats.Get(startx, starty).Loaded)
                    {
                        chunkStats.GetOriginal(ref startx, ref starty);
                        DiscardChunk(startx, starty);
                        //						Debug.Log ("is loaded " + Terrain.chunkStats.Get (startx, starty).Loaded);
                    }
                    //					Debug.Log ("is loaded " + Terrain.chunkStats.Get (centerChunkX + x, centerChunkY + y).Loaded);
                    LoadChunk(centerChunkX + x, centerChunkY + y);
                    //Debug.LogFormat("loading: {0}, {1}", centerChunkX + x, centerChunkY + y);
                }
            }
        }

        chunkStats.SetOffset(centerChunkX, centerChunkY);
    }

    public static Point2 CurrentChunk()
    {
        Point3 p = TerrainRaycast.ToCell(Camera.main.transform.position);
        return ToChunk(p.X, p.Z);
    }

    public static Point2 ToChunk(int x, int z)
    {
        return new Point2(x >> 4, z >> 4);
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

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;
    //    //		Point3 p = TerrainRaycast.ToCell (Camera.main.transform.position);
    //    BlockTerrain.ChunkStatus stats = Terrain.chunkStats;
    //    int startx = stats.offsetX;
    //    int starty = stats.offsetY;
    //    //		Terrain.chunkStats.SetOffset (startx, starty);

    //    int cx;
    //    int cy;
    //    for (int x = 0; x < 8; x++)
    //    {
    //        for (int y = 0; y < 8; y++)
    //        {
    //            cx = startx + x;
    //            cy = starty + y;

    //            if (stats.Get(cx, cy).Loaded)
    //            {
    //                DrawChunk(cx, cy);
    //            }
    //        }
    //    }
    //}

    //void DrawChunk(int x, int y)
    //{
    //    Gizmos.DrawCube(new Vector3((x << 4) + 8, 0, (y << 4) + 8), Vector3.one * 16f);
    //}

    class ChunkQueue
    {
        object locker = new object();
        Stack<int> main = new Stack<int>();
        Stack<int> terrain = new Stack<int>();

        public int MainCount
        {
            get
            {
                return main.Count;
            }
        }

        public int TerrainCount
        {
            get
            {
                return terrain.Count;
            }
        }

        public int PopMain()
        {
            lock (locker)
            {
                return main.Pop();
            }
        }

        public int PopTerrain()
        {
            lock (locker)
            {
                return terrain.Pop();
            }
        }

        public void AddMain(int index)
        {
            lock (locker)
            {
                main.Push(index);
            }
        }

        public void AddTerrain(int index)
        {
            lock (locker)
            {
                terrain.Push(index);
            }
        }

        //		public void Clean ()
        //		{
        //			lock (locker) {
        //				if (main.Last == null) {
        //					main.RemoveLast ();
        //				}
        //				if (terrain.Last == null) {
        //					terrain.RemoveLast ();
        //				}
        //			}
        //		}
    }
}

public class ChunkInstance
{
    public Matrix4x4 transform;
    public Mesh[] meshes = new Mesh[] { new Mesh(), new Mesh() };
    public BlockTerrain.Chunk chunkData;

    public bool dirty;

    public ChunkInstance()
    {
        //meshes[0].MarkDynamic();
        //meshes[1].MarkDynamic();
    }

    public void UpdateAll(BlockTerrain.Chunk chunk)
    {
        chunkData = chunk;
        transform = Matrix4x4.Translate(new Vector3(chunk.chunkx << 4, 0, chunk.chunky << 4));
    }

    public void UpdateMesh(int i)
    {
        chunkData.mesh[i].ToMesh(meshes[i]);
    }
}