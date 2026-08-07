using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    private bool isPlayerInRange = false; 

    [Header("ระบุประเภทไอเทมชิ้นนี้")]
    public ItemType itemType;

    [Header("ลากรูปภาพไอเทม (Sprite) จาก Krita มาใส่ช่องนี้")]
    public Sprite itemIcon; 

    [Header("รหัสเฉพาะของไอเทม (หากปล่อยว่างระบบจะอิงตามชื่อและตำแหน่งเกิดอัตโนมัติ)")]
    [SerializeField] private string uniqueItemID;

    void Start()
    {
        // หากไม่ได้ตั้งค่ารหัสไอเทมเอง ให้ระบบสร้างรหัสเฉพาะจากชื่อและตำแหน่งพิกัด X, Y, Z
        if (string.IsNullOrEmpty(uniqueItemID))
        {
            uniqueItemID = gameObject.name + "_" + transform.position.x.ToString("F2") + "_" + transform.position.y.ToString("F2") + "_" + transform.position.z.ToString("F2");
        }

        // ตรวจสอบจากสารบบกลางว่าไอเทมชิ้นนี้เคยถูกเก็บไปแล้วหรือยังในการเซฟรอบนี้
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.IsItemPickedUp(uniqueItemID))
        {
            Destroy(gameObject); // ถ้าเคยเก็บแล้ว ให้ลบทิ้งทันทีเมื่อเข้าฉากใหม่
        }
    }

    void Update()
    {
        if (isPlayerInRange && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (InventoryManager.Instance != null)
            {
                // ⚡ ส่งทั้งรูปภาพและประเภทไอเทมเข้าไปในกระเป๋า
                bool isPickedUp = InventoryManager.Instance.AddItem(itemIcon, itemType);
                
                if (isPickedUp)
                {
                    // ลงทะเบียนรหัสไอเทมนี้เข้าสารบบส่วนกลางเพื่อความคงทน
                    if (GameManagerSetup.Instance != null)
                    {
                        GameManagerSetup.Instance.RegisterPickedUpItem(uniqueItemID);
                    }

                    Debug.Log("🎉 ผู้เล่นเก็บไอเทม: " + itemType.ToString() + " เข้ากระเป๋าแล้ว! (ID: " + uniqueItemID + ")");
                    
                    // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial ก่อนทำลายวัตถุไอเทม
                    if (TutorialManager.Instance != null)
                    {
                        TutorialManager.Instance.OnItemPickedUp();
                    }

                    Destroy(gameObject); 
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("กด F เพื่อเก็บของ: " + itemType.ToString()); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}