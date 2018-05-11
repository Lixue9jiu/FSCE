using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMap {

	Color[] m_map = new Color[256];

	public ColorMap (Color32 c11, Color32 c21, Color32 c12, Color32 c22)
	{
		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 16; j++) {
				float f = Saturate ((float)i / 8f);
				float f2 = Saturate ((float)(j - 4) / 10f);
				Color tmp = Color32.Lerp (c11, c21, f);
				Color c = Color32.Lerp (c12, c22, f);
				Color color = Color32.Lerp (tmp, c, f2);
				int num = i + j * 16;
				this.m_map [num] = color;
			}
		}
	}

	public Color Lookup (int value)
	{
		return Lookup (BlockTerrain.GetTemperature (value), BlockTerrain.GetHumidity (value));
	}

	public Color Lookup (int temperature, int humidity)
	{
		int i = temperature + 16 * humidity;
		return m_map [i];
	}

	public static float Saturate (float f)
	{
		if (f < 0f) {
			return 0f;
		}
		if (f < 1f) {
			return f;
		}
		return 1f;
	}
}
