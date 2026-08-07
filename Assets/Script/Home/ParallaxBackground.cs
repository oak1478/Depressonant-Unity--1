using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("กล้องหลักของเกม")]
    public Transform cameraTransform;

    [Header("ความเร็วในการเลื่อนตามกล้อง (ค่าระหว่าง 0 ถึง 1)")]
    [Tooltip("0 = ภาพอยู่กับที่สนิท, 0.5 = ภาพเลื่อนตามครึ่งนึงของกล้อง, 1 = ภาพล็อคติดไปกับกล้องเลย")]
    [Range(0f, 1f)]
    public float parallaxEffectX = 0.2f; // ควบคุมทิศทางซ้าย-ขวา
    
    [Range(0f, 1f)]
    public float parallaxEffectZ = 0.2f; // ควบคุมทิศทางบน-ล่าง (สำหรับเกมมุมมองสูง top-down จะใช้แกน Z แทน Y)

    private Vector3 startPosition;
    private Vector3 startCameraPosition;

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้นของภาพ และตำแหน่งเริ่มต้นของกล้องไว้
        startPosition = transform.position;

        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("[ParallaxBackground] ไม่พบกล้องหลัก กรุณาลากกล้องมาใส่ใน Inspector ครับ!");
            }
        }

        if (cameraTransform != null)
        {
            startCameraPosition = cameraTransform.position;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // คำนวณระยะทางที่กล้องขยับไปจากจุดเริ่มต้น
        Vector3 cameraMovement = cameraTransform.position - startCameraPosition;

        // คำนวณระยะทางที่ภาพพื้นหลังควรจะขยับ (เอาทิศทางกล้อง คูณกับ ค่าความหน่วงที่เราตั้งไว้)
        float targetX = startPosition.x + (cameraMovement.x * parallaxEffectX);
        float targetZ = startPosition.z + (cameraMovement.z * parallaxEffectZ);

        // เลื่อนตำแหน่งภาพไปยังจุดที่คำนวณได้ (โดยรักษาความสูงแกน Y ของภาพไว้เท่าเดิม)
        transform.position = new Vector3(targetX, transform.position.y, targetZ);
    }
}