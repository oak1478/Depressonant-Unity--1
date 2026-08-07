using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DayChatterGroup
{
    [Tooltip("ระบุวันของเกม เช่น วันที่ 1, วันที่ 2")]
    public int dayNumber = 1;

    [Header("ชุดคำพูดสุ่มสำหรับวันนี้")]
    public string[] chatterLines;

    [Header("ชุดคำพูดความเครียดสูงสำหรับวันนี้ (ถ้ามี)")]
    public string[] highStressLines;
}

public class WorldSpaceDialogue : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("ลาก GameObject แผงฟองคำพูด (Bubble Container) ในแคนวาสของตัวละครมาใส่ตรงนี้")]
    public GameObject bubbleContainer;

    [Tooltip("ลาก TextMeshProUGUI ในแผงคำพูดมาใส่ตรงนี้")]
    public TextMeshProUGUI textMesh;

    [Header("📅 Chatter By Day (คำพูดแบ่งตามรายวัน)")]
    [Tooltip("รายการชุดคำพูดสุ่มที่จะเปลี่ยนไปตามวันของเกม (ผูกกับ GameManagerSetup.Instance.currentDay)")]
    public List<DayChatterGroup> chatterByDay;

    [Header("Chatter Fallback (คำพูดสำรองทั่วไป)")]
    [Tooltip("ชุดคำพูดสุ่มสำรองกรณีไม่มีข้อมูลในวันนั้นๆ")]
    public string[] chatterLines;

    [Tooltip("ชุดคำพูดสำรองแยกตามระดับความเครียดพุ่งสูง")]
    public string[] highStressLines;

    [Header("Proximity Settings")]
    [Tooltip("เปิดใช้งานระบบแสดงฟองคำพูดเมื่อผู้เล่นเดินมาใกล้ระยะตรวจจับ")]
    public bool triggerOnProximity = true;
    public float triggerRadius = 3f;

    [Header("Timer Settings")]
    [Tooltip("เปิดใช้งานระบบสุ่มแสดงป๊อปอัพคำพูดลอยเองเรื่อยๆ ทุกๆ ช่วงเวลาที่สุ่มขึ้นมา (เหมาะสำหรับติดที่ตัวผู้เล่น หรือให้ NPC บ่นลอยๆ)")]
    public bool triggerOnTimer = false;
    public float minTimerInterval = 10f;
    public float maxTimerInterval = 20f;
    public float displayDuration = 3f;

    private bool isPlayerInside = false;
    private Coroutine activeBubbleCoroutine;
    private SphereCollider proximityCollider;

    void Start()
    {
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
        SetupProximitySensor();
        if (triggerOnTimer) StartCoroutine(TimerLoop());
    }

    void SetupProximitySensor()
    {
        if (triggerOnProximity)
        {
            proximityCollider = gameObject.GetComponent<SphereCollider>();
            if (proximityCollider == null)
            {
                proximityCollider = gameObject.AddComponent<SphereCollider>();
            }
            proximityCollider.isTrigger = true;
            proximityCollider.radius = triggerRadius;
        }
    }

    void LateUpdate()
    {
        // ⚡ [บิลบอร์ดเอฟเฟกต์] หมุนฟองคำพูดหันสู้กล้องหลักตลอดเวลาในฉาก 3D เพื่อให้อ่านออกได้จากทุกทิศทาง
        if (bubbleContainer != null && bubbleContainer.activeSelf && Camera.main != null)
        {
            bubbleContainer.transform.LookAt(bubbleContainer.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                             Camera.main.transform.rotation * Vector3.up);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnProximity && other.CompareTag("Player"))
        {
            isPlayerInside = true;
            ShowRandomBubble();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (triggerOnProximity && other.CompareTag("Player"))
        {
            isPlayerInside = false;
            HideBubble();
        }
    }

    public void ShowRandomBubble()
    {
        int today = 1;
        float currentStress = 0f;

        if (GameManagerSetup.Instance != null)
        {
            today = GameManagerSetup.Instance.currentDay;
            currentStress = GameManagerSetup.Instance.currentStress;
        }

        string selectedLine = "";

        // 1. ค้นหาชุดคำพูดสุ่มของวันปัจจุบันก่อน
        DayChatterGroup todaysGroup = null;
        if (chatterByDay != null && chatterByDay.Count > 0)
        {
            foreach (var group in chatterByDay)
            {
                if (group.dayNumber == today)
                {
                    todaysGroup = group;
                    break;
                }
            }
        }

        // 2. ดึงคำพูดจากชุดรายวัน
        if (todaysGroup != null)
        {
            if (currentStress >= 75f && todaysGroup.highStressLines != null && todaysGroup.highStressLines.Length > 0)
            {
                selectedLine = todaysGroup.highStressLines[Random.Range(0, todaysGroup.highStressLines.Length)];
            }
            else if (todaysGroup.chatterLines != null && todaysGroup.chatterLines.Length > 0)
            {
                selectedLine = todaysGroup.chatterLines[Random.Range(0, todaysGroup.chatterLines.Length)];
            }
        }

        // 3. หากไม่มีชุดคำพูดของวันนี้ ให้ถอยกลับไปใช้ชุดคำพูดสำรองกลาง (Fallback)
        if (string.IsNullOrEmpty(selectedLine))
        {
            if (currentStress >= 75f && highStressLines != null && highStressLines.Length > 0)
            {
                selectedLine = highStressLines[Random.Range(0, highStressLines.Length)];
            }
            else if (chatterLines != null && chatterLines.Length > 0)
            {
                selectedLine = chatterLines[Random.Range(0, chatterLines.Length)];
            }
        }

        if (!string.IsNullOrEmpty(selectedLine))
        {
            if (activeBubbleCoroutine != null) StopCoroutine(activeBubbleCoroutine);
            activeBubbleCoroutine = StartCoroutine(DisplayBubbleCoroutine(selectedLine));
        }
    }

    public void HideBubble()
    {
        if (activeBubbleCoroutine != null) StopCoroutine(activeBubbleCoroutine);
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
    }

    private IEnumerator DisplayBubbleCoroutine(string line)
    {
        if (bubbleContainer != null) bubbleContainer.SetActive(true);
        if (textMesh != null) textMesh.text = line;

        if (!triggerOnProximity || !isPlayerInside)
        {
            yield return new WaitForSeconds(displayDuration);
            if (bubbleContainer != null) bubbleContainer.SetActive(false);
        }
    }

    private IEnumerator TimerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimerInterval, maxTimerInterval));
            if (!isPlayerInside)
            {
                ShowRandomBubble();
                yield return new WaitForSeconds(displayDuration);
                HideBubble();
            }
        }
    }
}
