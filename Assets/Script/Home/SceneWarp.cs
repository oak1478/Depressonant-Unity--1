using UnityEngine;
using UnityEngine.SceneManagement; // ต้องมีบรรทัดนี้เพื่อสั่งเปลี่ยนฉาก

public class SceneWarp : MonoBehaviour
{
    [Header("ใส่ชื่อฉากปลายทางที่ต้องการไป")]
    public string targetSceneName; 

    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าวัตถุที่เดินมาชนคือ Player ใช่หรือไม่
        if (other.CompareTag("Player"))
        {
            // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อเปิด/ข้ามประตูสำเร็จ
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnDoorOpened();
            }

            // สั่งโหลดฉากปลายทางทันที
            SceneManager.LoadScene(targetSceneName);
        }
    }
}