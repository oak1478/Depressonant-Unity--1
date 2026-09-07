using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day12StoryImporter
{
    static Day12StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay12;
    }

    [MenuItem("Tools/Import Day 12 Story (Rin, Park, Shia)")]
    public static void ImportAllDay12()
    {
        Debug.Log("⏳ [Day12StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 12...");
        ImportRin();
        ImportPrak();
        ImportShia();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day12StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 12 (Rin, Park, Shia) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 12);
        day.dayTitle = "DAY 12";
        day.dayNumber = 12;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! สวัสดีตอนเช้า... เมื่อวันศุกร์เธอเป็นอะไรหรือเปล่า? ฉันเป็นห่วงแทบบ้าเลย", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("เห็นเธอวิ่งหนีออกไป ฉันอยากตามไปแต่ดันติดธุระพอดี... เธอโอเคขึ้นแล้วใช่ไหม?", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(OFC ที่เคยประเมินคำชมและความหวังดีผิดพลาด เริ่มเปิดรับสัญญาณบวกได้ชัดเจนขึ้น)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ความหวังดีของรินไม่ใช่เรื่องเสแสร้ง... มันคือความจริงใจที่สมองป่วยๆ ของเราเคยมองข้ามไป)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ถึงเวลาแล้วที่เราจะกล้าขอบคุณคนที่อยู่ข้างเรามาตลอด)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ริน... ฉันขอโทษนะสำหรับวันศุกร์ และขอบคุณมากๆ สำหรับสรุปเคมี ถ้าไม่มีเธอฉันคงสอบไม่ผ่าน ขอบใจนะที่ไม่ทิ้งฉัน", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มกว้าง) ไม่เป็นไรเลยเนีย! แค่เธอผ่านและยิ้มได้ ฉันก็ดีใจมากแล้ว! เราเป็นเพื่อนกันนี่นา!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ฉันไม่เป็นไรแล้วริน... ขอบใจสำหรับสรุปวันก่อนนะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อื้ม! ไม่เป็นไรเลย ถือว่าช่วยๆ กันนะ!", "Rin", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("อย่ามาทำเป็นห่วงหน่อยเลยริน... ฉันดูแลตัวเองได้", "Nia", "bad1", 10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนีย... ฉันแค่เป็นห่วงจริงๆ นะ ทำไมต้องพูดแบบนั้นด้วย...", "Rin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นไว้คุยกันนะเนีย", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 12 สำเร็จ");
    }

    private static void ImportPrak()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 12);
        day.dayTitle = "DAY 12";
        day.dayNumber = 12;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ยืนหมุนลูกบาสอยู่ในมือคนเดียว พอเห็นเนียเดินผ่านก็ทักขึ้น)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไง เนีย นึกว่าจะสติแตกจนไม่มาโรงเรียนซะแล้ว", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เรื่องเมื่อวันก่อน... ที่ฉันพูดขวานผ่าซากไปหน่อย ก็ขอโทษละกัน แต่ฉันพูดเพราะอยากเห็นเธอฮึดสู้ ไม่ได้อยากให้เธอหนีปัญหา", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ที่เคยขยายความขัดแย้งให้กลายเป็นเรื่องใหญ่โต เริ่มแยกแยะเจตนาได้ดีขึ้น)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ป้ากอาจจะพูดตรงไป... แต่เขาไม่ได้มีเจตนาทำร้ายเราเหมือนที่เราเคยเข้าใจผิดไปเอง)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("คำพูดนายมันบาดหูจริงๆ นั่นแหละป้าก... แต่ก็นะ มันทำให้ฉันคิดอะไรได้หลายอย่าง ขอโทษที่วันก่อนฉันวิ่งหนีไปนะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(หัวเราะเบาๆ) เอากับเขาดิ! ยอมรับตรงๆ แบบนี้ค่อยน่าคุยด้วยหน่อย คราวหน้าก็อย่าอมทุกข์อีกล่ะ!", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("อืม... ช่างมันเถอะป้าก", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เออๆ รู้เรื่องละกัน", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("คำพูดนายมันแย่มากป้าก นายไม่มีวันเข้าใจคนอื่นหรอก!", "Nia", "bad1", 5f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เฮ้อ... แล้วแต่จะคิดละกัน พูดด้วยดีๆ แท้ๆ", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ฉันไปซ้อมบาสต่อละ", "Park", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 12 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 12);
        day.dayTitle = "DAY 12";
        day.dayNumber = 12;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(นั่งอยู่ที่โต๊ะกับหงส์ พอเห็นเนียเดินเข้ามาก็เชิดหน้าหนี ทำท่าอึดอัด)", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... สวัสดีนะ เมื่อวันศุกร์โอเคขึ้นหรือยัง?", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("จะไปถามมันทำไมหงส์? เดี๋ยวก็ร้องไห้ฟูมฟายหาว่าพวกเราไปรังแกอีกหรอก", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(mPFC และ SFG เริ่มทำงานร่วมกันอย่างเป็นระบบ...)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เสียงในหัวที่เคยคอยซ้ำเติมตัวเองเริ่มเงียบลง... เราไม่ต้องปล่อยให้คำพูดของชีอ่ามาเป็นตัวกำหนดคุณค่าของเราอีกต่อไป)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(การเผชิญหน้า ไม่ใช่การชวนทะเลาะ... แต่คือการกล้าพูดความจริงอย่างตรงไปตรงมา)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ชีอ่า... หงส์ ฉันขอโทษสำหรับอารมณ์วันก่อนนะ แต่ฉันอยากบอกให้รู้ว่า ที่ฉันเงียบ ไม่ใช่เพราะฉันเรียกร้องความสนใจ แต่เพราะฉันกำลังสู้กับความคิดตัวเองอยู่ ต่อจากนี้ฉันจะพยายามปรับปรุงตัวนะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนีย... ไม่หรอก พวกเราก็พูดแรงไปเหมือนกัน ขอโทษนะ", "Hong", "", 0f, 6, SpriteAction.None),
                CreateLine("(ชะงักไป ถอนหายใจ) เหอะ... รู้ตัวก็ดีแล้ว ทีหลังมีอะไรก็พูดออกมาตรงๆ สิ", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(ก้มหน้าแล้วเดินผ่านไปนั่งที่โต๊ะตัวเองเงียบๆ)", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ดูสิ ทำเป็นเมินอีกละ น่าอึดอัดชะมัด", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ชีอ่า! เธอเลิกทำตัวเป็นเจ้าชีวิตคนอื่นสักทีได้ไหม!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เอ๊ะ! นิสัยไม่เคยเปลี่ยนจริงๆ!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(นั่งลงที่โต๊ะเรียนของตัวเอง...)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(การกล้าเคลียร์ใจ ทำให้อากาศที่เคยอึดอัดรอบตัว ค่อยๆ เบาบางลงอย่างเห็นได้ชัด)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 12 สำเร็จ");
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
