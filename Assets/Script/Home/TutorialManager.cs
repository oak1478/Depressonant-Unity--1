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
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ทำงานเฉพาะวันแรกเท่านั้น (Day 1)
        if (GameManagerSetup.Instance != null && GameManagerSetup.Instance.currentDay > 1)
        {
            isTutorialActive = false;
            if (tutorialUI != null) tutorialUI.HideTutorial();
            return;
        }

        // ⚡ หากโหลดเข้าฉากบ้าน (Home) แล้วยังค้างอยู่สเต็ปเปิดประตู ให้ผ่านสเต็ปเปิดประตูอัตโนมัติ
        string activeScene = SceneManager.GetActiveScene().name;
        if (currentStep == TutorialStep.Open_Door && activeScene != "Bedroom_3D")
        {
            Debug.Log("🚪 [TutorialManager] ข้ามเข้าฉาก Home สำเร็จ! สั่งผ่านสเต็ปเปิดประตูอัตโนมัติ");
            CompleteStep(TutorialStep.Open_Door);
            return;
        }

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
        if (Keyboard.current == null) return;

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
