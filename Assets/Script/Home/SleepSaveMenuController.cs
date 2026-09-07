using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SleepSaveMenuController : MonoBehaviour
{
    public static SleepSaveMenuController Instance { get; private set; }

    [Header("UI Panels")]
    [Tooltip("Panel รวมทั้งหมดของหน้าต่างเลือกช่องเซฟ")]
    public GameObject saveMenuPanel;

    [Header("Save Slot Buttons (1 - 4)")]
    public Button[] slotButtons = new Button[4];
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[4];

    [Header("Cancel Button")]
    public Button cancelButton;

    [Header("Cinematic Day Transition")]
    [Tooltip("CanvasGroup สำหรับเฟดหน้าจอดำ")]
    public CanvasGroup fadeOverlayGroup;
    [Tooltip("ตัวหนังสือแสดงวันที่กึ่งกลางจอตอนจอดำ")]
    public TextMeshProUGUI dayNoticeText;
    [Tooltip("CanvasGroup สำหรับเฟดตัวหนังสือวันที่")]
    public CanvasGroup dayNoticeGroup;

    [Header("Audio Settings")]
    [Tooltip("เสียงคลิกยืนยันเลือกช่องเซฟ")]
    public AudioClip saveConfirmSound;
    [Tooltip("เสียงคลิกปุ่มทั่วไป")]
    public AudioClip buttonClickSound;
    [Tooltip("เสียงเลื่อนเมาส์ชี้ปุ่ม (Hover)")]
    public AudioClip buttonHoverSound;
    [Tooltip("เสียงตื่นนอน / นาฬิกาปลุก (นำไฟล์เสียงมาใส่เองได้ในอนาคต)")]
    public AudioClip wakeUpSound;

    private AudioSource audioSource;
    private bool isTransitioning = false;

    public bool IsMenuOpen => saveMenuPanel != null && saveMenuPanel.activeSelf;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
            audioSource.playOnAwake = false;
        }

        AutoFindReferences();
    }

    private void Start()
    {
        AutoFindReferences();

        // ซ่อนหน้าต่างเริ่มต้น
        if (saveMenuPanel != null) saveMenuPanel.SetActive(false);
        if (fadeOverlayGroup != null)
        {
            fadeOverlayGroup.alpha = 0f;
            fadeOverlayGroup.blocksRaycasts = false;
        }
        if (dayNoticeGroup != null)
        {
            dayNoticeGroup.alpha = 0f;
        }

        // ผูก Event ปุ่ม 1 - 4
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1;
            if (slotButtons[i] != null)
            {
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => OnSelectSlot(slotIndex));
                HookHoverSound(slotButtons[i]);
            }
        }

        // ผูกปุ่มยกเลิก (Back)
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CloseMenu);
            HookHoverSound(cancelButton);
        }
    }

    private void HookHoverSound(Button btn)
    {
        if (btn == null) return;
        UnityEngine.EventSystems.EventTrigger trigger = btn.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null) trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) =>
        {
            if (buttonHoverSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buttonHoverSound);
            }
        });
        trigger.triggers.Add(entry);
    }

    private void Update()
    {
        // กด ESC เพื่อปิดหน้าต่างได้หากยังไม่อยากนอน
        if (IsMenuOpen && !isTransitioning)
        {
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseMenu();
            }
        }
    }

    /// <summary>
    /// ค้นหา Reference ของ UI อัตโนมัติในกรณีที่ไม่ได้เชื่อมโยงผ่าน Inspector หรือสร้างขึ้นใหม่แบบไดนามิก
    /// </summary>
    public void AutoFindReferences()
    {
        if (saveMenuPanel == null)
        {
            Transform p = transform.Find("SlotSelectPanel");
            if (p == null) p = transform.Find("SaveMenuPanel");
            if (p != null) saveMenuPanel = p.gameObject;
        }

        if (saveMenuPanel != null)
        {
            if (cancelButton == null)
            {
                Transform backT = saveMenuPanel.transform.Find("Back");
                if (backT == null) backT = saveMenuPanel.transform.Find("Btn_Back");
                if (backT != null) cancelButton = backT.GetComponent<Button>();
            }

            for (int i = 0; i < 4; i++)
            {
                int slotIdx = i + 1;
                Transform slotT = saveMenuPanel.transform.Find($"Slot_{slotIdx}");
                if (slotT != null)
                {
                    if (slotButtons[i] == null) slotButtons[i] = slotT.GetComponent<Button>();
                    if (slotTexts[i] == null) slotTexts[i] = slotT.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        if (fadeOverlayGroup == null)
        {
            Transform fadeT = transform.Find("FadeOverlay");
            if (fadeT != null) fadeOverlayGroup = fadeT.GetComponent<CanvasGroup>();
        }

        if (dayNoticeGroup == null)
        {
            Transform noticeT = transform.Find("FadeOverlay/DayNoticeContainer");
            if (noticeT != null) dayNoticeGroup = noticeT.GetComponent<CanvasGroup>();
        }

        if (dayNoticeText == null)
        {
            Transform noticeT = transform.Find("FadeOverlay/DayNoticeContainer");
            if (noticeT != null) dayNoticeText = noticeT.GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// เปิดหน้าต่างเลือกช่องเซฟเมื่อผู้เล่นกดเข้านอน
    /// </summary>
    public void OpenMenu()
    {
        if (isTransitioning) return;

        AutoFindReferences();

        if (saveMenuPanel == null)
        {
            Debug.LogError("[SleepSaveMenuController] saveMenuPanel เป็น null! ไม่สามารถเปิดหน้าต่างเซฟได้", this);
            return;
        }

        UpdateSlotDisplay();

        saveMenuPanel.SetActive(true);

        // ปลดล็อกเมาส์ให้ผู้เล่นคลิกเลือกสล็อต
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// ปิดหน้าต่างและกลับไปเดินเล่นต่อ
    /// </summary>
    public void CloseMenu()
    {
        if (isTransitioning) return;

        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        if (saveMenuPanel != null) saveMenuPanel.SetActive(false);

        // ล็อกเมาส์กลับเข้าสู่มุมมองบุคคลที่หนึ่ง
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// อัปเดตรายละเอียดข้อมูลของแต่ละสล็อต 1 - 4
    /// </summary>
    public void UpdateSlotDisplay()
    {
        SaveSystem saveSys = SaveSystem.Instance;
        if (saveSys == null) saveSys = UnityEngine.Object.FindAnyObjectByType<SaveSystem>();

        for (int i = 0; i < 4; i++)
        {
            int slotIdx = i + 1;
            if (slotTexts[i] == null) continue;

            if (saveSys != null && saveSys.HasSaveFile(slotIdx))
            {
                SaveData data = saveSys.LoadGame(slotIdx);
                if (data != null)
                {
                    string sceneName = GetSceneName(data.currentSceneIndex);
                    string playTimeStr = FormatPlayTime(data.playTime);

                    slotTexts[i].text = $"Nia\t\t\t\t{sceneName}\nDAY {data.currentDay}\t\t\t\tPlay Time {playTimeStr}";
                }
            }
            else
            {
                slotTexts[i].text = $"SLOT {slotIdx}\t\t\t\t[ EMPTY ]\nDAY -\t\t\t\tPlay Time -";
            }

            if (slotButtons[i] != null)
            {
                slotButtons[i].interactable = true;
            }
        }
    }

    /// <summary>
    /// เมื่อคลิกเลือกช่องสล็อตเพื่อเซฟและเข้านอน
    /// </summary>
    private void OnSelectSlot(int slotIndex)
    {
        if (isTransitioning) return;

        // เล่นเสียงยืนยัน
        if (saveConfirmSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(saveConfirmSound);
        }
        else if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // ปิดหน้าต่างเลือกสล็อตทันที
        if (saveMenuPanel != null) saveMenuPanel.SetActive(false);

        // เริ่มต้นกระบวนการเข้านอนและข้ามวัน
        StartCoroutine(SleepAndDayTransitionRoutine(slotIndex));
    }

    /// <summary>
    /// โคโรทีนสำหรับเฟดจอดำ แสดงตัวหนังสือวันที่กลางจอ และสั่งข้ามวัน
    /// </summary>
    private IEnumerator SleepAndDayTransitionRoutine(int slotIndex)
    {
        isTransitioning = true;

        // ล็อกเมาส์
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 1. เฟดหน้าจอดำ (Fade Out)
        float duration = 1.0f;
        float elapsed = 0f;
        if (fadeOverlayGroup != null)
        {
            fadeOverlayGroup.blocksRaycasts = true;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlayGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            fadeOverlayGroup.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.3f);

        // คำนวณวันถัดไป
        DayManager dayManager = DayManager.Instance;
        if (dayManager == null) dayManager = UnityEngine.Object.FindAnyObjectByType<DayManager>();
        
        int currentDayBefore = (dayManager != null) ? dayManager.currentDay : (GameManagerSetup.Instance != null ? GameManagerSetup.Instance.currentDay : 1);
        int nextDay = currentDayBefore + 1;
        Debug.Log($"[SleepSaveMenu] กำลังเข้านอน: วันปัจจุบันในเกมคือ Day {currentDayBefore} ➔ ข้ามไปยัง Day {nextDay}");

        // 2. แสดงตัวหนังสือวันที่กลางจอ (เช่น "DAY 2")
        if (dayNoticeText != null)
        {
            dayNoticeText.text = $"DAY {nextDay}";
        }

        if (dayNoticeGroup != null)
        {
            // เฟดตัวหนังสือวันที่ขึ้นมา
            elapsed = 0f;
            while (elapsed < 0.6f)
            {
                elapsed += Time.deltaTime;
                dayNoticeGroup.alpha = Mathf.Clamp01(elapsed / 0.6f);
                yield return null;
            }
            dayNoticeGroup.alpha = 1f;

            // ให้ผู้เล่นอ่านข้อความสักครู่
            yield return new WaitForSeconds(1.5f);

            // เฟดตัวหนังสือวันที่จางหายไป
            elapsed = 0f;
            while (elapsed < 0.6f)
            {
                elapsed += Time.deltaTime;
                dayNoticeGroup.alpha = 1f - Mathf.Clamp01(elapsed / 0.6f);
                yield return null;
            }
            dayNoticeGroup.alpha = 0f;
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        // 3. สั่งระบบข้ามวันจริง
        if (dayManager != null)
        {
            dayManager.GoToNextDay();
        }

        // 4. บันทึกข้อมูลลงในสล็อตที่เลือกพร้อมวันใหม่
        SaveSystem saveSys = SaveSystem.Instance;
        if (saveSys == null) saveSys = UnityEngine.Object.FindAnyObjectByType<SaveSystem>();

        if (saveSys != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = GameObject.Find("Player");
            if (player == null)
            {
                CharacterController cc = UnityEngine.Object.FindAnyObjectByType<CharacterController>();
                if (cc != null) player = cc.gameObject;
            }

            // หากหาตัวละครไม่เจอจริง ๆ ให้ใช้พิกัดห้องนอนแทน Vector3.zero เพื่อป้องกันตกแมพ
            Vector3 playerPos = player != null ? player.transform.position : new Vector3(-0.808f, 1f, -6.931f);
            int currentSceneIdx = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            if (GameManagerSetup.Instance != null)
            {
                GameManagerSetup.Instance.activeSaveSlot = slotIndex;
            }

            // บันทึกเข้าสล็อต
            saveSys.SaveGameFromGlobal(slotIndex, playerPos, currentSceneIdx);
            Debug.Log($"[SleepSaveMenu] บันทึกเกมลงสล็อต {slotIndex} เรียบร้อย! (วันปัจจุบัน: {nextDay})");
        }

        // แจ้งระบบ Tutorial เมื่อเข้านอน
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnSleptInBed();
        }

        // 5. เล่นเสียงนาฬิกาปลุก / ตื่นนอน (ถ้ามี)
        if (wakeUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(wakeUpSound);
            yield return new WaitForSeconds(0.5f);
        }

        // 6. เฟดหน้าจอกลับมาสว่าง (Fade In)
        elapsed = 0f;
        if (fadeOverlayGroup != null)
        {
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlayGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            fadeOverlayGroup.alpha = 0f;
            fadeOverlayGroup.blocksRaycasts = false;
        }

        isTransitioning = false;
        Debug.Log($"[SleepSaveMenu] ตื่นนอนรับวันใหม่เรียบร้อย! ตอนนี้คือวันที่ {nextDay}");
    }

    private string GetSceneName(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 1: return "HOME TOWN";
            case 2: return "MY ROOM";
            case 3: return "SCHOOL";
            default: return "MY ROOM";
        }
    }

    private string FormatPlayTime(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
    }
}
