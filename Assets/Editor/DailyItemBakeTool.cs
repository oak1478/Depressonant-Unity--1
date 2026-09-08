using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public static class DailyItemBakeTool
{
    private struct ItemSpotDef
    {
        public string spotId;
        public string sceneName;
        public ItemType itemType;
        public int minDay;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;

        public ItemSpotDef(string id, string scene, ItemType type, int day, Vector3 pos, Vector3 rot)
        {
            spotId = id;
            sceneName = scene;
            itemType = type;
            minDay = day;
            localPosition = pos;
            localEulerAngles = rot;
        }
    }

    private static readonly ItemSpotDef[] allSpots = new ItemSpotDef[]
    {
        // Home
        new ItemSpotDef("Secret_Home_Candy_Day3", "Home", ItemType.Candy, 3, new Vector3(7.20f, 0.40f, -4.50f), new Vector3(90f, 0f, 25f)),
        new ItemSpotDef("Secret_Home_Milk_Day5", "Home", ItemType.StrawberryMilk, 5, new Vector3(0.50f, 0.40f, -4.20f), new Vector3(90f, 0f, 0f)),
        new ItemSpotDef("Secret_Home_Candy_Day7", "Home", ItemType.Candy, 7, new Vector3(-10.50f, 0.40f, -6.80f), new Vector3(90f, 0f, 45f)),
        new ItemSpotDef("Secret_Home_Milk_Day9", "Home", ItemType.StrawberryMilk, 9, new Vector3(8.30f, 0.40f, -12.00f), new Vector3(90f, 0f, 15f)),
        new ItemSpotDef("Secret_Home_Candy_Day11", "Home", ItemType.Candy, 11, new Vector3(-12.50f, 0.40f, -16.00f), new Vector3(90f, 0f, -30f)),

        // School
        new ItemSpotDef("Secret_School_Milk_Day1", "School", ItemType.StrawberryMilk, 1, new Vector3(13.20f, 0.30f, 1.80f), new Vector3(90f, 0f, 0f)),
        new ItemSpotDef("Secret_School_Candy_Day2", "School", ItemType.Candy, 2, new Vector3(-16.50f, 0.30f, 27.50f), new Vector3(90f, 0f, 20f)),
        new ItemSpotDef("Secret_School_Milk_Day3", "School", ItemType.StrawberryMilk, 3, new Vector3(-25.00f, 0.30f, 26.50f), new Vector3(90f, 0f, -10f)),
        new ItemSpotDef("Secret_School_Candy_Day4", "School", ItemType.Candy, 4, new Vector3(-45.00f, 0.30f, -13.50f), new Vector3(90f, 0f, 45f)),
        new ItemSpotDef("Secret_School_Milk_Day6", "School", ItemType.StrawberryMilk, 6, new Vector3(-116.00f, 0.30f, 38.20f), new Vector3(90f, 0f, 0f)),
        new ItemSpotDef("Secret_School_Candy_Day8", "School", ItemType.Candy, 8, new Vector3(9.50f, 0.30f, 1.80f), new Vector3(90f, 0f, 15f)),
        new ItemSpotDef("Secret_School_Milk_Day10", "School", ItemType.StrawberryMilk, 10, new Vector3(-4.50f, 0.30f, -13.00f), new Vector3(90f, 0f, -25f)),
        new ItemSpotDef("Secret_School_Candy_Day12", "School", ItemType.Candy, 12, new Vector3(-118.50f, 0.30f, 37.00f), new Vector3(90f, 0f, 30f)),

        // OutSide
        new ItemSpotDef("Secret_OutSide_Candy_Day1", "OutSide", ItemType.Candy, 1, new Vector3(-4.20f, 0.30f, -8.00f), new Vector3(90f, 0f, 35f)),
        new ItemSpotDef("Secret_OutSide_Milk_Day2", "OutSide", ItemType.StrawberryMilk, 2, new Vector3(25.00f, 0.30f, 10.00f), new Vector3(90f, 0f, 0f)),
        new ItemSpotDef("Secret_OutSide_Candy_Day3", "OutSide", ItemType.Candy, 3, new Vector3(50.50f, 0.30f, -7.50f), new Vector3(90f, 0f, -20f)),
        new ItemSpotDef("Secret_OutSide_Milk_Day4", "OutSide", ItemType.StrawberryMilk, 4, new Vector3(43.50f, 0.30f, -50.50f), new Vector3(90f, 0f, 15f)),
        new ItemSpotDef("Secret_OutSide_Candy_Day5", "OutSide", ItemType.Candy, 5, new Vector3(70.50f, 0.30f, -4.20f), new Vector3(90f, 0f, 40f)),
        new ItemSpotDef("Secret_OutSide_Milk_Day7", "OutSide", ItemType.StrawberryMilk, 7, new Vector3(35.00f, 0.30f, 64.00f), new Vector3(90f, 0f, -15f)),
        new ItemSpotDef("Secret_OutSide_Candy_Day9", "OutSide", ItemType.Candy, 9, new Vector3(39.50f, 0.30f, -49.00f), new Vector3(90f, 0f, 25f)),
        new ItemSpotDef("Secret_OutSide_Milk_Day11", "OutSide", ItemType.StrawberryMilk, 11, new Vector3(73.50f, 0.30f, -5.50f), new Vector3(90f, 0f, 0f)),
        new ItemSpotDef("Secret_OutSide_Candy_Day13", "OutSide", ItemType.Candy, 13, new Vector3(33.00f, 0.30f, 63.50f), new Vector3(90f, 0f, 45f))
    };

    [MenuItem("Tools/Depressonant/Deploy Hidden Items To Scenes")]
    public static void DeployAllItems()
    {
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/Home.unity",
            "Assets/Scenes/School.unity",
            "Assets/Scenes/OutSide.unity"
        };

        GameObject candyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Object/Item/Candy.prefab");
        GameObject milkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Object/Item/StrawberryMilf.prefab");

        if (candyPrefab == null || milkPrefab == null)
        {
            Debug.LogError("[DailyItemBakeTool] ไม่พบ Prefab ของ Candy หรือ StrawberryMilf!");
            return;
        }

        string originalScene = EditorSceneManager.GetActiveScene().path;

        foreach (string scenePath in scenePaths)
        {
            if (!File.Exists(scenePath)) continue;

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            GameObject container = GameObject.Find("SecretItemsContainer");
            if (container == null)
            {
                container = new GameObject("SecretItemsContainer");
                Undo.RegisterCreatedObjectUndo(container, "Create SecretItemsContainer");
            }

            int deployedCount = 0;
            foreach (var spot in allSpots)
            {
                if (!spot.sceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase)) continue;

                Transform existing = container.transform.Find(spot.spotId);
                GameObject itemObj = existing != null ? existing.gameObject : null;

                if (itemObj == null)
                {
                    GameObject prefab = spot.itemType == ItemType.Candy ? candyPrefab : milkPrefab;
                    itemObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
                    itemObj.name = spot.spotId;
                    Undo.RegisterCreatedObjectUndo(itemObj, "Create " + spot.spotId);
                }

                itemObj.transform.localPosition = spot.localPosition;
                itemObj.transform.localEulerAngles = spot.localEulerAngles;

                PickupItem pickup = itemObj.GetComponent<PickupItem>();
                if (pickup != null)
                {
                    var field = typeof(PickupItem).GetField("uniqueItemID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null) field.SetValue(pickup, spot.spotId);
                    EditorUtility.SetDirty(pickup);
                }

                ItemAppearanceController app = itemObj.GetComponent<ItemAppearanceController>();
                if (app != null)
                {
                    app.spawnFromDayOnward = true;
                    app.minDay = spot.minDay;
                    if (app.spawnDays == null) app.spawnDays = new List<int>();
                    app.spawnDays.Clear();
                    for (int d = spot.minDay; d <= 30; d++) app.spawnDays.Add(d);
                    EditorUtility.SetDirty(app);
                }

                EditorUtility.SetDirty(itemObj);
                deployedCount++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[DailyItemBakeTool] ติดตั้งไอเทม {deployedCount} ชิ้นลงในฉาก '{sceneName}' สำเร็จ");
        }

        if (!string.IsNullOrEmpty(originalScene) && File.Exists(originalScene))
        {
            EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
        }

        Debug.Log("[DailyItemBakeTool] ดำเนินการติดตั้งไอเทมลงในทุกฉากเรียบร้อยสมบูรณ์!");
    }
}
