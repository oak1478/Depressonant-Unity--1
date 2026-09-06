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
            CreateLine("(กำลังจัดโต๊ะบรรณารักษ์ในยามเย็น เงยหน้าขึ้นมายิ้มให้เนียที่เดินถือพวงกุญแจห้องสมุดมาคืน)", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("สัปดาห์นี้ทำหน้าที่ได้ยอดเยี่ยมมากนะเนีย... ผลงานการจัดระบบของเธอ ทำให้ห้องสมุดกลับมามีชีวิตชีวาขึ้นเยอะเลย", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูเห็นการเติบโตของเธอตลอดทั้งสัปดาห์นี้... เธอสู้กับความรู้สึกตัวเองจนก้าวผ่านมันมาได้ ครูภูมิใจในตัวเธอนะ", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(OFC และ mPFC ทำงานประสานกันอย่างสมบูรณ์แบบ... รับรู้ถึงความจริงใจและคุณค่าในตัวเองอย่างที่ไม่เคยเป็นมาก่อน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ครูโมม่อนคือคนที่มอบพื้นที่ปลอดภัยให้เรา ในวันที่เรามองไม่เห็นคุณค่าของตัวเองเลยสักนิด)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ถึงเวลาบอกขอบคุณครูด้วยความมั่นใจจากหัวใจจริงๆ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณมากๆ ค่ะครูโมม่อน! ถ้าไม่มีครูคอยเชื่อมั่นและให้โอกาสหนู หนูคงยังขังตัวเองอยู่ในมืด ขอบคุณที่ช่วยให้หนูค้นพบคุณค่าในตัวเองนะคะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มอบอุ่น ลูบหัวเนียเบาๆ) เธอทำได้ด้วยตัวเธอเองต่างหากเนีย... จำความรู้สึกนี้ไว้นะ ไม่ว่าเจออะไร เธอผ่านมันไปได้เสมอ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณสำหรับทุกอย่างนะคะครู... หนูดีใจมากที่ได้ช่วยงานห้องสมุด", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ยินดีเสมอจ้ะเนีย ห้องสมุดต้อนรับเธอเสมอทุกเมื่อนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนู... หนูแค่ทำตามที่ครูสั่งน่ะค่ะ ไม่ได้เก่งอะไรขนาดนั้น", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("มั่นใจในตัวเองหน่อยสิเนีย เธอทำได้ดีเยี่ยมจริงๆ ยอมรับมันเถอะนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("กลับบ้านดีๆ ล่ะเนียพักผ่อนให้เต็มที่นะ", "Momon", "idle", 0f, 4, SpriteAction.None)
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
            CreateLine("(เดินสวนกับเนียตรงประตูหน้าโรงเรียน ชะงักเล็กน้อยแล้วเป็นฝ่ายทักขึ้นก่อนด้วยรอยยิ้ม)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อ้าว เนีย! กำลังจะกลับบ้านเหรอจ๊ะ?", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ครูเพิ่งตรวจรายงานกลุ่มที่เธอช่วยจัดระบบส่งมา... ทำได้เป็นระเบียบเรียบร้อยมากเลยนะ", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("สัปดาห์นี้เธอเปลี่ยนไปเยอะเลยนะดูมีความมั่นใจและตั้งใจขึ้นมาก... ต่อจากนี้ครูขอฝากเธอช่วยดูแลเพื่อนๆ ในวิชาเคมีด้วยล่ะ!", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(Hippocampus และ SFG ไม่มีความทรงจำเลวร้ายหลงเหลืออยู่อีกแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ครูวิภาไม่เพียงแต่จำชื่อ เนีย ได้ถูกต้อง แต่ยังมอบความไว้วางใจให้อย่างเต็มที่)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("รับทราบค่ะครูวิภา! หนูจะตั้งใจเรียนและช่วยเพื่อนๆ เต็มที่ ขอบคุณครูมากๆ นะคะ!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(หัวเราะเบาๆ อย่างอารมณ์ดี) ดีมากจ้ะเนีย! ไว้เจอกันสัปดาห์หน้านะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบคุณค่ะครูวิภา หนูจะพยายามทำให้ดีที่สุดค่ะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะ เดินทางกลับบ้านปลอดภัยนะเนีย", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูไม่แน่ใจว่าจะช่วยคนอื่นได้ไหมค่ะครู...", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
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
            CreateLine("(เดินกอดแขนเนียระหว่างเดินออกจากอาคารเรียนด้วยรอยยิ้มสดใส)", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย... ศุกร์นี้ไม่เหมือนศุกร์ที่แล้วเลยเนอะ!", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ฉันดีใจมากๆ เลยนะที่เห็นเธอยิ้มได้กว้างขนาดนี้ ได้หัวเราะกับทุกคน ได้ทำสิ่งที่ชอบ...", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เธอรู้ไหมว่าเธอเก่งมากๆ เลยนะที่ก้าวผ่านความรู้สึกแย่ๆ พวกนั้นมาได้ ฉันภูมิใจในตัวเธอที่สุดเลย!", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("(OFC ส่งสัญญาณความสุขเต็มเปี่ยม... สมองสะท้อนภาพความผูกพันที่แท้จริง)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(รินไม่ได้มองเราเป็นภาระ... แต่รินคือเพื่อนแท้ที่คอยยื่นมือมาจับเราไว้เสมอ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ริน... ขอบใจมากๆ นะสำหรับทุกอย่าง ขอบใจที่ไม่เคยปล่อยมือฉันในวันที่ฉันแย่ที่สุด จากนี้ฉันจะเข้มแข็งขึ้น เพื่ออยู่ข้างๆ เธอเหมือนกัน!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ตาประกาย รวบตัวเนียมากอดแน่น) ฮืออ เนีย! สัญญาแล้วนะ! เราจะเป็นเพื่อนที่ดีที่สุดของกันและกันตลอดไปเลย!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ขอบใจนะริน เพราะเธอนั่นแหละที่คอยช่วยฉันไว้ตลอด", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เราช่วยกันและกันต่างหากล่ะเนีย!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("อย่าเว่อร์ไปหน่อยเลยริน... มันก็แค่สัปดาห์ธรรมดาๆ", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("ไม่เว่อร์สักหน่อย! เธอเก่งขึ้นจริงๆ นะเนีย", "Rin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("กลับบ้านไปพักผ่อนเยอะๆ นะเนีย ไว้คุยกันในแชตนะ!", "Rin", "idle", 0f, 4, SpriteAction.None)
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
            CreateLine("(ยืนพิงเสาข้างสนามบาสเกตบอล ชูสองนิ้วทักทายเนียที่กำลังจะเดินผ่านประตูโรงเรียน)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไง เนีย! สรุปว่ารอดตายจากสัปดาห์มหาโหดมาได้แบบครบสามสิบสองนะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เออ... จะว่าไป พอเธอเลิกทำตัวซังกะตาย โรงเรียนก็ดูน่าอยู่ขึ้นเยอะเหมือนกันนะเนี่ย", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("คราวหน้าคราวหลังมีเรื่องอะไรก็พูดออกมาตรงๆ แบบนี้แหละ อย่าเอาแต่เงียบให้อึดอัดอีก", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ตรวจจับคำพูดประโคมกวนๆ ของป้ากด้วยความสงบและหยอกล้อกลับได้)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เราเข้าใจสไตล์การพูดของป้ากแล้ว... มันคือความปรารถนาดีในแบบของผู้ชายขวานผ่าซาก)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ก็นะป้าก! ถ้าฉันไม่ทำตัวสดใส นายก็ไม่มีคนให้แขวะพอดีสิ คราวหน้าก็เพลามือลงหน่อยละกัน!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(หัวเราะร่า ชอบอกชอบใจ) ฮ่าๆๆ! เถียงคำไม่ตกฟากแบบนี้สิค่อยสมเป็นเธอหน่อย! เออ เจอกันสัปดาห์หน้ายัยตัวแสบ!", "Park", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ขอบใจนะป้าก... ที่ทักเตือนสติฉันวันก่อน", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เออๆ ไม่เป็นไร ถือว่าช่วยๆ กันในฐานะเพื่อนร่วมห้อง", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("นายก็เลิกปากเสียให้ได้ก่อนเถอะป้าก!", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เห้อ... อุตส่าห์คุยดีๆ ด้วยนะเนี่ย ไปซ้อมบาสดีกว่า", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("บายเนีย! กลับบ้านดีๆ ล่า!", "Park", "idle", 0f, 4, SpriteAction.None)
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

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 16);
        day.dayTitle = "DAY 16";
        day.dayNumber = 16;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาหาเนียพร้อมกับชีอ่าที่ยืนทำหน้าอึดอัดอยู่ข้างๆ)", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("เนีย! จะกลับบ้านแล้วเหรอ? วันนี้พวกเราว่าจะไปกินไอศกรีมหน้าโรงเรียน... ไปด้วยกันไหม?", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("(กระแอมเบาๆ เชิดหน้าขึ้นเล็กน้อยแต่สายตามองเนียตรงๆ)", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อืม... เรื่องเมื่อวันศุกร์ก่อนนู้นน่ะ... ฉันอาจจะพูดแรงไปหน่อย เรื่องที่ว่าเธอเป็นตัวดูดพลังงาน... ก็ ขอโทษละกัน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เห็นเธอตั้งใจทำหน้าที่บรรณารักษ์แล้วก็เคลียร์ใจกับทุกคน... เธอก็ไม่ได้แย่อย่างที่ฉันคิดหรอกนะ", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(SFG และ mPFC ตระหนักรู้ถึงการปลดล็อกความสัมพันธ์ทั้งหมดอย่างสมบูรณ์)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คำขอโทษจากชีอ่า คือเครื่องยืนยันว่าบาดแผลในจิตใจของเราได้รับการเยียวยาแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เราก้าวข้ามปมเรื่องเพื่อนที่โรงเรียนได้สำเร็จอย่างแท้จริง)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ช่างมันเถอะชีอ่า ฉันเองก็ขอโทษเหมือนกันนะ... ขอบใจนะหงส์ ไว้คราวหน้าฉันไปกินไอศกรีมด้วยแน่นอน วันนี้ขอตัวกลับบ้านก่อนนะ!", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(ยิ้มสดใส) โอเคเลยเนีย! สัปดาห์หน้าไปกินด้วยกันนะ!", "Hong", "", 0f, 6, SpriteAction.None),
                CreateLine("(พยักหน้ารับเบาๆ) อืม... กลับบ้านดีๆ ละกันเนีย", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ไม่เป็นไรหรอกชีอ่า... ขอบใจนะ หงส์ไว้โอกาสหน้านะจ๊ะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะเนีย แล้วเจอกันวันจันทร์นะ!", "Hong", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("เพิ่งมารู้ตัวเหรอว่าพูดแรง? ช่างมันเถอะ ฉันไม่อยากพูดถึงอีก", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เอ้า! คนเขาอุตส่าห์ขอโทษดีๆ! ช่างเถอะหงส์ ไปกันได้แล้ว!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(เดินออกจากรั้วโรงเรียนด้วยหัวใจที่เบาสบาย และความรู้สึกปลอดโปร่งอย่างที่ไม่เคยสัมผัสมาก่อน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ปัญหาที่โรงเรียน... ครู เพื่อน คะแนนสอบ ทุกอย่างถูกคลี่คลายและประสานรอยแตกร้าวหมดแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันพร้อมที่จะกลับบ้าน... พร้อมที่จะเผชิญหน้ากับวันพรุ่งนี้อย่างมั่นใจแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
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
