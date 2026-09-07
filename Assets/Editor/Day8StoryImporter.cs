using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day8StoryImporter
{
    static Day8StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay8;
    }

    [MenuItem("Tools/Import Day 8 Story (Rin, Shia, Vipar, Park)")]
    public static void ImportAllDay8()
    {
        Debug.Log("⏳ [Day8StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 8...");
        ImportRin();
        ImportShia();
        ImportVipar();
        ImportPrak();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day8StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 8 (Rin, Shia, Vipar, Park) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 8);
        day.dayTitle = "DAY 8";
        day.dayNumber = 8;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! เอกสารติวเคมีฉบับสรุปเสร็จแล้วนะ ฉันทำส่วนของเธอไว้ให้ด้วย", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("ลองดูตรงตารางธาตุสิ เธอจำตรรกะตรงนี้ได้แม่นมากเลยนะ เก่งมากๆ เลย!", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เอ๊ะ... ตรงนั้นฉันแค่บังเอิญเดาถูก", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ไม่บังเอิญหรอก! ครั้งก่อนเธอก็ทำได้ดีมาก ฉันว่าถ้าสอบพรุ่งนี้ เธอทำคะแนนได้ดีแน่ๆ", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(เก่งมาก?... OFC dysfunction ทำงานทันที)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(พลังบวกจากคำชมส่งมาไม่ถึงเลยสักนิด... สมองประเมินค่ามันไม่ได้เลย)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(รินคงแค่เกรงใจ... คงสมเพชเด็กห่วยๆ แบบฉันจนต้องหาคำมาปลอบใจล่ะสิ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(เสแสร้ง... ชมไปงั้นแหละเพื่อให้ตัวเองดูเป็นเพื่อนที่ดี)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบใจนะริน... แต่ไม่ต้องพยายามชมฉันขนาดนั้นก็ได้", "Nia", "bad1", -5f, 1, SpriteAction.None, null),
            CreateChoice("อย่ามาปลอบใจฉันเลย ริน เธอไม่ได้คิดแบบนั้นจริงๆ หรอก", "Nia", "bad1", 5f, 1, SpriteAction.None, null),
            CreateChoice("อืม... ขอบใจ", "Nia", "idle", 0f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 8 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 8);
        day.dayTitle = "DAY 8";
        day.dayNumber = 8;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เหอะ รินอวยยศให้ขนาดนั้น รู้สึกเหมือนตัวเองเป็นอัจฉริยะเคมีขึ้นมาบ้างยังล่ะเนีย?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่า พอก่อนเถอะน่า รินเขาแค่ช่วยติวเพื่อน", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ก็เห็นอยู่ว่ารินประคบประหงมสุดชีวิต แต่ดูหน้ามันสิ หน้าบึ้งเหมือนไม่อยากรับความช่วยเหลือจากใคร", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คนเขาหวังดีด้วยแท้ๆ ยังทำตัวน่าอึดอัดอีก", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... ถ้าไม่เข้าใจตรงไหน ถามพวกเราได้นะ ไม่ต้องเกรงใจ", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("(พอแล้ว... พอสักที)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ทุกคนมองฉันเป็นภาระที่ต้องมานั่งสงสาร...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันไม่อยากได้ความสมเพชจากพวกเธอ!)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(ลุกขึ้นยืนตบโต๊ะ) พวกเธอเลิกยุ่งกับฉันสักทีได้ไหม!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เออ! คิดว่าพวกฉันอยากยุ่งนักหรือไง!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            }),
            CreateChoice("ฉันไม่ได้ขอให้ใครมาสงสาร!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เนีย... พวกเราไม่ได้คิดแบบนั้นนะ", "Hong", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("(หยิบกระเป๋าแล้วเดินถอยห่างออกมาทันที)", "Nia", "bad1", 15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนีย! เดี๋ยวสิ! จะไปไหนน่ะ?!", "Rin", "", 0f, 7, SpriteAction.Jump),
                CreateLine("อย่าตามฉันมา! ออกไปให้พ้นจากฉันกันให้หมดนั่นแหละ!", "Nia", "bad1", 0f, 1, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>();

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 8 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 8);
        day.dayTitle = "DAY 8";
        day.dayNumber = 8;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เกิดอะไรขึ้นน่ะ? เสียงดังไปถึงหน้าอาคารเรียน", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย? จะไปไหน? ใกล้ได้เวลาเข้าเรียนแล้วนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("หนู... หนูขอโทษค่ะ!", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("ยึดติดอะไรกันนักหนาเด็กพวกนี้? บรรยากาศตึงเครียดกันไปหมด", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย เลขที่ 7 ช่างเถอะ... พรุ่งนี้สอบเคมีนะ ถ้าขาดสอบ ครูไม่ให้สอบซ่อมนะจะบอกให้", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(แม้แต่ครูวิภาก็แค่มองฉันเป็นเด็กสร้างปัญหาอีกคน...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ไม่มีใครเข้าใจสักคน... ถอยออกไปคนเดียวนั่นแหละดีที่สุดแล้ว)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices & Conclusion ---
        day.storyChoices = new List<DialogueChoice>();
        day.conclusionStory = new List<DialogueLine>();

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 8 สำเร็จ");
    }

    private static void ImportPrak()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 8);
        day.dayTitle = "DAY 8";
        day.dayNumber = 8;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินถือลูกบาสเข้ามา หมุนลูกบาสในมือแล้วมองมาที่เนีย)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย ฉันเห็นเธอนั่งทำหน้าอมทุกข์มาตั้งแต่เมื่อกี้แล้ว รินมันอุตส่าห์อดนอนทำสรุปเคมีมาให้เธอนะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ถ้าอ่านไม่รู้เรื่อง หรือไม่อยากเรียน ก็บอกมันไปตรงๆ ดิ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("นั่งเงียบทำหน้าเหมือนโดนบังคับกินยาขมแบบนี้ มันเสียเวลาติวกันเปล่าๆ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(คำพูดตรงๆ ของป้าก แทงเข้ากลางอกเต็มๆ...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(pgACC ทำงานหนักขึ้น... สมองตรวจจับความขัดแย้งและขยายคำพูดป้ากให้กลายเป็นความล้มเหลวใหญ่โต)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ป้ากพูดถูก... ฉันมันทำตัวน่าอึดอัด... ฉันทำให้คนอื่นเสียเวลา... ฉันเป็นภาระจริงๆ)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(ลุกขึ้นยืนตบโต๊ะ) ใช่สิ! ฉันมันตัวไร้ประโยชน์ที่ทำให้ทุกคนเสียเวลาใช่ไหมล่ะ?!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เอ้า ถ้ารู้ตัวแล้วก็แค่ปรับปรุงดิ จะมาประชดใส่คนอื่นทำไม?", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอโทษนะป้าก... ฉันมันแย่เองแหละ", "Nia", "bad1", 10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ฉันไม่ได้อยากได้คำขอโทษ แค่อยากให้เธอเลิกทำตัวอึดอัดสักที", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(ไม่พูดอะไร ลุกขึ้นเก็บของใส่กระเป๋าแล้วเดินหนีออกไปทันที)", "Nia", "bad1", 15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เอ้าว์ เดินหนีซะงั้น พูดเรื่องจริงแค่นี้ก็รับไม่ได้เรอะ?", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(วิ่งหนีออกมาคนเดียว...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ไม่มีใครอยากยุ่งกับฉันหรอก... ทุกคนแค่รำคาญฉันทั้งนั้น)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 8 สำเร็จ");
    }

    private static DailyDialogue GetOrCreateDay(List<DailyDialogue> list, int dayNum)
    {
        foreach (var d in list)
        {
            if (d.dayNumber == dayNum) return d;
        }
        DailyDialogue newDay = new DailyDialogue { dayNumber = dayNum };
        list.Add(newDay);
        return newDay;
    }

    private static DialogueLine CreateLine(string text, string speaker, string portrait, float stress, int slot, SpriteAction motion)
    {
        return new DialogueLine
        {
            text = text,
            speakerName = speaker,
            portraitName = portrait,
            stressChange = stress,
            otherName = "",
            activeSlotIndex = slot,
            motionEffect = motion,
            voiceSoundName = ""
        };
    }

    private static DialogueChoice CreateChoice(string buttonText, string speaker, string portrait, float stress, int slot, SpriteAction motion, List<DialogueLine> nextLines)
    {
        return new DialogueChoice
        {
            choiceButtonText = buttonText,
            speakerName = speaker,
            portraitName = portrait,
            stressChange = stress,
            otherName = "",
            activeSlotIndex = slot,
            motionEffect = motion,
            voiceSoundName = "",
            nextDialogueLines = nextLines != null ? nextLines : new List<DialogueLine>()
        };
    }
}
