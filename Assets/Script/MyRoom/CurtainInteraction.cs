using UnityEngine;
using System.Collections;

public class CurtainInteraction : MonoBehaviour
{
    [Header("Lighting Settings")]
    [Tooltip("ลากแสงแดดจากหน้าต่าง (เช่น Area Light, Directional Light หรือเมชแสงแดด) ที่ต้องการเปิด/ปิดเมื่อรูดม่าน")]
    [SerializeField] private GameObject windowLight;

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงรูดผ้าม่านมาใส่ (เช่น curtain-closing-323254)")]
    [SerializeField] private AudioClip curtainSound;

    [Header("Curtain Transforms (อนิเมชันผ้าม่าน)")]
    [Tooltip("ลาก Transform ของผ้าม่านซ้ายมาใส่ (หรือระบบจะค้นหาชื่อ Curtain_Left ให้อัตโนมัติ)")]
    [SerializeField] private Transform curtainLeft;

    [Tooltip("ลาก Transform ของผ้าม่านขวามาใส่ (หรือระบบจะค้นหาชื่อ Curtain_Right ให้อัตโนมัติ)")]
    [SerializeField] private Transform curtainRight;

    [Header("Animation Settings")]
    [Tooltip("ความเร็วในการรูดผ้าม่าน (วินาที)")]
    [SerializeField] private float animationDuration = 0.75f;

    private bool isCurtainOpen = false;
    private BedroomInteractable interactable;
    private AudioSource audioSource;
    private Coroutine animationCoroutine;

    // ตำแหน่งและขนาดของผ้าม่านตอน "เปิด" (ผูกรวบชิดขอบ)
    private Vector3 openPosLeft = new Vector3(-1.45f, 0.05f, 0.1f);
    private Vector3 openScaleLeft = new Vector3(0.32f, 2.15f, 0.08f);

    private Vector3 openPosRight = new Vector3(1.45f, 0.05f, 0.1f);
    private Vector3 openScaleRight = new Vector3(0.32f, 2.15f, 0.08f);

    // ตำแหน่งและขนาดของผ้าม่านตอน "ปิด" (กางออกปิดหน้าต่าง)
    private Vector3 closedPosLeft = new Vector3(-0.75f, 0.05f, 0.1f);
    private Vector3 closedScaleLeft = new Vector3(1.55f, 2.15f, 0.08f);

    private Vector3 closedPosRight = new Vector3(0.75f, 0.05f, 0.1f);
    private Vector3 closedScaleRight = new Vector3(1.55f, 2.15f, 0.08f);

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

        // ค้นหาผ้าม่านซ้ายและขวาอัตโนมัติหากไม่ได้ลากใส่ใน Inspector
        if (curtainLeft == null)
        {
            Transform tLeft = transform.Find("Curtain_Left");
            if (tLeft != null) curtainLeft = tLeft;
        }

        if (curtainRight == null)
        {
            Transform tRight = transform.Find("Curtain_Right");
            if (tRight != null) curtainRight = tRight;
        }

        // เซตสถานะเริ่มต้น (เริ่มต้นที่ม่านปิด มืดสลัว)
        ApplyCurtainStateImmediate(isCurtainOpen);

        if (windowLight != null)
        {
            windowLight.SetActive(isCurtainOpen);
        }

        UpdatePrompt();
    }

    // บันทึกวันที่ล่าสุดที่ได้รับผลลดความเครียด (เพื่อล็อกให้ลดได้เพียง 1 ครั้งต่อ 1 วัน)
    private static int lastStressReliefDay = -1;

    // ฟังก์ชันเปิด/ปิดผ้าม่าน (เรียกจาก BedroomInteractable)
    public void ToggleCurtain()
    {
        isCurtainOpen = !isCurtainOpen;

        // เล่นเสียงรูดผ้าม่าน
        if (curtainSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(curtainSound);
        }

        // เล่นอนิเมชันรูดผ้าม่านอย่างนุ่มนวล
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateCurtainsRoutine(isCurtainOpen));

        // เมื่อเปิดม่าน แสงแดดจะช่วยผ่อนคลายความเครียดลง 5% (จำกัด 1 ครั้งต่อ 1 วัน)
        if (isCurtainOpen)
        {
            int currentDay = 1;
            if (DayManager.Instance != null) currentDay = DayManager.Instance.currentDay;
            else if (GameManagerSetup.Instance != null) currentDay = GameManagerSetup.Instance.currentDay;

            if (lastStressReliefDay != currentDay)
            {
                lastStressReliefDay = currentDay;
                StressManager stress = Object.FindAnyObjectByType<StressManager>();
                if (stress != null)
                {
                    stress.ChangeStress(-5f);
                    Debug.Log($"☀️ [CurtainInteraction] เปิดผ้าม่าน (วันที่ {currentDay}): แสงแดดช่วยบรรเทาความเครียดลง 5%");
                }
            }
            else
            {
                Debug.Log($"☀️ [CurtainInteraction] เปิดผ้าม่าน: วันที่ {currentDay} ได้รับการลดความเครียดไปแล้วในวันนี้ (จำกัด 1 ครั้ง/วัน)");
            }
        }

        UpdatePrompt();
    }

    private IEnumerator AnimateCurtainsRoutine(bool targetOpen)
    {
        float elapsed = 0f;

        Vector3 startPosL = curtainLeft != null ? curtainLeft.localPosition : Vector3.zero;
        Vector3 startScaleL = curtainLeft != null ? curtainLeft.localScale : Vector3.one;
        Vector3 endPosL = targetOpen ? openPosLeft : closedPosLeft;
        Vector3 endScaleL = targetOpen ? openScaleLeft : closedScaleLeft;

        Vector3 startPosR = curtainRight != null ? curtainRight.localPosition : Vector3.zero;
        Vector3 startScaleR = curtainRight != null ? curtainRight.localScale : Vector3.one;
        Vector3 endPosR = targetOpen ? openPosRight : closedPosRight;
        Vector3 endScaleR = targetOpen ? openScaleRight : closedScaleRight;

        // หากเปิดม่าน ให้เปิดแสงขึ้นทันทีที่เริ่มรูด
        if (targetOpen && windowLight != null)
        {
            windowLight.SetActive(true);
        }

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            // SmoothStep เพื่อความนุ่มนวลในการเคลื่อนที่ (Ease-In-Out)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (curtainLeft != null)
            {
                curtainLeft.localPosition = Vector3.Lerp(startPosL, endPosL, smoothT);
                curtainLeft.localScale = Vector3.Lerp(startScaleL, endScaleL, smoothT);
            }

            if (curtainRight != null)
            {
                curtainRight.localPosition = Vector3.Lerp(startPosR, endPosR, smoothT);
                curtainRight.localScale = Vector3.Lerp(startScaleR, endScaleR, smoothT);
            }

            yield return null;
        }

        // ตั้งค่าปลายทางให้แน่นอน
        ApplyCurtainStateImmediate(targetOpen);

        // หากปิดม่าน ให้ดับแสงหลังจากรูดม่านปิดสนิท
        if (!targetOpen && windowLight != null)
        {
            windowLight.SetActive(false);
        }

        animationCoroutine = null;
    }

    private void ApplyCurtainStateImmediate(bool targetOpen)
    {
        if (curtainLeft != null)
        {
            curtainLeft.localPosition = targetOpen ? openPosLeft : closedPosLeft;
            curtainLeft.localScale = targetOpen ? openScaleLeft : closedScaleLeft;
        }

        if (curtainRight != null)
        {
            curtainRight.localPosition = targetOpen ? openPosRight : closedPosRight;
            curtainRight.localScale = targetOpen ? openScaleRight : closedScaleRight;
        }
    }

    private void UpdatePrompt()
    {
        if (interactable != null)
        {
            interactable.promptMessage = isCurtainOpen ? "ปิดผ้าม่าน" : "เปิดผ้าม่าน";
        }
    }
}
