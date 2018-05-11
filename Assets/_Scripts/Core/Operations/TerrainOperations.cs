using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOperations
{

	TerrainManager terrainManager;

	public TerrainOperations(TerrainManager terrainManager)
	{
		this.terrainManager = terrainManager;
	}

	public void FillWith(int startx, int starty, int startz, int endx, int endy, int endz, int value)
	{
		int sizex = endx - startx;
		int sizey = endy - starty;
		int sizez = endz - startz;

		for (int x = 0; x < sizex; x++)
		{
			for (int y = 0; y < sizey; y++)
			{
				for (int z = 0; z < sizez; z++)
				{
					terrainManager.ChangeCell(startx + x, starty + y, startz + z, value);
				}
			}
		}

		//ConsoleLog.LogFormat("filling: {0}, {1}, {2}", startx, starty, startz);

		//startx = (startx - 1) >> 4;
		//startz = (startz - 1) >> 4;
		//endx = (endx + 1) >> 4;
		//endz = (endz + 1) >> 4;
		//sizex = endx - startx;
		//sizez = endz - startz;

		//ConsoleLog.LogFormat("updating chunk: {0}, {1}", startx, startz);

		//for (int x = 0; x < sizex; x++)
		//{
		//    for (int z = 0; z < sizez; z++)
		//    {
		//        terrainManager.QuqueChunkUpdate(terrain.GetChunkIndex(startx + x, startz + z), 3);
		//    }
		//}
	}

}
