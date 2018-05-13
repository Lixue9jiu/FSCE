﻿using UnityEditor;
using UnityEngine;
#if UNITY_STANDALONE
using UnityEngine.PostProcessing;
using UnityEditor.SceneManagement;
#endif

using System.IO;

public class PostProcessingManager : UnityEditor.Build.IActiveBuildTargetChanged
{
	public int callbackOrder
	{
		get
		{
			return 1;
		}
	}

	void ChangePostProcessing(bool needPostProcessing)
	{
		//Object.DestroyImmediate(Object.FindObjectOfType<PostProcessingBehaviour>());      
        if (needPostProcessing)
        {
            if (Directory.Exists("Assets/PostProcessing~"))
            {
                Directory.Move("Assets/PostProcessing~", "Assets/PostProcessing");
            }
        }
        else
        {
            if (Directory.Exists("Assets/PostProcessing"))
            {
                Directory.Move("Assets/PostProcessing", "Assets/PostProcessing~");
            }
        }

#if UNITY_STANDALONE
		var scene = EditorSceneManager.OpenScene("Assets/_Scenes/MainScene.unity");

		foreach (GameObject obj in scene.GetRootGameObjects())
		{
			Camera camera = obj.GetComponent<Camera>();
			if (camera != null)
			{            
				var post = camera.GetComponent<PostProcessingBehaviour>();
				if (post != null)
				{
					if (!needPostProcessing)
						Object.DestroyImmediate(post);
				}
				else
				{
					if (needPostProcessing)
					{
						post = camera.gameObject.AddComponent<PostProcessingBehaviour>();
						post.profile = AssetDatabase.LoadAssetAtPath<PostProcessingProfile>("Assets/PPFancy.asset");
					}
				}
			}
		}

		EditorSceneManager.SaveScene(scene);
#endif
	}

	public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
	{
		switch(newTarget)
		{
			case BuildTarget.Android:
			case BuildTarget.iOS:
			case BuildTarget.WebGL:
				ChangePostProcessing(false);
				break;
			default:
				ChangePostProcessing(true);
                break;
		}
	}
}