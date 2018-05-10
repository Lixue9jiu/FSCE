using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBlock : Block
{
	FurnitureManager furnitureManager;

	public override void Initialize ()
	{
		IsTransparent = true;
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

	public override void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh)
	{
		int d = BlockTerrain.GetData(value);
		terrainMesh.AlphaTest.Mesh(x, y, z, furnitureManager.GetFurniture(GetDesignIndex(d), GetRotation(d)));
	}
}
