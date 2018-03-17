using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingWindow : MonoBehaviour
{
    const string VIEW_DISTANCE = "view_distance";
    const string IMAGE_QUALITY = "image_quality";

    static bool hasApplied;

    static GameObject main;

    public Slider viewDistanceSlider;
    public Button imageQualityButton;
    public Text blocks;

    static int viewDistance;
    static int imageQuality;

	void Awake()
	{
        if (!hasApplied)
        {
            LoadSettings();
            SaveSettings();
            hasApplied = true;
        }
	}

	void Start()
	{
        main = gameObject;
        gameObject.SetActive(false);
	}

	void LoadSettings()
    {
        viewDistance = PlayerPrefs.GetInt(VIEW_DISTANCE, 8);
        viewDistanceSlider.value = Mathf.Log(viewDistance, 2) - 2;
        SetBlockStr();

        imageQuality = PlayerPrefs.GetInt(IMAGE_QUALITY, QualitySettings.GetQualityLevel());
        SetImageQualityStr();
    }

	void SaveSettings()
	{
        //Debug.LogFormat("save settings {0}, {1}", viewDistance, imageQuality);

        BlockTerrain.terrainSize = viewDistance;
        QualitySettings.SetQualityLevel(imageQuality);

        PlayerPrefs.SetInt(VIEW_DISTANCE, viewDistance);
        PlayerPrefs.SetInt(IMAGE_QUALITY, imageQuality);

        PlayerPrefs.Save();
	}

	public static void Show()
    {
        main.SetActive(true);
        main.GetComponent<SettingWindow>().OnShow();
    }

    public void OnShow()
    {
        LoadSettings();
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

    public void OnOkButtonClicked()
    {
        SaveSettings();
        main.SetActive(false);
    }

    public void OnDefaultButtonClicked()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        SaveSettings();
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
}
