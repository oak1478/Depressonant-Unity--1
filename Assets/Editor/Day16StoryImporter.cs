using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day16StoryImporter
{
    static Day16StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay16;
    }

    [MenuItem("Tools/Import Day 16 Story (Momon, Vipar, Rin, Park, Shia)")]
    public static void ImportAllDay16()
    {
        Debug.Log("⏳ [Day16StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 16...");
        ImportMomon();
        ImportVipar();
        ImportRin();
        ImportPark();
        ImportShia();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day16StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 16 (Momon, Vipar, Rin, Park, Shia) เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportMomon()
    {
        string path = "Assets/Object/Characters/NPC_Momon.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("ไงเนีย สัปดาห์นี้เธอทำหน้าที่ได้ยอดเยี่ยมมากนะเนี่ย... ทำให้ห้องสมุดกลับมามีชีวิตชีวาขึ้นเยอะเลย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ถึงถึงแม้จะเป็นซึมเศร้า แต่เธอสู้กับความรู้สึกตัวเองจนก้าวผ่านมันมาได้ ครูภูมิใจในตัวเธอนะ", "Momon", "idle", -15f, 4, SpriteAction.None),
            CreateLine("... ครูค่ะ หนูมีอะไรจะบอก", "Nia", "idle", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณมากๆ ค่ะครูโมม่อน! ถ้าครูไม่พูดว่าเชื่อในตัวหนู หนูคงจะไม่มาโรงเรียนแล้ว ขอบคุณที่ช่วยนะคะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เธอทำได้ด้วยตัวเธอเองต่างหากเนีย... จำความรู้สึกนี้ไว้นะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณสำหรับทุกอย่างนะคะครู... หนูดีใจมากที่ได้ช่วยงานห้องสมุด", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ยินดีเสมอเนีย ห้องสมุดต้อนรับเธอเสมอทุกเมื่อนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนู... หนูแค่ทำตามที่ครูสั่งน่ะค่ะ ไม่ได้เก่งอะไรขนาดนั้น", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ยังถ่อมอยู่อีก มั่นใจในตัวเองหน่อยสิเนีย ยอมรับมันเถอะนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นกลับบ้านดีๆ ล่ะเนีย เสาร์อาทิตย์นี้ก็พักผ่อนให้เต็มที่นะ", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ค่ะ", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Momon] อัปเดต Day 16 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("อ้าว เนีย!ครูเพิ่งตรวจรายงานกลุ่มที่เธอช่วยจัดระบบส่งมา... ทำได้เป็นระเบียบเรียบร้อยมากเลยนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("สัปดาห์นี้เธอเปลี่ยนไปเยอะเลยนะดูมีความมั่นใจและตั้งใจขึ้นมาก... ต่อจากนี้ครูขอฝากเธอช่วยดูแลเพื่อนๆ ในวิชาเคมีด้วยล่ะ ก่อนที่พวกมันจะซ้ำชั้นกัน", "Vipar", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("รับทราบค่ะครูวิภา! หนูจะตั้งใจเรียนและช่วยเพื่อนๆ เต็มที่ ขอบคุณครูมากๆ นะคะ!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ฮึฮึ ดีมากเนีย! ไว้เจอกันสัปดาห์หน้านะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณค่ะครูวิภา", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้าๆ เดินทางกลับบ้านปลอดภัยนะเนีย", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูไม่แน่ใจว่าจะช่วยคนอื่นได้ไหมค่ะครู...", "Nia", "bad1", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เธอทำได้อยู่แล้วเนีย อย่าเพิ่งประเมินตัวเองต่ำไปสิ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ครูขอตัวก่อนนะเนีย พักผ่อนวันเสาร์อาทิตย์ให้สบายล่ะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Vipar] อัปเดต Day 16 สำเร็จ");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย... ศุกร์นี้ไม่เหมือนศุกร์ที่แล้วเลยเนอะ!", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ฉันดีใจมากๆ เลยนะที่เห็นเธอยิ้มได้กว้างขนาดนี้ ได้หัวเราะกับทุกคน", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เธอรู้ไหมว่าเธอเก่งมากๆ เลยนะที่ก้าวผ่านความรู้สึกแย่ๆ พวกนั้นมาได้ ฉันภูมิใจในตัวเธอที่สุดเลย!", "Rin", "idle", -10f, 4, SpriteAction.Jump)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ริน... ขอบใจมากๆ นะ!", "Nia", "good1", 0f, 1, SpriteAction.None, null),
            CreateChoice("ขอบใจนะริน เพราะเธอนั่นแหละที่คอยช่วยฉันไว้ตลอด", "Nia", "good1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เราช่วยกันและกันต่างหากล่ะเนีย!", "Rin", "idle", -5f, 4, SpriteAction.Jump)
            }),
            CreateChoice("อย่าเว่อร์ไปหน่อยเลยริน... มันก็แค่สัปดาห์ธรรมดาๆ", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ไม่เว่อร์สักหน่อย! เธอเก่งขึ้นจริงๆ นะเนีย", "Rin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("เอาเถอะ วันนี้พอแค่นี้ก่อน กลับบ้านไปพักผ่อนเยอะๆ นะเนีย ไว้คุยกันในแชตนะ!", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 16 สำเร็จ");
    }

    private static void ImportPark()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("ไง! สรุปว่ารอดตายจากสัปดาห์มหาโหดมาได้แบบครบสามสิบสองนะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เออ... จะว่าไป พอเธอเลิกทำตัวเป็นหินเดินได้ โรงเรียนก็ดูน่าอยู่ขึ้นเยอะเหมือนกันนะเนี่ย", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คราวหน้าคราวหลังมีเรื่องอะไรก็พูดออกมาตรงๆ แบบนี้แหละง่ายดี", "Park", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ถ้าฉันดีขึ้น นายก็ไม่มีคนให้แขวะพอดีสิ", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ฮ่าๆๆ! เถียงคำไม่ตกฟากแบบนี้สิค่อยสนุกหน่อย!", "Park", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ขอบใจนะป้าก... ที่ทักเตือนสติฉันวันก่อน", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เออๆ ไงก็ได้แหละ กันในฐานะเพื่อนร่วมห้อง", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("นายก็เลิกปากเสียให้ได้ก่อนเถอะป้าก!", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห้อ... อุตส่าห์คุยดีๆ ด้วยนะเนี่ย", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("จะไปไหนก็ไปเถอะ", "Park", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 16 สำเร็จ");
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
                    portraits = new List<PortraitEntry>
                    {
                        new PortraitEntry { portraitName = "idle", sprite = hongSprite }
                    }
                });
            }
        }

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("เนีย! จะไปไหน? วันนี้พวกเราว่าจะไปกินไอศกรีมหน้าโรงเรียนหน่อย ไปด้วยกันไหม?", "Hong", "idle", 0f, 6, SpriteAction.None),
            CreateLine("อะแฮ่ม", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อืม... เรื่องเมื่อวันศุกร์ก่อนนู้นน่ะ... ฉันอาจจะพูดแรงไปหน่อย เรื่องที่ว่าเธอเป็นไอนั่นนะ ขอโทษละกัน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เธอก็ไม่ได้แย่อย่างที่ฉันคิด", "Shia", "idle", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ช่างมันเถอะเชีย ฉันเองก็ขอโทษเหมือนกันนะ... ขอบใจนะหงส์ ไว้คราวหน้าฉันไปกินไอศกรีมด้วยแน่นอน วันนี้ขอตัวกลับก่อนนะ!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("โอเคเลย สัปดาห์หน้าไปกินด้วยกันนะ!", "Hong", "idle", 0f, 6, SpriteAction.None),
                CreateLine("อืม... กลับบ้านดีๆ ละกัน", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ไม่เป็นไรหรอกเชีย ขอบใจนะ งั้นหงส์ ไว้โอกาสหน้านะวันนี้ขอกลับก่อน", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("แล้วเจอกันวันจันทร์นะ!", "Hong", "idle", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("เพิ่งมารู้ตัวเหรอว่าพูดแรง? ช่างมันเถอะ ฉันไม่อยากพูดถึงอีก", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เอ้า! คนเขาอุตส่าห์ขอโทษดีๆ! ช่างเถอะหงส์ ไปกันได้แล้ว!", "Shia", "idle", 0f, 4, SpriteAction.Shake),
                CreateLine("แล้วเจอกันวันจันทร์นะ!", "Hong", "idle", 0f, 6, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("แล้วเจอกัน", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 16 สำเร็จ");
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
