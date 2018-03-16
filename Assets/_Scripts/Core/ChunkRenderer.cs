using UnityEngine;
using System.Collections.Generic;

public class ChunkRenderer : MonoBehaviour
{
    public BlockTerrain Terrain;
    public Material normal;
    public Material alaphaTest;

    public const int chunkLayer = 0;

    List<int> renderingChunks = new List<int>(256);

    public void AddChunk(int chunkIndex)
    {
        if (!renderingChunks.Contains(chunkIndex))
            renderingChunks.Add(chunkIndex);
    }

    public void RemoveChunk(int chunkIndex)
    {
        renderingChunks.Remove(chunkIndex);
    }

    //float deltaTime;

    private void Update()
    {
        for (int i = 0; i < renderingChunks.Count; i++)
        {
            RenderChunk(Terrain.chunkInstances[renderingChunks[i]]);
        }
    }

    void RenderChunk(ChunkInstance instance)
    {
        Graphics.DrawMesh(instance.meshes[0], instance.transform, normal, chunkLayer, Camera.main);
        Graphics.DrawMesh(instance.meshes[1], instance.transform, alaphaTest, chunkLayer, Camera.main);
    }
}