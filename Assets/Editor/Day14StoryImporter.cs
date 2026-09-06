using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day14StoryImporter
{
    static Day14StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay14;
    }

    [MenuItem("Tools/Import Day 14 Story (Den, Park, Shia, Vipar)")]
    public static void ImportAllDay14()
    {
        Debug.Log("⏳ [Day14StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 14...");
        ImportDen();
        ImportPark();
        ImportShia();
        ImportVipar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day14StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 14 (Den, Park, Shia, Vipar) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportDen()
    {
        string path = "Assets/Object/Characters/NPC_Den.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 14);
        day.dayTitle = "DAY 14";
        day.dayNumber = 14;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(กำลังนั่งขมวดคิ้วอยู่กับซาช่าและอีกอนตรงโต๊ะม้านั่ง พร้อมกระจายการ์ดเกมทายปัญหาบนโต๊ะ)", "Den", "idle", 0f, 4, SpriteAction.None),
            CreateLine("โอ๊ย! ข้อนี้ยากชะมัด! \"อะไรเอ่ย ยิ่งตัดยิ่งยาว ยิ่งถอยยิ่งใกล้\"?", "Den", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เดนคิดไม่ออกเองมากกว่า! พี่เรนก็ไม่อยู่ด้วยสิวันนี้...", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("พี่เนีย... (หันไปเห็นเนียเดินผ่านพอดี)", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("อ้าว! พี่เนีย! มาช่วยพวกเราหน่อยดิ! เกมทายปัญหากลุ่มนี้ถ้าพวกเราแพ้เด็กกลุ่มโน้น โดนล้อแน่เลย!", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(มองดูการ์ดเกมบนโต๊ะ... สมองส่วน mPFC และ SFG ประมวลผลได้อย่างราบรื่น)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เมื่อก่อนเราคงคิดว่าตัวเองไร้ประโยชน์จนไม่กล้าช่วย... แต่จริงๆ แค่โจทย์ปัญหาพวกนี้ เราอ่านเจอในหนังสือบ่อยมากๆ)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("คำตอบคือ \"ถนน\" ไงล่ะเดน... ยิ่งตัดถนนก็ยิ่งยาว ยิ่งถอยรถกลับก็ยิ่งใกล้จุดเริ่มต้น", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ตาโต) เฮ้ย! จริงด้วย! พี่เนียโคตรเจ๋งเลย!", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("หนูบอกแล้วว่าพี่เนียเป็นตัวลึกลับสุดยอด! งั้นมาเล่นรอบตัดสินเป็นหัวหน้าทีมให้พวกหนูหน่อยสิคะพี่!", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("(OFC ประมวลผลความรู้สึกของการถูกยอมรับได้อย่างเต็มที่... ม่านมัวในหัวแทบไม่เหลืออยู่แล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ได้สิ! รอบนี้พี่จะพาพวกเราชนะเอง มาเริ่มกันเลย!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เย้! มีพี่เนียอยู่ ทีมเราชนะชัวร์!", "Den", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("พี่ลองช่วยดูให้ก็ได้จ้ะ... แต่พวกเธอต้องช่วยกันคิดด้วยนะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("รับทราบค่ะพี่เนีย!", "Sasha", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("พี่แค่นึกออกพอดี... พวกเธอเล่นกันต่อเถอะ พี่ไม่ค่อยถนัดเท่าไหร่", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("โธ่ พี่เนีย... ลองเล่นด้วยกันก่อนดิครับ", "Den", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ชนะแล้ว... ขอบคุณครับพี่เนีย", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("สุดยอดไปเลย! วันหลังมาเล่นด้วยกันอีกนะครับพี่เนีย!", "Den", "idle", 0f, 4, SpriteAction.Jump)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Den] อัปเดต Day 14 สำเร็จ");
    }

    private static void ImportPark()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 14);
        day.dayTitle = "DAY 14";
        day.dayNumber = 14;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ยืนกอดอกมองเนียที่เพิ่งเดินแยกมาจากกลุ่มเด็กๆ)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไง เนีย เข้ากับเด็กๆ ได้ดีกว่าที่คิดนี่", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เมื่อก่อนเห็นทำหน้าเหมือนแบกโลกไว้คนเดียว เดี๋ยวนี้ดูเข้าถึงง่ายขึ้นเยอะเลยนะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("แบบนี้สิ ค่อยเหมือนสมาชิกในชุมชนเดียวกันหน่อย", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ไม่ตรวจจับเป็นความขัดแย้งเชิงลบอีกต่อไป... คำพูดตรงๆ ของป้ากกลายเป็นคำทักทายที่จริงใจ)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(การกล้าก้าวออกจากมุมมืด ทำให้เราเริ่มมองเห็นความเชื่อมโยงกับคนรอบข้างชัดขึ้น)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ก็แค่ออกมาซึมซับบรรยากาศบ้างน่ะป้าก คนเรามันก็ต้องปรับตัวกันบ้างจริงไหม?", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มขำ) พูดจาดีนี่! เออ แบบนี้แหละดูมีความมั่นใจขึ้นเยอะ!", "Park", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ก็เด็กๆ น่ารักดีน่ะ... ไม่มีอะไรหรอก", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เออ เห็นเธออารมณ์ดีได้ก็ดีแล้ว", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("นายยุ่งอะไรด้วยล่ะป้าก?", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เอ้า แค่ทักทายเฉยๆ เอง จะหงุดหงิดทำไมเนี่ย", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ฉันไปซ้อมบาสละ ไว้เจอกัน", "Park", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 14 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 14);
        day.dayTitle = "DAY 14";
        day.dayNumber = 14;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(กำลังยืนถกเครียดกับชีอ่าเรื่องจัดเวรทำความสะอาดห้องเรียน)", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("เนีย... พอดีตารางเวรทำความสะอาดมันซ้ำซ้อนกันน่ะ ชีอ่าบอกว่าจัดยาก แต่ฉันก็คิดไม่ออก", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ก็มันแบ่งคนไม่ลงตัวนี่หงส์! แถมนายป้ากก็ชอบเบี้ยวเวรซ้อมบาสอีก", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(เดินเข้าไปดูตารางบนกระดาษ แล้วใช้ทักษะการจัดระเบียบจากงานห้องสมุดมองภาพรวม)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("ชีอ่า... ลองย้ายป้ากไปอยู่วันพฤหัสบดีสิ แล้วสลับฉันมาอยู่วันพุธแทน งานจะกระจายเท่ากันพอดี ไม่ต้องมีใครทำเกินด้วย", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(มองตารางที่เนียเสนอแล้วชะงักไป นิ่งคิดอยู่ครู่หนึ่ง)", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("...เออแฮะ จัดแบบนี้ก็ลงตัวดีนี่ ทำไมฉันคิดไม่ได้หว่า", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("โห เนีย! จัดระบบเก่งมากเลย! ขอบใจนะ!", "Hong", "", 0f, 6, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เรื่องเล็กน้อยน่าชีอ่า หงส์ มีอะไรให้ช่วยจัดระบบบอกฉันได้เสมอนะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อืม... ขอบใจนะเนีย ครั้งนี้เธอช่วยได้เยอะจริงๆ", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("พอดีฉันชินกับการจัดหมวดหมู่ในห้องสมุดน่ะ...", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("สมกับเป็นบรรณารักษ์มือหนึ่งจริงๆ!", "Hong", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("เสร็จแล้วใช่ไหม? งั้นฉันขอตัวก่อนนะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อ้าว ไปซะละ... ยังไม่ทันได้ขอบคุณดีๆ เลย", "Shia", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นเอาตามตารางที่เนียบอกละกันหงส์", "Shia", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 14 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 14);
        day.dayTitle = "DAY 14";
        day.dayNumber = 14;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินตรวจความเรียบร้อยหน้าห้องเรียน พอเห็นเนียก็ยิ้มให้เล็กน้อย)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อ้าว เนีย เลขที่ 7! วันนี้มาเตรียมตัวเข้าเรียนไวดีนี่จ๊ะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เรื่องรายงานเคมีคราวหน้า ถ้าเธออยากลองเป็นหัวหน้ากลุ่มรวบรวมเล่ม บอกครูได้เลยนะ ครูเห็นความตั้งใจของเธอแล้ว", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ฟังคำพูดของครูวิภาอย่างชัดเจน โดยไม่มีเสียงค้านในหัวเลยสักนิด)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(mPFC สะท้อนภาพตัวเองที่มีความสามารถและได้รับความไว้วางใจจากครูอย่างสมบูรณ์)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครูวิภา! หนูยินดีช่วยรวบรวมเล่มรายงานให้เพื่อนๆ ค่ะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เยี่ยมมากจ้ะเนีย! ครูเชื่อว่าเธอทำได้ดีแน่นอน!", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณค่ะครู... เดี๋ยวหนูขอลองคิดดูก่อนนะคะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะ มีเวลาตัดสินใจ ไม่ต้องรีบนะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูเกรงว่าจะทำได้ไม่ดีพอค่ะครู...", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อย่าเพิ่งด้อยค่าตัวเองสิเนีย เธอทำได้อยู่แล้ว", "Vipar", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(เดินกลับไปนั่งที่โต๊ะเรียนด้วยความรู้สึกอบอุ่นในใจ)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(หัวใจและสมองของเรา... กำลังกลับมาเชื่อมโยงกับผู้คนรอบข้างทีละนิดแล้วจริงๆ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 14 สำเร็จ");
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
