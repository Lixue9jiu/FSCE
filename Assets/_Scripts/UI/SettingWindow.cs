using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseWindow
{
    const string VIEW_DISTANCE = "view_distance";
    const string IMAGE_QUALITY = "image_quality";
    const string V_SYNC = "is_v_sync_on";
    const string AMBIENT_BRIGHTNESS = "ambient_brightness";

    public Slider viewDistanceSlider;
    public Button imageQualityButton;
    public Text blocks;

    public Button vsyncButton;

    public Slider ambientSlider;

    static int viewDistance;
    static int imageQuality;
    static float ambientBrightness;
    static bool IsVSyncOn;

    public void Initialize()
    {
        LoadSettings();
        ApplySettings();
    }

    void LoadSettings()
    {
        viewDistance = PlayerPrefs.GetInt(VIEW_DISTANCE, 8);
        imageQuality = PlayerPrefs.GetInt(IMAGE_QUALITY, 0);
        IsVSyncOn = PlayerPrefs.GetInt(V_SYNC, 0) == 0;
        ambientBrightness = PlayerPrefs.GetFloat(AMBIENT_BRIGHTNESS, 1f);
    }

    void UpdateUI()
    {
        viewDistanceSlider.value = Mathf.Log(viewDistance, 2) - 2;
        SetBlockStr();

        SetImageQualityStr();

        SetVSyncStr();

        ambientSlider.value = ambientBrightness;
    }

    void SaveSettings()
    {
        //Debug.LogFormat("save settings {0}, {1}", viewDistance, imageQuality);

        ApplySettings();

        PlayerPrefs.SetInt(VIEW_DISTANCE, viewDistance);
        PlayerPrefs.SetInt(IMAGE_QUALITY, imageQuality);
        PlayerPrefs.SetInt(V_SYNC, IsVSyncOn ? 0 : 1);
        PlayerPrefs.SetFloat(AMBIENT_BRIGHTNESS, ambientBrightness);

        PlayerPrefs.Save();
    }

    void ApplySettings()
    {
        BlockTerrain.terrainSize = viewDistance;

        PPManager ppManager = FindObjectOfType<PPManager>();
        if (ppManager != null)
        {
            ppManager.SetQuality(imageQuality);
        }
        else
        {
            QualitySettings.SetQualityLevel(imageQuality);
        }

        QualitySettings.vSyncCount = IsVSyncOn ? 1 : 0;

        RenderSettings.ambientIntensity = ambientBrightness;
    }

    public override void Show()
    {
        base.Show();
        UpdateUI();
    }

    public void OnViewDistanceChange(float num)
    {
        viewDistance = 1 << ((int)num + 2);
        SetBlockStr();
    }

    public void OnImageQuialityClicked()
    {
        imageQuality++;
        imageQuality %= QualitySettings.names.Length;
        SetImageQualityStr();
    }

    public void OnVSyncClicked()
    {
        IsVSyncOn = !IsVSyncOn;
        SetVSyncStr();
    }

    public void OnAmbientBrightnessChange(float value)
    {
        ambientBrightness = value;
    }

    public void OnOkButtonClicked()
    {
        SaveSettings();
        Hide();
    }

    public void OnDefaultButtonClicked()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        UpdateUI();
    }

    public void OnOpenScreenshotFolderClicked()
    {
        OpenInFileBrowser.Open(ScreenshotManager.ScreenshotFolder);
    }

    void SetBlockStr()
    {
        blocks.text = string.Format("{0} {1}", viewDistance, LanguageManager.GetString("chunks"));
    }

    void SetImageQualityStr()
    {
        imageQualityButton.GetComponentInChildren<Text>().text = LanguageManager.GetString(QualitySettings.names[imageQuality]);
    }

    void SetVSyncStr()
    {
        vsyncButton.GetComponentInChildren<Text>().text = LanguageManager.GetString(IsVSyncOn ? "on" : "off");
    }
}
