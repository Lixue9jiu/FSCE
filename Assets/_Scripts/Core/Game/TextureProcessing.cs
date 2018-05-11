using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureProcessing
{
	public static Texture2D ProcessTexture(Texture2D origin)
	{
		if (origin.width != origin.height)
			throw new System.Exception(string.Format("texture {0} is not square", origin.name));
		int textureSize = origin.width;

		Texture2D result = new Texture2D(textureSize * 2, textureSize * 2, TextureFormat.ARGB32, true);
		result.filterMode = FilterMode.Point;
		result.mipMapBias = -0.5f;
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
					result.SetPixels(x2 * mipSize, y2 * mipSize, mipSize, mipSize, cell.GetPixels(i), i);
					result.SetPixels((x2 + 1) * mipSize, y2 * mipSize, mipSize, mipSize, cell.GetPixels(i), i);
					result.SetPixels(x2 * mipSize, (y2 + 1) * mipSize, mipSize, mipSize, cell.GetPixels(i), i);
					result.SetPixels((x2 + 1) * mipSize, (y2 + 1) * mipSize, mipSize, mipSize, cell.GetPixels(i), i);
				}
				result.SetPixels(x, y, 1, 1, cell.GetPixels(5), 6);
            }
		}
		result.Apply(false, true);
		return result;
	}
}
