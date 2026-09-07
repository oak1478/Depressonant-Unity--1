using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class ParkNameFixer
{
    static ParkNameFixer()
    {
        EditorApplication.delayCall += FixParkNames;
    }

    [MenuItem("Tools/Fix Park Speaker Names & Profiles")]
    public static void FixParkNames()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null)
        {
            Debug.LogError("[ParkNameFixer] ไม่พบไฟล์: " + path);
            return;
        }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        bool changed = false;

        // 1. ตรวจสอบ Display Name
        if (npc.npcDisplayName != "Park")
        {
            npc.npcDisplayName = "Park";
            changed = true;
        }

        // 2. ตรวจสอบ Profiles
        if (npc.npcProfiles != null)
        {
            foreach (var profile in npc.npcProfiles)
            {
                if (profile != null && profile.npcName == "Prak")
                {
                    profile.npcName = "Park";
                    changed = true;
                }
            }
        }

        // 3. ตรวจสอบทุกบทสนทนาในทุกวัน
        if (npc.dialoguesByDay != null)
        {
            foreach (var day in npc.dialoguesByDay)
            {
                if (day == null) continue;

                // Introduction lines
                if (day.introductionStory != null)
                {
                    foreach (var line in day.introductionStory)
                    {
                        if (line == null) continue;
                        if (line.speakerName == "Prak" || (string.IsNullOrEmpty(line.speakerName) && line.activeSlotIndex == 4))
                        {
                            line.speakerName = "Park";
                            changed = true;
                        }
                    }
                }

                // Choices
                if (day.storyChoices != null)
                {
                    foreach (var choice in day.storyChoices)
                    {
                        if (choice == null) continue;
                        if (choice.speakerName == "Prak")
                        {
                            choice.speakerName = "Park";
                            changed = true;
                        }

                        if (choice.nextDialogueLines != null)
                        {
                            foreach (var nextLine in choice.nextDialogueLines)
                            {
                                if (nextLine == null) continue;
                                if (nextLine.speakerName == "Prak" || (string.IsNullOrEmpty(nextLine.speakerName) && nextLine.activeSlotIndex == 4))
                                {
                                    nextLine.speakerName = "Park";
                                    changed = true;
                                }
                            }
                        }
                    }
                }

                // Conclusion lines
                if (day.conclusionStory != null)
                {
                    foreach (var line in day.conclusionStory)
                    {
                        if (line == null) continue;
                        if (line.speakerName == "Prak" || (string.IsNullOrEmpty(line.speakerName) && line.activeSlotIndex == 4))
                        {
                            line.speakerName = "Park";
                            changed = true;
                        }
                    }
                }
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(npc);
            PrefabUtility.SavePrefabAsset(go);
            AssetDatabase.SaveAssets();
            Debug.Log("✅ [ParkNameFixer] แก้ไขและกู้คืนชื่อผู้พูด 'Park' ใน NPC_Prak.prefab สำเร็จเรียบร้อย!");
        }
    }
}
