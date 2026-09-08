using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// ควบคุม Quest Canvas ที่แสดงรายชื่อ NPC ที่ยังมีเรื่องราวใหม่ที่ยังไม่ได้คุยในวันนี้
/// วางสคริปต์นี้ไว้ที่ออบเจกต์ใดก็ได้ในฉาก แล้วลาก Reference ใน Inspector ให้ครบ
/// </summary>
public class DailyQuestManager : MonoBehaviour
{
    public static DailyQuestManager Instance;

    [Header("UI References")]
    [Tooltip("ลาก NPCListContainer (ออบเจกต์ที่มี Vertical Layout Group) มาใส่")]
    public Transform npcListContainer;

    [Tooltip("ลาก Prefab ของแถว NPC แต่ละแถวมาใส่")]
    public GameObject npcRowPrefab;

    [Tooltip("ลาก TitleText หรือ QuestPanel มาใส่ เพื่อซ่อนเมื่อคุยครบแล้ว (ใส่หรือไม่ใส่ก็ได้)")]
    public GameObject titleObjectToHide;

    [Header("UI Visibility (ซ่อน/แสดง อัตโนมัติ)")]
    [Tooltip("ลาก QuestPanel หรือ QuestCanvas มาใส่ (หากเว้นว่างไว้ ระบบจะค้นหา Canvas จากตัวมันเองให้อัตโนมัติ)")]
    public GameObject questUIRoot;

    [Header("Quest Icons")]
    public Sprite bulletIcon;
    public Sprite completedIcon;

    [System.Serializable]
    public class DailyNPCQuestSchedule
    {
        public int dayNumber;
        public List<string> npcNames = new List<string>();

        public DailyNPCQuestSchedule(int day, params string[] names)
        {
            dayNumber = day;
            npcNames = new List<string>(names);
        }
    }

    [Header("Daily Quest Schedule")]
    [Tooltip("ตารางรายชื่อ NPC ที่มีเควสตามวัน (หากว่างไว้จะดึงค่าเริ่มต้นอัตโนมัติ)")]
    public List<DailyNPCQuestSchedule> questSchedule = new List<DailyNPCQuestSchedule>();

    [Header("Refresh Settings")]
    [Tooltip("อัปเดตรายชื่อทุกกี่วินาที (ตั้ง 1 ก็เพียงพอ ไม่ต้องทุกเฟรม)")]
    public float refreshInterval = 1f;

    private float refreshTimer = 0f;

    // เก็บแถวที่สร้างไว้เพื่อ destroy เมื่อ refresh
    private List<GameObject> activeRows = new List<GameObject>();
    private Canvas cachedCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        EnsureDefaultSchedule();

        cachedCanvas = GetComponent<Canvas>();
        if (cachedCanvas == null) cachedCanvas = GetComponentInParent<Canvas>();

        // กำหนด SortingOrder ให้อยู่ระดับ HUD (5) เพื่อให้อยู่ด้านหลังเมนูกระเป๋า (Inventory 15) และบทสนทนา (Dialogue)
        if (cachedCanvas != null && cachedCanvas.sortingOrder > 5)
        {
            cachedCanvas.sortingOrder = 5;
        }

        // ปิดการดักคลิกเมาส์ (Raycast) ทั้งหมดของ Canvas เควสโดยอัตโนมัติ
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null) raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.enabled = false;
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            SetVisible(false);
            return;
        }
        SetVisible(true);
        RefreshQuestList();
    }

    private void EnsureDefaultSchedule()
    {
        if (questSchedule == null || questSchedule.Count == 0)
        {
            questSchedule = new List<DailyNPCQuestSchedule>
            {
                new DailyNPCQuestSchedule(1, "Mom"),
                new DailyNPCQuestSchedule(2, "Rein", "Jin", "Den"),
                new DailyNPCQuestSchedule(3, "Mom", "Rein", "Dad"),
                new DailyNPCQuestSchedule(4, "Dad", "Mom", "Jin", "Den", "Shirou"),
                new DailyNPCQuestSchedule(5, "Vipar", "Rin"),
                new DailyNPCQuestSchedule(6, "Rin", "Vipar", "Shia", "Momon"),
                new DailyNPCQuestSchedule(7, "Shia", "Momon"),
                new DailyNPCQuestSchedule(8, "Rin", "Shia", "Vipar", "Park"),
                new DailyNPCQuestSchedule(9, "Park", "Shia", "Vipar"),
                new DailyNPCQuestSchedule(10, "Mom", "Dad", "Rein"),
                new DailyNPCQuestSchedule(11, "Mom", "Dad", "Jin", "Rein"),
                new DailyNPCQuestSchedule(12, "Rin", "Park", "Shia"),
                new DailyNPCQuestSchedule(13, "Momon", "Vipar", "Rin", "Shia", "Park"),
                new DailyNPCQuestSchedule(14, "Den", "Park", "Shia", "Vipar"),
                new DailyNPCQuestSchedule(15, "Vipar", "Rin", "Shia", "Park"),
                new DailyNPCQuestSchedule(16, "Momon", "Vipar", "Rin", "Park", "Shia"),
                new DailyNPCQuestSchedule(17, "Mom")
            };
        }
    }

    /// <summary>
    /// สั่งซ่อนหรือแสดงหน้าต่างเควส (เรียกจาก DialogueManager หรือ InventoryManager)
    /// </summary>
    public void SetVisible(bool isVisible)
    {
        // หากอยู่ในฉาก MainMenu ให้ซ่อนตลอดเวลา
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            isVisible = false;
        }

        // หากกระเป๋าเป้กำลังเปิดอยู่ หรือมีบทสนทนา ให้คงสถานะซ่อนไว้เสมอ
        if (isVisible)
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.inventoryPanel != null && InventoryManager.Instance.inventoryPanel.activeSelf)
            {
                isVisible = false;
            }
            DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
            if (dm != null && dm.IsDialogueActive())
            {
                isVisible = false;
            }
        }

        if (questUIRoot != null)
        {
            questUIRoot.SetActive(isVisible);
        }
        else if (cachedCanvas != null)
        {
            cachedCanvas.enabled = isVisible;
        }
        else
        {
            gameObject.SetActive(isVisible);
        }
    }

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            SetVisible(false);
            return;
        }
        RefreshQuestList();
    }

    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            RefreshQuestList();
        }
    }

    /// <summary>
    /// ตรวจสอบว่าเป้าหมายเควสประจำวันของวันนี้สำเร็จครบถ้วนแล้วหรือยัง
    /// </summary>
    public bool IsDailyQuestCompleted()
    {
        EnsureDefaultSchedule();
        int today = GetCurrentDay();
        List<string> unfinished = GetUnfinishedNPCsToday(today);
        return unfinished.Count == 0;
    }

    /// <summary>
    /// ดึงรายชื่อ NPC เป้าหมายทั้งหมดที่ต้องคุยในวันนี้
    /// </summary>
    public List<string> GetTargetNPCsToday(int today)
    {
        EnsureDefaultSchedule();
        List<string> targetNPCsToday = new List<string>();

        // 1. ดึงรายชื่อ NPC จากตารางประจำวัน
        foreach (var schedule in questSchedule)
        {
            if (schedule != null && schedule.dayNumber == today && schedule.npcNames != null)
            {
                foreach (string name in schedule.npcNames)
                {
                    if (!string.IsNullOrEmpty(name) && !targetNPCsToday.Contains(name))
                    {
                        targetNPCsToday.Add(name);
                    }
                }
            }
        }

        // 2. สแกน NPC ในฉากปัจจุบันเพิ่มเติม เพื่อรองรับกรณีมี NPC ใหม่ที่ไม่ได้ใส่ในตาราง
        NPCInteraction[] allNPCInteractions = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPCInteraction npc in allNPCInteractions)
        {
            if (npc == null) continue;

            bool hasStoryToday = false;
            foreach (var dialog in npc.dialoguesByDay)
            {
                if (dialog != null && dialog.dayNumber == today)
                {
                    hasStoryToday = true;
                    break;
                }
            }

            if (hasStoryToday)
            {
                string id = npc.GetNPCIdentifier();
                if (!string.IsNullOrEmpty(id))
                {
                    // ป้องกันการเพิ่ม Ben ซ้ำหากมี Jin อยู่แล้ว (หรือกลับกัน)
                    bool isPair = string.Equals(id, "Jin", System.StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(id, "Ben", System.StringComparison.OrdinalIgnoreCase);
                    if (isPair)
                    {
                        bool hasEither = targetNPCsToday.Exists(n => string.Equals(n, "Jin", System.StringComparison.OrdinalIgnoreCase) ||
                                                                    string.Equals(n, "Ben", System.StringComparison.OrdinalIgnoreCase));
                        if (hasEither) continue;
                    }

                    // ป้องกันการเพิ่ม Sasha หรือ Egon ซ้ำหากมี Den อยู่แล้ว
                    bool isDenFamily = string.Equals(id, "Den", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(id, "Sasha", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(id, "Egon", System.StringComparison.OrdinalIgnoreCase);
                    if (isDenFamily)
                    {
                        bool hasAnyDen = targetNPCsToday.Exists(n => string.Equals(n, "Den", System.StringComparison.OrdinalIgnoreCase) ||
                                                                     string.Equals(n, "Sasha", System.StringComparison.OrdinalIgnoreCase) ||
                                                                     string.Equals(n, "Egon", System.StringComparison.OrdinalIgnoreCase));
                        if (hasAnyDen) continue;
                    }

                    // ป้องกันการเพิ่ม Hong ซ้ำหากมี Shia อยู่แล้ว
                    bool isShiaFamily = string.Equals(id, "Shia", System.StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(id, "Hong", System.StringComparison.OrdinalIgnoreCase);
                    if (isShiaFamily)
                    {
                        bool hasAnyShia = targetNPCsToday.Exists(n => string.Equals(n, "Shia", System.StringComparison.OrdinalIgnoreCase) ||
                                                                      string.Equals(n, "Hong", System.StringComparison.OrdinalIgnoreCase));
                        if (hasAnyShia) continue;
                    }

                    if (!targetNPCsToday.Contains(id))
                    {
                        targetNPCsToday.Add(id);
                    }
                }
            }
        }

        return targetNPCsToday;
    }

    /// <summary>
    /// ตรวจสอบว่าได้พูดคุยกับ NPC คนนี้แล้วหรือยังในวันนี้ (รองรับระบบคู่และชื่อแฝง)
    /// </summary>
    public bool IsNPCTalkedToday(string npcName, int today)
    {
        if (string.IsNullOrEmpty(npcName)) return true;

        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.HasTalkedToNPCToday(npcName))
        {
            return true;
        }

        // เชื่อมโยงคู่ Ben & Jin
        bool isBenOrJin = string.Equals(npcName, "Jin", System.StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(npcName, "Ben", System.StringComparison.OrdinalIgnoreCase);

        if (isBenOrJin && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Jin") || GameManagerSetup.Instance.HasTalkedToNPCToday("Ben"))
            {
                return true;
            }
        }

        // เชื่อมโยงชื่อ Park / Prak
        bool isParkOrPrak = string.Equals(npcName, "Park", System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(npcName, "Prak", System.StringComparison.OrdinalIgnoreCase);

        if (isParkOrPrak && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Park") || GameManagerSetup.Instance.HasTalkedToNPCToday("Prak"))
            {
                return true;
            }
        }

        // เชื่อมโยงชื่อ Rein / Rain / Ren
        bool isRainOrRein = string.Equals(npcName, "Rein", System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(npcName, "Rain", System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(npcName, "Ren", System.StringComparison.OrdinalIgnoreCase);

        if (isRainOrRein && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Rein") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Rain") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Ren"))
            {
                return true;
            }
        }

        // เชื่อมโยงชื่อ Shirou / Stranger
        bool isShirouOrStranger = string.Equals(npcName, "Shirou", System.StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(npcName, "Stranger", System.StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(npcName, "A stranger", System.StringComparison.OrdinalIgnoreCase);

        if (isShirouOrStranger && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Shirou") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Stranger") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("A stranger"))
            {
                return true;
            }
        }

        // เชื่อมโยงกลุ่ม Den / Sasha / Egon
        bool isDenGroup = string.Equals(npcName, "Den", System.StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(npcName, "Sasha", System.StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(npcName, "Egon", System.StringComparison.OrdinalIgnoreCase);

        if (isDenGroup && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Den") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Sasha") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Egon"))
            {
                return true;
            }
        }

        // เชื่อมโยงคู่ Shia & Hong
        bool isShiaOrHong = string.Equals(npcName, "Shia", System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(npcName, "Hong", System.StringComparison.OrdinalIgnoreCase);

        if (isShiaOrHong && GameManagerSetup.Instance != null)
        {
            if (GameManagerSetup.Instance.HasTalkedToNPCToday("Shia") ||
                GameManagerSetup.Instance.HasTalkedToNPCToday("Hong"))
            {
                return true;
            }
        }

        // ตรวจสอบกับ instance ในฉากปัจจุบัน
        NPCInteraction[] allNPCInteractions = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPCInteraction npc in allNPCInteractions)
        {
            if (npc == null) continue;

            string id = npc.GetNPCIdentifier();
            bool matches = string.Equals(id, npcName, System.StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(npc.npcDisplayName, npcName, System.StringComparison.OrdinalIgnoreCase);

            if (isParkOrPrak)
            {
                if (string.Equals(id, "Park", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "Prak", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Park", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Prak", System.StringComparison.OrdinalIgnoreCase))
                {
                    matches = true;
                }
            }

            if (isRainOrRein)
            {
                if (string.Equals(id, "Rein", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "Rain", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "Ren", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Rein", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Rain", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Ren", System.StringComparison.OrdinalIgnoreCase))
                {
                    matches = true;
                }
            }

            if (isShirouOrStranger)
            {
                if (string.Equals(id, "Shirou", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "Stranger", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "Shirou", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(npc.npcDisplayName, "A stranger", System.StringComparison.OrdinalIgnoreCase))
                {
                    matches = true;
                }
            }

            if (matches && npc.lastTalkedDay == today)
            {
                return true;
            }

            if (isBenOrJin)
            {
                bool npcIsBenOrJin = string.Equals(id, "Jin", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(id, "Ben", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(npc.npcDisplayName, "Jin", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(npc.npcDisplayName, "Ben", System.StringComparison.OrdinalIgnoreCase);
                if (npcIsBenOrJin && npc.lastTalkedDay == today)
                {
                    return true;
                }
            }

            if (isDenGroup)
            {
                bool npcIsDenGroup = string.Equals(id, "Den", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(id, "Sasha", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(id, "Egon", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(npc.npcDisplayName, "Den", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(npc.npcDisplayName, "Sasha", System.StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(npc.npcDisplayName, "Egon", System.StringComparison.OrdinalIgnoreCase);
                if (npcIsDenGroup && npc.lastTalkedDay == today)
                {
                    return true;
                }
            }

            if (isShiaOrHong)
            {
                bool npcIsShiaOrHong = string.Equals(id, "Shia", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(id, "Hong", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(npc.npcDisplayName, "Shia", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(npc.npcDisplayName, "Hong", System.StringComparison.OrdinalIgnoreCase);
                if (npcIsShiaOrHong && npc.lastTalkedDay == today)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// ดึงรายชื่อ NPC เป้าหมายที่ยังไม่ได้คุยในวันนี้
    /// </summary>
    public List<string> GetUnfinishedNPCsToday(int today)
    {
        List<string> targetNPCsToday = GetTargetNPCsToday(today);
        List<string> availableNPCs = new List<string>();

        foreach (string npcName in targetNPCsToday)
        {
            if (!IsNPCTalkedToday(npcName, today))
            {
                availableNPCs.Add(npcName);
            }
        }

        return availableNPCs;
    }

    /// <summary>
    /// แปลงชื่อ NPC เป็นข้อความภารกิจที่อธิบายชัดเจนว่าต้องทำอะไรและไปที่ไหน
    /// </summary>
    public string GetQuestDescription(string npcName, int day)
    {
        string desc = "";
        switch (npcName.ToLowerInvariant())
        {
            case "mom":
                if (day == 11) desc = "พูดคุยกับ แม่ (Mom) ในครัว";
                else if (day == 17) desc = "พูดคุยกับ แม่ (Mom) บทสรุปเรื่องราว";
                else desc = "พูดคุยกับ แม่ (Mom) ในบ้าน";
                break;
            case "dad":
                if (day == 3) desc = "พูดคุยกับ พ่อ (Dad) ที่ป้ายรถบัส";
                else if (day == 11) desc = "พูดคุยกับ พ่อ (Dad) ที่โต๊ะกินข้าว";
                else desc = "พูดคุยกับ พ่อ (Dad) ในบ้าน";
                break;
            case "rein":
            case "rain":
            case "ren":
                if (day == 10 || day == 11) desc = "พูดคุยกับ เรน (Rain) ในห้องนอน";
                else desc = "พูดคุยกับ เรน (Rain) ในบ้าน";
                break;
            case "den":
                if (day == 14) desc = "พูดคุยกับ เดน (Den) และเด็กๆ ที่โต๊ะม้านั่ง";
                else desc = "พูดคุยกับ เดน (Den) และเพื่อนๆ นอกบ้าน";
                break;
            case "jin":
            case "ben":
                if (day == 11) desc = "พูดคุยกับ ป้าจิน และ ลุงเบน (Jin & Ben) หน้าบ้าน";
                else desc = "พูดคุยกับ ป้าจิน และ ลุงเบน (Jin & Ben) ที่สวน";
                break;
            case "shirou":
            case "stranger":
            case "a stranger":
                desc = "พูดคุยกับ คนแปลกหน้า (Shirou) นอกบ้าน";
                break;
            case "egon":
                desc = "พูดคุยกับ อีกอน (Egon) นอกบ้าน";
                break;
            case "sasha":
                desc = "พูดคุยกับ ซาช่า (Sasha) นอกบ้าน";
                break;
            case "rin":
                if (day == 6 || day == 13) desc = "พูดคุยกับ ริน (Rin) ในห้องสมุด";
                else if (day == 16) desc = "พูดคุยกับ ริน (Rin) หน้าอาคารเรียน";
                else desc = "พูดคุยกับ ริน (Rin) ที่โรงเรียน";
                break;
            case "vipar":
                if (day == 9) desc = "พูดคุยกับ ครูวิภา (Vipar) ที่โถงทางเดิน";
                else if (day == 13) desc = "พูดคุยกับ ครูวิภา (Vipar) ในห้องสมุด";
                else if (day == 14) desc = "พูดคุยกับ ครูวิภา (Vipar) หน้าห้องเรียน";
                else if (day == 15) desc = "รับผลคะแนนจาก ครูวิภา (Vipar) ในห้องเรียน";
                else if (day == 16) desc = "พูดคุยกับ ครูวิภา (Vipar) หน้าโรงเรียน";
                else desc = "พูดคุยกับ ครูวิภา (Vipar) ที่โรงเรียน";
                break;
            case "momon":
                if (day == 6 || day == 13) desc = "พูดคุยกับ ครูโมม่อน (Momon) ในห้องสมุด";
                else if (day == 16) desc = "คืนกุญแจให้ ครูโมม่อน (Momon) ในห้องสมุด";
                else desc = "พูดคุยกับ ครูโมม่อน (Momon) ที่โรงเรียน";
                break;
            case "shia":
                if (day == 7) desc = "พูดคุยกับ ชีอ่า (Shia) ในห้องเรียน";
                else if (day == 12) desc = "พูดคุยกับ ชีอ่า (Shia) ที่โต๊ะเรียน";
                else if (day == 13) desc = "พูดคุยกับ ชีอ่า (Shia) ในห้องสมุด";
                else if (day == 14) desc = "พูดคุยกับ ชีอ่า (Shia) เรื่องเวรทำความสะอาด";
                else if (day == 16) desc = "พูดคุยกับ ชีอ่า (Shia) หน้าโรงเรียน";
                else desc = "พูดคุยกับ ชีอ่า (Shia) ที่โรงเรียน";
                break;
            case "park":
            case "prak":
                if (day == 9) desc = "พูดคุยกับ ป้าก (Park) ที่บอร์ดคะแนน";
                else if (day == 12 || day == 16) desc = "พูดคุยกับ ป้าก (Park) ข้างสนามบาส";
                else if (day == 13) desc = "พูดคุยกับ ป้าก (Park) ในห้องสมุด";
                else desc = "พูดคุยกับ ป้าก (Park) ที่โรงเรียน";
                break;
            case "hong":
                desc = "พูดคุยกับ หงส์ (Hong) ที่โรงเรียน";
                break;
            default:
                desc = $"พูดคุยกับ {npcName}";
                break;
        }

        return ThaiTextAdjuster.Adjust(desc);
    }

    public void RefreshQuestList()
    {
        if (npcListContainer == null || npcRowPrefab == null) return;

        EnsureDefaultSchedule();
        int today = GetCurrentDay();

        List<string> availableNPCs = GetUnfinishedNPCsToday(today);

        // ล้างแถวเก่าออก
        foreach (var row in activeRows)
        {
            if (row != null) Destroy(row);
        }
        activeRows.Clear();

        if (bulletIcon == null) bulletIcon = Resources.Load<Sprite>("Quest_Bullet");
        if (completedIcon == null) completedIcon = Resources.Load<Sprite>("Quest_Check");

        if (titleObjectToHide != null)
        {
            titleObjectToHide.SetActive(true);
            TextMeshProUGUI titleTmp = titleObjectToHide.GetComponent<TextMeshProUGUI>();
            if (titleTmp == null) titleTmp = titleObjectToHide.GetComponentInChildren<TextMeshProUGUI>();
            if (titleTmp != null)
            {
                titleTmp.text = ThaiTextAdjuster.Adjust("เป้าหมายวันนี้");
            }
        }

        if (availableNPCs.Count == 0)
        {
            // สร้างแถวแจ้งเตือนว่าคุยครบแล้ว
            GameObject row = Instantiate(npcRowPrefab, npcListContainer);
            activeRows.Add(row);

            RectTransform rowRt = row.GetComponent<RectTransform>();
            if (rowRt != null)
            {
                rowRt.localScale = Vector3.one;
                rowRt.sizeDelta = new Vector2(rowRt.sizeDelta.x, 36f);
            }

            Image icon = row.transform.Find("StatusIcon")?.GetComponent<Image>();
            if (icon == null) icon = row.GetComponentInChildren<Image>();
            if (icon != null && completedIcon != null)
            {
                icon.sprite = completedIcon;
                icon.color = Color.white;
            }

            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = ThaiTextAdjuster.Adjust("พูดคุยครบตามเป้าหมายแล้ว");
                label.fontStyle = FontStyles.Normal;
                label.color = new Color(0.48f, 0.85f, 0.58f, 1f); // เขียวพาสเทลโมเดิร์น
            }
        }
        else
        {
            // สร้างแถวใหม่ตามจำนวน NPC ที่พบ (ไม่เกิน 6 คน) พร้อมป้องกันข้อความเควสซ้ำ
            HashSet<string> addedDescriptions = new HashSet<string>();
            for (int i = 0; i < availableNPCs.Count; i++)
            {
                string desc = GetQuestDescription(availableNPCs[i], today);
                if (addedDescriptions.Contains(desc)) continue;
                addedDescriptions.Add(desc);

                GameObject row = Instantiate(npcRowPrefab, npcListContainer);
                activeRows.Add(row);

                RectTransform rowRt = row.GetComponent<RectTransform>();
                if (rowRt != null)
                {
                    rowRt.localScale = Vector3.one;
                    rowRt.sizeDelta = new Vector2(rowRt.sizeDelta.x, 36f);
                }

                Image icon = row.transform.Find("StatusIcon")?.GetComponent<Image>();
                if (icon == null) icon = row.GetComponentInChildren<Image>();
                if (icon != null && bulletIcon != null)
                {
                    icon.sprite = bulletIcon;
                    icon.color = Color.white;
                }

                TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = desc;
                    label.fontStyle = FontStyles.Normal;
                    label.color = new Color(0.89f, 0.92f, 0.96f, 1f); // ขาวนวล
                }

                if (activeRows.Count >= 6) break;
            }
        }

        // ปรับขนาดความสูงของหน้าต่างเควสให้พอดีกับจำนวนรายการอัตโนมัติ
        RectTransform panelRect = null;
        if (questUIRoot != null)
        {
            panelRect = questUIRoot.GetComponent<RectTransform>();
        }
        if (panelRect == null && npcListContainer != null && npcListContainer.parent != null)
        {
            panelRect = npcListContainer.parent as RectTransform;
        }

        if (panelRect != null)
        {
            float topOffset = 46f;
            float bottomPadding = 14f;
            float rowHeight = 36f;
            float spacing = 8f;
            int count = Mathf.Max(1, activeRows.Count);
            float targetHeight = topOffset + (count * rowHeight) + ((count - 1) * spacing) + bottomPadding;
            panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, targetHeight);

            RectTransform listRect = npcListContainer as RectTransform;
            if (listRect != null)
            {
                listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, (count * rowHeight) + ((count - 1) * spacing));
            }
        }

        Canvas.ForceUpdateCanvases();
        if (npcListContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(npcListContainer as RectTransform);
        }
    }

    private int GetCurrentDay()
    {
        if (DayManager.Instance != null) return DayManager.Instance.currentDay;
        if (GameManagerSetup.Instance != null) return GameManagerSetup.Instance.currentDay;
        return 1;
    }

    /// <summary>
    /// เรียกฟังก์ชันนี้จากภายนอก (เช่น หลังคุยกับ NPC เสร็จ) เพื่ออัปเดตรายชื่อทันที
    /// </summary>
    public void ForceRefresh()
    {
        RefreshQuestList();
    }
}
