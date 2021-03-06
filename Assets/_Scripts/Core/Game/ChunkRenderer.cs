using UnityEngine;
using System.Collections.Generic;

public class ChunkRenderer : MonoBehaviour
{
    const int TEXTURE_SIZE = 256;

    private BlockTerrain Terrain;

    public Texture2D defautTexture;

    public Shader TerrainShader;
    public Shader VertexColorShader;

    public Material terrain;
    public Material alaphaTest;

    public const int chunkLayer = 0;

    List<int> renderingChunks = new List<int>(256);

    public int[] RenderingChunks
    {
        get
        {
            return renderingChunks.ToArray();
        }
    }

    private void Start()
    {
        Terrain = GetComponent<TerrainManager>().Terrain;

        string blockTextureName = WorldManager.Project.GameInfo.BlockTextureName;
        if (!string.IsNullOrEmpty(blockTextureName))
        {
            string path = System.IO.Path.Combine(WorldManager.CurrentEmbeddedContent, blockTextureName);
            defautTexture.LoadImage(System.IO.File.ReadAllBytes(path));
        }

        try
        {
            //Texture2D tex = new Texture2D (TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, true, true);
            //tex.filterMode = FilterMode.Point;
            //tex.LoadImage(System.IO.File.ReadAllBytes(path));

#if GREEDY_MESHING
            Texture2D main, alphaTest;
            TextureProcessing.ProcessTexture2(defautTexture, out main, out alphaTest);
            terrain.mainTexture = main;
            alaphaTest.mainTexture = alphaTest;
            terrain.shader = TerrainShader;
#else
            Texture2D tex;
            TextureProcessing.ProcessTexture(defautTexture, out tex);
            terrain.mainTexture = tex;
            alaphaTest.mainTexture = tex;
            terrain.shader = VertexColorShader;
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

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
        Graphics.DrawMesh(instance.Meshes[0], instance.transform, terrain, chunkLayer, Camera.main);
        Graphics.DrawMesh(instance.Meshes[1], instance.transform, alaphaTest, chunkLayer, Camera.main);
    }
}
