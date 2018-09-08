using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkIndexer
{
    public readonly int size;
    public readonly int halfSize;
    int mask;
    int shift;

    public int offsetX;
    public int offsetY;

    public ChunkIndexer(int size)
    {
        this.size = size;
        halfSize = size / 2;
        mask = size - 1;
        shift = Log2(size);
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

public struct ChunkStat
{
    
}
