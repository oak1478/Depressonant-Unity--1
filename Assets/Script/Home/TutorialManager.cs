using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum TutorialStep
{
    WASD_Movement = 0,
    LookInMirror = 1,
    Open_Door = 2,
    Pickup_Item = 3,
    Inventory_UseItem = 4,
    TalkTo_Mom = 5,
    SleepInBed = 6,
    Completed = 7
}

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager _instance;
    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<TutorialManager>();
            }
            return _instance;
        }
    }

    [Header("UI Reference")]
    public TutorialUI tutorialUI;

    [Header("Current Progress")]
    public TutorialStep currentStep = TutorialStep.WASD_Movement;
    public bool isTutorialActive = true;

    [Header("Step 1 WASD Holding Settings")]
    public float wasdRequiredHoldTime = 1.5f;
    private float wasdCurrentHoldTime = 0f;

    private bool hasSwitchedToInventoryUsePrompt = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
            return;
        }

        EnsureUIReference();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        EnsureUIReference();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public void EnsureUIReference()
    {
        if (tutorialUI == null)
        {
            tutorialUI = GetComponentInChildren<TutorialUI>(true);
        }
        if (tutorialUI == null)
        {
            tutorialUI = Object.FindAnyObjectByType<TutorialUI>(FindObjectsInactive.Include);
        }

        if (tutorialUI != null)
        {
            Canvas tutCanvas = tutorialUI.GetComponentInParent<Canvas>();
            if (tutCanvas != null)
            {
                tutCanvas.overrideSorting = true;
                tutCanvas.sortingOrder = 12;
            }
        }
    }

    public void SkipTutorial()
    {
        isTutorialActive = false;
        currentStep = TutorialStep.Completed;
        if (tutorialUI != null)
        {
            tutorialUI.HideTutorial();
        }
        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.SetVisible(true);
        }
        Debug.Log("[TutorialManager] ข้ามช่วงสอนเล่น (Tutorial) สำเร็จ");
    }

    /// <summary>
    /// รีเซ็ตสถานะของระบบสอนเล่นทั้งหมดสำหรับเริ่มเล่นเกมใหม่ (New Game)
    /// </summary>
    public void ResetTutorial()
    {
        Debug.Log("🔄 [TutorialManager] ทำการรีเซ็ตระบบสอนเล่นสำหรับเริ่มเกมใหม่ (Day 1)");
        currentStep = TutorialStep.WASD_Movement;
        isTutorialActive = true;
        wasdCurrentHoldTime = 0f;
        hasSwitchedToInventoryUsePrompt = false;

        EnsureUIReference();
        if (tutorialUI != null)
        {
            tutorialUI.SetProgress(0f);
            tutorialUI.HideTutorial();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureUIReference();

        // 1. ถ้าอยู่ในหน้าเมนูหลัก ให้ซ่อนหน้าต่าง Tutorial เสมอ
        if (scene.name == "MainMenu")
        {
            if (tutorialUI != null) tutorialUI.HideTutorial();
            return;
        }

        // 2. ทำงานเฉพาะวันแรกเท่านั้น (Day 1) หากเกินวันแรก ให้ปิดระบบสอนทั้งหมด
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.currentDay > 1)
        {
            isTutorialActive = false;
            if (tutorialUI != null) tutorialUI.HideTutorial();
            return;
        }

        // 3. หากระบบสอนปิดอยู่ หรือผ่านครบทุกขั้นตอนแล้ว ให้ซ่อน UI
        if (!isTutorialActive || currentStep == TutorialStep.Completed)
        {
            if (tutorialUI != null) tutorialUI.HideTutorial();
            return;
        }

        // 4. หากโหลดเข้าฉากบ้าน (Home) แล้วยังค้างอยู่สเต็ปเปิดประตู ให้ผ่านสเต็ปเปิดประตูอัตโนมัติ
        if (scene.name == "Home" && currentStep == TutorialStep.Open_Door)
        {
            Debug.Log("🚪 [TutorialManager] ข้ามเข้าฉาก Home สำเร็จ! สั่งผ่านสเต็ปเปิดประตูอัตโนมัติ");
            CompleteStep(TutorialStep.Open_Door);
            return;
        }

        // 5. แสดงผล UI ตามสเต็ปปัจจุบัน
        SetStep(currentStep);
    }

    void Update()
    {
        if (!isTutorialActive || tutorialUI == null) return;

        switch (currentStep)
        {
            case TutorialStep.WASD_Movement:
                HandleWASDStep();
                break;

            case TutorialStep.Inventory_UseItem:
                HandleInventoryStep();
                break;
        }
    }

    void HandleWASDStep()
    {
        if (Keyboard.current == null || (DevConsole.Instance != null && DevConsole.Instance.IsOpen)) return;

        bool isMoving = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || 
                        Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed ||
                        Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed ||
                        Keyboard.current.downArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        if (isMoving)
        {
            wasdCurrentHoldTime += Time.unscaledDeltaTime;
            float progress = wasdCurrentHoldTime / wasdRequiredHoldTime;
            if (tutorialUI != null) tutorialUI.SetProgress(progress);

            if (wasdCurrentHoldTime >= wasdRequiredHoldTime)
            {
                CompleteStep(TutorialStep.WASD_Movement);
            }
        }
    }

    void HandleInventoryStep()
    {
        // ⚡ เมื่อเปิดกระเป๋า (TAB) แล้ว เปลี่ยนโจทย์เป็นคำสั่งให้คลิกปุ่ม USE ที่ขนม
        if (!hasSwitchedToInventoryUsePrompt && InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.inventoryPanel != null && InventoryManager.Instance.inventoryPanel.activeSelf)
            {
                hasSwitchedToInventoryUsePrompt = true;
                if (tutorialUI != null)
                {
                    tutorialUI.ShowTutorial("คลิกปุ่ม USE ที่ขนมในกระเป๋าเพื่อกิน", new string[] { });
                }
            }
        }
    }

    public void SetStep(TutorialStep step)
    {
        currentStep = step;
        if (!isTutorialActive) return;

        EnsureUIReference();

        switch (step)
        {
            case TutorialStep.WASD_Movement:
                wasdCurrentHoldTime = 0f;
                if (tutorialUI != null)
                {
                    tutorialUI.SetProgress(0f);
                    tutorialUI.ShowTutorial("กดปุ่ม W, A, S, D เพื่อเคลื่อนที่สำรวจห้อง", new string[] { "W", "A", "S", "D" });
                }
                break;

            case TutorialStep.LookInMirror:
                if (tutorialUI != null) tutorialUI.ShowTutorial("เดินไปที่กระจกแล้วกดปุ่ม F เพื่อส่องกระจก", new string[] { "F" });
                break;

            case TutorialStep.Open_Door:
                if (tutorialUI != null) tutorialUI.ShowTutorial("เดินไปที่ประตูแล้วกดปุ่ม F เพื่อเปิดประตูออกจากห้อง", new string[] { "F" });
                break;

            case TutorialStep.Pickup_Item:
                if (tutorialUI != null) tutorialUI.ShowTutorial("เดินไปใกล้ขนมแล้วกดปุ่ม F เพื่อเก็บของ", new string[] { "F" });
                break;

            case TutorialStep.Inventory_UseItem:
                hasSwitchedToInventoryUsePrompt = false;
                if (tutorialUI != null) tutorialUI.ShowTutorial("กดปุ่ม TAB เพื่อเปิดกระเป๋า", new string[] { "TAB" });
                break;

            case TutorialStep.TalkTo_Mom:
                if (tutorialUI != null) tutorialUI.ShowTutorial("เดินไปใกล้แม่ (Mom) แล้วกดปุ่ม E เพื่อเริ่มคุย (การเลือกตอบจะมีผลต่อระดับความเครียดของเนีย Stress +/-)", new string[] { "E" });
                break;

            case TutorialStep.SleepInBed:
                if (tutorialUI != null) tutorialUI.ShowTutorial("เดินไปที่เตียงนอนแล้วกดปุ่ม F เพื่อเข้านอนจบวันที่ 1", new string[] { "F" });
                break;

            case TutorialStep.Completed:
                isTutorialActive = false;
                if (tutorialUI != null) tutorialUI.HideTutorial();
                Debug.Log("🎉 [TutorialManager] ทำภารกิจการสอนวันที่ 1 (Tutorial) สำเร็จครบถ้วน!");
                break;
        }
    }

    public void CompleteStep(TutorialStep completedStep)
    {
        if (currentStep == completedStep)
        {
            Debug.Log($"[TutorialManager] สเต็ป {completedStep} สำเร็จ! ขยับไปสเต็ปถัดไป");
            int nextStepIndex = (int)currentStep + 1;
            SetStep((TutorialStep)nextStepIndex);
        }
    }

    // --- Hooks สำหรับเชื่อมต่อกับสคริปต์โต้ตอบต่างๆ ---

    public void OnLookedInMirror()
    {
        if (currentStep == TutorialStep.LookInMirror)
        {
            CompleteStep(TutorialStep.LookInMirror);
        }
    }

    public void OnDoorOpened()
    {
        if (currentStep == TutorialStep.Open_Door)
        {
            CompleteStep(TutorialStep.Open_Door);
        }
    }

    public void OnItemPickedUp()
    {
        if (currentStep == TutorialStep.Pickup_Item)
        {
            CompleteStep(TutorialStep.Pickup_Item);
        }
    }

    public void OnItemUsed()
    {
        if (currentStep == TutorialStep.Inventory_UseItem)
        {
            CompleteStep(TutorialStep.Inventory_UseItem);
        }
    }

    public void OnTalkedToMom()
    {
        if (currentStep == TutorialStep.TalkTo_Mom)
        {
            CompleteStep(TutorialStep.TalkTo_Mom);
        }
    }

    public void OnDialogueFinished()
    {
        if (currentStep == TutorialStep.TalkTo_Mom)
        {
            CompleteStep(TutorialStep.TalkTo_Mom);
        }
    }

    public void OnSleptInBed()
    {
        if (currentStep == TutorialStep.SleepInBed)
        {
            CompleteStep(TutorialStep.SleepInBed);
        }
    }
}
