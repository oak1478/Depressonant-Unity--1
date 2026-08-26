using UnityEngine;

public class MirrorInteraction : MonoBehaviour
{
    [Header("UI Reference (Optional)")]
    [Tooltip("ลาก UI Dialogue/Text (TextMeshPro) ที่ต้องการใช้แสดงความในใจของเนีย (หากปล่อยว่างไว้ระบบจะแสดงขึ้นบนหน้าจอให้อัตโนมัติเวลาเล่น)")]
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงสะท้อนจิตใจหลอนๆ ตอนส่องกระจกมาใส่ (เช่น 867822...incoherent_additive-glitch)")]
    [SerializeField] private AudioClip mirrorReflectionSound;

    private StressManager stressManager;
    private AudioSource audioSource;
    private string reflectionMessage = "";
    private float messageDisplayTime = 4.0f;
    private float timer = 0f;

    private void Start()
    {
        // ค้นหา StressManager ในฉาก
        stressManager = Object.FindAnyObjectByType<StressManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                HideDialogue();
            }
        }
    }

    // ฟังก์ชันหลักเมื่อกดโต้ตอบกับกระจก (เรียกใช้จาก BedroomInteractable)
    public void LookInMirror()
    {
        // เล่นเสียงสะท้อนจิตใจหลอนๆ ตอนส่องกระจก
        if (mirrorReflectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(mirrorReflectionSound);
        }

        // ⚡ [เพิ่มใหม่] แจ้งระบบ Tutorial เมื่อส่องกระจกสำเร็จ
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnLookedInMirror();
        }

        float stress = 0f; // ค่าตั้งต้นกรณีหาตัวจัดการความเครียดไม่เจอ
        if (stressManager != null)
        {
            stress = stressManager.currentStress;
        }

        // ประเมินคำพูดของเนียอ้างอิงจากระดับความเครียดปัจจุบัน
        if (stress <= 30f)
        {
            reflectionMessage = "เนีย: \"วันนี้ใบหน้าของฉันดูปกติ... หวังว่าวันนี้จะเป็นวันที่ดีนะ\"";
        }
        else if (stress <= 70f)
        {
            reflectionMessage = "เนีย: \"ใต้ตาของฉันเริ่มคล้ำแล้ว... วันนี้หน้าตาดูอิดโรยและเหนื่อยล้าจัง\"";
        }
        else
        {
            reflectionMessage = "เนีย: \"หน้าตาของฉันดูหมองคล้ำและไร้ชีวิตชีวามาก... ฉันไม่อยากมองกระจกบานนี้เลย...\"";
        }

        ShowDialogue(reflectionMessage);
        Debug.Log($"[MirrorInteraction] ส่องกระจก: {reflectionMessage} (ความเครียดปัจจุบัน: {stress}%)");
    }

    private void ShowDialogue(string text)
    {
        if (dialogueText != null) dialogueText.text = text;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        timer = messageDisplayTime;
    }

    private void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    // กรณีที่ไม่ได้ทำ UI Dialogue สำเร็จรูปมาใส่ จะวาดตัวหนังสือคำพูดบนหน้าจอให้ตอนส่องกระจก
    private void OnGUI()
    {
        if (dialoguePanel != null || timer <= 0f) return;

        Rect labelRect = new Rect(Screen.width / 2 - 300, Screen.height - 150, 600, 50);

        Color oldColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(labelRect, Texture2D.whiteTexture);
        GUI.color = oldColor;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 18;
        style.normal.textColor = Color.yellow;

        GUI.Label(labelRect, reflectionMessage, style);
    }
}
