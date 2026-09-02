using UnityEngine;

public class RoomLightSwitch : MonoBehaviour
{
    [Header("Room Light References")]
    [Tooltip("ลาก Light ดวงไฟเพดานหลักที่ต้องการเปิด/ปิดมาใส่")]
    [SerializeField] private Light[] ceilingLights;

    [Header("Audio Settings")]
    [Tooltip("เสียงคลิกสวิตช์ไฟติดผนัง")]
    [SerializeField] private AudioClip switchSound;

    [Header("Switch Button (Visual)")]
    [Tooltip("ชิ้นส่วนปุ่มสวิตช์ที่จะกระดกเวลาเปิด/ปิด (ถ้ามี)")]
    [SerializeField] private Transform switchTogglePart;

    [Header("Initial State")]
    [Tooltip("สถานะเริ่มต้นเมื่อเข้าฉาก (ติ๊กถูก = เปิดไฟ, ไม่ติ๊ก = ปิดไฟ)")]
    [SerializeField] private bool isLightOn = true;

    private BedroomInteractable interactable;
    private AudioSource audioSource;

    private void Start()
    {
        interactable = GetComponent<BedroomInteractable>();
        if (interactable == null)
        {
            interactable = GetComponentInParent<BedroomInteractable>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D Sound
            audioSource.playOnAwake = false;
        }

        ApplyLightState();
    }

    // ฟังก์ชันสำหรับเปิด/ปิดไฟห้อง (เรียกใช้เมื่อกด F ที่สวิตช์)
    public void ToggleLight()
    {
        isLightOn = !isLightOn;

        // เล่นเสียงสวิตช์ไฟแป๊ก
        if (switchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

        ApplyLightState();
        Debug.Log($"💡 [RoomLightSwitch] สวิตช์ไฟห้องนอน: {(isLightOn ? "เปิดไฟสว่างทั่วห้อง" : "ปิดไฟห้อง")}");
    }

    private void ApplyLightState()
    {
        if (ceilingLights != null)
        {
            foreach (var light in ceilingLights)
            {
                if (light != null)
                {
                    light.enabled = isLightOn;
                }
            }
        }

        // ปรับมุมกระดกของปุ่มสวิตช์เพื่อความสมจริง
        if (switchTogglePart != null)
        {
            switchTogglePart.localRotation = Quaternion.Euler(isLightOn ? 15f : -15f, 0f, 0f);
        }

        if (interactable != null)
        {
            interactable.promptMessage = isLightOn ? "ปิดไฟห้อง" : "เปิดไฟห้อง";
        }
    }
}
