using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("Audio Settings (ระบบเสียง)")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public TextMeshProUGUI masterValueText;
    public Slider bgmSlider;
    public TextMeshProUGUI bgmValueText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValueText;

    [Header("Graphics & Display Settings (การแสดงผล)")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle vSyncToggle;
    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessValueText;

    [Header("Controls Settings (การควบคุม 3D)")]
    public Slider mouseSensitivitySlider;
    public TextMeshProUGUI mouseSensitivityValueText;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeResolutions();
        InitializeQualityPresets();
        LoadAllSettings();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // ==========================================
    // 1. ระบบจัดการความละเอียดหน้าจอ (Resolutions)
    // ==========================================
    private void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // หากตรวจไม่พบความละเอียดมาตรฐานจากจอ ให้ใส่ Resolution 16:9 มาตรฐาน
        if (options.Count == 0)
        {
            options.Add("1920 x 1080");
            options.Add("1600 x 900");
            options.Add("1366 x 768");
            options.Add("1280 x 720");
        }

        resolutionDropdown.AddOptions(options);

        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        if (savedResIndex < options.Count)
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
            qualityNames.AddRange(new string[] { "ต่ำ (Low)", "ปานกลาง (Medium)", "สูง (High)", "สวยงามสูงสุด (Ultra)" });
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
        // 1. เสียงหลัก (Master)
        float master = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        if (masterSlider != null) masterSlider.value = master;
        SetMasterVolume(master);

        // 2. เสียงเพลง (BGM)
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        if (bgmSlider != null) bgmSlider.value = bgm;
        SetBGMVolume(bgm);

        // 3. เสียงเอฟเฟกต์ (SFX)
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        if (sfxSlider != null) sfxSlider.value = sfx;
        SetSFXVolume(sfx);

        // 4. เต็มจอ (Fullscreen)
        bool isFull = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;
        SetFullscreen(isFull);

        // 5. VSync
        bool vsync = PlayerPrefs.GetInt("VSync", 1) == 1;
        if (vSyncToggle != null) vSyncToggle.isOn = vsync;
        SetVSync(vsync);

        // 6. ความไวเมาส์ (Mouse Sensitivity)
        float sens = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = sens;
        SetMouseSensitivity(sens);

        // 7. ความสว่าง (Brightness)
        float bright = PlayerPrefs.GetFloat("Brightness", 1.0f);
        if (brightnessSlider != null) brightnessSlider.value = bright;
        SetBrightness(bright);
    }

    // ==========================================
    // 4. ฟังก์ชันการตั้งค่าต่างๆ (เชื่อมต่อกับ UI Events)
    // ==========================================

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        AudioListener.volume = volume;

        if (mainMixer != null)
        {
            float db = Mathf.Log10(volume) * 20f;
            mainMixer.SetFloat("MasterVolume", db);
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
            mainMixer.SetFloat("BGMVolume", db);
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
            mainMixer.SetFloat("SFXVolume", db);
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
        if (resolutions != null && resolutionIndex < resolutions.Length)
        {
            Resolution res = resolutions[resolutionIndex];
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

    // รีเซ็ตการตั้งค่าทั้งหมดกลับเป็นค่าเริ่มต้น
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

        SetMouseSensitivity(1.0f);
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = 1.0f;

        SetBrightness(1.0f);
        if (brightnessSlider != null) brightnessSlider.value = 1.0f;

        PlayerPrefs.Save();
        Debug.Log("⚙️ [SettingsController] คืนค่าการตั้งค่าทั้งหมดเป็นค่ามาตรฐานเรียบร้อยแล้ว!");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadAllSettings(); // รีเฟรชค่าล่าสุดทุกครั้งที่เปิดหน้าต่าง
        }
    }

    public void CloseSettings()
    {
        PlayerPrefs.Save(); // บันทึกค่าลงดิสก์ทันทีเมื่อปิดหน้าต่าง
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
