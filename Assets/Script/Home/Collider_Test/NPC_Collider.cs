using UnityEngine;

public class NPC_Collider : MonoBehaviour
{
    // ฟังก์ชันนี้จะทำงานอัตโนมัติเมื่อมีวัตถุเดินมาชน (ถ้าติ๊ก Is Trigger ใน Box Collider แล้ว)
    void OnTriggerEnter(Collider other)
    {
        // เช็กว่าวัตถุที่เข้ามาชนมี Tag คำว่า "Player" หรือไม่
        if (other.CompareTag("Player"))
        {
            // สั่งให้ส่งข้อความไปแสดงในหน้าต่าง Console ของ Unity
            Debug.Log("Collider Test, NPC");
        }
    }
}