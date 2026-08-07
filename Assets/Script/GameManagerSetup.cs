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
    [Range(1, 15)] public int currentDay = 1;         // วันปัจจุบัน
    public int consecutiveMaxStressDays = 0;          // จำนวนวันที่ความเครียดเต็ม 100% ติดต่อกัน
    public int activeSaveSlot = 1; // ⚡ สล็อตเซฟปัจจุบัน (1-4)
    public float playTime = 0f;    // ⚡ เก็บเวลาเล่นรวมสะสม (หน่วยเป็นวินาที)

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
        // ⚡ [เพิ่มใหม่] ปลดล็อก/ล็อกเมาส์ตามฉากโดยอัตโนมัติเมื่อเกิดการเปลี่ยนซีน เพื่อป้องกันปัญหาเมาส์ล่องหนหรือค้างคา
        bool is3DScene = scene.name == "Bedroom_3D";
        Cursor.visible = !is3DScene;
        Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;
        Debug.Log($"🔌 [Cursor Sync] โหลดฉาก '{scene.name}' สำเร็จ! ตั้งค่าเมาส์เริ่มต้น: visible={Cursor.visible}, lockState={Cursor.lockState}");
    }

    void UnpackPendingSaveData()
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
            activeSaveSlot = SaveSystem.pendingLoadData.activeSaveSlot;
            playTime = SaveSystem.pendingLoadData.playTime;

            Debug.Log($"[GameManagerSetup] โหลดข้อมูลเซฟ (Slot {activeSaveSlot}) สำเร็จในช่วง Awake!");
            SaveSystem.pendingLoadData = null; // ล้างข้อมูลออก
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