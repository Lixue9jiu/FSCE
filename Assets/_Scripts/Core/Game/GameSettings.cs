using UnityEngine;
#if UNITY_STANDALONE
using UnityEngine.SceneManagement;
#endif

public class GameSettings : MonoBehaviour
{
	const string VIEW_DISTANCE = "view_distance";
	const string IMAGE_QUALITY = "image_quality";
	const string V_SYNC = "is_v_sync_on";
	const string AMBIENT_BRIGHTNESS = "ambient_brightness";

	public static int viewDistance;
	public static int imageQuality;
	public static float ambientBrightness;
	public static bool IsVSyncOn;

	static GameSettings instance;

	private void Start()
	{
		instance = this;
		LoadSettingsInternal();
		SaveSettingsInternal();

#if UNITY_STANDALONE
		SceneManager.sceneLoaded += SceneManager_SceneLoaded;;
#endif
	}

	void LoadSettingsInternal()
	{
		viewDistance = PlayerPrefs.GetInt(VIEW_DISTANCE, 8);
		imageQuality = PlayerPrefs.GetInt(IMAGE_QUALITY, 0);
		IsVSyncOn = PlayerPrefs.GetInt(V_SYNC, 0) == 0;
		ambientBrightness = PlayerPrefs.GetFloat(AMBIENT_BRIGHTNESS, 1f);
	}

	void SaveSettingsInternal()
	{
		//Debug.LogFormat("save settings {0}, {1}", viewDistance, imageQuality);

		ApplySettings();

		PlayerPrefs.SetInt(VIEW_DISTANCE, viewDistance);
		PlayerPrefs.SetInt(IMAGE_QUALITY, imageQuality);
		PlayerPrefs.SetInt(V_SYNC, IsVSyncOn ? 0 : 1);
		PlayerPrefs.SetFloat(AMBIENT_BRIGHTNESS, ambientBrightness);

		PlayerPrefs.Save();
	}

	public static void LoadSettings()
	{
		instance.LoadSettingsInternal();
	}

	public static void SaveSettings()
	{
		instance.SaveSettingsInternal();
	}

	public static void ApplySettings()
	{
		Debug.Log("apply settings");

		BlockTerrain.terrainSize = viewDistance;

		QualitySettings.SetQualityLevel(imageQuality);
#if UNITY_STANDALONE
		UpdatePostProcessing();
#endif

		QualitySettings.vSyncCount = IsVSyncOn ? 1 : 0;

		RenderSettings.ambientIntensity = ambientBrightness;
	}

#if UNITY_STANDALONE
	void SceneManager_SceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (arg0.name == "MainScene")
		{
			UpdatePostProcessing();
		}
	}

	static void UpdatePostProcessing()
	{
		Camera.main.gameObject.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled = imageQuality > 0;
	}
#endif
}
