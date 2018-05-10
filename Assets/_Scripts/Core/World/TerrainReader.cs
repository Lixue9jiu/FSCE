using UnityEngine;
using System.IO;

public interface ITerrainReader : System.IDisposable
{
	void Load (System.IO.Stream stream);
	void SaveChunk(BlockTerrain.Chunk chunk);
	BlockTerrain.Chunk ReadChunk(int chunkx, int chunky, BlockTerrain terrain);
	bool ChunkExist(int chunkx, int chunky);
}