using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBlock : Block, INormalBlock
{
    FurnitureManager furnitureManager;

    public override void Initialize(string extraData)
    {
        furnitureManager = FurnitureManager.instance;
    }

    public static int GetDesignIndex(int data)
    {
        return data >> 2 & 1023;
    }

    public static int GetRotation(int data)
    {
        return data & 3;
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
    {
        int d = BlockTerrain.GetData(value);
        if (furnitureManager.isTransparent[GetDesignIndex(d)])
            g.AlphaTest.Mesh(x, y, z, furnitureManager.GetFurniture(GetDesignIndex(d), GetRotation(d)));
        else
            g.Terrain.Mesh(x, y, z, furnitureManager.GetFurniture(GetDesignIndex(d), GetRotation(d)));
    }
}
