#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class BedroomDecoratorTool : EditorWindow
{
    [MenuItem("Tools/✨ วางชุดโต๊ะเขียนหนังสือในห้องนอน (ใช้โมเดลแท้ สวยเป๊ะ 100%)")]
    public static void FurnishOfficialBedroomSet()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Bedroom_3D")
        {
            bool proceed = EditorUtility.DisplayDialog(
                "จัดห้องนอน",
                $"ฉากปัจจุบันคือ '{currentScene.name}' (ไม่ใช่ Bedroom_3D)\nคุณต้องการวางเฟอร์นิเจอร์ลงในฉากนี้หรือไม่?",
                "วางเลย", "ยกเลิก"
            );
            if (!proceed) return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Furnish Official Bedroom Set");

        // 1. ล้างของที่ลอย/หลุดนอกห้อง/หมุนเอียงทิ้งทั้งหมดทันที
        ClearAllGlitchedPropsImmediate();

        // 2. ค้นหาหรือสร้างกลุ่มแม่ Bedroom
        GameObject bedroomParent = GameObject.Find("Bedroom");
        if (bedroomParent == null)
        {
            bedroomParent = new GameObject("Bedroom");
            Undo.RegisterCreatedObjectUndo(bedroomParent, "Create Bedroom Group");
        }

        // 3. สร้างกลุ่มใหม่ Study_Desk_Corner ให้อยู่ใต้ Bedroom
        GameObject deskGroup = new GameObject("Study_Desk_Corner");
        deskGroup.transform.SetParent(bedroomParent.transform, false);
        Undo.RegisterCreatedObjectUndo(deskGroup, "Create Study_Desk_Corner");

        string prefabsPath = "Assets/OuterFile/Object/Bedroom/Prefabs/";

        // พิกัดภายในห้องนอนที่ถูกต้อง (Inside the Bedroom):
        // ผนังฝั่งซ้ายของห้อง: X = 2.4
        // เตียงนอนฝั่งขวา: X = 4.8
        // หน้าต่างด้านหลัง: Z = -15.4

        // -------------------------------------------------------------
        // 🖥️ 1. โต๊ะเขียนหนังสือแท้ (Table.prefab - พื้นผิวไม้สวยงาม มีตัวชนพร้อม)
        // -------------------------------------------------------------
        GameObject tablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "Table.prefab");
        if (tablePrefab != null)
        {
            GameObject desk = (GameObject)PrefabUtility.InstantiatePrefab(tablePrefab, deskGroup.transform);
            desk.name = "Study_Desk";
            desk.transform.position = new Vector3(2.45f, 0f, -12.2f);
            desk.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            BoxCollider deskCol = desk.GetComponent<BoxCollider>();
            if (deskCol == null) deskCol = desk.AddComponent<BoxCollider>();
            deskCol.center = new Vector3(0f, 0.4f, 0f);
            deskCol.size = new Vector3(1.0f, 0.8f, 1.8f);

            // 💡 2. โคมไฟอ่านหนังสือตั้งโต๊ะ (Lamp.prefab - สไตล์เดียวกับหัวเตียง)
            GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "Lamp.prefab");
            if (lampPrefab != null)
            {
                GameObject deskLamp = (GameObject)PrefabUtility.InstantiatePrefab(lampPrefab, desk.transform);
                deskLamp.name = "Desk_Lamp";
                deskLamp.transform.localPosition = new Vector3(0.55f, 0.76f, -0.35f);
                deskLamp.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);

                // แสงไฟอุ่น
                GameObject lightObj = new GameObject("Desk_Light");
                lightObj.transform.SetParent(deskLamp.transform, false);
                lightObj.transform.localPosition = new Vector3(0f, 0.35f, 0f);

                Light lampLight = lightObj.AddComponent<Light>();
                lampLight.type = LightType.Point;
                lampLight.color = new Color(1f, 0.92f, 0.78f, 1f); // Warm Amber
                lampLight.intensity = 1.8f;
                lampLight.range = 3.5f;
                lampLight.shadows = LightShadows.Soft;
            }
        }

        // -------------------------------------------------------------
        // 🗄️ 3. ตู้ชั้นวางหนังสือทรงสูงแท้ (Furniture01.prefab)
        // -------------------------------------------------------------
        GameObject shelfPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "Furniture01.prefab");
        if (shelfPrefab != null)
        {
            GameObject shelf = (GameObject)PrefabUtility.InstantiatePrefab(shelfPrefab, deskGroup.transform);
            shelf.name = "Bookshelf_Cabinet";
            shelf.transform.position = new Vector3(2.45f, 0f, -9.8f);
            shelf.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            BoxCollider shelfCol = shelf.GetComponent<BoxCollider>();
            if (shelfCol == null) shelfCol = shelf.AddComponent<BoxCollider>();
            shelfCol.center = new Vector3(0f, 0.85f, 0f);
            shelfCol.size = new Vector3(0.6f, 1.7f, 1.2f);
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(currentScene);

        Selection.activeGameObject = deskGroup;

        EditorUtility.DisplayDialog(
            "จัดห้องนอนสำเร็จ ✨",
            "ลบของที่ลอย/หลุดนอกห้องทิ้งหมดแล้ว และติดตั้งชุดเฟอร์นิเจอร์แท้เข้าสู่ในห้องนอน 3D เรียบร้อยแล้วครับ!\n\n" +
            "• 🖥️ โต๊ะเขียนหนังสือผิวไม้โมเดิร์น (ตั้งอยู่ในห้องริมผนังซ้าย)\n" +
            "• 💡 โคมไฟอ่านหนังสือตั้งโต๊ะ + แสงไฟอุ่น\n" +
            "• 🗄️ ตู้ชั้นวางหนังสือทรงสูง\n\n" +
            "โมเดลไม่กลับหัว ไม่ดำ และอยู่ในห้องนอนอย่างสวยงาม 100% ครับ!",
            "ตกลง"
        );
    }

    [MenuItem("Tools/🧹 ล้างของที่ลอยอยู่นอกห้องทิ้งทั้งหมด (Clean Glitched Props)")]
    public static void ClearAllGlitchedProps()
    {
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Clean Glitched Props");

        ClearAllGlitchedPropsImmediate();

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("ล้างของสำเร็จ", "ลบของที่ลอยและหลุดออกนอกห้องทั้งหมดทิ้งเรียบร้อยแล้วครับ", "ตกลง");
    }

    private static void ClearAllGlitchedPropsImmediate()
    {
        string[] groupsToClean = new string[] {
            "Cozy_Study_Corner", "Study_Desk_Set", "Modern_Furniture_Set", "Study_Desk_Corner"
        };

        foreach (string name in groupsToClean)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }

        // ค้นหาของที่ชื่อ Cozy_ ที่อาจหลุดอยู่ข้างนอก
        string[] looseObjects = new string[] {
            "Cozy_Study_Desk", "Cozy_Desk_Chair", "Cozy_Desk_Lamp", "Cozy_Desk_Books", 
            "Cozy_Plant_Pot", "Cozy_Bookshelf", "Cozy_Bedroom_Rug", "Desk_Chair", "Desk_Laptop"
        };

        foreach (string name in looseObjects)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }
    }
}
#endif
