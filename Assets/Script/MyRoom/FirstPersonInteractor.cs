using UnityEngine;
using UnityEngine.InputSystem;

public enum CrosshairStyle
{
    Dot,        // จุดวงกลมมินิมอล
    Cross,      // กากบาท FPS คลาสสิก
    CircleDot   // จุดพร้อมวงแหวนรอบนอก
}

public class FirstPersonInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3.0f;
    [SerializeField] private LayerMask interactableLayers = ~0; // เริ่มต้นให้ตรวจจับทุก Layer
    
    [Header("Crosshair Settings (เป้ากลางจอ)")]
    [Tooltip("เปิด/ปิดการแสดงเป้ากลางจอ")]
    [SerializeField] private bool showCrosshair = true;
    [Tooltip("รูปแบบของเป้าเล็ง")]
    [SerializeField] private CrosshairStyle crosshairStyle = CrosshairStyle.Dot;
    [Tooltip("ขนาดของเป้ากลางจอ")]
    [SerializeField] private float crosshairSize = 8f;
    [Tooltip("สีปกติของเป้า")]
    [SerializeField] private Color defaultCrosshairColor = new Color(1f, 1f, 1f, 0.85f);
    [Tooltip("สีของเป้าเมื่อเล็งวัตถุที่สามารถกดโต้ตอบได้")]
    [SerializeField] private Color interactCrosshairColor = new Color(1f, 0.88f, 0.3f, 1f);
    [Tooltip("รูปภาพเป้าแบบกำหนดเอง (ถ้ามี สามารถลากใส่ได้ หากไม่ใส่ระบบจะวาดให้อัตโนมัติ)")]
    [SerializeField] private Texture2D customCrosshairTexture;

    [Header("Interaction Icon Settings (ไอคอนเมื่อเล็งวัตถุที่กดได้)")]
    [Tooltip("เปลี่ยนเป้าเป็นรูปมือเมื่อเล็งวัตถุที่สามารถกดโต้ตอบได้")]
    [SerializeField] private bool useHandOnInteract = true;
    [Tooltip("รูปภาพมือสำหรับโต้ตอบ (หากปล่อยว่าง ระบบจะโหลดไอคอน Hand_Interact อัตโนมัติ)")]
    [SerializeField] private Texture2D interactHandTexture;
    [Tooltip("ขนาดของไอคอนมือ")]
    [SerializeField] private float interactHandSize = 36f;
    [Tooltip("สีของไอคอนมือ")]
    [SerializeField] private Color handIconColor = Color.white;

    [Header("UI Reference (Optional)")]
    [Tooltip("ใส่ UI Text (TextMeshPro) ที่ต้องการให้แสดงข้อความแนะนำ เช่น 'กด F เพื่อ...' (หากปล่อยว่างไว้ระบบจะวาดอักษรกลางจอให้อัตโนมัติเวลาเล่น)")]
    [SerializeField] private TMPro.TextMeshProUGUI promptText;

    private Camera cam;
    private BedroomInteractable currentInteractable;

    private Texture2D cachedHandTex;
    private Texture2D whitePixelTex;
    private Texture2D circleTex;
    private Texture2D circleOutlineTex;
    private Texture2D promptBgTex;
    private GUIStyle promptStyle;

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

        // หากมีเมนูหรือหน้าต่างเซฟเปิดอยู่ ให้ซ่อนข้อความโต้ตอบทั้งหมดทันที
        if (IsAnyMenuOpen())
        {
            currentInteractable = null;
            UpdatePromptUI(null);
            return;
        }

        // ยิง Raycast จากจุดกึ่งกลางของหน้าจอกล้องออกไปตรงๆ
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // วาดเส้นแสงสีแดงในหน้าต่าง Scene ของ Unity เพื่อให้เห็นว่ากล้องเล็งไปที่ไหน
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

        currentInteractable = null;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayers))
        {
            // ค้นหาทั้งบนวัตถุที่ชน และวัตถุพ่อแม่ (Parent) เพื่อรองรับ Prefab ที่มีชิ้นส่วนลูก เช่น เตียง, หน้าต่าง, กระจก
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

    private void OnDisable()
    {
        currentInteractable = null;
        UpdatePromptUI(null);
    }

    private bool IsAnyMenuOpen()
    {
        // หากเคอร์เซอร์เมาส์ถูกปลดล็อกให้มองเห็น แปลว่ากำลังเปิดเมนูใดๆ อยู่
        if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked) return true;

        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return true;

        if (SleepSaveMenuController.Instance != null && (SleepSaveMenuController.Instance.IsMenuOpen || SleepSaveMenuController.Instance.IsTransitioning)) return true;

        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.IsDialogueActive()) return true;

        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        if (inv != null && inv.inventoryPanel != null && inv.inventoryPanel.activeSelf) return true;

        PauseMenuController pm = Object.FindAnyObjectByType<PauseMenuController>();
        if (pm != null && pm.pauseMenuPanel != null && pm.pauseMenuPanel.activeSelf) return true;

        return false;
    }

    private void UpdatePromptUI(BedroomInteractable interactable)
    {
        if (promptText != null)
        {
            if (interactable != null)
            {
                promptText.text = ThaiTextAdjuster.Adjust($"กด [F] เพื่อ {interactable.promptMessage}");
                promptText.gameObject.SetActive(true);
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void OnGUI()
    {
        if (!enabled || cam == null || IsAnyMenuOpen()) return;

        bool isHovering = (currentInteractable != null);

        // 1. วาดเป้ากลางจอ FPS
        DrawCrosshair(isHovering);

        // 2. วาดข้อความโต้ตอบบนหน้าจอชั่วคราว หากไม่ได้เชื่อมโยง UI Text (TextMeshPro) ใน Inspector
        if (promptText == null && isHovering)
        {
            EnsureTextures();

            if (promptStyle == null)
            {
                promptStyle = new GUIStyle();
                promptStyle.alignment = TextAnchor.MiddleCenter;
                promptStyle.fontSize = 20;
                promptStyle.normal.textColor = Color.white;
                promptStyle.normal.background = promptBgTex;
            }

            GUI.Label(new Rect(Screen.width / 2f - 150f, Screen.height / 2f + 80f, 300f, 40f), $"กด [F] เพื่อ {currentInteractable.promptMessage}", promptStyle);
        }
    }

    private void DrawCrosshair(bool isHoveringInteractable)
    {
        if (!showCrosshair) return;

        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        // เมื่อเล็งวัตถุที่สามารถกดโต้ตอบได้ และเปิดใช้งานไอคอนรูปมือ
        if (isHoveringInteractable && useHandOnInteract)
        {
            EnsureTextures();
            Texture2D handTex = interactHandTexture != null ? interactHandTexture : cachedHandTex;
            if (handTex != null)
            {
                float size = interactHandSize;
                GUI.color = handIconColor;
                GUI.DrawTexture(new Rect(centerX - size / 2f, centerY - size / 2f, size, size), handTex);
                GUI.color = Color.white;
                return;
            }
        }

        Color colorToUse = isHoveringInteractable ? interactCrosshairColor : defaultCrosshairColor;

        if (customCrosshairTexture != null)
        {
            float size = crosshairSize * (isHoveringInteractable ? 1.25f : 1f);
            GUI.color = colorToUse;
            GUI.DrawTexture(new Rect(centerX - size / 2f, centerY - size / 2f, size, size), customCrosshairTexture);
            GUI.color = Color.white;
            return;
        }

        switch (crosshairStyle)
        {
            case CrosshairStyle.Dot:
                DrawDot(centerX, centerY, isHoveringInteractable, colorToUse);
                break;
            case CrosshairStyle.Cross:
                DrawCross(centerX, centerY, isHoveringInteractable, colorToUse);
                break;
            case CrosshairStyle.CircleDot:
                DrawCircleDot(centerX, centerY, isHoveringInteractable, colorToUse);
                break;
        }
    }

    private void DrawDot(float cx, float cy, bool isHover, Color color)
    {
        float radius = isHover ? (crosshairSize * 0.65f) : (crosshairSize * 0.45f);
        if (radius < 2f) radius = 2f;

        // วาดเงาดำรอบนอกเพื่อให้มองเห็นชัดเจนในทุกสภาพแสง
        DrawSolidCircle(cx, cy, radius + 1f, new Color(0f, 0f, 0f, 0.6f));
        // วาดจุดแกนกลาง
        DrawSolidCircle(cx, cy, radius, color);
    }

    private void DrawCross(float cx, float cy, bool isHover, Color color)
    {
        float length = isHover ? (crosshairSize * 1.3f) : crosshairSize;
        float thickness = 2f;
        float gap = isHover ? (crosshairSize * 0.5f) : (crosshairSize * 0.35f);

        // วาดเงาดำด้านหลัง
        Color shadow = new Color(0f, 0f, 0f, 0.6f);
        DrawFilledRect(new Rect(cx - length - gap - 1, cy - thickness / 2f - 1, length + 2, thickness + 2), shadow);
        DrawFilledRect(new Rect(cx + gap - 1, cy - thickness / 2f - 1, length + 2, thickness + 2), shadow);
        DrawFilledRect(new Rect(cx - thickness / 2f - 1, cy - length - gap - 1, thickness + 2, length + 2), shadow);
        DrawFilledRect(new Rect(cx - thickness / 2f - 1, cy + gap - 1, thickness + 2, length + 2), shadow);

        // วาดเส้น 4 แฉก
        DrawFilledRect(new Rect(cx - length - gap, cy - thickness / 2f, length, thickness), color);
        DrawFilledRect(new Rect(cx + gap, cy - thickness / 2f, length, thickness), color);
        DrawFilledRect(new Rect(cx - thickness / 2f, cy - length - gap, thickness, length), color);
        DrawFilledRect(new Rect(cx - thickness / 2f, cy + gap, thickness, length), color);

        // จุดเล็กตรงกลาง
        DrawSolidCircle(cx, cy, 1.5f, color);
    }

    private void DrawCircleDot(float cx, float cy, bool isHover, Color color)
    {
        float dotRadius = isHover ? 3f : 2f;
        float ringRadius = isHover ? (crosshairSize * 1.25f) : crosshairSize;

        // วาดจุดกึ่งกลางพร้อมเงา
        DrawSolidCircle(cx, cy, dotRadius + 1f, new Color(0f, 0f, 0f, 0.6f));
        DrawSolidCircle(cx, cy, dotRadius, color);

        // วาดวงแหวนรอบนอก
        DrawRing(cx, cy, ringRadius, color);
    }

    private void DrawRing(float cx, float cy, float radius, Color color)
    {
        EnsureTextures();
        // วาดเงาวงแหวนด้านนอก
        GUI.color = new Color(0f, 0f, 0f, 0.4f);
        GUI.DrawTexture(new Rect(cx - radius - 1, cy - radius - 1, (radius + 1) * 2f, (radius + 1) * 2f), circleOutlineTex);
        // วาดวงแหวนสีจริง
        GUI.color = color;
        GUI.DrawTexture(new Rect(cx - radius, cy - radius, radius * 2f, radius * 2f), circleOutlineTex);
        GUI.color = Color.white;
    }

    private void DrawSolidCircle(float cx, float cy, float radius, Color color)
    {
        EnsureTextures();
        GUI.color = color;
        GUI.DrawTexture(new Rect(cx - radius, cy - radius, radius * 2f, radius * 2f), circleTex);
        GUI.color = Color.white;
    }

    private void DrawFilledRect(Rect rect, Color color)
    {
        EnsureTextures();
        GUI.color = color;
        GUI.DrawTexture(rect, whitePixelTex);
        GUI.color = Color.white;
    }

    private void EnsureTextures()
    {
        if (whitePixelTex == null)
        {
            whitePixelTex = new Texture2D(1, 1);
            whitePixelTex.SetPixel(0, 0, Color.white);
            whitePixelTex.Apply();
        }

        if (circleTex == null)
        {
            int res = 64;
            circleTex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            circleTex.filterMode = FilterMode.Bilinear;
            float center = (res - 1) / 2f;
            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist <= center - 1f)
                    {
                        circleTex.SetPixel(x, y, Color.white);
                    }
                    else if (dist <= center)
                    {
                        float a = Mathf.Clamp01(center - dist);
                        circleTex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                    else
                    {
                        circleTex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            circleTex.Apply();
        }

        if (circleOutlineTex == null)
        {
            int res = 64;
            circleOutlineTex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            circleOutlineTex.filterMode = FilterMode.Bilinear;
            float center = (res - 1) / 2f;
            float outerR = center;
            float innerR = center - 3f;
            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist >= innerR && dist <= outerR)
                    {
                        float edgeA = 1f;
                        if (dist < innerR + 1f) edgeA = Mathf.Clamp01(dist - innerR);
                        else if (dist > outerR - 1f) edgeA = Mathf.Clamp01(outerR - dist);
                        circleOutlineTex.SetPixel(x, y, new Color(1f, 1f, 1f, edgeA));
                    }
                    else
                    {
                        circleOutlineTex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            circleOutlineTex.Apply();
        }

        if (promptBgTex == null)
        {
            promptBgTex = new Texture2D(1, 1);
            promptBgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
            promptBgTex.Apply();
        }

        if (interactHandTexture == null && cachedHandTex == null)
        {
            cachedHandTex = Resources.Load<Texture2D>("Hand_Interact");
        }
    }
}
