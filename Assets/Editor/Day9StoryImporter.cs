using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day9StoryImporter
{
    static Day9StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay9;
    }

    [MenuItem("Tools/Import Day 9 Story (Park, Shia, Vipar)")]
    public static void ImportAllDay9()
    {
        Debug.Log("⏳ [Day9StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 9...");
        ImportPrak();
        ImportShia();
        ImportVipar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day9StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 9 (Park, Shia, Vipar) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportPrak()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 9);
        day.dayTitle = "DAY 9";
        day.dayNumber = 9;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ผลสอบออกแล้ว... ผ่านคาบเส้นพอดีเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(เดินเข้ามาดูบอร์ดคะแนน แล้วหันมามองเนีย)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ผ่านฉิวเฉียดเลยนะเนีย เกือบไม่รอดแล้วไหมล่ะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คะแนนคาบเส้นแบบนี้ ถ้าเป็นตอนซ้อมกีฬาเขาเรียกว่าแค่รอดตัวไปวันๆ นะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ถ้าคราวหน้าไม่พยายามให้มากกว่านี้ เธอร่วงแน่", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ตรวจจับความล้มเหลว... คำว่า \"รอดตัวไปวันๆ\" กับ \"ร่วงแน่\" ดังสะท้อนในหัว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันมันก็แค่คนโชคดีที่รอดมาได้... ไม่ได้มีความสามารถอะไรเลยสักนิด)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ฉันก็พยายามในแบบของฉันแล้วนะ!", "Nia", "bad1", 0f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("พยายามแล้วได้แค่นี้ ก็แปลว่ายังพยายามไม่พอไง", "Park", "idle", 10f, 4, SpriteAction.None)
            }),
            CreateChoice("อืม... ฉันมันห่วยเองแหละ", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ไม่ได้บอกว่าห่วย แค่บอกให้พยายามเพิ่ม", "Park", "idle", 10f, 4, SpriteAction.None)
            }),
            CreateChoice("(นิ่งเงียบ ก้มหน้ามองพื้น)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เงียบอีกละ เออ ช่างเถอะ", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ฉันไปเตะบอลต่อละ อย่าลืมล่ะ คราวหน้าก็อย่าให้เป็นภาระรินมันอีกล่ะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ภาระ... อีกแล้ว คำนี้อีกแล้ว)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 9 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 9);
        day.dayTitle = "DAY 9";
        day.dayNumber = 9;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เหอะ! ผ่านฉิวเฉียดแบบนี้ ภูมิใจมากไหมเนีย?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่า... พอเถอะ เนียเขาก็สอบผ่านแล้วนี่", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ผ่านเพราะรินแทบจะป้อนเข้าปากให้ไง! ถ้าไม่มีรินคอยประคบประหงม คนอย่างเธอจะทำอะไรเองได้บ้าง?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เอาแต่ทำหน้าซึม เรียกร้องความสนใจไปวันๆ", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เธอรู้ตัวบ้างไหมว่าเธอเป็นตัวดูดพลังงานคนอื่น? รินต้องอดหลับอดนอนมาติวให้คนที่เป็นเหมือนปลิงแบบเธอ!", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("!!", "Nia", "bad1", 0f, 1, SpriteAction.Shake),
            CreateLine("(ปลิง... ตัวดูดพลังงาน...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(Hippocampus ขุดภาพทุกอย่างขึ้นมาฉายซ้ำ ภาพแม่ที่ถอนหายใจ ภาพพ่อที่หงุดหงิด ภาพรินที่เหนื่อยล้า...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(SFG พังทลาย... ฉันไม่สามารถหาเหตุผลอะไรมาโต้แย้งได้เลย เสียงในหัวมีแต่คำว่า 'ฉันมันเป็นภาระ' 'ฉันทำลายชีวิตทุกคน')", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉัน... ไม่ควรอยู่ตรงนี้เลยด้วยซ้ำ)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ฉันไม่ได้ขอให้รินมาช่วยสักหน่อย!!", "Nia", "bad1", 0f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เห็นไหม! นอกจากจะเป็นภาระแล้วยังเนรคุณอีก!", "Shia", "idle", 10f, 4, SpriteAction.Shake)
            }),
            CreateChoice("ขอโทษ... ฉันขอโทษ ฉันมันเป็นตัวภาระจริงๆ...", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนีย... ไม่ใช่อย่างนั้นนะ...", "Hong", "", 10f, 6, SpriteAction.None)
            }),
            CreateChoice("(น้ำตาไหลออกมา... ทนไม่ไหวแล้ว)", "Nia", "bad1", 15f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ร้องไห้เหรอ? บีบน้ำตาเก่งนี่ คิดว่าทำแบบนี้แล้วคนอื่นจะสงสารเหรอ?", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ชีอ่า! หยุดเดี๋ยวนี้เลยนะ! มันเกินไปแล้ว!", "Hong", "", 0f, 6, SpriteAction.Shake),
            CreateLine("(ฉันวิ่งหนีออกมา... ไม่อยากได้ยินอะไรอีกแล้ว ไม่อยากรู้สึกอะไรอีกแล้ว)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 9 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 9);
        day.dayTitle = "DAY 9";
        day.dayNumber = 9;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(วิ่งร้องไห้มาตามโถงทางเดินจนชนเข้ากับใครบางคน)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("โอ๊ย! เดินระวังหน่อยสิ! นี่มันโถงทางเดินนะ ไม่ใช่สนามเด็กเล่น", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อ้าว... เธอ... ริน? ไม่สิ... เลขที่ 8 ใช่ไหม?", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("มาร้องไห้อะไรตรงนี้? สอบตกหรือไง? ครูบอกแล้วใช่ไหมว่าถ้าไม่ตั้งใจก็ช่วยไม่ได้", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ครูวิภา... ครูยังจำชื่อฉันไม่ได้ด้วยซ้ำ...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(mPFC มืดสนิท กระจกสะท้อนตัวตนแตกละเอียด... ฉันไม่มีตัวตนจริงๆ ไม่มีใครรู้จักฉัน ไม่มีใครต้องการฉัน)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("หนูสอบผ่านค่ะ... และหนูชื่อเนีย เลขที่ 7!", "Nia", "bad1", 0f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("ผ่านแล้วจะมาร้องไห้ฟูมฟายทำไม? ไร้สาระจริงๆ รีบกลับไปเข้าชั้นเรียนได้แล้ว", "Vipar", "idle", 5f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูขอโทษค่ะ... หนูมันแย่เอง", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("รู้ตัวก็ดี คราวหลังก็หัดเดินดูตาม้าตาเรือบ้าง", "Vipar", "idle", 10f, 4, SpriteAction.None)
            }),
            CreateChoice("(ไม่ตอบอะไร วิ่งหนีผ่านครูวิภาไปเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("นี่! ครูพูดยังไม่จบนะ กริยาแบบนี้ใช้ได้ที่ไหน!", "Vipar", "idle", 5f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(ฉันวิ่งเข้าไปขังตัวเองในห้องน้ำ...)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ไม่มีใครต้องการฉัน... ไม่มีใครเห็นฉัน... ฉันมันไม่มีค่าอะไรเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(...ฉันอยากหายไปจริงๆ)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 9 สำเร็จ");
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
