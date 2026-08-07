using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum ItemType { None, Candy, StrawberryMilk, Diary, Headphone, RainDrawing, Armband }

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("ลาก InventoryPanel มาใส่ช่องนี้")]
    public GameObject inventoryPanel; 

    [Header("ลาก Slot_1 ถึง Slot_6 มาใส่ในช่องนี้ตามลำดับ")]
    public Image[] itemSlots; 

    // ⚡ [เพิ่มใหม่] ลาก CountText ของ Slot 1-6 มาใส่ในช่องนี้ตามลำดับเดียวกับไอเทม
    [Header("ลาก Text แสดงจำนวนชิ้นของ Slot 1-6 มาใส่ตามลำดับ")]
    public TextMeshProUGUI[] slotCountTexts;

    private ItemType[] slotItemTypes = new ItemType[6];
    
    // ⚡ [เพิ่มใหม่] อาร์เรย์สำหรับเก็บจำนวนไอเทมในแต่ละช่อง
    private int[] slotItemCounts = new int[6];

    [Header("UI สำหรับ Diary")]
    public TextMeshProUGUI diaryStressText; 

    // [เพิ่มใหม่] ฐานข้อมูลรูปภาพไอเทมเพื่อใช้สเกลแสดงผลเมื่อเปลี่ยนซีน
    [System.Serializable]
    public struct ItemSpriteMapping
    {
        public ItemType type;
        public Sprite sprite;
    }
    [Header("ลากสไปรต์รูปภาพของไอเทมแต่ละชิ้นมาจับคู่ที่นี่ (สำหรับโหลดข้ามฉาก)")]
    public List<ItemSpriteMapping> itemSpriteDatabase = new List<ItemSpriteMapping>();

    private PlayerMovement playerMovement; 
    private StressManager stressManager;

    [HideInInspector] public bool hasUsedHeadphoneToday = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // ⚡ วางทับฟังก์ชัน Start เดิม เพื่อเพิ่มระบบเช็คปุ่มแบบละเอียด
    void Start()
    {
        inventoryPanel.SetActive(false);
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        stressManager = FindAnyObjectByType<StressManager>();

        // ⚡ [แก้ไข] เช็คประเภทซีนห้องนอน 3D เพื่อตั้งค่าเมาส์ตอนเกิดเฟรมแรกให้ถูกต้อง ไม่แย่งล็อกเมาส์กันเอง
        bool is3DScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Bedroom_3D";
        Cursor.visible = !is3DScene;
        Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;

        if (stressManager == null)
        {
            Debug.LogError("⚠️ [Inventory] หา StressManager ในฉากไม่เจอ! โปรดเช็คว่าวัตถุ StressManager มีสคริปต์แปะอยู่ไหม");
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].enabled = false; 
            slotItemTypes[i] = ItemType.None; 
            slotItemCounts[i] = 0; 

            if (slotCountTexts[i] != null) slotCountTexts[i].gameObject.SetActive(false);

            int index = i; 
            
            // ⚡ ค้นหาปุ่มแบบละเอียด (เช็คทั้งที่ตัวเอง, ตัวพ่อ, และตัวลูก)
            Button btn = itemSlots[index].GetComponent<Button>();
            if (btn == null) btn = itemSlots[index].GetComponentInParent<Button>();
            if (btn == null) btn = itemSlots[index].GetComponentInChildren<Button>();

            if (btn != null)
            {
                btn.onClick.AddListener(() => UseItem(index));
                Debug.Log($"✅ [Inventory] ผูกระบบคลิกเข้ากับปุ่มของ Slot_{index + 1} สำเร็จ!");
            }
            else
            {
                Debug.LogError($"❌ [Inventory] Slot_{index + 1} หาคอมโพเนนต์ Button ไม่เจอ! โปรดตรวจสอบว่าใน Hierarchy มีปุ่ม Button แปะอยู่กับช่องเก็บของไหม");
            }
        }

        // [เพิ่มใหม่] โหลดไอเทมข้ามฉอกจาก GameManager เข้ามาแสดงผลใน UI
        LoadFromGlobal();

        if (diaryStressText != null) diaryStressText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);

            // ⚡ [เพิ่มใหม่] ค้นหาสคริปต์เดินและจัดการความเครียดของฉากปัจจุบันใหม่ทุกครั้งเพื่อป้องกันลิงก์อ้างอิงพังเวลาข้ามฉาก
            playerMovement = Object.FindAnyObjectByType<PlayerMovement>();
            stressManager = Object.FindAnyObjectByType<StressManager>();

            // ⚡ เช็คว่าปัจจุบันเป็นฉากห้องนอน 3D หรือเปล่า
            bool is3DScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Bedroom_3D";

            // ⚡ ปลดล็อก/ล็อกเมาส์ตามสถานะการเปิดกระเป๋า (หากเป็นฉากบ้าน 2.5D จะไม่ล็อกเมาส์เลย)
            Cursor.visible = isActive || !is3DScene;
            Cursor.lockState = (isActive || !is3DScene) ? CursorLockMode.None : CursorLockMode.Locked;

            // ปิดการเดินในด่าน 2.5D
            if (playerMovement != null) playerMovement.enabled = !isActive; 

            // ⚡ [เพิ่มใหม่] ค้นหาและปิดการควบคุมกล้อง 3D บุคคลที่หนึ่ง (เพื่อไม่ให้กล้องส่ายตามการเลื่อนเมาส์ไปจิ้มปุ่ม)
            FirstPersonController fpController = Object.FindAnyObjectByType<FirstPersonController>();
            if (fpController != null) fpController.enabled = !isActive;

            // ⚡ [เพิ่มใหม่] ค้นหาและปิดตัวชี้เลเซอร์ F เพื่อไม่ให้เผลอไปกดใช้เตียงหรือกระจกระหว่างคลิกของ
            FirstPersonInteractor fpInteractor = Object.FindAnyObjectByType<FirstPersonInteractor>();
            if (fpInteractor != null) fpInteractor.enabled = !isActive;

            if (isActive && diaryStressText != null)
            {
                if (HasItem(ItemType.Diary) && stressManager != null)
                {
                    diaryStressText.gameObject.SetActive(true);
                    diaryStressText.text = "Stress: " + Mathf.RoundToInt(stressManager.currentStress) + "%";
                }
                else
                {
                    diaryStressText.gameObject.SetActive(false); 
                }
            }
        }
    }

    // ⚡ [ปรับปรุงใหม่] ฟังก์ชันเพิ่มไอเทม รองรับระบบเก็บซ้อนกัน (Stacking)
    public bool AddItem(Sprite itemSprite, ItemType type)
    {
        // เช็คก่อนว่าเป็นไอเทมประเภทที่ซ้อนกันได้ไหม (Candy และ StrawberryMilk)
        bool isStackable = (type == ItemType.Candy || type == ItemType.StrawberryMilk);

        if (isStackable)
        {
            // ลูปหาดูว่าในกระเป๋ามีไอเทมชนิดนี้อยู่แล้วหรือยัง
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].enabled && slotItemTypes[i] == type)
                {
                    slotItemCounts[i]++; // เพิ่มจำนวนชิ้นเข้าช่องเดิม
                    UpdateSlotUI(i);    // อัปเดตตัวเลขแสดงผล
                    SaveToGlobal();     // [เพิ่มใหม่] ซิงค์คลังข้ามซีน
                    return true;        // เก็บสำเร็จ
                }
            }
        }

        // ถ้าซ้อนไม่ได้ หรือยังไม่มีไอเทมชนิดนี้ในกระเป๋าเลย ให้หาช่องว่างใหม่
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].enabled == false)
            {
                itemSlots[i].sprite = itemSprite; 
                itemSlots[i].enabled = true;      
                slotItemTypes[i] = type; 
                slotItemCounts[i] = 1; // เริ่มนับชิ้นที่ 1
                UpdateSlotUI(i);       // อัปเดตตัวเลขแสดงผล
                SaveToGlobal();        // [เพิ่มใหม่] ซิงค์คลังข้ามซีน
                return true; 
            }
        }

        Debug.Log("กระเป๋าเต็มแล้ว!");
        return false; 
    }

    // ⚡ วางทับฟังก์ชัน UseItem เดิม เพื่อตรวจเช็คการคลิกเรียลไทม์
    public void UseItem(int slotIndex)
    {
        ItemType type = slotItemTypes[slotIndex];
        
        // ⚡ ข้อความแจ้งเตือนเมื่อกดคลิก
        Debug.Log($"🚨 [Inventory Click] คุณกดคลิกที่ช่อง Slot_{slotIndex + 1} | ไอเทมคือ: {type} | จำนวนในช่อง: {slotItemCounts[slotIndex]} ชิ้น");

        if (type == ItemType.None) 
        {
            Debug.Log("-> ช่องนี้เป็นช่องว่างเปล่า ไม่มีไอเทมให้ใช้งาน");
            return;
        }

        // ⚡ [เพิ่มใหม่] ค้นหา StressManager ของด่านปัจจุบันใหม่ในกรณีที่เพิ่งเปลี่ยนด่านเข้า/ออกแล้วลิงก์เดิมสูญหาย
        if (stressManager == null)
        {
            stressManager = Object.FindAnyObjectByType<StressManager>();
        }

        if (stressManager == null)
        {
            Debug.LogError("-> ❌ ไม่สามารถใช้ไอเทมได้เนื่องจากหา StressManager ไม่เจอ!");
            return;
        }

        switch (type)
        {
            case ItemType.Candy:
                stressManager.ChangeStress(-3f); 
                ConsumeItem(slotIndex);          
                break;

            case ItemType.StrawberryMilk:
                stressManager.ChangeStress(-5f); 
                ConsumeItem(slotIndex);          
                break;

            case ItemType.Headphone:
                if (!hasUsedHeadphoneToday)
                {
                    float reduction = stressManager.currentStress * 0.5f;
                    stressManager.ChangeStress(-reduction);
                    hasUsedHeadphoneToday = true;
                }
                else
                {
                    Debug.Log("🎧 วันนี้คุณใช้หูฟังไปแล้ว!");
                }
                break;

            case ItemType.Diary:
            case ItemType.RainDrawing:
            case ItemType.Armband:
                Debug.Log("-> ไอเทมประเภทนี้เป็น Passive ติดตัว ไม่ต้องกดใช้งาน");
                break;
        }

        if (diaryStressText != null && diaryStressText.gameObject.activeSelf)
        {
            diaryStressText.text = "Stress: " + Mathf.RoundToInt(stressManager.currentStress) + "%";
        }
    }

    // ⚡ [ฟังก์ชันใหม่] ใช้หักจำนวนไอเทมลงเมื่อกดใช้
    private void ConsumeItem(int index)
    {
        slotItemCounts[index]--; // ลดจำนวนลง 1
        
        // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อใช้งานไอเทมสำเร็จ
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnItemUsed();
        }

        if (slotItemCounts[index] <= 0)
        {
            RemoveItemAt(index); // ถ้าหมดเกลี้ยงให้ลบไอเทมออกจากช่องนั้น
        }
        else
        {
            UpdateSlotUI(index); // ถ้ายังเหลือให้อัปเดตตัวเลขใหม่
            SaveToGlobal();      // [เพิ่มใหม่] ซิงค์คลังข้ามซีน
        }
    }

    // ⚡ [ปรับปรุงใหม่] ฟังก์ชันอัปเดตตัวเลขจำนวนไอเทมบน UI ของช่องเก็บของ
    private void UpdateSlotUI(int index)
    {
        if (slotCountTexts[index] != null)
        {
            // ถ้าไอเทมมีมากกว่า 1 ชิ้น ให้เปิดการแสดงผลตัวเลข
            if (slotItemCounts[index] > 1)
            {
                slotCountTexts[index].gameObject.SetActive(true);
                slotCountTexts[index].text = slotItemCounts[index].ToString();
            }
            else
            {
                // ถ้ามีแค่ 1 ชิ้น หรือไม่มีเลย ไม่ต้องขึ้นตัวเลขให้รกตา
                slotCountTexts[index].gameObject.SetActive(false);
            }
        }
    }

    private void RemoveItemAt(int index)
    {
        itemSlots[index].sprite = null;
        itemSlots[index].enabled = false;
        slotItemTypes[index] = ItemType.None;
        slotItemCounts[index] = 0;
        if (slotCountTexts[index] != null) slotCountTexts[index].gameObject.SetActive(false);
        
        SaveToGlobal(); // [เพิ่มใหม่] ซิงค์คลังข้ามซีน
    }

    // [เพิ่มใหม่] แปลงสไปรต์สำหรับโหลดไอเทมข้ามซีน
    private Sprite GetSpriteForItemType(ItemType type)
    {
        foreach (var mapping in itemSpriteDatabase)
        {
            if (mapping.type == type) return mapping.sprite;
        }
        return null;
    }

    // [เพิ่มใหม่] โหลดคลังเก็บของจาก GameManager กลางระดับโลก
    public void LoadFromGlobal()
    {
        if (GameManagerSetup.Instance == null) return;

        // เคลียร์ค่าสล็อตเดิมเพื่อความสะอาด
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].enabled = false;
            slotItemTypes[i] = ItemType.None;
            slotItemCounts[i] = 0;
            if (slotCountTexts[i] != null) slotCountTexts[i].gameObject.SetActive(false);
        }

        // โหลดข้อมูล
        foreach (var globalItem in GameManagerSetup.Instance.savedInventory)
        {
            int index = globalItem.slotIndex;
            if (index < 0 || index >= itemSlots.Length) continue;

            ItemType itemType;
            if (System.Enum.TryParse(globalItem.itemTypeName, out itemType))
            {
                slotItemTypes[index] = itemType;
                slotItemCounts[index] = globalItem.count;

                Sprite itemSprite = GetSpriteForItemType(itemType);
                if (itemSprite != null)
                {
                    itemSlots[index].sprite = itemSprite;
                    itemSlots[index].enabled = true;
                }
                UpdateSlotUI(index);
            }
        }
        Debug.Log("[Inventory] โหลดไอเทมข้ามซีนมาแสดงผลบนกระเป๋าเรียบร้อย");
    }

    // [เพิ่มใหม่] บันทึกคลังเก็บของปัจจุบันไปที่ GameManager กลางระดับโลก
    public void SaveToGlobal()
    {
        if (GameManagerSetup.Instance == null) return;

        GameManagerSetup.Instance.savedInventory.Clear();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (slotItemTypes[i] != ItemType.None && slotItemCounts[i] > 0)
            {
                GlobalInventoryItem globalItem = new GlobalInventoryItem();
                globalItem.slotIndex = i;
                globalItem.itemTypeName = slotItemTypes[i].ToString();
                globalItem.count = slotItemCounts[i];

                GameManagerSetup.Instance.savedInventory.Add(globalItem);
            }
        }
        Debug.Log("[Inventory] บันทึกของในกระเป๋าเข้าสู่ข้อมูลข้ามฉากเสร็จสิ้น");
    }

    public bool HasItem(ItemType type)
    {
        foreach (ItemType item in slotItemTypes)
        {
            if (item == type) return true;
        }
        return false;
    }

    public void ResetDailyItems()
    {
        hasUsedHeadphoneToday = false;
    }
}