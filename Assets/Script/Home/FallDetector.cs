using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("ระดับความลึกต่ำสุด (หากค่า Y ต่ำกว่านี้ถือว่าตกแมพ)")]
    [SerializeField] private float thresholdY = -15f;

    [Header("จุดเกิดใหม่ (หากปล่อยว่างไว้ระบบจะยึดพิกัดจุดเริ่มต้นของด่านให้อัตโนมัติ)")]
    [SerializeField] private Transform respawnPoint;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // บันทึกตำแหน่งเดิมของตัวละครตอนเริ่มรันด่านไว้เป็นจุดเกิดสำรองปลอดภัยที่สุด
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    void Update()
    {
        // ตรวจเช็คว่าแกน Y (ความสูง) ของตัวละครร่วงต่ำกว่าเกณฑ์ลบหรือยัง
        if (transform.position.y < thresholdY)
        {
            WarpToSafety();
        }
    }

    private void WarpToSafety()
    {
        Vector3 targetPos = startingPosition;
        Quaternion targetRot = startingRotation;

        // 1. ถ้าหากผู้เล่นกำหนดจุดเกิดใหม่ Respawn Point ไว้ใน Inspector ให้ใช้ค่านั้น
        if (respawnPoint != null)
        {
            targetPos = respawnPoint.position;
            targetRot = respawnPoint.rotation;
        }
        else
        {
            // 2. ถ้าไม่ได้กำหนดไว้ ให้พยายามสแกนหาวัตถุชื่อจุดเกิดมาตรฐานในฉากปัจจุบันอัตโนมัติ
            GameObject fallbackSpawn = GameObject.Find("Player_Spawn_Point");
            if (fallbackSpawn == null) fallbackSpawn = GameObject.Find("Door_Warp_B31");
            
            if (fallbackSpawn != null)
            {
                targetPos = fallbackSpawn.transform.position;
                targetRot = fallbackSpawn.transform.rotation;
            }
        }

        // ⚡ ป้องกันบั๊กฟิสิกส์ดึงกลับตำแหน่งเดิม: ปิด CharacterController ก่อนสั่งเคลื่อนย้าย
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        // เปิด CharacterController กลับมาทำงาน
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Debug.LogWarning($"⚠️ [FallDetector] ตัวละครตกร่วงจากแมพ! ย้ายกลับมาพิกัดปลอดภัยอัตโนมัติที่: {targetPos}");
    }
}
