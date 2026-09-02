#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class BedroomDecoratorTool : EditorWindow
{
    [MenuItem("Tools/🪑 วางโต๊ะเรียนหนังสือ เก้าอี้ และหนังสือ (Student Desk + Chair + Textured Book)")]
    public static void FurnishStudentDeskAndChair()
    {
        AssetDatabase.Refresh();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Bedroom_3D")
        {
            bool proceed = EditorUtility.DisplayDialog(
                "จัดวางโต๊ะและเก้าอี้",
                $"ฉากปัจจุบันคือ '{currentScene.name}' (ไม่ใช่ Bedroom_3D)\nคุณต้องการวางโต๊ะเรียนและเก้าอี้ลงในฉากนี้หรือไม่?",
                "วางเลย", "ยกเลิก"
            );
            if (!proceed) return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Furnish Student Desk, Chair and Book");

        // 1. ล้างของเดิมทั้งหมด
        ClearAllDecorations();

        // 2. ค้นหากลุ่มแม่ Bedroom
        GameObject bedroomParent = GameObject.Find("Bedroom");
        if (bedroomParent == null)
        {
            bedroomParent = new GameObject("Bedroom");
            bedroomParent.transform.position = Vector3.zero;
            Undo.RegisterCreatedObjectUndo(bedroomParent, "Create Bedroom Group");
        }

        // 3. สร้างกลุ่มเฉพาะโต๊ะเรียนและเก้าอี้
        GameObject setGroup = new GameObject("Student_Study_Desk_Set");
        setGroup.transform.SetParent(bedroomParent.transform, false);
        setGroup.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(setGroup, "Create Student_Study_Desk_Set");

        string cozyPackPath = "Assets/OuterFile/Object/Cozy Study Asset Pack/";
        string kenneyPath = "Assets/OuterFile/Object/kenney_food_furniture-kit/FBX format/";
        string materialsPath = "Assets/OuterFile/Object/Bedroom/Materials/";
        string bookPath = "Assets/OuterFile/Object/Bedroom/Book/";

        Material woodMat = AssetDatabase.LoadAssetAtPath<Material>(materialsPath + "Chair_Wood_Mat.mat");
        if (woodMat == null) woodMat = AssetDatabase.LoadAssetAtPath<Material>(materialsPath + "Table/TableMat01.mat");

        // พิกัดริมผนังซ้ายในห้องนอน 3D
        Vector3 deskPos = new Vector3(0.85f, 0f, -12.0f);
        Vector3 chairPos = new Vector3(1.55f, 0f, -12.0f);

        float deskTopY = 0.75f;

        // -------------------------------------------------------------
        // 🖥️ 1. โต๊ะเรียนหนังสือแท้ (Cozy_Table.fbx)
        // -------------------------------------------------------------
        GameObject deskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cozyPackPath + "Cozy_Table.fbx");
        if (deskPrefab == null) deskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(kenneyPath + "desk.fbx");

        if (deskPrefab != null)
        {
            GameObject deskObj = (GameObject)PrefabUtility.InstantiatePrefab(deskPrefab, setGroup.transform);
            deskObj.name = "Student_Study_Desk";
            deskObj.transform.position = deskPos;
            deskObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            FitToTargetHeight(deskObj, 0.75f);
            if (woodMat != null) ApplyMaterialToRenderers(deskObj, woodMat);

            Bounds b = GetCombinedBounds(deskObj);
            deskTopY = b.max.y;

            BoxCollider col = deskObj.GetComponent<BoxCollider>();
            if (col == null) col = deskObj.AddComponent<BoxCollider>();
            col.center = deskObj.transform.InverseTransformPoint(b.center);
            col.size = new Vector3(b.size.x / deskObj.transform.localScale.x, 
                                   b.size.y / deskObj.transform.localScale.y, 
                                   b.size.z / deskObj.transform.localScale.z);
        }

        // -------------------------------------------------------------
        // 🪑 2. เก้าอี้เรียนหนังสือ (Cozy_Chair.fbx)
        // -------------------------------------------------------------
        GameObject chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cozyPackPath + "Cozy_Chair.fbx");
        if (chairPrefab == null) chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(kenneyPath + "chairModernCushion.fbx");

        if (chairPrefab != null)
        {
            GameObject chairObj = (GameObject)PrefabUtility.InstantiatePrefab(chairPrefab, setGroup.transform);
            chairObj.name = "Student_Desk_Chair";
            chairObj.transform.position = chairPos;
            chairObj.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            FitToTargetHeight(chairObj, 0.85f);
            if (woodMat != null) ApplyMaterialToRenderers(chairObj, woodMat);

            Bounds b = GetCombinedBounds(chairObj);
            BoxCollider col = chairObj.GetComponent<BoxCollider>();
            if (col == null) col = chairObj.AddComponent<BoxCollider>();
            col.center = chairObj.transform.InverseTransformPoint(b.center);
            col.size = new Vector3(b.size.x / chairObj.transform.localScale.x, 
                                   b.size.y / chairObj.transform.localScale.y, 
                                   b.size.z / chairObj.transform.localScale.z);
        }

        // -------------------------------------------------------------
        // 📖 3. หนังสือเรียนที่มี Texture กระดาษและปกผ้าแท้ (book.obj)
        // -------------------------------------------------------------
        GameObject bookObjPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bookPath + "book.obj");
        if (bookObjPrefab != null)
        {
            GameObject bookObj = (GameObject)PrefabUtility.InstantiatePrefab(bookObjPrefab, setGroup.transform);
            bookObj.name = "Textured_Study_Book";
            bookObj.transform.position = new Vector3(0.85f, deskTopY + 0.01f, -11.75f);
            bookObj.transform.rotation = Quaternion.Euler(0f, 15f, 0f);

            // ปรับขนาดความหนาหนังสือให้อยู่ที่ 0.05m
            FitToTargetHeight(bookObj, 0.05f);

            // เชื่อม Material URP กับ Textures/cloth.png และ paper.png
            Texture2D clothTex = AssetDatabase.LoadAssetAtPath<Texture2D>(bookPath + "Textures/cloth.png");
            Texture2D paperTex = AssetDatabase.LoadAssetAtPath<Texture2D>(bookPath + "Textures/paper.png");

            MeshRenderer r = bookObj.GetComponentInChildren<MeshRenderer>();
            if (r != null && (clothTex != null || paperTex != null))
            {
                Material[] mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] != null && mats[i].name.ToLower().Contains("pages"))
                    {
                        if (paperTex != null) mats[i].mainTexture = paperTex;
                    }
                    else if (mats[i] != null)
                    {
                        if (clothTex != null) mats[i].mainTexture = clothTex;
                    }
                }
                r.sharedMaterials = mats;
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(currentScene);

        Selection.activeGameObject = setGroup;

        EditorUtility.DisplayDialog(
            "จัดวางสำเร็จ ✨",
            "วาง 'โต๊ะเรียนหนังสือ', 'เก้าอี้ไม้' และ 'หนังสือที่มี Texture กระดาษและปกผ้า' เรียบร้อยแล้วครับ!\n\n" +
            "• 🖥️ Student_Study_Desk (โต๊ะเขียนหนังสือ)\n" +
            "• 🪑 Student_Desk_Chair (เก้าอี้ไม้เข้าชุด)\n" +
            "• 📖 Textured_Study_Book (หนังสือที่มี Texture กระดาษ paper.png และปกผ้า cloth.png วางบนโต๊ะ)\n\n" +
            "สะอาดตา สัดส่วนสมจริง ไม่มีของรกอื่นๆ ครับ!",
            "ตกลง"
        );
    }

    private static void FitToTargetHeight(GameObject go, float targetHeightMeters)
    {
        if (go == null) return;
        go.transform.localScale = Vector3.one;
        Bounds b = GetCombinedBounds(go);
        float currentHeight = b.size.y;
        if (currentHeight > 0.001f)
        {
            float scaleFactor = targetHeightMeters / currentHeight;
            go.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    private static Bounds GetCombinedBounds(GameObject go)
    {
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.one);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            b.Encapsulate(renderers[i].bounds);
        }
        return b;
    }

    private static void ApplyMaterialToRenderers(GameObject root, Material mat)
    {
        if (root == null || mat == null) return;
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in renderers)
        {
            if (r != null)
            {
                Material[] mats = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                r.sharedMaterials = mats;
            }
        }
    }

    [MenuItem("Tools/🧹 ล้างของตกแต่งออกทั้งหมด (Clear All Props)")]
    public static void ClearAllDecorationsMenu()
    {
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Clear All Props");

        ClearAllDecorations();

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("ล้างของสำเร็จ", "ลบชุดของตกแต่งออกทั้งหมดเรียบร้อยแล้วครับ", "ตกลง");
    }

    private static void ClearAllDecorations()
    {
        string[] groupsToClean = new string[] {
            "Student_Study_Desk_Set", "Study_Desk_And_Chair", "Cozy_Study_Corner", 
            "Study_Desk_Corner", "Study_Desk_Set", "Modern_Furniture_Set"
        };

        foreach (string name in groupsToClean)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }

        string[] looseObjects = new string[] {
            "Student_Study_Desk", "Student_Desk_Chair", "Cozy_Study_Desk", "Cozy_Desk_Chair", 
            "Cozy_Desk_Lamp", "Cozy_Desk_Books", "Cozy_Plant_Pot", "Cozy_Bookshelf", 
            "Cozy_Bedroom_Rug", "Desk_Chair", "Desk_Laptop", "Bookshelf_Cabinet", "Desk_Lamp", 
            "Study_Desk", "Textured_Study_Book"
        };

        foreach (string name in looseObjects)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }
    }
}
#endif
