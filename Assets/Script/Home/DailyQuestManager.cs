using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// ควบคุม Quest Canvas ที่แสดงรายชื่อ NPC ที่ยังมีเรื่องราวใหม่ที่ยังไม่ได้คุยในวันนี้
/// วางสคริปต์นี้ไว้ที่ออบเจกต์ใดก็ได้ในฉาก แล้วลาก Reference ใน Inspector ให้ครบ
/// </summary>
public class DailyQuestManager : MonoBehaviour
{
    public static DailyQuestManager Instance;

    [Header("UI References")]
    [Tooltip("ลาก NPCListContainer (ออบเจกต์ที่มี Vertical Layout Group) มาใส่")]
    public Transform npcListContainer;

    [Tooltip("ลาก Prefab ของแถว NPC แต่ละแถวมาใส่")]
    public GameObject npcRowPrefab;

    [Tooltip("ลาก TitleText หรือ QuestPanel มาใส่ เพื่อซ่อนเมื่อคุยครบแล้ว (ใส่หรือไม่ใส่ก็ได้)")]
    public GameObject titleObjectToHide;

    [Header("Refresh Settings")]
    [Tooltip("อัปเดตรายชื่อทุกกี่วินาที (ตั้ง 1 ก็เพียงพอ ไม่ต้องทุกเฟรม)")]
    public float refreshInterval = 1f;

    private float refreshTimer = 0f;

    // เก็บแถวที่สร้างไว้เพื่อ destroy เมื่อ refresh
    private List<GameObject> activeRows = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        RefreshQuestList();
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            RefreshQuestList();
        }
    }

    public void RefreshQuestList()
    {
        if (npcListContainer == null || npcRowPrefab == null) return;

        int today = GetCurrentDay();

        // หา NPC ทุกตัวในฉากที่มี NPCInteraction
        NPCInteraction[] allNPCInteractions = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // สร้างลิสต์ NPC ที่มี Story ใหม่วันนี้และยังไม่ได้คุย
        List<string> availableNPCs = new List<string>();

        foreach (NPCInteraction npc in allNPCInteractions)
        {
            if (npc == null) continue;

            // ข้ามถ้าคุยไปแล้วในวันนี้
            if (npc.lastTalkedDay == today) continue;

            // เช็คว่ามี Story วันนี้ไหม
            bool hasStoryToday = false;
            foreach (var dialog in npc.dialoguesByDay)
            {
                if (dialog.dayNumber == today)
                {
                    hasStoryToday = true;
                    break;
                }
            }

            if (hasStoryToday)
            {
                availableNPCs.Add(npc.npcDisplayName);
            }
        }

        // ล้างแถวเก่าออก
        foreach (var row in activeRows)
        {
            if (row != null) Destroy(row);
        }
        activeRows.Clear();

        if (availableNPCs.Count == 0)
        {
            // ซ่อน TitleText หรือตัวแผง Panel ถ้ายัดมา
            if (titleObjectToHide != null) titleObjectToHide.SetActive(false);

            // สร้างแถวแจ้งเตือนว่าคุยครบแล้ว
            GameObject row = Instantiate(npcRowPrefab, npcListContainer);
            activeRows.Add(row);
            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = "พูดคุยครบแล้ว!";
                label.fontStyle = FontStyles.Italic;
                label.color = Color.gray; // ทำให้สีดูจางลงนิดนึง
            }
        }
        else
        {
            // เปิด TitleText กลับมาเมื่อมีคนให้คุย
            if (titleObjectToHide != null) titleObjectToHide.SetActive(true);

            // สร้างแถวใหม่ตามจำนวน NPC ที่พบ (ไม่เกิน 6 คน)
            for (int i = 0; i < Mathf.Min(availableNPCs.Count, 6); i++)
            {
                GameObject row = Instantiate(npcRowPrefab, npcListContainer);
                activeRows.Add(row);

                // หา TextMeshPro ในแถวแล้วกำหนดชื่อ
                TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = "- " + availableNPCs[i];
                }
            }
        }
    }

    private int GetCurrentDay()
    {
        if (DayManager.Instance != null) return DayManager.Instance.currentDay;
        if (GameManagerSetup.Instance != null) return GameManagerSetup.Instance.currentDay;
        return 1;
    }

    /// <summary>
    /// เรียกฟังก์ชันนี้จากภายนอก (เช่น หลังคุยกับ NPC เสร็จ) เพื่ออัปเดตรายชื่อทันที
    /// </summary>
    public void ForceRefresh()
    {
        RefreshQuestList();
    }
}
