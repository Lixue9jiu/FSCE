using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    const int updateMili = 200;

    public int chunkLoadDistanceSqu = 64;
    public int chunkDiscardDistanceSqu = 81;

    ITerrainReader terrainReader = new TerrainReader129();

    ChunkQueue chunkQueue = new ChunkQueue();

    public BlockTerrain Terrain;
    public MeshGenerator terrainGenerator = new MeshGenerator();

    private ChunkRenderer chunkRenderer;

    private ChunkInstanceManager chunkInstanceManager;

    bool chunkUpdateWorking;

    Thread thread;
    bool threadRunning = true;

    private void Awake()
    {
        chunkRenderer = GetComponent<ChunkRenderer>();
        chunkInstanceManager = GetComponent<ChunkInstanceManager>();
    }

    private void Start()
    {
        if (GetComponent<MyGameManager>().loadingSuccessful)
        {
            Load();
            StartChunkUpdateThread();
        }
    }

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
            chunkUpdateWorking = true;
            try
            {
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
                            terrainGenerator.GenerateAllBlocks(chunk);
                            terrainGenerator.Terrain.PushToMesh(out chunk.mesh[0]);
                            terrainGenerator.AlphaTest.PushToMesh(out chunk.mesh[1]);
                            //Terrain.chunkStats.Get(index).state = 0;
                            //switch (Terrain.chunkStats.Get(index).state)
                            //{
                            //	case 3:
                            //		terrainGenerator.GenerateChunkMesh(chunk, out chunk.mesh[0]);
                            //		terrainGenerator.GenerateNormalBlocks(chunk, out chunk.mesh[1]);
                            //		break;
                            //	case 1:
                            //		terrainGenerator.GenerateChunkMesh(chunk, out chunk.mesh[0]);
                            //		break;
                            //	case 2:
                            //		terrainGenerator.GenerateNormalBlocks(chunk, out chunk.mesh[1]);
                            //		break;
                            //}
                            chunkQueue.AddMain(index);
                        }
                    }
                }
                else if (needTerrainUpdate)
                {
                    UpdateTerrain(centerChunk.X, centerChunk.Y);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                chunkUpdateWorking = false;
            }
        }
        Debug.Log("chunk loading thread closed");
    }

    void OnDisable()
    {
        try
        {
            threadRunning = false;
            chunkQueue.Clear();
            while (chunkUpdateWorking)
            {
            }
            thread.Join();
        }
        finally
        {
            SaveAllChunks();
            terrainReader.Dispose();
        }
    }

    public void Load()
    {
        Debug.Log("loading terrain: " + BlockTerrain.terrainSize);
        Terrain = new BlockTerrain();
        LoadTerrain(WorldManager.ChunkDat);
        Camera.main.transform.position = WorldManager.Project.PlayerPosition;

        Point2 p = CurrentChunk();
        Terrain.chunkStats.SetOffset(p.X + 2000, p.Y + 2000);
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
                state.state = 3;
            }
            content = BlockTerrain.GetContent(newValue);
            if (content != 0)
            {
                state.state = 3;
            }
            c.SetCellValue(cx, y, cz, newValue);
            c.isEdited = true;
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
                c = terrainReader.ReadChunk(x, y, Terrain);
                QuqueChunkUpdate(c.index, 3);
                Terrain.chunkStats.Get(c.index).needsToBeCreated = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("error loading chunk");
            Debug.LogException(e);
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
            Terrain.chunkStats.Get(c.index).needsToBeDestroyed = true;
            chunkQueue.AddMain(c.index);
        }
    }

    public void SaveAllChunks()
    {
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
        instance.UpdateAll(c);
        instance.UpdateMesh(0);
        instance.UpdateMesh(1);
        //chunkRenderer.AddChunk(index);
        chunkInstanceManager.LoadChunkInstance(instance);
    }

    public void UpdateChunk(int index)
    {
        BlockTerrain.ChunkStatics statics = Terrain.chunkStats.Get(index);

        if (statics.needsToBeCreated && !statics.needsToBeDestroyed)
        {
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
            statics.needsToBeCreated = false;
        }
        else if (statics.needsToBeDestroyed)
        {
            chunkInstanceManager.UnloadChunkInstance(Terrain.chunkInstances[index]);
            statics.needsToBeDestroyed = false;
        }

        ChunkInstance instance;
        switch (statics.state)
        {
            case 3:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(0);
                instance.UpdateMesh(1);
                chunkInstanceManager.UpdateChunkInstance(instance);
                break;
            case 1:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(0);
                chunkInstanceManager.UpdateChunkInstance(instance);
                break;
            case 2:
                instance = Terrain.chunkInstances[index];
                instance.UpdateMesh(1);
                chunkInstanceManager.UpdateChunkInstance(instance);
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
        //needTerrainUpdate = true;
    }

    public void QuqueChunkUpdate(int index, int state)
    {
        Terrain.chunkStats.Get(index).state |= state;
        QuqueChunkUpdate(index);
    }

    void UpdateTerrain(int centerChunkX, int centerChunkY)
    {
        BlockTerrain.ChunkStatus chunkStats = Terrain.chunkStats;

        Vector2 center = new Vector2(centerChunkX, centerChunkY);

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
                    }
                    LoadChunk(centerChunkX + x, centerChunkY + y);
                }
            }
        }

        chunkStats.SetOffset(centerChunkX, centerChunkY);
        needTerrainUpdate = false;
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

    class ChunkQueue
    {
        object locker = new object();
        Queue<int> main = new Queue<int>();
        Queue<int> terrain = new Queue<int>();

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
                return main.Dequeue();
            }
        }

        public int PopTerrain()
        {
            lock (locker)
            {
                return terrain.Dequeue();
            }
        }

        public void AddMain(int index)
        {
            lock (locker)
            {
                main.Enqueue(index);
            }
        }

        public void AddTerrain(int index)
        {
            lock (locker)
            {
                terrain.Enqueue(index);
            }
        }

        public void Clear()
        {
            lock (locker)
            {
                main.Clear();
                terrain.Clear();
            }
        }
    }
}