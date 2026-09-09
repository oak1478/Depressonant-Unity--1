using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day6StoryImporter
{
    static Day6StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay6;
    }

    [MenuItem("Tools/Import Day 6 Story (Rin, Vipar, Shia, Momon)")]
    public static void ImportAllDay6()
    {
        Debug.Log("⏳ [Day6StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 6...");
        ImportRin();
        ImportVipar();
        ImportShia();
        ImportMomon();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day6StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 6 (Rin, Vipar, Shia, Momon) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! วันนี้งานจัดชั้นหนังสือในห้องสมุดเยอะเลยถ้าได้เธอช่วย ต้องเสร็จทันแน่ๆ", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("อื้ม... เดี๋ยวฉันช่วยเอง", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(ที่จริงฉันเป็นบรรณารักษ์นะ ฉันต้องทำอยู่แล้ว)", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("(แต่ทำได้ดีแล้วยังไงล่ะ? ที่จริงก็ต้องให้รินช่วยเตือนตลอด บางทีนางก็ทำงานที่ฉันลืมด้วย)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("เนีย? ยืนเหม่ออะไรอยู่เหรอ? เป็นอะไรรึเปล่า?", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ไม่มีอะไร", "Nia", "idle", 0f, 1, SpriteAction.None, null),
            CreateChoice("เปล่าหรอก แค่กำลังคิดว่าจะเริ่มจัดหมวดไหนก่อนดี", "Nia", "good1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("มีความตั้งใจดีนะ", "Rin", "idle", -5f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ที่จริงแล้ว ฉันจัดคนเดียวได้น่า! เดิมทีก็งานฉันนะ", "Nia", "bad1", 0f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("โถ่ อุส่าห์จะช่วยแท้ ๆ", "Rin", "idle", 10f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นเหรอ... งั้นฉันฝากด้วยนะ พอดีครูวิภาเร่งฉันไปรอบนึงแล้วนะ", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว เนีย วันนี้จัดหนังสือเสร็จหรือยัง?", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ยังเลยค่ะครู", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("อ้อ... งั้นเหรอ บางทีรินอาจช่วยได้เยอะเลย ลองไปถามดูซิ แล้วก็ฉันไปรู้บางอย่างมานะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ดูเหมือนเธอจะมีอาการซึมเศร้าเล็กน้อยซินะ ไม่ต้องห่วงหรอกนะฉันไม่บอกใครหรอก", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("...", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(พ่อสินะ เขาไม่เห็นหัวฉันด้วยซ้ำ)", "Nia", "bad1", 5f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ที่จริงแล้วมันไม่ใช่นะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อ้าวเหรอ? งั้นช่างมันเถอะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ครูขอตัวก่อนนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
                CreateLine("รอดไปที", "Nia", "idle", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("ครูคะ! หนูว่ามีคนโกหกครูแล้วละคะ!", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อ้าวเหรอ? งั้นช่างมันเถอะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ครูขอตัวก่อนนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
                CreateLine("รอดไปที", "Nia", "idle", 0f, 1, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณมากค่ะที่เก็บเอาไว้", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("งั้นครูขอตัวด่อนละ พอดงานเยอะน่ะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("เฮ้อ", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        // Ensure Hong profile exists
        bool hasHong = false;
        foreach (var p in npc.npcProfiles)
        {
            if (p.npcName == "Hong") { hasHong = true; break; }
        }
        if (!hasHong)
        {
            Sprite hongSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Character/Dialog/Hong.png");
            if (hongSprite != null)
            {
                npc.npcProfiles.Add(new NPCProfile
                {
                    npcName = "Hong",
                    portraits = new List<NPCPortrait>
                    {
                        new NPCPortrait { portraitName = "idle", sprite = hongSprite }
                    }
                });
            }
        }

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว เนีย เดินหน้าซีดออกมาเชียว", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("การเจออะไรมาเหนื่อยขนาดนั้น?", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("(ทำงานมาก? เจอเรื่องแย่ๆ? แต่ไม่ได้อยากโดนพวกเธอปลอบเลย)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("เอาเถอะ เธอดูแลห้องสมุดด้วยหนิ เก่งแล้วละ", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(ชมตามมารยาทล่ะสิ... สมเพชฉันอยู่ใช่ไหม? ปลอบใจเด็กห่วยๆ คนนึงอยู่สินะ)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("หางานหาการทำก็ดีแล้ว ดีกว่านั่งซึมเป็นเป็ดหลงฝูงอยู่ในห้องเรียน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เชีย... พูดแรงไปแล้วนะ", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("จริงนี่ วันๆ เอาแต่หมกตัวด้วย", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("นั่นสินะ ก็จริง!!", "Nia", "bad1", 10f, 1, SpriteAction.Shake),
            CreateLine("(ใช่... เชียพูดถูกแล้ว ฉันมันไร้ค่า ทำอะไรก็ล้มเหลว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("(ขยะ... ไร้ประโยชน์... อยากหายไปซะตอนนี้เลย)", "Nia", "bad1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("(นิ่งเงียบ)", "Nia", "bad1", 10f, 1, SpriteAction.None, null),
            CreateChoice("พวกเธอจะไปเข้าใจอะไรล่ะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เหรอ? งั้นก็ทำให้ดูสิว่าเข้าใจอะไรบ้าง!", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอโทษนะ...", "Nia", "bad1", 5f, 1, SpriteAction.None, null)
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(ออกมาดีกว่า)", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("หนีซะละ", "Shia", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 6 สำเร็จ");
    }

    private static void ImportMomon()
    {
        string path = "Assets/Object/Characters/NPC_Momon.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 6);
        day.dayTitle = "DAY 6";
        day.dayNumber = 6;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("...", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("จัดชั้นหนังสือได้ไวเหมือนเดิมเลยนะเนีย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูโมม่อน", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("ตรงมุมการ์ตูนดีขึ้นเยอะ ปกติชั้นนี้ยุ่งเหยิงตลอดถ้าเธอไม่อยู่", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไม่หรอกค่ะ แค่เรื่องงั้น ๆ", "Nia", "bad1", 0f, 1, SpriteAction.None),
            CreateLine("ทำไมล่ะ?", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เปล่าค่ะ... แค่คิดว่า ใครๆ ก็ทำได้", "Nia", "idle", 0f, 1, SpriteAction.None),
            CreateLine("...", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คนอื่นทำได้ แต่เธอทำมัน ใส่ใจตัวเองกว่านี้หน่อยสิห้องสมุดนี้ ถ้าไม่มีเธอคอยดูแลฉันก็ต้องปล่อยมันทิ้งไว้ให้รกต่อไปละนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครู... หนูจะพยายามดูแลให้ดีขึ้นอีก", "Nia", "good1", -15f, 1, SpriteAction.None, null),
            CreateChoice("ครูพูดปลอบใจหนูหรือเปล่าคะ?", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("หน้าครูดูเหมือนคนชอบพูดไปแบบงั้น ๆ ขนาดนั้นเลยเหรอ?", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(ก้มหน้าไม่พูดอะไร)", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เอาเถอะ ขอบใจมากนะ", "Momon", "idle", -5f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ขอตัวก่อนนะค่ะ", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Momon] อัปเดต Day 6 สำเร็จ");
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
