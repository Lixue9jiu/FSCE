using System;
public interface INormalBlock
{
	void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainMesh terrainMesh);
}