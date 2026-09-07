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
        if (scene.name == "MainMenu") return;
        RefreshQuestList();
    }

    private void EnsureDefaultSchedule()
    {
        if (questSchedule == null || questSchedule.Count == 0)
        {
            questSchedule = new List<DailyNPCQuestSchedule>
            {
                new DailyNPCQuestSchedule(1, "Mom"),
                new DailyNPCQuestSchedule(2, "Rein", "Den", "Jin"),
                new DailyNPCQuestSchedule(3, "Mom", "Dad", "Rein"),
                new DailyNPCQuestSchedule(4, "Mom", "Dad", "Den", "Jin", "Shirou"),
                new DailyNPCQuestSchedule(5, "Rin", "Vipar"),
                new DailyNPCQuestSchedule(6, "Momon", "Rin", "Shia", "Vipar")
            };
        }
    }

    /// <summary>
    /// สั่งซ่อนหรือแสดงหน้าต่างเควส (เรียกจาก DialogueManager หรือ InventoryManager)
    /// </summary>
    public void SetVisible(bool isVisible)
    {
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
        RefreshQuestList();
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            RefreshQuestList();
        }
    }

    public void RefreshQuestList()
    {
        if (npcListContainer == null || npcRowPrefab == null) return;

        EnsureDefaultSchedule();
        int today = GetCurrentDay();

        // 1. ดึงรายชื่อ NPC จากตารางประจำวัน
        List<string> targetNPCsToday = new List<string>();
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
                if (!string.IsNullOrEmpty(id) && !targetNPCsToday.Contains(id))
                {
                    targetNPCsToday.Add(id);
                }
            }
        }

        // 3. กรองเอาเฉพาะ NPC ที่ยังไม่ได้คุยในวันนี้
        List<string> availableNPCs = new List<string>();
        foreach (string npcName in targetNPCsToday)
        {
            bool alreadyTalked = false;

            // เช็คกับ GameManagerSetup ส่วนกลาง
            if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.HasTalkedToNPCToday(npcName))
            {
                alreadyTalked = true;
            }

            // เช็คกับ instance ในฉากปัจจุบัน (หากมี)
            if (!alreadyTalked)
            {
                foreach (NPCInteraction npc in allNPCInteractions)
                {
                    if (npc != null)
                    {
                        string id = npc.GetNPCIdentifier();
                        if (string.Equals(id, npcName, System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(npc.npcDisplayName, npcName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (npc.lastTalkedDay == today)
                            {
                                alreadyTalked = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!alreadyTalked)
            {
                availableNPCs.Add(npcName);
            }
        }

        // ล้างแถวเก่าออก
        foreach (var row in activeRows)
        {
            if (row != null) Destroy(row);
        }
        activeRows.Clear();

        if (bulletIcon == null) bulletIcon = Resources.Load<Sprite>("Quest_Bullet");
        if (completedIcon == null) completedIcon = Resources.Load<Sprite>("Quest_Check");

        if (availableNPCs.Count == 0)
        {
            if (titleObjectToHide != null) titleObjectToHide.SetActive(true);

            // สร้างแถวแจ้งเตือนว่าคุยครบแล้ว
            GameObject row = Instantiate(npcRowPrefab, npcListContainer);
            activeRows.Add(row);

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
                label.text = "พูดคุยครบแล้ว";
                label.fontStyle = FontStyles.Normal;
                label.color = new Color(0.48f, 0.85f, 0.58f, 1f); // เขียวพาสเทลโมเดิร์น
            }
        }
        else
        {
            if (titleObjectToHide != null) titleObjectToHide.SetActive(true);

            // สร้างแถวใหม่ตามจำนวน NPC ที่พบ (ไม่เกิน 6 คน)
            for (int i = 0; i < Mathf.Min(availableNPCs.Count, 6); i++)
            {
                GameObject row = Instantiate(npcRowPrefab, npcListContainer);
                activeRows.Add(row);

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
                    label.text = availableNPCs[i];
                    label.fontStyle = FontStyles.Normal;
                    label.color = new Color(0.89f, 0.92f, 0.96f, 1f); // ขาวนวล
                }
            }
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
