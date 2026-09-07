using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct GlobalInventoryItem
{
    public int slotIndex;
    public string itemTypeName; // เก็บเป็นชื่อ string ของ Enum เพื่อความปลอดภัยในการเซฟเป็นไฟล์ JSON
    public int count;
}

public class GameManagerSetup : MonoBehaviour
{
    // ⚡ ระบบ Singleton เพื่อการเรียกใช้งานข้ามคลาสที่ง่ายขึ้น
    public static GameManagerSetup Instance { get; private set; }

    [Header("Global Game State")]
    [Range(0, 100)] public float currentStress = 0f; // ค่าความเครียดสะสมหลัก
    [Range(1, 20)] public int currentDay = 1;         // วันปัจจุบัน
    public int consecutiveMaxStressDays = 0;          // จำนวนวันที่ความเครียดเต็ม 100% ติดต่อกัน
    public int activeSaveSlot = 1; // ⚡ สล็อตเซฟปัจจุบัน (1-4)
    public float playTime = 0f;    // ⚡ เก็บเวลาเล่นรวมสะสม (หน่วยเป็นวินาที)

    [Header("Player Load State")]
    public bool hasLoadedPosition = false;
    public Vector3 loadedPlayerPosition;

    [Header("Global Inventory")]
    public List<GlobalInventoryItem> savedInventory = new List<GlobalInventoryItem>(); // ลิสต์เก็บไอเทมในกระเป๋า

    [Header("Items Picked Up")]
    public List<string> pickedUpItemIDs = new List<string>(); // ลิสต์เก็บรหัสไอเทมที่ถูกเก็บไปแล้วในเกม

    public bool IsItemPickedUp(string id)
    {
        return pickedUpItemIDs.Contains(id);
    }

    public void RegisterPickedUpItem(string id)
    {
        if (!pickedUpItemIDs.Contains(id))
        {
            pickedUpItemIDs.Add(id);
        }
    }

    [Header("Daily Talk State")]
    public List<string> talkedNPCsToday = new List<string>();

    public bool HasTalkedToNPCToday(string npcName)
    {
        if (string.IsNullOrEmpty(npcName)) return false;

        string cleanSearch = npcName.Trim();
        if (cleanSearch.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanSearch = cleanSearch.Substring(4);
        }

        foreach (string talked in talkedNPCsToday)
        {
            if (string.IsNullOrEmpty(talked)) continue;

            string cleanTalked = talked.Trim();
            if (cleanTalked.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanTalked = cleanTalked.Substring(4);
            }

            if (string.Equals(talked, npcName.Trim(), System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(cleanTalked, cleanSearch, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterNPCTalkedToday(string npcName)
    {
        if (string.IsNullOrEmpty(npcName)) return;

        string cleanName = npcName.Trim();
        if (cleanName.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanName = cleanName.Substring(4);
        }

        if (!HasTalkedToNPCToday(cleanName))
        {
            talkedNPCsToday.Add(cleanName);
            Debug.Log($"[GameManagerSetup] บันทึกการพูดคุยกับ '{cleanName}' ประจำวันสำเร็จ (รวมคุยแล้ววันนี้: {talkedNPCsToday.Count} คน)");
        }
    }

    public void ResetDailyNPCTalk()
    {
        talkedNPCsToday.Clear();
        Debug.Log("[GameManagerSetup] รีเซ็ตรายชื่อ NPC ที่พูดคุยแล้วสำหรับวันใหม่");
    }

    void Awake()
    {
        // ⚡ ป้องกันไม่ให้มี GameManager ซ้ำซ้อนกันในซีน
        if (Instance == null)
        {
            Instance = this;
            
            // สั่งให้วัตถุนี้ (รวมถึงวัตถุลูก) อมตะ ข้ามซีนได้ไม่โดนทำลาย!
            DontDestroyOnLoad(gameObject); 

            // แปะ SaveSystem เข้ากับ GameManager อัตโนมัติเพื่อให้สั่งเซฟได้จากทุกที่
            if (GetComponent<SaveSystem>() == null)
            {
                gameObject.AddComponent<SaveSystem>();
            }

            // ⚡ [เพิ่มใหม่] โหลดค่าจากหน่วยความจำชั่วคราว (Pending Data) ทันทีใน Awake เพื่อแก้ปัญหา Race Conditions
            UnpackPendingSaveData();
        }
        else
        {
            // ถ้ามีอยู่แล้วและเกิดใหม่ ให้ทำลายตัวใหม่ทิ้งเพื่อยึดตัวเก่าที่มีข้อมูลเดิม
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        // หากมีข้อมูลเซฟรออยู่ใน Pending Data ให้อัปเดตข้อมูลกลางทันที
        UnpackPendingSaveData();

        // ปลดล็อก/ล็อกเมาส์ตามฉากโดยอัตโนมัติเมื่อเกิดการเปลี่ยนซีน เพื่อป้องกันปัญหาเมาส์ล่องหนหรือค้างคา
        bool is3DScene = scene.name == "Bedroom_3D";
        Cursor.visible = !is3DScene;
        Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;
        Debug.Log($"[Cursor Sync] โหลดฉาก '{scene.name}' สำเร็จ! ตั้งค่าเมาส์เริ่มต้น: visible={Cursor.visible}, lockState={Cursor.lockState}");
    }

    public void UnpackPendingSaveData()
    {
        if (SaveSystem.pendingLoadData != null)
        {
            currentDay = SaveSystem.pendingLoadData.currentDay > 0 ? SaveSystem.pendingLoadData.currentDay : 1;
            currentStress = SaveSystem.pendingLoadData.currentStress;
            consecutiveMaxStressDays = SaveSystem.pendingLoadData.consecutiveMaxStressDays;
            savedInventory = SaveSystem.pendingLoadData.inventoryItems != null 
                ? new List<GlobalInventoryItem>(SaveSystem.pendingLoadData.inventoryItems) 
                : new List<GlobalInventoryItem>();
            pickedUpItemIDs = SaveSystem.pendingLoadData.pickedUpItemIDs != null 
                ? new List<string>(SaveSystem.pendingLoadData.pickedUpItemIDs) 
                : new List<string>();
            talkedNPCsToday = SaveSystem.pendingLoadData.talkedNPCsToday != null
                ? new List<string>(SaveSystem.pendingLoadData.talkedNPCsToday)
                : new List<string>();
            activeSaveSlot = SaveSystem.pendingLoadData.activeSaveSlot;
            playTime = SaveSystem.pendingLoadData.playTime;

            // โหลดพิกัด (เฉพาะเมื่อมีพิกัดจริงที่ไม่ใช่ 0,0,0)
            if (SaveSystem.pendingLoadData.playerX != 0 || SaveSystem.pendingLoadData.playerY != 0 || SaveSystem.pendingLoadData.playerZ != 0)
            {
                hasLoadedPosition = true;
                loadedPlayerPosition = new Vector3(SaveSystem.pendingLoadData.playerX, SaveSystem.pendingLoadData.playerY, SaveSystem.pendingLoadData.playerZ);
            }
            else
            {
                hasLoadedPosition = false;
            }

            Debug.Log($"[GameManagerSetup] โหลดข้อมูลเซฟ (Slot {activeSaveSlot}) สำเร็จ! วันที่: {currentDay}");
            SaveSystem.pendingLoadData = null; // ล้างข้อมูลออก

            // ซิงค์ไปยัง DayManager ทันที
            if (DayManager.Instance != null)
            {
                DayManager.Instance.currentDay = currentDay;
                DayManager.Instance.UpdateAllAppearances();
            }
        }
    }

    void Start()
    {
    }

    void Update()
    {
        // ⚡ สะสมเวลาเล่นรวมของเซฟเกม
        playTime += Time.deltaTime;
    }
}