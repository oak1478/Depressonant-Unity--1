using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    public static BedInteraction Instance { get; private set; }

    private bool isPlayerClose = false;
    private string blockedWarningMessage = "";
    private float warningDisplayTimer = 0f;
    private GUIStyle warningStyle;
    private Texture2D warningBgTex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        // ถ้าผู้เล่นอยู่ใกล้เตียง แล้วกดปุ่ม E
        if (isPlayerClose && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerSleep();
        }
    }

    // ฟังก์ชันโต้ตอบจากระบบ Interact แบบใหม่ (ใช้ปุ่ม F และ Raycast)
    public void Sleep()
    {
        TriggerSleep();
    }

    public void ShowSleepBlockedWarning(string message)
    {
        blockedWarningMessage = ThaiTextAdjuster.Adjust(message);
        warningDisplayTimer = 3.5f;
    }

    private void OnGUI()
    {
        if (warningDisplayTimer > 0f)
        {
            warningDisplayTimer -= Time.deltaTime;

            if (warningStyle == null)
            {
                warningStyle = new GUIStyle();
                warningStyle.alignment = TextAnchor.MiddleCenter;
                warningStyle.fontSize = 20;
                warningStyle.fontStyle = FontStyle.Bold;
                warningStyle.normal.textColor = new Color(1f, 0.88f, 0.35f, 1f); // สีเหลืองอบอุ่น
                
                warningBgTex = new Texture2D(1, 1);
                warningBgTex.SetPixel(0, 0, new Color(0.08f, 0.10f, 0.15f, 0.90f)); // พื้นหลังเข้ม
                warningBgTex.Apply();
                warningStyle.normal.background = warningBgTex;
                warningStyle.padding = new RectOffset(20, 20, 10, 10);
            }

            float width = 560f;
            float height = 48f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height * 0.72f;

            GUI.Label(new Rect(x, y, width, height), blockedWarningMessage, warningStyle);
        }
    }

    void TriggerSleep()
    {
        // ตรวจสอบว่าทำเควสประจำวันครบแล้วหรือยัง หากยังคุยไม่ครบ ไม่อนุญาตให้นอน
        if (DailyQuestManager.Instance != null && !DailyQuestManager.Instance.IsDailyQuestCompleted())
        {
            Debug.LogWarning("[BedInteraction] ยังนอนไม่ได้! ต้องไปพูดคุยให้ครบตามเป้าหมายวันนี้ก่อน");
            ShowSleepBlockedWarning("ยังนอนไม่ได้นะ! ต้องไปพูดคุยให้ครบตามเป้าหมายวันนี้ก่อน");
            return;
        }

        // ตรวจสอบว่ามีหน้าต่างเลือกช่องเซฟ (SleepSaveMenuController) หรือไม่
        SleepSaveMenuController saveMenu = SleepSaveMenuController.Instance;
        if (saveMenu == null) saveMenu = Object.FindAnyObjectByType<SleepSaveMenuController>();

        // หากในฉากยังไม่มี SleepSaveCanvas ให้โหลดขึ้นมาใช้งานอัตโนมัติ
        if (saveMenu == null)
        {
            GameObject prefab = Resources.Load<GameObject>("SleepSaveCanvas");
#if UNITY_EDITOR
            if (prefab == null)
            {
                prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SleepSaveCanvas.prefab");
            }
#endif
            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab);
                instance.name = "SleepSaveCanvas";
                saveMenu = instance.GetComponent<SleepSaveMenuController>();
            }
        }

        if (saveMenu != null)
        {
            saveMenu.OpenMenu();
            return;
        }

        // กรณีฉุกเฉิน (Fallback): ถ้าไม่มีหน้าต่างเลือกช่องเซฟ ให้เซฟและข้ามวันตามปกติ
        DayManager dayManager = Object.FindAnyObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.GoToNextDay(); // สั่งข้ามวัน
            
            // บันทึกเกมลงไฟล์เซฟโดยอัตโนมัติเมื่อข้ามวัน
            if (SaveSystem.Instance != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) player = GameObject.Find("Player");
                if (player == null)
                {
                    CharacterController cc = Object.FindAnyObjectByType<CharacterController>();
                    if (cc != null) player = cc.gameObject;
                }

                Vector3 playerPos = player != null ? player.transform.position : new Vector3(-0.808f, 1f, -6.931f);
                int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

                SaveSystem.Instance.SaveGameFromGlobal(playerPos, sceneIndex);
                Debug.Log("[Auto-Save] บันทึกเกมเมื่อเข้านอนเรียบร้อย!");
            }

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnSleptInBed();
            }

            Debug.Log("[BedInteraction] Nia เข้านอนแล้ว... กำลังข้ามไปยังวันถัดไป");
        }
    }

    // ตรวจจับเมื่อ Player เดินมาเข้าใกล้เตียง
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null) 
        {
            isPlayerClose = true;
            Debug.Log("อยู่ใกล้เตียง: กด E เพื่อเข้านอนข้ามวัน");
        }
    }

    // ตรวจจับเมื่อ Player เดินห่างออกจากเตียง
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null) 
        {
            isPlayerClose = false;
        }
    }
}