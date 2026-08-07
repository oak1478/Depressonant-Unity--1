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

    private Vignette vignette;
    private ChromaticAberration chromatic;
    private LensDistortion distortion;

    // ⚡ [ระบบใหม่] ตัวแปรสำหรับคุม Armband (ปลอกแขน)
    [HideInInspector] public bool hasArmbandTriggeredToday = false;

    void Start()
    {
        // โหลดค่าความเครียดระดับโลกมาใช้งาน
        if (GameManagerSetup.Instance != null)
        {
            currentStress = GameManagerSetup.Instance.currentStress;
        }

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

        // เซฟค่ากลับไปยังข้อมูลระดับโลก
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentStress = currentStress;
        }
    }

    // [เพิ่มใหม่] ฟังก์ชันสำหรับเปลี่ยนแปลงค่าความเครียด (จำกัดที่ 0 - 100)
    public void ChangeStress(float amount)
    {
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

    // ⚡ เรียกฟังก์ชันนี้ตอนข้ามวัน (จากเตียง) เพื่อรีเซ็ตค่าปลอกแขน
    public void ResetDailyModifiers()
    {
        hasArmbandTriggeredToday = false;
    }
}