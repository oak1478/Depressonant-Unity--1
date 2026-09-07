using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [Header("ข้อมูลวันปัจจุบัน")]
    [Range(1, 15)]
    [SerializeField] private int dayIndex = 1;

    public int currentDay
    {
        get
        {
            if (GameManagerSetup.Instance != null)
            {
                return GameManagerSetup.Instance.currentDay;
            }
            return dayIndex;
        }
        set
        {
            dayIndex = value;
            if (GameManagerSetup.Instance != null)
            {
                GameManagerSetup.Instance.currentDay = value;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        // ซิงค์ค่าวันปัจจุบันระดับโลกเมื่อเข้าสู่ฉากใหม่
        if (GameManagerSetup.Instance != null)
        {
            dayIndex = GameManagerSetup.Instance.currentDay;
        }

        UpdateAllAppearances();
    }

    private void Start()
    {
        // โหลดค่าวันปัจจุบันระดับโลกมาใช้เมื่อเริ่มฉาก
        if (GameManagerSetup.Instance != null)
        {
            dayIndex = GameManagerSetup.Instance.currentDay;
        }

        UpdateAllAppearances();
    }

    public void GoToNextDay()
    {
        if (currentDay < 15)
        {
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
                    Debug.Log("[DayManager] Nia รับไม่ไหว ความเครียดสูงสุดติดต่อกัน 2 วัน! ตัดเข้าฉากจบพิเศษ");
                    SceneManager.LoadScene("GameOver_Stress");
                    return;
                }
            }

            currentDay++;
            
            // ซิงค์ค่าวันปัจจุบันกลับคืนระบบกลางระดับโลก
            if (GameManagerSetup.Instance != null)
            {
                GameManagerSetup.Instance.currentDay = currentDay;
            }

            if (currentDay >= 15)
            {
                TriggerEnding();
                return;
            }

            Debug.Log("[DayManager] ข้ามวันสำเร็จ! ตอนนี้คือวันที่ " + currentDay);
            
            UpdateAllAppearances();

            if (InventoryManager.Instance != null) InventoryManager.Instance.ResetDailyItems();
            StressManager stress = Object.FindAnyObjectByType<StressManager>();
            if (stress != null) stress.ResetDailyModifiers();
        }
    }

    // ⚡ [เพิ่มใหม่] ฟังก์ชันสำหรับอัปเดตตัวตน NPC และไอเทมในฉากทั้งหมดอย่างปลอดภัย
    public void UpdateAllAppearances()
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
            Debug.Log("[Ending] Ending A: The Existence (ความเครียดต่ำกว่า 30% และมีภาพวาดเรน)");
            SceneManager.LoadScene("Ending_A");
        }
        else if (finalStress <= 70f)
        {
            Debug.Log("[Ending] Ending B: Fading Away (ความเครียด 31-70% หรือ ความเครียดต่ำกว่า 30% แต่ไม่มีภาพวาดเรน)");
            SceneManager.LoadScene("Ending_B");
        }
        else
        {
            Debug.Log("[Ending] Ending C: Bad or Die (ความเครียด 71-100%)");
            SceneManager.LoadScene("Ending_C");
        }
    }
}