﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainReader129
{

	Stream stream;

	Dictionary<Point2, long> chunkOffsets = new Dictionary<Point2, long> ();

	byte[] buffer = new byte[131072];
	MemoryStream memStr;

	public TerrainReader129 ()
	{
		memStr = new MemoryStream (buffer);
	}

	public BlockTerrain.Chunk[] AlaphaTest (BlockTerrain terrain)
	{
		BlockTerrain.Chunk chunk;

		Dictionary<Point2, long>.Enumerator enu = chunkOffsets.GetEnumerator ();
		List<BlockTerrain.Chunk> c = new List<BlockTerrain.Chunk> ();

		while (enu.MoveNext ()) {
			Point2 p = enu.Current.Key;
			chunk = ReadChunk (p.X, p.Y, terrain);
			c.Add (chunk);
		}
		return c.ToArray ();
	}

	public void Load (Stream stream)
	{
		this.stream = stream;
		while (true) {
			int x;
			int y;
			int index;
			ReadChunkEntry (stream, out x, out y, out index);
			if (index < 0)
				break;
			chunkOffsets [new Point2 (-x, y)] = 786444L + 132112L * (long)index;
		}
	}

	public BlockTerrain.Chunk ReadChunk (int chunkx, int chunky, BlockTerrain terrain)
	{
		return ReadChunk (chunkx, chunky, terrain.CreateChunk (chunkx, chunky));
	}

	private BlockTerrain.Chunk ReadChunk (int chunkx, int chunky, BlockTerrain.Chunk chunk)
	{
		Point2 p = new Point2 (chunkx, chunky);
		long value;
		if (chunkOffsets.TryGetValue (p, out value)) {
			stream.Seek (value, SeekOrigin.Begin);
			ReadChunkHeader (stream);
			stream.Read (buffer, 0, 131072);

			memStr.Seek (0, SeekOrigin.Begin);

			for (int x = 0; x < 16; x++) {
				for (int y = 0; y < 16; y++) {
					int index = BlockTerrain.GetCellIndex (15 - x, 0, y);
					int h = 0;
					while (h < 128) {
						chunk.SetCellValue (index, ReadInt (memStr));
						h++;
						index++;
					}
				}
			}
		}
		return chunk;
	}

	public void SaveChunk (BlockTerrain.Chunk chunk)
	{
		Point2 p = new Point2 (chunk.chunkx, chunk.chunky);
		long value;
		if (chunkOffsets.TryGetValue (p, out value)) {
			stream.Seek (value, SeekOrigin.Begin);
			WriteChunkHeader (stream, chunk.chunkx, chunk.chunky);

			memStr.Seek (0, SeekOrigin.Begin);
			for (int x = 0; x < 16; x++) {
				for (int y = 0; y < 16; y++) {
					int index = BlockTerrain.GetCellIndex (15 - x, 0, y);
					int h = 0;
					while (h < 128) {
						WriteInt (memStr, chunk.GetCellValue (index));
						h++;
						index++;
					}
				}
			}
			stream.Write (buffer, 0, 131072);
		}
	}

	public void Destory ()
	{
		stream.Close ();
	}

	static int ReadInt (Stream stream)
	{
		return stream.ReadByte () + (stream.ReadByte () << 8) + (stream.ReadByte () << 16) + (stream.ReadByte () << 24);
	}

	static void WriteInt (Stream stream, int value)
	{
		stream.WriteByte ((byte)value);
		stream.WriteByte ((byte)(value >> 8));
		stream.WriteByte ((byte)(value >> 16));
		stream.WriteByte ((byte)(value >> 24));
	}

	static void ReadChunkEntry (Stream stream, out int chunkx, out int chunky, out int index)
	{
		chunkx = ReadInt (stream);
		chunky = ReadInt (stream);
		index = ReadInt (stream);
	}

	static void WriteChunkEntry (Stream stream, int chunkx, int chunky, int index)
	{
		WriteInt (stream, chunkx);
		WriteInt (stream, chunky);
		WriteInt (stream, index);
	}

	public static void ReadChunkHeader (Stream stream)
	{
		int v1 = ReadInt (stream);
		int v2 = ReadInt (stream);
		int chunkx = ReadInt (stream);
		int chunky = ReadInt (stream);
		if (v1 != -559038737 || v2 != -2) {
			throw new UnityException (string.Format ("invalid chunk header at: {0}, {1}", chunkx, chunky));
		}
	}

	public static void WriteChunkHeader (Stream stream, int chunkx, int chunky)
	{
		WriteInt (stream, -559038737);
		WriteInt (stream, -2);
		WriteInt (stream, chunkx);
		WriteInt (stream, chunky);
	}
}