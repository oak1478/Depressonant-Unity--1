using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("🎯 ตัวละครหรือเป้าหมายที่ต้องการให้เลื่อนตาม")]
    [Tooltip("ลากตัวละคร Player มาใส่ช่องนี้ได้เลยครับ (ถ้าปล่อยว่าง ระบบจะหา Player ให้อัตโนมัติ)")]
    public Transform playerTransform;

    [Tooltip("(ตัวเลือกสำรอง) ถ้าไม่มี Player จะสลับไปใช้กล้องแทน")]
    public Transform cameraTransform;

    [Header("ความเร็วในการเลื่อนตาม (ค่าระหว่าง 0 ถึง 1)")]
    [Tooltip("0 = ภาพอยู่นิ่งกับที่, 0.3 = เลื่อนตามช้าๆ เกิดมิติ Parallax, 1 = วิ่งล็อกติดกับตัวละคร")]
    [Range(0f, 1f)]
    public float parallaxEffectX = 0.3f; // ควบคุมทิศทางซ้าย-ขวา
    
    [Range(0f, 1f)]
    public float parallaxEffectZ = 0.3f; // ควบคุมทิศทางบน-ล่าง (สำหรับมุมมอง Top-down/Isometric แกน Z)

    [Range(0f, 1f)]
    public float parallaxEffectY = 0f;   // ควบคุมความสูง (ปกติปล่อย 0)

    private Vector3 startPosition;
    private Vector3 startTargetPosition;
    private Transform activeTarget;

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้นของภาพพื้นหลัง
        startPosition = transform.position;

        // 1. ค้นหา Player ถ้ายังไม่ได้ลากใส่
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = GameObject.Find("Caracter"); // รองรับชื่อ Caracter ใน Hierarchy
            if (player == null) player = GameObject.Find("Character");

            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // 2. เลือกเป้าหมาย (ให้ความสำคัญกับ Player ก่อน ถ้าไม่มีค่อยใช้ Camera)
        if (playerTransform != null)
        {
            activeTarget = playerTransform;
        }
        else if (cameraTransform != null)
        {
            activeTarget = cameraTransform;
        }
        else if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            activeTarget = cameraTransform;
        }

        // 3. บันทึกตำแหน่งเริ่มต้นของเป้าหมาย
        if (activeTarget != null)
        {
            startTargetPosition = activeTarget.position;
        }
        else
        {
            Debug.LogWarning("[ParallaxBackground] ไม่พบเป้าหมาย (Player หรือ Camera) กรุณาลากตัวละครมาใส่ใน Inspector ครับ!");
        }
    }

    void LateUpdate()
    {
        if (activeTarget == null) return;

        // คำนวณระยะทางที่เป้าหมาย (Player) เดินขยับไปจากจุดเริ่มต้น
        Vector3 movement = activeTarget.position - startTargetPosition;

        // คำนวณตำแหน่งใหม่ของภาพพื้นหลังตามค่า Parallax Effect
        float targetX = startPosition.x + (movement.x * parallaxEffectX);
        float targetY = startPosition.y + (movement.y * parallaxEffectY);
        float targetZ = startPosition.z + (movement.z * parallaxEffectZ);

        // เลื่อนตำแหน่งภาพไปยังจุดที่คำนวณได้
        transform.position = new Vector3(targetX, targetY, targetZ);
    }
}