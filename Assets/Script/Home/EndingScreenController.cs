using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class EndingScreenController : MonoBehaviour
{
    private static EndingScreenController instance;
    public static EndingScreenController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindAnyObjectByType<EndingScreenController>();
                if (instance == null)
                {
                    GameObject go = new GameObject("[EndingScreenController]");
                    instance = go.AddComponent<EndingScreenController>();
                }
            }
            return instance;
        }
    }

    private bool isEndingActive = false;
    private bool canAcceptInput = false;
    private bool isTransitioningToMenu = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private GameObject canvasObj;
    private Image blackOverlay;
    private CanvasGroup contentCanvasGroup;
    private TextMeshProUGUI hintText;

    /// <summary>
    /// สั่งเริ่มกระบวนการแสดงฉากจบ (จอดำ + The End Good / Bad / Normal)
    /// </summary>
    public static void ShowEnding(string sceneName = null)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        Instance.TriggerEndingSequence(sceneName);
    }

    public void TriggerEndingSequence(string sceneName)
    {
        if (isEndingActive) return;
        isEndingActive = true;

        // 1. ล็อกการเคลื่อนที่และการกระทำของผู้เล่นทั้งหมด
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        CharacterController cc = Object.FindAnyObjectByType<CharacterController>();
        if (cc != null) cc.enabled = false;

        PauseMenuController pause = Object.FindAnyObjectByType<PauseMenuController>();
        if (pause != null) pause.enabled = false;

        // 2. ซ่อนหน้าต่างเควสประจำวัน
        if (DailyQuestManager.Instance != null)
        {
            DailyQuestManager.Instance.SetVisible(false);
        }

        // 3. ปลดล็อกเคอร์เซอร์เมาส์และตั้งค่าความเร็วเวลาปกติ
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 4. สร้าง Canvas และ UI ฉากจบ
        BuildEndingUI(sceneName);

        // 5. เริ่ม Coroutine อนิเมชัน Fade ฉากจบ
        StartCoroutine(EndingAnimationRoutine(sceneName));
    }

    private void BuildEndingUI(string sceneName)
    {
        TMP_FontAsset font = GetBestFont();

        // Canvas หลัก (อยู่บนสุดของทุกองค์ประกอบ sortingOrder = 32767)
        canvasObj = new GameObject("EndingScreen_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // 1. พื้นหลังสีดำ (Black Overlay)
        GameObject bgObj = new GameObject("BlackOverlay", typeof(RectTransform));
        bgObj.transform.SetParent(canvasObj.transform, false);
        bgObj.AddComponent<CanvasRenderer>();
        blackOverlay = bgObj.AddComponent<Image>();
        blackOverlay.color = new Color(0f, 0f, 0f, 0f); // เริ่มต้นที่โปร่งใส 0
        blackOverlay.raycastTarget = true;

        RectTransform bgRt = bgObj.GetComponent<RectTransform>() ?? bgObj.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        bgRt.anchoredPosition = Vector2.zero;

        // 2. คอนเทนเนอร์รวมข้อความและปุ่ม (Ending Content)
        GameObject contentObj = new GameObject("EndingContent", typeof(RectTransform));
        contentObj.transform.SetParent(canvasObj.transform, false);
        contentCanvasGroup = contentObj.AddComponent<CanvasGroup>();
        contentCanvasGroup.alpha = 0f; // เริ่มต้นที่โปร่งใส 0

        RectTransform contentRt = contentObj.GetComponent<RectTransform>() ?? contentObj.AddComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.sizeDelta = Vector2.zero;
        contentRt.anchoredPosition = Vector2.zero;

        // 3. ข้อความหลัก "THE END"
        GameObject titleObj = new GameObject("TitleText", typeof(RectTransform));
        titleObj.transform.SetParent(contentObj.transform, false);
        titleObj.AddComponent<CanvasRenderer>();
        TextMeshProUGUI titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
        if (font != null) titleTmp.font = font;
        titleTmp.text = "THE END";
        titleTmp.fontSize = 88;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        titleTmp.raycastTarget = false;

        RectTransform titleRt = titleObj.GetComponent<RectTransform>() ?? titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.pivot = new Vector2(0.5f, 0.5f);
        titleRt.sizeDelta = new Vector2(1000, 120);
        titleRt.anchoredPosition = new Vector2(0, 90);

        // 4. ข้อความรองระบุตอนจบ (Good / Bad / Normal Ending)
        GameObject subtitleObj = new GameObject("SubtitleText", typeof(RectTransform));
        subtitleObj.transform.SetParent(contentObj.transform, false);
        subtitleObj.AddComponent<CanvasRenderer>();
        TextMeshProUGUI subtitleTmp = subtitleObj.AddComponent<TextMeshProUGUI>();
        if (font != null) subtitleTmp.font = font;

        string subText = "Good Ending";
        Color subColor = new Color(0.48f, 0.93f, 0.62f, 1f); // เขียวมิ้นต์สดใส (#7bed9f)

        if (sceneName.IndexOf("Bad", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            subText = "Bad Ending";
            subColor = new Color(1f, 0.42f, 0.51f, 1f); // แดงกุหลาบ (#ff6b81)
        }
        else if (sceneName.IndexOf("Normal", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            subText = "Normal Ending";
            subColor = new Color(0.44f, 0.63f, 1f, 1f); // ฟ้าอ่อนละมุน (#70a1ff)
        }

        subtitleTmp.text = subText;
        subtitleTmp.fontSize = 50;
        subtitleTmp.fontStyle = FontStyles.Bold;
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.color = subColor;
        subtitleTmp.raycastTarget = false;

        RectTransform subRt = subtitleObj.GetComponent<RectTransform>() ?? subtitleObj.AddComponent<RectTransform>();
        subRt.anchorMin = new Vector2(0.5f, 0.5f);
        subRt.anchorMax = new Vector2(0.5f, 0.5f);
        subRt.pivot = new Vector2(0.5f, 0.5f);
        subRt.sizeDelta = new Vector2(1000, 80);
        subRt.anchoredPosition = new Vector2(0, -10);

        // 5. ปุ่มกลับสู่หน้าหลัก (Main Menu Button)
        GameObject btnObj = new GameObject("ReturnButton", typeof(RectTransform));
        btnObj.transform.SetParent(contentObj.transform, false);
        btnObj.AddComponent<CanvasRenderer>();
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.18f, 0.20f, 0.24f, 0.9f);
        btnImg.raycastTarget = true;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.18f, 0.20f, 0.24f, 0.9f);
        colors.highlightedColor = new Color(0.30f, 0.35f, 0.42f, 1f);
        colors.pressedColor = new Color(0.10f, 0.12f, 0.15f, 1f);
        btn.colors = colors;
        btn.onClick.AddListener(ReturnToMainMenu);

        RectTransform btnRt = btnObj.GetComponent<RectTransform>() ?? btnObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.pivot = new Vector2(0.5f, 0.5f);
        btnRt.sizeDelta = new Vector2(360, 60);
        btnRt.anchoredPosition = new Vector2(0, -150);

        // ข้อความบนปุ่ม
        GameObject btnTextObj = new GameObject("Text", typeof(RectTransform));
        btnTextObj.transform.SetParent(btnObj.transform, false);
        btnTextObj.AddComponent<CanvasRenderer>();
        TextMeshProUGUI btnTmp = btnTextObj.AddComponent<TextMeshProUGUI>();
        if (font != null) btnTmp.font = font;
        btnTmp.text = "กลับสู่หน้าหลัก (Main Menu)";
        btnTmp.fontSize = 24;
        btnTmp.fontStyle = FontStyles.Bold;
        btnTmp.alignment = TextAlignmentOptions.Center;
        btnTmp.color = Color.white;
        btnTmp.raycastTarget = false;

        RectTransform btnTextRt = btnTextObj.GetComponent<RectTransform>() ?? btnTextObj.AddComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.sizeDelta = Vector2.zero;
        btnTextRt.anchoredPosition = Vector2.zero;

        // 6. ข้อความคำใบ้ด้านล่าง (Hint Text)
        GameObject hintObj = new GameObject("HintText", typeof(RectTransform));
        hintObj.transform.SetParent(contentObj.transform, false);
        hintObj.AddComponent<CanvasRenderer>();
        hintText = hintObj.AddComponent<TextMeshProUGUI>();
        if (font != null) hintText.font = font;
        hintText.text = "[ คลิกหน้าจอ หรือ กด Space / Enter เพื่อกลับสู่หน้าหลัก ]";
        hintText.fontSize = 20;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = new Color(0.75f, 0.75f, 0.75f, 0.8f);
        hintText.raycastTarget = false;

        RectTransform hintRt = hintObj.GetComponent<RectTransform>() ?? hintObj.AddComponent<RectTransform>();
        hintRt.anchorMin = new Vector2(0.5f, 0.5f);
        hintRt.anchorMax = new Vector2(0.5f, 0.5f);
        hintRt.pivot = new Vector2(0.5f, 0.5f);
        hintRt.sizeDelta = new Vector2(1000, 50);
        hintRt.anchoredPosition = new Vector2(0, -230);
    }

    private IEnumerator EndingAnimationRoutine(string sceneName)
    {
        // หน่วงเวลาสั้นๆ ก่อนเริ่มการเฟด
        yield return new WaitForSecondsRealtime(0.3f);

        // ขั้นตอนที่ 1: จอดำค่อยๆ เฟดเข้า (Fade to Black - 1.5 วินาที)
        float fadeDuration = 1.5f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (blackOverlay != null)
            {
                blackOverlay.color = new Color(0f, 0f, 0f, t);
            }
            yield return null;
        }
        if (blackOverlay != null) blackOverlay.color = Color.black;

        // ขั้นตอนที่ 2: หยุดดำสนิทชั่วขณะเพื่อสร้างอารมณ์ร่วม (0.5 วินาที)
        yield return new WaitForSecondsRealtime(0.5f);

        // ขั้นตอนที่ 3: ข้อความ "THE END" และประเภทตอนจบ ค่อยๆ ปรากฏขึ้น (Fade in Text - 1.5 วินาที)
        float textDuration = 1.5f;
        elapsed = 0f;
        while (elapsed < textDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / textDuration);
            if (contentCanvasGroup != null)
            {
                contentCanvasGroup.alpha = t;
            }
            yield return null;
        }
        if (contentCanvasGroup != null) contentCanvasGroup.alpha = 1f;

        // หน่วงเล็กน้อยก่อนเริ่มเปิดรับการกดปุ่ม (ป้องกันการกดข้ามโดยไม่ตั้งใจ)
        yield return new WaitForSecondsRealtime(0.5f);
        canAcceptInput = true;

        // นับเวลาถอยหลังเพื่อดีดกลับสู่ MainMenu อัตโนมัติ (6 วินาที)
        float countdown = 6f;
        while (countdown > 0f && !isTransitioningToMenu)
        {
            countdown -= Time.unscaledDeltaTime;
            if (hintText != null)
            {
                int sec = Mathf.Max(1, Mathf.CeilToInt(countdown));
                hintText.text = $"[ จะกลับสู่เมนูหลักในอีก {sec} วินาที | หรือคลิกหน้าจอ / กด Space / Enter เพื่อกลับทันที ]";
            }
            yield return null;
        }

        if (!isTransitioningToMenu)
        {
            ReturnToMainMenu();
        }
    }

    void Update()
    {
        if (!isEndingActive || !canAcceptInput || isTransitioningToMenu) return;

        // ลูกเล่นตัวหนังสือ Hint กระพริบแบบนุ่มนวล
        if (hintText != null)
        {
            float alpha = 0.4f + Mathf.PingPong(Time.unscaledTime * 0.8f, 0.55f);
            Color c = hintText.color;
            hintText.color = new Color(c.r, c.g, c.b, alpha);
        }

        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        // ตรวจจับการกดปุ่มใดๆ หรือคลิกเมาส์
        bool triggerReturn = false;

        if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
        {
            triggerReturn = true;
        }

        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || 
                                         Keyboard.current.enterKey.wasPressedThisFrame || 
                                         Keyboard.current.escapeKey.wasPressedThisFrame ||
                                         Keyboard.current.anyKey.wasPressedThisFrame))
        {
            triggerReturn = true;
        }

        if (triggerReturn)
        {
            ReturnToMainMenu();
        }
    }

    /// <summary>
    /// ฟังก์ชันเปลี่ยนฉากกลับสู่ MainMenu
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (isTransitioningToMenu) return;
        isTransitioningToMenu = true;

        Debug.Log("[EndingScreenController] กำลังเปลี่ยนฉากกลับสู่ MainMenu...");

        if (canvasObj != null)
        {
            Destroy(canvasObj);
        }

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject, 1f);
    }

    private TMP_FontAsset GetBestFont()
    {
        // 1. ลองดึงจาก DialogueManager ที่เปิดอยู่
        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.bodyText != null && dm.bodyText.font != null)
        {
            return dm.bodyText.font;
        }
        if (dm != null && dm.nameText != null && dm.nameText.font != null)
        {
            return dm.nameText.font;
        }

        // 2. ลองค้นหา Font ในโปรเจกต์ (เช่น Kanit)
        TMP_FontAsset[] allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        foreach (var f in allFonts)
        {
            if (f != null && f.name.Contains("Kanit"))
            {
                return f;
            }
        }
        if (allFonts.Length > 0 && allFonts[0] != null)
        {
            return allFonts[0];
        }

        return TMP_Settings.defaultFontAsset;
    }
}
