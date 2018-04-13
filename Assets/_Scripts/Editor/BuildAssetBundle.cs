using UnityEngine;
using System.IO;
using UnityEditor;

public class BuildAssetBundle : ScriptableObject
{
    const string assetBundleDirectory = "Assets/AssetBundles";

    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundlesMac()
    {
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        File.Copy(Path.Combine(assetBundleDirectory, "meshes"), "Assets/StreamingAssets/meshes", true);
    }
}
