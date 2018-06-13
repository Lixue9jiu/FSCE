using UnityEngine;

public class ChunkInstance
{
    public Matrix4x4 transform;
    public Mesh[] Meshes = new Mesh[] { new Mesh(), new Mesh() };

    public BlockTerrain.Chunk chunkData;

    public GameObject chunkObj;

    public void UpdateAll(BlockTerrain.Chunk chunk)
    {
        chunkData = chunk;
        transform = Matrix4x4.Translate(new Vector3(chunk.chunkx << 4, 0, chunk.chunky << 4));
    }

    public void UpdateMesh(int i)
    {
        chunkData.mesh[i].ToMesh(Meshes[i]);
    }
}
