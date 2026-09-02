using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BedroomInteractable : MonoBehaviour
{
    [Header("Prompt Message")]
    [Tooltip("ข้อความที่จะแสดงเมื่อผู้เล่นมองที่วัตถุนี้ เช่น 'นอนพักผ่อน', 'ส่องกระจก', 'ออกจากห้อง'")]
    public string promptMessage = "โต้ตอบ";

    [Header("Interaction Event")]
    [Tooltip("เหตุการณ์ที่จะเกิดขึ้นเมื่อผู้เล่นกดปุ่มโต้ตอบ (ปุ่ม F)")]
    public UnityEvent onInteract = new UnityEvent();

    [Header("Audio Settings (ระบบเสียงโต้ตอบ) [เพิ่มใหม่]")]
    [Tooltip("ลากไฟล์เสียงเวลาโต้ตอบ/เปิดประตู/เตียง มาใส่ (เช่น door_open)")]
    public AudioClip interactSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && interactSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }
    }

    // ฟังก์ชันโต้ตอบ
    public void Interact()
    {
        // ⚡ เล่นเสียงโต้ตอบแบบข้ามฉากได้ (ป้องกันเสียงดับทันทีตอนโหลดฉากใหม่)
        if (interactSound != null)
        {
            PlayPersistentSound(interactSound);
        }

        // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อโต้ตอบกับกระจกหรือประตู
        if (TutorialManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(promptMessage) && (promptMessage.Contains("กระจก") || promptMessage.Contains("Mirror")))
            {
                TutorialManager.Instance.OnLookedInMirror();
            }
            else
            {
                TutorialManager.Instance.OnDoorOpened();
            }
        }

        onInteract?.Invoke();
    }

    // ฟังก์ชันเสริมสำหรับใช้โหลดฉาก (สามารถลากตัวสคริปต์นี้ใส่ Event ใน Inspector แล้วเลือกฟังก์ชันนี้ได้เลย)
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[BedroomInteractable] กำลังเปลี่ยนไปยังฉาก: {sceneName}");

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnDoorOpened();
        }

        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันย้ายฉากและกำหนดจุดเกิด (ลากใส่ Event แล้วเลือกฟังก์ชันนี้และกรอก: ชื่อฉาก, จุดเกิด)
    public void WarpToScene(string sceneAndSpawn)
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnDoorOpened();
        }

        string[] parts = sceneAndSpawn.Split(',');
        if (parts.Length == 2)
        {
            string sceneName = parts[0].Trim();
            string spawnPointName = parts[1].Trim();
            SceneTransitionManager.LoadSceneWithSpawn(sceneName, spawnPointName);
        }
        else
        {
            Debug.LogError("[BedroomInteractable] รูปแบบข้อความไม่ถูกต้อง! กรุณาใช้รูปแบบ: 'ชื่อฉาก, ชื่อจุดเกิด' เช่น 'Home, Interior_Furniture_Misc_Door (7)'");
        }
    }

    // ⚡ [ระบบเสียงข้ามฉาก] เล่นเสียงโดยไม่โดนตัดทิ้งเมื่อเปลี่ยนฉาก (DontDestroyOnLoad)
    public static void PlayPersistentSound(AudioClip clip)
    {
        if (clip == null) return;
        GameObject audioGo = new GameObject("PersistentSFX_" + clip.name);
        AudioSource source = audioGo.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.volume = 1f;
        source.Play();
        DontDestroyOnLoad(audioGo);
        Destroy(audioGo, clip.length + 0.2f);
    }
}
