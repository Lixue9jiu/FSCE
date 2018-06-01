using UnityEngine;

public class ChunkInstance
{
    public Matrix4x4 transform;
    public Mesh CombinedMesh = new Mesh();

    public BlockTerrain.Chunk chunkData;

    public GameObject chunkObj;

    public CombineInstance[] combineInstances = new CombineInstance[]
    {
        new CombineInstance
        {
            mesh = new Mesh()
        },
        new CombineInstance
        {
            mesh = new Mesh()
        }
    };

    public void UpdateAll(BlockTerrain.Chunk chunk)
    {
        chunkData = chunk;
        transform = Matrix4x4.Translate(new Vector3(chunk.chunkx << 4, 0, chunk.chunky << 4));
        CombinedMesh.Clear();
    }

    public void UpdateMesh(int i)
    {
        chunkData.mesh[i].ToMesh(combineInstances[i].mesh);
        CombinedMesh.Clear();
        CombinedMesh.CombineMeshes(combineInstances, false, false, false);
    }
}
