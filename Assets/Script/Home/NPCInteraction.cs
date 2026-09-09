using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class NPCPortrait
{
    [Tooltip("Name of the portrait emotion (e.g. smile, angry, sad) - displays as Element title")]
    public string portraitName;

    [Tooltip("Sprite asset for this facial expression")]
    public Sprite sprite;
}

[System.Serializable]
public class NPCProfile
{
    [Tooltip("Name of the NPC (This string will display as the Element name in the Inspector list)")]
    public string npcName;
    
    [Tooltip("All emotional/facial sprite expressions for this character")]
    public List<NPCPortrait> portraits = new List<NPCPortrait>();
}

[System.Serializable]
public class DailyDialogue
{
    [HideInInspector] public string dayTitle; // ⚡ บรรทัดแรกสุดของคลาสต้องเป็น String เสมอ เพื่อให้ Unity นำไปแสดงผลเป็นหัวข้อ Element แทนคำว่า Element 0

    [Tooltip("Active day number for this dialogue set (This will display as the Element title)")]
    public int dayNumber = 1; 

    [Space(10)]
    [Header("--- 🟩 Intro Dialogue ---")]
    public List<DialogueLine> introductionStory = new List<DialogueLine>();

    [Space(10)]
    [Header("--- 🟨 Choice Dialogue ---")]
    public List<DialogueChoice> storyChoices = new List<DialogueChoice>();

    [Space(10)]
    [Header("--- 🟦 Conclusion Dialogue ---")]
    public List<DialogueLine> conclusionStory = new List<DialogueLine>();
}

[System.Serializable]
public class DailyFallback
{
    [HideInInspector] public string dayTitle; // ⚡ บรรทัดแรกสุดของคลาสต้องเป็น String เสมอ เพื่อให้ Unity นำไปแสดงผลเป็นหัวข้อ Element

    [Tooltip("Day number for this fallback story (This will display as the Element title)")]
    public int dayNumber = 1;

    [Tooltip("Fallback dialogue lines to play for this specific day")]
    public List<DialogueLine> fallbackStory = new List<DialogueLine>();
}

public class NPCInteraction : MonoBehaviour
{
    [Header("👤 NPC Settings")]
    [Tooltip("Display name of the NPC")]
    public string npcDisplayName = "Mom";

    [Header("👥 NPC Data Profiles")]
    [Tooltip("List of character profiles involved in this interaction")]
    public List<NPCProfile> npcProfiles = new List<NPCProfile>();

    [Header("📅 Dialogues By Day")]
    [Tooltip("List of daily dialogues mapped by day number")]
    public List<DailyDialogue> dialoguesByDay = new List<DailyDialogue>();

    [Header("🔁 Fallback Dialogues By Day")]
    [Tooltip("List of day-based fallback dialogues to play when already talked or no main story exists")]
    public List<DailyFallback> fallbackStoryByDay = new List<DailyFallback>();

    [Header("❌ General Fallback Dialogue")]
    [Tooltip("General fallback dialogue to play if no day-specific story or fallback is found")]
    public List<DialogueLine> fallbackStory = new List<DialogueLine>();

    private bool isPlayerClose = false;
    private bool isTriggerClose = false;
    private GUIStyle npcPromptStyle;
    private Texture2D npcPromptBgTex;
    [HideInInspector] public int lastTalkedDay = 0;

    // กลุ่มตัวละครที่อยู่ด้วยกันและใช้บทสนทนาร่วมกัน
    private static readonly string[][] CompanionGroups = new string[][]
    {
        new string[] { "Jin", "Ben" },
        new string[] { "Den", "Sasha", "Egon" },
        new string[] { "Shia", "Hong" }
    };

    public static bool AreCompanions(string id1, string id2)
    {
        if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2)) return false;
        if (string.Equals(id1, id2, System.StringComparison.OrdinalIgnoreCase)) return true;
        foreach (var group in CompanionGroups)
        {
            bool has1 = false, has2 = false;
            foreach (var member in group)
            {
                if (string.Equals(member, id1, System.StringComparison.OrdinalIgnoreCase)) has1 = true;
                if (string.Equals(member, id2, System.StringComparison.OrdinalIgnoreCase)) has2 = true;
            }
            if (has1 && has2) return true;
        }
        return false;
    }

    public static string[] GetCompanionGroup(string id)
    {
        if (string.IsNullOrEmpty(id)) return new string[] { id };
        foreach (var group in CompanionGroups)
        {
            foreach (var member in group)
            {
                if (string.Equals(member, id, System.StringComparison.OrdinalIgnoreCase)) return group;
            }
        }
        return new string[] { id };
    }

    private bool IsClosestInteractableNPC()
    {
        CharacterController player = Object.FindAnyObjectByType<CharacterController>();
        if (player == null) return true;

        Vector3 playerPos = player.transform.position;
        float myDist = Vector3.Distance(transform.position, playerPos);

        NPCInteraction[] allNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var npc in allNPCs)
        {
            if (npc != null && npc != this && npc.isPlayerClose)
            {
                float otherDist = Vector3.Distance(npc.transform.position, playerPos);
                if (otherDist < myDist)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public string GetNPCIdentifier()
    {
        if (!string.IsNullOrEmpty(npcDisplayName) && npcDisplayName != "-" && npcDisplayName != "'-'")
        {
            return npcDisplayName.Trim();
        }

        if (npcProfiles != null)
        {
            foreach (var profile in npcProfiles)
            {
                if (profile != null && !string.IsNullOrEmpty(profile.npcName))
                {
                    if (!profile.npcName.Equals("Nia", System.StringComparison.OrdinalIgnoreCase) &&
                        !profile.npcName.Equals("Player", System.StringComparison.OrdinalIgnoreCase))
                    {
                        return profile.npcName.Trim();
                    }
                }
            }
        }

        string cleanName = gameObject.name.Replace("(Clone)", "").Trim();
        if (cleanName.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanName = cleanName.Substring(4);
        }
        return cleanName;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(npcDisplayName) || npcDisplayName == "-" || npcDisplayName == "'-'")
        {
            npcDisplayName = GetNPCIdentifier();
        }

        DayManager dm = Object.FindAnyObjectByType<DayManager>();
        int today = dm != null ? (int)dm.currentDay : (GameManagerSetup.Instance != null ? GameManagerSetup.Instance.currentDay : 1);

        string npcId = GetNPCIdentifier();
        if (GameManagerSetup.Instance != null)
        {
            string[] companions = GetCompanionGroup(npcId);
            foreach (var comp in companions)
            {
                if (GameManagerSetup.Instance.HasTalkedToNPCToday(comp))
                {
                    lastTalkedDay = today;
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        // คำนวณระยะห่างระหว่างตัวละครกับ NPC (รัศมี 2.2 เมตร)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            float dist = Vector3.Distance(transform.position, playerObj.transform.position);
            if (dist <= 2.2f)
            {
                isPlayerClose = true;
            }
            else if (!isTriggerClose)
            {
                isPlayerClose = false;
            }
        }

        if (isPlayerClose && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            DialogueManager dmCheck = Object.FindAnyObjectByType<DialogueManager>();
            if (dmCheck != null && dmCheck.IsDialogueActive()) return;

            // หากผู้เล่นอยู่ใกล้ NPC หลายตัวพร้อมกัน ให้ตัวที่ใกล้ผู้เล่นที่สุดเป็นตัวเรียก Trigger
            if (!IsClosestInteractableNPC()) return;

            TriggerAction();
        }
    }

    void TriggerAction()
    {
        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        DayManager dayManager = Object.FindAnyObjectByType<DayManager>();

        if (dm != null && dayManager != null)
        {
            int today = (int)dayManager.currentDay;
            string npcId = GetNPCIdentifier();

            // 1. ดึงภาพใบหน้าเริ่มต้นแบบ fallback จากโปรไฟล์ที่ผู้ใช้กรอก
            Sprite playerFallbackImg = null;
            Sprite npcFallbackImg = null;
            foreach (var profile in npcProfiles)
            {
                if (profile != null)
                {
                    bool isPlayer = profile.npcName.Equals("Nia", System.StringComparison.OrdinalIgnoreCase) || 
                                     profile.npcName.Equals("Player", System.StringComparison.OrdinalIgnoreCase);
                    
                    if (isPlayer)
                    {
                        if (profile.portraits != null && profile.portraits.Count > 0) 
                            playerFallbackImg = profile.portraits[0].sprite;
                    }
                    else
                    {
                        if (profile.portraits != null && profile.portraits.Count > 0 && npcFallbackImg == null) 
                            npcFallbackImg = profile.portraits[0].sprite;
                    }
                }
            }

            // 2. เช็คคุยซ้ำในวันเดียวกัน (รวมกลุ่มคู่หู/กลุ่มเพื่อน)
            bool hasTalkedToday = (today == lastTalkedDay);
            if (GameManagerSetup.Instance != null)
            {
                string[] companions = GetCompanionGroup(npcId);
                foreach (var comp in companions)
                {
                    if (GameManagerSetup.Instance.HasTalkedToNPCToday(comp))
                    {
                        hasTalkedToday = true;
                        break;
                    }
                }
            }

            if (hasTalkedToday)
            {
                Debug.Log($"[{npcDisplayName}] วันนี้คุยไปแล้ว! เล่นข้อความสำรองประจำวัน");
                List<DialogueLine> activeFallback = GetTodaysFallback(today, true);
                ResolveDialogueLineSprites(activeFallback);
                ShowFallbackDialogue(dm, activeFallback, playerFallbackImg, npcFallbackImg);
                return; 
            }

            lastTalkedDay = today;
            if (GameManagerSetup.Instance != null)
            {
                string[] companions = GetCompanionGroup(npcId);
                foreach (var comp in companions)
                {
                    GameManagerSetup.Instance.RegisterNPCTalkedToday(comp);
                }

                NPCInteraction[] allNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var otherNpc in allNPCs)
                {
                    if (otherNpc != null && AreCompanions(npcId, otherNpc.GetNPCIdentifier()))
                    {
                        otherNpc.lastTalkedDay = today;
                    }
                }
            }
            if (DailyQuestManager.Instance != null)
            {
                DailyQuestManager.Instance.RefreshQuestList();
            }

            DailyDialogue todaysDialogue = null;
            string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string targetEnding = activeScene.StartsWith("Ending_", System.StringComparison.OrdinalIgnoreCase) && activeScene != "GameOver_Stress"
                ? activeScene
                : (today >= 17 ? DayManager.GetCalculatedEndingScene() : null);

            if (!string.IsNullOrEmpty(targetEnding))
            {
                foreach (var dialog in dialoguesByDay)
                {
                    if (dialog != null && dialog.dayNumber == today)
                    {
                        if (!string.IsNullOrEmpty(dialog.dayTitle) && dialog.dayTitle.IndexOf(targetEnding, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            todaysDialogue = dialog;
                            break;
                        }
                    }
                }
            }

            if (todaysDialogue == null)
            {
                foreach (var dialog in dialoguesByDay)
                {
                    if (dialog != null && dialog.dayNumber == today)
                    {
                        todaysDialogue = dialog;
                        break;
                    }
                }
            }

            // หากตัวละครนี้ไม่มีบทสนทนาประจำวัน ให้สืบค้นจากตัวละครในกลุ่มเดียวกัน (เช่น Sasha/Egon ดึงจาก Den, Ben ดึงจาก Jin, Hong ดึงจาก Shia)
            if (todaysDialogue == null)
            {
                NPCInteraction[] allNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var otherNpc in allNPCs)
                {
                    if (otherNpc != null && otherNpc != this && AreCompanions(npcId, otherNpc.GetNPCIdentifier()))
                    {
                        foreach (var d in otherNpc.dialoguesByDay)
                        {
                            if (d != null && d.dayNumber == today)
                            {
                                todaysDialogue = d;
                                break;
                            }
                        }
                        if (todaysDialogue != null) break;
                    }
                }
            }

            // 3. แสดงบทสนทนาประจำวัน
            if (todaysDialogue != null)
            {
                // Resolve สไปรต์สำหรับประโยครันไทม์ทั้งหมดก่อนเล่น
                ResolveDialogueLineSprites(todaysDialogue.introductionStory);
                ResolveDialogueLineSprites(todaysDialogue.conclusionStory);
                ResolveDialogueChoiceSprites(todaysDialogue.storyChoices);

                dm.StartCustomDialogue(npcDisplayName, playerFallbackImg, npcFallbackImg, 
                                       todaysDialogue.introductionStory, 
                                       todaysDialogue.storyChoices, 
                                       todaysDialogue.conclusionStory);

                if (TutorialManager.Instance != null && (npcDisplayName.Equals("Mom", System.StringComparison.OrdinalIgnoreCase) || npcId.Equals("Mom", System.StringComparison.OrdinalIgnoreCase)))
                {
                    TutorialManager.Instance.OnTalkedToMom();
                }
            }
            else
            {
                List<DialogueLine> activeFallback = GetTodaysFallback(today, false);
                ResolveDialogueLineSprites(activeFallback);
                ShowFallbackDialogue(dm, activeFallback, playerFallbackImg, npcFallbackImg);
            }
        }
    }

    private List<DialogueLine> GetTodaysFallback(int today, bool hasTalkedToday = false)
    {
        if (fallbackStoryByDay != null)
        {
            foreach (var fb in fallbackStoryByDay)
            {
                if (fb != null && fb.dayNumber == today && fb.fallbackStory != null && fb.fallbackStory.Count > 0)
                {
                    return fb.fallbackStory;
                }
            }
        }
        if (fallbackStory != null && fallbackStory.Count > 0)
        {
            return fallbackStory;
        }

        // ค้นหาข้อความสำรองจากเพื่อนในกลุ่มเดียวกัน
        NPCInteraction[] allNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var otherNpc in allNPCs)
        {
            if (otherNpc != null && otherNpc != this && AreCompanions(GetNPCIdentifier(), otherNpc.GetNPCIdentifier()))
            {
                if (otherNpc.fallbackStoryByDay != null)
                {
                    foreach (var fb in otherNpc.fallbackStoryByDay)
                    {
                        if (fb != null && fb.dayNumber == today && fb.fallbackStory != null && fb.fallbackStory.Count > 0)
                        {
                            return fb.fallbackStory;
                        }
                    }
                }
                if (otherNpc.fallbackStory != null && otherNpc.fallbackStory.Count > 0)
                {
                    return otherNpc.fallbackStory;
                }
            }
        }

        // หากไม่มีข้อความสำรองใดๆ เลย ให้สร้างประโยคสำรองอัตโนมัติ เพื่อป้องกัน UI เปิดหน้าต่างเปล่า
        List<DialogueLine> defaultFallback = new List<DialogueLine>();
        string msg = hasTalkedToday ? $"(คุยกับ {npcDisplayName} เรียบร้อยแล้ว)" : $"(ดูเหมือน {npcDisplayName} จะไม่มีอะไรคุยในตอนนี้)";
        defaultFallback.Add(new DialogueLine
        {
            text = msg,
            speakerName = "Nia",
            portraitName = "idle",
            stressChange = 0f,
            activeSlotIndex = 1,
            motionEffect = SpriteAction.None
        });
        return defaultFallback;
    }

    public Sprite GetNPCPortrait(string speakerName, string portraitName)
    {
        if (string.IsNullOrEmpty(speakerName)) return null;

        // 1. ค้นหาจาก npcProfiles ของตนเอง
        if (npcProfiles != null)
        {
            foreach (var profile in npcProfiles)
            {
                if (profile != null && profile.npcName.Equals(speakerName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(portraitName))
                    {
                        foreach (var portrait in profile.portraits)
                        {
                            if (portrait != null && portrait.portraitName.Equals(portraitName, System.StringComparison.OrdinalIgnoreCase) && portrait.sprite != null)
                            {
                                return portrait.sprite;
                            }
                        }
                    }
                    if (profile.portraits != null && profile.portraits.Count > 0 && profile.portraits[0] != null && profile.portraits[0].sprite != null)
                    {
                        return profile.portraits[0].sprite;
                    }
                }
            }
        }

        // 2. ค้นหาจาก CharacterPortraitDatabase สากล
        return CharacterPortraitDatabase.GetPortrait(speakerName, portraitName);
    }

    private void ResolveDialogueLineSprites(List<DialogueLine> lines)
    {
        if (lines == null) return;
        foreach (var line in lines)
        {
            if (line != null)
            {
                line.resolvedPortraitSprite = GetNPCPortrait(line.speakerName, line.portraitName);
            }
        }
    }

    private void ResolveDialogueChoiceSprites(List<DialogueChoice> choices)
    {
        if (choices == null) return;
        foreach (var choice in choices)
        {
            if (choice != null)
            {
                choice.resolvedPortraitSprite = GetNPCPortrait(choice.speakerName, choice.portraitName);
                ResolveDialogueLineSprites(choice.nextDialogueLines);
            }
        }
    }

    private void ShowFallbackDialogue(DialogueManager dm, List<DialogueLine> lines, Sprite playerImg, Sprite npcImg)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.Log($"[{npcDisplayName}] ไม่มีข้อความสำรอง");
            return;
        }
        dm.StartCustomDialogue(npcDisplayName, playerImg, npcImg, 
                               lines, 
                               new List<DialogueChoice>(), 
                               new List<DialogueLine>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponent<PlayerMovement>() != null)
        {
            isTriggerClose = true;
            isPlayerClose = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponent<PlayerMovement>() != null)
        {
            isTriggerClose = false;
        }
    }

    private void OnGUI()
    {
        if (!isPlayerClose) return;
        if (DevConsole.Instance != null && DevConsole.Instance.IsOpen) return;

        DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
        if (dm != null && dm.IsDialogueActive()) return;

        // หากผู้เล่นอยู่ใกล้ NPC หลายตัวพร้อมกัน แสดงข้อความเฉพาะตัวที่ใกล้ที่สุด
        if (!IsClosestInteractableNPC()) return;

        if (npcPromptStyle == null)
        {
            npcPromptStyle = new GUIStyle();
            npcPromptStyle.alignment = TextAnchor.MiddleCenter;
            npcPromptStyle.fontSize = 18;
            npcPromptStyle.fontStyle = FontStyle.Bold;
            npcPromptStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);

            npcPromptBgTex = new Texture2D(1, 1);
            npcPromptBgTex.SetPixel(0, 0, new Color(0.08f, 0.12f, 0.18f, 0.88f));
            npcPromptBgTex.Apply();
            npcPromptStyle.normal.background = npcPromptBgTex;
            npcPromptStyle.padding = new RectOffset(16, 16, 8, 8);
        }

        string nameToShow = string.IsNullOrEmpty(npcDisplayName) ? GetNPCIdentifier() : npcDisplayName;
        string message = ThaiTextAdjuster.Adjust($"กด [E] เพื่อพูดคุยกับ {nameToShow}");

        float width = 320f;
        float height = 44f;
        float x = (Screen.width - width) / 2f;
        float y = Screen.height * 0.82f;

        GUI.Label(new Rect(x, y, width, height), message, npcPromptStyle);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // ⚡ ออโต้โคลนและเคลียร์ค่าเริ่มต้นของสล็อตประโยคใหม่ทันทีเมื่อกดเพิ่มบรรทัดใน Inspector
        SanitizeDialogueLines(null, fallbackStory);
        
        if (fallbackStoryByDay != null)
        {
            foreach (var fb in fallbackStoryByDay)
            {
                // อัปเดตชื่อหัวข้อวันใน Inspector
                fb.dayTitle = $"DAY {fb.dayNumber} (FALLBACK)";
                SanitizeDialogueLines(null, fb.fallbackStory);
            }
        }

        if (dialoguesByDay != null)
        {
            foreach (var daily in dialoguesByDay)
            {
                // อัปเดตชื่อหัวข้อวันใน Inspector
                if (string.IsNullOrEmpty(daily.dayTitle) || !daily.dayTitle.StartsWith("Ending_"))
                {
                    daily.dayTitle = $"DAY {daily.dayNumber}";
                }
                
                SanitizeDialogueLines(daily, daily.introductionStory);
                SanitizeDialogueLines(daily, daily.conclusionStory);
                SanitizeDialogueChoices(daily, daily.storyChoices);
            }
        }
    }

    private void SanitizeDialogueLines(DailyDialogue daily, List<DialogueLine> lines)
    {
        if (lines == null) return;

        for (int i = 0; i < lines.Count; i++)
        {
            DialogueLine current = lines[i];
            if (current == null) continue;

            // 1. ตรวจสอบเงื่อนไขตัวแอดใหม่ (โคลนจากตัวเก่า)
            if (i > 0)
            {
                DialogueLine previous = lines[i - 1];
                if (previous != null && current.text == previous.text && !string.IsNullOrEmpty(current.text))
                {
                    current.speakerName = previous.speakerName;
                    current.portraitName = previous.portraitName;
                    current.otherName = previous.otherName;

                    current.text = "";
                    current.stressChange = 0f;
                    current.activeSlotIndex = 0; // ตั้งเป็น Auto-Snap (0) เสมอตอนโคลนตัวใหม่
                    current.motionEffect = SpriteAction.None;
                    
                    // ทำการสืบค้นหาตำแหน่งก่อนหน้าของตัวละครนี้ และระบุทันที
                    int historicalSlot = FindLastSlotForSpeaker(daily, lines, i, current.speakerName);
                    if (historicalSlot != 0) current.activeSlotIndex = historicalSlot;
                    else current.activeSlotIndex = 4; // หากไม่พบประวัติ ให้ดีฟอลต์เป็น 4 (Center)
                    
                    continue;
                }
            }

            // 2. หากสล็อตยังเป็น Auto-Snap (0) และมีชื่อคนพูด ให้ประมวลผลดึงค่าประวัติล่าสุดมาจัดวาง
            if (current.activeSlotIndex == 0 && !string.IsNullOrEmpty(current.speakerName))
            {
                int historicalSlot = FindLastSlotForSpeaker(daily, lines, i, current.speakerName);
                if (historicalSlot != 0)
                {
                    current.activeSlotIndex = historicalSlot;
                }
                else
                {
                    // หากไม่มีประวัติในระบบเลย ให้หลุดจาก Auto (0) ไปยึดสล็อต 4 (Center) ทันที
                    current.activeSlotIndex = 4;
                }
            }
        }
    }

    private void SanitizeDialogueChoices(DailyDialogue daily, List<DialogueChoice> choices)
    {
        if (choices == null) return;

        for (int i = 0; i < choices.Count; i++)
        {
            DialogueChoice current = choices[i];
            if (current == null) continue;

            // 1. จัดการโคลนและเคลียร์ค่าของช่อง Choices ที่สร้างใหม่
            if (i > 0)
            {
                DialogueChoice previous = choices[i - 1];
                if (previous != null && current.choiceButtonText == previous.choiceButtonText && !string.IsNullOrEmpty(current.choiceButtonText))
                {
                    current.speakerName = previous.speakerName;
                    current.portraitName = previous.portraitName;
                    current.otherName = previous.otherName;

                    current.choiceButtonText = "";
                    current.stressChange = 0f;
                    current.activeSlotIndex = 0; // ตั้งเป็น Auto-Snap (0) เสมอตอนโคลนช้อยส์ใหม่
                    current.motionEffect = SpriteAction.None;
                    current.nextDialogueLines = new List<DialogueLine>();

                    // ทำการสืบค้นตำแหน่งก่อนหน้าสำหรับช้อยส์
                    int historicalSlot = FindLastSlotForSpeaker(daily, null, 0, current.speakerName);
                    if (historicalSlot != 0) current.activeSlotIndex = historicalSlot;
                    else current.activeSlotIndex = 4; // หากไม่พบ ให้ดีฟอลต์เป็น 4 (Center)
                    
                    continue;
                }
            }

            // 2. เคลียร์/Snap ตำแหน่งอัตโนมัติหากเป็น 0
            if (current.activeSlotIndex == 0 && !string.IsNullOrEmpty(current.speakerName))
            {
                int historicalSlot = FindLastSlotForSpeaker(daily, null, 0, current.speakerName);
                if (historicalSlot != 0)
                {
                    current.activeSlotIndex = historicalSlot;
                }
                else
                {
                    current.activeSlotIndex = 4; // หากไม่มีประวัติเลย ให้สแนปเข้า 4 (Center)
                }
            }

            // ดำเนินการล้างค่าสับเซตของช้อยส์นี้ต่อ
            SanitizeDialogueLines(daily, current.nextDialogueLines);
        }
    }

    private int FindLastSlotForSpeaker(DailyDialogue daily, List<DialogueLine> currentList, int currentIndex, string speakerName)
    {
        if (string.IsNullOrEmpty(speakerName)) return 0; // ส่ง 0 กลับเมื่อหาประวัติไม่เจอ

        // 1. ค้นหาย้อนหลังจากบทพูดในลิสต์ย่อยเดียวกันก่อนหน้าตำแหน่งปัจจุบัน
        if (currentList != null)
        {
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (currentList[i] != null && currentList[i].speakerName == speakerName)
                {
                    // คืนค่าเฉพาะค่าสล็อตจริง (1-7) ไม่ดึงค่า Auto (0)
                    if (currentList[i].activeSlotIndex >= 1 && currentList[i].activeSlotIndex <= 7)
                    {
                        return currentList[i].activeSlotIndex;
                    }
                }
            }
        }

        // 2. หากไม่พบ และเราอยู่ใน DailyDialogue เดียวกัน ให้สแกนบทพูดกลุ่มอื่นที่เล่นก่อนหน้านี้ของวันนั้น
        if (daily != null)
        {
            // ลำดับการเล่นคือ: introductionStory -> storyChoices -> conclusionStory
            bool inConclusion = (currentList == daily.conclusionStory);
            bool inChoices = false;
            
            // เช็คว่าลิสต์ปัจจุบันเป็นหนึ่งในลิสต์ย่อยของช้อยส์คำตอบหรือไม่
            if (daily.storyChoices != null)
            {
                foreach (var choice in daily.storyChoices)
                {
                    if (currentList == choice.nextDialogueLines)
                    {
                        inChoices = true;
                        break;
                    }
                }
            }

            // ถ้าอยู่ในบทสรุป ให้ย้อนไปหาในช้อยส์ และบทนำ
            if (inConclusion)
            {
                int slot = FindLastInChoices(daily.storyChoices, speakerName);
                if (slot >= 1 && slot <= 7) return slot;

                slot = FindLastInList(daily.introductionStory, speakerName);
                if (slot >= 1 && slot <= 7) return slot;
            }
            // ถ้าอยู่ในช้อยส์ ให้ย้อนไปหาในบทนำ
            else if (inChoices)
            {
                int slot = FindLastInList(daily.introductionStory, speakerName);
                if (slot >= 1 && slot <= 7) return slot;
            }
        }

        return 0; // ไม่พบ คืนค่า 0
    }

    private int FindLastInList(List<DialogueLine> lines, string speakerName)
    {
        if (lines == null) return 0;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (lines[i] != null && lines[i].speakerName == speakerName)
            {
                if (lines[i].activeSlotIndex >= 1 && lines[i].activeSlotIndex <= 7)
                    return lines[i].activeSlotIndex;
            }
        }
        return 0;
    }

    private int FindLastInChoices(List<DialogueChoice> choices, string speakerName)
    {
        if (choices == null) return 0;
        for (int i = choices.Count - 1; i >= 0; i--)
        {
            var choice = choices[i];
            if (choice == null) continue;

            // ค้นหาในประโยคต่อไปของช้อยส์ (จากท้ายสุด)
            int slot = FindLastInList(choice.nextDialogueLines, speakerName);
            if (slot >= 1 && slot <= 7) return slot;

            // ตรวจสอบที่ตัวช้อยส์เอง
            if (choice.speakerName == speakerName)
            {
                if (choice.activeSlotIndex >= 1 && choice.activeSlotIndex <= 7)
                    return choice.activeSlotIndex;
            }
        }
        return 0;
    }
#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DialogueLine))]
public class DialogueLineDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        NPCInteraction npc = property.serializedObject.targetObject as NPCInteraction;

        SerializedProperty textProp = property.FindPropertyRelative("text");
        SerializedProperty speakerProp = property.FindPropertyRelative("speakerName");
        SerializedProperty portraitProp = property.FindPropertyRelative("portraitName");
        SerializedProperty stressProp = property.FindPropertyRelative("stressChange");
        SerializedProperty otherProp = property.FindPropertyRelative("otherName");
        SerializedProperty slotProp = property.FindPropertyRelative("activeSlotIndex");
        SerializedProperty motionProp = property.FindPropertyRelative("motionEffect");
        SerializedProperty voiceProp = property.FindPropertyRelative("voiceSoundName");

        float y = position.y;
        float spacing = 2f;
        float fieldHeight = 18f;

        // วาดส่วนหัว Foldout
        string labelText = string.IsNullOrEmpty(textProp.stringValue) ? "New Message Line" : textProp.stringValue;
        if (labelText.Length > 40) labelText = labelText.Substring(0, 37) + "...";
        
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, y, position.width, fieldHeight), property.isExpanded, labelText, true);
        y += fieldHeight + spacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // 1. ช่องพิมพ์ข้อความ (TextArea)
            EditorGUI.LabelField(new Rect(position.x, y, position.width, fieldHeight), "Text");
            y += fieldHeight;
            textProp.stringValue = EditorGUI.TextArea(new Rect(position.x, y, position.width, 50f), textProp.stringValue);
            y += 50f + spacing;

            // 2. Dropdown เลือกผู้พูด (Speaker Name) ดึงมาจากลิสต์ npcProfiles
            List<string> speakerOptions = new List<string> { "" };
            if (npc != null && npc.npcProfiles != null)
            {
                foreach (var profile in npc.npcProfiles)
                {
                    if (profile != null && !string.IsNullOrEmpty(profile.npcName))
                        speakerOptions.Add(profile.npcName);
                }
            }

            int selectedSpeakerIndex = speakerOptions.IndexOf(speakerProp.stringValue);
            if (selectedSpeakerIndex < 0) selectedSpeakerIndex = 0;

            Rect speakerRect = new Rect(position.x, y, position.width, fieldHeight);
            int newSpeakerIndex = EditorGUI.Popup(speakerRect, "Speaker Name", selectedSpeakerIndex, speakerOptions.ToArray());
            if (newSpeakerIndex != selectedSpeakerIndex)
            {
                speakerProp.stringValue = speakerOptions[newSpeakerIndex];
            }
            y += fieldHeight + spacing;

            // 3. Dropdown เลือกรูปใบหน้า (Portrait) ดึงจากสไปรต์ที่ใส่ของตัวละครนั้นๆ
            List<string> portraitOptions = new List<string> { "" };
            string currentSpeaker = speakerProp.stringValue;
            if (npc != null && !string.IsNullOrEmpty(currentSpeaker))
            {
                foreach (var profile in npc.npcProfiles)
                {
                    if (profile != null && profile.npcName == currentSpeaker && profile.portraits != null)
                    {
                        foreach (var port in profile.portraits)
                        {
                            if (port != null && !string.IsNullOrEmpty(port.portraitName))
                                portraitOptions.Add(port.portraitName);
                        }
                    }
                }
            }

            int selectedPortraitIndex = portraitOptions.IndexOf(portraitProp.stringValue);
            if (selectedPortraitIndex < 0) selectedPortraitIndex = 0;

            Rect portraitRect = new Rect(position.x, y, position.width, fieldHeight);
            int newPortraitIndex = EditorGUI.Popup(portraitRect, "Portrait", selectedPortraitIndex, portraitOptions.ToArray());
            if (newPortraitIndex != selectedPortraitIndex)
            {
                portraitProp.stringValue = portraitOptions[newPortraitIndex];
            }
            y += fieldHeight + spacing;

            // 4. คะแนนผลกระทบความเครียด
            Rect stressRect = new Rect(position.x, y, position.width, fieldHeight);
            stressProp.floatValue = EditorGUI.FloatField(stressRect, "Stress Charge", stressProp.floatValue);
            y += fieldHeight + spacing;

            // 5. ชื่ออื่นๆ ที่จะให้ขึ้นแสดง
            Rect otherRect = new Rect(position.x, y, position.width, fieldHeight);
            otherProp.stringValue = EditorGUI.TextField(otherRect, "Other Name", otherProp.stringValue);
            y += fieldHeight + spacing;

            // 6. Dropdown เลือกสล็อตตำแหน่งขึ้นรูป (0 = Auto-Snap, 1 ถึง 7)
            string[] slotOptions = { "0 (Auto-Snap History)", "1 (Far Left)", "2", "3", "4 (Center)", "5", "6", "7 (Far Right)" };
            int currentSlotVal = slotProp.intValue;
            if (currentSlotVal < 0 || currentSlotVal > 7) currentSlotVal = 0; // Default to 0
            int selectedSlotIndex = currentSlotVal;

            Rect slotRect = new Rect(position.x, y, position.width, fieldHeight);
            int newSlotIndex = EditorGUI.Popup(slotRect, "Active Slot Position", selectedSlotIndex, slotOptions);
            slotProp.intValue = newSlotIndex;
            y += fieldHeight + spacing;

            // 7. รูปแบบเอนิเมชันตัวสั่นไหว (Motion Effect)
            Rect motionRect = new Rect(position.x, y, position.width, fieldHeight);
            EditorGUI.PropertyField(motionRect, motionProp);
            y += fieldHeight + spacing;

            // 8. เสียงพากย์ (Voice Sound)
            DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
            List<string> voiceOptions = new List<string> { "None" };
            if (dm != null && dm.voiceClips != null)
            {
                foreach (var c in dm.voiceClips)
                {
                    if (c != null && !string.IsNullOrEmpty(c.name))
                        voiceOptions.Add(c.name);
                }
            }

            int selectedVoiceIndex = voiceOptions.IndexOf(voiceProp.stringValue);
            if (selectedVoiceIndex < 0) selectedVoiceIndex = 0;

            Rect voiceRect = new Rect(position.x, y, position.width - 25f, fieldHeight);
            Rect playBtnRect = new Rect(position.x + position.width - 22f, y, 22f, fieldHeight);

            EditorGUI.BeginChangeCheck();
            int newVoiceIndex = EditorGUI.Popup(voiceRect, "Voice Sound", selectedVoiceIndex, voiceOptions.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                voiceProp.stringValue = newVoiceIndex == 0 ? "" : voiceOptions[newVoiceIndex];
                
                if (newVoiceIndex > 0 && dm != null && dm.voiceClips != null)
                {
                    AudioClip selectedClip = dm.voiceClips.Find(c => c != null && c.name == voiceOptions[newVoiceIndex]);
                    if (selectedClip != null) EditorAudioUtility.PlayClip(selectedClip);
                }
            }
            
            if (GUI.Button(playBtnRect, "▶"))
            {
                int currentIndex = voiceOptions.IndexOf(voiceProp.stringValue);
                if (currentIndex > 0 && dm != null && dm.voiceClips != null)
                {
                    AudioClip selectedClip = dm.voiceClips.Find(c => c != null && c.name == voiceOptions[currentIndex]);
                    if (selectedClip != null) EditorAudioUtility.PlayClip(selectedClip);
                }
            }
            y += fieldHeight + spacing;

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = 18f;
        if (property.isExpanded)
        {
            return baseHeight + 18f + 50f + (6 * 20f) + 20f;
        }
        return baseHeight;
    }
}

[CustomPropertyDrawer(typeof(DialogueChoice))]
public class DialogueChoiceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        NPCInteraction npc = property.serializedObject.targetObject as NPCInteraction;

        SerializedProperty buttonTextProp = property.FindPropertyRelative("choiceButtonText");
        SerializedProperty stressProp = property.FindPropertyRelative("stressChange");
        SerializedProperty speakerProp = property.FindPropertyRelative("speakerName");
        SerializedProperty portraitProp = property.FindPropertyRelative("portraitName");
        SerializedProperty otherProp = property.FindPropertyRelative("otherName");
        SerializedProperty slotProp = property.FindPropertyRelative("activeSlotIndex");
        SerializedProperty motionProp = property.FindPropertyRelative("motionEffect");
        SerializedProperty voiceProp = property.FindPropertyRelative("voiceSoundName");
        SerializedProperty nextProp = property.FindPropertyRelative("nextDialogueLines");

        float y = position.y;
        float spacing = 2f;
        float fieldHeight = 18f;

        // วาดส่วนหัว Foldout ช้อยส์
        string labelText = string.IsNullOrEmpty(buttonTextProp.stringValue) ? "New Choice Option" : buttonTextProp.stringValue;
        if (labelText.Length > 40) labelText = labelText.Substring(0, 37) + "...";
        
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, y, position.width, fieldHeight), property.isExpanded, labelText, true);
        y += fieldHeight + spacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // 1. ข้อความปุ่มช้อยส์
            Rect buttonRect = new Rect(position.x, y, position.width, fieldHeight);
            buttonTextProp.stringValue = EditorGUI.TextField(buttonRect, "Choice Button Text", buttonTextProp.stringValue);
            y += fieldHeight + spacing;

            // 2. ค่าผลกระทบความเครียดของช้อยส์
            Rect stressRect = new Rect(position.x, y, position.width, fieldHeight);
            stressProp.floatValue = EditorGUI.FloatField(stressRect, "Stress Change", stressProp.floatValue);
            y += fieldHeight + spacing;

            // 3. Dropdown เลือกผู้พูดที่จะเปลี่ยนเมื่อกดช้อยส์นี้
            List<string> speakerOptions = new List<string> { "" };
            if (npc != null && npc.npcProfiles != null)
            {
                foreach (var profile in npc.npcProfiles)
                {
                    if (profile != null && !string.IsNullOrEmpty(profile.npcName))
                        speakerOptions.Add(profile.npcName);
                }
            }

            int selectedSpeakerIndex = speakerOptions.IndexOf(speakerProp.stringValue);
            if (selectedSpeakerIndex < 0) selectedSpeakerIndex = 0;

            Rect speakerRect = new Rect(position.x, y, position.width, fieldHeight);
            int newSpeakerIndex = EditorGUI.Popup(speakerRect, "Speaker Name", selectedSpeakerIndex, speakerOptions.ToArray());
            if (newSpeakerIndex != selectedSpeakerIndex)
            {
                speakerProp.stringValue = speakerOptions[newSpeakerIndex];
            }
            y += fieldHeight + spacing;

            // 4. Dropdown เลือกสีหน้าเมื่อกดช้อยส์นี้
            List<string> portraitOptions = new List<string> { "" };
            string currentSpeaker = speakerProp.stringValue;
            if (npc != null && !string.IsNullOrEmpty(currentSpeaker))
            {
                foreach (var profile in npc.npcProfiles)
                {
                    if (profile != null && profile.npcName == currentSpeaker && profile.portraits != null)
                    {
                        foreach (var port in profile.portraits)
                        {
                            if (port != null && !string.IsNullOrEmpty(port.portraitName))
                                portraitOptions.Add(port.portraitName);
                        }
                    }
                }
            }

            int selectedPortraitIndex = portraitOptions.IndexOf(portraitProp.stringValue);
            if (selectedPortraitIndex < 0) selectedPortraitIndex = 0;

            Rect portraitRect = new Rect(position.x, y, position.width, fieldHeight);
            int newPortraitIndex = EditorGUI.Popup(portraitRect, "Portrait", selectedPortraitIndex, portraitOptions.ToArray());
            if (newPortraitIndex != selectedPortraitIndex)
            {
                portraitProp.stringValue = portraitOptions[newPortraitIndex];
            }
            y += fieldHeight + spacing;

            // 5. ชื่ออื่นๆ ที่จะให้แสดงเมื่อกดช้อยส์นี้
            Rect otherRect = new Rect(position.x, y, position.width, fieldHeight);
            otherProp.stringValue = EditorGUI.TextField(otherRect, "Other Name", otherProp.stringValue);
            y += fieldHeight + spacing;

            // 6. Dropdown เลือกสล็อตตำแหน่งรูปสำหรับช้อยส์นี้ (0 = Auto-Snap, 1 ถึง 7)
            string[] slotOptions = { "0 (Auto-Snap History)", "1 (Far Left)", "2", "3", "4 (Center)", "5", "6", "7 (Far Right)" };
            int currentSlotVal = slotProp.intValue;
            if (currentSlotVal < 0 || currentSlotVal > 7) currentSlotVal = 0; // Default to 0
            int selectedSlotIndex = currentSlotVal;

            Rect slotRect = new Rect(position.x, y, position.width, fieldHeight);
            int newSlotIndex = EditorGUI.Popup(slotRect, "Active Slot Position", selectedSlotIndex, slotOptions);
            slotProp.intValue = newSlotIndex;
            y += fieldHeight + spacing;

            // 7. เอนิเมชันเคลื่อนไหวสีหน้าเมื่อกดช้อยส์นี้
            Rect motionRect = new Rect(position.x, y, position.width, fieldHeight);
            EditorGUI.PropertyField(motionRect, motionProp);
            y += fieldHeight + spacing;

            // 8. เสียงพากย์ (Voice Sound)
            DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
            List<string> voiceOptions = new List<string> { "None" };
            if (dm != null && dm.voiceClips != null)
            {
                foreach (var c in dm.voiceClips)
                {
                    if (c != null && !string.IsNullOrEmpty(c.name))
                        voiceOptions.Add(c.name);
                }
            }

            int selectedVoiceIndex = voiceOptions.IndexOf(voiceProp.stringValue);
            if (selectedVoiceIndex < 0) selectedVoiceIndex = 0;

            Rect voiceRect = new Rect(position.x, y, position.width - 25f, fieldHeight);
            Rect playBtnRect = new Rect(position.x + position.width - 22f, y, 22f, fieldHeight);

            EditorGUI.BeginChangeCheck();
            int newVoiceIndex = EditorGUI.Popup(voiceRect, "Voice Sound", selectedVoiceIndex, voiceOptions.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                voiceProp.stringValue = newVoiceIndex == 0 ? "" : voiceOptions[newVoiceIndex];
                
                if (newVoiceIndex > 0 && dm != null && dm.voiceClips != null)
                {
                    AudioClip selectedClip = dm.voiceClips.Find(c => c != null && c.name == voiceOptions[newVoiceIndex]);
                    if (selectedClip != null) EditorAudioUtility.PlayClip(selectedClip);
                }
            }
            
            if (GUI.Button(playBtnRect, "▶"))
            {
                int currentIndex = voiceOptions.IndexOf(voiceProp.stringValue);
                if (currentIndex > 0 && dm != null && dm.voiceClips != null)
                {
                    AudioClip selectedClip = dm.voiceClips.Find(c => c != null && c.name == voiceOptions[currentIndex]);
                    if (selectedClip != null) EditorAudioUtility.PlayClip(selectedClip);
                }
            }
            y += fieldHeight + spacing;

            // 9. กล่องลิสต์ประโยคที่จะพูดต่อหลังเลือกช้อยส์นี้
            float nextHeight = EditorGUI.GetPropertyHeight(nextProp, true);
            Rect nextRect = new Rect(position.x, y, position.width, nextHeight);
            EditorGUI.PropertyField(nextRect, nextProp, true);
            y += nextHeight + spacing;

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = 18f;
        if (property.isExpanded)
        {
            SerializedProperty nextProp = property.FindPropertyRelative("nextDialogueLines");
            float nextHeight = EditorGUI.GetPropertyHeight(nextProp, true);
            return baseHeight + (8 * 20f) + nextHeight + 10f;
        }
        return baseHeight;
    }
}

public static class EditorAudioUtility
{
    private static AudioSource previewSource;

    public static void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        // ถ้ายังไม่มีลำโพงซ่อน ให้สร้างขึ้นมา 1 ตัว
        if (previewSource == null)
        {
            GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(
                "AudioPreviewWorker", 
                HideFlags.HideAndDontSave, 
                typeof(AudioSource)
            );
            previewSource = go.GetComponent<AudioSource>();
            previewSource.spatialBlend = 0f; // 2D Sound
        }

        previewSource.clip = clip;
        previewSource.Play();
    }
}
#endif