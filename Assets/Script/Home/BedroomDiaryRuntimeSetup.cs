using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ระบบตรวจสอบและตั้งค่าไอเทมไดอารี่ (Cozy_Books) ในห้องนอนแบบ Runtime
/// ทำงานอัตโนมัติเมื่อเข้าฉาก Bedroom_3D เพื่อให้มั่นใจ 100% ว่า:
/// 1. มีโมเดล Cozy_Books วางอยู่บนโต๊ะพร้อมให้เก็บตั้งแต่วันที่ 1
/// 2. มีคอมโพเนนต์ PickupItem, BoxCollider, ItemAppearanceController, ItemGlowEffect ครบถ้วน
/// 3. หากผู้เล่นเคยเก็บไปแล้วในไฟล์เซฟ โมเดลจะไม่ปรากฏขึ้นมาซ้ำ
/// </summary>
public static class BedroomDiaryRuntimeSetup
{
    private static bool isInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        CheckAndSetupBedroomDiary(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndSetupBedroomDiary(scene);
    }

    public static void CheckAndSetupBedroomDiary(Scene scene)
    {
        if (scene.name != "Bedroom_3D") return;

        // 1. ตรวจสอบก่อนว่าไอเทมชิ้นนี้เคยถูกเก็บไปแล้วหรือยัง
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.IsItemPickedUp("Cozy_Books_Diary"))
        {
            GameObject pickedObj = FindCozyBooks(scene);
            if (pickedObj != null)
            {
                pickedObj.SetActive(false);
                Debug.Log("[BedroomDiaryRuntimeSetup] ไดอารี่ 'Cozy_Books_Diary' ถูกเก็บไปแล้ว ทำการซ่อนโมเดลในห้องนอน");
            }
            return;
        }

        // 2. ค้นหา GameObject Cozy_Books บนโต๊ะ
        GameObject cozyBooks = FindCozyBooks(scene);
        if (cozyBooks == null) return;

        // 3. ติดตั้ง BoxCollider สำหรับ Trigger การชน
        BoxCollider col = cozyBooks.GetComponent<BoxCollider>();
        if (col == null) col = cozyBooks.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, 2f, 0f);
        col.size = new Vector3(15f, 10f, 15f);

        // 4. ติดตั้งคอมโพเนนต์ PickupItem
        PickupItem pickup = cozyBooks.GetComponent<PickupItem>();
        if (pickup == null) pickup = cozyBooks.AddComponent<PickupItem>();
        pickup.itemType = ItemType.Diary;
        pickup.interactionRadius = 2.5f;
        pickup.maxVerticalDistance = 2.5f;
        pickup.UniqueItemID = "Cozy_Books_Diary";

        if (pickup.itemIcon == null && InventoryManager.Instance != null)
        {
            pickup.itemIcon = InventoryManager.Instance.GetSpriteForItemType(ItemType.Diary);
        }

        // 5. ติดตั้งคอมโพเนนต์ ItemAppearanceController (เริ่มเก็บได้ตั้งแต่ Day 1)
        ItemAppearanceController appearance = cozyBooks.GetComponent<ItemAppearanceController>();
        if (appearance == null) appearance = cozyBooks.AddComponent<ItemAppearanceController>();
        appearance.spawnFromDayOnward = true;
        appearance.minDay = 1;
        appearance.UpdateItemAppearance();

        // 6. ติดตั้งคอมโพเนนต์ ItemGlowEffect (เรืองแสง)
        ItemGlowEffect glow = cozyBooks.GetComponent<ItemGlowEffect>();
        if (glow == null) glow = cozyBooks.AddComponent<ItemGlowEffect>();
        glow.pulseSpeed = 2.5f;
        glow.minGlow = 0.4f;
        glow.maxGlow = 1.0f;
        glow.glowColor = new Color(1f, 0.9f, 0.4f, 1f);

        Debug.Log("[BedroomDiaryRuntimeSetup] ยืนยันการตั้งค่า Cozy_Books เป็นไดอารี่ในห้องนอนสำเร็จ (Day 1 เป็นต้นไป)");
    }

    private static GameObject FindCozyBooks(Scene scene)
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
