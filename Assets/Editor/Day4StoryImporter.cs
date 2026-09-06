using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day4StoryImporter
{
    static Day4StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay4;
    }

    [MenuItem("Tools/Import Day 4 Story (Dad, Mom)")]
    public static void ImportAllDay4()
    {
        Debug.Log("⏳ [Day4StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 4...");
        ImportDad();
        ImportMom();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day4StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 4 ลง Prefab (Dad, Mom) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportDad()
    {
        string path = "Assets/Object/Characters/NPC_Dad.prefab";
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
            CreateLine("...", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("เตรียมของไปโรงเรียนพรุ่งนี้หรือยัง?", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ละก็อย่าก่อเรื่องให้ครูอีกล่ะ", "Dad", "idle", 0f, 6, SpriteAction.None),
            CreateLine("ได้ค่ะ ว่าจะไปเตรียมตอนเย็น ๆ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ตอนนี้เลย !!", "Dad", "idle", 0f, 6, SpriteAction.Shake),
            CreateLine("จะทิ้งช่วงไปทำไม", "Dad", "idle", 5f, 6, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("เข้าใจแล้วค่ะ จะไปทำเดี๋ยวนี้", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ดีแล้ว", "Dad", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("เห้อ ไปดีกว่า", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ลืมมารยาททรามแล้วหรอ", "Dad", "idle", 0f, 6, SpriteAction.None),
                CreateLine("เฮ้ย !!", "Dad", "idle", 10f, 6, SpriteAction.Shake),
                CreateLine("ชิ้ !", "Dad", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("ขอหนูพักสักนิดไม่ได้รึไง ?", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จะพักอะไรนักหนา", "Dad", "idle", 5f, 6, SpriteAction.Shake),
                CreateLine("หน้าที่ของลูกคือเรียนไม่ใช่เหรอ ไปจัดการซะ", "Dad", "idle", 0f, 6, SpriteAction.None),
                CreateLine("ได้ค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ค่อยทำก็ได้แท้ ๆ", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Dad] อัปเดต Day 4 สำเร็จ");
    }

    private static void ImportMom()
    {
        string path = "Assets/Object/Characters/NPC_Mom.prefab";
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
            CreateLine("ถ้าว่างนักก็ไปดูน้องหน่อย วันนี้เรนออกไปเล่นข้างนอก", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("อย่าปล่อยเวลาให้ไร้ประโยชน์สิ เดี๋ยวฉันจะเตรียมอาหารไว้ให้", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("ได้ค่ะ แล้วน้องอยู่ไหนคะ", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("บ้านของเดนนั่นแหละ เห็นว่าจะไปช่วยกันทำอาหาร น่าดีใจจริง ๆ", "Mom", "", 0f, 4, SpriteAction.None),
            CreateLine("นี่ อย่างน้อยก็เอาแบบน้องแกหน่อยสิ แทนที่จะนั้งฟังอะไรไม่รู้ที่บ้านทั้งวัน", "Mom", "", 5f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ก็แค่เอาไปฟังแก้เบื่อเอง", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ยังจะเถียงอีก ไปได้แล้ว ไป !", "Mom", "", 5f, 4, SpriteAction.Shake)
            }),
            CreateChoice("ได้ค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>()),
            CreateChoice("หนูขอโทษค่ะ งั้นขอออกไปหาน้องก่อน", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ปกติไม่ยอมไปนะเนี่ย", "Mom", "", 0f, 4, SpriteAction.None),
                CreateLine("ดีมาก", "Mom", "", -5f, 4, SpriteAction.None),
                CreateLine("รีบไปรีบมาล่ะ ก่อนอาหารจะเย็นหมด", "Mom", "", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Mom] อัปเดต Day 4 สำเร็จ");
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
