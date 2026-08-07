using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        // ตรวจสอบว่ามีการบันทึกจุดเกิดปลายทางไว้หรือไม่
        if (!string.IsNullOrEmpty(SceneTransitionManager.targetSpawnPointName))
        {
            string targetName = SceneTransitionManager.targetSpawnPointName;
            
            // ค้นหาวัตถุ (จุดเกิด/ประตู) ในฉากที่มีชื่อตรงกัน
            GameObject spawnPoint = GameObject.Find(targetName);
            
            if (spawnPoint != null)
            {
                // บังคับปิด CharacterController ก่อนย้ายตำแหน่งชั่วคราวเพื่อป้องกันบั๊กเด้งกลับจุดเดิมของ Unity
                CharacterController cc = GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                }

                // ปรับตำแหน่งและมุมหันของตัวละคร
                transform.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;

                // เปิดกลับมาใช้งานใหม่
                if (cc != null)
                {
                    cc.enabled = true;
                }

                Debug.Log($"[PlayerSpawner] บันทึกจุดเกิดสำเร็จ! ย้ายตัวละครไปยังวัตถุชื่อ: '{targetName}'");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] ไม่พบวัตถุชื่อ '{targetName}' ในฉากนี้เพื่อทำการย้ายจุดเกิด!");
            }

            // เคลียร์ค่าตัวแปรเพื่อไม่ให้เกิดการสปอน์ซ้ำในการเปลี่ยนฉากครั้งต่อไป
            SceneTransitionManager.targetSpawnPointName = "";
        }
    }
}
