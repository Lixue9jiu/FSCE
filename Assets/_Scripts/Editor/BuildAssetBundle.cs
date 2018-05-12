using UnityEngine;
using System.IO;
using UnityEditor;

public class BuildAssetBundle : ScriptableObject, UnityEditor.Build.IActiveBuildTargetChanged
{
	const string assetBundleDirectory = "Assets/AssetBundles";

	public int callbackOrder
	{
		get
		{
			return 1;
		}
	}

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
    {
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        File.Copy(Path.Combine(assetBundleDirectory, "meshes"), "Assets/StreamingAssets/meshes", true);
    }

	public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
	{
		BuildAllAssetBundles();
	}
}
