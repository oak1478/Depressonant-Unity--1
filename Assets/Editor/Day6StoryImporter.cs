using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day6StoryImporter
{
    static Day6StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay6;
    }

    [MenuItem("Tools/Import Day 6 Story (Rin, Vipar, Shia, Momon)")]
    public static void ImportAllDay6()
    {
        Debug.Log("⏳ [Day6StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 6...");
        ImportRin();
        ImportVipar();
        ImportShia();
        ImportMomon();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day6StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 6 (Rin, Vipar, Shia, Momon) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! มาเร็ว วันนี้งานจัดชั้นหนังสือในห้องสมุดเยอะเลย", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("ถ้าได้เธอช่วย ต้องเสร็จทันก่อนคาบบ่ายแน่ๆ", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อื้ม... เดี๋ยวฉันช่วยเอง", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันเป็นบรรณารักษ์นะ... เรื่องแค่นี้ฉันทำได้ดีอยู่แล้ว)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(...แต่ทำได้ดีแล้วยังไงล่ะ? สุดท้ายเธอก็แค่เด็กเรียกลูกมือไม่ใช่หรือไง)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("เนีย? ยืนเหม่ออะไรอยู่เหรอ? เป็นอะไรรึเปล่า?", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เปล่าหรอก แค่กำลังคิดว่าจะเริ่มจัดหมวดไหนก่อนดี", "Nia", "good1", -5f, 1, SpriteAction.None, null),
            CreateChoice("ฉันแค่... รู้สึกว่าทำไปก็คงไม่มีใครสนหรอก", "Nia", "bad1", 0f, 1, SpriteAction.None, null),
            CreateChoice("ถามอยู่นั่นแหละ! ฉันจัดคนเดียวได้น่า!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นเหรอ... งั้นฉันฝากหมวดวิทยาศาสตร์นะ พอดีครูวิภาสั่งไว้", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว ริน จัดหนังสือเสร็จหรือยัง?", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูต้องการเล่มที่มีเนื้อหาเคมีตารางธาตุด่วนเลยนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เรียบร้อยแล้วค่ะครู เนียเป็นคนช่วยจัดหมวดนั้นให้พอดีเลยค่ะ!", "Rin", "idle", 0f, 6, SpriteAction.Jump),
            CreateLine("อ้อ... งั้นเหรอ ขอบใจมากนะริน ช่วยได้เยอะเลย", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(เดินหยิบหนังสือแล้วเดินออกไปโดยไม่มองเนียแม้แต่น้อย)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("...", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(เห็นไหมล่ะ... เขาไม่เห็นหัวเธอด้วยซ้ำ รินเป็นคนได้หน้าทั้งหมด: เสียงในหัว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(กระจกมันฝ้ามัวไปหมด mPFC พังยับ... ฉันไม่มีตัวตนจริงๆ ในสายตาใครเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(เงียบไว้... ครูคงแค่รีบเฉยๆ)", "Nia", "bad1", 0f, 1, SpriteAction.None, null),
            CreateChoice("ครูคะ! หนูเป็นคนจัดกองนั้นคนเดียวนะคะ!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("อ้าวเหรอจ๊ะ? จ้าๆ ขอบใจนะเนีย (พูดแบบไม่ใส่ใจก่อนเดินไป)", "Vipar", "idle", 5f, 4, SpriteAction.None)
            }),
            CreateChoice("ริน... ฉันขอตัวไปห้องน้ำก่อนนะ", "Nia", "bad1", 5f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("เฮ้อ", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว เนีย เดินหน้าซีดออกมาเชียว", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("การจัดหนังสือมันเหนื่อยขนาดนั้นเลยเหรอ?", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("พวกเธออย่าทักเนียแบบนั้นสิ เนียเขาตั้งใจทำงานมากเลยนะ!", "Rin", "idle", 0f, 7, SpriteAction.Jump),
            CreateLine("(ทำงานมาก? ตั้งใจ? เสียงของรินฟังดูปลอมชะมัด... OFC dysfunction)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ชมตามมารยาทล่ะสิ... สมเพชฉันอยู่ใช่ไหม? ปลอบใจเด็กห่วยๆ คนนึงอยู่สินะ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("หางานหาการทำก็ดีแล้ว ดีกว่านั่งซึมเป็นเป็ดหลงฝูงอยู่ในห้องเรียน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่า... พูดแรงไปแล้วนะ", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ก็มันเรื่องจริงนี่ สมองขี้เลื่อยแบบนั้น วันๆ เอาแต่หมกตัว ไม่รู้จะมาโรงเรียนทำไม", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("!!", "Nia", "bad1", 0f, 1, SpriteAction.Shake),
            CreateLine("(pgACC ทำงานหนักขึ้น... ความขัดแย้งเล็กๆ โดนขยายใหญ่จนหูอื้อ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ใช่... ชีอ่าพูดถูกแล้ว ฉันมันไร้ค่า ทำอะไรก็ล้มเหลว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ขยะ... ไร้ประโยชน์... อยากหายไปซะตอนนี้เลย)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(นิ่งเงียบ กดความรู้สึกทั้งหมดไว้ข้างใน)", "Nia", "bad1", 10f, 1, SpriteAction.None, null),
            CreateChoice("พวกเธอจะไปเข้าใจอะไรล่ะ!", "Nia", "bad1", -15f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เหรอ? งั้นก็ทำให้ดูสิว่าเข้าใจอะไรบ้าง!", "Shia", "idle", 10f, 4, SpriteAction.Shake)
            }),
            CreateChoice("ขอโทษนะ... ฉันมันแย่เอง", "Nia", "bad1", 5f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("อือ", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportMomon()
    {
        string path = "Assets/Object/Characters/NPC_Momon.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("...", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(เดินถือแก้วกาแฟเข้ามาเงียบๆ ด้านหลังเนีย)", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("จัดชั้นหนังสือหมวดวิทยาศาสตร์เสร็จไวเหมือนเดิมเลยนะเนีย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูโมม่อน...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ตรงนี้มุมระเบียบดีขึ้นเยอะ ปกติชั้นนี้ยุ่งเหยิงตลอดถ้าเธอไม่อยู่", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ครูแค่พูดไปงั้นๆ แหละ... เขาคงแค่เกรงใจ ปลอบใจเด็กห่วยๆ คนนึงอยู่สินะ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("เงียบทำไมล่ะ? กาแฟดำเข้าตาเหรอ?", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เปล่าค่ะ... แค่คิดว่า ใครๆ ก็ทำได้", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("...", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คนอื่นทำได้ แต่เธอทำมัน ใส่ใจ กว่าคนอื่น เนีย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ห้องสมุดนี้ ถ้าไม่มีเธอคอยดูแล ป่านนี้หนังสือคงสลับหมวดจนครูวิภาหาไม่เจอสักเล่มแล้ว", "Momon", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครู... หนูจะพยายามดูแลให้ดีขึ้นอีก", "Nia", "good1", -15f, 1, SpriteAction.None, null),
            CreateChoice("ครูพูดประชดหนูหรือเปล่าคะ?", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("หน้าครูดูเหมือนคนชอบพูดประชดขนาดนั้นเลยเหรอ?", "Momon", "idle", 5f, 4, SpriteAction.None)
            }),
            CreateChoice("(ก้มหน้าไม่พูดอะไร)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เอาเถอะ ขอบใจมากนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Momon] อัปเดต Day 6 สำเร็จ");
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
