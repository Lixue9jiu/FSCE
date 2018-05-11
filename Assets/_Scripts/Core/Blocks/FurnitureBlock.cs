using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBlock : Block, INormalBlock
{
	FurnitureManager furnitureManager;

	public override void Initialize ()
	{
        furnitureManager = FurnitureManager.instance;
	}

	public static int GetDesignIndex (int data)
	{
		return data >> 2 & 1023;
	}

	public static int GetRotation (int data)
	{
		return data & 3;
	}

	public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainMesh terrainMesh)
	{
		int d = BlockTerrain.GetData(value);
		terrainMesh.Mesh(x, y, z, furnitureManager.GetFurniture(GetDesignIndex(d), GetRotation(d)));
	}
}
