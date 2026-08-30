using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StressManager : MonoBehaviour
{
    [Header("Stress Settings")]
    [Range(0, 100)] public float currentStress = 0f;

    [Header("Visual References")]
    public Volume globalVolume;
    public Image stressVignetteUI;

    [Header("Stress Audio Settings (เสียงหัวใจเต้นตามความเครียด)")]
    [Tooltip("ลากไฟล์เสียงหัวใจเต้นปกติมาใส่ (เล่นเมื่อ Stress > 50%) เช่น 485076...heartbeat-regular")]
    public AudioClip heartbeatRegularSound;
    [Tooltip("ลากไฟล์เสียงหัวใจเต้นเร็วพร้อมเสียงหายใจหอบมาใส่ (เล่นเมื่อ Stress > 75%) เช่น 410390...heartbeatbreath")]
    public AudioClip heartbeatPanicSound;

    private Vignette vignette;
    private ChromaticAberration chromatic;
    private LensDistortion distortion;
    private AudioSource stressAudioSource;

    [Header("Flash Effect")]
    public Image flashVignetteUI;
    public Color damageFlashColor = new Color(1f, 0f, 0f, 0.5f); // สีแดงโปร่งแสงตอนเครียดเพิ่ม
    public Color healFlashColor = new Color(0f, 1f, 0.5f, 0.5f); // สีเขียวโปร่งแสงตอนเครียดลด
    public float flashDuration = 1f; // ระยะเวลาในการแฟลชและจางหาย
    
    private Coroutine flashCoroutine;

    // ⚡ [ระบบใหม่] ตัวแปรสำหรับคุม Armband (ปลอกแขน)
    [HideInInspector] public bool hasArmbandTriggeredToday = false;

    void Start()
    {
        // โหลดค่าความเครียดระดับโลกมาใช้งาน
        if (GameManagerSetup.Instance != null)
        {
            currentStress = GameManagerSetup.Instance.currentStress;
        }

        // ตั้งค่าระบบเสียงหัวใจเต้น
        stressAudioSource = GetComponent<AudioSource>();
        if (stressAudioSource == null)
        {
            stressAudioSource = gameObject.AddComponent<AudioSource>();
        }
        stressAudioSource.spatialBlend = 0f; // 2D sound สำหรับเสียงในร่างกาย
        stressAudioSource.loop = true;
        stressAudioSource.playOnAwake = false;

        // โค้ดนี้ต้องอยู่ฝั่งสคริปต์จัดการความเครียดที่มีการเรียกใช้ Volume ครับ
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet(out vignette)) { }
            if (globalVolume.profile.TryGet(out chromatic)) { }
            if (globalVolume.profile.TryGet(out distortion)) { }
        }
        else
        {
            Debug.LogWarning("อย่าลืมลากออบเจกต์ Volume มาใส่ในช่อง Global Volume บน StressManager นะครับ!");
        }

        if (stressVignetteUI != null)
        {
            Color c = stressVignetteUI.color;
            c.a = currentStress / 100f; 
            stressVignetteUI.color = c;
        }
    }

    void Update()
    {
        // ⚡ [แก้ไข] ดึงค่าความเครียดจากข้อมูลระดับโลก (GameManagerSetup) เป็นศูนย์กลางเสมอ
        if (GameManagerSetup.Instance != null)
        {
            currentStress = GameManagerSetup.Instance.currentStress;
        }

        // ⚡ [เพิ่มใหม่] ค้นหาและเชื่อมต่อกับ Volume ของฉากปัจจุบันอัตโนมัติ (เช่น หลังจากโหลดฉากสลับไปมา)
        if (globalVolume == null || vignette == null)
        {
            globalVolume = Object.FindAnyObjectByType<Volume>();
            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.TryGet(out vignette);
                globalVolume.profile.TryGet(out chromatic);
                globalVolume.profile.TryGet(out distortion);
                Debug.Log("[StressManager] ตรวจพบการย้ายฉาก: ทำการเชื่อมโยง URP Volume ของฉากนี้อัตโนมัติสำเร็จ!");
            }
        }

        // ค้นหา UI จอดำแดงของฉากปัจจุบันอัตโนมัติหากว่างเปล่า
        if (stressVignetteUI == null)
        {
            GameObject uiObj = GameObject.Find("StressVignette");
            if (uiObj != null)
            {
                stressVignetteUI = uiObj.GetComponent<Image>();
            }
        }

        float stressRatio = currentStress / 100f;

        if (vignette != null) vignette.intensity.value = stressRatio * 0.5f;
        if (chromatic != null) chromatic.intensity.value = stressRatio;
        if (distortion != null) distortion.intensity.value = -stressRatio * 0.5f;

        if (stressVignetteUI != null)
        {
            Color c = stressVignetteUI.color;
            c.a = stressRatio;
            stressVignetteUI.color = c;
        }

        // ⚡ อัปเดตเสียงหัวใจเต้นและเสียงหายใจตามระดับความเครียดสะสม
        UpdateStressAudio();

        // เซฟค่ากลับไปยังข้อมูลระดับโลก
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentStress = currentStress;
        }
    }

    // ⚡ [ระบบเสียงจำลองความเครียด] เล่นเสียงหัวใจเต้นตามระดับความเครียด
    private void UpdateStressAudio()
    {
        if (stressAudioSource == null) return;

        AudioClip targetClip = null;
        float targetVolume = 0f;

        if (currentStress >= 75f)
        {
            targetClip = heartbeatPanicSound != null ? heartbeatPanicSound : heartbeatRegularSound;
            targetVolume = Mathf.Clamp01((currentStress - 75f) / 25f * 0.7f + 0.3f); // ปรับระดับความดัง 0.3 - 1.0
        }
        else if (currentStress >= 50f)
        {
            targetClip = heartbeatRegularSound;
            targetVolume = Mathf.Clamp01((currentStress - 50f) / 25f * 0.5f + 0.1f); // ปรับระดับความดัง 0.1 - 0.6
        }

        if (targetClip != null)
        {
            if (stressAudioSource.clip != targetClip)
            {
                stressAudioSource.clip = targetClip;
                stressAudioSource.Play();
            }
            else if (!stressAudioSource.isPlaying)
            {
                stressAudioSource.Play();
            }

            stressAudioSource.volume = Mathf.MoveTowards(stressAudioSource.volume, targetVolume, Time.deltaTime * 1.5f);
        }
        else
        {
            if (stressAudioSource.isPlaying)
            {
                stressAudioSource.volume = Mathf.MoveTowards(stressAudioSource.volume, 0f, Time.deltaTime * 1.5f);
                if (stressAudioSource.volume <= 0.01f)
                {
                    stressAudioSource.Stop();
                    stressAudioSource.clip = null;
                }
            }
        }
    }

    // [เพิ่มใหม่] ฟังก์ชันสำหรับเปลี่ยนแปลงค่าความเครียด (จำกัดที่ 0 - 100)
    public void ChangeStress(float amount)
    {
        if (amount == 0) return;

        // ⚡ เรียกใช้งานแฟลชหน้าจอ
        if (amount > 0)
        {
            TriggerFlash(damageFlashColor);
        }
        else if (amount < 0)
        {
            TriggerFlash(healFlashColor);
        }

        // ตรวจสอบความสามารถของ Armband: ถ้าค่าความเครียดกำลังจะ "เพิ่มขึ้น" (amount เป็นบวก)
        if (amount > 0 && InventoryManager.Instance != null && InventoryManager.Instance.HasItem(ItemType.Armband))
        {
            if (!hasArmbandTriggeredToday)
            {
                amount *= 0.5f; // ลดระดับการเพิ่มของความเครียดลง 50%
                hasArmbandTriggeredToday = true;
                Debug.Log("🛡️ เอฟเฟกต์ Armband ทำงาน: ซึมซับและลดการเพิ่มค่าความเครียดลง 50% ครั้งแรกของวัน!");
            }
        }

        // ⚡ [แก้ไข] ปรับเปลี่ยนค่าส่งเข้า GameManagerSetup ข้อมูลกลางโดยตรง
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentStress = Mathf.Clamp(GameManagerSetup.Instance.currentStress + amount, 0f, 100f);
            currentStress = GameManagerSetup.Instance.currentStress;
        }
        else
        {
            currentStress = Mathf.Clamp(currentStress + amount, 0f, 100f);
        }
        Debug.Log("ค่าความเครียดปัจจุบัน: " + currentStress);
    }

    private void TriggerFlash(Color flashColor)
    {
        // หากไม่ได้ลาก UI มาใส่ไว้ จะไม่เกิดอะไรขึ้น (ป้องกัน Error)
        if (flashVignetteUI == null) return;

        // ถ้ายำลังกะพริบอยู่ก่อนแล้ว ให้หยุดของเก่าก่อน
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }

    private System.Collections.IEnumerator FlashRoutine(Color flashColor)
    {
        // ตั้งสีสว่างสุดทันทีที่โดนดาเมจ/ฮีล
        flashVignetteUI.color = flashColor;
        
        float timer = 0f;

        // ค่อยๆ หรี่แสงลงจนกว่าจะครบเวลา flashDuration
        while (timer < flashDuration)
        {
            timer += Time.unscaledDeltaTime; // ⚡ ใช้ unscaledDeltaTime เพื่อไม่ให้หยุดเวลาคุยกับ NPC (ตอน Time.timeScale = 0)
            float normalizedTime = timer / flashDuration;
            
            // เลิร์ป (Lerp) สีจากสีเต็มๆ ไปเป็นสีใส
            flashVignetteUI.color = Color.Lerp(flashColor, Color.clear, normalizedTime);
            
            yield return null; // รอเฟรมถัดไป
        }

        // ทำให้แน่ใจว่าสีเคลียร์หมดจดเมื่อจบเวลา
        flashVignetteUI.color = Color.clear;
        flashCoroutine = null;
    }

    // ⚡ เรียกฟังก์ชันนี้ตอนข้ามวัน (จากเตียง) เพื่อรีเซ็ตค่าปลอกแขน
    public void ResetDailyModifiers()
    {
        hasArmbandTriggeredToday = false;
    }
}