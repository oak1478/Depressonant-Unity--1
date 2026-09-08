using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance { get; private set; }
    public static AudioMixerGroup MasterGroup { get; private set; }
    public static AudioMixerGroup BGMGroup { get; private set; }
    public static AudioMixerGroup SFXGroup { get; private set; }

    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("Category Tabs (หมวดหมู่การตั้งค่า)")]
    public GameObject audioTabPanel;
    public GameObject graphicsTabPanel;
    public GameObject controlsTabPanel;

    public Button audioTabButton;
    public Button graphicsTabButton;
    public Button controlsTabButton;

    public Image audioTabIndicator;
    public Image graphicsTabIndicator;
    public Image controlsTabIndicator;

    [Header("Audio Settings (ระบบเสียง)")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public TextMeshProUGUI masterValueText;
    public Slider bgmSlider;
    public TextMeshProUGUI bgmValueText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValueText;

    [Header("Graphics and Display Settings (การแสดงผล)")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle vSyncToggle;
    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessValueText;

    [Header("Controls Settings (การควบคุม 3D)")]
    public Slider mouseSensitivitySlider;
    public TextMeshProUGUI mouseSensitivityValueText;

    private List<Resolution> filteredResolutions = new List<Resolution>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        RefreshMixerGroups();
    }

    public void RefreshMixerGroups()
    {
        if (mainMixer != null)
        {
            var master = mainMixer.FindMatchingGroups("Master");
            if (master != null && master.Length > 0) MasterGroup = master[0];

            var bgm = mainMixer.FindMatchingGroups("BGM");
            if (bgm != null && bgm.Length > 0) BGMGroup = bgm[0];

            var sfx = mainMixer.FindMatchingGroups("SFX");
            if (sfx != null && sfx.Length > 0) SFXGroup = sfx[0];
        }
    }

    public static void RouteToSFX(AudioSource source)
    {
        if (source != null && source.outputAudioMixerGroup == null)
        {
            if (SFXGroup != null)
            {
                source.outputAudioMixerGroup = SFXGroup;
            }
            else if (Instance != null && Instance.mainMixer != null)
            {
                var sfx = Instance.mainMixer.FindMatchingGroups("SFX");
                if (sfx != null && sfx.Length > 0)
                {
                    SFXGroup = sfx[0];
                    source.outputAudioMixerGroup = SFXGroup;
                }
            }
        }
    }

    public static void RouteToBGM(AudioSource source)
    {
        if (source != null && source.outputAudioMixerGroup == null)
        {
            if (BGMGroup != null)
            {
                source.outputAudioMixerGroup = BGMGroup;
            }
            else if (Instance != null && Instance.mainMixer != null)
            {
                var bgm = Instance.mainMixer.FindMatchingGroups("BGM");
                if (bgm != null && bgm.Length > 0)
                {
                    BGMGroup = bgm[0];
                    source.outputAudioMixerGroup = BGMGroup;
                }
            }
        }
    }

    private void Start()
    {
        InitializeResolutions();
        InitializeQualityPresets();
        LoadAllSettings();
        ShowAudioTab();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // ==========================================
    // 0. ระบบสลับแท็บหมวดหมู่ (Tabs Management)
    // ==========================================

    public void ShowAudioTab()
    {
        if (audioTabPanel != null) audioTabPanel.SetActive(true);
        if (graphicsTabPanel != null) graphicsTabPanel.SetActive(false);
        if (controlsTabPanel != null) controlsTabPanel.SetActive(false);
        UpdateTabIndicators(0);
    }

    public void ShowGraphicsTab()
    {
        if (audioTabPanel != null) audioTabPanel.SetActive(false);
        if (graphicsTabPanel != null) graphicsTabPanel.SetActive(true);
        if (controlsTabPanel != null) controlsTabPanel.SetActive(false);
        UpdateTabIndicators(1);
    }

    public void ShowControlsTab()
    {
        if (audioTabPanel != null) audioTabPanel.SetActive(false);
        if (graphicsTabPanel != null) graphicsTabPanel.SetActive(false);
        if (controlsTabPanel != null) controlsTabPanel.SetActive(true);
        UpdateTabIndicators(2);
    }

    private void UpdateTabIndicators(int activeTabIndex)
    {
        Color activeTextColor = Color.white;
        Color inactiveTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        if (audioTabIndicator != null) audioTabIndicator.gameObject.SetActive(activeTabIndex == 0);
        SetButtonTextColor(audioTabButton, activeTabIndex == 0 ? activeTextColor : inactiveTextColor);

        if (graphicsTabIndicator != null) graphicsTabIndicator.gameObject.SetActive(activeTabIndex == 1);
        SetButtonTextColor(graphicsTabButton, activeTabIndex == 1 ? activeTextColor : inactiveTextColor);

        if (controlsTabIndicator != null) controlsTabIndicator.gameObject.SetActive(activeTabIndex == 2);
        SetButtonTextColor(controlsTabButton, activeTabIndex == 2 ? activeTextColor : inactiveTextColor);
    }

    private void SetButtonTextColor(Button btn, Color color)
    {
        if (btn == null) return;
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.color = color;
        }
    }

    // ==========================================
    // 1. ระบบจัดการความละเอียดหน้าจอ (Resolutions)
    // ==========================================
    private void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;

        Resolution[] systemResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        HashSet<string> seenResolutions = new HashSet<string>();

        for (int i = 0; i < systemResolutions.Length; i++)
        {
            string resKey = systemResolutions[i].width + "x" + systemResolutions[i].height;
            if (!seenResolutions.Contains(resKey))
            {
                seenResolutions.Add(resKey);
                filteredResolutions.Add(systemResolutions[i]);

                string option = systemResolutions[i].width + " x " + systemResolutions[i].height;
                options.Add(option);

                if (systemResolutions[i].width == Screen.width &&
                    systemResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        if (options.Count == 0)
        {
            options.Add("1920 x 1080");
            options.Add("1600 x 900");
            options.Add("1366 x 768");
            options.Add("1280 x 720");
        }

        resolutionDropdown.AddOptions(options);

        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        if (savedResIndex >= 0 && savedResIndex < options.Count)
        {
            resolutionDropdown.value = savedResIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    // ==========================================
    // 2. ระบบจัดการระดับกราฟิก (Quality Presets)
    // ==========================================
    private void InitializeQualityPresets()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();
        List<string> qualityNames = new List<string>(QualitySettings.names);
        if (qualityNames.Count == 0)
        {
            qualityNames.AddRange(new string[] { "Very Low", "Low", "Medium", "High", "Very High", "Ultra" });
        }
        qualityDropdown.AddOptions(qualityNames);

        int savedQuality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = Mathf.Clamp(savedQuality, 0, qualityNames.Count - 1);
        qualityDropdown.RefreshShownValue();
    }

    // ==========================================
    // 3. โหลดการตั้งค่าทั้งหมดที่เคยเซฟไว้
    // ==========================================
    public void LoadAllSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        if (masterSlider != null) masterSlider.value = master;
        SetMasterVolume(master);

        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        if (bgmSlider != null) bgmSlider.value = bgm;
        SetBGMVolume(bgm);

        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        if (sfxSlider != null) sfxSlider.value = sfx;
        SetSFXVolume(sfx);

        bool isFull = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;
        SetFullscreen(isFull);

        bool vsync = PlayerPrefs.GetInt("VSync", 1) == 1;
        if (vSyncToggle != null) vSyncToggle.isOn = vsync;
        SetVSync(vsync);

        float sens = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = sens;
        SetMouseSensitivity(sens);

        float bright = PlayerPrefs.GetFloat("Brightness", 1.0f);
        if (brightnessSlider != null) brightnessSlider.value = bright;
        SetBrightness(bright);
    }

    // ==========================================
    // 4. ฟังก์ชันการตั้งค่าต่างๆ (เชื่อมต่อกับ UI Events)
    // ==========================================

    public void SetVolume(float volume)
    {
        SetMasterVolume(volume);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        AudioListener.volume = volume;

        if (mainMixer != null)
        {
            float db = Mathf.Log10(volume) * 20f;
            try { mainMixer.SetFloat("MasterVolume", db); } catch { }
        }

        if (masterValueText != null)
        {
            masterValueText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }

        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        if (mainMixer != null)
        {
            float db = Mathf.Log10(volume) * 20f;
            try { mainMixer.SetFloat("BGMVolume", db); } catch { }
        }

        if (bgmValueText != null)
        {
            bgmValueText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }

        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        if (mainMixer != null)
        {
            float db = Mathf.Log10(volume) * 20f;
            try { mainMixer.SetFloat("SFXVolume", db); } catch { }
        }

        if (sfxValueText != null)
        {
            sfxValueText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (filteredResolutions != null && resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count)
        {
            Resolution res = filteredResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSync ? 1 : 0);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        sensitivity = Mathf.Clamp(sensitivity, 0.1f, 3.0f);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);

        if (mouseSensitivityValueText != null)
        {
            mouseSensitivityValueText.text = sensitivity.ToString("F1") + "x";
        }
    }

    public void SetBrightness(float brightness)
    {
        brightness = Mathf.Clamp(brightness, 0.5f, 1.5f);
        RenderSettings.ambientLight = Color.white * brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);

        if (brightnessValueText != null)
        {
            brightnessValueText.text = Mathf.RoundToInt(brightness * 100f) + "%";
        }
    }

    public void ResetToDefaults()
    {
        SetMasterVolume(1.0f);
        if (masterSlider != null) masterSlider.value = 1.0f;

        SetBGMVolume(0.8f);
        if (bgmSlider != null) bgmSlider.value = 0.8f;

        SetSFXVolume(1.0f);
        if (sfxSlider != null) sfxSlider.value = 1.0f;

        SetFullscreen(true);
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;

        SetVSync(true);
        if (vSyncToggle != null) vSyncToggle.isOn = true;

        int defaultQuality = QualitySettings.names.Length > 2 ? 2 : 0;
        SetQuality(defaultQuality);
        if (qualityDropdown != null) qualityDropdown.value = defaultQuality;

        SetMouseSensitivity(1.0f);
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = 1.0f;

        SetBrightness(1.0f);
        if (brightnessSlider != null) brightnessSlider.value = 1.0f;

        PlayerPrefs.Save();
        Debug.Log("[SettingsController] คืนค่าการตั้งค่าทั้งหมดเป็นค่ามาตรฐานเรียบร้อยแล้ว");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadAllSettings();
            ShowAudioTab();
        }
    }

    public void CloseSettings()
    {
        PlayerPrefs.Save();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
