using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class BedroomHierarchyOrganizer : EditorWindow
{
    [MenuItem("Tools/🧹 จัดระเบียบ Hierarchy (Bedroom_3D)")]
    public static void OrganizeHierarchy()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Bedroom_3D")
        {
            bool proceed = EditorUtility.DisplayDialog(
                "จัดระเบียบ Hierarchy",
                $"ฉากปัจจุบันคือ '{currentScene.name}' (ไม่ใช่ Bedroom_3D)\nคุณต้องการให้จัดระเบียบฉากนี้ด้วยหรือไม่?",
                "จัดระเบียบเลย", "ยกเลิก"
            );
            if (!proceed) return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Organize Bedroom 3D Hierarchy");

        // 1. สร้างหรือค้นหาโฟลเดอร์แม่หลัก (Root Groups)
        GameObject characterGroup = GetOrCreateGroup("Caracter"); // ตั้งชื่อตามโครงสร้าง Home
        GameObject bedroomGroup = GetOrCreateGroup("Bedroom");     // กลุ่มสิ่งของ/ห้องนอน (เหมือน Home)
        GameObject systemsGroup = GetOrCreateGroup("Systems");     // กลุ่มระบบและสคริปต์จัดการ

        // 2. จัดหมวดหมู่สิ่งของ
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject go in rootObjects)
        {
            if (go == null) continue;

            string name = go.name;

            // วัตถุที่ปล่อยให้อยู่ที่ Root ตามมาตรฐาน (เหมือนฉาก Home):
            // - Main Camera (ถ้าอยู่ข้างนอก)
            // - Directional Light / Sunlight
            // - EventSystem
            // - [GameManager_Core]
            if (name == "Caracter" || name == "Bedroom" || name == "Systems" || name == "Home") continue;
            if (name.Contains("Directional Light") || name == "Sunlight" || name == "EventSystem" || name == "[GameManager_Core]")
            {
                // ปล่อยไว้ที่ Root
                continue;
            }

            // 🎮 หมวดตัวละคร (Caracter)
            if (name == "Player" || name.StartsWith("Player_Spawn") || name.Contains("SpawnPoint") || name.Contains("Character"))
            {
                SetParentSafe(go, characterGroup);
                continue;
            }

            // ⚙️ หมวดระบบ (Systems)
            if (name.Contains("Manager") || name.Contains("System") || name.Contains("Controller") && name != "Player")
            {
                SetParentSafe(go, systemsGroup);
                continue;
            }

            // 🛏️ หมวดเฟอร์นิเจอร์และห้องนอน (Bedroom)
            if (name.Contains("Bedroom") || name.Contains("Furniture") || name.Contains("Bed") ||
                name.Contains("Curtain") || name.Contains("Mirror") || name.Contains("Light") ||
                name.Contains("Switch") || name.Contains("Lamp") || name.Contains("Wall") ||
                name.Contains("Glass") || name == "default")
            {
                SetParentSafe(go, bedroomGroup);
                continue;
            }

            // 📦 ถ้าเป็น UI / Canvas อื่นๆ
            if (name.Contains("Canvas") || name.Contains("Inventory"))
            {
                // ถ้าเป็น Canvas ของกระเป๋าที่ซ้ำซ้อนในฉาก จัดเข้า Systems
                SetParentSafe(go, systemsGroup);
                continue;
            }

            // สิ่งของอื่นๆ ในฉาก จัดเข้า Bedroom
            SetParentSafe(go, bedroomGroup);
        }

        // 3. จัดลำดับใน Hierarchy ให้สวยงามตามฉาก Home
        ReorderRootObjects(currentScene);

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(currentScene);

        EditorUtility.DisplayDialog(
            "จัดระเบียบสำเร็จ ✨",
            "จัดระเบียบ Hierarchy ของฉาก Bedroom_3D เรียบร้อยแล้วครับ!\nโครงสร้างถูกแบ่งเป็นหมวดหมู่อย่างสวยงามเหมือนฉาก Home แล้ว",
            "ตกลง"
        );
    }

    private static GameObject GetOrCreateGroup(string groupName)
    {
        GameObject group = GameObject.Find("/" + groupName);
        if (group == null)
        {
            group = new GameObject(groupName);
            group.transform.position = Vector3.zero;
            group.transform.rotation = Quaternion.identity;
            group.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(group, "Create " + groupName);
        }
        return group;
    }

    private static void SetParentSafe(GameObject child, GameObject parent)
    {
        if (child == null || parent == null || child == parent) return;
        Undo.SetTransformParent(child.transform, parent.transform, "Move " + child.name + " to " + parent.name);
    }

    private static void ReorderRootObjects(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        int index = 0;

        // ลำดับที่ 1: แสง
        foreach (var go in roots)
        {
            if (go != null && (go.name == "Sunlight" || go.name.Contains("Directional Light")))
            {
                go.transform.SetSiblingIndex(index++);
            }
        }

        // ลำดับที่ 2: EventSystem
        foreach (var go in roots)
        {
            if (go != null && go.name == "EventSystem")
            {
                go.transform.SetSiblingIndex(index++);
            }
        }

        // ลำดับที่ 3: Caracter
        foreach (var go in roots)
        {
            if (go != null && (go.name == "Caracter" || go.name == "Character"))
            {
                go.transform.SetSiblingIndex(index++);
            }
        }

        // ลำดับที่ 4: Bedroom / Home
        foreach (var go in roots)
        {
            if (go != null && (go.name == "Bedroom" || go.name == "Home"))
            {
                go.transform.SetSiblingIndex(index++);
            }
        }

        // ลำดับที่ 5: Systems
        foreach (var go in roots)
        {
            if (go != null && go.name == "Systems")
            {
                go.transform.SetSiblingIndex(index++);
            }
        }

        // ลำดับที่ 6: [GameManager_Core]
        foreach (var go in roots)
        {
            if (go != null && go.name.Contains("GameManager"))
            {
                go.transform.SetSiblingIndex(index++);
            }
        }
    }
}
