using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct GlobalInventoryItem
{
    public int slotIndex;
    public string itemTypeName; // เก็บเป็นชื่อ string ของ Enum เพื่อความปลอดภัยในการเซฟเป็นไฟล์ JSON
    public int count;
}

public class GameManagerSetup : MonoBehaviour
{
    // ระบบ Singleton เพื่อการเรียกใช้งานข้ามคลาสที่ง่ายขึ้น
    public static GameManagerSetup Instance { get; private set; }

    [Header("Global Game State")]
    [Range(0, 100)] public float currentStress = 0f; // ค่าความเครียดสะสมหลัก
    [Range(1, 20)] public int currentDay = 1;         // วันปัจจุบัน
    public int consecutiveMaxStressDays = 0;          // จำนวนวันที่ความเครียดเต็ม 100% ติดต่อกัน
    public int activeSaveSlot = 1; // สล็อตเซฟปัจจุบัน (1-4)
    public float playTime = 0f;    // เก็บเวลาเล่นรวมสะสม (หน่วยเป็นวินาที)

    [Header("Player Load State")]
    public bool hasLoadedPosition = false;
    public Vector3 loadedPlayerPosition;

    [Header("Global Inventory")]
    public List<GlobalInventoryItem> savedInventory = new List<GlobalInventoryItem>(); // ลิสต์เก็บไอเทมในกระเป๋า

    [Header("Items Picked Up")]
    public List<string> pickedUpItemIDs = new List<string>(); // ลิสต์เก็บรหัสไอเทมที่ถูกเก็บไปแล้วในเกม

    public bool IsItemPickedUp(string id)
    {
        return pickedUpItemIDs.Contains(id);
    }

    public void RegisterPickedUpItem(string id)
    {
        if (!pickedUpItemIDs.Contains(id))
        {
            pickedUpItemIDs.Add(id);
        }
    }

    [Header("Daily Talk State")]
    public List<string> talkedNPCsToday = new List<string>();

    public bool HasTalkedToNPCToday(string npcName)
    {
        if (string.IsNullOrEmpty(npcName)) return false;

        string cleanSearch = npcName.Trim();
        if (cleanSearch.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanSearch = cleanSearch.Substring(4);
        }

        foreach (string talked in talkedNPCsToday)
        {
            if (string.IsNullOrEmpty(talked)) continue;

            string cleanTalked = talked.Trim();
            if (cleanTalked.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanTalked = cleanTalked.Substring(4);
            }

            if (string.Equals(talked, npcName.Trim(), System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(cleanTalked, cleanSearch, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterNPCTalkedToday(string npcName)
    {
        if (string.IsNullOrEmpty(npcName)) return;

        string cleanName = npcName.Trim();
        if (cleanName.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanName = cleanName.Substring(4);
        }

        if (!HasTalkedToNPCToday(cleanName))
        {
            talkedNPCsToday.Add(cleanName);
            Debug.Log($"[GameManagerSetup] บันทึกการพูดคุยกับ '{cleanName}' ประจำวันสำเร็จ (รวมคุยแล้ววันนี้: {talkedNPCsToday.Count} คน)");
        }
    }

    public void ResetDailyNPCTalk()
    {
        talkedNPCsToday.Clear();
        Debug.Log("[GameManagerSetup] รีเซ็ตรายชื่อ NPC ที่พูดคุยแล้วสำหรับวันใหม่");
    }

    public void ResetGameSession()
    {
        currentStress = 0f;
        currentDay = 1;
        consecutiveMaxStressDays = 0;
        activeSaveSlot = 1;
        playTime = 0f;
        hasLoadedPosition = false;
        loadedPlayerPosition = Vector3.zero;

        savedInventory.Clear();
        pickedUpItemIDs.Clear();
        talkedNPCsToday.Clear();

        Debug.Log("[GameManagerSetup] รีเซ็ตสถานะข้อมูลเกมทั้งหมดเรียบร้อย");
    }

    void Awake()
    {
        // ป้องกันไม่ให้มี GameManager ซ้ำซ้อนกันในซีน
        if (Instance == null)
        {
            Instance = this;
            
            // สั่งให้วัตถุนี้ (รวมถึงวัตถุลูก) ข้ามซีนได้ไม่โดนทำลาย
            DontDestroyOnLoad(gameObject); 

            // แปะ SaveSystem เข้ากับ GameManager อัตโนมัติเพื่อให้สั่งเซฟได้จากทุกที่
            if (GetComponent<SaveSystem>() == null)
            {
                gameObject.AddComponent<SaveSystem>();
            }

            // โหลดค่าจากหน่วยความจำชั่วคราว (Pending Data) ทันทีใน Awake เพื่อแก้ปัญหา Race Conditions
            UnpackPendingSaveData();
        }
        else
        {
            // ถ้ามีอยู่แล้วและเกิดใหม่ ให้ทำลายตัวใหม่ทิ้งเพื่อยึดตัวเก่าที่มีข้อมูลเดิม
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ResetDailyNPCTalk();
            return;
        }

        // ตรวจสอบและสร้าง EventSystem อัตโนมัติหากฉากนั้นไม่มี เพื่อให้ UI และการใช้ไอเทมทำงานได้ 100%
        EnsureEventSystem();

        // ตรวจสอบและติดตั้ง Collider ให้กับเฟอร์นิเจอร์และสิ่งก่อสร้างที่ยังไม่มี Collider เพื่อป้องกันการเดินทะลุ
        EnsureSceneColliders();

        // ตรวจสอบและเชื่อมต่อ AudioSource ทั้งหมดในฉากเข้ากับ Mixer SFX Group อัตโนมัติ
        EnsureAudioRouting();

        // หากมีข้อมูลเซฟรออยู่ใน Pending Data ให้อัปเดตข้อมูลกลางทันที
        UnpackPendingSaveData();

        // ปลดล็อก/ล็อกเมาส์ตามฉากโดยอัตโนมัติเมื่อเกิดการเปลี่ยนซีน เพื่อป้องกันปัญหาเมาส์ล่องหนหรือค้างคา
        bool is3DScene = scene.name == "Bedroom_3D";
        Cursor.visible = !is3DScene;
        Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;
        Debug.Log($"[Cursor Sync] โหลดฉาก '{scene.name}' สำเร็จ! ตั้งค่าเมาส์เริ่มต้น: visible={Cursor.visible}, lockState={Cursor.lockState}");
    }

    private void EnsureEventSystem()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var existing = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (existing == null)
            {
                GameObject esObj = new GameObject("EventSystem_Auto");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("[GameManagerSetup] ตรวจไม่พบ EventSystem ในฉาก ได้สร้าง EventSystem_Auto ให้อัตโนมัติ");
            }
        }
    }

    public void EnsureAudioRouting()
    {
        UnityEngine.Audio.AudioMixerGroup sfxGroup = SettingsController.SFXGroup;
        if (sfxGroup == null && SettingsController.Instance != null && SettingsController.Instance.mainMixer != null)
        {
            var groups = SettingsController.Instance.mainMixer.FindMatchingGroups("SFX");
            if (groups != null && groups.Length > 0) sfxGroup = groups[0];
        }

        if (sfxGroup == null) return;

        AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int routedCount = 0;
        foreach (var src in sources)
        {
            if (src != null && src.outputAudioMixerGroup == null)
            {
                string objName = src.gameObject.name.ToLower();
                if (!objName.Contains("bgm") && !objName.Contains("music"))
                {
                    src.outputAudioMixerGroup = sfxGroup;
                    routedCount++;
                }
            }
        }
        if (routedCount > 0)
        {
            Debug.Log($"[GameManagerSetup] เชื่อมต่อ AudioSource จำนวน {routedCount} ตัวเข้ากับกลุ่ม SFX สำเร็จ");
        }
    }

    public void EnsureSceneColliders()
    {
        string[] targetContainerNames = new string[] { "Furniture", "Furnitur", "Building", "Mapwall", "Lamps", "Fences" };
        int collidersAddedCount = 0;

        foreach (string containerName in targetContainerNames)
        {
            GameObject[] matchingObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (GameObject targetObj in matchingObjects)
            {
                if (targetObj != null && targetObj.name.Equals(containerName, System.StringComparison.OrdinalIgnoreCase))
                {
                    bool isBuildingOrWall = containerName.Equals("Building", System.StringComparison.OrdinalIgnoreCase) || 
                                           containerName.Equals("Mapwall", System.StringComparison.OrdinalIgnoreCase);
                    collidersAddedCount += AddCollidersToHierarchy(targetObj.transform, isBuildingOrWall);
                }
            }
        }

        if (collidersAddedCount > 0)
        {
            Debug.Log($"[GameManagerSetup] ติดตั้ง Collider อัตโนมัติให้เฟอร์นิเจอร์/สิ่งก่อสร้างสำเร็จ {collidersAddedCount} ชิ้น");
        }
    }

    private int AddCollidersToHierarchy(Transform root, bool isBuildingOrWall)
    {
        int added = 0;
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer mr in renderers)
        {
            if (mr == null) continue;
            GameObject go = mr.gameObject;

            // 1. ยกเว้นไอเทมเก็บได้ (PickupItem)
            if (go.GetComponent<PickupItem>() != null || go.GetComponentInParent<PickupItem>() != null)
                continue;

            // 2. ยกเว้นตัวละครผู้เล่น และ NPC
            if (go.CompareTag("Player") || go.tag == "NPC" ||
                go.GetComponent<NPCInteraction>() != null || go.GetComponentInParent<NPCInteraction>() != null ||
                go.GetComponent<CharacterController>() != null || go.GetComponent<PlayerMovement>() != null)
                continue;

            // 3. ยกเว้นจุดวาร์ปหรือประตูเชื่อมฉาก
            if (go.GetComponent<TriggerSceneWarp>() != null || go.GetComponent<RoomWarp>() != null)
                continue;

            // 4. ตรวจสอบว่ามี Collider แบบทึบ (isTrigger == false) อยู่แล้วหรือไม่
            Collider[] colliders = go.GetComponents<Collider>();
            bool hasSolidCollider = false;
            foreach (Collider c in colliders)
            {
                if (c != null && !c.isTrigger)
                {
                    hasSolidCollider = true;
                    break;
                }
            }

            if (hasSolidCollider) continue;

            // 5. ติดตั้ง Collider
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Vector3 meshSize = mf.sharedMesh.bounds.size;
                if (meshSize.magnitude < 0.08f) continue;

                bool meshColliderSuccess = false;
                if (isBuildingOrWall)
                {
                    try
                    {
                        MeshCollider mc = go.AddComponent<MeshCollider>();
                        mc.sharedMesh = mf.sharedMesh;
                        mc.convex = false;
                        meshColliderSuccess = true;
                        added++;
                    }
                    catch
                    {
                        meshColliderSuccess = false;
                    }
                }

                if (!meshColliderSuccess)
                {
                    BoxCollider bc = go.AddComponent<BoxCollider>();
                    bc.center = mf.sharedMesh.bounds.center;
                    bc.size = mf.sharedMesh.bounds.size;
                    added++;
                }
            }
            else
            {
                Bounds b = mr.bounds;
                if (b.size.magnitude >= 0.08f)
                {
                    BoxCollider bc = go.AddComponent<BoxCollider>();
                    Vector3 ls = go.transform.lossyScale;
                    bc.center = go.transform.InverseTransformPoint(b.center);
                    bc.size = new Vector3(
                        b.size.x / Mathf.Max(0.001f, Mathf.Abs(ls.x)),
                        b.size.y / Mathf.Max(0.001f, Mathf.Abs(ls.y)),
                        b.size.z / Mathf.Max(0.001f, Mathf.Abs(ls.z))
                    );
                    added++;
                }
            }
        }

        return added;
    }

    public void UnpackPendingSaveData()
    {
        if (SaveSystem.pendingLoadData != null)
        {
            currentDay = SaveSystem.pendingLoadData.currentDay > 0 ? SaveSystem.pendingLoadData.currentDay : 1;
            currentStress = SaveSystem.pendingLoadData.currentStress;
            consecutiveMaxStressDays = SaveSystem.pendingLoadData.consecutiveMaxStressDays;
            savedInventory = SaveSystem.pendingLoadData.inventoryItems != null 
                ? new List<GlobalInventoryItem>(SaveSystem.pendingLoadData.inventoryItems) 
                : new List<GlobalInventoryItem>();
            pickedUpItemIDs = SaveSystem.pendingLoadData.pickedUpItemIDs != null 
                ? new List<string>(SaveSystem.pendingLoadData.pickedUpItemIDs) 
                : new List<string>();
            talkedNPCsToday = new List<string>(); // เริ่มวันใหม่ ล้างรายชื่อ NPC ที่คุยไปแล้ว
            activeSaveSlot = SaveSystem.pendingLoadData.activeSaveSlot;
            playTime = SaveSystem.pendingLoadData.playTime;

            // โหลดพิกัด (เฉพาะเมื่อมีพิกัดจริงที่ไม่ใช่ 0,0,0)
            if (SaveSystem.pendingLoadData.playerX != 0 || SaveSystem.pendingLoadData.playerY != 0 || SaveSystem.pendingLoadData.playerZ != 0)
            {
                hasLoadedPosition = true;
                loadedPlayerPosition = new Vector3(SaveSystem.pendingLoadData.playerX, SaveSystem.pendingLoadData.playerY, SaveSystem.pendingLoadData.playerZ);
            }
            else
            {
                hasLoadedPosition = false;
            }

            Debug.Log($"[GameManagerSetup] โหลดข้อมูลเซฟ (Slot {activeSaveSlot}) สำเร็จ! วันที่: {currentDay}");
            SaveSystem.pendingLoadData = null; // ล้างข้อมูลออก

            // ซิงค์ไปยัง DayManager ทันที
            if (DayManager.Instance != null)
            {
                DayManager.Instance.currentDay = currentDay;
                DayManager.Instance.UpdateAllAppearances();
            }
        }
    }

    void Start()
    {
        EnsureSceneColliders();
    }

    void Update()
    {
        // สะสมเวลาเล่นรวมของเซฟเกม
        playTime += Time.deltaTime;
    }
}