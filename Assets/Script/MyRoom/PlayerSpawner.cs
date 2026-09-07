using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        ApplySpawning(gameObject);
    }

    public static void ApplySpawning(GameObject playerObj)
    {
        if (playerObj == null) return;
        CharacterController cc = playerObj.GetComponent<CharacterController>();

        // 1. ตรวจสอบก่อนว่าเป็นการโหลดเกมจากไฟล์เซฟหรือไม่
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.hasLoadedPosition)
        {
            Vector3 targetPos = GameManagerSetup.Instance.loadedPlayerPosition;

            // ป้องกันปัญหาเซฟเก่าบันทึกพิกัด (0,0,0) ซึ่งจะทำให้ตกแมพ
            if (targetPos == Vector3.zero)
            {
                Debug.LogWarning("[PlayerSpawner] ตรวจพบพิกัดเซฟเป็น (0,0,0) ทำการแก้พิกัดตกแมพให้อัตโนมัติ!");
                GameObject defaultPoint = GameObject.Find("Player_Spawn_Point");
                if (defaultPoint != null)
                {
                    targetPos = defaultPoint.transform.position;
                }
                else
                {
                    targetPos = new Vector3(-0.808f, 1f, -6.931f); // พิกัดปลอดภัยในห้องนอน
                }
            }

            Teleport(playerObj, cc, targetPos, playerObj.transform.rotation);
            Debug.Log($"[PlayerSpawner] โหลดตำแหน่งตัวละครจากไฟล์เซฟสำเร็จ! ย้ายไปที่พิกัด: {targetPos}");

            // รีเซ็ตค่าเพื่อไม่ให้โหลดซ้ำซ้อน
            GameManagerSetup.Instance.hasLoadedPosition = false;
            SceneTransitionManager.targetSpawnPointName = "";
        }
        // 2. ถ้าไม่ได้โหลดเซฟ ให้ตรวจสอบว่าเป็นการเปลี่ยนฉากเดินข้ามประตูมาหรือไม่
        else if (!string.IsNullOrEmpty(SceneTransitionManager.targetSpawnPointName))
        {
            string targetName = SceneTransitionManager.targetSpawnPointName;

            // ค้นหาวัตถุ (จุดเกิด/ประตู) ในฉากที่มีชื่อตรงกัน
            GameObject spawnPoint = GameObject.Find(targetName);

            if (spawnPoint != null)
            {
                Teleport(playerObj, cc, spawnPoint.transform.position, spawnPoint.transform.rotation);
                Debug.Log($"[PlayerSpawner] บันทึกจุดเกิดสำเร็จ! ย้ายตัวละครไปยังวัตถุชื่อ: '{targetName}' ที่พิกัด {spawnPoint.transform.position}");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] ไม่พบวัตถุชื่อ '{targetName}' ในฉากนี้! พยายามค้นหาจุดเกิดสำรอง Player_Spawn_Point...");
                GameObject fallbackPoint = GameObject.Find("Player_Spawn_Point");
                if (fallbackPoint != null)
                {
                    Teleport(playerObj, cc, fallbackPoint.transform.position, fallbackPoint.transform.rotation);
                }
            }

            // เคลียร์ค่าตัวแปรเพื่อไม่ให้เกิดการสปอน์ซ้ำในการเปลี่ยนฉากครั้งต่อไป
            SceneTransitionManager.targetSpawnPointName = "";
        }
    }

    private static void Teleport(GameObject playerObj, CharacterController cc, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (cc != null) cc.enabled = false;

        playerObj.transform.position = targetPosition;
        playerObj.transform.rotation = targetRotation;

        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;
    }
}
