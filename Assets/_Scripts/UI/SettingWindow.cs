using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseWindow
{
	const string VIEW_DISTANCE = "view_distance";
	const string IMAGE_QUALITY = "image_quality";

	public Slider viewDistanceSlider;
	public Button imageQualityButton;
	public Text blocks;

	static int viewDistance;
	static int imageQuality;

	public void Initialize ()
	{
		LoadSettings ();
		ApplySettings ();
	}

	void LoadSettings ()
	{
		viewDistance = PlayerPrefs.GetInt (VIEW_DISTANCE, 8);
		imageQuality = PlayerPrefs.GetInt (IMAGE_QUALITY, 0);
	}

	void UpdateUI ()
	{
		viewDistanceSlider.value = Mathf.Log (viewDistance, 2) - 2;
		SetBlockStr ();

		SetImageQualityStr ();
	}

	void SaveSettings ()
	{
		//Debug.LogFormat("save settings {0}, {1}", viewDistance, imageQuality);

		ApplySettings ();

		PlayerPrefs.SetInt (VIEW_DISTANCE, viewDistance);
		PlayerPrefs.SetInt (IMAGE_QUALITY, imageQuality);

		PlayerPrefs.Save ();
	}

	void ApplySettings ()
	{
		BlockTerrain.terrainSize = viewDistance;

		PPManager ppManager = FindObjectOfType<PPManager> ();
		if (ppManager != null) {
			ppManager.SetQuality (imageQuality);
		} else {
			QualitySettings.SetQualityLevel (imageQuality);
		}
	}

	public override void Show ()
	{
		base.Show ();
		UpdateUI ();
	}

	public void OnViewDistanceChange (float num)
	{
		viewDistance = 1 << ((int)num + 2);
		SetBlockStr ();
	}

	public void OnImageQuialityClicked ()
	{
		imageQuality++;
		imageQuality %= QualitySettings.names.Length;
		SetImageQualityStr ();
	}

	public void OnOkButtonClicked ()
	{
		SaveSettings ();
		Hide ();
	}

	public void OnDefaultButtonClicked ()
	{
		PlayerPrefs.DeleteAll ();
		LoadSettings ();
		UpdateUI ();
	}

	public void OnOpenScreenshotFolderClicked ()
	{
		OpenInFileBrowser.Open (ScreenshotManager.ScreenshotFolder);
	}

	void SetBlockStr ()
	{
		blocks.text = string.Format ("{0} {1}", viewDistance, LanguageManager.GetString ("chunks"));
	}

	void SetImageQualityStr ()
	{
		imageQualityButton.GetComponentInChildren<Text> ().text = LanguageManager.GetString (QualitySettings.names [imageQuality]);
	}
}
