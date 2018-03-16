using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTerrain : MonoBehaviour
{
    public static int terrainSize = 8;

    public const int chunkSizeX = 16;
    public const int chunkSizeY = 256;
    public const int chunkSizeZ = 16;

    public IEnumerable<Chunk> Chunks
    {
        get
        {
            return chunks;
        }
    }

    Chunk[] chunks = new Chunk[terrainSize * terrainSize];
    Stack<Chunk> freeChunks = new Stack<Chunk>();

    public ChunkStatus chunkStats = new ChunkStatus(terrainSize);
    public readonly ChunkInstance[] chunkInstances = new ChunkInstance[terrainSize * terrainSize];

    public int GetChunkIndex(int x, int y)
    {
        return chunkStats.GetChunkIndex(x, y);
    }

    public Chunk GetChunk(int x, int y)
    {
        if (chunkStats.IsValid(x, y))
        {
            return chunks[chunkStats.GetChunkIndex(x, y)];
        }
        return null;
    }

    public Chunk GetChunk(int index)
    {
        return chunks[index];
    }

    public bool TryGetChunk(int x, int y, out Chunk chunk)
    {
        chunk = chunks[GetChunkIndex(x, y)];
        return chunk != null && chunk.chunkx == x && chunk.chunky == y;
    }

    public Chunk DiscardChunk(int x, int y)
    {
        //		Debug.Log (string.Format ("contains chunk {0}, {1}: {2}", x, y, chunks.ContainsKey (p)));

        int index = GetChunkIndex(x, y);
        Chunk c = chunks[index];
        if (c != null)
        {
            freeChunks.Push(c);
            chunks[index] = null;

            if (c.XminusOne != null)
            {
                c.XminusOne.XplusOne = null;
                c.XminusOne = null;
            }
            if (c.XplusOne != null)
            {
                c.XplusOne.XminusOne = null;
                c.XplusOne = null;
            }
            if (c.YminusOne != null)
            {
                c.YminusOne.YplusOne = null;
                c.YminusOne = null;
            }
            if (c.YplusOne != null)
            {
                c.YplusOne.YminusOne = null;
                c.YplusOne = null;
            }

            //			c.instance.SetActive (false);

            chunkStats.Get(x, y).Loaded = false;

            return c;
        }
        return null;
    }

    public Chunk CreateChunk(int x, int y)
    {
        chunkStats.Get(x, y).Loaded = true;

        Chunk c;
        if (freeChunks.Count != 0)
        {
            c = freeChunks.Pop();
        }
        else
        {
            c = new Chunk(chunkSizeX, chunkSizeY, chunkSizeZ);
        }

        c.chunkx = x;
        c.chunky = y;
        c.index = GetChunkIndex(x, y);
        chunks[GetChunkIndex(x, y)] = c;

        Chunk neighbor;
        if (TryGetChunk(x - 1, y, out neighbor))
        {
            c.XminusOne = neighbor;
            neighbor.XplusOne = c;
        }
        if (TryGetChunk(x + 1, y, out neighbor))
        {
            c.XplusOne = neighbor;
            neighbor.XminusOne = c;
        }
        if (TryGetChunk(x, y - 1, out neighbor))
        {
            c.YminusOne = neighbor;
            neighbor.YplusOne = c;
        }
        if (TryGetChunk(x, y + 1, out neighbor))
        {
            c.YplusOne = neighbor;
            neighbor.YminusOne = c;
        }
        return c;
    }

    public int GetCellValue(int x, int y, int z)
    {
        int chunkx = x >> 4;
        int chunky = z >> 4;
        BlockTerrain.Chunk chunk = GetChunk(chunkx, chunky);
        if (chunk != null)
        {
            return chunk.GetCellValue(x - (chunkx << 4), y, z - (chunky << 4));
        }
        return 0;
    }

    public bool SetCellValue(int x, int y, int z, int value)
    {
        int chunkx = x >> 4;
        int chunky = z >> 4;
        Chunk chunk = GetChunk(chunkx, chunky);
        if (chunk != null)
        {
            chunk.SetCellValue(x - (chunkx << 4), y, z - (chunky << 4), value);
            return true;
        }
        return false;
    }

    public class ChunkStatus
    {
        public readonly int size;
        public readonly int halfSize;
        int mask;
        int shift;

        public int offsetX;
        public int offsetY;

        ChunkStatics[] data;

        public ChunkStatus(int size)
        {
            this.size = size;
            halfSize = size / 2;
            mask = size - 1;
            shift = Log2(size);
            data = new ChunkStatics[size * size];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new ChunkStatics();
            }
        }

        int Log2(int x)
        {
            int i = -1;
            while (x != 0)
            {
                i++;
                x = x >> 1;
            }
            return i;
        }

        public void SetOffset(int x, int y)
        {
            //Debug.Log(string.Format("set offset to: {0}, {1}", x, y));
            offsetX = x;
            offsetY = y;
        }

        public void Offset(int x, int y)
        {
            offsetX += x;
            offsetY += y;
        }

        public ChunkStatics Get(int x, int y)
        {
            return data[GetChunkIndex(x, y)];
        }

        public ChunkStatics Get(int index)
        {
            return data[index];
        }

        public int GetChunkIndex(int x, int y)
        {
            return (x & mask) + ((y & mask) << shift);
        }

        public bool IsValid(int x, int y)
        {
            x -= offsetX;
            y -= offsetY;
            return x > -1 && x < size && y > -1 && y < size;
        }

        public void GetOriginal(ref int x, ref int y)
        {
            x = ((x - offsetX) & mask) + offsetX;
            y = ((y - offsetY) & mask) + offsetY;
        }
    }

    public class ChunkStatics
    {
        public bool Loaded;
        public int state;
        public ChunkUpdateState updateState = ChunkUpdateState.None;

        public bool IsNormal
        {
            get
            {
                return Loaded && state == 0;
            }
        }
    }

    public class Chunk
    {
        public readonly int sizeX;
        public readonly int sizeY;
        public readonly int sizeZ;

        public int index;

        public int chunkx;
        public int chunky;

        public MeshData[] mesh = new MeshData[2];

        public Chunk XminusOne;
        public Chunk YminusOne;
        public Chunk XplusOne;
        public Chunk YplusOne;

        public ChunkStatics Statics(BlockTerrain terrain)
        {
            return terrain.chunkStats.Get(index);
        }

        int[] cells;
        int[] shifts;

        public Chunk(int sizex, int sizey, int sizez)
        {
            sizeX = sizex;
            sizeY = sizey;
            sizeZ = sizez;
            cells = new int[sizeX * sizeY * sizeZ];
            shifts = new int[sizeX * sizeZ];
        }

        public int GetCellValue(int x, int y, int z)
        {
            if (x >= 0 && x < chunkSizeX && y >= 0 && y < chunkSizeY && z >= 0 && z < chunkSizeZ)
            {
                return GetCellValue(GetCellIndex(x, y, z));
            }
            if (XminusOne != null && x == -1)
                return XminusOne.GetCellValue(15, y, z);
            else if (XplusOne != null && x == 16)
                return XplusOne.GetCellValue(0, y, z);
            if (YminusOne != null && z == -1)
                return YminusOne.GetCellValue(x, y, 15);
            else if (YplusOne != null && z == 16)
                return YplusOne.GetCellValue(x, y, 0);
            return 1023;
        }

        public int GetCellValue(int index)
        {
            return cells[index];
        }

        public int GetCellContent(int x, int y, int z)
        {
            return GetContent(GetCellValue(x, y, z));
        }

        public void SetCellValue(int x, int y, int z, int value)
        {
            SetCellValue(GetCellIndex(x, y, z), value);
        }

        public void SetCellValue(int index, int value)
        {
            cells[index] = value;
        }

        public int GetShiftValue(int index)
        {
            return shifts[index];
        }

        public int GetShiftValue(int x, int y)
        {
            return GetShiftValue(GetShiftIndex(x, y));
        }

        public void SetShiftValue(int index, int value)
        {
            shifts[index] = value;
        }
    }

    public static int GetShiftIndex(int x, int y)
    {
        return y + chunkSizeZ * x;
    }

    public static int GetCellIndex(int x, int y, int z)
    {
        return y + chunkSizeY * x + chunkSizeX * chunkSizeY * z;
    }

    public static int GetContent(int value)
    {
        return value & 1023;
    }

    public static int GetData(int value)
    {
        return (value & -16384) >> 14;
    }

    public static int MakeBlockValue(int contents, int light, int data)
    {
        return (contents & 1023) | (light << 10 & 15360) | (data << 14 & -16384);
    }

    public static int ReplaceData(int value, int data)
    {
        return value ^ ((value ^ data << 14) & -16384);
    }

    public static int GetTemperature(int value)
    {
        return (value & 3840) >> 8;
    }

    public static int GetHumidity(int value)
    {
        return (value & 61440) >> 12;
    }

    public enum ChunkUpdateState
    {
        NeedsToBeCreated,
        NeedsToBeDestoryed,
        None
    }
}
