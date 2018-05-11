using System;
public interface IStandardCubeBlock
{
	void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data);
}