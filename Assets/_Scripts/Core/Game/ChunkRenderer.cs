using UnityEngine;
using System.Collections.Generic;

public class ChunkRenderer : MonoBehaviour
{
	const int TEXTURE_SIZE = 256;

	private BlockTerrain Terrain;

	public Texture2D defautTexture;

	public Material normal;
	public Material alaphaTest;

	public const int chunkLayer = 0;

	List<int> renderingChunks = new List<int> (256);

    public int[] RenderingChunks
    {
        get
        {
            return renderingChunks.ToArray();
        }
    }

	private void Start ()
	{
		Terrain = GetComponent<TerrainManager>().Terrain;

		string blockTextureName = WorldManager.Project.GetGameInfo ().BlockTextureName;
		if (!string.IsNullOrEmpty (blockTextureName)) {
			LoadTexture (blockTextureName);
		} else {
			normal.mainTexture = TextureProcessing.ProcessTexture(defautTexture);
			alaphaTest.mainTexture = defautTexture;
		}
	}

	public void LoadTexture (string name)
	{
		string path = System.IO.Path.Combine (WorldManager.CurrentEmbeddedContent, name);

		try {
			//Texture2D tex = new Texture2D (TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, true, true);
			//tex.filterMode = FilterMode.Point;
			//tex.LoadImage(System.IO.File.ReadAllBytes(path));

			defautTexture.LoadImage(System.IO.File.ReadAllBytes(path));

			normal.mainTexture = TextureProcessing.ProcessTexture(defautTexture);;
			alaphaTest.mainTexture = defautTexture;
		} catch (System.Exception e) {
			Debug.LogException (e);
		}
	}

	public void AddChunk (int chunkIndex)
	{
		if (!renderingChunks.Contains (chunkIndex))
			renderingChunks.Add (chunkIndex);
	}

	public void RemoveChunk (int chunkIndex)
	{
		renderingChunks.Remove (chunkIndex);
	}

	//float deltaTime;

	private void Update ()
	{
		for (int i = 0; i < renderingChunks.Count; i++) {
			RenderChunk (Terrain.chunkInstances [renderingChunks [i]]);
		}
	}

	void RenderChunk (ChunkInstance instance)
	{
		Graphics.DrawMesh (instance.meshes [0], instance.transform, normal, chunkLayer, Camera.main);
		Graphics.DrawMesh (instance.meshes [1], instance.transform, alaphaTest, chunkLayer, Camera.main);
	}
}
