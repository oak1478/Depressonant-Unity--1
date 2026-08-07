using UnityEngine;
using System.Collections.Generic;

public enum FloatingTextTriggerMode
{
    OnSceneStart = 0,
    OnEnterArea = 1,
    OnInteract = 2
}

[System.Serializable]
public class DailyThought
{
    [Tooltip("ระบุวันปัจจุบันของเกม เช่น วันที่ 1, วันที่ 2")]
    public int dayNumber = 1;

    [Tooltip("โหมดการทำงาน: OnSceneStart (ขึ้นตอนโหลดฉาก), OnEnterArea (ขึ้นเมื่อเดินเข้าใกล้), OnInteract (ขึ้นเมื่อกดปุ่ม F)")]
    public FloatingTextTriggerMode triggerMode = FloatingTextTriggerMode.OnEnterArea;

    [Tooltip("รัศมีระยะทางสำหรับโหมด OnEnterArea (หน่วยเป็นเมตร) สามารถกำหนดได้อย่างอิสระ")]
    public float triggerRadius = 2.5f;

    [Tooltip("ระยะเวลาแสดงผลข้อความลอยบนหน้าจอ (หน่วยเป็นวินาที)")]
    public float displayDuration = 3.5f;

    [TextArea(2, 5)]
    [Tooltip("ข้อความในใจลอยในฉาก 3D เช่น 'เนีย: ...ฝนตกซาลงแล้วแฮะ'")]
    public string thoughtMessage = "เนีย: ...";

    [Tooltip("ตำแหน่งความสูง/ทิศทางที่ข้อความจะลอยขึ้นจากวัตถุนี้")]
    public Vector3 spawnOffset = new Vector3(0, 1.5f, 0);

    [Tooltip("แสดงเพียงครั้งเดียวต่อวัน (ไม่ขึ้นซ้ำถ้าเดินเข้าออกโซนเดิมในวันเดียวกัน)")]
    public bool triggerOncePerDay = true;

    [HideInInspector] public bool hasTriggeredToday = false;
}

public class FloatingTextTrigger : MonoBehaviour
{
    [Header("📅 รายการข้อความในใจแบ่งตามรายวัน (Daily Thoughts)")]
    public List<DailyThought> dailyThoughts = new List<DailyThought>();

    [Header("Gizmos Visual Settings")]
    public bool showGizmosInScene = true;
    public Color gizmosColor = new Color(0f, 0.9f, 1f, 0.4f);

    private Transform playerTransform;

    void Start()
    {
        FindPlayer();
        CheckSceneStartTriggers();
    }

    void Update()
    {
        int today = GetCurrentDay();

        // ⚡ ตรวจจับโหมด OnEnterArea (เมื่อผู้เล่นเดินเข้าใกล้ในรัศมีที่กำหนด)
        foreach (var thought in dailyThoughts)
        {
            if (thought.dayNumber == today && thought.triggerMode == FloatingTextTriggerMode.OnEnterArea)
            {
                if (thought.triggerOncePerDay && thought.hasTriggeredToday) continue;

                if (playerTransform == null) FindPlayer();
                if (playerTransform != null)
                {
                    float dist = Vector3.Distance(playerTransform.position, transform.position);
                    if (dist <= thought.triggerRadius)
                    {
                        TriggerThought(thought);
                    }
                }
            }
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private int GetCurrentDay()
    {
        if (GameManagerSetup.Instance != null)
        {
            return GameManagerSetup.Instance.currentDay;
        }
        return 1;
    }

    private void CheckSceneStartTriggers()
    {
        int today = GetCurrentDay();
        foreach (var thought in dailyThoughts)
        {
            if (thought.dayNumber == today && thought.triggerMode == FloatingTextTriggerMode.OnSceneStart)
            {
                if (thought.triggerOncePerDay && thought.hasTriggeredToday) continue;
                TriggerThought(thought);
            }
        }
    }

    // ⚡ ตรวจจับเมื่อผู้เล่นเดินเข้ามาชนกับ Trigger Collider 3D โดยตรง
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int today = GetCurrentDay();
            foreach (var thought in dailyThoughts)
            {
                if (thought.dayNumber == today && thought.triggerMode == FloatingTextTriggerMode.OnEnterArea)
                {
                    if (thought.triggerOncePerDay && thought.hasTriggeredToday) continue;
                    Debug.Log($"🚨 [OnTriggerEnter] ตัวละครชนกับ Trigger Box ของวัตถุ '{gameObject.name}'!");
                    TriggerThought(thought);
                }
            }
        }
    }

    // ฟังก์ชันโต้ตอบเมื่อกดปุ่ม F (สำหรับเรียกใช้จาก BedroomInteractable หรือสคริปต์โต้ตอบ)
    public void TriggerInteract()
    {
        int today = GetCurrentDay();
        foreach (var thought in dailyThoughts)
        {
            if (thought.dayNumber == today && thought.triggerMode == FloatingTextTriggerMode.OnInteract)
            {
                if (thought.triggerOncePerDay && thought.hasTriggeredToday) continue;
                Debug.Log($"🚨 [TriggerInteract] กดปุ่มโต้ตอบกับวัตถุ '{gameObject.name}'!");
                TriggerThought(thought);
            }
        }
    }

    public void TriggerThought(DailyThought thought)
    {
        if (thought == null || string.IsNullOrEmpty(thought.thoughtMessage)) return;

        thought.hasTriggeredToday = true;
        Vector3 spawnPos = transform.position + thought.spawnOffset;

        Floating3DTextManager.Instance.SpawnFloatingText(spawnPos, thought.thoughtMessage, thought.displayDuration);

        // ⚡ [Debug Log] แจ้งเตือนลงหน้าต่าง Console ชัดเจนเมื่อเดินชนโซนพื้นที่หรือทริกเกอร์ข้อความลอย
        Debug.Log($"🚨 [3D Thought Trigger] เดินเข้าโซนวัตถุ '{gameObject.name}' สำเร็จ! | วันที่: {thought.dayNumber} | ข้อความ: \"{thought.thoughtMessage}\" | แสดงนาน: {thought.displayDuration} วินาที");
    }

    // ⚡ วาดวงกลมรัศมีใน Scene View ของ Unity เพื่อให้ปรับขนาดระยะทางได้ง่ายแบบ Visual
    void OnDrawGizmosSelected()
    {
        if (!showGizmosInScene) return;

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, 1.0f);

        if (dailyThoughts != null)
        {
            foreach (var thought in dailyThoughts)
            {
                if (thought.triggerMode == FloatingTextTriggerMode.OnEnterArea)
                {
                    Gizmos.DrawWireSphere(transform.position, thought.triggerRadius);
                }
            }
        }
    }
}
