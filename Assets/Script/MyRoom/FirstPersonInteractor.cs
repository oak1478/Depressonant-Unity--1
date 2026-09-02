using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3.0f;
    [SerializeField] private LayerMask interactableLayers = ~0; // เริ่มต้นให้ตรวจจับทุก Layer
    
    [Header("UI Reference (Optional)")]
    [Tooltip("ใส่ UI Text (TextMeshPro) ที่ต้องการให้แสดงข้อความแนะนำ เช่น 'กด F เพื่อ...' (หากปล่อยว่างไว้ระบบจะวาดอักษรกลางจอให้อัตโนมัติเวลาเล่น)")]
    [SerializeField] private TMPro.TextMeshProUGUI promptText;

    private Camera cam;

    private void Start()
    {
        // ค้นหากล้องหลักในตัวละคร
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("[FirstPersonInteractor] ไม่พบกล้องหลัก (Main Camera) บนตัวละครผู้เล่น!", this);
        }
    }

    private void Update()
    {
        if (cam == null) return;

        // ยิง Raycast จากจุดกึ่งกลางของหน้าจอกล้องออกไปตรงๆ
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // วาดเส้นแสงสีแดงในหน้าต่าง Scene ของ Unity เพื่อให้เห็นว่ากล้องเล็งไปที่ไหน
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

        BedroomInteractable currentInteractable = null;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayers))
        {
            // ⚡ ค้นหาทั้งบนวัตถุที่ชน และวัตถุพ่อแม่ (Parent) เพื่อรองรับ Prefab ที่มีชิ้นส่วนลูก เช่น เตียง, หน้าต่าง, กระจก
            currentInteractable = hit.collider.GetComponentInParent<BedroomInteractable>();
        }

        // จัดการเปิด/ปิด UI แสดงปุ่มกดโต้ตอบ
        UpdatePromptUI(currentInteractable);

        // ตรวจสอบการกดปุ่มโต้ตอบ (ปุ่ม F) ในระบบ New Input System
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (currentInteractable != null)
            {
                Debug.Log($"[FirstPersonInteractor] กำลังโต้ตอบกับ: {currentInteractable.name}");
                currentInteractable.Interact();
            }
            else
            {
                // โค้ดตรวจสอบเหตุผลว่าทำไมไม่ทำงานเวลาผู้เล่นกดปุ่ม F
                if (Physics.Raycast(ray, out hit, interactRange, interactableLayers))
                {
                    Debug.LogWarning($"[FirstPersonInteractor] เล็งไปที่วัตถุ '{hit.collider.name}' แต่ไม่พบสคริปต์ BedroomInteractable อยู่ในวัตถุหรือตัวแม่!", hit.collider.gameObject);
                }
                else
                {
                    Debug.Log($"[FirstPersonInteractor] กด F แต่ตัวละครอยู่ห่างจากวัตถุมากเกินไป หรือไม่ได้เล็งไปที่ตัวชนใดๆ เลย (ระยะสูงสุดคือ {interactRange} เมตร)");
                }
            }
        }
    }

    private void UpdatePromptUI(BedroomInteractable interactable)
    {
        if (promptText != null)
        {
            if (interactable != null)
            {
                promptText.text = $"กด [F] เพื่อ {interactable.promptMessage}";
                promptText.gameObject.SetActive(true);
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    // ฟังก์ชันวาดตัวอักษรบนหน้าจอชั่วคราวกลางจอด้านล่าง หากไม่ได้เชื่อมโยง UI Text (TextMeshPro) ใน Inspector
    private void OnGUI()
    {
        if (promptText != null || cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayers))
        {
            BedroomInteractable interactable = hit.collider.GetComponentInParent<BedroomInteractable>();
            if (interactable != null)
            {
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 20;
                style.normal.textColor = Color.white;
                
                // กำหนดกรอบพื้นหลังข้อความเพื่อให้มองเห็นง่ายขึ้น
                Texture2D background = new Texture2D(1, 1);
                background.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
                background.Apply();
                style.normal.background = background;

                GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 80, 300, 40), $"กด [F] เพื่อ {interactable.promptMessage}", style);
            }
        }
    }
}
