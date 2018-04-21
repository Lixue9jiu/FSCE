using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseWindow
{

    public Slider viewDistanceSlider;
    public Button imageQualityButton;
    public Text blocks;

    public Button vsyncButton;

    public Slider ambientSlider;

    void UpdateUI()
    {
		viewDistanceSlider.value = Mathf.Log(GameSettings.viewDistance, 2) - 2;
        SetBlockStr();

        SetImageQualityStr();

        SetVSyncStr();

		ambientSlider.value = GameSettings.ambientBrightness;
    }

    public override void Show()
    {
        base.Show();
        UpdateUI();
    }

	public override void Hide ()
	{
		GameSettings.SaveSettings();
		base.Hide ();
	}

    public void OnViewDistanceChange(float num)
    {
		GameSettings.viewDistance = 1 << ((int)num + 2);
        SetBlockStr();
    }

    public void OnImageQuialityClicked()
    {
		GameSettings.imageQuality++;
		GameSettings.imageQuality %= QualitySettings.names.Length;
        SetImageQualityStr();
    }

    public void OnVSyncClicked()
    {
		GameSettings.IsVSyncOn = !GameSettings.IsVSyncOn;
        SetVSyncStr();
    }

    public void OnAmbientBrightnessChange(float value)
    {
		GameSettings.ambientBrightness = value;
    }

    public void OnOkButtonClicked()
    {
        Hide();
    }

    public void OnDefaultButtonClicked()
    {
        PlayerPrefs.DeleteAll();
		GameSettings.LoadSettings();
        UpdateUI();
    }

    public void OnOpenScreenshotFolderClicked()
    {
        OpenInFileBrowser.Open(ScreenshotManager.ScreenshotFolder);
    }

    void SetBlockStr()
    {
		blocks.text = string.Format("{0} {1}", GameSettings.viewDistance, LanguageManager.GetString("chunks"));
    }

    void SetImageQualityStr()
    {
		imageQualityButton.GetComponentInChildren<Text>().text = LanguageManager.GetString(QualitySettings.names[GameSettings.imageQuality]);
    }

    void SetVSyncStr()
    {
		vsyncButton.GetComponentInChildren<Text>().text = LanguageManager.GetString(GameSettings.IsVSyncOn ? "on" : "off");
    }
}
