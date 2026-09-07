#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;
using TMPro;

public class SettingsMenuBuilder : MonoBehaviour
{
    [MenuItem("Tools/อัปเกรดหน้าต่างการตั้งค่าครบวงจร (Build Complete Settings UI)")]
    public static void BuildCompleteSettingsUI()
    {
        // 1. ค้นหา SettingsPanel หรือสร้าง Canvas ใหม่ถ้าไม่มี
        GameObject settingsPanel = GameObject.Find("SettingsPanel");
        if (settingsPanel == null)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.SetParent(canvas.transform, false);
            RectTransform panelRT = settingsPanel.AddComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.sizeDelta = new Vector2(700, 600);

            Image panelBg = settingsPanel.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.12f, 0.95f);
        }

        // 2. ติดตั้ง / ดึง SettingsController
        SettingsController settingsCtrl = settingsPanel.GetComponent<SettingsController>();
        if (settingsCtrl == null)
        {
            settingsCtrl = settingsPanel.AddComponent<SettingsController>();
        }
        settingsCtrl.settingsPanel = settingsPanel;

        // ดึง Font มาตรฐาน
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        // ล้างอ็อบเจกต์ลูกเก่า (ถ้ามี UI ที่เคยสร้างไว้ใน Container)
        Transform contentContainer = settingsPanel.transform.Find("SettingsContent");
        if (contentContainer != null)
        {
            Undo.DestroyObjectImmediate(contentContainer.gameObject);
        }

        // 3. สร้าง ScrollView / Container เนื้อหาการตั้งค่า
        GameObject contentObj = new GameObject("SettingsContent");
        contentObj.transform.SetParent(settingsPanel.transform, false);
        RectTransform contentRT = contentObj.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0.05f, 0.12f);
        contentRT.anchorMax = new Vector2(0.95f, 0.92f);
        contentRT.offsetMin = Vector2.zero;
        contentRT.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = contentObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 14f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // 4. หัวข้อหลัก (Title)
        CreateHeader(contentObj, "⚙️ การตั้งค่าเกม (Settings)", 24, font);

        // ==========================================
        // หมวด 1: ระบบเสียง (Audio)
        // ==========================================
        CreateSectionHeader(contentObj, "🔊 ระบบเสียง (Audio)", font);

        Slider masterSlider; TextMeshProUGUI masterVal;
        CreateSliderRow(contentObj, "ระดับเสียงหลัก (Master):", 0f, 1f, 1f, font, out masterSlider, out masterVal);
        settingsCtrl.masterSlider = masterSlider;
        settingsCtrl.masterValueText = masterVal;
        UnityEventTools.AddPersistentListener(masterSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetMasterVolume));

        Slider bgmSlider; TextMeshProUGUI bgmVal;
        CreateSliderRow(contentObj, "เสียงเพลงประกอบ (BGM):", 0f, 1f, 0.8f, font, out bgmSlider, out bgmVal);
        settingsCtrl.bgmSlider = bgmSlider;
        settingsCtrl.bgmValueText = bgmVal;
        UnityEventTools.AddPersistentListener(bgmSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetBGMVolume));

        Slider sfxSlider; TextMeshProUGUI sfxVal;
        CreateSliderRow(contentObj, "เสียงเอฟเฟกต์ (SFX):", 0f, 1f, 1f, font, out sfxSlider, out sfxVal);
        settingsCtrl.sfxSlider = sfxSlider;
        settingsCtrl.sfxValueText = sfxVal;
        UnityEventTools.AddPersistentListener(sfxSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetSFXVolume));

        // ==========================================
        // หมวด 2: การแสดงผล & กราฟิก (Display)
        // ==========================================
        CreateSectionHeader(contentObj, "🖥️ การแสดงผล & กราฟิก (Display & Graphics)", font);

        Toggle fullToggle;
        CreateToggleRow(contentObj, "เปิดโหมดเต็มจอ (Fullscreen):", true, font, out fullToggle);
        settingsCtrl.fullscreenToggle = fullToggle;
        UnityEventTools.AddPersistentListener(fullToggle.onValueChanged, new UnityAction<bool>(settingsCtrl.SetFullscreen));

        Toggle vsyncToggle;
        CreateToggleRow(contentObj, "เปิด VSync (ล็อกเฟรมเรต ป้องกันภาพฉีก):", true, font, out vsyncToggle);
        settingsCtrl.vSyncToggle = vsyncToggle;
        UnityEventTools.AddPersistentListener(vsyncToggle.onValueChanged, new UnityAction<bool>(settingsCtrl.SetVSync));

        TMP_Dropdown qualDropdown;
        CreateDropdownRow(contentObj, "คุณภาพกราฟิก (Quality Preset):", font, out qualDropdown);
        settingsCtrl.qualityDropdown = qualDropdown;
        UnityEventTools.AddPersistentListener(qualDropdown.onValueChanged, new UnityAction<int>(settingsCtrl.SetQuality));

        // ==========================================
        // หมวด 3: การควบคุม 3D (Controls)
        // ==========================================
        CreateSectionHeader(contentObj, "🎮 การควบคุมมุมมอง 3D (Controls)", font);

        Slider mouseSensSlider; TextMeshProUGUI mouseSensVal;
        CreateSliderRow(contentObj, "ความไวเมาส์ 3D (Sensitivity):", 0.2f, 3.0f, 1.0f, font, out mouseSensSlider, out mouseSensVal);
        settingsCtrl.mouseSensitivitySlider = mouseSensSlider;
        settingsCtrl.mouseSensitivityValueText = mouseSensVal;
        UnityEventTools.AddPersistentListener(mouseSensSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetMouseSensitivity));

        // ==========================================
        // หมวด 4: ปุ่มควบคุมด้านล่าง (Bottom Buttons)
        // ==========================================
        Transform oldBtnBar = settingsPanel.transform.Find("BottomButtonBar");
        if (oldBtnBar != null) Undo.DestroyObjectImmediate(oldBtnBar.gameObject);

        GameObject btnBar = new GameObject("BottomButtonBar");
        btnBar.transform.SetParent(settingsPanel.transform, false);
        RectTransform btnBarRT = btnBar.AddComponent<RectTransform>();
        btnBarRT.anchorMin = new Vector2(0.05f, 0.02f);
        btnBarRT.anchorMax = new Vector2(0.95f, 0.10f);
        btnBarRT.offsetMin = Vector2.zero;
        btnBarRT.offsetMax = Vector2.zero;

        HorizontalLayoutGroup btnLayout = btnBar.AddComponent<HorizontalLayoutGroup>();
        btnLayout.spacing = 20f;
        btnLayout.childControlWidth = true;
        btnLayout.childControlHeight = true;

        // ปุ่มคืนค่าเริ่มต้น
        Button resetBtn = CreateButton(btnBar, "🔄 คืนค่าเริ่มต้น (Reset Defaults)", new Color(0.7f, 0.3f, 0.3f, 1f), font);
        UnityEventTools.AddPersistentListener(resetBtn.onClick, new UnityAction(settingsCtrl.ResetToDefaults));

        // ปุ่มปิด / บันทึก
        Button closeBtn = CreateButton(btnBar, "💾 บันทึกและปิด (Save & Close)", new Color(0.2f, 0.6f, 0.4f, 1f), font);
        UnityEventTools.AddPersistentListener(closeBtn.onClick, new UnityAction(settingsCtrl.CloseSettings));

        // โหลดและซิงค์ค่าทั้งหมด
        settingsCtrl.LoadAllSettings();

        Selection.activeGameObject = settingsPanel;
        Debug.Log("⚙️✨ [SettingsBuilder] อัปเกรดหน้าต่างการตั้งค่า (Settings UI) ครบวงจรสมบูรณ์แบบเรียบร้อยแล้วครับ!");
    }

    private static void CreateHeader(GameObject parent, string text, float size, TMP_FontAsset font)
    {
        GameObject textObj = new GameObject("TitleText");
        textObj.transform.SetParent(parent.transform, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 35);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        if (font != null) tmp.font = font;
    }

    private static void CreateSectionHeader(GameObject parent, string text, TMP_FontAsset font)
    {
        GameObject textObj = new GameObject("SectionHeader");
        textObj.transform.SetParent(parent.transform, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 26);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 17;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = new Color(0.95f, 0.8f, 0.4f, 1f); // Warm Gold
        if (font != null) tmp.font = font;
    }

    private static void CreateSliderRow(GameObject parent, string label, float min, float max, float def, TMP_FontAsset font, out Slider outSlider, out TextMeshProUGUI outValText)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(0, 32);

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10f;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(250, 30);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 15;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = Color.white;
        if (font != null) labelText.font = font;

        // Slider Object
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(row.transform, false);
        RectTransform sliderRT = sliderObj.AddComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(260, 20);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRT = bgObj.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.25f);
        bgRT.anchorMax = new Vector2(1, 0.75f);
        bgRT.sizeDelta = Vector2.zero;
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.25f, 0.28f, 1f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.25f);
        fillAreaRT.anchorMax = new Vector2(1, 0.75f);
        fillAreaRT.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fillObj.AddComponent<RectTransform>();
        fillRT.sizeDelta = Vector2.zero;
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.65f, 0.95f, 1f); // Blue fill

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRT = handleArea.AddComponent<RectTransform>();
        handleAreaRT.anchorMin = new Vector2(0, 0);
        handleAreaRT.anchorMax = new Vector2(1, 1);
        handleAreaRT.sizeDelta = Vector2.zero;

        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleArea.transform, false);
        RectTransform handleRT = handleObj.AddComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(20, 0);
        Image handleImg = handleObj.AddComponent<Image>();
        handleImg.color = Color.white;

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImg;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = def;
        outSlider = slider;

        // Value Text
        GameObject valObj = new GameObject("ValueText");
        valObj.transform.SetParent(row.transform, false);
        RectTransform valRT = valObj.AddComponent<RectTransform>();
        valRT.sizeDelta = new Vector2(60, 30);
        TextMeshProUGUI valText = valObj.AddComponent<TextMeshProUGUI>();
        valText.text = "100%";
        valText.fontSize = 14;
        valText.alignment = TextAlignmentOptions.Center;
        valText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        if (font != null) valText.font = font;
        outValText = valText;
    }

    private static void CreateToggleRow(GameObject parent, string label, bool def, TMP_FontAsset font, out Toggle outToggle)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(0, 32);

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10f;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(350, 30);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 15;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = Color.white;
        if (font != null) labelText.font = font;

        // Toggle Box
        GameObject toggleObj = new GameObject("Toggle");
        toggleObj.transform.SetParent(row.transform, false);
        RectTransform toggleRT = toggleObj.AddComponent<RectTransform>();
        toggleRT.sizeDelta = new Vector2(30, 30);

        Image bgImg = toggleObj.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.25f, 0.28f, 1f);

        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(toggleObj.transform, false);
        RectTransform checkRT = checkObj.AddComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.2f, 0.2f);
        checkRT.anchorMax = new Vector2(0.8f, 0.8f);
        checkRT.sizeDelta = Vector2.zero;

        Image checkImg = checkObj.AddComponent<Image>();
        checkImg.color = new Color(0.3f, 0.8f, 0.4f, 1f); // Green Checkmark

        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.graphic = checkImg;
        toggle.targetGraphic = bgImg;
        toggle.isOn = def;
        outToggle = toggle;
    }

    private static void CreateDropdownRow(GameObject parent, string label, TMP_FontAsset font, out TMP_Dropdown outDropdown)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(0, 35);

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10f;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(250, 30);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 15;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = Color.white;
        if (font != null) labelText.font = font;

        // Dropdown
        GameObject ddObj = new GameObject("Dropdown");
        ddObj.transform.SetParent(row.transform, false);
        RectTransform ddRT = ddObj.AddComponent<RectTransform>();
        ddRT.sizeDelta = new Vector2(300, 32);

        Image ddBg = ddObj.AddComponent<Image>();
        ddBg.color = new Color(0.22f, 0.22f, 0.25f, 1f);

        GameObject captionObj = new GameObject("CaptionText");
        captionObj.transform.SetParent(ddObj.transform, false);
        RectTransform capRT = captionObj.AddComponent<RectTransform>();
        capRT.anchorMin = new Vector2(0.05f, 0);
        capRT.anchorMax = new Vector2(0.95f, 1);
        capRT.sizeDelta = Vector2.zero;

        TextMeshProUGUI capText = captionObj.AddComponent<TextMeshProUGUI>();
        capText.fontSize = 14;
        capText.alignment = TextAlignmentOptions.Left;
        capText.color = Color.white;
        if (font != null) capText.font = font;

        TMP_Dropdown dropdown = ddObj.AddComponent<TMP_Dropdown>();
        dropdown.targetGraphic = ddBg;
        dropdown.captionText = capText;
        outDropdown = dropdown;
    }

    private static Button CreateButton(GameObject parent, string text, Color color, TMP_FontAsset font)
    {
        GameObject btnObj = new GameObject("Button_" + text);
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRT = btnObj.AddComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(200, 45);

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;

        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = text;
        btnText.fontSize = 14;
        btnText.fontStyle = FontStyles.Bold;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        if (font != null) btnText.font = font;

        return btn;
    }
}
#endif
