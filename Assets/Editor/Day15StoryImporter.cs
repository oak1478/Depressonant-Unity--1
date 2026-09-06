using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day15StoryImporter
{
    static Day15StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay15;
    }

    [MenuItem("Tools/Import Day 15 Story (Vipar, Rin, Shia, Park)")]
    public static void ImportAllDay15()
    {
        Debug.Log("⏳ [Day15StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 15...");
        ImportVipar();
        ImportRin();
        ImportShia();
        ImportPark();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day15StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 15 (Vipar, Rin, Shia, Park) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 15);
        day.dayTitle = "DAY 15";
        day.dayNumber = 15;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาในห้องเรียนพร้อมตั้งตั้งกระดาษคำตอบและใบคะแนนสอบเคมีลงบนโต๊ะ)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เอาล่ะทุกคน... ใบคะแนนสอบเคมีและเกรดเฉลี่ยเก็บคะแนนกลางภาคออกแล้วนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... ออกมารับใบแจกคะแนนของตัวเองไปสิ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(เดินออกไปรับใบคะแนน... สายตามองตัวเลขคะแนนบนกระดาษ)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(ตัวเลขคะแนนและเกรดไม่ได้เพอร์เฟกต์สมบูรณ์แบบ... แต่มันคือผลลัพธ์ของความพยายามที่เกิดขึ้นจริง)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(สมองส่วน mPFC และ SFG มองเห็นข้อเท็จจริงตรงหน้าชัดเจนขึ้น... นี่ไม่ใช่ตราบาปความล้มเหลว แต่เป็นจุดเริ่มต้นที่เราสามารถก้าวต่อไปได้)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(ยิ้มรับ) ขอบคุณค่ะครูวิภา! ถึงคะแนนจะยังไม่สูงมาก แต่หนูจะพยายามให้ดีขึ้นกว่าเดิมแน่นอนค่ะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(มองเนียด้วยความประหลาดใจก่อนจะยิ้มแย้ม) ทัศนคติแบบนี้แหละดีมากเนีย! ครูเชื่อว่าเทอมหน้าเธอทำได้ดีกว่านี้แน่", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณค่ะครู... หนูจะนำไปปรับปรุงนะคะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะ ค่อยๆ พยายามไปนะเนีย", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(จ้องมองตัวเลขแล้วมือสั่น) คะแนน... คะแนนแค่นี้เองเหรอ... หนูมันห่วยจริงๆ", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เนีย! ทำไมสติแตกแบบนั้นล่ะ? ผ่านเกณฑ์ก็ดีแล้ว คราวหน้าเอาใหม่สิ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("เอาล่ะ ทุกคนแยกย้ายกันไปเตรียมตัวเรียนคาบถัดไปได้", "Vipar", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 15 สำเร็จ");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 15);
        day.dayTitle = "DAY 15";
        day.dayNumber = 15;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาหาเนียที่โต๊ะ มองหน้าเนียด้วยความลุ้นระทึก)", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย! เป็นยังไงบ้างคะแนนสอบเคมี? โอเคไหม?", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อื้ม... ก็ตามผลงานน่ะริน ไม่ได้สูงลิ่ว แต่ก็ไม่ได้แย่เลยนะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ถอนหายใจด้วยความโล่งอกพร้อมยิ้มกว้าง) เฮ้อ... ดีใจจัง! ตอนแรกฉันกลัวเธอจะเครียดเรื่องเกรดซะอีก", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เห็นเธอยิ้มรับคะแนนแบบนี้ได้ ฉันรู้สึกภูมิใจในตัวเธอมากๆ เลยนะเนีย!", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(OFC ทำงานอย่างสมบูรณ์... รับรู้พลังบวกและความหวังดีของรินได้โดยไม่มีม่านบังตา)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เราไม่จำเป็นต้องได้คะแนนเต็มเพื่อเรียกร้องการยอมรับ... แค่เรายอมรับตัวเอง ความช่วยเหลือจากรินก็มีคุณค่ามหาศาลแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบใจมากนะริน ถ้าไม่มีเธอช่วยติววันนั้น ฉันคงทำไม่ได้ขนาดนี้ เทอมหน้ามาพยายามด้วยกันอีกนะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มสดใสโผกอดเนีย) ได้เลยเนีย! เราจะช่วยกันเรียนไปเรื่อยๆ เลย!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ขอบใจนะริน... คะแนนออกมาน่าพอใจเลยล่ะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เย้! ดีใจด้วยจริงๆ นะเนีย!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("มันก็น้อยกว่าเธอตั้งเยอะ... เธอคงสมเพชฉันอยู่สินะ", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เนีย... ทำไมพูดแบบนั้นล่ะ ฉันดีใจกับเธอจริงๆ นะ...", "Rin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("สู้ๆ นะเนีย! ฉันอยู่ข้างเธอเสมอ", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 15 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 15);
        day.dayTitle = "DAY 15";
        day.dayNumber = 15;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ดูใบคะแนนของตัวเองแล้วเดินมาทักเนีย)", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("เนีย... คะแนนสอบเคมีเป็นยังไงบ้าง? ของฉันรอดตายมาได้นิดเดียวเอง", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ผ่านแล้วล่ะหงส์ ได้มากกว่าที่คิดไว้หน่อยนึงด้วย", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เดินเข้ามาพิงโต๊ะ กอดอกมองเนีย)", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("หึ ก็ไม่ได้แย่นี่คะแนนเธอ... ถึงจะไม่ใช่ระดับท็อปของห้อง แต่ก็ถือว่าไม่ได้ทำตัวเป็นภาระผลงานกลุ่มใครละกัน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ก็ถือว่า... พยายามได้ดีขึ้นนะ", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ตรวจจับคำพูดของชีอ่าอย่างสงบ... สัญญาณความขัดแย้งไม่ขยายตัวเป็นความกดดันอีกต่อไป)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คำยอมรับของชีอ่าถึงจะฟังดูปากแข็ง แต่เรามองออกว่าเธอไม่ได้เจตนาทำร้ายเราอีกแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบใจนะชีอ่า คราวหน้าฉันจะทำคะแนนไล่ตามพวกเธอให้ทันเลย คอยดูสิ!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มมุมปาก) เหอะ! พูดแล้วทำให้ได้อย่างที่พูดด้วยล่ะ!", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบใจนะ... ฉันก็จะพยายามต่อไปเรื่อยๆ นั่นแหละ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เก่งมากเลยเนีย!", "Hong", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("เธอก็ยังชอบพูดจาแซะฉันไม่เปลี่ยนเลยนะชีอ่า!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("ฉันแค่พูดตามเนื้อผ้า! เธอจะหาเรื่องฉันอีกทำไมเนี่ย!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ไปหาอะไรกินตอนเที่ยงกันดีกว่าพวกเรา", "Hong", "", 0f, 6, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 15 สำเร็จ");
    }

    private static void ImportPark()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 15);
        day.dayTitle = "DAY 15";
        day.dayNumber = 15;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ชูใบคะแนนสอบเคมีของตัวเองขึ้นแล้วเดินเข้ามาหาเนีย)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไง เนีย! คะแนนสอบเคมีออกแล้ว ได้ข่าวว่าผ่านแล้วนี่", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เป็นไงล่ะ? พอเห็นเกรดจริงแล้ว โลกมันไม่ได้แตกสลายอย่างที่เธอเคยกลัวใช่ไหม?", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(Hippocampus ไม่ดึงฉากล้มเหลวในอดีตมาฉายซ้ำ... มีเพียงความจริงในปัจจุบันที่อยู่ตรงหน้า)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คำพูดตรงๆ ของป้ากกระตุ้นให้เรามองอนาคตด้วยความท้าทาย มากกว่าความกลัว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ใช่ป้าก! โลกไม่ได้แตกสลายสักนิด และครั้งหน้าฉันจะเอาคะแนนแซงนายให้ดู!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(หัวเราะสะใจ) ฮ่าๆๆ! ก้าวร้าวขึ้นเยอะนี่เนีย! ชอบว่ะ! มาลองดูกันว่าใครจะชนะคราวหน้า!", "Park", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("อื้ม... ก็ไม่ได้แย่อย่างที่คิดจริงๆ นั่นแหละ ขอบใจนะป้าก", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห็นไหมล่ะ บอกแล้วว่าอย่าไปกลัวมัน!", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("นายมาพูดเยาะเย้ยฉันหรือไงป้าก?", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห้อ... ย้อนกลับไปทำตัวสติแตกอีกละ พูดดีๆ ด้วยก็หาว่าเยาะเย้ย", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(มองใบคะแนนในมือด้วยหัวใจที่มั่นคง...)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คะแนนอาจจะไม่ใช่ที่สุด... แต่จิตใจของฉันในตอนนี้ แข็งแกร่งขึ้นมากพอที่จะยิ้มรับทุกความจริงแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 15 สำเร็จ");
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
