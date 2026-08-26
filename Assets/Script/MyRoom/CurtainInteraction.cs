using UnityEngine;

public class CurtainInteraction : MonoBehaviour
{
    [Header("Lighting Settings")]
    [Tooltip("ลากแสงแดดจากหน้าต่าง (เช่น Area Light, Directional Light หรือเมชแสงแดด) ที่ต้องการเปิด/ปิดเมื่อรูดม่าน")]
    [SerializeField] private GameObject windowLight;

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงรูดผ้าม่านมาใส่ (เช่น curtain-closing-323254)")]
    [SerializeField] private AudioClip curtainSound;

    private bool isCurtainOpen = false;
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
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }

        // เซตสถานะแสงแดดเริ่มต้นตามม่าน
        if (windowLight != null)
        {
            windowLight.SetActive(isCurtainOpen);
        }

        UpdatePrompt();
    }

    // ฟังก์ชันเปิด/ปิดผ้าม่าน (เรียกจาก BedroomInteractable)
    public void ToggleCurtain()
    {
        isCurtainOpen = !isCurtainOpen;

        // เล่นเสียงรูดผ้าม่าน
        if (curtainSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(curtainSound);
        }

        // เปิดหรือปิดแสงแดดหน้าต่าง
        if (windowLight != null)
        {
            windowLight.SetActive(isCurtainOpen);
        }

        // เมื่อเปิดม่าน แสงแดดจะช่วยผ่อนคลายความเครียดลง 5% ตามจิตวิทยา
        if (isCurtainOpen)
        {
            StressManager stress = Object.FindAnyObjectByType<StressManager>();
            if (stress != null)
            {
                stress.ChangeStress(-5f);
                Debug.Log("[CurtainInteraction] เปิดผ้าม่าน: แสงแดดช่วยบรรเทาความเครียดลง 5%");
            }
        }

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (interactable != null)
        {
            interactable.promptMessage = isCurtainOpen ? "ปิดผ้าม่าน" : "เปิดผ้าม่าน";
        }
    }
}
