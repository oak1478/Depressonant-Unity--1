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

    [Header("UI สำหรับเปิดหน้าไดอารี่จริง (Dynamic UI) [เพิ่มใหม่]")]
    public GameObject diaryPanel;
    public Image diaryBgImage;
    public TextMeshProUGUI diaryStoryText;
    public Color diaryLowStressColor = new Color(0.85f, 1f, 0.85f); // เขียวอ่อนสบายตา
    public Color diaryMedStressColor = new Color(1f, 0.95f, 0.75f); // เหลืองกังวล
    public Color diaryHighStressColor = new Color(1f, 0.75f, 0.75f); // แดงเครียดสูง
    public TMP_FontAsset diaryNormalFont;
    public TMP_FontAsset diaryShakyFont; 

    [Header("รูปภาพปกไดอารี่ตามระดับความเครียด (Dynamic Covers) [เพิ่มใหม่]")]
    [Tooltip("ลากรูปปกไดอารี่ตอนความเครียดต่ำ (< 30%) มาใส่")]
    public Sprite diaryCoverLowStress;
    [Tooltip("ลากรูปปกไดอารี่ตอนความเครียดปานกลาง (30% - 70%) มาใส่")]
    public Sprite diaryCoverMedStress;
    [Tooltip("ลากรูปปกไดอารี่ตอนความเครียดสูง (> 70%) มาใส่")]
    public Sprite diaryCoverHighStress; 

    [Header("ระบบเสียง SFX ประจำกระเป๋าและไอเทม (Audio SFX) [เพิ่มใหม่]")]
    [Tooltip("เสียงรูดซิปเปิดกระเป๋า (เช่น zipper_up)")]
    public AudioClip bagOpenSound;
    [Tooltip("เสียงรูดซิปปิดกระเป๋า (เช่น zipper_down)")]
    public AudioClip bagCloseSound;
    [Tooltip("เสียงเคี้ยวลูกอม (เช่น 392883...hard-candy-bone-crunch)")]
    public AudioClip candyEatSound;
    [Tooltip("เสียงดื่มนมสตรอเบอร์รี่ (เช่น 56271...water_gulp)")]
    public AudioClip milkDrinkSound;
    [Tooltip("เสียงเปิดอ่านไดอารี่ / พลิกหน้ากระดาษ (เช่น page_turn)")]
    public AudioClip diaryOpenSound;
    [Tooltip("เสียงปิดหน้าต่างไดอารี่ (เช่น book_close)")]
    public AudioClip diaryCloseSound;

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
    private AudioSource audioSource;

    [HideInInspector] public bool hasUsedHeadphoneToday = false;
    private bool isDiaryOpenOnGUI = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // ⚡ ป้องกันกระเป๋าเป้ซ้ำซ้อน: ถ้ามี InventoryManager ตัวหลักอยู่แล้ว ให้ทำลายตัวที่เกิดซ้ำทิ้งทันที!
            Debug.LogWarning($"⚠️ [InventoryManager] ตรวจพบกระเป๋าเป้ซ้ำซ้อนในฉาก ({gameObject.name})! ระบบทำการลบตัวซ้ำทิ้งอัตโนมัติเพื่อป้องกัน UI ซ้อนกัน");
            Destroy(gameObject);
            return;
        }
    }

    // ⚡ วางทับฟังก์ชัน Start เดิม เพื่อเพิ่มระบบเช็คปุ่มแบบละเอียด
    void Start()
    {
        inventoryPanel.SetActive(false);
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        stressManager = FindAnyObjectByType<StressManager>();

        // ตั้งค่าระบบเสียง SFX ประจำกระเป๋า
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // 2D UI sound
        audioSource.playOnAwake = false;

        // ⚡ [เพิ่มใหม่] โหลดเสียงและรูปภาพไอเทมทั้งหมดอัตโนมัติหากยังไม่ได้ลากใส่ใน Inspector
        AutoLoadSFXAndSpritesIfMissing();

        // ⚡ [เพิ่มใหม่] สแกนหาไอเทมบนพื้นทั้งหมดในฉาก เพื่อดึงรูปภาพที่คุณลากใส่ไว้ใน PickupItem มาจดจำอัตโนมัติ!
        PickupItem[] groundItems = Object.FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        foreach (var groundItem in groundItems)
        {
            if (groundItem != null && groundItem.itemIcon != null && groundItem.itemType != ItemType.None)
            {
                RegisterSpriteIfMissing(groundItem.itemType, groundItem.itemIcon);
            }
        }

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
        if (Keyboard.current != null)
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
                if (dm == null || !dm.IsDialogueActive())
                {
                    ToggleInventory();
                }
            }

            // ⚡ [ระบบปุ่มคีย์ลัดสำหรับทดสอบ Playtest]
            // กด F1 - F6 เพื่อเสกไอเทมแต่ละชิ้นเข้ากระเป๋าเป้ทันที
            if (Keyboard.current.f1Key.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.Candy), ItemType.Candy);
                Debug.Log("🧪 [Test] เสกไอเทม: ลูกอม (Candy) เข้ากระเป๋าแล้ว!");
            }
            if (Keyboard.current.f2Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.StrawberryMilk), ItemType.StrawberryMilk);
                Debug.Log("🧪 [Test] เสกไอเทม: นมสตรอเบอร์รี่ (StrawberryMilk) เข้ากระเป๋าแล้ว!");
            }
            if (Keyboard.current.f3Key.wasPressedThisFrame || Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.Diary), ItemType.Diary);
                Debug.Log("🧪 [Test] เสกไอเทม: ไดอารี่ (Diary) เข้ากระเป๋าแล้ว!");
            }
            if (Keyboard.current.f4Key.wasPressedThisFrame || Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.Headphone), ItemType.Headphone);
                Debug.Log("🧪 [Test] เสกไอเทม: หูฟัง (Headphone) เข้ากระเป๋าแล้ว!");
            }
            if (Keyboard.current.f5Key.wasPressedThisFrame || Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.Armband), ItemType.Armband);
                Debug.Log("🧪 [Test] เสกไอเทม: ปลอกแขน (Armband) เข้ากระเป๋าแล้ว!");
            }
            if (Keyboard.current.f6Key.wasPressedThisFrame || Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                AddItem(GetSpriteForItemType(ItemType.RainDrawing), ItemType.RainDrawing);
                Debug.Log("🧪 [Test] เสกไอเทม: ภาพวาดของเรน (RainDrawing) เข้ากระเป๋าแล้ว!");
            }

            // กด F7 เพื่อปรับวันเป็นวันที่ 8 (ใช้ทดสอบหูฟัง)
            if (Keyboard.current.f7Key.wasPressedThisFrame || Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                if (DayManager.Instance != null) DayManager.Instance.currentDay = 8;
                if (GameManagerSetup.Instance != null) GameManagerSetup.Instance.currentDay = 8;
                Debug.Log("🧪 [Test] วาร์ปข้ามไป 'วันที่ 8' เรียบร้อย! ตอนนี้สามารถทดสอบกดใช้หูฟังได้แล้ว");
            }

            // กด F8 / F9 เพื่อปรับค่าความเครียดเพิ่ม/ลด 25% (ใช้ทดสอบการเปลี่ยนสีหน้าไดอารี่ Dynamic UI)
            if (Keyboard.current.f8Key.wasPressedThisFrame || Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                if (stressManager != null) stressManager.ChangeStress(25f);
                Debug.Log("🧪 [Test] เพิ่มความเครียด +25% เพื่อทดสอบหน้าไดอารี่");
            }
            if (Keyboard.current.f9Key.wasPressedThisFrame || Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                if (stressManager != null) stressManager.ChangeStress(-25f);
                Debug.Log("🧪 [Test] ลดความเครียด -25% เพื่อทดสอบหน้าไดอารี่");
            }
        }

        // ⚡ [เพิ่มใหม่] อัปเดตรูปปกไดอารี่ในกระเป๋าเป้ตามค่าความเครียดแบบเรียลไทม์ (Dynamic Cover)
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].enabled && slotItemTypes[i] == ItemType.Diary)
            {
                Sprite dynamicCover = GetDynamicDiarySprite();
                if (dynamicCover != null) itemSlots[i].sprite = dynamicCover;
            }
        }
    }

    // ⚡ [ฟังก์ชันคลิกขวาใน Inspector] เสกไอเทมครบทุกชิ้นเข้ากระเป๋าทันที
    [ContextMenu("Give All Test Items")]
    public void GiveAllTestItems()
    {
        AddItem(GetSpriteForItemType(ItemType.Diary), ItemType.Diary);
        AddItem(GetSpriteForItemType(ItemType.Headphone), ItemType.Headphone);
        AddItem(GetSpriteForItemType(ItemType.Candy), ItemType.Candy);
        AddItem(GetSpriteForItemType(ItemType.StrawberryMilk), ItemType.StrawberryMilk);
        AddItem(GetSpriteForItemType(ItemType.Armband), ItemType.Armband);
        AddItem(GetSpriteForItemType(ItemType.RainDrawing), ItemType.RainDrawing);
        Debug.Log("🎉 เสกไอเทมทดสอบครบทั้ง 6 ชนิดเข้ากระเป๋าเป้เรียบร้อย!");
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);

            // ⚡ เล่นเสียงรูดซิปกระเป๋า
            if (isActive)
            {
                PlaySFX(bagOpenSound);
            }
            else
            {
                PlaySFX(bagCloseSound);
            }

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

    // ⚡ [เพิ่มใหม่] ฟังก์ชันสำหรับปุ่มกากบาท [X] บนหน้ากระเป๋าเป้ สั่งปิดกระเป๋าโดยตรง
    public void CloseInventory()
    {
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            ToggleInventory();
        }
    }

    // ⚡ [ปรับปรุงใหม่] ฟังก์ชันเพิ่มไอเทม รองรับระบบเก็บซ้อนกัน (Stacking)
    public bool AddItem(Sprite itemSprite, ItemType type)
    {
        // ถ้ามี Sprite ส่งเข้ามา ให้จดจำเข้าฐานข้อมูลทันที
        if (itemSprite != null)
        {
            RegisterSpriteIfMissing(type, itemSprite);
        }
        else
        {
            // ถ้าไม่มี Sprite ให้ดึงจากฐานข้อมูลที่เคยสแกนไว้
            itemSprite = GetSpriteForItemType(type);
        }

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
                PlaySFX(candyEatSound);
                stressManager.ChangeStress(-3f); 
                ConsumeItem(slotIndex);          
                break;

            case ItemType.StrawberryMilk:
                PlaySFX(milkDrinkSound);
                stressManager.ChangeStress(-5f); 
                ConsumeItem(slotIndex);          
                break;

            case ItemType.Headphone:
                int currentDay = 1;
                if (DayManager.Instance != null) currentDay = DayManager.Instance.currentDay;

                if (currentDay < 8)
                {
                    Debug.LogWarning("🎧 หูฟังนี้ยังใช้ไม่ได้! (สวมใส่และใช้งานได้ตั้งแต่วันที่ 8 เป็นต้นไป)");
                    break;
                }

                if (!hasUsedHeadphoneToday)
                {
                    stressManager.ChangeStress(-10f); // ลดค่าความเครียด 10% (10 หน่วย)
                    hasUsedHeadphoneToday = true;
                    Debug.Log("🎧 สวมหูฟัง: ลดความเครียดลง 10 หน่วยเรียบร้อย!");
                }
                else
                {
                    Debug.Log("🎧 วันนี้คุณใช้หูฟังไปแล้ว!");
                }
                break;

            case ItemType.Diary:
                PlaySFX(diaryOpenSound);
                if (diaryPanel != null)
                {
                    OpenDiaryUI();
                }
                else
                {
                    // ⚡ หากยังไม่ได้สร้าง UI ใน Canvas ระบบจะเปิดหน้าต่างไดอารี่ OnGUI ให้ทดสอบได้ทันที!
                    isDiaryOpenOnGUI = true;
                    Debug.Log("📖 ไดอารี่: เปิดหน้าต่างบันทึกไดอารี่จำลอง (Dynamic UI) ขึ้นมาบนหน้าจอเรียบร้อย!");
                }
                break;

            case ItemType.RainDrawing:
            case ItemType.Armband:
                Debug.Log("-> ไอเทมประเภทนี้ส่งผลแบบติดตัว (Passive) หรือทำงานตามเงื่อนไขเนื้อเรื่อง ไม่ต้องกดใช้งาน");
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

    // ⚡ [เพิ่มใหม่] ฟังก์ชันจดจำ Sprite ของไอเทมเข้าฐานข้อมูลอัตโนมัติ
    public void RegisterSpriteIfMissing(ItemType type, Sprite sprite)
    {
        if (sprite == null || type == ItemType.None) return;

        for (int i = 0; i < itemSpriteDatabase.Count; i++)
        {
            if (itemSpriteDatabase[i].type == type)
            {
                if (itemSpriteDatabase[i].sprite == null)
                {
                    var entry = itemSpriteDatabase[i];
                    entry.sprite = sprite;
                    itemSpriteDatabase[i] = entry;
                }
                return;
            }
        }

        ItemSpriteMapping mapping = new ItemSpriteMapping();
        mapping.type = type;
        mapping.sprite = sprite;
        itemSpriteDatabase.Add(mapping);
    }

    // ⚡ [เพิ่มใหม่] โหลดไฟล์เสียง SFX และ Sprite ไอเทมทั้งหมดอัตโนมัติหากยังว่างอยู่
    private void AutoLoadSFXAndSpritesIfMissing()
    {
        AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>();
        foreach (var c in clips)
        {
            if (c == null) continue;
            string lower = c.name.ToLower();
            if (bagOpenSound == null && lower.Contains("zipper_up")) bagOpenSound = c;
            if (bagCloseSound == null && lower.Contains("zipper_down")) bagCloseSound = c;
            if (candyEatSound == null && (lower.Contains("candy") || lower.Contains("crunch"))) candyEatSound = c;
            if (milkDrinkSound == null && (lower.Contains("gulp") || lower.Contains("drink"))) milkDrinkSound = c;
            if (diaryOpenSound == null && lower.Contains("page_turn")) diaryOpenSound = c;
            if (diaryCloseSound == null && lower.Contains("book_close")) diaryCloseSound = c;
        }

        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var s in allSprites)
        {
            if (s == null) continue;
            string lower = s.name.ToLower();
            if (lower.Contains("candy")) RegisterSpriteIfMissing(ItemType.Candy, s);
            if (lower.Contains("milk") || lower.Contains("strawberry")) RegisterSpriteIfMissing(ItemType.StrawberryMilk, s);
            if (lower.Contains("diary") || lower.Contains("book")) RegisterSpriteIfMissing(ItemType.Diary, s);
            if (lower.Contains("headphone")) RegisterSpriteIfMissing(ItemType.Headphone, s);
            if (lower.Contains("armband")) RegisterSpriteIfMissing(ItemType.Armband, s);
            if (lower.Contains("rain") || lower.Contains("drawing")) RegisterSpriteIfMissing(ItemType.RainDrawing, s);
        }
    }

    // ⚡ [เพิ่มใหม่] ฟังก์ชันเลือกรูปปกไดอารี่ตามระดับความเครียดสะสม (Dynamic Covers)
    public Sprite GetDynamicDiarySprite()
    {
        float stress = stressManager != null ? stressManager.currentStress : 0f;

        if (stress > 70f && diaryCoverHighStress != null)
        {
            return diaryCoverHighStress;
        }
        else if (stress > 30f && diaryCoverMedStress != null)
        {
            return diaryCoverMedStress;
        }
        else if (diaryCoverLowStress != null)
        {
            return diaryCoverLowStress;
        }

        // หากยังไม่ได้ใส่รูปปกแยกตามความเครียด ให้ค้นหารูป Diary ปกติจากฐานข้อมูล
        foreach (var mapping in itemSpriteDatabase)
        {
            if (mapping.type == ItemType.Diary && mapping.sprite != null) return mapping.sprite;
        }

        return CreateFallbackSprite(ItemType.Diary);
    }

    // [เพิ่มใหม่] แปลงสไปรต์สำหรับโหลดไอเทมข้ามซีน
    private Sprite GetSpriteForItemType(ItemType type)
    {
        foreach (var mapping in itemSpriteDatabase)
        {
            if (mapping.type == type && mapping.sprite != null) return mapping.sprite;
        }

        // ⚡ [เพิ่มใหม่] หากยังไม่ได้ลากรูปใส่ใน Inspector ให้สร้าง Sprite สีตามชนิดไอเทมชั่วคราวเพื่อใช้ทดสอบ (ไม่ให้ขึ้นเป็นกล่องขาวโล้นๆ)
        return CreateFallbackSprite(type);
    }

    private Sprite CreateFallbackSprite(ItemType type)
    {
        Color color = Color.white;
        switch (type)
        {
            case ItemType.Candy: color = new Color(1f, 0.4f, 0.7f); break; // ชมพูลูกอม
            case ItemType.StrawberryMilk: color = new Color(1f, 0.75f, 0.85f); break; // ชมพูพาสเทลนมสตรอเบอร์รี่
            case ItemType.Diary: color = new Color(0.65f, 0.4f, 0.2f); break; // น้ำตาลปกไดอารี่
            case ItemType.Headphone: color = new Color(0.2f, 0.6f, 1f); break; // ฟ้าหูฟัง
            case ItemType.Armband: color = new Color(1f, 0.85f, 0.2f); break; // เหลืองทองปลอกแขน
            case ItemType.RainDrawing: color = new Color(0.4f, 0.85f, 0.95f); break; // ฟ้าครามภาพวาดฝน
        }

        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
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

    // ⚡ [เพิ่มใหม่] ฟังก์ชันเปิดแสดงหน้าต่างไดอารี่ และเปลี่ยนสไตล์ตามความเครียดสะสม (Dynamic UI)
    public void OpenDiaryUI()
    {
        if (diaryPanel == null || stressManager == null) return;

        diaryPanel.SetActive(true);
        float stress = stressManager.currentStress;

        // ปิดการเดินตัวละครระหว่างอ่านไดอารี่
        if (playerMovement != null) playerMovement.enabled = false;

        // ⚡ ระบบ Dynamic UI ปรับเปลี่ยนสีพื้นหลังและสไตล์ฟอนต์ตามระดับความเครียด
        if (diaryBgImage != null)
        {
            Sprite dynamicCover = GetDynamicDiarySprite();
            if (dynamicCover != null) diaryBgImage.sprite = dynamicCover;
        }

        if (stress < 30f)
        {
            if (diaryBgImage != null) diaryBgImage.color = diaryLowStressColor;
            if (diaryStoryText != null)
            {
                if (diaryNormalFont != null) diaryStoryText.font = diaryNormalFont;
                diaryStoryText.fontStyle = FontStyles.Normal;
            }
        }
        else if (stress <= 70f)
        {
            if (diaryBgImage != null) diaryBgImage.color = diaryMedStressColor;
            if (diaryStoryText != null)
            {
                if (diaryNormalFont != null) diaryStoryText.font = diaryNormalFont;
                diaryStoryText.fontStyle = FontStyles.Italic;
            }
        }
        else
        {
            if (diaryBgImage != null) diaryBgImage.color = diaryHighStressColor;
            if (diaryStoryText != null)
            {
                if (diaryShakyFont != null) diaryStoryText.font = diaryShakyFont;
                diaryStoryText.fontStyle = FontStyles.Bold;
            }
        }

        // ⚡ อัปเดตเนื้อเรื่องสรุปรายวันตามวันปัจจุบัน
        int day = 1;
        if (DayManager.Instance != null) day = DayManager.Instance.currentDay;

        if (diaryStoryText != null)
        {
            diaryStoryText.text = GetDiarySummary(day, stress);
        }

        // ปลดล็อกเมาส์ให้สามารถลากมากดปุ่มปิดไดอารี่ได้
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ฟังก์ชันสำหรับผูกกับปุ่มกากบาท [X] ปิดหน้าต่างไดอารี่
    public void CloseDiaryUI()
    {
        // เล่นเสียงปิดสมุดไดอารี่
        PlaySFX(diaryCloseSound);

        isDiaryOpenOnGUI = false;

        if (diaryPanel != null)
        {
            diaryPanel.SetActive(false);
        }

        // ปล่อยคืนอิสระให้ระบบเดินตามปกติ (เว้นแต่ว่าหน้ากระเป๋ายังเปิดอยู่)
        if (inventoryPanel != null && !inventoryPanel.activeSelf)
        {
            if (playerMovement != null) playerMovement.enabled = true;
        }
    }

    // ⚡ ฟังก์ชันช่วยเล่นเสียง SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ฟังก์ชันสร้างข้อความบันทึกของเนียตามเงื่อนไขวันและความเครียด
    private string GetDiarySummary(int day, float stress)
    {
        string statusText = "";
        string diaryText = "";

        if (stress > 70f)
        {
            statusText = "มันอึดอัด... เหมือนกำลังจะหายใจไม่ออก...";
            diaryText = day == 1 ? "วันแรกของโรงเรียนทำไมมันน่ากลัวขนาดนี้ ทุกคนกำลังจ้องมองและวิจารณ์ฉันอยู่ใช่ไหม... ฉันอยากกลับบ้าน..." :
                        day == 8 ? "วันที่แปดแล้ว... ทำไมไม่มีอะไรดีขึ้นเลย หูฟังเสียงเพลงก็เกือบจะไม่ได้ช่วยปลอบประโลมใจฉันแล้ว..." :
                        "หัวสมองมันขาวโพลนไปหมด... ไม่อยากทำอะไร ไม่อยากเจอใครทั้งนั้น...";
        }
        else if (stress > 30f)
        {
            statusText = "รู้สึกกังวลอยู่ตลอดเวลา...";
            diaryText = day == 1 ? "เริ่มวันแรกด้วยความรู้สึกแปลกๆ ทุกคนที่นี่ดูแปลกหน้าไปหมด หวังว่าพรุ่งนี้จะปรับตัวได้นะ..." :
                        day == 8 ? "วันนี้ลองเอาหูฟังมาฟังเพลงดูบ้าง รู้สึกจิตใจสงบขึ้นมาหน่อยนึง แต่ก็ยังกลัวการไปสบตาคนอื่นอยู่ดี..." :
                        "วันนี้ผ่านไปได้แบบเหนื่อยๆ มีเรื่องให้กังวลอยู่ตลอดเวลาเลย...";
        }
        else
        {
            statusText = "วันนี้ค่อนข้างโอเค สงบสุขดี";
            diaryText = day == 1 ? "วันแรกเริ่มต้นได้ราบรื่นกว่าที่คิด ฉันพยายามตั้งสติและเผชิญหน้ากับมันอย่างค่อยเป็นค่อยไป..." :
                        day == 8 ? "วันที่แปดแล้ว ฉันเริ่มชินกับบรรยากาศการฟังเพลงเงียบๆ และจัดการความรู้สึกตัวเองได้ดีขึ้นมาก" :
                        "วันนี้ไม่มีเรื่องแย่ๆ เกิดขึ้นเลย รู้สึกปลอดภัยและอบอุ่นดี...";
        }

        return $"<b>บันทึกของเนีย - วันที่ {day}</b>\n" +
               $"<size=80%>สภาวะจิตใจ: {statusText} (Stress: {Mathf.RoundToInt(stress)}%)</size>\n\n" +
               $"{diaryText}";
    }

    // ⚡ [ระบบหน้าต่างจำลองอัตโนมัติ] เปิดหน้าต่างไดอารี่ Dynamic UI ให้ทดสอบได้ทันทีในหน้า Game View
    private void OnGUI()
    {
        if (!isDiaryOpenOnGUI) return;

        float stress = stressManager != null ? stressManager.currentStress : 0f;
        int day = 1;
        if (DayManager.Instance != null) day = DayManager.Instance.currentDay;
        else if (GameManagerSetup.Instance != null) day = GameManagerSetup.Instance.currentDay;

        // คำนวณขนาดและสีกรอบตามความเครียดสะสม (Dynamic UI)
        Color bgColor = diaryLowStressColor;
        if (stress > 70f) bgColor = diaryHighStressColor;
        else if (stress > 30f) bgColor = diaryMedStressColor;

        float winWidth = Mathf.Min(600, Screen.width * 0.85f);
        float winHeight = Mathf.Min(450, Screen.height * 0.85f);
        Rect winRect = new Rect((Screen.width - winWidth) / 2, (Screen.height - winHeight) / 2, winWidth, winHeight);

        // วาดพื้นหลังไดอารี่ตามสีความเครียด
        Color oldColor = GUI.color;
        GUI.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0.95f);
        GUI.DrawTexture(winRect, Texture2D.whiteTexture);
        GUI.color = oldColor;

        GUILayout.BeginArea(winRect);
        
        // หัวข้อ
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 22;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.black;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label($"📖 บันทึกของเนีย - วันที่ {day}", titleStyle);

        // สถานะความเครียด
        GUIStyle statusStyle = new GUIStyle(GUI.skin.label);
        statusStyle.fontSize = 15;
        statusStyle.fontStyle = FontStyle.Italic;
        statusStyle.normal.textColor = stress > 70f ? Color.red : (stress > 30f ? new Color(0.6f, 0.4f, 0f) : new Color(0f, 0.5f, 0f));
        statusStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label($"สภาวะจิตใจ: {(stress > 70f ? "ทรมานเหลือเกิน..." : (stress > 30f ? "รู้สึกกังวลอยู่ตลอดเวลา..." : "วันนี้ค่อนข้างโอเค สงบสุขดี"))} (Stress: {Mathf.RoundToInt(stress)}%)", statusStyle);

        GUILayout.Space(15);

        // เนื้อหาบันทึก
        GUIStyle storyStyle = new GUIStyle(GUI.skin.label);
        storyStyle.fontSize = 16;
        storyStyle.wordWrap = true;
        storyStyle.normal.textColor = Color.black;
        if (stress > 70f) storyStyle.fontStyle = FontStyle.Bold;
        else if (stress > 30f) storyStyle.fontStyle = FontStyle.Italic;
        else storyStyle.fontStyle = FontStyle.Normal;

        string summary = "";
        if (stress > 70f)
        {
            summary = day == 1 ? "วันแรกของโรงเรียนทำไมมันน่ากลัวขนาดนี้ ทุกคนกำลังจ้องมองและวิจารณ์ฉันอยู่ใช่ไหม... ฉันอยากกลับบ้าน..." :
                      day == 8 ? "วันที่แปดแล้ว... ทำไมไม่มีอะไรดีขึ้นเลย หูฟังเสียงเพลงก็เกือบจะไม่ได้ช่วยปลอบประโลมใจฉันแล้ว..." :
                      "หัวสมองมันขาวโพลนไปหมด... ไม่อยากทำอะไร ไม่อยากเจอใครทั้งนั้น...";
        }
        else if (stress > 30f)
        {
            summary = day == 1 ? "เริ่มวันแรกด้วยความรู้สึกแปลกๆ ทุกคนที่นี่ดูแปลกหน้าไปหมด หวังว่าพรุ่งนี้จะปรับตัวได้นะ..." :
                      day == 8 ? "วันนี้ลองเอาหูฟังมาฟังเพลงดูบ้าง รู้สึกจิตใจสงบขึ้นมาหน่อยนึง แต่ก็ยังกลัวการไปสบตาคนอื่นอยู่ดี..." :
                      "วันนี้ผ่านไปได้แบบเหนื่อยๆ มีเรื่องให้กังวลอยู่ตลอดเวลาเลย...";
        }
        else
        {
            summary = day == 1 ? "วันแรกเริ่มต้นได้ราบรื่นกว่าที่คิด ฉันพยายามตั้งสติและเผชิญหน้ากับมันอย่างค่อยเป็นค่อยไป..." :
                      day == 8 ? "วันที่แปดแล้ว ฉันเริ่มชินกับบรรยากาศการฟังเพลงเงียบๆ และจัดการความรู้สึกตัวเองได้ดีขึ้นมาก" :
                      "วันนี้ไม่มีเรื่องแย่ๆ เกิดขึ้นเลย รู้สึกปลอดภัยและอบอุ่นดี...";
        }

        GUILayout.Label(summary, storyStyle);

        GUILayout.FlexibleSpace();

        // ปุ่มปิด
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 16;
        btnStyle.fontStyle = FontStyle.Bold;
        if (GUILayout.Button("✖ ปิดไดอารี่", btnStyle, GUILayout.Height(40)))
        {
            PlaySFX(diaryCloseSound);
            isDiaryOpenOnGUI = false;
        }

        GUILayout.EndArea();
    }
}