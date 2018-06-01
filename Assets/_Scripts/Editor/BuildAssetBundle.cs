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
			return 0;
		}
	}

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
    {
		Debug.Log("building asset bundle");      
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

    [MenuItem("Assets/Save All Meshes Transformed...")]
    public static void TransformAllMesh()
    {
        MeshFilter[] ms = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter m in ms)
        {
            Mesh meshToSave = BlockMeshes.TranslateMeshRaw(m.sharedMesh, m.transform.localToWorldMatrix);

            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Meshes/", meshToSave.name, "asset");
            if (string.IsNullOrEmpty(path)) continue;

            path = FileUtil.GetProjectRelativePath(path);

            MeshUtility.Optimize(meshToSave);

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }
    }
}
