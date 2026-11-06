using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    public static SaveData pendingLoadData = null; // บัฟเฟอร์เก็บเซฟไว้ชั่วคราวก่อนย้ายซีน

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this); // ทำลายเฉพาะคอมโพเนนต์ที่เป็นตัวซ้ำซ้อนเท่านั้น เพื่อไม่ให้ GameObject หลัก (เช่น GameManagerSetup) โดนลบตามไปด้วย
            return;
        }
    }

    // ดึงตำแหน่งไฟล์เซฟตามสล็อตโดยระบุชื่อไฟล์เป็น gamesave_x.json
    public string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/gamesave_{slotIndex}.json";
    }

    public SaveData ReadSlotPreview(int slotIndex)
    {
        return File.Exists(GetSaveFilePath(slotIndex)) ? LoadGame(slotIndex) : null;
    }

    public static string FormatSlotSummary(SaveData data)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(data.currentSceneIndex);
        string sceneName = Path.GetFileNameWithoutExtension(scenePath) switch
        {
            "Home" => "HOME TOWN",
            "OutSide" => "OUTSIDE",
            "School" => "SCHOOL",
            "MainMenu" or "Bedroom_3D" or "" or null => "MY ROOM",
            string name => name.Replace('_', ' ').ToUpperInvariant()
        };
        double totalSeconds = data.playTime;
        if (double.IsNaN(totalSeconds) || totalSeconds < 0 || totalSeconds >= TimeSpanMaxSeconds)
        {
            totalSeconds = 0;
        }
        var time = System.TimeSpan.FromSeconds(System.Math.Truncate(totalSeconds));
        return $"Nia\t\t\t\t{sceneName}\nDAY {data.currentDay}\t\t\t\tPlay Time {(long)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}";
    }

    private static readonly double TimeSpanMaxSeconds = System.TimeSpan.MaxValue.TotalSeconds;

    // ฟังก์ชันสำหรับ "เซฟเกม" ลงสล็อตที่กำหนด
    public void SaveGame(int slotIndex, SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetSaveFilePath(slotIndex);
        WriteSaveFile(path, json);
        Debug.Log($"บันทึกเกมเรียบร้อยที่ Slot {slotIndex}: {path}");
    }

    private static void WriteSaveFile(string path, string json)
    {
        // เขียนในโฟลเดอร์เดียวกันให้เสร็จ ก่อนแทนไฟล์เซฟเดิม
        string tempPath = path + "." + System.Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(tempPath, json);
            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
            }
            else
            {
                File.Move(tempPath, path);
            }
        }
        finally
        {
            try
            {
                File.Delete(tempPath);
            }
            catch (System.Exception ex) when (ex is IOException || ex is System.UnauthorizedAccessException)
            {
                Debug.LogWarning("[SaveSystem] ลบไฟล์เซฟชั่วคราวไม่สำเร็จ");
            }
        }
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
            data.currentDay = GameManagerSetup.Instance.currentDay > 0 ? GameManagerSetup.Instance.currentDay : 1;
            data.currentStress = GameManagerSetup.Instance.currentStress;
            data.consecutiveMaxStressDays = GameManagerSetup.Instance.consecutiveMaxStressDays;
            data.inventoryItems = new List<GlobalInventoryItem>(GameManagerSetup.Instance.savedInventory);
            data.pickedUpItemIDs = new List<string>(GameManagerSetup.Instance.pickedUpItemIDs);
            data.talkedNPCsToday = new List<string>(GameManagerSetup.Instance.talkedNPCsToday);
            data.playTime = GameManagerSetup.Instance.playTime;
        }
        else if (pendingLoadData != null)
        {
            data.currentDay = pendingLoadData.currentDay > 0 ? pendingLoadData.currentDay : 1;
            data.currentStress = pendingLoadData.currentStress;
            data.consecutiveMaxStressDays = pendingLoadData.consecutiveMaxStressDays;
            data.inventoryItems = pendingLoadData.inventoryItems != null ? new List<GlobalInventoryItem>(pendingLoadData.inventoryItems) : new List<GlobalInventoryItem>();
            data.pickedUpItemIDs = pendingLoadData.pickedUpItemIDs != null ? new List<string>(pendingLoadData.pickedUpItemIDs) : new List<string>();
            data.talkedNPCsToday = pendingLoadData.talkedNPCsToday != null ? new List<string>(pendingLoadData.talkedNPCsToday) : new List<string>();
            data.playTime = pendingLoadData.playTime;
        }
        else
        {
            data.currentDay = 1;
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
        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning($"[SaveSystem] ไฟล์เซฟสล็อต {slotIndex} ว่างเปล่า");
                return null;
            }

            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
            {
                Debug.LogWarning($"[SaveSystem] แปลงข้อมูล JSON เซฟสล็อต {slotIndex} ล้มเหลว (ได้ค่า null)");
                return null;
            }

            if (data.currentDay <= 0)
            {
                Debug.LogWarning($"[SaveSystem] ข้อมูลเซฟสล็อต {slotIndex} ไม่ถูกต้อง (currentDay <= 0)");
                return null;
            }

            // ค่า 0 ใช้ฉากสำรองสำหรับเซฟที่ไม่ได้ระบุฉาก
            if (data.currentSceneIndex < 0 || data.currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning($"[SaveSystem] เซฟสล็อต {slotIndex} ระบุฉากที่ไม่มีใน build (index {data.currentSceneIndex})");
                return null;
            }

            return data;
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning($"[SaveSystem] ข้อมูล JSON ในสล็อต {slotIndex} เสียหาย");
            return null;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"[SaveSystem] ไม่พบไฟล์เซฟสล็อต {slotIndex}");
            return null;
        }
        catch (IOException)
        {
            Debug.LogWarning($"[SaveSystem] เกิดข้อผิดพลาด IO ขณะอ่านไฟล์เซฟสล็อต {slotIndex}");
            return null;
        }
        catch (System.UnauthorizedAccessException)
        {
            Debug.LogWarning($"[SaveSystem] ไม่มีสิทธิ์เข้าถึงไฟล์เซฟสล็อต {slotIndex}");
            return null;
        }
    }

    // โหลดข้อมูลจากไฟล์ JSON ของสล็อตที่ระบุเพื่อส่งไปอัปเดตในคลาสระดับโลก GameManagerSetup
    public SaveData LoadGameToGlobal(int slotIndex)
    {
        SaveData data = LoadGame(slotIndex);
        if (data == null) return null;

        int targetDay = data.currentDay > 0 ? data.currentDay : 1;

        if (GameManagerSetup.Instance != null)
        {
            // อัปเดตข้อมูลกลางของเกม
            GameManagerSetup.Instance.currentDay = targetDay;
            GameManagerSetup.Instance.currentStress = data.currentStress;
            GameManagerSetup.Instance.consecutiveMaxStressDays = data.consecutiveMaxStressDays;
            GameManagerSetup.Instance.savedInventory = data.inventoryItems != null 
                ? new List<GlobalInventoryItem>(data.inventoryItems) 
                : new List<GlobalInventoryItem>();
            GameManagerSetup.Instance.pickedUpItemIDs = data.pickedUpItemIDs != null 
                ? new List<string>(data.pickedUpItemIDs) 
                : new List<string>();
            GameManagerSetup.Instance.talkedNPCsToday = new List<string>(); // เริ่มวันใหม่ ล้างรายชื่อ NPC ที่คุยไปแล้ว
            GameManagerSetup.Instance.activeSaveSlot = slotIndex; // ซิงค์เลขสล็อตกลับไปด้วย
            GameManagerSetup.Instance.playTime = data.playTime;
            
            // โหลดพิกัด
            GameManagerSetup.Instance.hasLoadedPosition = true;
            GameManagerSetup.Instance.loadedPlayerPosition = new Vector3(data.playerX, data.playerY, data.playerZ);
            
            Debug.Log($"[SaveSystem] อัปเดตข้อมูลกลางสำเร็จ (Slot {slotIndex})! วันที่: {GameManagerSetup.Instance.currentDay} | ความเครียด: {GameManagerSetup.Instance.currentStress}%");
        }

        // ล้างรายชื่อ NPC ใน data ด้วย เพื่อไม่ให้ค้างส่งต่อไปยัง pendingLoadData
        if (data.talkedNPCsToday != null)
        {
            data.talkedNPCsToday.Clear();
            if (InventoryManager.Instance != null) InventoryManager.Instance.ResetDailyItems();
            StressManager loadedStress = Object.FindAnyObjectByType<StressManager>();
            if (loadedStress != null) loadedStress.ResetDailyModifiers();
        }

        // บันทึกเซฟไว้ใน Pending Data เสมอ เพื่อเป็นข้อมูลสำรองรับประกันก่อนย้ายซีน
        pendingLoadData = data;
        pendingLoadData.activeSaveSlot = slotIndex;
        pendingLoadData.currentDay = targetDay;

        // ซิงค์ค่าวันไปยัง DayManager และบังคับอัปเดตตัวตนในฉากทันที
        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentDay = targetDay;
            DayManager.Instance.UpdateAllAppearances();
        }

        return data;
    }

    // เช็กว่ามีไฟล์เซฟสำหรับสล็อตนี้หรือเปล่า (ต้องมีไฟล์จริงและมีการบันทึกวัน currentDay > 0)
    public bool HasSaveFile(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (!File.Exists(path)) return false;

        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return false;
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data != null && data.currentDay > 0;
        }
        catch
        {
            return false;
        }
    }
}
