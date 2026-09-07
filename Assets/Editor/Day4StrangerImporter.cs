using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day4StrangerImporter
{
    static Day4StrangerImporter()
    {
        EditorApplication.delayCall += ImportStranger;
    }

    [MenuItem("Tools/Import Day 4 Stranger Story (Shirou)")]
    public static void ImportStranger()
    {
        Debug.Log("⏳ [Day4StrangerImporter] เริ่มทำการลงข้อมูลเนื้อเรื่อง Day 4 (A stranger - Shirou)...");
        string path = "Assets/Object/Characters/NPC_Shirou.prefab";
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
            CreateLine("...", "Nia", "idle", "", 0f, 1, SpriteAction.None),
            CreateLine("มองอะไร?", "Shirou", "idle", "A", 0f, 4, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขะ ขอโทษค่ะ!", "Nia", "bad1", -5f, 1, SpriteAction.None, new List<DialogueLine>()),
            CreateChoice("ขะ ขอโทษค่ะ!", "Nia", "bad1", -5f, 1, SpriteAction.None, new List<DialogueLine>()),
            CreateChoice("ขะ ขอโทษค่ะ!", "Nia", "bad1", -5f, 1, SpriteAction.None, new List<DialogueLine>())
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(น่ากลัวชะมัด)", "Nia", "bad1", "", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [Day4StrangerImporter] บันทึกข้อมูล A stranger ลงใน NPC_Shirou.prefab สำเร็จเรียบร้อย!");
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

    private static DialogueLine CreateLine(string text, string speaker, string portrait, string otherName, float stress, int slot, SpriteAction motion)
    {
        return new DialogueLine
        {
            text = text,
            speakerName = speaker,
            portraitName = portrait,
            otherName = otherName,
            stressChange = stress,
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
