using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day2StoryImporter
{
    static Day2StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay2;
    }

    [MenuItem("Tools/Import Day 2 Story (Rain, Jin, Den)")]
    public static void ImportAllDay2()
    {
        Debug.Log("⏳ [Day2StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 2...");
        ImportRain();
        ImportJin();
        ImportDen();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day2StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 2 ลง Prefab ทั้ง 3 ตัวเสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportRain()
    {
        string path = "Assets/Object/Characters/NPC_Rein.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 2);
        day.dayTitle = "DAY 2";
        day.dayNumber = 2;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เปิดทีวีช่องอะไรเนี่ย ทำไมไม่เปิดช่องที่ดูประจำล่ะ ?", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พี่น่าเบื่อ หนูเปิดตั้งไว้งั้นแหละ หนูจะเล่นเกมไปด้วย", "Rein", "Idle", 5f, 4, SpriteAction.Shake)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เล่นเกมไปดูทีวีไป จะรู้เรื่องหรอ ?", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พี่~~ จุกจิกชะมัด", "Rein", "Idle", 5f, 4, SpriteAction.None)
            }),
            CreateChoice("งั้นพี่ขอนั่งด้วยคนสิ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ได้สิ มาเลย ๆ", "Rein", "Idle", -5f, 4, SpriteAction.Jump)
            }),
            CreateChoice("(พยายามเมินดีกว่า)", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>())
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("เอ้ะ!!", "Nia", "idle", 0f, 1, SpriteAction.Jump),
            CreateLine("รูปภาพนั่นอะไรนะ การบ้านหรอ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ประมาณนั้นพี่", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("หือ~~ วาดสวยดีนะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("!!!!!!!!", "Rein", "Idle", 0f, 4, SpriteAction.Shake),
            CreateLine("ก็แค่ระบายสีไปมั่วๆ แหละ พี่ไปทำอย่างอื่นเถอะน่า", "Rein", "Idle", 0f, 4, SpriteAction.None),
            CreateLine("แต่ก็ ขอบคุณค่ะ", "Rein", "Idle", -5f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rein] อัปเดต Day 2 สำเร็จ");
    }

    private static void ImportJin()
    {
        string path = "Assets/Object/Characters/NPC_Jin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 2);
        day.dayTitle = "DAY 2";
        day.dayNumber = 2;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว เนีย! ตื่นสายเชียวนะวันนี้", "Jin", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เอ๊ะ!", "Jin", "idle", 0f, 6, SpriteAction.Jump),
            CreateLine("หรือว่าฉันจำผิด หนูเรนรึเปล่านะ?", "Jin", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ป้าพูดอะไรน่ะป้าจิน ?", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("แต่ก็สวัสดีค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ฮะฮาฮา สงสัยฉันจะแก่แล้วจริงๆ", "Jin", "idle", 0f, 6, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("หนูมีน้องสาวชื่อเรนไงค่ะ ป้าลืมแล้วหรอ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("งั้นหรอ ฉันคงเห็นน้องเรนนั้นบ่อยกว่าแกละนะ", "Jin", "idle", 0f, 6, SpriteAction.None),
                CreateLine("ก็แกนะ เนีย", "Jin", "idle", 0f, 6, SpriteAction.None),
                CreateLine("เอาแต่หมกตัวอยู่ในบ้าน ไม่ออกมาอาบแสงให้เห็นเลยนิ จนฉันลืมแกไปละ", "Jin", "idle", 5f, 6, SpriteAction.None),
                CreateLine("ถ้าไม่มีอะไรแล้วหนูขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("บ๊ายบายจ้ะ", "Jin", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("ถ้าไม่มีอะไรแล้วหนูขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("บ๊ายบายจ้ะ", "Jin", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("ป้าลืมหนูได้ไง", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("บ้านอยู่ใกล้กันแท้ ๆ", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("พูดงี้กับผู้ใหญ่ได้หรอ", "Jin", "idle", 0f, 6, SpriteAction.None),
                CreateLine("ไอเด็กนี่!!", "Jin", "idle", 10f, 6, SpriteAction.Shake),
                CreateLine("ขอโทษค่ะ !!", "Nia", "bad1", 0f, 1, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(คนแก่อะไรเนี่ย!)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Jin] อัปเดต Day 2 สำเร็จ");
    }

    private static void ImportDen()
    {
        string path = "Assets/Object/Characters/NPC_Den.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 2);
        day.dayTitle = "DAY 2";
        day.dayNumber = 2;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("พี่เนีย! มาเล่นด้วยกันไหม? เรากำลังขาดคนกันอยู่!", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("สวัสดีเดน แล้วมันเล่นยังไงน่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พี่ไม่รู้จักหรอ", "Den", "idle", 0f, 4, SpriteAction.None),
            CreateLine("งั้นก็แค่เริ่มเกมส์ก่อน ที่เหลือเดี๋ยวก็รู้เอง", "Den", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ทักทายพี่เขาดี ๆ สิเดน!", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("พี่เขาอุตส่าห์สวัสดีก่อนนะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("สวัสดีค่ะพี่", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("...", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("พี่ชื่ออะไรนะค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("(เจ้าเด็กนี่!)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("พี่เนียเอง ซาช่า", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ค่ะพี่เนีย วันนี้มาแปลกนะคะ ปกติไม่ออกมาข้างนอกนิ แต่ก็มาเล่นเกมกันเถอะค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("เริ่มเกมเดี๋ยวก็เล่นเป็นเองค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("เออ", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("สวัสดีครับ", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("สวัสดี", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("งั้น ฉันเล่นด้วยก็ได้", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("เรนล่ะครับ? ปกติออกมาเล่นกับเราตลอดเลยนะ", "Den", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เรนยุ่งอยู่น่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เมื่อกี้ยังเห็นนั่งดูทีวีอยู่", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("วันนี้ขยันผิดปกติแฮะ", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ถ้านางมาเล่นด้วยก็สนุกกว่านี้", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("แต่เอาเถอะ เรามีพี่เนียอยู่", "Den", "idle", -5f, 4, SpriteAction.Jump)
            }),
            CreateChoice("พวกเธอจำผิดหรือเปล่า ? เรนไม่ค่อยชอบออกนอกบ้านนะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จะจำผิดได้ไงคะพี่เนีย ?", "Sasha", "", 0f, 6, SpriteAction.None),
                CreateLine("ก็เรนชอบมาวาดรูปเล่นกับหนูประจำ พี่เนียไหวไหมคะเนี่ย?", "Sasha", "", 5f, 6, SpriteAction.None)
            }),
            CreateChoice("ไม่แน่ใจเหมือนกัน", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("พี่ไม่ได้คุยกับน้องเท่าไหร่", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("เรนไม่คุยด้วยหรอครับ ?", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ไม่ต้องห่วงนะครับ วันนี้พวกผมจะเป็นเพื่อนเล่นด้วยเอง", "Den", "idle", -10f, 4, SpriteAction.Jump)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ชนะแล้ว", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("แพ้ซะแล้ว", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("ไม่เป็นไรนะพี่ครั้งแรกก็งี้แหละ", "Den", "idle", 0f, 4, SpriteAction.None),
            CreateLine("งั้นพี่กลับก่อนนะ โชคดีทุกคน", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("บ๊ายบายครับ", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("บ๊ายบายค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("...", "Egon", "", 0f, 7, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Den] อัปเดต Day 2 สำเร็จ");
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
