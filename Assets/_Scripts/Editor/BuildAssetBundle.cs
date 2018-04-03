using UnityEngine;
using System.IO;
using UnityEditor;

public class BuildAssetBundle : ScriptableObject
{
    const string assetBundleDirectory = "Assets/AssetBundles";

    [MenuItem("Assets/Build AssetBundles For MacOS")]
    static void BuildAllAssetBundlesMac()
    {
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    }

    [MenuItem("Assets/Build AssetBundles For Windows")]
    static void BuildAllAssetBundlesWin()
    {
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Copy Meshes")]
    static void CopyMeshes()
    {
        CopyMeshes("Assets/Meshes");
    }

    static void CopyMeshes(string dir)
    {
        foreach (string file in Directory.GetFiles(dir))
        {
            File.Copy(file, Path.Combine(assetBundleDirectory, Path.GetFileName(file)), true);
        }
        foreach (string d in Directory.GetDirectories(dir))
        {
            CopyMeshes(d);
        }
    }
}
