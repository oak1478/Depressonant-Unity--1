#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
public static class SetupCozyBooksDiary
{
    static SetupCozyBooksDiary()
    {
        EditorApplication.delayCall += AutoRun;
    }

    private static void AutoRun()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying || Application.isPlaying || EditorApplication.isCompiling) return;
        ExecuteSetup(false);
    }

    [MenuItem("Tools/Depressonant/Setup Cozy Books Diary")]
    public static void ManualSetup()
    {
        ExecuteSetup(true);
    }

    public static void ExecuteSetup(bool force)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying || Application.isPlaying) return;

        string scenePath = "Assets/Scenes/Bedroom_3D.unity";
        if (!File.Exists(scenePath)) return;

        bool isCurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == scenePath;
        UnityEngine.SceneManagement.Scene scene;

        if (isCurrentScene)
        {
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
        else
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }

        GameObject cozyBooks = FindCozyBooksInScene(scene);
        if (cozyBooks == null)
        {
            Debug.LogWarning("[SetupCozyBooksDiary] ไม่พบ GameObject 'Cozy_Books' ในซีน Bedroom_3D");
            if (!isCurrentScene) EditorSceneManager.CloseScene(scene, true);
            return;
        }

        // ตรวจสอบว่าได้รับการตั้งค่าไปแล้วหรือยัง
        PickupItem existingPickup = cozyBooks.GetComponent<PickupItem>();
        if (!force && existingPickup != null && existingPickup.itemType == ItemType.Diary && existingPickup.itemIcon != null)
        {
            // ตั้งค่าเรียบร้อยแล้ว ไม่ต้องบันทึกซ้ำ
            if (!isCurrentScene) EditorSceneManager.CloseScene(scene, true);
            return;
        }

        Undo.RegisterCompleteObjectUndo(cozyBooks, "Setup Cozy Books Diary");

        // 1. BoxCollider สำหรับตรวจจับการกดเก็บไอเทม
        BoxCollider col = cozyBooks.GetComponent<BoxCollider>();
        if (col == null) col = cozyBooks.AddComponent<BoxCollider>();
        col.isTrigger = true;
        // เนื่องจาก Cozy_Books มี localScale = 0.1 จึงกำหนดขนาด localSize 15 x 10 x 15 เพื่อให้ได้ระยะทริกเกอร์ 1.5m x 1.0m x 1.5m
        col.center = new Vector3(0f, 2f, 0f);
        col.size = new Vector3(15f, 10f, 15f);

        // 2. โหลดไอคอนไดอารี่ดั้งเดิม ($Item_1 จาก $Item.png)
        Sprite originalDiarySprite = null;
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/$Item.png");
        foreach (var a in allAssets)
        {
            if (a is Sprite s && s.name == "$Item_1")
            {
                originalDiarySprite = s;
                break;
            }
        }
        if (originalDiarySprite == null)
        {
            // หากไม่พบชื่อ $Item_1 ให้ค้นหา Sprite ชิ้นที่ 2 ใน Sprite Sheet
            foreach (var a in allAssets)
            {
                if (a is Sprite s && (s.name.Contains("1") || s.name.ToLower().Contains("diary")))
                {
                    originalDiarySprite = s;
                    break;
                }
            }
        }

        // 3. โหลดเสียงเก็บไอเทม
        AudioClip pickupClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/pick up item.mp3");

        // 4. คอมโพเนนต์ PickupItem
        PickupItem pickup = cozyBooks.GetComponent<PickupItem>();
        if (pickup == null) pickup = cozyBooks.AddComponent<PickupItem>();
        pickup.itemType = ItemType.Diary;
        if (originalDiarySprite != null) pickup.itemIcon = originalDiarySprite;
        pickup.pickupSound = pickupClip;
        pickup.interactionRadius = 2.5f;
        pickup.maxVerticalDistance = 2.5f;
        pickup.UniqueItemID = "Cozy_Books_Diary";

        // 5. คอมโพเนนต์ ItemAppearanceController (เปิดให้เก็บได้ตั้งแต่วันที่ 1 เป็นต้นไป)
        ItemAppearanceController appearance = cozyBooks.GetComponent<ItemAppearanceController>();
        if (appearance == null) appearance = cozyBooks.AddComponent<ItemAppearanceController>();
        appearance.spawnFromDayOnward = true;
        appearance.minDay = 1;
        if (appearance.spawnDays == null) appearance.spawnDays = new List<int>();
        appearance.spawnDays.Clear();

        // 6. คอมโพเนนต์ ItemGlowEffect (แสงกระพริบสีทองอ่อน)
        ItemGlowEffect glow = cozyBooks.GetComponent<ItemGlowEffect>();
        if (glow == null) glow = cozyBooks.AddComponent<ItemGlowEffect>();
        glow.pulseSpeed = 2.5f;
        glow.minGlow = 0.4f;
        glow.maxGlow = 1.0f;
        glow.glowColor = new Color(1f, 0.9f, 0.4f, 1f);

        EditorUtility.SetDirty(cozyBooks);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[SetupCozyBooksDiary] สำเร็จ! ตั้งค่า Cozy_Books ใน Bedroom_3D เป็นไอเทม Diary พร้อมรูปเวอร์ชันเดิม เริ่มเก็บได้ตั้งแต่ Day 1 เรียบร้อย");

        if (!isCurrentScene)
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static GameObject FindCozyBooksInScene(UnityEngine.SceneManagement.Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (var r in roots)
        {
            if (r.name == "Cozy_Books") return r;
            Transform[] children = r.GetComponentsInChildren<Transform>(true);
            foreach (var c in children)
            {
                if (c.gameObject.name == "Cozy_Books") return c.gameObject;
            }
        }
        return null;
    }
}
#endif
