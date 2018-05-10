using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block
{
    public int Index;
    public bool IsTransparent;

    public int TextureSlot;

    public string Name;

    public string ToString(int value)
    {
        return string.Format("{0}: {1}, {2}", Name, Index, BlockTerrain.GetData(value));
    }

    public virtual void Initialize()
    {
    }

	public abstract void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data, TerrainGenerator terrainMesh);
}
