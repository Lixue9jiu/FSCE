using UnityEngine;
using System.IO;

public interface ITerrainReader : System.IDisposable
{
	void Load (Stream stream);
	void SaveChunk(BlockTerrain.Chunk chunk);
    void ReadChunk(int chunkx, int chunky, BlockTerrain.Chunk chunk);
	bool ChunkExist(int chunkx, int chunky);
}