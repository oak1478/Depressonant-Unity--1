#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class StoryDataUpdater
{
    [MenuItem("Tools/📖 อัปเดตเนื้อเรื่อง Day 1 - Day 4 ทั้งหมด (Update Story Day 1-4)")]
    public static void UpdateAllStories()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Update Day 1-4 Stories");

        int updatedCount = 0;

        // 1. อัปเดต NPC_Mom.prefab (Day 1 & Day 3)
        string momPrefabPath = "Assets/Object/Characters/NPC_Mom.prefab";
        GameObject momPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(momPrefabPath);
        if (momPrefab != null)
        {
            NPCInteraction momInteraction = momPrefab.GetComponent<NPCInteraction>();
            if (momInteraction != null)
            {
                UpdateMomDialogues(momInteraction);
                EditorUtility.SetDirty(momPrefab);
                PrefabUtility.SavePrefabAsset(momPrefab);
                updatedCount++;
                Debug.Log("✅ [StoryUpdater] อัปเดตเนื้อเรื่อง NPC_Mom.prefab สำเร็จ (Day 1 และ Day 3)");
            }
        }

        // 2. อัปเดต NPC_Rein.prefab (Rain - Day 2 & Day 3)
        string reinPrefabPath = "Assets/Object/Characters/NPC_Rein.prefab";
        GameObject reinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(reinPrefabPath);
        if (reinPrefab != null)
        {
            NPCInteraction reinInteraction = reinPrefab.GetComponent<NPCInteraction>();
            if (reinInteraction != null)
            {
                UpdateReinDialogues(reinInteraction);
                EditorUtility.SetDirty(reinPrefab);
                PrefabUtility.SavePrefabAsset(reinPrefab);
                updatedCount++;
                Debug.Log("✅ [StoryUpdater] อัปเดตเนื้อเรื่อง NPC_Rein.prefab สำเร็จ (Day 2 และ Day 3)");
            }
        }

        // 3. อัปเดต NPC_Dad.prefab (Day 3)
        string dadPrefabPath = "Assets/Object/Characters/NPC_Dad.prefab";
        GameObject dadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(dadPrefabPath);
        if (dadPrefab != null)
        {
            NPCInteraction dadInteraction = dadPrefab.GetComponent<NPCInteraction>();
            if (dadInteraction != null)
            {
                UpdateDadDialogues(dadInteraction);
                EditorUtility.SetDirty(dadPrefab);
                PrefabUtility.SavePrefabAsset(dadPrefab);
                updatedCount++;
                Debug.Log("✅ [StoryUpdater] อัปเดตเนื้อเรื่อง NPC_Dad.prefab สำเร็จ (Day 3)");
            }
        }

        // 4. อัปเดต NPC_Jin.prefab (Day 2)
        string jinPrefabPath = "Assets/Object/Characters/NPC_Jin.prefab";
        GameObject jinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(jinPrefabPath);
        if (jinPrefab != null)
        {
            NPCInteraction jinInteraction = jinPrefab.GetComponent<NPCInteraction>();
            if (jinInteraction != null)
            {
                UpdateJinDialogues(jinInteraction);
                EditorUtility.SetDirty(jinPrefab);
                PrefabUtility.SavePrefabAsset(jinPrefab);
                updatedCount++;
                Debug.Log("✅ [StoryUpdater] อัปเดตเนื้อเรื่อง NPC_Jin.prefab สำเร็จ (Day 2)");
            }
        }

        // 5. อัปเดต NPC_Den.prefab (Day 2 & Day 4)
        string denPrefabPath = "Assets/Object/Characters/NPC_Den.prefab";
        GameObject denPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(denPrefabPath);
        if (denPrefab != null)
        {
            NPCInteraction denInteraction = denPrefab.GetComponent<NPCInteraction>();
            if (denInteraction != null)
            {
                UpdateDenDialogues(denInteraction);
                EditorUtility.SetDirty(denPrefab);
                PrefabUtility.SavePrefabAsset(denPrefab);
                updatedCount++;
                Debug.Log("✅ [StoryUpdater] อัปเดตเนื้อเรื่อง NPC_Den.prefab สำเร็จ (Day 2 และ Day 4)");
            }
        }

        // 6. ตรวจสอบและอัปเดต NPC ในฉากปัจจุบันที่เปิดอยู่ (Active Scene)
        var allSceneNpcs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool sceneModified = false;
        foreach (var npc in allSceneNpcs)
        {
            string id = npc.GetNPCIdentifier();
            if (id == "Mom" || npc.name.Contains("Mom"))
            {
                UpdateMomDialogues(npc);
                EditorUtility.SetDirty(npc);
                sceneModified = true;
                Debug.Log($"✅ [StoryUpdater] อัปเดต Mom ในฉาก {npc.gameObject.scene.name} เรียบร้อย");
            }
            else if (id == "Rein" || id == "Rain" || npc.name.Contains("Rein"))
            {
                UpdateReinDialogues(npc);
                EditorUtility.SetDirty(npc);
                sceneModified = true;
                Debug.Log($"✅ [StoryUpdater] อัปเดต Rein ในฉาก {npc.gameObject.scene.name} เรียบร้อย");
            }
            else if (id == "Dad" || npc.name.Contains("Dad"))
            {
                UpdateDadDialogues(npc);
                EditorUtility.SetDirty(npc);
                sceneModified = true;
                Debug.Log($"✅ [StoryUpdater] อัปเดต Dad ในฉาก {npc.gameObject.scene.name} เรียบร้อย");
            }
            else if (id == "Jin" || npc.name.Contains("Jin"))
            {
                UpdateJinDialogues(npc);
                EditorUtility.SetDirty(npc);
                sceneModified = true;
                Debug.Log($"✅ [StoryUpdater] อัปเดต Jin ในฉาก {npc.gameObject.scene.name} เรียบร้อย");
            }
            else if (id == "Den" || npc.name.Contains("Den"))
            {
                UpdateDenDialogues(npc);
                EditorUtility.SetDirty(npc);
                sceneModified = true;
                Debug.Log($"✅ [StoryUpdater] อัปเดต Den ในฉาก {npc.gameObject.scene.name} เรียบร้อย");
            }
        }

        if (sceneModified)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("อัปเดตเนื้อเรื่องสำเร็จ!", 
            $"ระบบได้ทำการอัปเดตบทสนทนา Day 1-4 เรียบร้อยแล้วทั้งใน Prefab และฉากปัจจุบันครับ! (รวม {updatedCount} ตัวละคร)", "ตกลง");
    }

    // -------------------------------------------------------------
    // เนื้อเรื่อง MOM (Day 1 & Day 3)
    // -------------------------------------------------------------
    private static void UpdateMomDialogues(NPCInteraction npc)
    {
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();

        // --- Day 1 (Tutorial) ---
        DailyDialogue day1 = npc.dialoguesByDay.Find(d => d.dayNumber == 1);
        if (day1 == null && npc.dialoguesByDay.Count > 0 && npc.dialoguesByDay[0].dayNumber == 2)
        {
            day1 = npc.dialoguesByDay[0];
        }

        if (day1 == null)
        {
            day1 = new DailyDialogue();
            npc.dialoguesByDay.Insert(0, day1);
        }

        day1.dayNumber = 1;
        day1.dayTitle = "DAY 1";

        day1.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "ในที่สุดก็ตื่นนะเนีย", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "เมื่อคืนเล่นเกมอีกแล้วเรอะ เฮ้อ~~ แล้วแต่เถอ", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day1.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice { choiceButtonText = "หนูไม่ได้เล่นเกมนะค่ะ หนูอ่านหนังสือ", stressChange = -15f, speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, nextDialogueLines = new List<DialogueLine>() },
            new DialogueChoice { choiceButtonText = "(พยายามนิ่งเงียบอย่างใจเย็น)", stressChange = 0f, speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, nextDialogueLines = new List<DialogueLine>() },
            new DialogueChoice { choiceButtonText = "ค่ะ", stressChange = 10f, speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, nextDialogueLines = new List<DialogueLine>() }
        };

        day1.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ช่างมันเถอะ", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.SlideOut }
        };

        // --- Day 3 (Saturday - Mom & Dad) ---
        DailyDialogue day3 = npc.dialoguesByDay.Find(d => d.dayNumber == 3);
        if (day3 == null)
        {
            day3 = new DailyDialogue();
            npc.dialoguesByDay.Add(day3);
        }

        day3.dayNumber = 3;
        day3.dayTitle = "DAY 3";

        day3.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "เนีย หลังกินข้าวเสร็จแล้วไปจัดการกองหนังสือในห้องแกด้วยนะ", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "แม่บอกกี่ครั้งแล้วว่าอย่าปล่อยให้มันรก", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "แล้วเรื่องเกรดเทอมนี้ถ้ายังเป็นแบบเดิม แม่คงต้องลดค่าขนมแกแล้วนะ", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เออ แล้วนี่ใครจะไปซื้อของให้ฉันละเนี่ย ?", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พ่อมีธุระด่วนนะ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ฉันไปซื้อเองก็ได้ แต่ทำไมคุณไม่ไปซื้อเองบ้างล่ะ เอาแต่ให้ฉันไปตลอดเลย", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.Shake },
            new DialogueLine { text = "เออ เออ เอาเป็นว่าฉันไม่ว่างนะ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "อย่ามาพูดส่งเดชนะ!", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.Shake },
            new DialogueLine { text = "ก็บอกว่ามีธุระด่วนไง เฮ้อ~~", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "แล้วเธอละเนีย! ยืนบื้ออยู่ทำไม ?", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "เอาแต่ทำตัวไร้ประโยชน์อยู่ในห้อง ไม่ลองออกไปข้างนอกรับแสงบ้างล่ะ", speakerName = "Mom", portraitName = "1", stressChange = 10f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day3.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "ขอโทษค่ะ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "เห้อ~~ ให้ตายเถอะ", speakerName = "Mom", portraitName = "1", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "...",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "พูดด้วยก็เหมือนพูดกับกำแพงจริง ๆ", speakerName = "Mom", portraitName = "1", stressChange = 10f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ก็ได้ ก็ได้ งั้นหนูจะไปซื้อเอง",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "อย่ามาพูดประชดนะเนีย !", speakerName = "Mom", portraitName = "1", stressChange = 10f, activeSlotIndex = 4, motionEffect = SpriteAction.Shake },
                    new DialogueLine { text = "พ่อเขาทำงานหาเงินได้บ้าง ไม่เหมือนแกที่ว่างได้ว่างดี", speakerName = "Mom", portraitName = "1", stressChange = 10f, activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ขอโทษค่ะ", speakerName = "Nia", portraitName = "bad1", stressChange = 0f, activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            }
        };

        day3.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "ฉันอารมณ์เสียละเดี๋ยวจะไปเองก็ได้", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "...", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
        };

        // --- Day 4 (Mom) ---
        DailyDialogue day4 = npc.dialoguesByDay.Find(d => d.dayNumber == 4);
        if (day4 == null)
        {
            day4 = new DailyDialogue();
            npc.dialoguesByDay.Add(day4);
        }

        day4.dayNumber = 4;
        day4.dayTitle = "DAY 4";

        day4.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "ถ้าว่างนักก็ไปดูน้องหน่อย วันนี้เรนออกไปเล่นข้างนอก เดี๋ยวฉันจะเตรียมอาหารไว้ให้", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ได้ค่ะ แล้วน้องอยู่ไหนคะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ข้างนอกกับเดนนั่นแหละ เห็นว่าจะไปเล่นกันนะ น่าดีใจจริง ๆ", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "นี่ อย่างน้อยก็เอาแบบน้องแกหน่อยสิ แทนที่จะนั้งซึมฟังเพลงกับเกมทั้งวันนะ", speakerName = "Mom", portraitName = "1", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day4.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "ก็แค่เอาไปฟังแก้เบื่อเอง",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ยังจะเถียงอีก ไปได้แล้ว ไป !", speakerName = "Mom", portraitName = "1", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.Shake }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ได้ค่ะ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>()
            },
            new DialogueChoice
            {
                choiceButtonText = "หนูขอโทษค่ะ งั้นขอออกไปหาน้องก่อน",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ปกติไม่ยอมไปนะเนี่ย", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ดีมาก", speakerName = "Mom", portraitName = "1", stressChange = -5f, activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "รีบไปรีบมาล่ะ ก่อนอาหารจะเย็นหมด", speakerName = "Mom", portraitName = "1", activeSlotIndex = 4, motionEffect = SpriteAction.None }
                }
            }
        };

        day4.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };
    }

    // -------------------------------------------------------------
    // เนื้อเรื่อง REIN / RAIN (Day 2 & Day 3)
    // -------------------------------------------------------------
    private static void UpdateReinDialogues(NPCInteraction npc)
    {
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();

        // --- Day 2 (Rain) ---
        DailyDialogue day2 = npc.dialoguesByDay.Find(d => d.dayNumber == 2);
        if (day2 == null)
        {
            day2 = new DailyDialogue();
            npc.dialoguesByDay.Insert(0, day2);
        }

        day2.dayNumber = 2;
        day2.dayTitle = "DAY 2";

        day2.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "สวัสดีเรน ทำอะไรอยู่หรอ ?", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่อย่าพึ่งมายุ่งสิ หนูมีเรื่องต้องคิดอยู่น้ะ!!", speakerName = "Rein", portraitName = "Idle", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.Shake },
            new DialogueLine { text = "คิดอะไรอยู่ละนั่น", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "วันนี้จะออกไปเล่นหรืออยู่วาดรูปดีนะสิ", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day2.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "เป็นเรื่องใหญ่ขนาดนั้นเลย ?",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "พี่~~ จุกจิกชะมัด", speakerName = "Rein", portraitName = "Idle", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "เอาเถอะ งั้นพี่ขอนั่งด้วยคนสิ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ได้สิ มาเลย ๆ", speakerName = "Rein", portraitName = "Idle", stressChange = -5f, activeSlotIndex = 4, motionEffect = SpriteAction.Jump }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "(พยายามเมินดีกว่า)",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>()
            }
        };

        day2.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "รูปภาพนั่นอะไรนะ สวยดีนี่น่า", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "!!!!!!!!", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 4, motionEffect = SpriteAction.Shake },
            new DialogueLine { text = "ก็แค่ระบายสีไปมั่วๆ แหละ ถ้าอยากได้ก็เอาไปเถอะ จะวางไว้งี้แหละ", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ได้หรอขอบคุณนะ", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ละก็ขอบคุณค่ะ ที่ชม", speakerName = "Rein", portraitName = "Idle", stressChange = -5f, activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        // --- Day 3 (Rain) ---
        DailyDialogue day3 = npc.dialoguesByDay.Find(d => d.dayNumber == 3);
        if (day3 == null)
        {
            day3 = new DailyDialogue();
            npc.dialoguesByDay.Add(day3);
        }

        day3.dayNumber = 3;
        day3.dayTitle = "DAY 3";

        day3.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "เรน", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ขอโทษนะพี่ หนูไม่ว่างนะ", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "งั้นโอเค", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };

        day3.storyChoices = new List<DialogueChoice>();
        day3.conclusionStory = new List<DialogueLine>();
    }

    // -------------------------------------------------------------
    // เนื้อเรื่อง DAD (Day 3)
    // -------------------------------------------------------------
    private static void UpdateDadDialogues(NPCInteraction npc)
    {
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();

        DailyDialogue day3 = npc.dialoguesByDay.Find(d => d.dayNumber == 3);
        if (day3 == null)
        {
            day3 = new DailyDialogue();
            npc.dialoguesByDay.Add(day3);
        }

        day3.dayNumber = 3;
        day3.dayTitle = "DAY 3";

        day3.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "มีอะไรหรอ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เปล่า แค่เดินมาดูเฉยๆ ว่ามาทำอะไรตรงนี้ ไหนว่าไม่ว่างไง", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เรื่องนั้นลืมไปเถอะ อย่าไปคิดมาก", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
        };

        day3.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "ธุระที่ว่าตอนแรก คืออะไรหรอคะ ?",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "โห้! มีหางเสียงกับพ่อบ้างแล้วสินะ แต่ว่าเรื่องของผู้ใหญ่ อย่าถามเซ้าซี้สิ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ตอบมา หนูนึกว่าพ่อรีบไปทำงานซะอีก",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "จะบ่นอะไรนักหนากันละ", speakerName = "Dad", portraitName = "idle", stressChange = 10f, activeSlotIndex = 6, motionEffect = SpriteAction.Shake }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "(ไม่อยากพูดอะไรแล้ว)",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>()
            }
        };

        day3.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "ฉันทำงานมาเหนื่อยๆ จะพักบ้างจะเป็นไรไป", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พ่อก็ว่างบ่อยนะ ถ้ามีเรื่องไม่สบายใจก็มาหาได้เลย แต่ไม่ใช่วันนี้", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เอาเป็นตั้งใจเรียนละเนีย อย่ายอมแพ้ให้โรคซึมเศร้านะ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ค่าาาา~~~~~", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "...", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เนีย", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "หือ ?", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พ่อรู้ว่าเราไม่ถูกกันนักแต่อย่าเอาเรื่องที่บ้านไปเที่ยวพูดให้ใครฟังล่ะ มันไม่ช่วยอะไรหรอก เข้าใจใช่ไหม?", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ตอบด้วย", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เข้าใจแล้วน่า", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };

        // --- Day 4 (Dad) ---
        DailyDialogue day4 = npc.dialoguesByDay.Find(d => d.dayNumber == 4);
        if (day4 == null)
        {
            day4 = new DailyDialogue();
            npc.dialoguesByDay.Add(day4);
        }

        day4.dayNumber = 4;
        day4.dayTitle = "DAY 4";

        day4.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เตรียมของไปโรงเรียนพรุ่งนี้หรือยัง?", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ละก็อย่าก่อเรื่องให้ครูเยอะล่ะ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ได้ๆ ว่าจะไปเตรียมตอนเย็น ๆ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "จะทิ้งช่วงไปทำไม ตอนนี้เลย !!", speakerName = "Dad", portraitName = "idle", stressChange = 5f, activeSlotIndex = 6, motionEffect = SpriteAction.Shake }
        };

        day4.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "เข้าใจแล้ว จะไปทำเดี๋ยวนี้",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ดีแล้ว", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "เห้อ ไปดีกว่า",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ลืมมารยาทแล้วหรอ เฮ้ย !!", speakerName = "Dad", portraitName = "idle", stressChange = 10f, activeSlotIndex = 6, motionEffect = SpriteAction.Shake },
                    new DialogueLine { text = "ชิ้ !", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ขอหนูพักสักนิดไม่ได้รึไง ?",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "จะพักอะไรนักหนาละ", speakerName = "Dad", portraitName = "idle", stressChange = 5f, activeSlotIndex = 6, motionEffect = SpriteAction.Shake },
                    new DialogueLine { text = "หน้าที่ของลูกคือเรียนไม่ใช่เหรอ ไปจัดการซะ เรื่องสุขภาพนะอ้างกับงานไม่ได้หรอกนะ", speakerName = "Dad", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ได้ค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            }
        };

        day4.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ค่อยทำก็ได้แท้ ๆ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };
    }

    // -------------------------------------------------------------
    // เนื้อเรื่อง JIN (Day 2)
    // -------------------------------------------------------------
    private static void UpdateJinDialogues(NPCInteraction npc)
    {
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();

        DailyDialogue day2 = npc.dialoguesByDay.Find(d => d.dayNumber == 2);
        if (day2 == null)
        {
            day2 = new DailyDialogue();
            npc.dialoguesByDay.Insert(0, day2);
        }

        day2.dayNumber = 2;
        day2.dayTitle = "DAY 2";

        day2.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "อ้าว เจ้าหนูเนีย! ตื่นสายเชียวนะวันนี้", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "นานๆ ทีจะเห็นออกจากบ้านด้วย", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "แต่ก็สวัสดีค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ฮะฮาฮา จำหน้าหนูไม่ค่อยได้แล้วแฮะ สงสัยฉันจะแก่แล้วจริงๆ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "จะว่าไปน้องสาวเธอละ ?", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
        };

        day2.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "ไม่แน่ใจเหมือนกันค่ะ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "งั้นหรอ ฉันเห็นน้องเรนนั้นบ่อยกว่าหนูละนะ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ก็หนูนะ เอาแต่หมกตัวอยู่ในบ้าน ไม่ออกมาอาบแสงให้เห็นเลยนิ จนฉันลืมหน้าไปละ", speakerName = "Jin", portraitName = "idle", stressChange = 5f, activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ถ้าไม่มีอะไรแล้วหนูขอตัวก่อนนะค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "บ๊ายบายจ้ะ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ถ้าไม่มีอะไรแล้วหนูขอตัวก่อนนะค่ะ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "บ๊ายบายจ้ะ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ป้าลืมหนูได้ไง ! บ้านอยู่ใกล้กันแท้ ๆ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "เอาน่า สุภาพหน่อยเจ้าไอเด็กนี่ เดี๋นวก็ไปบอกแม่เธอหรอก", speakerName = "Jin", portraitName = "idle", stressChange = 10f, activeSlotIndex = 6, motionEffect = SpriteAction.Shake },
                    new DialogueLine { text = "ขอโทษค่ะ !!", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            }
        };

        day2.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "(รีบไปดีกว่า!)", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };

        // --- Day 4 (Jin & Ben) ---
        DailyDialogue day4 = npc.dialoguesByDay.Find(d => d.dayNumber == 4);
        if (day4 == null)
        {
            day4 = new DailyDialogue();
            npc.dialoguesByDay.Add(day4);
        }

        day4.dayNumber = 4;
        day4.dayTitle = "DAY 4";

        day4.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "จะรดน้ำต้นไม้หรอค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "แม่จินดูซิใครมา นั่นหนูเนียใช่ไหม เห็นว่าวันก่อนไปหาหมอมาจนรู้ว่าเป็นโรคซึมเศร้าสินะ", speakerName = "Ben", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ค้ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "นั่นจะไปไหนเหรอ เนีย ?", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day4.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "จะไปหาเรนนะค่ะ ที่บ้านของเดน",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "อ้าวเหรอ ระวังรถระด้วยล่ะ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "เป็นเด็กดีจังนะเรา ดูแลน้องดี ๆ แล้วก็เดินทางปลอดภัยนะ หนู", speakerName = "Ben", portraitName = "idle", stressChange = -10f, activeSlotIndex = 6, motionEffect = SpriteAction.Jump },
                    new DialogueLine { text = "ว่าแต่รู้เรื่องโรคได้ไง", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ช่างเรื่องนั้นเถอะ อย่าใส่ใจไปเลย", speakerName = "Ben", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "งั้นหนูขอตัวก่อนนะค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ไม่มีอะไรคะ แค่เดินเล่นแถวนี้แหละ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ดีแล้วๆ ออกมารับแดดบ้าง ผิวจะได้รับวิตามิน", speakerName = "Jin", portraitName = "idle", stressChange = -5f, activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ว่าแต่รู้เรื่องโรคได้ไง", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ช่างเรื่องนั้นเถอะ อย่าใส่ใจไปเลย", speakerName = "Ben", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "งั้นหนูขอตัวก่อนนะค่ะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "(เดินหนีออกมา)",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "อะไรของเด็กนั่น ?", speakerName = "Ben", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ช่างมันเถอะ", speakerName = "Jin", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None }
                }
            }
        };

        day4.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "(ไม่รู้จะพูดอะไรดี ทำไมถึงรู้อะไรแบบนั้นได้)", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
        };
    }

    // -------------------------------------------------------------
    // เนื้อเรื่อง DEN / SASHA / EGON (Day 2)
    // -------------------------------------------------------------
    private static void UpdateDenDialogues(NPCInteraction npc)
    {
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();

        DailyDialogue day2 = npc.dialoguesByDay.Find(d => d.dayNumber == 2);
        if (day2 == null)
        {
            day2 = new DailyDialogue();
            npc.dialoguesByDay.Insert(0, day2);
        }

        day2.dayNumber = 2;
        day2.dayTitle = "DAY 2";

        day2.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "พี่เนีย! มาเล่นด้วยกันไหม? เรากำลังขาดคนกันอยู่!", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "สวัสดี เดน แต่พี่ไม่เล่นดีกว่า", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เอาน่า หรือพี่ไม่รู้จักหรอ งั้นก็แค่เริ่มเกมส์ก่อน ที่เหลือเดี๋ยวก็รู้เอง", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ทักทายพี่เขาดี ๆ สิเดน!", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่เขาอุตส่าห์สวัสดีก่อนนะ สวัสดีค่ะพี่", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "สวัสดี ซาช่า", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่เนีย วันนี้มาแปลกนะคะ ปกติไม่ออกมาข้างนอกนิ แต่ก็มาเล่นเกมกันเถอะค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ไม่ละ ฉันแค่อยากออกมาเดินเล่นนะ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เรนล่ะครับ? ปกติออกมาเล่นกับเราตลอดเลยนะ วันนี้ทำอะไรอยู่", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None }
        };

        day2.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "เรนยุ่งอยู่น่ะ เห็นว่ามีการบ้าน",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "วันนี้ขยันผิดปกติแฮะ ถ้านางมาเล่นด้วยก็สนุกกว่านี้แต่เอาเถอะ เรามีพี่เนียอยู่", speakerName = "Den", portraitName = "idle", stressChange = -5f, activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
                    new DialogueLine { text = "(เด็กดีจัง)", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ปกติเรนก็ไม่ค่อยออกจากบ้านไม่ใช่หรอ ?",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "จะจำผิดได้ไงคะพี่เนีย ?", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ก็เรนชอบมาวาดรูปเล่นกับหนูประจำ พี่เนียนั่นแหละไหวไหมคะเนี่ย?", speakerName = "Sasha", portraitName = "idle", stressChange = 5f, activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "(โดนว่าอีกละ)", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "ไม่แน่ใจเหมือนกัน",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ฉันไม่ได้คุยกับน้องเท่าไหร่", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ไม่ค่อยได้คุบกับเรนหรอครับ ? ไม่ต้องห่วงนะครับ วันนี้พวกผมจะเป็นเพื่อนเล่นด้วยเอง", speakerName = "Den", portraitName = "idle", stressChange = -10f, activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
                    new DialogueLine { text = "(เด็กดีจัง)", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            }
        };

        day2.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "งั้นพี่กลับก่อนนะ โชคดีทุกคน", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "บ๊ายบายครับ", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "บ๊ายบายค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "บาย", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None }
        };

        // --- Day 4 (Den, Sasha, Egon, Rein) ---
        DailyDialogue day4 = npc.dialoguesByDay.Find(d => d.dayNumber == 4);
        if (day4 == null)
        {
            day4 = new DailyDialogue();
            npc.dialoguesByDay.Add(day4);
        }

        day4.dayNumber = 4;
        day4.dayTitle = "DAY 4";

        day4.introductionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "อ้าวพี่เนีย ! สวัสดีครับ วันนี้ก็ออกมาเล่นกันไหม วันนี้เราเล่นทายเรื่องของพี่อยู่", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "สวัสดีเด็กๆ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "สวัสดีค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ดีครับ", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่ มานี่สิ เพื่อน ๆ กำลังถามถึงพี่อยู่พอดีเลย", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "นี่พี่เป็นคนดังเหรอ ?", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ใช่ค่ะพี่เนีย คุณคือคนดังเรื่องขี้ซึมเลยนะค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เมื่อกี้เราเพิ่งคุยกันว่า พี่เนียดูน่าจะจับออกมานอกบ้านบ้าง แบบว่า ก็ปกติปีนึงจะเห็นสักครั้งจนหนูแทบจำไม่ได้", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None }
        };

        day4.storyChoices = new List<DialogueChoice>
        {
            new DialogueChoice
            {
                choiceButtonText = "นี่กำลังชมหรือกำลังหลอกด่าพี่กันแน่เนี่ย",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "เปล่าครับพี่เนีย ปกติเรารู้จักพี่จากการเล่าของเรน แต่ว่าพี่นะ ไม่ค่อย รึแทบไม่เคยออกมาจากบ้านมาให้เราเห็นเลย", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ทั้ง ๆ ที่เราอยู่บ้านใกล้กัน แต่ผมเห็นพี่นับครั้งได้เลยนะ เราคิดว่าพี่ออกไปข้างนอกบ่อยละมั้ง แต่เรนก็ยืนยันว่าพี่อยู่ในบ้าน", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "และว่างด้วย", speakerName = "Den", portraitName = "idle", stressChange = 5f, activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "(ฉันไม่ควรคิดมากกับคำพูดของเด็กแต่ว่า)", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ลึกลับน่าค้นหาดีไม่ใช่เหรอ เป็นคาแรคเตอร์พิเศษไง", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "พี่เนียเป็นแบบนี้ น่ารักจะตาย", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "(ยิ้ม)", speakerName = "Rein", portraitName = "Idle", stressChange = -15f, activeSlotIndex = 3, motionEffect = SpriteAction.Jump },
                    new DialogueLine { text = "ขอบคุณนะเรน", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "หา? ไปเอาความคิดนั้นจากไหน",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "อึก! คือว่า...", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "แบบว่า", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "เข้าใจผิด ทุกคนแต่เป็นห่วงพี่นะ", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "พี่ รังแกเด็ก จะไปฟ้องแม่", speakerName = "Rein", portraitName = "Idle", stressChange = 5f, activeSlotIndex = 3, motionEffect = SpriteAction.Shake },
                    new DialogueLine { text = "พี่ขอโทษ ไม่ได้ตั้งใจจะให้กลัว", speakerName = "Nia", portraitName = "bad1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "พวกเราหมายถึง", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ปกติเรารู้จักพี่จากการเล่าของเรนแต่ว่าพี่นะ ไม่ค่อย รึแทบไม่เคยออกมาจากบ้านมาให้เราเห็นเลย", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ทั้ง ๆ ที่เราอยู่บ้านใกล้กัน แต่ผมเห็นพี่นับครั้งได้เลยนะเราคิดว่าพี่ออกไปข้างนอกบ่อยละมั้ง แต่เรนก็ยืนยันว่าพี่อยู่ในบ้านนะครับ", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "อย่างนี้นี่เอง", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            },
            new DialogueChoice
            {
                choiceButtonText = "พูดซะพี่ดูเป็นยอดมนุษย์เลยนะ แบบตัวละครเงาอะไรพวกนี้เหรอ",
                stressChange = 0f,
                speakerName = "Nia",
                portraitName = "idle",
                activeSlotIndex = 1,
                nextDialogueLines = new List<DialogueLine>
                {
                    new DialogueLine { text = "ไม่ค่ะ เป็นแบบตัวประกอบมากกว่า", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "แค่เรนมาเล่าเรื่องพี่บ่อย ๆ นะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "ลึกลับน่าค้นหาดีไม่ใช่เหรอ เป็นคาแรคเตอร์พิเศษไง", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "พี่เนียเป็นแบบนี้ น่ารักจะตาย", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
                    new DialogueLine { text = "(ยิ้ม)", speakerName = "Rein", portraitName = "Idle", stressChange = -15f, activeSlotIndex = 3, motionEffect = SpriteAction.Jump },
                    new DialogueLine { text = "ขอบคุณนะเรน", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None }
                }
            }
        };

        day4.conclusionStory = new List<DialogueLine>
        {
            new DialogueLine { text = "(พยักหน้า)", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่ต้องไปโรงเรียนสินะพรุ่งนี้ โรงเรียนของพี่เป็นไงบ้าง", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่บ้านจะให้ผมเข้าโรงเรียนเดียวกันในอนาคตนะ", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
            new DialogueLine { text = "อีกอนสนใจเหรอ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ก็ใช้ได้นะ ในเรื่องการเรียนน่ะ ห้องสมุดก็เงียบดีด้วย", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ผมชอบที่เงียบ ๆ", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None },
            new DialogueLine { text = "คนก็ไม่ค่อยเยอะด้วย", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่มักชอบทำอะไรในโรงเรียนค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เออ คือ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "พี่น่ะสุดยอดนะ!", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "เป็นบรรณารักษ์ห้องสมุดด้วย ถึงจะชอบบ่นเรื่องที่นั่นก็เถอะ", speakerName = "Rein", portraitName = "Idle", stressChange = -10f, activeSlotIndex = 3, motionEffect = SpriteAction.None },
            new DialogueLine { text = "...", speakerName = "Nia", portraitName = "good1", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ละก็", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
            new DialogueLine { text = "เอาเป็นว่าพี่ขอตัวก่อนนะ เรนเองก็รีบ ๆ กลับละ", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "หาาา~~~~ หนูขอต่อเวลา", speakerName = "Rein", portraitName = "Idle", activeSlotIndex = 3, motionEffect = SpriteAction.None },
            new DialogueLine { text = "ก็ได้ เดี๋ยวพี่บอกแม่เอง บ้ายบาย", speakerName = "Nia", portraitName = "idle", activeSlotIndex = 1, motionEffect = SpriteAction.None },
            new DialogueLine { text = "บ้ายบายครับ", speakerName = "Den", portraitName = "idle", activeSlotIndex = 4, motionEffect = SpriteAction.Jump },
            new DialogueLine { text = "บ้ายบายค่ะ", speakerName = "Sasha", portraitName = "idle", activeSlotIndex = 6, motionEffect = SpriteAction.None },
            new DialogueLine { text = "บายครับ", speakerName = "Egon", portraitName = "idle", activeSlotIndex = 7, motionEffect = SpriteAction.None }
        };
    }
}
#endif
