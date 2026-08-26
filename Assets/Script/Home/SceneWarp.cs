using UnityEngine;
using UnityEngine.SceneManagement; // ต้องมีบรรทัดนี้เพื่อสั่งเปลี่ยนฉาก

public class SceneWarp : MonoBehaviour
{
    [Header("ใส่ชื่อฉากปลายทางที่ต้องการไป")]
    public string targetSceneName; 

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงเปิดประตูมาใส่ (เช่น door_open)")]
    public AudioClip doorOpenSound;

    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าวัตถุที่เดินมาชนคือ Player ใช่หรือไม่
        if (other.CompareTag("Player"))
        {
            // เล่นเสียงเปิดประตูแบบข้ามฉาก (ดึงเสียงอัตโนมัติหากยังไม่ได้ลากใส่)
            AudioClip soundToPlay = GetDoorSound();
            if (soundToPlay != null)
            {
                BedroomInteractable.PlayPersistentSound(soundToPlay);
            }

            // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อเปิด/ข้ามประตูสำเร็จ
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnDoorOpened();
            }

            // สั่งโหลดฉากปลายทางทันที
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private AudioClip GetDoorSound()
    {
        if (doorOpenSound != null) return doorOpenSound;

        AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>();
        foreach (var c in clips)
        {
            if (c != null && (c.name.ToLower().Contains("door_open") || c.name.ToLower().Contains("door")))
            {
                doorOpenSound = c;
                return c;
            }
        }
        return null;
    }
}