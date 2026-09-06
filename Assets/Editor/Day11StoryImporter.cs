using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day11StoryImporter
{
    static Day11StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay11;
    }

    [MenuItem("Tools/Import Day 11 Story (Mom, Dad, Jin & Ben, Rain)")]
    public static void ImportAllDay11()
    {
        Debug.Log("⏳ [Day11StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 11...");
        ImportMom();
        ImportDad();
        ImportJin();
        ImportRain();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day11StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 11 (Mom, Dad, Jin & Ben, Rain) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportMom()
    {
        string path = "Assets/Object/Characters/NPC_Mom.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 11);
        day.dayTitle = "DAY 11";
        day.dayNumber = 11;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ฉันเดินลงมาจากห้องนอนในเช้าวันอาทิตย์... บรรยากาศในบ้านเงียบสงบกว่าปกติ)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(กำลังยืนเตรียมอาหารเช้าอยู่ในครัว พอเห็นเนียเดินลงมาก็ชะงักไปเล็กน้อย)", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("อ้าว... ตื่นแล้วเหรอเนีย? วันนี้แปลกแฮะ เดินลงมาจากห้องเองแต่เช้าปกติเห็นหมกตัวอยู่จนเที่ยง", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("หนู... หนูอยากลงมาช่วยแม่ทำอาหารเช้าค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(ถอนหายใจยาว วางตะหลวกลง แล้วมองหน้าเนียตรงๆ ด้วยสายตาที่เหนื่อยล้าแต่ไม่มีความหงุดหงิดเหมือนทุกที)", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... แม่ถามจริงนะ ช่วงนี้แกเป็นอะไรไป? ทำไมดูซูบผอม แล้วก็ทำเหมือนบ้านนี้เป็นคุกล่ะ?", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("แม่ยอมรับนะว่าแม่ปากร้าย ชอบบ่นเรื่องเกรด เรื่องความสะอาด แต่แม่เห็นแกทำหน้าเหมือนโลกจะพังทุกครั้งที่แม่พูด... แม่ก็ไม่รู้จะเข้าหาแกยังไงแล้ว", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("(mPFC ที่เคยมีฝ้าบดบัง เริ่มพยายามทำงานอย่างหนัก...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ความคิดใน SFG ตีกันในหัว 'แม่แค่อึดอัดกับแก' หรือ 'จริงๆ แม่กำลังพยายามเข้าใจแกกันแน่?')", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ความรู้สึกว่าตัวเองเป็นภาระของบ้านมาตลอดหลายปี มันกำลังจุกอยู่ในคอ...)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("แม่คะ... หนูขอโทษ หนูรู้สึกเหมือนตัวเองเป็นภาระของแม่กับพ่อมาตลอด ทำอะไรก็ล้มเหลว หนูเลยกลัวที่จะเผชิญหน้ากับแม่ค่ะ", "Nia", "bad1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ชะงักไป น้ำตาซึม) เนีย... แกคิดแบบนั้นเหรอ? แม่ไม่เคยคิดว่าแกเป็นภาระนะ แกคือลูกแม่ ถึงแกจะไม่ได้เก่งที่สุด แต่แกก็คือลูกแม่นะ...", "Mom", "", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูแค่... เหนื่อยๆ ค่ะแม่ ไม่มีอะไรหรอก เดี๋ยวหนูช่วยยกจานนะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนีย มีอะไรก็พูดกับแม่บ้างสิ เอาแต่เก็บไว้คนเดียวแบบนี้ แม่ก็ปวดหัวเหมือนกันนะ", "Mom", "", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ก็แม่เอาแต่บ่นกับด่าหนูตลอดเวลา ใครจะอยากลงมาล่ะคะ!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เนีย! แม่พยายามจะคุยดีๆ ด้วยนะ ทำไมต้องประชดประชันตลอดเวลาด้วย!", "Mom", "", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("กินข้าวเช้าก่อนสิ เดี๋ยวไข่ดาวจะเย็นหมด", "Mom", "", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Mom] อัปเดต Day 11 สำเร็จ");
    }

    private static void ImportDad()
    {
        string path = "Assets/Object/Characters/NPC_Dad.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 11);
        day.dayTitle = "DAY 11";
        day.dayNumber = 11;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(นั่งอ่านข่าวอยู่ที่โต๊ะกินข้าว พ่อวางโทรศัพท์ลงเมื่อเห็นเนียนั่งลงฝั่งตรงข้าม)", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... เรื่องเมื่อวันก่อนที่พ่อพูดแรงใส่เรา เรื่องธุระเรื่องรถ... พ่อขอโทษนะ", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ช่วงนี้งานที่บริษัทพ่อตึงเครียดมาก โดนลดงบประมาณ พ่อเลยเอาอารมณ์หงุดหงิดมาลงที่บ้าน", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("พอเห็นเราเอาแต่เก็บตัวในห้อง พ่อก็ยิ่งกังวลว่าอนาคตเราจะอยู่ยังไง... พ่อเลยพูดจาไม่ดีออกไป", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ที่เคยตรวจจับแต่ความขัดแย้งเชิงลบ เริ่มสัมผัสได้ถึงความจริงใจของพ่อ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(คำพูดของพ่อกระตุ้น Hippocampus... ภาพพ่อที่เคยอุ้มเราตอนเด็กๆ ซ้อนทับขึ้นมาแทนภาพพ่อที่กำลังตวาด)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันเอาแต่คิดว่าพ่อเกลียดฉัน... แต่จริง ๆ แล้ว พ่อแค่กำลังแบกรับความเครียดของตัวเองเหมือนกัน)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("หนูเข้าใจค่ะพ่อ... หนูเองก็ขอโทษที่เอาแต่หนีปัญหา หนูคิดว่าพ่อรำคาญหนู และมองว่าหนูไม่มีอนาคต...", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ไม่มีพ่อคนไหนมองลูกตัวเองแบบนั้นหรอกเนีย พ่อแค่เป็นห่วงแต่แสดงออกไม่เป็น... จากนี้เรามาค่อยๆ ปรับกันนะ", "Dad", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ไม่เป็นไรค่ะพ่อ หนูชินแล้ว...", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อย่าพูดว่าชินสิเนีย... พ่อกำลังพยายามปรับตัวอยู่นะ", "Dad", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("พ่อห่วงแต่งานกับหน้าตาตัวเองมากกว่าความรู้สึกหนูอยู่แล้วนี่คะ!", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("ทำไมพูดจาใจแคบแบบนี้เนีย! พ่อทำงานหนักก็เพื่อพวกเธอนะ!", "Dad", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Dad", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เอาเป็นว่า วันนี้ถ้าว่าง ออกไปรับลมข้างนอกบ้างนะ พ่ออยากเห็นเราแจ่มใสขึ้น", "Dad", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Dad] อัปเดต Day 11 สำเร็จ");
    }

    private static void ImportJin()
    {
        string path = "Assets/Object/Characters/NPC_Jin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 11);
        day.dayTitle = "DAY 11";
        day.dayNumber = 11;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(หลังกินข้าวเสร็จ เนียเดินออกมาทิ้งขยะหน้าบ้าน และเจอเข้ากับป้าจินและลุงเบนที่กำลังรดน้ำต้นไม้)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("อ้าว! หนูเนีย วันนี้ตื่นเช้าจังเลยนะจ๊ะ ดูหน้าตาแจ่มใสขึ้นกว่าวันก่อนเยอะเลย", "Jin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("นั่นสิแม่จิน วันก่อนลุงทักผิดเป็นหนูเรน ต้องขอโทษด้วยนะ ลุงมันสายตาฝ้าฟางตามประสาคนแก่", "Ben", "", 0f, 6, SpriteAction.None),
            CreateLine("เมื่อวันก่อนป้าเห็นเนียเดินก้มหน้าก้มตา ป้าก็เป็นห่วง คิดว่ามีเรื่องไม่สบายใจอะไรที่โรงเรียนหรือเปล่า", "Jin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คนเราน่ะนะเนีย บางทีปัญหาในบ้านมันก็เหมือนเมฆหมอก เเต่พอแดดออกเดี๋ยวก็จางไป อย่าเก็บมาคิดมากคนเดียวเลยนะจ๊ะ", "Jin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(OFC ที่เคยรับคำชมไม่ได้... วันนี้เริ่มส่งสัญญาณแรงพอที่จะรู้สึกถึงความปรารถนาดีจากผู้ใหญ่ข้างบ้าน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ผู้ใหญ่ไม่ได้เกลียดเรา... ทุกคนแค่หมุนไปตามชีวิตของตัวเอง และพร้อมจะสนับสนุนเราถ้าเราเปิดใจ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณนะคะป้าจิน ลุงเบน... วันก่อนหนูอารมณ์ไม่ดี ต้องขอโทษด้วยนะคะที่ไม่ได้ทักทายดีๆ", "Nia", "good1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("โอ๊ย ไม่เป็นไรเลยหลานเอ๋ย! เด็กๆ ก็มีเรื่องเครียดเป็นธรรมดา แค่เห็นเนียยิ้มได้ลุงก็ดีใจแล้ว", "Ben", "", -10f, 6, SpriteAction.Jump)
            }),
            CreateChoice("ค่ะป้าจิน... หนูขอตัวเข้าบ้านก่อนนะคะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะๆ ดูแลสุขภาพด้วยนะลูก", "Jin", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("พวกป้าก็เอาแต่ยุ่งเรื่องคนอื่นตลอดเลยนะคะ", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อ้าว... ป้าแค่เห็นว่าเป็นคนกันเองหรอกนะถึงทักทาย ทำไมพูดแบบนี้ล่ะเนีย?", "Jin", "idle", 5f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ไปเถอะพ่อเบน เข้าบ้านเรากันดีกว่า", "Jin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Jin & Ben] อัปเดต Day 11 สำเร็จ");
    }

    private static void ImportRain()
    {
        string path = "Assets/Object/Characters/NPC_Rein.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 11);
        day.dayTitle = "DAY 11";
        day.dayNumber = 11;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาในห้องนอนของเนียในตอนเย็น ถือถาดขนมเค้กที่แม่ทำไว้เข้ามารวมถึงสมุดวาดเขียน)", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("พี่เนีย... วันนี้บรรยากาศตอนกินข้าวเช้า ดูแปลกไปมากเลยนะ... หนูไม่เห็นพ่อยิ้มให้พี่มานานมากแล้ว", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("หนูขอโทษนะพี่เนีย... ที่ผ่านมาหนูเอาแต่เล่น เอาแต่เรียกร้องความสนใจจากพ่อแม่ จนทำให้พี่รู้สึกเหมือนถูกลืม", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("หนูเห็นพี่นั่งร้องไห้คนเดียวในห้องบ่อยๆ หนูอยากช่วย... แต่หนูก็กลัวว่าพี่จะรำคาญหนู", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("หนูอยากได้พี่สาวคนเดิมกลับมานะ... พี่เนียคนที่เคยพาหนูวาดรูป คนที่เคยหัวเราะด้วยกัน...", "Rein", "Idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(ม่านดำมืดในสมองเริ่มถดถอยออกไปทีละนิด...)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เสียงลบๆ ในหัวที่เคยบอกว่า 'เรนแย่งทุกอย่างไปจากเธอ' ถูกลบทับด้วยน้ำตาและสายตาแห่งความรักของน้องสาว)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เรนไม่ได้ผิดอะไรเลย... ความจริงคือเราเองต่างหากที่ปิดกั้นตัวเองจากทุกคน)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เรน... พี่ขอโทษนะ พี่ต่างหากที่เป็นฝ่ายกั้นตัวเองออกมา พี่คิดมาตลอดว่าพี่เป็นภาระของทุกคน... ขอบใจนะที่ไม่ทิ้งพี่", "Nia", "good1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พี่เนียไม่ใช่ภาระนะ! หนูรักพี่เนียที่สุดเลย! จากนี้มีอะไรเรามาช่วยกันนะ!", "Rein", "Idle", -15f, 4, SpriteAction.Jump)
            }),
            CreateChoice("พี่ไม่ได้เป็นอะไรหรอกเรน ขอบใจสำหรับขนมนะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พี่เนีย... พี่อย่าฝืนยิ้มแบบนั้นสิ หนูดูออกนะ...", "Rein", "Idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("เธอจะไปเข้าใจอะไร! เธอเป็นเด็กโปรดของพ่อแม่นี่ ทุกคนรักเธอหมดนั่นแหละ!", "Nia", "bad1", 0f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("ทำไมพี่เนียต้องคิดแบบนี้ด้วย... หนูแค่เป็นห่วงพี่เองนะ!", "Rein", "Idle", 15f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(มองออกไปนอกหน้าต่างห้องนอน... แสงอาทิตย์ยามเย็นกำลังตกดิน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(หัวใจที่เคยหนักอึ้งเริ่มรู้สึกเบาลงเล็กน้อย... ม่านบังตาค่อยๆ เปิดออกทีละนิดแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rain] อัปเดต Day 11 สำเร็จ");
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
