using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // ตัวแปร static เพื่อเก็บชื่อจุดเกิดปลายทางข้ามฉาก
    public static string targetSpawnPointName = "";

    // ฟังก์ชันหลักสำหรับโหลดฉากและระบุจุดเกิด (ใช้เรียกในโค้ด)
    public static void LoadSceneWithSpawn(string sceneName, string spawnPointName)
    {
        targetSpawnPointName = spawnPointName;

        // รีเซ็ตพิกัดโหลดจากไฟล์เซฟ เพื่อบังคับให้ตัวละครเกิดที่จุดเกิดของประตูเสมอ
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.hasLoadedPosition = false;
        }

        Debug.Log($"[SceneTransitionManager] กำลังโหลดฉาก '{sceneName}' และบันทึกจุดเกิดปลายทาง: '{spawnPointName}'");
        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันสำหรับเรียกผ่าน Unity Inspector (ลากใส่ Event แล้วกรอก 'ชื่อฉาก,ชื่อจุดเกิด')
    // เช่น กรอกว่า: Home, Interior_Furniture_Misc_Door (7)
    public void WarpToScene(string sceneAndSpawn)
    {
        string[] parts = sceneAndSpawn.Split(',');
        if (parts.Length == 2)
        {
            string sceneName = parts[0].Trim();
            string spawnPointName = parts[1].Trim();
            LoadSceneWithSpawn(sceneName, spawnPointName);
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] รูปแบบข้อความไม่ถูกต้อง! กรุณาใช้รูปแบบ: 'ชื่อฉาก, ชื่อจุดเกิด' เช่น 'Home, Interior_Furniture_Misc_Door (7)'");
        }
    }
}
