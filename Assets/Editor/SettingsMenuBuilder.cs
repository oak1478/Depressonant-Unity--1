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
    private const string FONT_PATH = "Assets/TextMesh Pro/Fonts/Kanit-Black SDF EditAtlas.asset";
    private const string MENUS_SPRITE_PATH = "Assets/Sprites/Menus.png";

    [MenuItem("Tools/สร้างหน้าต่างตั้งค่าฉบับสมบูรณ์ (Build Complete Tabbed Settings UI)")]
    public static void BuildCompleteTabbedSettingsUI()
    {
        // 1. ค้นหา SettingsPanel ในฉาก
        GameObject settingsPanel = GameObject.Find("SettingsPanel");
        if (settingsPanel == null)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("MainMenuCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler cs = canvasObj.AddComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.SetParent(canvas.transform, false);
        }

        BuildSettingsPanelContent(settingsPanel, isPauseMenu: false, pmc: null);
        Debug.Log("สร้างหน้าต่างตั้งค่า Main Menu สำเร็จเรียบร้อย!");
    }

    [MenuItem("Tools/สร้างหน้าต่างตั้งค่าใน PauseMenu Prefab (Build In-Game Pause Settings UI)")]
    public static void BuildInGamePauseSettingsUI()
    {
        string prefabPath = "Assets/Object/Systems/[GameManager_Core].prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        if (root == null)
        {
            Debug.LogError("ไม่สามารถโหลด [GameManager_Core].prefab ได้");
            return;
        }

        try
        {
            PauseMenuController pmc = root.GetComponentInChildren<PauseMenuController>(true);
            Transform settingsPanelTr = null;
            if (pmc != null && pmc.settingsPanel != null)
            {
                settingsPanelTr = pmc.settingsPanel.transform;
            }
            else
            {
                Transform pauseCanvas = root.transform.Find("PauseMenuCanvas");
                if (pauseCanvas != null) settingsPanelTr = pauseCanvas.Find("SettingsPanel");
            }

            if (settingsPanelTr == null)
            {
                Debug.LogError("ไม่พบ SettingsPanel ใน [GameManager_Core].prefab");
                return;
            }

            BuildSettingsPanelContent(settingsPanelTr.gameObject, isPauseMenu: true, pmc: pmc);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log("สร้างและอัปเดตหน้าต่างตั้งค่าใน [GameManager_Core].prefab สำเร็จเรียบร้อย!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("เกิดข้อผิดพลาดในการสร้าง Settings ใน Prefab: " + ex.Message);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [InitializeOnLoadMethod]
    private static void AutoBuildOnLoad()
    {
        EditorApplication.delayCall += () =>
        {
            try
            {
                string prefabPath = "Assets/Object/Systems/[GameManager_Core].prefab";
                GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
                if (root == null) return;
                try
                {
                    PauseMenuController pmc = root.GetComponentInChildren<PauseMenuController>(true);
                    if (pmc != null && pmc.settingsPanel != null)
                    {
                        if (pmc.settingsPanel.GetComponent<SettingsController>() == null)
                        {
                            BuildSettingsPanelContent(pmc.settingsPanel, isPauseMenu: true, pmc: pmc);
                            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                            Debug.Log("[SettingsMenuBuilder] ดำเนินการอัปเดตหน้าต่างตั้งค่าใน [GameManager_Core].prefab โดยอัตโนมัติสำเร็จ!");
                        }
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[SettingsMenuBuilder] Auto-build deferred: " + ex.Message);
            }
        };
    }

    public static void BuildSettingsPanelContent(GameObject settingsPanel, bool isPauseMenu, PauseMenuController pmc)
    {
        // ปรับ RectTransform ของ SettingsPanel ให้เต็มจอ 1920x1080
        RectTransform panelRT = settingsPanel.GetComponent<RectTransform>();
        if (panelRT == null) panelRT = settingsPanel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // ดึง Font และ Sprite Menus_3
        TMP_FontAsset kanitFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
        Sprite menus3Sprite = GetMenus3Sprite();

        // Background Image ของ SettingsPanel
        Image bgImg = settingsPanel.GetComponent<Image>();
        if (bgImg == null) bgImg = settingsPanel.AddComponent<Image>();
        bgImg.color = Color.white;
        if (menus3Sprite != null) bgImg.sprite = menus3Sprite;

        // ดึงหรือเพิ่ม SettingsController
        SettingsController settingsCtrl = settingsPanel.GetComponentInParent<SettingsController>();
        if (settingsCtrl == null)
        {
            settingsCtrl = settingsPanel.GetComponent<SettingsController>();
            if (settingsCtrl == null) settingsCtrl = settingsPanel.AddComponent<SettingsController>();
        }
        settingsCtrl.settingsPanel = settingsPanel;

        UnityEngine.Audio.AudioMixer mixer = AssetDatabase.LoadAssetAtPath<UnityEngine.Audio.AudioMixer>("Assets/Audio/MainMixer.mixer");
        if (mixer != null) settingsCtrl.mainMixer = mixer;

        // ล้างอ็อบเจกต์ลูกเก่าภายใน SettingsPanel ยกเว้น Background Image
        Transform oldBack = settingsPanel.transform.Find("Back");
        GameObject backBtnObj = null;
        if (oldBack != null)
        {
            backBtnObj = oldBack.gameObject;
        }

        List<GameObject> childrenToDestroy = new List<GameObject>();
        for (int i = 0; i < settingsPanel.transform.childCount; i++)
        {
            Transform child = settingsPanel.transform.GetChild(i);
            if (child.name != "Back" && child.name != "Image")
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (var go in childrenToDestroy)
        {
            Undo.DestroyObjectImmediate(go);
        }

        // ==========================================
        // 1. หัวข้อบนแถบรอยแปรงซ้ายบน (Title Text)
        // ==========================================
        Transform oldTitle = settingsPanel.transform.Find("HeaderTitle");
        if (oldTitle != null) Undo.DestroyObjectImmediate(oldTitle.gameObject);

        GameObject titleObj = new GameObject("HeaderTitle");
        titleObj.transform.SetParent(settingsPanel.transform, false);
        RectTransform titleRT = titleObj.AddComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.5f);
        titleRT.anchorMax = new Vector2(0.5f, 0.5f);
        titleRT.anchoredPosition = new Vector2(-720f, 465f);
        titleRT.sizeDelta = new Vector2(460f, 70f);

        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "OPTIONS";
        titleTMP.fontSize = 38;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        if (kanitFont != null) titleTMP.font = kanitFont;

        // ==========================================
        // 2. ปุ่ม BACK ด้านซ้ายล่าง
        // ==========================================
        if (backBtnObj == null)
        {
            backBtnObj = new GameObject("Back");
            backBtnObj.transform.SetParent(settingsPanel.transform, false);
        }
        RectTransform backRT = backBtnObj.GetComponent<RectTransform>();
        if (backRT == null) backRT = backBtnObj.AddComponent<RectTransform>();
        backRT.anchorMin = new Vector2(0.5f, 0.5f);
        backRT.anchorMax = new Vector2(0.5f, 0.5f);
        backRT.anchoredPosition = new Vector2(-721.88f, -480.01f);
        backRT.sizeDelta = new Vector2(475.81f, 120.23f);

        Button backBtn = backBtnObj.GetComponent<Button>();
        if (backBtn == null) backBtn = backBtnObj.AddComponent<Button>();

        Transform backTextTr = backBtnObj.transform.Find("Text (TMP)");
        GameObject backTextObj = backTextTr != null ? backTextTr.gameObject : new GameObject("Text (TMP)");
        backTextObj.transform.SetParent(backBtnObj.transform, false);
        RectTransform backTextRT = backTextObj.GetComponent<RectTransform>();
        if (backTextRT == null) backTextRT = backTextObj.AddComponent<RectTransform>();
        backTextRT.anchorMin = Vector2.zero;
        backTextRT.anchorMax = Vector2.one;
        backTextRT.offsetMin = Vector2.zero;
        backTextRT.offsetMax = Vector2.zero;

        TextMeshProUGUI backTMP = backTextObj.GetComponent<TextMeshProUGUI>();
        if (backTMP == null) backTMP = backTextObj.AddComponent<TextMeshProUGUI>();
        backTMP.text = "BACK";
        backTMP.fontSize = 36;
        backTMP.fontStyle = FontStyles.Bold;
        backTMP.alignment = TextAlignmentOptions.Center;
        backTMP.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        if (kanitFont != null) backTMP.font = kanitFont;

        backBtn.onClick.RemoveAllListeners();
        if (isPauseMenu && pmc != null)
        {
            UnityEventTools.AddPersistentListener(backBtn.onClick, new UnityAction(pmc.CloseSettings));
        }
        else
        {
            MainMenuController mmc = Object.FindAnyObjectByType<MainMenuController>();
            if (mmc != null)
            {
                UnityEventTools.AddPersistentListener(backBtn.onClick, new UnityAction(mmc.ShowMainMenu));
            }
        }

        // ==========================================
        // 3. คอลัมน์หมวดหมู่แท็บด้านซ้าย (Left Column)
        // ==========================================
        GameObject tabsColObj = new GameObject("CategoryTabs");
        tabsColObj.transform.SetParent(settingsPanel.transform, false);
        RectTransform tabsRT = tabsColObj.AddComponent<RectTransform>();
        tabsRT.anchorMin = new Vector2(0.5f, 0.5f);
        tabsRT.anchorMax = new Vector2(0.5f, 0.5f);
        tabsRT.anchoredPosition = new Vector2(-605f, 25f);
        tabsRT.sizeDelta = new Vector2(360f, 760f);

        // แท็บ 1: AUDIO
        Button audioBtn; Image audioInd;
        CreateTabButton(tabsColObj, "Tab_Audio", "AUDIO\n(ระบบเสียง)", 240f, kanitFont, out audioBtn, out audioInd);
        settingsCtrl.audioTabButton = audioBtn;
        settingsCtrl.audioTabIndicator = audioInd;
        UnityEventTools.AddPersistentListener(audioBtn.onClick, new UnityAction(settingsCtrl.ShowAudioTab));

        // แท็บ 2: GRAPHICS
        Button graphicsBtn; Image graphicsInd;
        CreateTabButton(tabsColObj, "Tab_Graphics", "GRAPHICS\n(การแสดงผล)", 130f, kanitFont, out graphicsBtn, out graphicsInd);
        settingsCtrl.graphicsTabButton = graphicsBtn;
        settingsCtrl.graphicsTabIndicator = graphicsInd;
        UnityEventTools.AddPersistentListener(graphicsBtn.onClick, new UnityAction(settingsCtrl.ShowGraphicsTab));

        // แท็บ 3: CONTROLS
        Button controlsBtn; Image controlsInd;
        CreateTabButton(tabsColObj, "Tab_Controls", "CONTROLS\n(การควบคุม)", 20f, kanitFont, out controlsBtn, out controlsInd);
        settingsCtrl.controlsTabButton = controlsBtn;
        settingsCtrl.controlsTabIndicator = controlsInd;
        UnityEventTools.AddPersistentListener(controlsBtn.onClick, new UnityAction(settingsCtrl.ShowControlsTab));

        // ปุ่มคืนค่าเริ่มต้น (Reset Defaults)
        GameObject resetBtnObj = new GameObject("Btn_ResetDefaults");
        resetBtnObj.transform.SetParent(tabsColObj.transform, false);
        RectTransform resetRT = resetBtnObj.AddComponent<RectTransform>();
        resetRT.anchoredPosition = new Vector2(0f, -270f);
        resetRT.sizeDelta = new Vector2(320f, 65f);

        Image resetImg = resetBtnObj.AddComponent<Image>();
        resetImg.color = new Color(0.35f, 0.18f, 0.18f, 0.9f);

        Button resetBtn = resetBtnObj.AddComponent<Button>();
        resetBtn.targetGraphic = resetImg;
        UnityEventTools.AddPersistentListener(resetBtn.onClick, new UnityAction(settingsCtrl.ResetToDefaults));

        GameObject resetTextObj = new GameObject("Text");
        resetTextObj.transform.SetParent(resetBtnObj.transform, false);
        RectTransform resetTextRT = resetTextObj.AddComponent<RectTransform>();
        resetTextRT.anchorMin = Vector2.zero;
        resetTextRT.anchorMax = Vector2.one;
        resetTextRT.offsetMin = Vector2.zero;
        resetTextRT.offsetMax = Vector2.zero;
        TextMeshProUGUI resetTMP = resetTextObj.AddComponent<TextMeshProUGUI>();
        resetTMP.text = "🔄 คืนค่าเริ่มต้น (RESET)";
        resetTMP.fontSize = 18;
        resetTMP.fontStyle = FontStyles.Bold;
        resetTMP.alignment = TextAlignmentOptions.Center;
        resetTMP.color = Color.white;
        if (kanitFont != null) resetTMP.font = kanitFont;

        // ==========================================
        // 4. แผงเนื้อหาการตั้งค่าด้านขวา (Right Panel)
        // ==========================================
        GameObject contentAreaObj = new GameObject("ContentArea");
        contentAreaObj.transform.SetParent(settingsPanel.transform, false);
        RectTransform contentAreaRT = contentAreaObj.AddComponent<RectTransform>();
        contentAreaRT.anchorMin = new Vector2(0.5f, 0.5f);
        contentAreaRT.anchorMax = new Vector2(0.5f, 0.5f);
        contentAreaRT.anchoredPosition = new Vector2(210f, 25f);
        contentAreaRT.sizeDelta = new Vector2(1050f, 760f);

        // ------------------------------------------
        // A. แท็บเสียง (AudioTabPanel)
        // ------------------------------------------
        GameObject audioPanel = CreateSubPanel(contentAreaObj, "AudioTabPanel");
        settingsCtrl.audioTabPanel = audioPanel;

        CreateSectionTitle(audioPanel, "ระบบควบคุมเสียง (Audio Settings)", 280f, kanitFont);

        Slider masterSlider; TextMeshProUGUI masterVal;
        CreateSliderRow(audioPanel, "ระดับเสียงหลัก (Master Volume)", 180f, 0f, 1f, 1f, kanitFont, out masterSlider, out masterVal);
        settingsCtrl.masterSlider = masterSlider;
        settingsCtrl.masterValueText = masterVal;
        UnityEventTools.AddPersistentListener(masterSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetMasterVolume));

        Slider bgmSlider; TextMeshProUGUI bgmVal;
        CreateSliderRow(audioPanel, "เสียงเพลงประกอบ (BGM Volume)", 70f, 0f, 1f, 0.8f, kanitFont, out bgmSlider, out bgmVal);
        settingsCtrl.bgmSlider = bgmSlider;
        settingsCtrl.bgmValueText = bgmVal;
        UnityEventTools.AddPersistentListener(bgmSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetBGMVolume));

        Slider sfxSlider; TextMeshProUGUI sfxVal;
        CreateSliderRow(audioPanel, "เสียงเอฟเฟกต์ (SFX Volume)", -40f, 0f, 1f, 1f, kanitFont, out sfxSlider, out sfxVal);
        settingsCtrl.sfxSlider = sfxSlider;
        settingsCtrl.sfxValueText = sfxVal;
        UnityEventTools.AddPersistentListener(sfxSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetSFXVolume));

        CreateHintCard(audioPanel, "ระดับเสียงหลักจะควบคุมความดังรวมของเกมทั้งหมด และจะถูกบันทึกอัตโนมัติ", -190f, kanitFont);

        // ------------------------------------------
        // B. แท็บการแสดงผล (GraphicsTabPanel)
        // ------------------------------------------
        GameObject graphicsPanel = CreateSubPanel(contentAreaObj, "GraphicsTabPanel");
        settingsCtrl.graphicsTabPanel = graphicsPanel;

        CreateSectionTitle(graphicsPanel, "การแสดงผลและกราฟิก (Display & Graphics)", 290f, kanitFont);

        Toggle fullToggle;
        CreateToggleRow(graphicsPanel, "โหมดเต็มจอ (Fullscreen Mode)", 210f, true, kanitFont, out fullToggle);
        settingsCtrl.fullscreenToggle = fullToggle;
        UnityEventTools.AddPersistentListener(fullToggle.onValueChanged, new UnityAction<bool>(settingsCtrl.SetFullscreen));

        TMP_Dropdown resDropdown;
        CreateDropdownRow(graphicsPanel, "ความละเอียดหน้าจอ (Resolution)", 120f, kanitFont, out resDropdown);
        settingsCtrl.resolutionDropdown = resDropdown;
        UnityEventTools.AddPersistentListener(resDropdown.onValueChanged, new UnityAction<int>(settingsCtrl.SetResolution));

        TMP_Dropdown qualDropdown;
        CreateDropdownRow(graphicsPanel, "คุณภาพกราฟิก (Graphics Quality)", 30f, kanitFont, out qualDropdown);
        settingsCtrl.qualityDropdown = qualDropdown;
        UnityEventTools.AddPersistentListener(qualDropdown.onValueChanged, new UnityAction<int>(settingsCtrl.SetQuality));

        Toggle vsyncToggle;
        CreateToggleRow(graphicsPanel, "เปิด VSync (ล็อกเฟรมเรต ป้องกันภาพฉีก)", -60f, true, kanitFont, out vsyncToggle);
        settingsCtrl.vSyncToggle = vsyncToggle;
        UnityEventTools.AddPersistentListener(vsyncToggle.onValueChanged, new UnityAction<bool>(settingsCtrl.SetVSync));

        Slider brightSlider; TextMeshProUGUI brightVal;
        CreateSliderRow(graphicsPanel, "ระดับความสว่าง (Brightness)", -150f, 0.5f, 1.5f, 1.0f, kanitFont, out brightSlider, out brightVal);
        settingsCtrl.brightnessSlider = brightSlider;
        settingsCtrl.brightnessValueText = brightVal;
        UnityEventTools.AddPersistentListener(brightSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetBrightness));

        // ------------------------------------------
        // C. แท็บการควบคุม (ControlsTabPanel)
        // ------------------------------------------
        GameObject controlsPanel = CreateSubPanel(contentAreaObj, "ControlsTabPanel");
        settingsCtrl.controlsTabPanel = controlsPanel;

        CreateSectionTitle(controlsPanel, "การควบคุมมุมมองและตัวละคร (Controls)", 290f, kanitFont);

        Slider sensSlider; TextMeshProUGUI sensVal;
        CreateSliderRow(controlsPanel, "ความไวเมาส์ 3D (Mouse Sensitivity)", 200f, 0.1f, 3.0f, 1.0f, kanitFont, out sensSlider, out sensVal);
        settingsCtrl.mouseSensitivitySlider = sensSlider;
        settingsCtrl.mouseSensitivityValueText = sensVal;
        UnityEventTools.AddPersistentListener(sensSlider.onValueChanged, new UnityAction<float>(settingsCtrl.SetMouseSensitivity));

        CreateKeybindsCard(controlsPanel, -80f, kanitFont);

        settingsCtrl.ShowAudioTab();
        settingsCtrl.LoadAllSettings();

        Selection.activeGameObject = settingsPanel;
        Debug.Log("[SettingsMenuBuilder] สร้างหน้าต่างตั้งค่าฉบับสมบูรณ์ (Tabbed Settings UI) เรียบร้อยแล้ว!");
    }

    private static Sprite GetMenus3Sprite()
    {
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(MENUS_SPRITE_PATH);
        foreach (var asset in allAssets)
        {
            if (asset is Sprite s && s.name == "Menus_3")
            {
                return s;
            }
        }
        return null;
    }

    private static void CreateTabButton(GameObject parent, string name, string label, float yPos, TMP_FontAsset font, out Button outBtn, out Image outIndicator)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, yPos);
        rt.sizeDelta = new Vector2(320f, 85f);

        Image bg = btnObj.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.18f, 0.22f, 0.85f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bg;
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.28f, 0.28f, 0.35f, 1f);
        cb.pressedColor = new Color(0.15f, 0.15f, 0.18f, 1f);
        btn.colors = cb;
        outBtn = btn;

        GameObject indObj = new GameObject("Indicator");
        indObj.transform.SetParent(btnObj.transform, false);
        RectTransform indRT = indObj.AddComponent<RectTransform>();
        indRT.anchorMin = new Vector2(0, 0);
        indRT.anchorMax = new Vector2(0, 1);
        indRT.anchoredPosition = new Vector2(4f, 0f);
        indRT.sizeDelta = new Vector2(8f, 0f);
        Image indImg = indObj.AddComponent<Image>();
        indImg.color = new Color(0.95f, 0.85f, 0.4f, 1f);
        outIndicator = indImg;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(25f, 5f);
        textRT.offsetMax = new Vector2(-10f, -5f);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color = Color.white;
        if (font != null) tmp.font = font;
    }

    private static GameObject CreateSubPanel(GameObject parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return panel;
    }

    private static void CreateSectionTitle(GameObject parent, string text, float yPos, TMP_FontAsset font)
    {
        GameObject obj = new GameObject("SectionTitle");
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, yPos);
        rt.sizeDelta = new Vector2(980f, 50f);

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 26;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = new Color(0.95f, 0.85f, 0.45f, 1f);
        if (font != null) tmp.font = font;
    }

    private static void CreateSliderRow(GameObject parent, string label, float yPos, float min, float max, float def, TMP_FontAsset font, out Slider outSlider, out TextMeshProUGUI outValText)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.anchoredPosition = new Vector2(0f, yPos);
        rowRT.sizeDelta = new Vector2(980f, 65f);

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0.5f);
        labelRT.anchorMax = new Vector2(0f, 0.5f);
        labelRT.anchoredPosition = new Vector2(200f, 0f);
        labelRT.sizeDelta = new Vector2(400f, 50f);

        TextMeshProUGUI lblTMP = labelObj.AddComponent<TextMeshProUGUI>();
        lblTMP.text = label;
        lblTMP.fontSize = 22;
        lblTMP.alignment = TextAlignmentOptions.MidlineLeft;
        lblTMP.color = Color.white;
        if (font != null) lblTMP.font = font;

        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(row.transform, false);
        RectTransform sliderRT = sliderObj.AddComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0f, 0.5f);
        sliderRT.anchorMax = new Vector2(0f, 0.5f);
        sliderRT.anchoredPosition = new Vector2(620f, 0f);
        sliderRT.sizeDelta = new Vector2(400f, 26f);

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRT = bgObj.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.22f, 0.22f, 0.26f, 1f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fillObj.AddComponent<RectTransform>();
        fillRT.sizeDelta = Vector2.zero;
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRT = handleArea.AddComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.sizeDelta = Vector2.zero;

        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleArea.transform, false);
        RectTransform handleRT = handleObj.AddComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(30f, 36f);
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

        GameObject valObj = new GameObject("ValueText");
        valObj.transform.SetParent(row.transform, false);
        RectTransform valRT = valObj.AddComponent<RectTransform>();
        valRT.anchorMin = new Vector2(0f, 0.5f);
        valRT.anchorMax = new Vector2(0f, 0.5f);
        valRT.anchoredPosition = new Vector2(900f, 0f);
        valRT.sizeDelta = new Vector2(100f, 40f);

        TextMeshProUGUI valTMP = valObj.AddComponent<TextMeshProUGUI>();
        valTMP.text = "100%";
        valTMP.fontSize = 20;
        valTMP.fontStyle = FontStyles.Bold;
        valTMP.alignment = TextAlignmentOptions.Center;
        valTMP.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        if (font != null) valTMP.font = font;
        outValText = valTMP;
    }

    private static void CreateToggleRow(GameObject parent, string label, float yPos, bool def, TMP_FontAsset font, out Toggle outToggle)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.anchoredPosition = new Vector2(0f, yPos);
        rowRT.sizeDelta = new Vector2(980f, 60f);

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0.5f);
        labelRT.anchorMax = new Vector2(0f, 0.5f);
        labelRT.anchoredPosition = new Vector2(250f, 0f);
        labelRT.sizeDelta = new Vector2(500f, 50f);

        TextMeshProUGUI lblTMP = labelObj.AddComponent<TextMeshProUGUI>();
        lblTMP.text = label;
        lblTMP.fontSize = 22;
        lblTMP.alignment = TextAlignmentOptions.MidlineLeft;
        lblTMP.color = Color.white;
        if (font != null) lblTMP.font = font;

        GameObject toggleObj = new GameObject("Toggle");
        toggleObj.transform.SetParent(row.transform, false);
        RectTransform toggleRT = toggleObj.AddComponent<RectTransform>();
        toggleRT.anchorMin = new Vector2(0f, 0.5f);
        toggleRT.anchorMax = new Vector2(0f, 0.5f);
        toggleRT.anchoredPosition = new Vector2(620f, 0f);
        toggleRT.sizeDelta = new Vector2(46f, 46f);

        Image bgImg = toggleObj.AddComponent<Image>();
        bgImg.color = new Color(0.22f, 0.22f, 0.26f, 1f);

        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(toggleObj.transform, false);
        RectTransform checkRT = checkObj.AddComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.2f, 0.2f);
        checkRT.anchorMax = new Vector2(0.8f, 0.8f);
        checkRT.sizeDelta = Vector2.zero;

        Image checkImg = checkObj.AddComponent<Image>();
        checkImg.color = new Color(0.95f, 0.85f, 0.45f, 1f);

        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.graphic = checkImg;
        toggle.targetGraphic = bgImg;
        toggle.isOn = def;
        outToggle = toggle;
    }

    private static void CreateDropdownRow(GameObject parent, string label, float yPos, TMP_FontAsset font, out TMP_Dropdown outDropdown)
    {
        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent.transform, false);
        RectTransform rowRT = row.AddComponent<RectTransform>();
        rowRT.anchoredPosition = new Vector2(0f, yPos);
        rowRT.sizeDelta = new Vector2(980f, 65f);

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0.5f);
        labelRT.anchorMax = new Vector2(0f, 0.5f);
        labelRT.anchoredPosition = new Vector2(200f, 0f);
        labelRT.sizeDelta = new Vector2(400f, 50f);

        TextMeshProUGUI lblTMP = labelObj.AddComponent<TextMeshProUGUI>();
        lblTMP.text = label;
        lblTMP.fontSize = 22;
        lblTMP.alignment = TextAlignmentOptions.MidlineLeft;
        lblTMP.color = Color.white;
        if (font != null) lblTMP.font = font;

        GameObject ddObj = new GameObject("Dropdown");
        ddObj.transform.SetParent(row.transform, false);
        RectTransform ddRT = ddObj.AddComponent<RectTransform>();
        ddRT.anchorMin = new Vector2(0f, 0.5f);
        ddRT.anchorMax = new Vector2(0f, 0.5f);
        ddRT.anchoredPosition = new Vector2(660f, 0f);
        ddRT.sizeDelta = new Vector2(450f, 50f);

        Image ddBg = ddObj.AddComponent<Image>();
        ddBg.color = new Color(0.2f, 0.2f, 0.24f, 1f);

        GameObject capObj = new GameObject("CaptionText");
        capObj.transform.SetParent(ddObj.transform, false);
        RectTransform capRT = capObj.AddComponent<RectTransform>();
        capRT.anchorMin = new Vector2(0.05f, 0f);
        capRT.anchorMax = new Vector2(0.85f, 1f);
        capRT.sizeDelta = Vector2.zero;

        TextMeshProUGUI capTMP = capObj.AddComponent<TextMeshProUGUI>();
        capTMP.fontSize = 20;
        capTMP.alignment = TextAlignmentOptions.MidlineLeft;
        capTMP.color = Color.white;
        if (font != null) capTMP.font = font;

        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.SetParent(ddObj.transform, false);
        RectTransform arrowRT = arrowObj.AddComponent<RectTransform>();
        arrowRT.anchorMin = new Vector2(0.9f, 0.5f);
        arrowRT.anchorMax = new Vector2(0.9f, 0.5f);
        arrowRT.sizeDelta = new Vector2(30f, 30f);
        TextMeshProUGUI arrowTMP = arrowObj.AddComponent<TextMeshProUGUI>();
        arrowTMP.text = "▼";
        arrowTMP.fontSize = 16;
        arrowTMP.alignment = TextAlignmentOptions.Center;
        arrowTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        GameObject templateObj = new GameObject("Template");
        templateObj.transform.SetParent(ddObj.transform, false);
        RectTransform tempRT = templateObj.AddComponent<RectTransform>();
        tempRT.anchorMin = new Vector2(0f, 0f);
        tempRT.anchorMax = new Vector2(1f, 0f);
        tempRT.pivot = new Vector2(0.5f, 1f);
        tempRT.sizeDelta = new Vector2(0f, 220f);
        Image tempBg = templateObj.AddComponent<Image>();
        tempBg.color = new Color(0.15f, 0.15f, 0.18f, 0.98f);
        ScrollRect scrollRect = templateObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        templateObj.SetActive(false);

        GameObject viewObj = new GameObject("Viewport");
        viewObj.transform.SetParent(templateObj.transform, false);
        RectTransform viewRT = viewObj.AddComponent<RectTransform>();
        viewRT.anchorMin = Vector2.zero;
        viewRT.anchorMax = Vector2.one;
        viewRT.sizeDelta = Vector2.zero;
        viewObj.AddComponent<Mask>().showMaskGraphic = false;
        Image viewImg = viewObj.AddComponent<Image>();
        scrollRect.viewport = viewRT;

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewObj.transform, false);
        RectTransform cRT = contentObj.AddComponent<RectTransform>();
        cRT.anchorMin = new Vector2(0f, 1f);
        cRT.anchorMax = new Vector2(1f, 1f);
        cRT.pivot = new Vector2(0.5f, 1f);
        cRT.sizeDelta = new Vector2(0f, 45f);
        scrollRect.content = cRT;

        GameObject itemObj = new GameObject("Item");
        itemObj.transform.SetParent(contentObj.transform, false);
        RectTransform itemRT = itemObj.AddComponent<RectTransform>();
        itemRT.anchorMin = new Vector2(0f, 0.5f);
        itemRT.anchorMax = new Vector2(1f, 0.5f);
        itemRT.sizeDelta = new Vector2(0f, 40f);
        Toggle itemToggle = itemObj.AddComponent<Toggle>();

        GameObject itemBg = new GameObject("Item Background");
        itemBg.transform.SetParent(itemObj.transform, false);
        RectTransform itemBgRT = itemBg.AddComponent<RectTransform>();
        itemBgRT.anchorMin = Vector2.zero;
        itemBgRT.anchorMax = Vector2.one;
        itemBgRT.sizeDelta = Vector2.zero;
        Image itemBgImg = itemBg.AddComponent<Image>();
        itemBgImg.color = new Color(0.22f, 0.22f, 0.26f, 1f);
        itemToggle.targetGraphic = itemBgImg;

        GameObject itemLabel = new GameObject("Item Label");
        itemLabel.transform.SetParent(itemObj.transform, false);
        RectTransform itemLabelRT = itemLabel.AddComponent<RectTransform>();
        itemLabelRT.anchorMin = new Vector2(0.05f, 0f);
        itemLabelRT.anchorMax = new Vector2(0.95f, 1f);
        itemLabelRT.sizeDelta = Vector2.zero;
        TextMeshProUGUI itemTMP = itemLabel.AddComponent<TextMeshProUGUI>();
        itemTMP.fontSize = 18;
        itemTMP.alignment = TextAlignmentOptions.MidlineLeft;
        itemTMP.color = Color.white;
        if (font != null) itemTMP.font = font;

        TMP_Dropdown dropdown = ddObj.AddComponent<TMP_Dropdown>();
        dropdown.targetGraphic = ddBg;
        dropdown.template = tempRT;
        dropdown.captionText = capTMP;
        dropdown.itemText = itemTMP;
        outDropdown = dropdown;
    }

    private static void CreateHintCard(GameObject parent, string text, float yPos, TMP_FontAsset font)
    {
        GameObject card = new GameObject("HintCard");
        card.transform.SetParent(parent.transform, false);
        RectTransform rt = card.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, yPos);
        rt.sizeDelta = new Vector2(980f, 80f);

        Image img = card.AddComponent<Image>();
        img.color = new Color(0.16f, 0.16f, 0.2f, 0.6f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(card.transform, false);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(25f, 10f);
        textRT.offsetMax = new Vector2(-25f, -10f);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color = new Color(0.75f, 0.75f, 0.75f, 1f);
        if (font != null) tmp.font = font;
    }

    private static void CreateKeybindsCard(GameObject parent, float yPos, TMP_FontAsset font)
    {
        GameObject card = new GameObject("KeybindsCard");
        card.transform.SetParent(parent.transform, false);
        RectTransform rt = card.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, yPos);
        rt.sizeDelta = new Vector2(980f, 320f);

        Image img = card.AddComponent<Image>();
        img.color = new Color(0.14f, 0.14f, 0.18f, 0.75f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(card.transform, false);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(30f, 20f);
        textRT.offsetMax = new Vector2(-30f, -20f);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "<b>[KEYBOARD & MOUSE] คู่มือการควบคุมในเกม:</b>\n\n" +
                   "  •  <b>W, A, S, D</b>  :  เคลื่อนที่ตัวละคร (Movement)\n" +
                   "  •  <b>Left Shift</b>  :  วิ่งเร็ว (Sprint)\n" +
                   "  •  <b>E</b>  :  ปฏิสัมพันธ์ / พูดคุยกับ NPC / เก็บไอเทม (Interact / Talk / Pick up)\n" +
                   "  •  <b>Tab</b>  :  เปิดและปิดกระเป๋าเป้ (Inventory)\n" +
                   "  •  <b>ESC / P</b>  :  เปิดเมนูหยุดเกม (Pause Menu)\n" +
                   "  •  <b>~ (Tilde) / F12</b>  :  เปิดคอนโซลคำสั่งผู้พัฒนา (Developer Console)";
        tmp.fontSize = 20;
        tmp.lineSpacing = 15;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        if (font != null) tmp.font = font;
    }
}
#endif
