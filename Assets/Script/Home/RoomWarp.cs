using UnityEngine;

public class RoomWarp : MonoBehaviour
{
    [Header("ลากวัตถุที่เป็นจุดเกิดปลายทาง (ฝั่ง B) มาใส่ตรงนี้")]
    public Transform targetSpawnPoint; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetSpawnPoint != null)
            {
                // ⚠️ ทริคสำคัญ: ใน Unity ถ้าตัวละครใช้ CharacterController 
                // เราต้องสั่งปิดมันก่อน 1 เฟรมก่อนจะวาร์ป ไม่งั้นตัวละครจะเด้งกลับมาที่เดิมครับ
                CharacterController cc = other.GetComponent<CharacterController>();
                
                if (cc != null)
                {
                    cc.enabled = false; // ปิดชั่วคราว
                    other.transform.position = targetSpawnPoint.position; // วาร์ปตำแหน่ง
                    other.transform.rotation = targetSpawnPoint.rotation; // หันหน้าตามจุดเกิดฝั่ง B
                    cc.enabled = true;  // เปิดกลับมาใช้งานใหม่
                }
                else
                {
                    // ถ้าไม่ได้ใช้ CharacterController (เช่นใช้ Rigidbody หรือบอร์ดเคลื่อนที่ทั่วไป) สั่งวาร์ปตรงๆ ได้เลย
                    other.transform.position = targetSpawnPoint.position;
                    other.transform.rotation = targetSpawnPoint.rotation;
                }

                // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อเปิด/วาร์ปข้ามประตูห้องสำเร็จ
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnDoorOpened();
                }
            }
            else
            {
                Debug.LogWarning("คุณลืมลากจุดเกิดปลายทาง (ฝั่ง B) มาใส่ในช่อง Target Spawn Point นะครับ!");
            }
        }
    }
}