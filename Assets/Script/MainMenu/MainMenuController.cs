using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("ลาก GameObject ของหน้าเมนูแรกสุดมาใส่ตรงนี้ (ที่มีปุ่ม START, OPTIONS, EXIT)")]
    public GameObject mainPanel;

    [Tooltip("ลาก GameObject ของหน้าสองมาใส่ตรงนี้ (ที่มีปุ่ม NEW GAME, LOAD GAME, BACK)")]
    public GameObject playOptionsPanel;

    [Tooltip("ลาก GameObject ของหน้าโหลดเกมมาใส่ตรงนี้ (ที่มีสล็อต 4 ช่อง และปุ่ม BACK)")]
    public GameObject slotSelectPanel;

    [Tooltip("ลาก GameObject ของหน้าตั้งค่าออปชั่นมาใส่ตรงนี้ (ที่มีแท็บ GRAPHIC, AUDIO, etc.)")]
    public GameObject settingsPanel;

    [Header("Save Slot Buttons (Load Game)")]
    [Tooltip("ลากปุ่ม Slot 1 ถึง Slot 4 มาใส่เรียงตามลำดับ")]
    public Button[] slotButtons = new Button[4];

    [Tooltip("ลาก TMP Text ของสล็อต 1 ถึง 4 ที่ใช้แสดงรายละเอียดมาใส่เรียงตามลำดับ")]
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[4];

    [Header("Menu Audio Settings (ระบบเสียงเมนูหลัก) [เพิ่มใหม่]")]
    [Tooltip("ลากไฟล์เสียงตอนกดคลิกปุ่มมาใส่ (เช่น select_1 หรือ click_double_on)")]
    public AudioClip buttonClickSound;
    [Tooltip("ลากไฟล์เสียงตอนเลื่อนเมาส์ไปชี้ปุ่ม (Hover) มาใส่ (เช่น select_2 หรือ pop_1)")]
    public AudioClip buttonHoverSound;
    [Tooltip("ลากไฟล์เสียงตอนกดย้อนกลับ (Back / Cancel) มาใส่ (เช่น cancel หรือ select_3)")]
    public AudioClip backClickSound;

    private SaveSystem saveSystem;
    private AudioSource audioSource;
    private GameObject newGameDialog;
    private GameObject previousMenuSelection;
    private bool isStartingNewGame;

    void Awake()
    {
        // ป้องกันบั๊ก MissingReference เมื่อสลับฉากกลับมาเมนูหลัก
        // เช็คก่อนว่ามี SaveSystem อยู่แล้วหรือไม่ ถ้ามีให้ใช้ตัวเดิม ห้ามสร้างซ้ำ
        saveSystem = SaveSystem.Instance;
        if (saveSystem == null)
        {
            saveSystem = Object.FindAnyObjectByType<SaveSystem>();
        }

        if (saveSystem == null)
        {
            GameObject saveGo = new GameObject("SaveSystem");
            saveSystem = saveGo.AddComponent<SaveSystem>();
        }

        // ตั้งค่า AudioSource ประจำหน้าเมนู
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // 2D UI sound
        audioSource.playOnAwake = false;
        SettingsController.RouteToSFX(audioSource);
    }

    void Start()
    {
        // ซ่อนหน้าต่างเควสและกระเป๋าทันทีเมื่ออยู่ใน MainMenu
        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.SetVisible(false);
        }
        if (InventoryManager.Instance != null && InventoryManager.Instance.inventoryPanel != null)
        {
            InventoryManager.Instance.inventoryPanel.SetActive(false);
        }

        MainMenuPresentation.Apply(mainPanel, playOptionsPanel, slotSelectPanel, slotButtons, slotTexts);

        // เริ่มต้นแสดงผลหน้าแรกสุด และปิดหน้าอื่นๆ ทั้งหมด
        ShowMainMenu();

        // ผูกเสียงคลิกและเสียง Hover เมาส์ให้กับปุ่มทั้งหมดในหน้าเมนูอัตโนมัติ
        HookAllButtonSounds();
    }

    // ฟังก์ชันช่วยผูกเสียงให้กับปุ่ม UI ทั้งหมดอัตโนมัติ
    private void HookAllButtonSounds()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (var btn in allButtons)
        {
            if (btn == null) continue;

            // ผูกเสียงคลิก
            btn.onClick.AddListener(() => PlayButtonClick());

            // ผูกเสียง Hover ชี้เมาส์
            UnityEngine.EventSystems.EventTrigger trigger = btn.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { PlayButtonHover(); });
            trigger.triggers.Add(entry);
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayButtonHover()
    {
        if (buttonHoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }

    public void PlayBackClick()
    {
        if (backClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(backClickSound);
        }
        else
        {
            PlayButtonClick();
        }
    }

    // --- ฟังก์ชันควบคุมการแสดงผลของหน้าจอ (Panel Triggers) ---

    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowPlayOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(true);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSlotSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // อัปเดตรายละเอียดและปุ่มของแต่ละสล็อต
        UpdateSlotDetails();
    }

    public void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // --- ฟังก์ชันหลักเกี่ยวกับระบบเซฟและการเลือกสล็อต ---

    public void StartNewGame()
    {
        if (newGameDialog != null || isStartingNewGame) return;

        int oldestSlot = 1;
        DateTime oldestWrite = DateTime.MaxValue;
        try
        {
            for (int slot = 1; slot <= 4; slot++)
            {
                DateTime? lastWrite = GetSlotWriteTime(saveSystem.GetSaveFilePath(slot));
                if (!lastWrite.HasValue)
                {
                    StartGameAtSlot(slot, null);
                    return;
                }

                if (lastWrite.Value < oldestWrite)
                {
                    oldestWrite = lastWrite.Value;
                    oldestSlot = slot;
                }
            }
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            ShowNewGameDialog("Save slots could not be checked. Check storage access and try again.");
            return;
        }

        ShowNewGameDialog(
            $"All 4 save slots are occupied.\nStarting a new game will overwrite Slot {oldestSlot}, the oldest save.",
            () => StartGameAtSlot(oldestSlot, oldestWrite));
    }

    private static DateTime? GetSlotWriteTime(string path)
    {
        try
        {
            // ไฟล์ที่อ่านเนื้อหาไม่ได้ยังถือว่าเป็นสล็อตที่มีข้อมูล
            File.GetAttributes(path);
            return File.GetLastWriteTimeUtc(path);
        }
        catch (FileNotFoundException) { return null; }
        catch (DirectoryNotFoundException) { return null; }
    }

    private void StartGameAtSlot(int targetSlot, DateTime? expectedWrite)
    {
        if (isStartingNewGame) return;

        const string bedroomScene = "Bedroom_3D";
        SaveData initialData = new SaveData
        {
            currentDay = 1,
            activeSaveSlot = targetSlot,
            playerX = -0.808f,
            playerY = 1f,
            playerZ = -6.931f,
            currentSceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/Bedroom_3D.unity")
        };
        if (initialData.currentSceneIndex < 0)
        {
            ShowNewGameDialog("The starting scene is unavailable.");
            return;
        }

        try
        {
            if (GetSlotWriteTime(saveSystem.GetSaveFilePath(targetSlot)) != expectedWrite)
            {
                ShowNewGameDialog("This save slot changed. Choose New Game again to review it.");
                return;
            }

            saveSystem.SaveGame(targetSlot, initialData);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            ShowNewGameDialog("The new game could not be saved. Check storage access and try again.");
            return;
        }

        isStartingNewGame = true;
        SaveSystem.pendingLoadData = initialData;
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.ResetGameSession();
            GameManagerSetup.Instance.activeSaveSlot = targetSlot;
        }

        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentDay = 1;
        }

        // กำหนดจุดเกิดเริ่มต้นที่ประตูห้องนอน
        SceneTransitionManager.targetSpawnPointName = "Player_Spawn_Point";

        // รีเซ็ตระบบสอนเล่นเมื่อเริ่มเกมใหม่
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResetTutorial();
        }

        // เคลียร์ไอเทมตกค้างในกระเป๋าเป้เมื่อเริ่มเกมใหม่
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadFromGlobal();
        }

        SceneManager.LoadScene(bedroomScene);
    }

    private void Update()
    {
        if (newGameDialog != null &&
            UnityEngine.InputSystem.Keyboard.current?.escapeKey.wasPressedThisFrame == true)
        {
            CloseNewGameDialog();
        }
    }

    private void ShowNewGameDialog(string message, Action confirm = null)
    {
        Canvas canvas = playOptionsPanel != null ? playOptionsPanel.GetComponentInParent<Canvas>() : null;
        if (canvas == null)
        {
            Debug.LogError($"[New Game] {message}");
            return;
        }

        previousMenuSelection = EventSystem.current?.currentSelectedGameObject;
        RectTransform overlay = CreateDialogRect(canvas.rootCanvas.transform, "NewGameConfirmation", Vector2.zero, Vector2.one);
        newGameDialog = overlay.gameObject;
        overlay.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
        RectTransform box = CreateDialogRect(overlay, "DialogBox", new Vector2(0.22f, 0.30f), new Vector2(0.78f, 0.70f));
        box.gameObject.AddComponent<Image>().color = new Color(0.13f, 0.15f, 0.20f);
        TMP_FontAsset font = playOptionsPanel.GetComponentInChildren<TextMeshProUGUI>(true)?.font;
        CreateDialogLabel(box, "Message", message, font, new Vector2(0.07f, 0.36f), new Vector2(0.93f, 0.92f), 36f);

        Button cancel = CreateDialogButton(box, confirm == null ? "OK" : "Cancel", font,
            confirm == null ? 0.32f : 0.07f, confirm == null ? 0.68f : 0.47f, new Color(0.28f, 0.33f, 0.42f));
        cancel.onClick.AddListener(CloseNewGameDialog);
        if (confirm != null)
        {
            Button overwrite = CreateDialogButton(box, "Overwrite Save", font, 0.53f, 0.93f, new Color(0.62f, 0.22f, 0.26f));
            overwrite.onClick.AddListener(() =>
            {
                if (newGameDialog == null) return;
                CloseNewGameDialog();
                confirm();
            });
            cancel.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = overwrite };
            overwrite.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = cancel };
        }
        else
        {
            cancel.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        EventSystem.current?.SetSelectedGameObject(cancel.gameObject);
    }

    private void CloseNewGameDialog()
    {
        if (newGameDialog == null) return;
        newGameDialog.SetActive(false);
        Destroy(newGameDialog);
        newGameDialog = null;
        EventSystem.current?.SetSelectedGameObject(previousMenuSelection);
        previousMenuSelection = null;
    }

    private static RectTransform CreateDialogRect(Transform parent, string name, Vector2 min, Vector2 max)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static void CreateDialogLabel(Transform parent, string name, string value, TMP_FontAsset font,
        Vector2 min, Vector2 max, float maxFontSize)
    {
        var label = CreateDialogRect(parent, name, min, max).gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) label.font = font;
        label.text = value;
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = maxFontSize;
        label.color = Color.white;
        label.raycastTarget = false;
    }

    private static Button CreateDialogButton(Transform parent, string label, TMP_FontAsset font, float left, float right, Color color)
    {
        var rect = CreateDialogRect(parent, label, new Vector2(left, 0.09f), new Vector2(right, 0.30f));
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        CreateDialogLabel(rect, "Label", label, font, Vector2.zero, Vector2.one, 30f);
        return button;
    }

    // ฟังก์ชันสำหรับอัปเดตตัวหนังสือแสดงข้อมูลเซฟในแต่ละสล็อต 1 - 4
    private void UpdateSlotDetails()
    {
        for (int i = 0; i < 4; i++)
        {
            if (slotButtons[i] == null) continue;

            SaveData data = saveSystem.ReadSlotPreview(i + 1);
            if (slotTexts[i] != null)
            {
                slotTexts[i].text = data != null ? SaveSystem.FormatSlotSummary(data).Replace("\t\t\t\t", "  /  ") : "Empty slot";
            }
            slotButtons[i].interactable = data != null;
        }
    }

    // ฟังก์ชันสำหรับกดโหลดไฟล์ตามสล็อต (เรียกผ่าน Unity Event หรือปุ่มสล็อต)
    public void LoadSlot(int slotIndex)
    {
        SaveData data = saveSystem.LoadGameToGlobal(slotIndex);
        if (data == null)
        {
            UpdateSlotDetails();
            return;
        }

        // เคลียร์ชื่อจุดเกิดของประตู เพื่อให้ตัวเกมใช้พิกัดจากไฟล์เซฟ
        SceneTransitionManager.targetSpawnPointName = "";

        int loadedDay = data.currentDay;
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.isTutorialActive = (loadedDay <= 1);
        }

        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentDay = loadedDay;
        }

        if (data.currentSceneIndex > 0)
        {
            Debug.Log($"[Load Game] โหลดสล็อตที่: {slotIndex} สำเร็จ! วันที่: {loadedDay} กำลังเข้าซีนรหัส {data.currentSceneIndex}");
            SceneManager.LoadScene(data.currentSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("Bedroom_3D");
        }
    }

    public void ExitGame()
    {
        Debug.Log("ออกจากเกม!");
        Application.Quit();
    }
}
