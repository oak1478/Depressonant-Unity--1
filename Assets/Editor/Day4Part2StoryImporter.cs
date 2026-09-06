using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day4Part2StoryImporter
{
    static Day4Part2StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay4Part2;
    }

    [MenuItem("Tools/Import Day 4 Part 2 Story (Jin, Den)")]
    public static void ImportAllDay4Part2()
    {
        Debug.Log("⏳ [Day4Part2StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 4 Part 2...");
        ImportJin();
        ImportDen();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day4Part2StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 4 Part 2 ลง Prefab (Jin, Den) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportJin()
    {
        string path = "Assets/Object/Characters/NPC_Jin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 4);
        day.dayTitle = "DAY 4";
        day.dayNumber = 4;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("รดน้ำต้นไม้หรอค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("อ้าว ! แม่จิน นั่นหนูเรนใช่ไหม", "Ben", "", 0f, 6, SpriteAction.None),
            CreateLine("ไม่ใช่สิ รึว่าคงเป็นเพื่อนของเรนสินะ ?", "Ben", "", 0f, 6, SpriteAction.None),
            CreateLine("หนูเนียไงคะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("สงสัยลุงเค้าจะหลง ๆ ลืม ๆ ปล่อยไปเถอะ นั่นจะไปไหนเหรอ เนีย ?", "Jin", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("จะไปหาเรนนะค่ะ ที่บ้านของเดน", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อ้าวเหรอ ระวังรถระด้วยล่ะ", "Jin", "idle", 0f, 4, SpriteAction.None),
                CreateLine("เป็นเด็กดีจังนะเรา ดูแลน้องดี ๆ ล่ะ", "Ben", "", 0f, 6, SpriteAction.None),
                CreateLine("เดินทางปลอดภัยนะ หนู", "Ben", "", -10f, 6, SpriteAction.Jump),
                CreateLine("งั้นหนูขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("ไม่มีอะไรคะ แค่เดินเล่นแถวนี้แหละ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ดีแล้วๆ ออกมารับแดดบ้าง ผิวจะได้รับวิตามิน", "Jin", "idle", -5f, 4, SpriteAction.None),
                CreateLine("งั้นหนูขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("(เดินหนีออกมา)", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อะไรของเด็กนั่น ?", "Ben", "", 0f, 6, SpriteAction.None),
                CreateLine("ช่างมันเถอะ", "Jin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(ไม่รู้จะพูดอะไรดี)", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Jin & Ben] อัปเดต Day 4 สำเร็จ");
    }

    private static void ImportDen()
    {
        string path = "Assets/Object/Characters/NPC_Den.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 4);
        day.dayTitle = "DAY 4";
        day.dayNumber = 4;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าวพี่เนีย ! สวัสดีครับ วันนี้ก็ออกมาเล่นกันไหม วันนี้เราเล่นทายปัญหาคนดังกันอยู่", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("สวัสดีเด็กๆ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("สวัสดีค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("ดีครับ", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("พี่ มานี่สิ เพื่อน ๆ กำลังถามถึงพี่อยู่พอดีเลย", "Rein", "Idle", 0f, 3, SpriteAction.Jump),
            CreateLine("ไม่ใช้พูดเรื่องคนดังหรอ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ก็พี่นั่งแหละ คนดังมีปัญหา", "Rein", "Idle", 0f, 3, SpriteAction.None),
            CreateLine("นี่พี่เป็นคนดังมีปัญหาเหรอ ?", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ใช่ค่ะพี่เนีย", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("เมื่อกี้เราเพิ่งคุยกันว่า พี่เนียดูเหมือนตัวละครในนิยายที่ลึกลับดี แบบว่า ปีนึงจะเห็นสักครั้ง", "Sasha", "", 0f, 6, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("นี่กำลังชมหรือกำลังหลอกด่าพี่กันแน่เนี่ย", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เปล่าครับพี่เนีย พวกเราหมายถึง", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ปกติเรารู้จักพี่จากการเล่าของเรน แต่ว่าพี่นะ ไม่ค่อย รึแทบไม่เคยออกมาจากบ้านมาให้เราเห็นเลย", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ทั้ง ๆ ที่เราอยู่บ้านใกล้กัน แต่ผมเห็นพี่นับครั้งได้เลยนะ เราคิดว่าพี่ออกไปข้างนอกบ่อยละมั้ง แต่เรนก็ยืนยันว่าพี่อยู่ในบ้าน", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("และว่างด้วย", "Den", "idle", 5f, 4, SpriteAction.None),
                CreateLine("(ฉันไม่ควรคิดมากกับคำพูดของเด็กแต่ว่า)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("ลึกลับน่าค้นหาดีไม่ใช่เหรอ เป็นคาแรคเตอร์พิเศษไง", "Rein", "Idle", 0f, 3, SpriteAction.None),
                CreateLine("พี่เนียเป็นแบบนี้ น่ารักจะตาย", "Rein", "Idle", 0f, 3, SpriteAction.None),
                CreateLine("(ยิ้ม)", "Rein", "Idle", -15f, 3, SpriteAction.Jump),
                CreateLine("ขอบคุณนะเรน", "Nia", "good1", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("หา? ไปเอาความคิดนั้นจากไหน", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อึก! คือว่า...", "Sasha", "", 0f, 6, SpriteAction.None),
                CreateLine("แบบว่า", "Sasha", "", 0f, 6, SpriteAction.None),
                CreateLine("เข้าใจผิด ทุกคนแต่เป็นห่วงพี่นะ", "Egon", "", 0f, 7, SpriteAction.None),
                CreateLine("พี่ รังแกเด็ก จะไปฟ้องแม่", "Rein", "Idle", 5f, 3, SpriteAction.Shake),
                CreateLine("พี่ขอโทษ ไม่ได้ตั้งใจจะให้กลัว", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("พวกเราหมายถึง ปกติเรารู้จักพี่จากการเล่าของเรนแต่ว่าพี่นะ ไม่ค่อย รึแทบไม่เคยออกมาจากบ้านมาให้เราเห็นเลย", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ทั้ง ๆ ที่เราอยู่บ้านใกล้กัน แต่ผมเห็นพี่นับครั้งได้เลยนะเราคิดว่าพี่ออกไปข้างนอกบ่อยละมั้ง แต่เรนก็ยืนยันว่าพี่อยู่ในบ้านนะครับ", "Den", "idle", 0f, 4, SpriteAction.None),
                CreateLine("อย่างนี้นี่เอง", "Nia", "idle", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("พูดซะพี่ดูเป็นยอดมนุษย์เลยนะ แบบตัวละครเงาอะไรพวกนี้เหรอ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ไม่ค่ะ เป็นแบบตัวประกอบมากกว่า", "Sasha", "", 0f, 6, SpriteAction.None),
                CreateLine("แค่เรนมาเล่าเรื่องพี่บ่อย ๆ นะ", "Sasha", "", 0f, 6, SpriteAction.None),
                CreateLine("ลึกลับน่าค้นหาดีไม่ใช่เหรอ เป็นคาแรคเตอร์พิเศษไง", "Rein", "Idle", 0f, 3, SpriteAction.None),
                CreateLine("พี่เนียเป็นแบบนี้ น่ารักจะตาย", "Rein", "Idle", 0f, 3, SpriteAction.None),
                CreateLine("(ยิ้ม)", "Rein", "Idle", -15f, 3, SpriteAction.Jump),
                CreateLine("ขอบคุณนะเรน", "Nia", "good1", 0f, 1, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(พยักหน้า)", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("พี่ต้องไปโรงเรียนสินะพรุ่งนี้ โรงเรียนของพี่เป็นไงบ้าง พี่บ้านจะให้ผมเข้าโรงเรียนเดียวกันในอนาคตนะ", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("อีกอนสนใจเหรอ ก็ใช้ได้นะ ในเรื่องการเรียนน่ะ ห้องสมุดก็เงียบดีด้วย", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ผมชอบที่เงียบ ๆ", "Egon", "", 0f, 7, SpriteAction.None),
            CreateLine("คนก็ไม่ค่อยเยอะด้วย", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พี่มักชอบทำอะไรในโรงเรียนค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("เออ คือ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("พี่น่ะสุดยอดนะ!", "Rein", "Idle", 0f, 3, SpriteAction.Jump),
            CreateLine("เป็นบรรณารักษ์ห้องสมุดด้วย ถึงจะชอบบ่นเรื่องที่นั่นก็เถอะ", "Rein", "Idle", -10f, 3, SpriteAction.None),
            CreateLine("...", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("ละก็", "Rein", "Idle", 0f, 3, SpriteAction.None),
            CreateLine("เอาเป็นว่าพี่ขอตัวก่อนนะ เรนเองก็รีบ ๆ กลับละ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("หาาา~~~~ หนูขอต่อเวลา", "Rein", "Idle", 0f, 3, SpriteAction.None),
            CreateLine("ก็ได้ เดี๋ยวพี่บอกแม่เอง บ้ายบาย", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("บ้ายบายครับ", "Den", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("บ้ายบายค่ะ", "Sasha", "", 0f, 6, SpriteAction.None),
            CreateLine("บายครับ", "Egon", "", 0f, 7, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Den & Kids] อัปเดต Day 4 สำเร็จ");
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
