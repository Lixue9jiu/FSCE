using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
#if UNITY_STANDALONE
using UnityEngine.PostProcessing;
using UnityEditor.SceneManagement;
#endif

using System.IO;

public class PostProcessingManager : UnityEditor.Build.IActiveBuildTargetChanged, UnityEditor.Build.IPreprocessBuildWithReport
{
	readonly MaterialShaderData[] materials =
	{
		new MaterialShaderData
		{
			Name = "Assets/Materials/VertexColor.mat",
			PCShader = "Assets/Shaders/VertexColor.shader",
			MobileShader = "Assets/Shaders/Mobile-VertexLit.shader"
		},
		new MaterialShaderData
		{
			Name = "Assets/Materials/AlphaTest.mat",
			PCShader = "Assets/Shaders/VCAlphaTest.shader",
			MobileShader = "Assets/Shaders/Mobile-AlphaTest.shader"
		}
	};

	struct MaterialShaderData
	{
		public string Name;
		public string PCShader;
		public string MobileShader;
	}

	public int callbackOrder
	{
		get
		{
			return 1;
		}
	}

	void ChangePostProcessing(bool needPostProcessing)
	{
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
	}

	void ChangePostProcessingComponent(bool needPostProcessing)
	{
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
		switch (newTarget)
		{
			case BuildTarget.Android:
			case BuildTarget.iOS:
			case BuildTarget.WebGL:
				ChangePostProcessing(false);
				ChangeShaders(false);
				break;
			default:
				ChangePostProcessing(true);
				ChangeShaders(true);
				break;
		}
	}

	public void ChangeShaders(bool isPC)
	{
		foreach (MaterialShaderData data in materials)
		{
			Material material = AssetDatabase.LoadAssetAtPath<Material>(data.Name);
			string shaderName = isPC ? data.PCShader : data.MobileShader;
			material.shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderName);
		}
	}

	public void OnPreprocessBuild(BuildReport report)
	{
		switch (EditorUserBuildSettings.activeBuildTarget)
		{
			case BuildTarget.Android:
			case BuildTarget.iOS:
			case BuildTarget.WebGL:
				ChangePostProcessingComponent(false);
				break;
			default:
				ChangePostProcessingComponent(true);
				break;
		}
	}
}
