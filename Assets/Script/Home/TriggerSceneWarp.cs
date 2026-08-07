using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class TriggerSceneWarp : MonoBehaviour
{
    [Header("ฉากปลายทางที่ต้องการไป")]
    [SerializeField] private string targetSceneName = "Bedroom_3D";

    [Header("ชื่อวัตถุในฉากใหม่ที่จะใช้เป็นจุดเกิด")]
    [SerializeField] private string targetSpawnPointName = "Player_Spawn_Point";

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าผู้เล่นเดินมาชน
        if (other.CompareTag("Player"))
        {
            // ⚡ [เพิ่มใหม่] ออโต้เซฟสถานะก่อนเปลี่ยนฉาก
            if (SaveSystem.Instance != null)
            {
                int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                SaveSystem.Instance.SaveGameFromGlobal(other.transform.position, currentSceneIndex);
                Debug.Log("[Auto-Save] บันทึกเกมก่อนเปลี่ยนฉากเรียบร้อย!");
            }

            // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อเปิด/ข้ามประตูสำเร็จ
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnDoorOpened();
            }

            // โหลดฉากและส่งข้อมูลจุดเกิด
            SceneTransitionManager.LoadSceneWithSpawn(targetSceneName, targetSpawnPointName);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TriggerSceneWarp))]
public class TriggerSceneWarpEditor : Editor
{
    private readonly string[] sceneOptions = { "Bedroom_3D", "Home", "OutSide", "School" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty sceneNameProp = serializedObject.FindProperty("targetSceneName");
        SerializedProperty spawnPointProp = serializedObject.FindProperty("targetSpawnPointName");

        // 1. ตรวจสอบและหาตำแหน่ง Index ปัจจุบันของซีนจากตัวเลือกที่มี
        int currentIndex = System.Array.IndexOf(sceneOptions, sceneNameProp.stringValue);
        if (currentIndex < 0)
        {
            currentIndex = 0;
            sceneNameProp.stringValue = sceneOptions[0];
        }

        // 2. วาดช่องสลับซีนปลายทางแบบ Dropdown
        int newIndex = EditorGUILayout.Popup("Target Scene Name", currentIndex, sceneOptions);
        sceneNameProp.stringValue = sceneOptions[newIndex];

        // 3. วาดช่องจุดเกิดแบบปกติ
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(spawnPointProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
