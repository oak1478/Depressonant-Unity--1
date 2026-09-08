using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance { get; private set; }

    [Header("Console State")]
    [SerializeField] private bool isConsoleOpen = false;
    public bool IsOpen => isConsoleOpen;

    [Header("UI References (Optional - Will Auto-Generate if Null)")]
    public GameObject consoleCanvas;
    public GameObject consolePanel;
    public TextMeshProUGUI outputLogText;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;

    private List<string> logEntries = new List<string>();
    private const int MaxLogLines = 60;

    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("DevConsole");
            go.AddComponent<DevConsole>();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            EnsureConsoleUI();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (consolePanel != null)
        {
            consolePanel.SetActive(false);
        }
        isConsoleOpen = false;
    }

    private bool clearOnNextFrame = false;

    private void Update()
    {
        if (Keyboard.current != null)
        {
            // กดปุ่ม ~ (Tilde/Backquote) หรือ F12 เพื่อสลับเปิด/ปิดหน้าต่าง Console
            if (Keyboard.current.backquoteKey.wasPressedThisFrame || Keyboard.current.f12Key.wasPressedThisFrame)
            {
                ToggleConsole();
                return;
            }

            // เมื่อเปิดหน้าต่าง Console อยู่
            if (isConsoleOpen)
            {
                // กด ESC เพื่อปิดหน้าต่าง
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    CloseConsole();
                    return;
                }

                // นำทางประวัติคำสั่งด้วยปุ่มลูกศรขึ้น / ลง (Command History)
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                {
                    NavigateHistory(-1);
                }
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                {
                    NavigateHistory(1);
                }
                else if (inputField != null && !inputField.isFocused && Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    inputField.ActivateInputField();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (clearOnNextFrame)
        {
            clearOnNextFrame = false;
            if (inputField != null)
            {
                inputField.text = string.Empty;
                inputField.ActivateInputField();
            }
        }
    }

    public void ToggleConsole()
    {
        if (isConsoleOpen)
        {
            CloseConsole();
        }
        else
        {
            OpenConsole();
        }
    }

    public void OpenConsole()
    {
        EnsureConsoleUI();

        isConsoleOpen = true;
        clearOnNextFrame = true;
        if (consolePanel != null)
        {
            consolePanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (inputField != null)
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    public void CloseConsole()
    {
        isConsoleOpen = false;
        if (consolePanel != null)
        {
            consolePanel.SetActive(false);
        }

        // ปรับการล็อกเมาส์กลับตามประเภทของฉากปัจจุบันและสถานะบทสนทนา
        bool is3DScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Bedroom_3D";
        
        bool isDialogueActive = false;
        DialogueManager dm = UnityEngine.Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.IsDialogueActive())
        {
            isDialogueActive = true;
        }

        if (isDialogueActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !is3DScene;
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0 || inputField == null) return;

        historyIndex += direction;
        if (historyIndex < 0) historyIndex = 0;
        if (historyIndex >= commandHistory.Count)
        {
            historyIndex = commandHistory.Count;
            inputField.text = string.Empty;
            return;
        }

        inputField.text = commandHistory[historyIndex];
        inputField.caretPosition = inputField.text.Length;
    }

    public void OnSubmitCommand(string rawInput)
    {
        // หากเป็นการปิดหน้าต่างด้วยปุ่ม ~ จะไม่ประมวลผลเครื่องหมาย `
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            if (inputField != null)
            {
                inputField.text = string.Empty;
                inputField.ActivateInputField();
            }
            return;
        }

        string trimmed = rawInput.Trim().Trim('`');
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            if (inputField != null)
            {
                inputField.text = string.Empty;
                inputField.ActivateInputField();
            }
            return;
        }

        // บันทึกลงประวัติคำสั่ง
        if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != trimmed)
        {
            commandHistory.Add(trimmed);
        }
        historyIndex = commandHistory.Count;

        Log($"<color=#61AFEF>> {trimmed}</color>");

        ExecuteCommand(trimmed);

        if (inputField != null)
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }
    }

    private void ExecuteCommand(string input)
    {
        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string cmd = parts[0].ToLower();

        switch (cmd)
        {
            case "help":
                PrintHelp();
                break;

            case "give":
            case "item":
                HandleGiveCommand(parts);
                break;

            case "clear":
            case "cls":
                if (parts.Length > 1 && (parts[1].ToLower() == "inv" || parts[1].ToLower() == "items" || parts[1].ToLower() == "inventory"))
                {
                    HandleClearInventory();
                }
                else
                {
                    ClearLog();
                }
                break;

            case "clearinv":
            case "clearitems":
                HandleClearInventory();
                break;

            case "day":
            case "setday":
                HandleDayCommand(parts);
                break;

            case "stress":
            case "setstress":
                HandleStressCommand(parts);
                break;

            case "addstress":
                HandleAddStressCommand(parts);
                break;

            case "heal":
                HandleHealCommand();
                break;

            case "save":
                HandleSaveCommand(parts);
                break;

            case "load":
                HandleLoadCommand(parts);
                break;

            case "talked":
                HandleTalkedCommand();
                break;

            case "resettalk":
                HandleResetTalkCommand();
                break;

            case "speed":
                HandleSpeedCommand(parts);
                break;

            case "warp":
            case "tp":
                HandleWarpCommand(parts);
                break;

            case "skiptutorial":
            case "skiptut":
            case "skip":
                HandleSkipTutorialCommand();
                break;

            case "close":
            case "exit":
                CloseConsole();
                break;

            default:
                Log($"<color=#E06C75>[Error] ไม่พบคำสั่ง '{cmd}' (พิมพ์ 'help' เพื่อดูรายการคำสั่งทั้งหมด)</color>");
                break;
        }
    }

    private void PrintHelp()
    {
        Log("<color=#98C379>=== รายการคำสั่งของ Command Prompt (Developer Console) ===</color>");
        Log(" <color=#E5C07B>give <item> [จำนวน]</color> : เสกไอเทมเข้ากระเป๋า (candy, milk, diary, headphone, armband, drawing, all)");
        Log(" <color=#E5C07B>clear inv</color> : ล้างไอเทมทั้งหมดในกระเป๋าเป้");
        Log(" <color=#E5C07B>day <1-30></color> : เปลี่ยนวันในเกม (เช่น day 8, day 2)");
        Log(" <color=#E5C07B>stress <0-100></color> : ตั้งค่าระดับความเครียด (เช่น stress 50, stress 0)");
        Log(" <color=#E5C07B>addstress <+/-ค่า></color> : เพิ่มหรือลดความเครียด (เช่น addstress 25, addstress -25)");
        Log(" <color=#E5C07B>heal</color> : รีเซ็ตความเครียดเป็น 0% ทันที");
        Log(" <color=#E5C07B>save <1-4></color> : บันทึกเกมลงช่องเซฟที่ระบุ");
        Log(" <color=#E5C07B>load <1-4></color> : โหลดเกมจากช่องเซฟที่ระบุ");
        Log(" <color=#E5C07B>cls / clear</color> : ล้างข้อความบนหน้าจอ Console");
        Log(" <color=#E5C07B>talked</color> : ดูรายชื่อ NPC ที่คุยจบแล้วในวันนี้");
        Log(" <color=#E5C07B>resettalk</color> : รีเซ็ตสถานะการคุย NPC วันนี้ทั้งหมดให้กลับมาคุยใหม่ได้");
        Log(" <color=#E5C07B>warp <scene|x y z></color> : วาร์ปข้ามฉาก (home, school, outside, bedroom) หรือวาร์ปพิกัด X Y Z");
        Log(" <color=#E5C07B>skiptutorial / skip</color> : ข้ามขั้นตอนสอนเล่น (Tutorial) ทันที");
        Log(" <color=#E5C07B>close / exit</color> : ปิดหน้าต่าง Command Prompt (หรือกด ~ / ESC)");
    }

    private void HandleGiveCommand(string[] parts)
    {
        if (parts.Length < 2)
        {
            Log("<color=#E06C75>[Usage] give <item_name> [amount]</color>");
            Log(" รายชื่อไอเทม: candy, milk, diary, headphone, armband, drawing, all");
            return;
        }

        string itemArg = parts[1].ToLower();
        int amount = 1;
        if (parts.Length >= 3 && !int.TryParse(parts[2], out amount))
        {
            amount = 1;
        }
        if (amount < 1) amount = 1;

        if (InventoryManager.Instance == null)
        {
            Log("<color=#E06C75>[Error] ไม่พบ InventoryManager ในฉากปัจจุบัน!</color>");
            return;
        }

        if (itemArg == "all")
        {
            InventoryManager.Instance.GiveItem(ItemType.Candy, 1);
            InventoryManager.Instance.GiveItem(ItemType.StrawberryMilk, 1);
            InventoryManager.Instance.GiveItem(ItemType.Diary, 1);
            InventoryManager.Instance.GiveItem(ItemType.Headphone, 1);
            InventoryManager.Instance.GiveItem(ItemType.Armband, 1);
            InventoryManager.Instance.GiveItem(ItemType.RainDrawing, 1);
            Log("<color=#98C379>[Success] เสกไอเทมครบทั้ง 6 ชนิดเข้ากระเป๋าเป้เรียบร้อย!</color>");
            return;
        }

        ItemType targetType = ItemType.None;
        switch (itemArg)
        {
            case "candy":
                targetType = ItemType.Candy;
                break;
            case "milk":
            case "strawberry":
            case "strawberrymilk":
                targetType = ItemType.StrawberryMilk;
                break;
            case "diary":
            case "book":
                targetType = ItemType.Diary;
                break;
            case "headphone":
            case "headphones":
                targetType = ItemType.Headphone;
                break;
            case "armband":
                targetType = ItemType.Armband;
                break;
            case "drawing":
            case "rain":
            case "raindrawing":
                targetType = ItemType.RainDrawing;
                break;
            default:
                Log($"<color=#E06C75>[Error] ไม่รู้จักไอเทม '{itemArg}'</color>");
                Log(" รายชื่อที่รองรับ: candy, milk, diary, headphone, armband, drawing, all");
                return;
        }

        bool success = InventoryManager.Instance.GiveItem(targetType, amount);
        if (success)
        {
            Log($"<color=#98C379>[Success] เสกไอเทม '{targetType}' x{amount} เข้ากระเป๋าเรียบร้อย!</color>");
        }
        else
        {
            Log("<color=#E06C75>[Warn] ช่องกระเป๋าเป้เต็ม ไม่สามารถเพิ่มไอเทมได้ทั้งหมด</color>");
        }
    }

    private void HandleClearInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Log("<color=#E06C75>[Error] ไม่พบ InventoryManager ในฉากปัจจุบัน!</color>");
            return;
        }

        InventoryManager.Instance.ClearAllItems();
        Log("<color=#98C379>[Success] ล้างไอเทมในกระเป๋าเป้ทั้งหมดเรียบร้อย!</color>");
    }

    private void HandleDayCommand(string[] parts)
    {
        if (parts.Length < 2 || !int.TryParse(parts[1], out int targetDay))
        {
            Log("<color=#E06C75>[Usage] day <number> (เช่น day 8 หรือ day 2)</color>");
            return;
        }

        if (targetDay < 1) targetDay = 1;

        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentDay = targetDay;
        }
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentDay = targetDay;
            GameManagerSetup.Instance.ResetDailyNPCTalk();
        }

        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.ForceRefresh();
        }

        // อัปเดตการแสดงผลของไอเทมและ NPC ในฉากปัจจุบันตามวันที่เปลี่ยนทันที
        ItemAppearanceController[] allItems = UnityEngine.Object.FindObjectsByType<ItemAppearanceController>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        foreach (var itm in allItems)
        {
            if (itm != null) itm.UpdateItemAppearance();
        }

        NPCAppearanceController[] allNPCs = UnityEngine.Object.FindObjectsByType<NPCAppearanceController>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        foreach (var npc in allNPCs)
        {
            if (npc != null) npc.UpdateAppearance();
        }

        if (DailySecretItemManager.Instance != null)
        {
            DailySecretItemManager.Instance.RefreshSecretItems();
        }

        Log($"<color=#98C379>[Success] ปรับเปลี่ยนวันในเกมเป็น Day {targetDay} เรียบร้อย!</color>");
    }

    private void HandleTalkedCommand()
    {
        if (GameManagerSetup.Instance == null)
        {
            Log("<color=#E06C75>[Error] ไม่พบ GameManagerSetup ในระบบ!</color>");
            return;
        }

        var list = GameManagerSetup.Instance.talkedNPCsToday;
        if (list == null || list.Count == 0)
        {
            Log("<color=#E5C07B>[Info] วันนี้ยังไม่ได้พูดคุยกับ NPC คนใดเลย</color>");
        }
        else
        {
            Log($"<color=#98C379>[Info] รายชื่อ NPC ที่พูดคุยแล้วในวันนี้ ({list.Count} คน):</color>");
            foreach (var name in list)
            {
                Log($"  - <color=#61AFEF>{name}</color>");
            }
        }
    }

    private void HandleResetTalkCommand()
    {
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.ResetDailyNPCTalk();
        }

        // รีเซ็ต lastTalkedDay ของ NPC ในฉากปัจจุบัน
        NPCInteraction[] currentNPCs = UnityEngine.Object.FindObjectsByType<NPCInteraction>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        foreach (var npc in currentNPCs)
        {
            if (npc != null) npc.lastTalkedDay = 0;
        }

        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.ForceRefresh();
        }

        Log("<color=#98C379>[Success] รีเซ็ตสถานะการพูดคุยของ NPC ประจำวันเรียบร้อยแล้ว!</color>");
    }

    private void HandleStressCommand(string[] parts)
    {
        if (parts.Length < 2 || !float.TryParse(parts[1], out float targetStress))
        {
            Log("<color=#E06C75>[Usage] stress <0-100> (เช่น stress 50)</color>");
            return;
        }

        StressManager sm = UnityEngine.Object.FindAnyObjectByType<StressManager>();
        if (sm != null)
        {
            targetStress = Mathf.Clamp(targetStress, 0f, 100f);
            float diff = targetStress - sm.currentStress;
            sm.ChangeStress(diff);
            Log($"<color=#98C379>[Success] ปรับระดับความเครียดเป็น {Mathf.RoundToInt(sm.currentStress)}% เรียบร้อย!</color>");
        }
        else
        {
            Log("<color=#E06C75>[Error] ไม่พบ StressManager ในฉากปัจจุบัน!</color>");
        }
    }

    private void HandleAddStressCommand(string[] parts)
    {
        if (parts.Length < 2 || !float.TryParse(parts[1], out float delta))
        {
            Log("<color=#E06C75>[Usage] addstress <+/-ค่า> (เช่น addstress 25 หรือ addstress -25)</color>");
            return;
        }

        StressManager sm = UnityEngine.Object.FindAnyObjectByType<StressManager>();
        if (sm != null)
        {
            sm.ChangeStress(delta);
            Log($"<color=#98C379>[Success] เปลี่ยนแปลงความเครียด {delta:+0;-0}% (ปัจจุบัน: {Mathf.RoundToInt(sm.currentStress)}%)</color>");
        }
        else
        {
            Log("<color=#E06C75>[Error] ไม่พบ StressManager ในฉากปัจจุบัน!</color>");
        }
    }

    private void HandleHealCommand()
    {
        StressManager sm = UnityEngine.Object.FindAnyObjectByType<StressManager>();
        if (sm != null)
        {
            sm.ChangeStress(-sm.currentStress);
            Log("<color=#98C379>[Success] ล้างความเครียดทั้งหมดกลับสู่ 0% เรียบร้อย!</color>");
        }
        else
        {
            Log("<color=#E06C75>[Error] ไม่พบ StressManager ในฉากปัจจุบัน!</color>");
        }
    }

    private void HandleSaveCommand(string[] parts)
    {
        int slot = 1;
        if (parts.Length >= 2 && int.TryParse(parts[1], out int s)) slot = s;
        slot = Mathf.Clamp(slot, 1, 4);

        SaveSystem saveSys = SaveSystem.Instance != null ? SaveSystem.Instance : UnityEngine.Object.FindAnyObjectByType<SaveSystem>();
        if (saveSys != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 pos = player != null ? player.transform.position : new Vector3(-0.8f, 1f, -6.9f);
            int sceneIdx = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            saveSys.SaveGameFromGlobal(slot, pos, sceneIdx);
            Log($"<color=#98C379>[Success] บันทึกเกมลงช่อง Slot {slot} เรียบร้อย!</color>");
        }
        else
        {
            Log("<color=#E06C75>[Error] ไม่พบ SaveSystem!</color>");
        }
    }

    private void HandleLoadCommand(string[] parts)
    {
        int slot = 1;
        if (parts.Length >= 2 && int.TryParse(parts[1], out int s)) slot = s;
        slot = Mathf.Clamp(slot, 1, 4);

        SaveSystem saveSys = SaveSystem.Instance != null ? SaveSystem.Instance : UnityEngine.Object.FindAnyObjectByType<SaveSystem>();
        if (saveSys != null && saveSys.HasSaveFile(slot))
        {
            Log($"<color=#98C379>[Success] กำลังโหลดข้อมูลเกมจาก Slot {slot}...</color>");
            CloseConsole();
            SaveData data = saveSys.LoadGameToGlobal(slot);
            if (data != null)
            {
                int sceneToLoad = data.currentSceneIndex > 0 ? data.currentSceneIndex : 1;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Log($"<color=#E06C75>[Error] ไม่พบไฟล์เซฟใน Slot {slot}</color>");
        }
    }

    private void HandleSpeedCommand(string[] parts)
    {
        if (parts.Length < 2 || !float.TryParse(parts[1], out float spd))
        {
            Log("<color=#E06C75>[Usage] speed <value> (เช่น speed 8 หรือ speed 5)</color>");
            return;
        }

        FirstPersonController fpc = UnityEngine.Object.FindAnyObjectByType<FirstPersonController>();
        if (fpc != null)
        {
            var field = typeof(FirstPersonController).GetField("moveSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(fpc, spd);
                Log($"<color=#98C379>[Success] ปรับความเร็วเดิน (FirstPersonController) เป็น {spd} เรียบร้อย!</color>");
                return;
            }
        }

        PlayerMovement pm = UnityEngine.Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            pm.speed = spd;
            Log($"<color=#98C379>[Success] ปรับความเร็วเดิน (PlayerMovement 2.5D) เป็น {spd} เรียบร้อย!</color>");
            return;
        }

        Log("<color=#E06C75>[Error] ไม่พบคอมโพเนนต์ควบคุมตัวละครในฉากนี้</color>");
    }

    public void Log(string message)
    {
        logEntries.Add(message);
        if (logEntries.Count > MaxLogLines)
        {
            logEntries.RemoveAt(0);
        }

        UpdateLogText();
    }

    public void ClearLog()
    {
        logEntries.Clear();
        UpdateLogText();
    }

    private void UpdateLogText()
    {
        if (outputLogText != null)
        {
            outputLogText.text = string.Join("\n", logEntries);
        }

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// สร้าง UI ของ Developer Console แบบ Dynamic อัตโนมัติหากยังไม่มีในฉาก
    /// </summary>
    public void EnsureConsoleUI()
    {
        if (consolePanel != null && consoleCanvas != null) return;

        // ค้นหาฟอนต์ที่ใช้ในโปรเจกต์
        TMP_FontAsset font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Length > 0 ? Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0] : null;
        foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
        {
            if (f != null && (f.name.Contains("Kanit") || f.name.Contains("Thai") || f.name.Contains("Sarabun") || f.name.Contains("Prompt")))
            {
                font = f;
                break;
            }
        }

        // 1. สร้าง Canvas
        GameObject canvasObj = new GameObject("DevConsoleCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObj.transform.SetParent(transform, false);
        Canvas c = canvasObj.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 99; // ให้อยู่เหนือเมนูปกติ (แต่อยู่ใต้จอดำข้ามวัน)

        CanvasScaler cs = canvasObj.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;

        consoleCanvas = canvasObj;

        // 2. สร้าง Main Panel
        GameObject panelObj = new GameObject("ConsolePanel", typeof(RectTransform), typeof(Image));
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRt = panelObj.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(980f, 540f);
        panelRt.anchoredPosition = Vector2.zero;

        Image panelImg = panelObj.GetComponent<Image>();
        panelImg.color = new Color(0.08f, 0.1f, 0.14f, 0.96f);

        // กรอบขอบของ Panel (Outline)
        Outline outline = panelObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.35f, 0.45f, 0.6f, 0.8f);
        outline.effectDistance = new Vector2(2f, -2f);

        consolePanel = panelObj;

        // 3. Header Bar
        GameObject headerObj = new GameObject("HeaderBar", typeof(RectTransform), typeof(Image));
        headerObj.transform.SetParent(panelObj.transform, false);
        RectTransform headerRt = headerObj.GetComponent<RectTransform>();
        headerRt.anchorMin = new Vector2(0f, 1f);
        headerRt.anchorMax = new Vector2(1f, 1f);
        headerRt.pivot = new Vector2(0.5f, 1f);
        headerRt.sizeDelta = new Vector2(0f, 40f);
        headerRt.anchoredPosition = Vector2.zero;

        Image headerImg = headerObj.GetComponent<Image>();
        headerImg.color = new Color(0.12f, 0.15f, 0.22f, 1f);

        GameObject headerTextObj = new GameObject("HeaderText", typeof(RectTransform), typeof(TextMeshProUGUI));
        headerTextObj.transform.SetParent(headerObj.transform, false);
        RectTransform headerTextRt = headerTextObj.GetComponent<RectTransform>();
        headerTextRt.anchorMin = Vector2.zero;
        headerTextRt.anchorMax = Vector2.one;
        headerTextRt.offsetMin = new Vector2(16f, 0f);
        headerTextRt.offsetMax = new Vector2(-16f, 0f);

        TextMeshProUGUI headerTmp = headerTextObj.GetComponent<TextMeshProUGUI>();
        if (font != null) headerTmp.font = font;
        headerTmp.text = "> COMMAND PROMPT (DEVELOPER CONSOLE)  -  [กด ~ หรือ ESC เพื่อปิด]";
        headerTmp.fontSize = 18f;
        headerTmp.fontStyle = FontStyles.Bold;
        headerTmp.color = new Color(0.4f, 0.8f, 1f);
        headerTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // 4. Output Scroll Area
        GameObject scrollObj = new GameObject("LogScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(RectMask2D));
        scrollObj.transform.SetParent(panelObj.transform, false);
        RectTransform scrollRt = scrollObj.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0f, 0f);
        scrollRt.anchorMax = new Vector2(1f, 1f);
        scrollRt.offsetMin = new Vector2(16f, 65f);
        scrollRt.offsetMax = new Vector2(-16f, -48f);

        scrollRect = scrollObj.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 25f;

        GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObj.transform.SetParent(scrollObj.transform, false);
        RectTransform contentRt = contentObj.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup vlg = contentObj.GetComponent<VerticalLayoutGroup>();
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;

        ContentSizeFitter csf = contentObj.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRt;

        GameObject textObj = new GameObject("LogText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(contentObj.transform, false);
        outputLogText = textObj.GetComponent<TextMeshProUGUI>();
        if (font != null) outputLogText.font = font;
        outputLogText.fontSize = 17f;
        outputLogText.color = Color.white;
        outputLogText.textWrappingMode = TextWrappingModes.Normal;
        outputLogText.lineSpacing = 4f;

        // 5. Input Field Bar (ล่างสุด)
        GameObject inputBgObj = new GameObject("InputBackground", typeof(RectTransform), typeof(Image));
        inputBgObj.transform.SetParent(panelObj.transform, false);
        RectTransform inputBgRt = inputBgObj.GetComponent<RectTransform>();
        inputBgRt.anchorMin = new Vector2(0f, 0f);
        inputBgRt.anchorMax = new Vector2(1f, 0f);
        inputBgRt.pivot = new Vector2(0.5f, 0f);
        inputBgRt.sizeDelta = new Vector2(-32f, 44f);
        inputBgRt.anchoredPosition = new Vector2(0f, 12f);

        Image inputBgImg = inputBgObj.GetComponent<Image>();
        inputBgImg.color = new Color(0.04f, 0.05f, 0.08f, 1f);

        Outline inputOutline = inputBgObj.AddComponent<Outline>();
        inputOutline.effectColor = new Color(0.2f, 0.35f, 0.5f, 0.8f);
        inputOutline.effectDistance = new Vector2(1.5f, -1.5f);

        // TextArea
        GameObject textAreaObj = new GameObject("TextArea", typeof(RectTransform), typeof(RectMask2D));
        textAreaObj.transform.SetParent(inputBgObj.transform, false);
        RectTransform textAreaRt = textAreaObj.GetComponent<RectTransform>();
        textAreaRt.anchorMin = Vector2.zero;
        textAreaRt.anchorMax = Vector2.one;
        textAreaRt.offsetMin = new Vector2(14f, 4f);
        textAreaRt.offsetMax = new Vector2(-14f, -4f);

        // Placeholder Text
        GameObject placeholderObj = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
        placeholderObj.transform.SetParent(textAreaObj.transform, false);
        RectTransform placeholderRt = placeholderObj.GetComponent<RectTransform>();
        placeholderRt.anchorMin = Vector2.zero;
        placeholderRt.anchorMax = Vector2.one;
        placeholderRt.offsetMin = Vector2.zero;
        placeholderRt.offsetMax = Vector2.zero;

        TextMeshProUGUI placeholderTmp = placeholderObj.GetComponent<TextMeshProUGUI>();
        if (font != null) placeholderTmp.font = font;
        placeholderTmp.text = "พิมพ์คำสั่งที่นี่... (พิมพ์ help เพื่อดูคำสั่งทั้งหมด)";
        placeholderTmp.fontSize = 17f;
        placeholderTmp.fontStyle = FontStyles.Italic;
        placeholderTmp.color = new Color(0.5f, 0.55f, 0.65f, 0.6f);
        placeholderTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // Input Text
        GameObject inputTextObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        inputTextObj.transform.SetParent(textAreaObj.transform, false);
        RectTransform inputTextRt = inputTextObj.GetComponent<RectTransform>();
        inputTextRt.anchorMin = Vector2.zero;
        inputTextRt.anchorMax = Vector2.one;
        inputTextRt.offsetMin = Vector2.zero;
        inputTextRt.offsetMax = Vector2.zero;

        TextMeshProUGUI inputTextTmp = inputTextObj.GetComponent<TextMeshProUGUI>();
        if (font != null) inputTextTmp.font = font;
        inputTextTmp.fontSize = 17f;
        inputTextTmp.color = Color.white;
        inputTextTmp.alignment = TextAlignmentOptions.MidlineLeft;

        inputField = inputBgObj.AddComponent<TMP_InputField>();
        inputField.textViewport = textAreaRt;
        inputField.textComponent = inputTextTmp;
        inputField.placeholder = placeholderTmp;
        inputField.fontAsset = font;
        inputField.pointSize = 17f;
        inputField.caretColor = new Color(0.4f, 0.8f, 1f);
        inputField.caretWidth = 2;
        inputField.customCaretColor = true;

        inputField.onSubmit.AddListener(OnSubmitCommand);

        // ข้อความต้อนรับ
        Log("<color=#61AFEF>Depressonant Developer Console v1.0 พร้อมใช้งาน</color>");
        Log("<color=#ABB2BF>พิมพ์ <color=#E5C07B>help</color> เพื่อดูรายชื่อคำสั่งทั้งหมด หรือกด <color=#E5C07B>~</color> / <color=#E5C07B>ESC</color> เพื่อปิดหน้าต่าง</color>");
    }

    private void HandleWarpCommand(string[] parts)
    {
        if (parts.Length < 2)
        {
            Log("<color=#E06C75>[Usage] warp <home|school|outside|bedroom|mainmenu> หรือ warp <x> <y> <z></color>");
            return;
        }

        // กรณีระบุพิกัด X Y Z
        if (parts.Length >= 4 && float.TryParse(parts[1], out float x) && float.TryParse(parts[2], out float y) && float.TryParse(parts[3], out float z))
        {
            CharacterController cc = UnityEngine.Object.FindAnyObjectByType<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.transform.position = new Vector3(x, y, z);
                cc.enabled = true;
                Log($"<color=#98C379>[Success] วาร์ปตัวละครไปยังพิกัด ({x:F2}, {y:F2}, {z:F2}) สำเร็จ!</color>");
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) player = GameObject.Find("Player");
                if (player != null)
                {
                    player.transform.position = new Vector3(x, y, z);
                    Log($"<color=#98C379>[Success] วาร์ปตัวละครไปยังพิกัด ({x:F2}, {y:F2}, {z:F2}) สำเร็จ!</color>");
                }
                else
                {
                    Log("<color=#E06C75>[Error] ไม่พบตัวละคร Player ในฉากปัจจุบัน</color>");
                }
            }
            return;
        }

        string target = parts[1].ToLower();
        string sceneName = "";
        string spawnPoint = "Player_Spawn_Point";

        switch (target)
        {
            case "home":
            case "house":
                sceneName = "Home";
                spawnPoint = "Spawn_From_Bedroom";
                break;

            case "school":
                sceneName = "School";
                spawnPoint = "Player_Spawn_Point";
                break;

            case "outside":
            case "park":
            case "street":
                sceneName = "OutSide";
                spawnPoint = "Player_Spawn_Point";
                break;

            case "bedroom":
            case "room":
            case "3d":
                sceneName = "Bedroom_3D";
                spawnPoint = "Player_Spawn_Point";
                break;

            case "mainmenu":
            case "menu":
                sceneName = "MainMenu";
                spawnPoint = "";
                break;

            default:
                sceneName = parts[1];
                break;
        }

        Log($"<color=#98C379>[Warp] กำลังวาร์ปไปยังฉาก '{sceneName}'...</color>");
        CloseConsole();

        if (string.IsNullOrEmpty(spawnPoint))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneTransitionManager.LoadSceneWithSpawn(sceneName, spawnPoint);
        }
    }

    private void HandleSkipTutorialCommand()
    {
        TutorialManager tm = TutorialManager.Instance;
        if (tm == null) tm = UnityEngine.Object.FindAnyObjectByType<TutorialManager>();

        if (tm != null)
        {
            tm.SkipTutorial();
            Log("<color=#98C379>[Success] ข้ามช่วงสอนเล่น (Tutorial) เรียบร้อยแล้ว!</color>");
        }
        else
        {
            Log("<color=#E5C07B>[Notice] ไม่พบตัวจัดการ Tutorial ในฉากนี้ หรือช่วงสอนเล่นเสร็จสิ้นไปแล้ว</color>");
        }
    }
}
