#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SleepSaveUIBuilder : EditorWindow
{
    [MenuItem("Tools/🛌 ติดตั้งหน้าต่างเซฟเกมแท้จาก Main Menu (Use Real Main Menu Save Panel)")]
    public static void BuildWithMainMenuPanel()
    {
        BuildForActiveScene(false);
    }

    [MenuItem("Tools/🛌 ติดตั้งเซฟเกมลงทุกฉากห้องนอน (Setup Sleep Save In All Scenes)")]
    public static void SetupAllBedroomScenes()
    {
        string[] scenes = new string[] { "Assets/Scenes/Bedroom_3D.unity", "Assets/Scenes/Home.unity" };
        foreach (string scenePath in scenes)
        {
            if (System.IO.File.Exists(scenePath))
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                BuildForScene(scene, true);
            }
        }
        EditorUtility.DisplayDialog("เสร็จสิ้น", "ติดตั้งระบบเซฟเกมครบทุกฉากห้องนอนเรียบร้อยแล้ว!", "ตกลง");
    }

    public static GameObject EnsureSlotSelectPrefab()
    {
        string prefabPath = "Assets/Prefabs/UI/SlotSelectPanel.prefab";
        GameObject panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (panelPrefab != null) return panelPrefab;

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }

        string mainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        var mainMenuScene = EditorSceneManager.OpenScene(mainMenuScenePath, OpenSceneMode.Additive);
        GameObject originalPanel = null;

        foreach (GameObject root in mainMenuScene.GetRootGameObjects())
        {
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "SlotSelectPanel")
                {
                    originalPanel = t.gameObject;
                    break;
                }
            }
            if (originalPanel != null) break;
        }

        if (originalPanel != null)
        {
            panelPrefab = PrefabUtility.SaveAsPrefabAsset(originalPanel, prefabPath);
            Debug.Log($"[SleepSaveUIBuilder] สกัด SlotSelectPanel จาก MainMenu มาบันทึกเป็น Prefab สำเร็จ: {prefabPath}");
        }
        else
        {
            Debug.LogError("[SleepSaveUIBuilder] ไม่พบ SlotSelectPanel ใน MainMenu.unity!");
        }

        EditorSceneManager.CloseScene(mainMenuScene, true);
        return panelPrefab;
    }

    public static void BuildForActiveScene(bool silent)
    {
        BuildForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), silent);
    }

    public static void BuildForScene(UnityEngine.SceneManagement.Scene currentScene, bool silent)
    {
        GameObject panelPrefab = EnsureSlotSelectPrefab();
        if (panelPrefab == null)
        {
            Debug.LogError("[SleepSaveUIBuilder] ไม่สามารถเตรียม SlotSelectPanel.prefab ได้!");
            return;
        }
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Sleep Save UI with Real MainMenu Panel");

        // ลบ SleepSaveCanvas เก่าออก
        GameObject oldCanvas = GameObject.Find("SleepSaveCanvas");
        if (oldCanvas != null)
        {
            Undo.DestroyObjectImmediate(oldCanvas);
        }

        // 3. สร้าง Canvas หลัก
        GameObject canvasGo = new GameObject("SleepSaveCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGo, "Create SleepSaveCanvas");

        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200; // ลำดับบนสุด

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        SleepSaveMenuController controller = canvasGo.AddComponent<SleepSaveMenuController>();

        // -------------------------------------------------------------
        // 4. สร้าง FadeOverlay สำหรับฉากจอดำข้ามวัน
        // -------------------------------------------------------------
        GameObject fadeOverlayGo = new GameObject("FadeOverlay");
        fadeOverlayGo.transform.SetParent(canvasGo.transform, false);

        RectTransform fadeRect = fadeOverlayGo.AddComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.sizeDelta = Vector2.zero;

        Image fadeImg = fadeOverlayGo.AddComponent<Image>();
        fadeImg.color = Color.black;

        CanvasGroup fadeGroup = fadeOverlayGo.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        controller.fadeOverlayGroup = fadeGroup;

        // ตัวหนังสือแสดงวันที่กึ่งกลางจอ
        GameObject dayNoticeGo = new GameObject("DayNoticeContainer");
        dayNoticeGo.transform.SetParent(fadeOverlayGo.transform, false);

        RectTransform dayRect = dayNoticeGo.AddComponent<RectTransform>();
        dayRect.anchorMin = new Vector2(0.5f, 0.5f);
        dayRect.anchorMax = new Vector2(0.5f, 0.5f);
        dayRect.sizeDelta = new Vector2(800, 200);

        CanvasGroup dayNoticeGroup = dayNoticeGo.AddComponent<CanvasGroup>();
        dayNoticeGroup.alpha = 0f;
        controller.dayNoticeGroup = dayNoticeGroup;

        TextMeshProUGUI dayText = dayNoticeGo.AddComponent<TextMeshProUGUI>();
        dayText.text = "DAY 2";
        TMP_FontAsset kanitFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/Kanit-Black SDF.asset");
        if (kanitFont != null) dayText.font = kanitFont;
        dayText.fontSize = 76;
        dayText.alignment = TextAlignmentOptions.Center;
        dayText.color = new Color(1f, 0.95f, 0.82f, 1f); // Warm Gold
        controller.dayNoticeText = dayText;

        // -------------------------------------------------------------
        // 5. วาง SlotSelectPanel ของแท้จาก Main Menu
        // -------------------------------------------------------------
        if (panelPrefab != null)
        {
            GameObject realPanel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, canvasGo.transform);
            realPanel.name = "SlotSelectPanel";
            controller.saveMenuPanel = realPanel;

            RectTransform panelRect = realPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.anchoredPosition = Vector2.zero;
                panelRect.sizeDelta = Vector2.zero;
                panelRect.localScale = Vector3.one;
                panelRect.localPosition = Vector3.zero;
            }

            // ค้นหาและเปลี่ยนชื่อ Title เป็น Save Game
            Transform titleTransform = realPanel.transform.Find("Load Game");
            if (titleTransform != null)
            {
                titleTransform.name = "Save Game";
                TextMeshProUGUI titleTMP = titleTransform.GetComponent<TextMeshProUGUI>();
                if (titleTMP != null) titleTMP.text = "Save Game";
            }

            // ผูกปุ่ม Back
            Button backBtn = null;
            Transform backTransform = realPanel.transform.Find("Back");
            if (backTransform == null) backTransform = realPanel.transform.Find("Btn_Back");
            if (backTransform != null) backBtn = backTransform.GetComponent<Button>();
            controller.cancelButton = backBtn;

            // ลบ Event ตกค้างเดิมจาก MainMenuController
            if (backBtn != null)
            {
                while (backBtn.onClick.GetPersistentEventCount() > 0)
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(backBtn.onClick, 0);
                }
            }

            // ผูกปุ่มสล็อต 1 ถึง 4 และ Text
            controller.slotButtons = new Button[4];
            controller.slotTexts = new TextMeshProUGUI[4];

            for (int i = 0; i < 4; i++)
            {
                int slotIdx = i + 1;
                Transform slotT = realPanel.transform.Find($"Slot_{slotIdx}");
                if (slotT != null)
                {
                    Button btn = slotT.GetComponent<Button>();
                    controller.slotButtons[i] = btn;
                    controller.slotTexts[i] = slotT.GetComponentInChildren<TextMeshProUGUI>();

                    if (btn != null)
                    {
                        while (btn.onClick.GetPersistentEventCount() > 0)
                        {
                            UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
                        }
                    }
                }
            }

            // ซ่อนหน้าต่างเริ่มต้น
            realPanel.SetActive(false);
        }

        // ทำให้ FadeOverlay แสดงผลทับหน้าต่างเซฟเสมอเมื่อเริ่มตัดฉากจอดำ
        fadeOverlayGo.transform.SetAsLastSibling();

        // ผูกเสียงตอนเซฟและกดปุ่ม
        AudioClip selectSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/select_1.wav");
        if (selectSound == null) selectSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/DialogueAudio/Complease.wav");
        controller.saveConfirmSound = selectSound;
        controller.buttonClickSound = selectSound;

        // บันทึกสร้างเป็น Prefab เพื่อให้โหลดไปใช้งานได้ทุกที่
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        PrefabUtility.SaveAsPrefabAssetAndConnect(canvasGo, "Assets/Prefabs/UI/SleepSaveCanvas.prefab", InteractionMode.AutomatedAction);
        PrefabUtility.SaveAsPrefabAsset(canvasGo, "Assets/Resources/SleepSaveCanvas.prefab");

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);

        Selection.activeGameObject = canvasGo;

        Debug.Log($"[SleepSaveUIBuilder] ติดตั้ง SleepSaveCanvas ลงในฉาก {currentScene.name} เรียบร้อย!");

        if (!silent)
        {
            EditorUtility.DisplayDialog(
                "ติดตั้ง UI ของแท้สำเร็จ ✨",
                "นำ 'SlotSelectPanel' ของแท้จากหน้า Main Menu มาใช้กับระบบนอนเรียบร้อยแล้วครับ!\n\n" +
                "• 🎨 ดีไซน์สไตล์มังงะ/ลายน้ำขาวดำตรงตามต้นฉบับเป๊ะ 100%\n" +
                "• 🗂️ ช่องสล็อต 1 - 4 แสดงข้อมูล Nia, วันที่, และเวลาเล่น\n" +
                "• 🔙 ปุ่ม Back สำหรับย้อนกลับไปเดินเล่นต่อ\n" +
                "• 🎬 ระบบเฟดจอดำบอก 'DAY X' พร้อมเข้าวันใหม่อย่างสมบูรณ์!",
                "ตกลง"
            );
        }
    }

    public static void AutoSetupIfNeeded()
    {
        string canvasPrefabPath = "Assets/Prefabs/UI/SleepSaveCanvas.prefab";
        GameObject canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(canvasPrefabPath);
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        if (canvasPrefab == null)
        {
            BuildForActiveScene(true);
            return;
        }

        if (activeScene.name == "Bedroom_3D" || activeScene.name == "Home")
        {
            GameObject existing = GameObject.Find("SleepSaveCanvas");
            if (existing == null || existing.GetComponent<SleepSaveMenuController>()?.saveMenuPanel == null)
            {
                BuildForActiveScene(true);
            }
        }
    }
}

[InitializeOnLoad]
public static class SleepSaveAutoRunner
{
    static SleepSaveAutoRunner()
    {
        EditorApplication.delayCall += AutoRun;
    }

    private static void AutoRun()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        SleepSaveUIBuilder.AutoSetupIfNeeded();
    }
}
#endif
