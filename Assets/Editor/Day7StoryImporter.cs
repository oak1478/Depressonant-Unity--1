using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day7StoryImporter
{
    static Day7StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay7;
    }

    [MenuItem("Tools/Import Day 7 Story (Shia, Momon)")]
    public static void ImportAllDay7()
    {
        Debug.Log("⏳ [Day7StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 7...");
        ImportShia();
        ImportMomon();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day7StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 7 (Shia, Momon) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 7);
        day.dayTitle = "DAY 7";
        day.dayNumber = 7;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เหอะ มาสายตามเคย คิดว่าเป็นเด็กโปรดครูโมม่อนแล้วจะทำอะไรก็ได้หรือไง?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่า พอก่อนเถอะ เนียเขาคงมีเหตุผลของเขา", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("เหตุผลอะไร? นั่งทำหน้าซึมเป็นภาระคนอื่นไปวันๆ เนียเนี่ยนะ?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("งานกลุ่มคราวที่แล้วก็ทำช้า จนคนอื่นเขาต้องมาตามแก้ให้", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คนไร้ประโยชน์แบบเธอ มันเคยทำอะไรสำเร็จบ้างถามจริง?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("!!", "Nia", "bad1", 0f, 1, SpriteAction.Shake),
            CreateLine("(ไร้ประโยชน์...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(คำคำนี้ปลุก Hippocampus ในหัวขึ้นมาทวนความจำ...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉากความล้มเหลวเก่าๆ พรั่งพรูออกมาไม่หยุด ภาพเกรดตก ภาพโดนแม่บ่น ภาพทำขวดแก้วทดลองแตกเมื่อเทอมก่อน...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉายซ้ำไปซ้ำมา เหมือนภาพยนตร์ที่ไม่มีวันจบ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(จริงด้วย... เราไม่เคยทำอะไรสำเร็จเลยสักอย่างตั้งแต่เกิดมา)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ถ้าตัวเรามันไร้ค่าและเป็นภาระขนาดนี้... แล้วเราจะเกิดมาทำไม?)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอโทษ... ที่ฉันมันไร้ประโยชน์", "Nia", "bad1", 10f, 1, SpriteAction.None, null),
            CreateChoice("เธอเองก็ไม่ได้ดีไปกว่าฉันนักหรอก!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("อย่างน้อยฉันก็ไม่นั่งไร้ค่าเป็นขยะแบบเธอก็แล้วกัน!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            }),
            CreateChoice("(นิ่งเงียบ มองผ่านชีอ่าไปเหมือนไม่มีอะไรเกิดขึ้น)", "Nia", "bad1", 0f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ชีอ่า! เธอพูดแรงเกินไปแล้วนะ!", "Rin", "", 0f, 7, SpriteAction.Jump),
            CreateLine("ก็มันเรื่องจริงนี่!", "Shia", "idle", 0f, 4, SpriteAction.Shake),
            CreateLine("(เดินเข้ามาในห้องเรียน บรรยากาศเงียบลงทันที)", "Momon", "", 0f, 5, SpriteAction.None),
            CreateLine("มีอะไรกัน? เสียงดังไปถึงห้องพักครู", "Momon", "", 0f, 5, SpriteAction.None),
            CreateLine("ไม่มีอะไรค่ะครู แค่คุยกันนิดหน่อย", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คุยกัน? แต่หน้าตาเนียดูเหมือนไม่ใช่การคุยนะ", "Momon", "", 0f, 5, SpriteAction.None),
            CreateLine("เนีย... ออกมาช่วยครูยกเอกสารที่ห้องพักครูหน่อยสิ", "Momon", "", 0f, 5, SpriteAction.None),
            CreateLine("ค่ะ... ครู", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 7 สำเร็จ");
    }

    private static void ImportMomon()
    {
        string path = "Assets/Object/Characters/NPC_Momon.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 7);
        day.dayTitle = "DAY 7";
        day.dayNumber = 7;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินนำเนียออกมาตามทางเดิน)", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่าพูดอะไรกับเธอใช่ไหม?", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เปล่าค่ะ... เธอแค่พูดเรื่องจริง", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("เรื่องจริงของชีอ่า ไม่ใช่เรื่องจริงของเธอ เนีย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("สมองคนเราชอบจำแต่เรื่องแย่ๆ แล้วลืมเรื่องดีๆ ที่ตัวเองเคยทำ", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูเห็นว่าเธอพยายามแค่ไหนในห้องสมุด อย่าปล่อยให้คำพูดคนอื่นมาตัดสินชีวิตเธอ", "Momon", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครู... หนูจะพยายามย้ำเตือนตัวเอง", "Nia", "good1", -15f, 1, SpriteAction.None, null),
            CreateChoice("แต่หนูรู้สึกแบบนั้นจริงๆ นี่คะ... หนูรู้สึกไม่มีคุณค่าอะไรเลย", "Nia", "bad1", 5f, 1, SpriteAction.None, null),
            CreateChoice("หนูขอตัวไปห้องน้ำก่อนนะคะ", "Nia", "idle", 0f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Momon] อัปเดต Day 7 สำเร็จ");
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
