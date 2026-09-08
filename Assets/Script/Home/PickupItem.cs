using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    [Header("ระบุประเภทไอเทมชิ้นนี้")]
    public ItemType itemType;

    [Header("ลากรูปภาพไอเทม (Sprite) จาก Krita มาใส่ช่องนี้")]
    public Sprite itemIcon; 

    [Header("รหัสเฉพาะของไอเทม (หากปล่อยว่างระบบจะอิงตามชื่อและตำแหน่งเกิดอัตโนมัติ)")]
    [SerializeField] private string uniqueItemID;

    [Header("เสียงเวลาเก็บไอเทม (SFX)")]
    public AudioClip pickupSound;

    [Header("ระยะตรวจจับการเก็บไอเทม (เมตร)")]
    [Tooltip("ระยะห่างสูงสุดที่ผู้เล่นสามารถกดเก็บไอเทมได้")]
    public float interactionRadius = 2.2f;

    [Tooltip("ความต่างระดับความสูงสูงสุด (เมตร) เผื่อไอเทมวางบนโต๊ะหรือพื้นต่างระดับ")]
    public float maxVerticalDistance = 2.5f;

    private bool isPlayerInRange = false;
    private bool isTriggerInside = false;
    private Transform cachedPlayerTransform;

    private GUIStyle promptStyle;
    private Texture2D promptBgTex;

    public string GetUniqueID()
    {
        if (!string.IsNullOrEmpty(uniqueItemID))
        {
            return uniqueItemID;
        }
        return gameObject.name + "_" + transform.position.x.ToString("F2") + "_" + transform.position.y.ToString("F2") + "_" + transform.position.z.ToString("F2");
    }

    void Start()
    {
        // โหลดเสียงเก็บไอเทมอัตโนมัติหากยังไม่ได้ลากใส่ใน Inspector
        if (pickupSound == null)
        {
            pickupSound = Resources.Load<AudioClip>("Audio/pick up item");
            if (pickupSound == null)
            {
                pickupSound = Resources.Load<AudioClip>("pick up item");
            }
#if UNITY_EDITOR
            if (pickupSound == null)
            {
                pickupSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/pick up item.mp3");
            }
#endif
        }

        // กำหนดรหัสเฉพาะของไอเทมหากยังว่างอยู่
        if (string.IsNullOrEmpty(uniqueItemID))
        {
            uniqueItemID = GetUniqueID();
        }

        // ตรวจสอบจากสารบบกลางว่าไอเทมชิ้นนี้เคยถูกเก็บไปแล้วหรือยังในการเซฟรอบนี้
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.IsItemPickedUp(uniqueItemID))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        // ค้นหาตำแหน่งผู้เล่นหากยังไม่ได้จำไว้
        if (cachedPlayerTransform == null)
        {
            PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
            if (pm != null)
            {
                cachedPlayerTransform = pm.transform;
            }
            else
            {
                CharacterController cc = Object.FindAnyObjectByType<CharacterController>();
                if (cc != null)
                {
                    cachedPlayerTransform = cc.transform;
                }
                else
                {
                    GameObject pObj = GameObject.FindGameObjectWithTag("Player");
                    if (pObj != null) cachedPlayerTransform = pObj.transform;
                }
            }
        }

        // คำนวณระยะห่างระหว่างตัวละครกับไอเทม (ตรวจจับทั้งแนวราบ XZ และความสูง Y)
        if (cachedPlayerTransform != null)
        {
            Vector3 pPos = cachedPlayerTransform.position;
            Vector3 myPos = transform.position;

            float horizontalDist = Vector2.Distance(new Vector2(myPos.x, myPos.z), new Vector2(pPos.x, pPos.z));
            float verticalDist = Mathf.Abs(myPos.y - pPos.y);

            if (horizontalDist <= interactionRadius && verticalDist <= maxVerticalDistance)
            {
                isPlayerInRange = true;
            }
            else if (!isTriggerInside)
            {
                isPlayerInRange = false;
            }
        }

        // ตรวจสอบการกดปุ่มเก็บไอเทม (รองรับทั้งปุ่ม F และปุ่ม E)
        bool interactPressed = false;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            {
                interactPressed = true;
            }
        }

        if (isPlayerInRange && interactPressed)
        {
            // หากกำลังมีบทสนทนาอยู่ ให้งดการเก็บไอเทมชั่วคราว
            DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
            if (dm != null && dm.IsDialogueActive()) return;

            ExecutePickup();
        }
    }

    private void ExecutePickup()
    {
        InventoryManager inv = InventoryManager.Instance != null ? InventoryManager.Instance : Object.FindAnyObjectByType<InventoryManager>();
        if (inv != null)
        {
            bool isPickedUp = inv.AddItem(itemIcon, itemType);
            if (isPickedUp)
            {
                // เล่นเสียงเก็บไอเทม
                if (pickupSound != null)
                {
                    inv.PlaySFX(pickupSound);
                }

                // ลงทะเบียนรหัสไอเทมเข้าสารบบส่วนกลางเพื่อความคงทน
                string id = GetUniqueID();
                if (GameManagerSetup.Instance != null)
                {
                    GameManagerSetup.Instance.RegisterPickedUpItem(id);
                }

                Debug.Log("[PickupItem] ผู้เล่นเก็บไอเทม: " + itemType.ToString() + " เข้ากระเป๋าแล้ว! (ID: " + id + ")");

                // แจ้งระบบ Tutorial
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnItemPickedUp();
                }

                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponent<PlayerMovement>() != null)
        {
            isTriggerInside = true;
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponent<PlayerMovement>() != null)
        {
            isTriggerInside = false;
        }
    }

    private void OnGUI()
    {
        if (!isPlayerInRange) return;
        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.IsDialogueActive()) return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle();
            promptStyle.alignment = TextAnchor.MiddleCenter;
            promptStyle.fontSize = 18;
            promptStyle.fontStyle = FontStyle.Bold;
            promptStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);

            promptBgTex = new Texture2D(1, 1);
            promptBgTex.SetPixel(0, 0, new Color(0.08f, 0.12f, 0.18f, 0.88f));
            promptBgTex.Apply();
            promptStyle.normal.background = promptBgTex;
            promptStyle.padding = new RectOffset(16, 16, 8, 8);
        }

        string itemNameTh = GetItemThaiName(itemType);
        string message = ThaiTextAdjuster.Adjust($"กด [F] เพื่อเก็บ {itemNameTh}");

        float width = 300f;
        float height = 44f;
        float x = (Screen.width - width) / 2f;
        float y = Screen.height * 0.82f;

        GUI.Label(new Rect(x, y, width, height), message, promptStyle);
    }

    private string GetItemThaiName(ItemType type)
    {
        switch (type)
        {
            case ItemType.Candy: return "ลูกอม";
            case ItemType.StrawberryMilk: return "นมสตรอว์เบอร์รี่";
            case ItemType.Headphone: return "หูฟัง";
            case ItemType.Armband: return "ปลอกแขน";
            case ItemType.Diary: return "ไดอารี่";
            case ItemType.RainDrawing: return "ภาพวาดร่มฝน";
            default: return "ไอเทม";
        }
    }
}
