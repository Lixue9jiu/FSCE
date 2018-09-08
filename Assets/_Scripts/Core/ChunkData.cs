using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    const int terrainSize = 8;
    const int chunkSize = 32768;

    int[][] chunks;

    ChunkIndexer chunkIndexer;

    public ChunkData()
    {
        chunks = new int[terrainSize * terrainSize][];
        chunkIndexer = new ChunkIndexer(terrainSize);
    }

    public int[] GetChunk(int index)
    {
        return chunks[index];
    }

    public int GetBlock(int x, int y, int z)
    {
        return chunks[chunkIndexer.GetChunkIndex(x >> 4, z >> 4)][GetBlockIndex(x & 15, y & 127, z & 15)];
    }

    public int GetBlockIndex(int x, int y, int z)
    {
        return y + x << 4 + z << 11;
    }

    public int[] CreateChunk(int index)
    {
        if (chunks[index] == null)
            chunks[index] = new int[chunkSize];
        return chunks[index];
    }

    public void DestoryChunk(int index)
    {
        chunks[index] = null;
    }

    public static bool GetBlockNeighbors(int x, int y, int z, int blockIndex, int[] currentChunk, int[][] chunks, ref int[] neighbors)
    {
        if (y == 127 || y == 0)
            return true;

        neighbors[CellFace.BOTTOM] = currentChunk[blockIndex - 1];
        neighbors[CellFace.TOP] = currentChunk[blockIndex + 1];

        neighbors[CellFace.FRONT] = currentChunk[blockIndex + 16];
        neighbors[CellFace.BACK] = currentChunk[blockIndex - 16];

        neighbors[CellFace.LEFT] = currentChunk[blockIndex - 16];
        neighbors[CellFace.RIGHT] = currentChunk[blockIndex - 16];
        return false;
    }
}
