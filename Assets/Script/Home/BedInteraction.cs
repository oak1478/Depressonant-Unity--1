using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    private bool isPlayerClose = false;

    void Update()
    {
        // ถ้าผู้เล่นอยู่ใกล้เตียง แล้วกดปุ่ม E
        if (isPlayerClose && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerSleep();
        }
    }

    // ฟังก์ชันโต้ตอบจากระบบ Interact แบบใหม่ (ใช้ปุ่ม F และ Raycast)
    public void Sleep()
    {
        TriggerSleep();
    }

    void TriggerSleep()
    {
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