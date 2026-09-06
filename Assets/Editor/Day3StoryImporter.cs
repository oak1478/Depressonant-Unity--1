using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day3StoryImporter
{
    static Day3StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay3;
    }

    [MenuItem("Tools/Import Day 3 Story (Mom, Rain, Dad)")]
    public static void ImportAllDay3()
    {
        Debug.Log("⏳ [Day3StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 3...");
        ImportMomAndDad();
        ImportRain();
        ImportDad();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day3StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 3 ลง Prefab ทั้ง 3 ตัวเสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportMomAndDad()
    {
        string path = "Assets/Object/Characters/NPC_Mom.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 3);
        day.dayTitle = "DAY 3";
        day.dayNumber = 3;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย หลังกินข้าวเสร็จแล้วไปจัดการกองหนังสือในห้องด้วยนะ", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("แม่บอกกี่ครั้งแล้วว่าอย่าปล่อยให้มันรก", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("แล้วเรื่องเกรดเทอมนี้", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("ถ้ายังเป็นแบบเดิม แม่คงต้องลดค่าขนมแกแล้วนะ", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("เออ แล้วนี่ใครจะไปส่งเรนไปเรียนพิเศษละ ?", "Dad", "", 0f, 6, SpriteAction.None),
            CreateLine("พ่อมีธุระด่วนนะ", "Dad", "", 0f, 6, SpriteAction.None),
            CreateLine("ฉันไปส่งเองก็ได้ แต่ทำไมคุณไม่ไปส่งลูกเองบ้างล่ะ เอาแต่ให้ฉันไปตลอดเลย", "Mom", "", 0f, 4, SpriteAction.Shake),
            CreateLine("เออ เออ", "Dad", "", 0f, 6, SpriteAction.None),
            CreateLine("เอาเป็นว่า แบบไหนก็ได้ละกัน", "Dad", "", 0f, 6, SpriteAction.None),
            CreateLine("นั่นปากเหรอ", "Mom", "", 0f, 4, SpriteAction.Shake),
            CreateLine("ก็บอกว่ามีธุระด่วนไง เฮ้อ~~", "Dad", "", 0f, 6, SpriteAction.None),
            CreateLine("(ฉันมั่นใจว่าวันนี้พ่อว่างแน่ ๆ)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("แล้วเธอน่ะเนีย! ยืนบื้ออยู่ทำไม ?", "Mom", "", 0f, 4, SpriteAction.Jump),
            CreateLine("เอาแต่ทำตัวไร้ประโยชน์อยู่ในห้อง ไม่ลองออกไปข้างนอกรับแสงบ้างล่ะ", "Mom", "", 10f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอโทษค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห้อ~~ ให้ตายเถอะ", "Mom", "", 5f, 4, SpriteAction.None)
            }),
            CreateChoice("...", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พูดด้วยก็เหมือนพูดกับกำแพงจริง ๆ", "Mom", "", 10f, 4, SpriteAction.None)
            }),
            CreateChoice("แล้ว แม่จะให้หนูทำยังไงละค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อย่ามาต่อปากนะเนีย !", "Mom", "", 10f, 4, SpriteAction.Shake),
                CreateLine("พ่อเขาทำงานหาเงิน ไม่เหมือนแกที่ว่างได้ว่างดี", "Mom", "", 0f, 4, SpriteAction.None),
                CreateLine("ขอโทษค่ะ", "Nia", "bad1", 0f, 1, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("แล้วเรนล่ะ ? ถ้าพ่อไม่ไปรับเดี๋ยวหนูไปเอง", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ดี", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("น้องอยู่ในห้อง ไปตามน้องออกมาด้วย ถ้าช้านักก็บังคับลากออกมาเลย", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("...", "Dad", "", 0f, 6, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Mom & Dad] อัปเดต Day 3 สำเร็จ");
    }

    private static void ImportRain()
    {
        string path = "Assets/Object/Characters/NPC_Rein.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 3);
        day.dayTitle = "DAY 3";
        day.dayNumber = 3;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เรน", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("หนูรู้แล้ว ไปเรียนพิเศษสินะ เราไปกันเถอะ", "Rein", "Idle", 0f, 4, SpriteAction.Jump),
            CreateLine("งั้นตามพี่มาที่ป้ายรถบัสนะ", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        day.storyChoices = new List<DialogueChoice>();
        day.conclusionStory = new List<DialogueLine>();

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rain] อัปเดต Day 3 สำเร็จ");
    }

    private static void ImportDad()
    {
        string path = "Assets/Object/Characters/NPC_Dad.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 3);
        day.dayTitle = "DAY 3";
        day.dayNumber = 3;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เดี๋ยวก่อนเนีย", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เดี๋ยวไปส่งด้วยคน", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ชิ !", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("กะจะเดินไปกับน้องสบาย ๆ แท้ ๆ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("เย้ พ่อมาด้วย", "Rein", "Idle", 0f, 3, SpriteAction.Jump),
            CreateLine("มาทำอะไรตรงนี้เนี่ยพ่อ ไหนว่าไม่ว่างไง", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("เอ้า ยืนบื้อทำไม ขึ้นมาสิ เดี๋ยวไปส่งน้องไม่ทันหรอก", "Dad", "idle", 0f, 6, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ธุระที่ว่าตอนแรก คืออะไรหรอคะ ?", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("โห มีหางเสียงกับต่อบ้างแล้วสินะ", "Dad", "idle", 0f, 6, SpriteAction.None),
                CreateLine("แต่ว่าเรื่องของผู้ใหญ่ อย่าถามเซ้าซี้สิ", "Dad", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("ตอบมา หนูนึกว่าพ่อรีบไปทำงานซะอีก", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จะบ่นอะไรนักหนา", "Dad", "idle", 10f, 6, SpriteAction.Shake)
            }),
            CreateChoice("(ไม่อยากพูดอะไรแล้ว)", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>())
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("พ่อจะมารับหนูไหมคะ ตอนเรียนเสร็จ ?", "Rein", "Idle", 0f, 3, SpriteAction.None),
            CreateLine("ช่วงค่ำๆ พ่อว่าง พ่อจะมารับเองละกัน", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เอาละถึงแล้ว ตั้งใจเรียนละเรน", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ค่าาาา~~~~~", "Rein", "Idle", 0f, 3, SpriteAction.Jump),
            CreateLine("...", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("เนีย", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("หือ ?", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พ่อรู้ว่าเราไม่ถูกกันนัก", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("แต่อย่าเอาเรื่องที่บ้านไปเที่ยวพูดให้ใครฟังล่ะ มันไม่ช่วยอะไร เข้าใจใช่ไหม?", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ตอบด้วย", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เข้าใจแล้ว", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พ่อจะส่งเธอที่เดิมแล้วไปทำธุระต่อแล้วกัน", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ส่วนเรื่องรับเรนกลับพ่อจะจัดการเอง กลับบ้านไปพักเถอะ", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เข้าใจแล้วน่า", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Dad] อัปเดต Day 3 สำเร็จ");
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
