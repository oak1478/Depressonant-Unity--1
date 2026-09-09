using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day17StoryImporter
{
    static Day17StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay17;
    }

    [MenuItem("Tools/Import Day 17 Story (Mom - Normal, Bad, Good)")]
    public static void ImportAllDay17()
    {
        Debug.Log("⏳ [Day17StoryImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 17 (Ending Normal, Bad, Good) ให้กับ NPC Mom...");
        ImportMomEndings();
        EnsureActiveDay17();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day17StoryImporter] บันทึกข้อมูลเนื้อเรื่อง Day 17 (3 ตอนจบ) เข้า NPC Mom เสร็จสมบูรณ์เรียบร้อย!");
    }

    private static void ImportMomEndings()
    {
        string path = "Assets/Object/Characters/NPC_Mom.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        // นำเข้าโปรไฟล์ Dad และ Rein เข้ามาใน Mom หากยังไม่มี เพื่อให้แสดงภาพใบหน้าถูกต้อง
        CopyProfileIfMissing(npc, "Dad", "Assets/Object/Characters/NPC_Dad.prefab");
        CopyProfileIfMissing(npc, "Rein", "Assets/Object/Characters/NPC_Rein.prefab");
        CopyProfileIfMissing(npc, "Rain", "Assets/Object/Characters/NPC_Rein.prefab");
        CopyProfileIfMissing(npc, "Nia", "Assets/Object/Characters/NPC_Dad.prefab");

        // Ensure Mom profile exists
        bool hasMom = false;
        foreach (var p in npc.npcProfiles)
        {
            if (p != null && p.npcName.Equals("Mom", System.StringComparison.OrdinalIgnoreCase)) { hasMom = true; break; }
        }
        if (!hasMom)
        {
            Sprite momSprite = null;
            foreach (var p in npc.npcProfiles)
            {
                if (p != null && p.npcName.Equals("แม่", System.StringComparison.OrdinalIgnoreCase) && p.portraits != null && p.portraits.Count > 0)
                {
                    momSprite = p.portraits[0].sprite;
                    break;
                }
            }
            if (momSprite != null)
            {
                npc.npcProfiles.Add(new NPCProfile
                {
                    npcName = "Mom",
                    portraits = new List<NPCPortrait>
                    {
                        new NPCPortrait { portraitName = "idle", sprite = momSprite }
                    }
                });
            }
        }

        // ลบข้อมูล Day 17 เดิมออกเพื่อลงชุดใหม่ทั้ง 3 ตอนจบ
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();
        npc.dialoguesByDay.RemoveAll(d => d.dayNumber == 17);

        // -------------------------------------------------------------
                // -------------------------------------------------------------
        // 1. NORMAL ENDING (Ending_Normal)
        // -------------------------------------------------------------
        DailyDialogue normalDay = new DailyDialogue
        {
            dayNumber = 17,
            dayTitle = "Ending_Normal",
            introductionStory = new List<DialogueLine>
            {
                CreateLine("เนีย มานี่สิ พ่อกับแม่มีเรื่องอยากจะคุยด้วยหน่อย", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("หลายวันที่ผ่านมา แม่เห็นว่าแกดูโอเคขึ้นนะ เกิดอะไรขึ้นหรอ", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อกับแม่คุยกันแล้วนะเนีย มีเรื่องโรคซึมเศร้านั้นด้วย เราก็ไม่อยากกดดันแกมาก", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("พี่เนีย เกิดอะไรขึ้นหรอ?", "Rain", "Idle", 0f, 7, SpriteAction.None),
                CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("หนูเองก็ต้องขอโทษที่เอาแต่ขังตัวเองค่ะ... ต่อจากนี้มีอะไรหนูจะพูดตรงๆ นะคะ", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("งั้นวันนี้เราไปหาอะไรอร่อยๆ กินกันทั้งบ้านเลยดีไหมคะ!", "Rain", "Idle", 0f, 7, SpriteAction.Jump),
                CreateLine("ช่วยดูบรรยากาศหน่อยสิ", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ไม่เห็นเป็นไรเลย", "Rain", "Idle", 0f, 7, SpriteAction.None),
                CreateLine("นั่นสินานๆทีไปกินด้วยกันบ้างก็ไม่เลว", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("เนอะ เนีย?", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("ไปกินด้วยกันเถอะค่ะ", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("เย้ ต้องงี้สิพี่เนีย", "Rain", "Idle", 0f, 7, SpriteAction.Jump)
            },
            storyChoices = new List<DialogueChoice>(),
            conclusionStory = new List<DialogueLine>()
        };
        npc.dialoguesByDay.Add(normalDay);

        // -------------------------------------------------------------
                // -------------------------------------------------------------
        // 2. BAD ENDING (Ending_Bad)
        // -------------------------------------------------------------
        DailyDialogue badDay = new DailyDialogue
        {
            dayNumber = 17,
            dayTitle = "Ending_Bad",
            introductionStory = new List<DialogueLine>
            {
                CreateLine("เนีย มานี่สิ พ่อกับแม่มีเรื่องต้องคุยให้ชัดเจน", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("เรื่องที่โรงเรียนติดต่อมา... เรื่องที่แกบอกว่าเป็นซึมเศร้าน่ะ มันคืออะไรกันแน่?", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("วันๆ เอาแต่เก็บตัว ทำตัวมีปัญหา แกคิดว่าคนอื่นเขาไม่เหนื่อยกันหรือไง?", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อทำงานหาเงินงกๆ แทบตาย แกมีหน้าที่แค่เรียนแท้ๆ จะมาเครียดอะไรนักหนา", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("พ่อคะ... แม่คะ... อย่าดุพี่เนียเลย พี่เนียไม่สบายอยู่นะคะ", "Rain", "Idle", 0f, 7, SpriteAction.None),
                CreateLine("เรน เงียบไปเลย! อย่าให้ท้ายพี่เขา เดี๋ยวก็เคยตัวจนไม่ทำอะไรพอดี", "Mom", "idle", 0f, 4, SpriteAction.Shake),
                CreateLine("...", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("หนูไม่ได้แกล้งทำนะคะ... ข้างในหนูมันทรมานจริงๆ...", "Nia", "bad1", 0f, 1, SpriteAction.Shake),
                CreateLine("ถ้าเหนื่อยนักก็หัดมองคนที่เขาลำบากกว่าบ้างสิ! เลิกทำตัวอ่อนแอเรียกร้องความสนใจสักที!", "Dad", "idle", 0f, 5, SpriteAction.Shake),
                CreateLine("...", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("นั่นสิคะ... หนูคงเป็นตัวปัญหาจริงๆ ขอโทษที่เกิดมาเป็นภาระนะคะ", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("เนีย! พูดจาแบบนี้หมายความว่าไง ประชดพ่อแม่เหรอ?!", "Mom", "idle", 0f, 4, SpriteAction.Shake),
                CreateLine("พี่เนีย... อย่าร้องไห้นะพี่...", "Rain", "Idle", 0f, 7, SpriteAction.None),
                CreateLine("ขอตัวนะคะ...", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("จะเดินหนีไปไหน! กลับมาคุยให้รู้เรื่องเดี๋ยวนี้นะเนีย!", "Dad", "idle", 0f, 5, SpriteAction.Shake)
            },
            storyChoices = new List<DialogueChoice>(),
            conclusionStory = new List<DialogueLine>()
        };
        npc.dialoguesByDay.Add(badDay);

        // -------------------------------------------------------------
        // 3. GOOD ENDING (Ending_Good)
        // -------------------------------------------------------------
        DailyDialogue goodDay = new DailyDialogue
        {
            dayNumber = 17,
            dayTitle = "Ending_Good",
            introductionStory = new List<DialogueLine>
            {
                CreateLine("เนีย มานั่งตรงนี้สิลูก วันนี้แม่เขาทำของโปรดไว้ให้เต็มโต๊ะเลย", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("เนีย... หลายวันที่ผ่านมา แม่เห็นความพยายามของแกนะ แล้วแม่ก็... ไปหาข้อมูลเรื่องซึมเศร้ามาด้วย", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("แม่ขอโทษนะลูก ที่ผ่านมาแม่เอาแต่กดดัน เอาแต่เปรียบเทียบ จนลืมถามว่าเนียรู้สึกยังไง...", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อเองก็ขอโทษนะเนีย พ่อเอาความเครียดข้างนอกมาลงกับบ้าน ละเลยความรู้สึกของลูกไปมากจริงๆ", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("ขอบใจนะที่สู้มาจนถึงวันนี้ พ่อภูมิใจในตัวเนียมากนะลูก", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("พี่เนีย! ดูนี่สิ หนูวาดรูปครอบครัวเรามาให้ด้วยล่ะ!", "Rain", "Idle", 0f, 7, SpriteAction.Jump),
                CreateLine("หนูอยากให้พี่เนียยิ้มได้แบบในรูปทุกวันเลยนะ!", "Rain", "Idle", 0f, 7, SpriteAction.Jump),
                CreateLine("...", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("ขอบคุณนะคะพ่อ... แม่... เรน...", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("หนูคิดว่าไม่มีใครต้องการหนูแล้วซะอีก... ขอบคุณที่ยอมรับฟังหนูนะคะ", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("ลูกของแม่ทั้งคน ทำไมจะไม่ต้องการล่ะ... จากนี้ไปมีอะไรเราคุยกันนะ ไม่ต้องแบกคนเดียวแล้ว", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("ใช่แล้ว! พวกเราอยู่ข้างพี่เนียเสมอนะ!", "Rain", "Idle", 0f, 7, SpriteAction.Jump),
                CreateLine("เอาล่ะ เลิกทำหน้าเศร้ากันได้แล้ว มาลงมือกินข้าวกันดีกว่า เดี๋ยวจะเย็นหมดนะ", "Dad", "idle", 0f, 5, SpriteAction.None),
                CreateLine("ค่ะ! มากินด้วยกันนะคะ", "Nia", "good1", 0f, 1, SpriteAction.Jump),
                CreateLine("เย้! หนูขอน่องไก่ชิ้นใหญ่นะคะ!", "Rain", "Idle", 0f, 7, SpriteAction.Jump)
            },
            storyChoices = new List<DialogueChoice>(),
            conclusionStory = new List<DialogueLine>()
        };
        npc.dialoguesByDay.Add(goodDay);

        

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [NPC_Mom] อัปเดต Day 17 (Ending Normal, Bad, Good) สำเร็จ");
    }

    private static void EnsureActiveDay17()
    {
        string[] prefabs = new string[]
        {
            "Assets/Object/Characters/NPC_Mom.prefab",
            "Assets/Object/Characters/NPC_Dad.prefab",
            "Assets/Object/Characters/NPC_Rein.prefab"
        };

        foreach (var p in prefabs)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            if (go == null) continue;

            NPCAppearanceController app = go.GetComponent<NPCAppearanceController>();
            if (app != null)
            {
                if (app.activeDays == null) app.activeDays = new List<int>();
                if (!app.activeDays.Contains(17))
                {
                    app.activeDays.Add(17);
                    EditorUtility.SetDirty(app);
                    PrefabUtility.SavePrefabAsset(go);
                    Debug.Log($"✅ [{go.name}] เพิ่ม Day 17 ใน activeDays สำเร็จ");
                }
            }
        }
    }

    private static void CopyProfileIfMissing(NPCInteraction targetNpc, string profileName, string sourcePrefabPath)
    {
        if (targetNpc.npcProfiles == null) targetNpc.npcProfiles = new List<NPCProfile>();

        foreach (var p in targetNpc.npcProfiles)
        {
            if (p != null && p.npcName.Equals(profileName, System.StringComparison.OrdinalIgnoreCase))
                return; // มีอยู่แล้ว
        }

        GameObject srcGo = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        if (srcGo == null) return;
        NPCInteraction srcNpc = srcGo.GetComponent<NPCInteraction>();
        if (srcNpc == null || srcNpc.npcProfiles == null) return;

        foreach (var p in srcNpc.npcProfiles)
        {
            if (p != null && (p.npcName.Equals(profileName, System.StringComparison.OrdinalIgnoreCase) ||
                             (profileName.Equals("Rain", System.StringComparison.OrdinalIgnoreCase) && p.npcName.Equals("Rein", System.StringComparison.OrdinalIgnoreCase))))
            {
                NPCProfile copy = new NPCProfile
                {
                    npcName = profileName,
                    portraits = new List<NPCPortrait>()
                };
                if (p.portraits != null)
                {
                    foreach (var port in p.portraits)
                    {
                        if (port != null)
                        {
                            copy.portraits.Add(new NPCPortrait
                            {
                                portraitName = port.portraitName,
                                sprite = port.sprite
                            });
                        }
                    }
                }
                targetNpc.npcProfiles.Add(copy);
                Debug.Log($"✅ [NPC_Mom] คัดลอกโปรไฟล์ '{profileName}' จาก {sourcePrefabPath} สำเร็จ");
                return;
            }
        }
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
