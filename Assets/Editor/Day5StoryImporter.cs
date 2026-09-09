using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day5StoryImporter
{
    static Day5StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay5;
    }

    [MenuItem("Tools/Import Day 5 Story (Vipar, Rin)")]
    public static void ImportAllDay5()
    {
        Debug.Log("⏳ [Day5StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 5...");
        ImportVipar();
        ImportRin();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day5StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 5 (Vipar, Rin) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 5);
        day.dayTitle = "DAY 5";
        day.dayNumber = 5;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("ครูวิภาค่ะ สวัสดีคะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("สวัสดีเนีย ว่าแต่เธอเลขที่ 8 ใช่ไหม ? ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อ้อ ไม่ต้องห่วง แค่เลขที่ในใบรายชื่อมันสลับกันอยู่นะฉันไม่อยากหักคะแนนนักเรียนผิดคนหรอกนะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ไม่ใช่ค่ะ คือ หนูเลขที่ 7 นะคะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จะอึกอักเสียงน้อยไปทำไม แค่ถามเลขที่เอง เข้าไปในห้องได้แล้ว คนอื่นรออยู่", "Vipar", "idle", 5f, 4, SpriteAction.None),
                CreateLine("(ครูไม่สนใจฉันเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("อีกแล้วหรอค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ครูจำสลับ ระหว่างหนูกับรินตลอดเลยนะคะ หนูเลขที่ 7 นะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("!!! โทษที เชิญเข้าไปได้", "Vipar", "idle", 0f, 4, SpriteAction.Shake)
            }),
            CreateChoice("หนูเลขที่ 7 ค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("วันนี้น้ำเสียงดูมีความมั่นใจดีนะ ถ้าเป็นแบบนี้ตลอดก็ได้ครูจะดีใจมาก เชิญเข้าไปได้", "Vipar", "idle", -5f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ว่าแล้วเชียว สลับกับรินซะแล้ว", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(สุดยอดเลยครูวิภา นักเรียนมีแค่ไม่กี่คนคนแท้ ๆ ยังลืมได้อีก ฉันเข้าห้องดีกว่า)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 5 สำเร็จ");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 5);
        day.dayTitle = "DAY 5";
        day.dayNumber = 5;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! ทางนี้ๆ ฉันนึกว่าเธอจะไม่มาซะแล้ว เห็นเงียบไม่ตอบในแชทกลุ่ม วันนี้ไปช่วยงานที่ห้องสมุดหรือเปล่า? ฉันว่าจะไปยืมหนังสือพอดี", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("แน่นอนฉันอยู่ที่นั้นทุกตอนเที่ยงนะ ว่าแต่เอาอะไรมานะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ของฝากของพ่อฉันน่ะ พอดีฉันเล่าเรื่องเธอให้พ่อฟัง เขาเลยฝากมาทักทายนะ เชิญเลยๆ", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อือ ขอบใจนะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("นี่พวกเธอ", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เหลือเวลาอีกหลายนาทีก่อนถึงคาบ ครูวิภาก็เฝ้าหน้าห้องด้วย คงเล่นเกมไม่ได้แฮะ", "Shia", "idle", 0f, 7, SpriteAction.None),
            CreateLine("เพราะงั้น ริน มาทบทวนหนังสือกันไหม อีกไม่นานก็จะมีสอบเคมีด้วย รู้สึกจะเป็นวันศุกร์นะ", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ได้สิ ชวนพวก'ป้าก'มาด้วยไหม", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไม่ต้องหรอก พวกสมองขี้เลื่อยนั่นจะทำเธอเสียเวลาเปล่า", "Shia", "idle", 0f, 7, SpriteAction.None),
            CreateLine("!!", "Nia", "bad1", 0f, 1, SpriteAction.Jump),
            CreateLine("ฉันว่า ฉันไม่เอาดีกว่า ฉันขอตัว...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พูดอะไรนะ เธอก็ต้องมาด้วย ห้ามปฏิเสธ", "Rin", "idle", 0f, 4, SpriteAction.Shake),
            CreateLine("อึก", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("...", "Shia", "idle", 0f, 7, SpriteAction.None),
            CreateLine("...", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("มีใครไม่เข้าใจตรงไหนไหม", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คำพูดคลาสสิก แล้วจะมีคนพูดว่า ไม่เข้าใจหมดเลย", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ไม่เข้าใจหมดเลย รินช่วยด้วย!!", "Shia", "idle", 0f, 7, SpriteAction.Jump),
            CreateLine("นั่นไง", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("มุกมันจะสมบูรณ์แบบกว่านี้ถ้าเธอไม่ขัดฉันนะหงส์", "Shia", "idle", 0f, 7, SpriteAction.None),
            CreateLine("จ้าา แม่คนสวย ส่วนฉันคิดว่าแค่นี้คงพร้อมแล้วสำหรับสอบ", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ได้ยินแบบนั้นก็ภูมิใจ เนียล่ะเนีย?", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เอ๊ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ฉันก็เข้าใจมากขึ้นนะ แต่คงยังไม่", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(จ้อง)", "Shia", "idle", 0f, 7, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ไม่เข้าใจอะไรเลยสักนิดนะ", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เฮ้อ", "Shia", "idle", 5f, 7, SpriteAction.None),
                CreateLine("ไม่เป็นไรนะ ค่อยๆ ทบทวนดูกันใหม่", "Rin", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("พอจับจุดได้แล้วล่ะ (จริง ๆ ไม่เข้าใจอะไรเลยสักนิดนะ)", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ขอบใจมากนะรินที่เป็นห่วง", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("เยี่ยมมากเนีย! ฉันเชื่อว่าเธอทำได้", "Rin", "idle", 5f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ไว้รินช่วยติวส่วนตัวให้ทีหลังได้ไหม", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ได้เสมอ! งั้นเดี๋ยวเราไปติวต่อที่ห้องสมุดกันนะ", "Rin", "idle", -5f, 4, SpriteAction.Jump)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("มาคิดอีกที ริน คือฉันขอโทษ แต่ไม่ต้องสอนฉันแล้วก็ได้ เดี๋ยวฉันทบทวนเอง", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("แต่ว่า", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ริน ปล่อยนางไปเถอะ นางเลือกเอง เราต้องเคารพนางนะ", "Shia", "idle", 0f, 7, SpriteAction.None),
            CreateLine("เห็นด้วย", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ว่าไงเนีย แน่นะว่าไหว", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ขอบคุณริน ฉันทำเองได้", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("...", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ดูเหมือนครูโมม่อนจะไม่มาสอนแล้วนะ ไม่ไหวเลยคน ๆ นั้น คาบเช้าคงว่างอีกละ", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ว่าแต่เนีย ฉันรู้ว่าคุณกังวลเรื่องการเรียน แต่คุณจะเอาแต่พึ่งรินไม่ได้นะคะ คนอื่นก็มีนะ เช่นฉัน", "Hong", "idle", -10f, 6, SpriteAction.None),
            CreateLine("อืม โอเค", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 5 สำเร็จ");
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
