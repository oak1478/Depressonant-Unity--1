using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        CharacterController cc = GetComponent<CharacterController>();

        // 1. ตรวจสอบก่อนว่าเป็นการโหลดเกมจากไฟล์เซฟหรือไม่
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.hasLoadedPosition)
        {
            if (cc != null) cc.enabled = false;
            
            transform.position = GameManagerSetup.Instance.loadedPlayerPosition;
            
            if (cc != null) cc.enabled = true;
            
            Debug.Log($"[PlayerSpawner] โหลดตำแหน่งตัวละครจากไฟล์เซฟสำเร็จ! ย้ายไปที่พิกัด: {transform.position}");
            
            // รีเซ็ตค่าเพื่อไม่ให้โหลดซ้ำซ้อน
            GameManagerSetup.Instance.hasLoadedPosition = false;
        }
        // 2. ถ้าไม่ได้โหลดเซฟ ให้ตรวจสอบว่าเป็นการเปลี่ยนฉากเดินข้ามประตูมาหรือไม่
        else if (!string.IsNullOrEmpty(SceneTransitionManager.targetSpawnPointName))
        {
            string targetName = SceneTransitionManager.targetSpawnPointName;
            
            // ค้นหาวัตถุ (จุดเกิด/ประตู) ในฉากที่มีชื่อตรงกัน
            GameObject spawnPoint = GameObject.Find(targetName);
            
            if (spawnPoint != null)
            {
                if (cc != null) cc.enabled = false;

                // ปรับตำแหน่งและมุมหันของตัวละคร
                transform.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;

                if (cc != null) cc.enabled = true;

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
