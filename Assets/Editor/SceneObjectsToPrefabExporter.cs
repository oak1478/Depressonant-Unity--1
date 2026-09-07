using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneObjectsToPrefabExporter : EditorWindow
{
    private static string targetFolderPath = "Assets/OuterFile/Object/Bedroom_Prefabs";

    [MenuItem("Tools/📦 แปลงวัตถุในฉากเป็น Prefab ลง OuterFile/Object")]
    public static void ExportSelectedOrAllToPrefabs()
    {
        // 1. ตรวจสอบและสร้างโฟลเดอร์ปลายทาง
        if (!AssetDatabase.IsValidFolder("Assets/OuterFile/Object/Bedroom_Prefabs"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/OuterFile/Object"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/OuterFile"))
                {
                    AssetDatabase.CreateFolder("Assets", "OuterFile");
                }
                AssetDatabase.CreateFolder("Assets/OuterFile", "Object");
            }
            AssetDatabase.CreateFolder("Assets/OuterFile/Object", "Bedroom_Prefabs");
        }

        // 2. ตรวจสอบว่าผู้เล่นเลือกวัตถุไว้หรือไม่
        GameObject[] selectedObjects = Selection.gameObjects;
        GameObject[] objectsToExport;

        if (selectedObjects != null && selectedObjects.Length > 0)
        {
            objectsToExport = selectedObjects;
        }
        else
        {
            // ถ้าไม่ได้เลือกวัตถุไว้ ให้ดึงวัตถุหลักทั้งหมดในฉาก (Root Objects)
            objectsToExport = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }

        int count = 0;
        foreach (GameObject go in objectsToExport)
        {
            if (go == null) continue;

            // ข้ามกล้องหลัก, แสงอาทิตย์, และระบบหลัก เพื่อไม่ให้ปนกับโมเดลสิ่งของ
            if (go.name == "Main Camera" || go.name == "Directional Light" || go.name == "Sunlight" ||
                go.name == "EventSystem" || go.name == "[GameManager_Core]")
            {
                continue;
            }

            string safeName = go.name.Replace(" (1)", "").Replace(" ", "_");
            string prefabPath = $"{targetFolderPath}/{safeName}.prefab";

            // บันทึกเป็น Prefab และเชื่อมโยง (Connect) กับในฉาก
            PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.UserAction);
            count++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "สร้าง Prefab สำเร็จ ✨",
            $"ทำการบันทึกวัตถุจำนวน {count} ชิ้น เป็น Prefab ลงในโฟลเดอร์:\n'{targetFolderPath}' เรียบร้อยแล้วครับ!",
            "ตกลง"
        );
    }
}
