using UnityEngine;

public class LampInteraction : MonoBehaviour
{
    [Header("Lamp Settings")]
    [Tooltip("ลาก Light ที่เป็นแสงไฟของโคมไฟมาใส่ (หากปล่อยว่างจะค้นหาในวัตถุลูกอัตโนมัติ)")]
    [SerializeField] private Light lampLight;

    [Header("Audio Settings")]
    [Tooltip("เสียงคลิกสวิตช์เปิด/ปิดไฟ")]
    [SerializeField] private AudioClip switchSound;

    [Header("Initial State")]
    [Tooltip("สถานะตอนเริ่มเกม (ติ๊กถูก = เปิดไฟ, ไม่ติ๊ก = ปิดไฟ)")]
    [SerializeField] private bool isLampOn = false;

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

        // ค้นหาดวงไฟในตัวโคมไฟอัตโนมัติหากไม่ได้ลากใส่
        if (lampLight == null)
        {
            lampLight = GetComponentInChildren<Light>(true);
        }

        ApplyLampState();
    }

    // ฟังก์ชันสำหรับเปิด/ปิดโคมไฟ (เรียกใช้เมื่อกด F)
    public void ToggleLamp()
    {
        isLampOn = !isLampOn;

        // เล่นเสียงคลิกสวิตช์ไฟ
        if (switchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

        ApplyLampState();
        Debug.Log($"💡 [LampInteraction] โคมไฟหัวเตียง: {(isLampOn ? "เปิดไฟ" : "ปิดไฟ")}");
    }

    private void ApplyLampState()
    {
        if (lampLight != null)
        {
            lampLight.enabled = isLampOn;
        }

        if (interactable != null)
        {
            interactable.promptMessage = isLampOn ? "ปิดโคมไฟ" : "เปิดโคมไฟ";
        }
    }
}
