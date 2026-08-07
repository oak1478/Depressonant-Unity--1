using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    public static SaveData pendingLoadData = null; // ⚡ [เพิ่มใหม่] บัฟเฟอร์เก็บเซฟไว้ชั่วคราวก่อนย้ายซีน

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this); // ⚡ [แก้ไข] ทำลายเฉพาะคอมโพเนนต์ที่เป็นตัวซ้ำซ้อนเท่านั้น เพื่อไม่ให้ GameObject หลัก (เช่น GameManagerSetup) โดนลบตามไปด้วย
            return;
        }
    }

    // ⚡ [เพิ่มใหม่] ดึงตำแหน่งไฟล์เซฟตามสล็อตโดยระบุชื่อไฟล์เป็น gamesave_x.json
    public string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/gamesave_{slotIndex}.json";
    }

    // ฟังก์ชันสำหรับ "เซฟเกม" ลงสล็อตที่กำหนด
    public void SaveGame(int slotIndex, SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetSaveFilePath(slotIndex);
        File.WriteAllText(path, json);
        Debug.Log($"บันทึกเกมเรียบร้อยที่ Slot {slotIndex}: {path}");
    }

    // เซฟข้อมูลของเกมจาก GameManagerSetup ลงไฟล์ JSON ของสล็อตที่กำหนด
    public void SaveGameFromGlobal(int slotIndex, Vector3 playerPosition, int currentSceneIndex)
    {
        SaveData data = new SaveData();
        data.playerX = playerPosition.x;
        data.playerY = playerPosition.y;
        data.playerZ = playerPosition.z;
        data.currentSceneIndex = currentSceneIndex;
        data.activeSaveSlot = slotIndex;

        // ดึงค่าสถานะระดับโลกมาเซฟลงไฟล์
        if (GameManagerSetup.Instance != null)
        {
            data.currentDay = GameManagerSetup.Instance.currentDay;
            data.currentStress = GameManagerSetup.Instance.currentStress;
            data.consecutiveMaxStressDays = GameManagerSetup.Instance.consecutiveMaxStressDays;
            data.inventoryItems = new List<GlobalInventoryItem>(GameManagerSetup.Instance.savedInventory);
            data.pickedUpItemIDs = new List<string>(GameManagerSetup.Instance.pickedUpItemIDs);
            data.playTime = GameManagerSetup.Instance.playTime;
        }

        SaveGame(slotIndex, data);
    }

    // ฟังก์ชันสำหรับเรียกเซฟทั่วไป (จะดึงช่อง activeSaveSlot ล่าสุดมาเซฟโดยอัตโนมัติ)
    public void SaveGameFromGlobal(Vector3 playerPosition, int currentSceneIndex)
    {
        int activeSlot = 1;
        if (GameManagerSetup.Instance != null)
        {
            activeSlot = GameManagerSetup.Instance.activeSaveSlot;
        }
        SaveGameFromGlobal(activeSlot, playerPosition, currentSceneIndex);
    }

    // ฟังก์ชันสำหรับ "โหลดเกม" ดิบของแต่ละสล็อต
    public SaveData LoadGame(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning($"ไม่พบไฟล์เซฟสล็อต {slotIndex}");
            return null;
        }
    }

    // โหลดข้อมูลจากไฟล์ JSON ของสล็อตที่ระบุเพื่อส่งไปอัปเดตในคลาสระดับโลก GameManagerSetup
    public SaveData LoadGameToGlobal(int slotIndex)
    {
        SaveData data = LoadGame(slotIndex);
        if (data == null) return null;

        if (GameManagerSetup.Instance != null)
        {
            // อัปเดตข้อมูลกลางของเกม
            GameManagerSetup.Instance.currentDay = data.currentDay > 0 ? data.currentDay : 1;
            GameManagerSetup.Instance.currentStress = data.currentStress;
            GameManagerSetup.Instance.consecutiveMaxStressDays = data.consecutiveMaxStressDays;
            GameManagerSetup.Instance.savedInventory = data.inventoryItems != null 
                ? new List<GlobalInventoryItem>(data.inventoryItems) 
                : new List<GlobalInventoryItem>();
            GameManagerSetup.Instance.pickedUpItemIDs = data.pickedUpItemIDs != null 
                ? new List<string>(data.pickedUpItemIDs) 
                : new List<string>();
            GameManagerSetup.Instance.activeSaveSlot = slotIndex; // ซิงค์เลขสล็อตกลับไปด้วย
            GameManagerSetup.Instance.playTime = data.playTime;
            
            Debug.Log($"[SaveSystem] อัปเดตข้อมูลกลางสำเร็จ (Slot {slotIndex})! วันที่: {GameManagerSetup.Instance.currentDay} | ความเครียด: {GameManagerSetup.Instance.currentStress}%");
        }
        else
        {
            // ⚡ [เพิ่มใหม่] เก็บเซฟไว้รอย้ายซีน
            pendingLoadData = data;
            pendingLoadData.activeSaveSlot = slotIndex;
            Debug.Log($"[SaveSystem] บันทึกเซฟไว้ใน Pending Data (Slot {slotIndex}) เพื่อรอสปอนเซอร์ตอนเริ่มซีนถัดไป!");
        }

        return data;
    }

    // เช็กว่ามีไฟล์เซฟสำหรับสล็อตนี้หรือเปล่า
    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetSaveFilePath(slotIndex));
    }
}