using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day10StoryImporter
{
    static Day10StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay10;
    }

    [MenuItem("Tools/Import Day 10 Story (Mom, Dad, Rain)")]
    public static void ImportAllDay10()
    {
        Debug.Log("⏳ [Day10StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 10...");
        ImportMom();
        ImportDad();
        ImportRain();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day10StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 10 (Mom, Dad, Rain) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportMom()
    {
        string path = "Assets/Object/Characters/NPC_Mom.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 10);
        day.dayTitle = "DAY 10";
        day.dayNumber = 10;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย วันนี้วันเสาร์แท้ๆ ทำไมยังเอาแต่หมกตัวอยู่ในห้องมืดๆ อีก?", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("ข้าวเช้าก็ไม่ยอมลงไปกิน หน้าตาดูไม่ได้เลยนะช่วงนี้", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("แม่บอกกี่ครั้งแล้วว่าอย่าทำตัวซึมกะทืออยู่ในบ้าน มันเห็นแล้วอึดอัด!", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("(คำพูดของแม่กระทบเข้าที่ pgACC... เซนเซอร์ในหัวเริ่มส่องแสงเตือนอีกครั้ง)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(เมื่อวานที่โรงเรียนก็เพิ่งพังมา... ที่บ้านก็ไม่มีพื้นที่ปลอดภัยให้เราเลยเหรอ?)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("หนู... รู้สึกเหนื่อยๆ ค่ะแม่ ขอหนูพักสักหน่อยได้ไหมคะ?", "Nia", "bad1", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เหนื่อยอะไรกัน? วันๆ ไม่ได้ทำอะไรหนักหนาสักหน่อย... เอาเถอะ อย่าให้มันนานนักละกัน", "Mom", "", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(เงียบ ไม่ตอบอะไร แล้วเดินกลับเข้าห้อง)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พูดด้วยก็เงียบ! ทำตัวน่าเอือมระอาขึ้นทุกวันนะเนีย!", "Mom", "", 5f, 4, SpriteAction.Shake)
            }),
            CreateChoice("แม่ก็เอาแต่ตำหนิหนูตลอดเวลา! เคยถามหนูสักคำไหมว่าหนูเป็นยังไงบ้าง!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("อย่ามาระเบิดอารมณ์ใส่แม่นะเนีย! ไร้สัมมาคารวะจริง ๆ!", "Mom", "", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Mom] อัปเดต Day 10 สำเร็จ");
    }

    private static void ImportDad()
    {
        string path = "Assets/Object/Characters/NPC_Dad.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 10);
        day.dayTitle = "DAY 10";
        day.dayNumber = 10;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย วันนี้พ่อต้องออกไปข้างนอกทั้งวัน", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เมื่อวานเห็นแม่เราบอกว่าเรามีเรื่องชกตบกับเพื่อนที่โรงเรียนเหรอ?", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อย่าสร้างเรื่องปวดหัวให้พ่อกับแม่เพิ่มได้ไหม เรื่องงานพ่อก็เครียดพอแล้ว", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ไม่มีใครถามเหตุผล... ไม่มีใครฟังข้อเท็จจริง... ทุกคนมองว่าฉันคือตัวปัญหาไว้ก่อนแล้ว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(mPFC ที่มัวมืดบีบให้คิดว่า... ฉันมันไม่มีค่าพอที่จะให้ใครรับฟังจริงๆ)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("หนูขอโทษค่ะพ่อ... หนูไม่ได้อยากให้เกิดเรื่องแบบนั้นขึ้นเลย", "Nia", "bad1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห้อ... รู้ตัวก็ดี คราวหลังควบคุมอารมณ์ตัวเองบ้าง", "Dad", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(พยักหน้ารับเงียบๆ โดยไม่พูดอะไร)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พ่อไปละ อยู่บ้านก็ช่วยแม่ดูน้องด้วย", "Dad", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("พ่อฟังแต่คนอื่น แล้วพ่อเคยฟังหนูบ้างไหมล่ะคะ?!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("พ่อไม่มีเวลามาฟังเธอใช้อารมณ์หรอกนะเนีย! พอที!", "Dad", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Dad", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Dad] อัปเดต Day 10 สำเร็จ");
    }

    private static void ImportRain()
    {
        string path = "Assets/Object/Characters/NPC_Rein.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 10);
        day.dayTitle = "DAY 10";
        day.dayNumber = 10;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เคาะประตูห้องเนียเบาๆ ก่อนจะเปิดเข้ามา)", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("พี่เนีย... พวกเดน ซาช่า แล้วก็อีกอน รออยู่ข้างล่างน่ะ", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("วันนี้พวกเขาชวนไปเดินเล่นที่สวนสาธารณะ แล้วก็วาดรูปกัน... พี่ไปด้วยกันไหม?", "Rein", "Idle", 0f, 4, SpriteAction.Jump),
            CreateLine("เรน... พี่...", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("ช่วงนี้พี่ดูเหนื่อยๆ มากเลยนะ... หนูเป็นห่วงพี่นะพี่เนีย", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("ออกไปข้างนอกด้วยกันเถอะนะ หนูอยากให้พี่ยิ้มบ้าง", "Rein", "Idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(คำว่าเป็นห่วงของเรน... สัญญาณส่งมาถึง OFC ที่พังอยู่)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(สมองในหัวลังเล... เสียงลบๆ ใน SFG ยังคอยโต้แย้งว่า 'เรนแค่สงสาร' 'เธอไปก็ทำคนอื่นหมดสนุก')", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(แต่นี่คือโอกาส... ที่เราจะเลือกว่าจะเผชิญหน้า หรือ จะถอยหนีไปอยู่ในมืดเหมือนเดิม)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เรน... จริงๆ แล้วพี่รู้สึกแย่มากเลย... พี่รู้สึกเหมือนตัวเองไม่มีคุณค่าอะไรเลยสักอย่าง", "Nia", "bad1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พี่เนีย... ไม่จริงเลยนะ! สำหรับหนู พี่เนียเป็นพี่สาวที่ดีที่สุดเสมอ! ไปข้างนอกกันนะ หนูจะอยู่ข้างๆ พี่เอง!", "Rein", "Idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("พี่ขอโทษนะเรน... แต่พี่อยากอยู่คนเดียวจริงๆ วันนี้", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("งั้นเหรอคะ... งั้นพี่พักผ่อนนะ เดี๋ยวหนูซื้อขนมมาฝาก", "Rein", "Idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("อย่ามายุ่งกับพี่ได้ไหม! ออกไปให้พ้นเลย!", "Nia", "bad1", 15f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("พี่เนีย... ทำไมต้องใจร้ายใส่หนูขนาดนี้ด้วย...", "Rein", "Idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(มองเรนที่ยืนอยู่ตรงหน้า... ทางเลือกในตอนนี้ อยู่ที่ตัวฉันเองแล้ว)", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rain] อัปเดต Day 10 สำเร็จ");
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
