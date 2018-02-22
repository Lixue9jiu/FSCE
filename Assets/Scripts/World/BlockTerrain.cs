using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTerrain : MonoBehaviour {

	public const int chunkSizeX = 16;
	public const int chunkSizeY = 256;
	public const int chunkSizeZ = 16;

	Dictionary<Point2, Chunk> chunks = new Dictionary<Point2, Chunk> ();
	LinkedList<Chunk> freeChunks = new LinkedList<Chunk> ();

	public Chunk GetChunk(int x, int y)
	{
		Point2 p = new Point2 (x, y);
		if (chunks.ContainsKey (p)) {
			return chunks [p];
		}
		return null;
	}

	public void DiscardChunk(int x, int y)
	{
		Point2 p = new Point2 (x, y);
		if (chunks.ContainsKey (p)) {
			BlockTerrain.Chunk c = chunks [p];
			freeChunks.AddLast (c);
			chunks.Remove (p);

			if (c.XminusOne != null) {
				c.XminusOne.XplusOne = null;
				c.XminusOne = null;
			}
			if (c.XplusOne != null) {
				c.XplusOne.XminusOne = null;
				c.XplusOne = null;
			}
			if (c.YminusOne != null) {
				c.YminusOne.YplusOne = null;
				c.YminusOne = null;
			}
			if (c.YplusOne != null) {
				c.YplusOne.YminusOne = null;
				c.YplusOne = null;
			}
		}
	}

	public Chunk CreateChunk(int x, int y)
	{
		Chunk c;
		if (freeChunks.Count != 0) {
			c = freeChunks.First.Value;
			freeChunks.RemoveFirst ();
		} else {
			c = new Chunk (chunkSizeX, chunkSizeY, chunkSizeZ);
		}

		c.chunkx = x;
		c.chunky = y;
		chunks [new Point2 (x, y)] = c;

		Chunk neighbor;
		if (chunks.TryGetValue (new Point2(x - 1, y), out neighbor)) {
			c.XminusOne = neighbor;
			neighbor.XplusOne = c;
		}
		if (chunks.TryGetValue (new Point2(x + 1, y), out neighbor)) {
			c.XplusOne = neighbor;
			neighbor.XminusOne = c;
		}
		if (chunks.TryGetValue (new Point2(x, y - 1), out neighbor)) {
			c.YminusOne = neighbor;
			neighbor.YplusOne = c;
		}
		if (chunks.TryGetValue (new Point2(x, y + 1), out neighbor)) {
			c.YplusOne = neighbor;
			neighbor.YminusOne = c;
		}
		return c;
	}

	public int GetCellValue(int x, int y, int z)
	{
		int chunkx = x >> 4;
		int chunky = z >> 4;
		BlockTerrain.Chunk chunk = GetChunk (chunkx, chunky);
		if (chunk != null) {
			return chunk.GetCellValue (x - (chunkx << 4), y, z - (chunky << 4));
		}
		return 0;
	}

	public class Chunk
	{
		public readonly int sizeX;
		public readonly int sizeY;
		public readonly int sizeZ;

		public int chunkx;
		public int chunky;

		public Chunk XminusOne;
		public Chunk YminusOne;
		public Chunk XplusOne;
		public Chunk YplusOne;

		int[] data;

		public Chunk(int sizex, int sizey, int sizez)
		{
			sizeX = sizex;
			sizeY = sizey;
			sizeZ = sizez;
			data = new int[sizeX * sizeY * sizeZ];
		}

		public int GetCellValue(int x, int y, int z)
		{
			if (x >= 0 && x < chunkSizeX && y >= 0 && y < chunkSizeY && z >= 0 && z < chunkSizeZ) {
				return data [GetCellIndex (x, y, z)];
			}
			if (XminusOne != null && x == -1)
				return XminusOne.GetCellValue (15, y, z);
			else if (XplusOne != null && x == 16)
				return XplusOne.GetCellValue (0, y, z);
			if (YminusOne != null && z == -1)
				return YminusOne.GetCellValue (x, y, 15);
			else if (YplusOne != null && z == 16)
				return YplusOne.GetCellValue (x, y, 0);
			return 0;
		}

		public int GetCellContent(int x, int y, int z)
		{
			return GetContent (GetCellValue (x, y, z));
		}

		public void SetCellValue(int x, int y, int z, int value)
		{
			SetCellValue(GetCellIndex(x, y, z), value);
		}

		public void SetCellValue(int index, int value)
		{
			data [index] = value;
		}
	}

	public static int GetCellIndex(int x, int y, int z)
	{
		return y + chunkSizeY * x + chunkSizeX * chunkSizeY * z;
	}

	public static int GetContent(int value)
	{
		return value & 1023;
	}

	public static int GetData (int value)
	{
		return (value & -16384) >> 14;
	}

	public static int MakeBlockValue (int contents, int light, int data)
	{
		return (contents & 1023) | (light << 10 & 15360) | (data << 14 & -16384);
	}

	public static int ReplaceData (int value, int data)
	{
		return value ^ ((value ^ data << 14) & -16384);
	}
}