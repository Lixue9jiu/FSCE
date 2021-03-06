﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;

public class NewTestScript
{
    [Test]
    public void AllNormalBlockTimeTrial()
    {
        BlockMeshes meshes = new BlockMeshes();
        meshes.Initialize();
        BlocksData data = new BlocksData();
        data.Initialize();

        BlockTerrain.Chunk chunk = new BlockTerrain.Chunk(16, 128, 16);
        MeshGenerator g = new MeshGenerator();

        Stopwatch stopwatch = new Stopwatch();

        int count = BlocksData.Blocks.Length;
        for (int i = 0; i < count; i++)
        {
            if (BlocksData.IsTransparent[i])
            {
                try
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    for (int k = 0; k < 50; k++)
                        BlocksData.NormalBlocks[i].GenerateTerrain(1, 1, 1, i, chunk, g);
                    stopwatch.Stop();
                    UnityEngine.Debug.LogFormat("tested block: {0}; time: {1}ms", BlocksData.Blocks[i].Name, stopwatch.ElapsedMilliseconds);
                }
                catch(System.Exception e)
                {
                    //UnityEngine.Debug.LogException(e);
                }
            }
        }
    }
}
