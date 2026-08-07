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
        DayManager dayManager = Object.FindAnyObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.GoToNextDay(); // สั่งข้ามวัน
            
            // ⚡ [เพิ่มใหม่] บันทึกเกมลงไฟล์เซฟโดยอัตโนมัติเมื่อข้ามวัน
            if (SaveSystem.Instance != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector3 playerPos = player != null ? player.transform.position : Vector3.zero;
                int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

                SaveSystem.Instance.SaveGameFromGlobal(playerPos, sceneIndex);
                Debug.Log("[Auto-Save] บันทึกเกมเมื่อเข้านอนเรียบร้อย!");
            }

            // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อเข้านอนจบวันที่ 1
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnSleptInBed();
            }

            // ตรงนี้ในอนาคต (Phase 2) เราจะเอาหน้าจอดับ Fade to black มาใส่เพิ่มได้ครับ
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