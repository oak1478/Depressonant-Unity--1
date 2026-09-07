using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panel References")]
    [Tooltip("ลาก GameObject แผงเมนูหยุดเกม (Pause Menu Panel) มาใส่ตรงนี้")]
    public GameObject pauseMenuPanel;

    [Tooltip("ลาก GameObject แผงตั้งค่า (Settings Panel) ในเกมมาใส่ตรงนี้ (ถ้ามี)")]
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Awake()
    {
        // ⚡ [ดีบัก] แจ้งเตือนเมื่อสคริปต์เริ่มโหลดในซีน
        Debug.Log($"[PauseMenuController] Awake ทำงานบน GameObject: '{gameObject.name}' (Active: {gameObject.activeInHierarchy})", this);
    }

    void Start()
    {
        // ซ่อนเมนูทั้งหมดตอนเริ่มต้นฉาก
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Debug.Log("[PauseMenuController] พบบล็อกแผง PauseMenuPanel เชื่อมต่อสำเร็จ", this);
        }
        else
        {
            Debug.LogError("[PauseMenuController] ⚠️ คุณยังไม่ได้ลากแผง PauseMenuPanel มาใส่ใน Inspector ของสคริปต์นี้!", this);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("[PauseMenuController] พบบล็อกแผง SettingsPanel เชื่อมต่อสำเร็จ", this);
        }

        isPaused = false;
        Time.timeScale = 1f; // ปลดล็อกเวลาเริ่มต้น
    }

    void Update()
    {
        bool escPressed = false;

        // ⚡ [แก้ไข] ใช้เฉพาะระบบป้อนข้อมูลแบบใหม่ (New Input System) ตามการตั้งค่าโปรเจกต์ของคุณ เพื่อไม่ให้เกิด InvalidOperationException
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            // ตรวจจับปุ่ม ESC
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                escPressed = true;
                Debug.Log("[PauseMenuController] ตรวจพบการกดปุ่ม ESC (ผ่านระบบ New Input System)", this);
            }
            // ตรวจจับปุ่ม P (ปุ่มลัดสำหรับทดสอบใน Unity Editor)
            if (UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
            {
                escPressed = true;
                Debug.Log("[PauseMenuController] ตรวจพบการกดปุ่มสำรอง P (ผ่านระบบ New Input System)", this);
            }
        }

        if (escPressed)
        {
            // หากหน้าต่าง Command Prompt (DevConsole) เปิดอยู่ ให้ปิดหน้าต่าง Console แทนการเปิด Pause Menu
            if (DevConsole.Instance != null && DevConsole.Instance.IsOpen)
            {
                DevConsole.Instance.CloseConsole();
                return;
            }

            // หากกระเป๋าเป้เปิดอยู่ ให้ปิดกระเป๋าเป้ก่อน
            if (InventoryManager.Instance != null && InventoryManager.Instance.inventoryPanel != null && InventoryManager.Instance.inventoryPanel.activeSelf)
            {
                InventoryManager.Instance.CloseInventory();
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // หยุดเวลาและแสดงเมนู
    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            Debug.Log("[PauseMenuController] ทำการเปิดหน้าจอเมนูหยุดเกม (SetActive = true)", this);
        }
        else
        {
            Debug.LogError("[PauseMenuController] ไม่สามารถแสดงหน้าเมนูได้เนื่องจากตัวแปร pauseMenuPanel เป็น NULL ใน Inspector!", this);
        }

        Time.timeScale = 0f; // หยุดเวลาฟิสิกส์และการเคลื่อนไหวในเกม
        Debug.Log("[PauseMenuController] หยุดเวลาฟิสิกส์ของเกม (Time.timeScale = 0)", this);

        // แสดงเคอร์เซอร์เมาส์เพื่อกดปุ่มเมนู
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // รันเวลาต่อและปิดเมนู
    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f; // ปลดล็อกเวลาเป็นปกติ
        Debug.Log("[PauseMenuController] กลับมาเล่นเกมต่อ ปลดล็อกเวลาฟิสิกส์เป็นปกติ (Time.timeScale = 1)", this);

        // ตรวจสอบว่ามีบทสนทนากำลังเปิดอยู่หรือไม่
        bool isDialogueActive = false;
        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.IsDialogueActive())
        {
            isDialogueActive = true;
        }

        // ตรวจสอบว่าเป็นซีน 3D หรือไม่
        bool is3DScene = SceneManager.GetActiveScene().name == "Bedroom_3D" 
                      || Object.FindAnyObjectByType<FirstPersonController>(FindObjectsInactive.Include) != null;

        // จัดการเคอร์เซอร์เมาส์ตามโหมดซีนและสถานะบทสนทนา
        if (isDialogueActive)
        {
            // ถ้าอยู่ในบทสนทนา ปล่อยเมาส์ให้สามารถคลิกเลือกช้อยส์หรือข้อความได้
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("[PauseMenuController] Resume กลับมาขณะมีบทสนทนาค้างอยู่ เปิดแสดงเคอร์เซอร์เมาส์อิสระ", this);
        }
        else if (is3DScene)
        {
            // ซีน 3D: ล็อกเมาส์ไว้ตรงกลางหน้าจอตามเดิม
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("[PauseMenuController] ตรวจพบซีน 3D ทำการซ่อนและล็อกเคอร์เซอร์เมาส์กลางหน้าจอ", this);
        }
        else
        {
            // ซีน 2.5D: แสดงเคอร์เซอร์เพื่อให้คลิกโต้ตอบได้ปกติ
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("[PauseMenuController] ตรวจพบซีน 2.5D เปิดแสดงเคอร์เซอร์เมาส์อิสระ", this);
        }
    }

    // เปิดหน้าตั้งค่า
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    // ปิดหน้าตั้งค่า (กลับมาที่หน้าพักเมนูเดิม)
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    // ออกไปหน้าเมนูหลัก (Exit to Main Menu)
    public void ExitSave()
    {
        Time.timeScale = 1f; // ปรับความเร็วเวลากลับเป็นปกติเพื่อให้เปลี่ยนซีนสมบูรณ์

        // ปิดแผงหน้าจอเมนูหยุดเกมและเคลียร์สถานะพักเกม
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        isPaused = false;

        // ซ่อนหน้าต่างเควส
        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.SetVisible(false);
        }

        // ซ่อนหน้าต่างกระเป๋า (ถ้ามี)
        if (InventoryManager.Instance != null && InventoryManager.Instance.inventoryPanel != null)
        {
            InventoryManager.Instance.inventoryPanel.SetActive(false);
        }

        // รีเซ็ตสถานะเซสชันเกมเพื่อไม่ให้ข้อมูลเก่าค้างในหน่วยความจำ
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.ResetGameSession();
        }

        // โหลดฉากเมนูหลัก
        SceneManager.LoadScene("MainMenu");
    }

    // ออกจากเกม (Quit Game)
    public void ExitGame()
    {
        Debug.Log("ออกจากโปรแกรมเกม!");
        Application.Quit();
    }
}
