using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBlock : Block
{
	FurnitureManager furnitureManager;

	public override void Initialize (GameObject game)
	{
		IsTransparent = true;
		furnitureManager = game.GetComponent<FurnitureManager> ();
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int data = BlockTerrain.GetData (value);
        g.MeshFromMesh (x, y, z, furnitureManager.GetFurniture (GetDesignIndex (data), GetRotation (data)), true);
	}

	public static int GetDesignIndex (int data)
	{
		return data >> 2 & 1023;
	}

	public static int GetRotation (int data)
	{
		return data & 3;
	}
}
