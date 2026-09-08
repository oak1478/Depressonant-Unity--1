using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ระบบกระจายไอเทมลับประจำวัน (ลูกอม และ นมสตรอว์เบอร์รี่) ในฉากต่างๆ
/// เพื่อให้ผู้เล่นเดินสำรวจและเก็บรางวัลตามแต่ละวัน
/// </summary>
public class DailySecretItemManager : MonoBehaviour
{
    public static DailySecretItemManager Instance { get; private set; }

    [System.Serializable]
    public class SecretItemSpot
    {
        public string spotId;
        public string sceneName;
        public ItemType itemType;
        public int minDay;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
    }

    private static readonly List<SecretItemSpot> allSecretSpots = new List<SecretItemSpot>
    {
        // ==================== ฉากบ้าน (Home) ====================
        new SecretItemSpot
        {
            spotId = "Secret_Home_Candy_Day3",
            sceneName = "Home",
            itemType = ItemType.Candy,
            minDay = 3,
            localPosition = new Vector3(7.20f, 0.40f, -4.50f),
            localEulerAngles = new Vector3(90f, 0f, 25f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_Home_Milk_Day5",
            sceneName = "Home",
            itemType = ItemType.StrawberryMilk,
            minDay = 5,
            localPosition = new Vector3(0.50f, 0.40f, -4.20f),
            localEulerAngles = new Vector3(90f, 0f, 0f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_Home_Candy_Day7",
            sceneName = "Home",
            itemType = ItemType.Candy,
            minDay = 7,
            localPosition = new Vector3(-10.50f, 0.40f, -6.80f),
            localEulerAngles = new Vector3(90f, 0f, 45f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_Home_Milk_Day9",
            sceneName = "Home",
            itemType = ItemType.StrawberryMilk,
            minDay = 9,
            localPosition = new Vector3(8.30f, 0.40f, -12.00f),
            localEulerAngles = new Vector3(90f, 0f, 15f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_Home_Candy_Day11",
            sceneName = "Home",
            itemType = ItemType.Candy,
            minDay = 11,
            localPosition = new Vector3(-12.50f, 0.40f, -16.00f),
            localEulerAngles = new Vector3(90f, 0f, -30f)
        },

        // ==================== ฉากโรงเรียน (School) ====================
        new SecretItemSpot
        {
            spotId = "Secret_School_Milk_Day1",
            sceneName = "School",
            itemType = ItemType.StrawberryMilk,
            minDay = 1,
            localPosition = new Vector3(13.20f, 0.30f, 1.80f),
            localEulerAngles = new Vector3(90f, 0f, 0f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Candy_Day2",
            sceneName = "School",
            itemType = ItemType.Candy,
            minDay = 2,
            localPosition = new Vector3(-16.50f, 0.30f, 27.50f),
            localEulerAngles = new Vector3(90f, 0f, 20f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Milk_Day3",
            sceneName = "School",
            itemType = ItemType.StrawberryMilk,
            minDay = 3,
            localPosition = new Vector3(-25.00f, 0.30f, 26.50f),
            localEulerAngles = new Vector3(90f, 0f, -10f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Candy_Day4",
            sceneName = "School",
            itemType = ItemType.Candy,
            minDay = 4,
            localPosition = new Vector3(-45.00f, 0.30f, -13.50f),
            localEulerAngles = new Vector3(90f, 0f, 45f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Milk_Day6",
            sceneName = "School",
            itemType = ItemType.StrawberryMilk,
            minDay = 6,
            localPosition = new Vector3(-116.00f, 0.30f, 38.20f),
            localEulerAngles = new Vector3(90f, 0f, 0f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Candy_Day8",
            sceneName = "School",
            itemType = ItemType.Candy,
            minDay = 8,
            localPosition = new Vector3(9.50f, 0.30f, 1.80f),
            localEulerAngles = new Vector3(90f, 0f, 15f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Milk_Day10",
            sceneName = "School",
            itemType = ItemType.StrawberryMilk,
            minDay = 10,
            localPosition = new Vector3(-4.50f, 0.30f, -13.00f),
            localEulerAngles = new Vector3(90f, 0f, -25f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_School_Candy_Day12",
            sceneName = "School",
            itemType = ItemType.Candy,
            minDay = 12,
            localPosition = new Vector3(-118.50f, 0.30f, 37.00f),
            localEulerAngles = new Vector3(90f, 0f, 30f)
        },

        // ==================== ฉากนอกบ้าน / ถนน (OutSide) ====================
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Candy_Day1",
            sceneName = "OutSide",
            itemType = ItemType.Candy,
            minDay = 1,
            localPosition = new Vector3(-4.20f, 0.30f, -8.00f),
            localEulerAngles = new Vector3(90f, 0f, 35f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Milk_Day2",
            sceneName = "OutSide",
            itemType = ItemType.StrawberryMilk,
            minDay = 2,
            localPosition = new Vector3(25.00f, 0.30f, 10.00f),
            localEulerAngles = new Vector3(90f, 0f, 0f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Candy_Day3",
            sceneName = "OutSide",
            itemType = ItemType.Candy,
            minDay = 3,
            localPosition = new Vector3(50.50f, 0.30f, -7.50f),
            localEulerAngles = new Vector3(90f, 0f, -20f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Milk_Day4",
            sceneName = "OutSide",
            itemType = ItemType.StrawberryMilk,
            minDay = 4,
            localPosition = new Vector3(43.50f, 0.30f, -50.50f),
            localEulerAngles = new Vector3(90f, 0f, 15f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Candy_Day5",
            sceneName = "OutSide",
            itemType = ItemType.Candy,
            minDay = 5,
            localPosition = new Vector3(70.50f, 0.30f, -4.20f),
            localEulerAngles = new Vector3(90f, 0f, 40f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Milk_Day7",
            sceneName = "OutSide",
            itemType = ItemType.StrawberryMilk,
            minDay = 7,
            localPosition = new Vector3(35.00f, 0.30f, 64.00f),
            localEulerAngles = new Vector3(90f, 0f, -15f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Candy_Day9",
            sceneName = "OutSide",
            itemType = ItemType.Candy,
            minDay = 9,
            localPosition = new Vector3(39.50f, 0.30f, -49.00f),
            localEulerAngles = new Vector3(90f, 0f, 25f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Milk_Day11",
            sceneName = "OutSide",
            itemType = ItemType.StrawberryMilk,
            minDay = 11,
            localPosition = new Vector3(73.50f, 0.30f, -5.50f),
            localEulerAngles = new Vector3(90f, 0f, 0f)
        },
        new SecretItemSpot
        {
            spotId = "Secret_OutSide_Candy_Day13",
            sceneName = "OutSide",
            itemType = ItemType.Candy,
            minDay = 13,
            localPosition = new Vector3(33.00f, 0.30f, 63.50f),
            localEulerAngles = new Vector3(90f, 0f, 45f)
        }
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("DailySecretItemManager");
            go.AddComponent<DailySecretItemManager>();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        RefreshSecretItems();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSecretItems();
    }

    /// <summary>
    /// สั่งอัปเดตและสร้างไอเทมลับในฉากปัจจุบันตามวันที่ปัจจุบัน
    /// </summary>
    public void RefreshSecretItems()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(currentScene)) return;

        int today = 1;
        if (GameManagerSetup.Instance != null)
        {
            today = GameManagerSetup.Instance.currentDay;
        }
        else if (DayManager.Instance != null)
        {
            today = DayManager.Instance.currentDay;
        }

        // ค้นหาหรือสร้าง Container สำหรับเก็บไอเทมลับในฉากปัจจุบัน
        GameObject container = GameObject.Find("SecretItemsContainer");
        if (container == null)
        {
            container = new GameObject("SecretItemsContainer");
        }

        // โหลด Prefab ต้นแบบจาก Resources
        GameObject candyPrefab = Resources.Load<GameObject>("Item/Candy");
        GameObject milkPrefab = Resources.Load<GameObject>("Item/StrawberryMilf");

        if (candyPrefab == null)
        {
            candyPrefab = Resources.Load<GameObject>("Candy");
        }
        if (milkPrefab == null)
        {
            milkPrefab = Resources.Load<GameObject>("StrawberryMilf");
        }

        foreach (var spot in allSecretSpots)
        {
            if (!spot.sceneName.Equals(currentScene, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Transform existingInContainer = container != null ? container.transform.Find(spot.spotId) : null;
            GameObject itemObj = existingInContainer != null ? existingInContainer.gameObject : GameObject.Find(spot.spotId);

            // ตรวจสอบว่าเคยเก็บไอเทมชิ้นนี้ไปแล้วหรือยังในการเซฟนี้
            if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.IsItemPickedUp(spot.spotId))
            {
                if (itemObj != null) Destroy(itemObj);
                continue;
            }

            // ตรวจสอบว่าถึงวันที่ไอเทมชิ้นนี้จะปรากฏแล้วหรือไม่
            bool shouldSpawn = today >= spot.minDay;

            if (shouldSpawn)
            {
                if (itemObj == null)
                {
                    GameObject prefabToUse = spot.itemType == ItemType.Candy ? candyPrefab : milkPrefab;
                    if (prefabToUse != null)
                    {
                        itemObj = Instantiate(prefabToUse, spot.localPosition, Quaternion.Euler(spot.localEulerAngles), container.transform);
                        itemObj.name = spot.spotId;

                        // ตั้งค่าใน PickupItem
                        PickupItem pickup = itemObj.GetComponent<PickupItem>();
                        if (pickup != null)
                        {
                            var field = typeof(PickupItem).GetField("uniqueItemID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (field != null) field.SetValue(pickup, spot.spotId);
                        }

                        // ตั้งค่าใน ItemAppearanceController
                        ItemAppearanceController app = itemObj.GetComponent<ItemAppearanceController>();
                        if (app != null)
                        {
                            app.spawnFromDayOnward = true;
                            app.minDay = spot.minDay;
                        }

                        itemObj.SetActive(true);
                    }
                }
                else
                {
                    itemObj.SetActive(true);
                }
            }
            else
            {
                if (itemObj != null)
                {
                    itemObj.SetActive(false);
                }
            }
        }
    }
}
