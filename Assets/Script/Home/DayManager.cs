using UnityEngine;

public class DayManager : MonoBehaviour
{
    [Header("ข้อมูลวันปัจจุบัน")]
    [Range(1, 15)]
    public int currentDay = 1;

    private void Start()
    {
        // โหลดค่าวันปัจจุบันระดับโลกมาใช้เมื่อเริ่มฉาก
        if (GameManagerSetup.Instance != null)
        {
            currentDay = GameManagerSetup.Instance.currentDay;
        }

        // ⚡ [เพิ่มใหม่] บังคับอัปเดตสถานะตัวละครและไอเทมทั้งหมดตามวันปัจจุบันทันทีที่โหลดฉาก
        UpdateAllAppearances();
    }

    public void GoToNextDay()
    {
        if (currentDay < 15)
        {
            // ⚡ [เพิ่มใหม่] ตรวจสอบเงื่อนไขความเครียดสะสมต่อเนื่องก่อนข้ามวัน
            if (GameManagerSetup.Instance != null)
            {
                if (GameManagerSetup.Instance.currentStress >= 100f)
                {
                    GameManagerSetup.Instance.consecutiveMaxStressDays++;
                }
                else
                {
                    GameManagerSetup.Instance.consecutiveMaxStressDays = 0;
                }

                // หากเครียด 100% สองวันติดกัน จะคัทเข้าฉากจบพิเศษ Game Over
                if (GameManagerSetup.Instance.consecutiveMaxStressDays >= 2)
                {
                    Debug.Log("⚠️ Nia รับไม่ไหว ความเครียดสูงสุดติดต่อกัน 2 วัน! ตัดเข้าฉากจบพิเศษ");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver_Stress");
                    return;
                }
            }

            currentDay++;
            
            // ซิงค์ค่าวันปัจจุบันกลับคืนระบบกลางระดับโลก
            if (GameManagerSetup.Instance != null)
            {
                GameManagerSetup.Instance.currentDay = currentDay;
            }

            // ⚡ [เพิ่มใหม่] หากข้ามวันมาจนถึงวันที่ 15 ให้ทำระบบตัดสินฉากจบปกติ
            if (currentDay >= 15)
            {
                TriggerEnding();
                return;
            }

            Debug.Log("ข้ามวันสำเร็จ! ตอนนี้คือวันที่ " + currentDay);
            
            // ⚡ [ปรับปรุงใหม่] บังคับอัปเดตสถานะตัวละครและไอเทมในซีนใหม่
            UpdateAllAppearances();

            // ⚡ [เพิ่มใหม่] รีเซ็ตความสามารถไอเทมรายวัน
            if (InventoryManager.Instance != null) InventoryManager.Instance.ResetDailyItems();
            StressManager stress = Object.FindAnyObjectByType<StressManager>();
            if (stress != null) stress.ResetDailyModifiers();
        }
    }

    // ⚡ [เพิ่มใหม่] ฟังก์ชันสำหรับอัปเดตตัวตน NPC และไอเทมในฉากทั้งหมดอย่างปลอดภัย
    private void UpdateAllAppearances()
    {
        // อัปเดต NPC ทั้งหมด
        NPCAppearanceController[] allNPCs = Object.FindObjectsByType<NPCAppearanceController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPCAppearanceController npc in allNPCs)
        {
            try
            {
                if (npc != null)
                {
                    npc.UpdateAppearance();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DayManager] เกิดความเสียหายระหว่างอัปเดต NPC: {npc.name} - {e.Message}");
            }
        }

        // อัปเดตไอเทมในฉากทั้งหมด
        ItemAppearanceController[] allItems = Object.FindObjectsByType<ItemAppearanceController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (ItemAppearanceController item in allItems)
        {
            try
            {
                if (item != null)
                {
                    item.UpdateItemAppearance();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DayManager] เกิดความเสียหายระหว่างอัปเดต Item: {item.name} - {e.Message}");
            }
        }
    }

    // ⚡ [เพิ่มใหม่] ฟังก์ชันตัดสินฉากจบตามข้อกำหนดรายงาน
    private void TriggerEnding()
    {
        float finalStress = 0f;
        bool hasRainDrawing = false;

        if (GameManagerSetup.Instance != null)
        {
            finalStress = GameManagerSetup.Instance.currentStress;
            foreach (var globalItem in GameManagerSetup.Instance.savedInventory)
            {
                if (globalItem.itemTypeName == ItemType.RainDrawing.ToString() && globalItem.count > 0)
                {
                    hasRainDrawing = true;
                    break;
                }
            }
        }

        if (!hasRainDrawing && InventoryManager.Instance != null)
        {
            hasRainDrawing = InventoryManager.Instance.HasItem(ItemType.RainDrawing);
        }

        Debug.Log($"[Ending Triggered] finalStress: {finalStress} | hasRainDrawing: {hasRainDrawing}");

        if (finalStress <= 30f && hasRainDrawing)
        {
            Debug.Log("🎉 Ending A: The Existence (ความเครียดต่ำกว่า 30% และมีภาพวาดเรน)");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Ending_A");
        }
        else if (finalStress <= 70f)
        {
            Debug.Log("🎭 Ending B: Fading Away (ความเครียด 31-70% หรือ ความเครียดต่ำกว่า 30% แต่ไม่มีภาพวาดเรน)");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Ending_B");
        }
        else
        {
            Debug.Log("🥀 Ending C: Bad or Die (ความเครียด 71-100%)");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Ending_C");
        }
    }
}