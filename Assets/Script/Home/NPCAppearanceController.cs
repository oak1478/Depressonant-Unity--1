using UnityEngine;
using System.Collections.Generic;

public class NPCAppearanceController : MonoBehaviour
{
    [Header("วันที่ NPC ตัวนี้จะปรากฏตัว (เช่น 1, 2, 5, 14)")]
    public List<int> activeDays = new List<int>();

    void Start()
    {
        // ตอนเริ่มเกมให้เช็ค 1 ครั้งตามปกติ
        UpdateAppearance();
    }

    // ⚡ ฟังก์ชันใหม่: สั่งให้อัปเดตการซ่อน/โชว์ตัวได้ตลอดเวลา
    public void UpdateAppearance()
    {
        // ⚡ ในซีนฉากจบ ให้ NPC ทุกตัวที่วางไว้ในฉากแสดงตัวเสมอ
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(currentScene) && currentScene.IndexOf("Ending", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            gameObject.SetActive(true);
            return;
        }

        int today = 1;
        if (GameManagerSetup.Instance != null)
        {
            today = GameManagerSetup.Instance.currentDay;
        }
        else
        {
            DayManager dayManager = Object.FindAnyObjectByType<DayManager>();
            if (dayManager != null)
            {
                today = dayManager.currentDay;
            }
        }
        
        if (activeDays != null)
        {
            gameObject.SetActive(activeDays.Contains(today));
        }
    }
}