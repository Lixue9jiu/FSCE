using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureProcessing
{
    public static void ProcessTexture(Texture2D origin, out Texture2D terrainTex, out Texture2D alphaTestTex)
	{
		if (origin.width != origin.height)
			throw new System.Exception(string.Format("texture {0} is not square", origin.name));
		int textureSize = origin.width;

        alphaTestTex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
        alphaTestTex.filterMode = FilterMode.Point;
        alphaTestTex.mipMapBias = -0.5f;

		terrainTex = new Texture2D(textureSize * 2, textureSize * 2, TextureFormat.ARGB32, true);
		terrainTex.filterMode = FilterMode.Point;
		terrainTex.mipMapBias = -0.5f;
		//result.anisoLevel = 3;
		int cellSize = textureSize / 16;

        Texture2D cell = new Texture2D(cellSize, cellSize, TextureFormat.ARGB32, true);
		for (int x = 0; x < 16; x++)
		{
			for (int y = 0; y < 16; y++)
            {            
				cell.SetPixels(origin.GetPixels(x * cellSize, y * cellSize, cellSize, cellSize));
				cell.Apply(true, false);

                int x2 = x * 2;
                int y2 = y * 2;
				for (int i = 0; i < 6; i++)
				{
					int mipSize = cellSize >> i;
                    Color[] colors = cell.GetPixels(i);
                    alphaTestTex.SetPixels(x * mipSize, y * mipSize, mipSize, mipSize, colors, i);

                    terrainTex.SetPixels(x2 * mipSize, y2 * mipSize, mipSize, mipSize, colors, i);
                    terrainTex.SetPixels((x2 + 1) * mipSize, y2 * mipSize, mipSize, mipSize, colors, i);
                    terrainTex.SetPixels(x2 * mipSize, (y2 + 1) * mipSize, mipSize, mipSize, colors, i);
                    terrainTex.SetPixels((x2 + 1) * mipSize, (y2 + 1) * mipSize, mipSize, mipSize, colors, i);
				}
				terrainTex.SetPixels(x, y, 1, 1, cell.GetPixels(5), 6);
            }
		}
        alphaTestTex.Apply(false, true);
        terrainTex.Apply(false, true);
	}
}
