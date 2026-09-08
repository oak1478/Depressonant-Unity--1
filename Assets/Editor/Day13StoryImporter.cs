using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class Day13StoryImporter
{
    static Day13StoryImporter()
    {
        EditorApplication.delayCall += ImportAllDay13;
    }

    [MenuItem("Tools/Import Day 13 Story (Momon, Rin, Shia, Park, Vipar)")]
    public static void ImportAllDay13()
    {
        Debug.Log("[Day13StoryImporter] Starting Day 13 story import...");
        ImportMomon();
        ImportRin();
        ImportShia();
        ImportPrak();
        ImportVipar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Day13StoryImporter] Day 13 story import complete!");
    }

    private static void ImportMomon()
    {
        string path = "Assets/Object/Characters/NPC_Momon.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 13);
        day.dayTitle = "DAY 13";
        day.dayNumber = 13;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(ยื่นแฟ้มจัดหมวดหมู่หนังสือเล่มใหม่ให้เนีย พร้อมมองดูชั้นหนังสือที่ถูกเรียงไว้อย่างเป็นระเบียบ)", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ระบบดรรชนีหมวดใหม่ที่เธอเสนอมั่นใจว่าทำเสร็จทันวันนี้ใช่ไหมเนีย?", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เสร็จเรียบร้อยแล้วค่ะครู... หนูจัดแบ่งตามหมวดรหัสสีใหม่ ทำให้หาหนังสือหมวดวิทยาศาตร์กับวรรณกรรมง่ายขึ้นเยอะเลยค่ะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เดินไปสำรวจที่ชั้นหนังสือ ลองหยิบออกมาเล่มหนึ่งแล้วยิ้มมุมปาก)", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เรียบร้อยและละเอียดมาก... ครูอยู่ห้องสมุดนี้มาหลายปี ยังไม่เคยเห็นใครจัดระบบได้เข้ามือเท่าเธอเลยนะ", "Momon", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(mPFC เริ่มสะท้อนภาพตัวเองได้ชัดเจนขึ้น... ม่านฝ้ามัวลดลง)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ความรู้สึกภาคภูมิใจเล็กๆ เริ่มเกิดขึ้น โดยที่ไม่มีเสียงลบในหัวคอยค้านเหมือนเมื่อก่อน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เรา... ทำสิ่งที่ชอบได้ดีจริงๆ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครู! หนูชอบงานในห้องสมุดจริงๆ ค่ะ และหนูตั้งใจทำมันเต็มที่", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ดีแล้วล่ะ การได้ทำสิ่งที่ตัวเองชอบและทำมันได้ดี นั่นคือคุณค่าที่ไม่มีใครแย่งไปจากเธอได้นะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หนูแค่ลองทำดูตามหน้าที่น่ะค่ะ...", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ต่อให้ทำตามหน้าที่ แต่ผลงานมันฟ้องว่าเธอใส่ใจ ยอมรับคำชมบ้างเถอะนะ", "Momon", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(ก้มหน้าหลบตา) หนูไม่แน่ใจว่าคนอื่นจะชอบระบบนี้ไหม...", "Nia", "bad1", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เชื่อมั่นในตัวเองหน่อยสิเนีย เธอทำได้ดีแล้ว", "Momon", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ไปพักกินข้าวเที่ยงได้แล้วล่ะ ที่เหลือเดี๋ยวครูเฝ้าต่อให้เอง", "Momon", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Momon] อัปเดต Day 13 สำเร็จ");
    }

    private static void ImportRin()
    {
        string path = "Assets/Object/Characters/NPC_Rin.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 13);
        day.dayTitle = "DAY 13";
        day.dayNumber = 13;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาในห้องสมุด กวาดสายตามองชั้นหนังสือที่เปลี่ยนไปอย่างมีระเบียบ)", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("โห... เนีย! ป้ายหมวดหมู่รหัสสีพวกนี้เธอทำเองหมดเลยเหรอ?", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เมื่อกี้เห็นเพื่อนห้องอื่นคุยกันว่า ห้องสมุดค้นหนังสือง่ายขึ้นเยอะเลย เพราะบรรณารักษ์คนใหม่จัดระบบไว้ดีมาก", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อื้ม... ฉันกับครูโมม่อนช่วยกันทำเมื่อเช้าน่ะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ยิ้มสดใส) เธอเนี่ยเวลานั่งทำงานในห้องสมุด ดูมีเสน่ห์และดูมั่นใจมากๆ เลยนะเนีย!", "Rin", "idle", 0f, 4, SpriteAction.Jump),
            CreateLine("ฉันชอบเวลาเธอทำสิ่งที่เธอรักจัง มันดูส่องประกายมากๆ เลย!", "Rin", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(pgACC ตรวจจับความรู้สึกดีๆ โดยไม่มีเสียงคัดค้านในหัวอีกต่อไป)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คำพูดของรินทำให้เรารู้สึกว่า... เราเองก็มีจุดแข็งและความสุขในแบบของตัวเองเหมือนกัน)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบใจนะริน... เพราะเธอคอยให้กำลังใจฉันด้วยแหละ ฉันเลยกล้าทำสิ่งนี้", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ฮืออ เนีย! ฉันดีใจจริงๆ ที่เห็นเธอกลับมามีความสุขกับสิ่งที่ชอบนะ!", "Rin", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ขอบใจนะริน... ฉันก็แค่ชอบอยู่นิ่งๆ กับหนังสือน่ะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("นั่นแหละคือความถนัดของเธอเลยล่ะ!", "Rin", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("อวยเกินไปแล้วมั้งริน... ก็แค่งานจัดหนังสือธรรมดาๆ", "Nia", "bad1", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ไม่เกินจริงเลยสักนิด! เธอเก่งจริงๆ นะ!", "Rin", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นฉันไม่รบกวนเวลาทำงานของเธอนะเนีย สู้ๆ น้า!", "Rin", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Rin] อัปเดต Day 13 สำเร็จ");
    }

    private static void ImportShia()
    {
        string path = "Assets/Object/Characters/NPC_Shia.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 13);
        day.dayTitle = "DAY 13";
        day.dayNumber = 13;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเข้ามาเลือกหนังสือติวในห้องสมุดกับชีอ่า สะดุดตากับชั้นหนังสือเคมีที่จัดไว้อย่างเป็นระเบียบ)", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("เนีย... หนังสือหมวดสอบเข้ามหาวิทยาลัยนี่ เธอเป็นคนแยกเล่มโจทย์กับเนื้อหาออกจากกันเหรอ?", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("ใช่จ้ะหงส์... ฉันเห็นว่าคนส่วนใหญ่ชอบมาหาเล่มโจทย์ทำ เลยแยกไว้ให้หยิบง่ายๆ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("โห สุดยอดเลย! หยิบง่ายขึ้นเยอะเลย ขอบใจมากนะเนีย!", "Hong", "", 0f, 6, SpriteAction.None),
            CreateLine("(ยืนมองชั้นหนังสือรอบๆ ก่อนจะกระแอมเบาๆ แล้วพูดขึ้น)", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อืม... ก็ต้องยอมรับนะว่าเธอทำระบบห้องสมุดดีจริง ดีกว่าปล่อยให้หนังสือรกรุงรังเหมือนเมื่อก่อน", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("อย่างน้อยเธอก็ไม่ได้ไร้ประโยชน์ไปซะทุกเรื่อง... ตรงนี้เธอทำได้ดีมากเนีย", "Shia", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(SFG จัดระเบียบความคิดได้อย่างมั่นคง... คำพูดของชีอ่าไม่ได้ทำให้เราดิ่งอีกต่อไป)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(เราไม่จำเป็นต้องให้ทุกคนมาชอบเรา แต่เรารับรู้ได้ว่าคำยอมรับนี้มาจากผลงานที่เราทำจริงๆ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบใจนะชีอ่า หงส์ มีเล่มไหนหาไม่เจอถามฉันได้ตลอดเลยนะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("อืม... ไว้ถ้าหาไม่เจอจะมาถามละกัน", "Shia", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ขอบใจนะ... ฉันแค่ทำตามหน้าที่น่ะ", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เนียทำได้ดีมากจริงๆ จ้ะ!", "Hong", "", 0f, 6, SpriteAction.None)
            }),
            CreateChoice("จะประชดฉันอีกหรือไงชีอ่า?", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
            {
                CreateLine("เอ้า! ชมก็หาว่าประชดอีก อะไรของเธอเนี่ย?!", "Shia", "idle", 0f, 4, SpriteAction.Shake)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("งั้นพวกเราไปนั่งติวตรงนู้นก่อนนะเนีย", "Hong", "", 0f, 6, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Shia] อัปเดต Day 13 สำเร็จ");
    }

    private static void ImportPrak()
    {
        string path = "Assets/Object/Characters/NPC_Prak.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 13);
        day.dayTitle = "DAY 13";
        day.dayNumber = 13;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินถือลูกบาสสไลด์ตัวเข้ามาในห้องสมุด สายตากวาดมองเนียที่กำลังยืนเช็ดเคาน์เตอร์บรรณารักษ์)", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ไง เนีย นั่งเฝ้าถ้ำหนังสือทั้งวันไม่เบื่อมั่งเหรอ?", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("แต่ก็นะ... วันนี้หน้าตาเธอดูไม่เหมือนคนโดนวิญญาณหลอนเหมือนอาทิตย์ก่อนแล้วแฮะ", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ดูท่าเธอจะชอบงานเฝ้าหนังสือจริงจังนะเนี่ย ดูเอาการเอางานใช้ได้เลยนี่", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(Hippocampus ดึงภาพความทรงจำแย่ๆ ในอดีตออกไป... แทนที่ด้วยภาพความสำเร็จในปัจจุบัน)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(คำพูดตรงๆ ของป้ากครั้งนี้ ฟังดูเป็นคำชมในสไตล์ของเขาจริงๆ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ก็นะป้าก... งานเฝ้าหนังสือมันเหมาะกับคนสงบๆ อย่างฉัน มากกว่าวิ่งไล่ลูกบาสแบบนายไงล่ะ", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("(หัวเราะร่า) เออจริง! ให้ฉันมานั่งเฝ้าหนังสือทั้งวันคงคลั่งตายแน่ แต่ละคนก็มีความถนัดต่างกันสินะ!", "Park", "idle", 0f, 4, SpriteAction.Jump)
            }),
            CreateChoice("ก็นิดหน่อยน่ะ... อย่างน้อยอยู่ที่นี่ฉันก็สบายใจดี", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เออ ก็ดีแล้ว เห็นเธอมีเรื่องทำแล้วดูเป็นผู้เป็นคนขึ้นเยอะ", "Park", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("หน้าตาฉันเหมือนผีหลอนขนาดนั้นเลยหรือไง?", "Nia", "idle", 5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("ก็เออดิ แต่ตอนนี้ดีขึ้นละ ถือว่าฉันชมละกัน!", "Park", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("ไปละ จะไปซ้อมบาสต่อ อย่าก้มหน้าอ่านหนังสือจนคอหักล่ะ!", "Park", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(มองตามหลังทุกคนไป พร้อมกับรอยยิ้มบางๆ บนใบหน้า)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ฉันเริ่มมองเห็นตัวตนของตัวเองในกระจกชัดเจนขึ้นมาอีกก้าวหนึ่งแล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("✅ [Park] อัปเดต Day 13 สำเร็จ");
    }

    private static void ImportVipar()
    {
        string path = "Assets/Object/Characters/NPC_Vipar.prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) { Debug.LogError("ไม่พบไฟล์: " + path); return; }

        NPCInteraction npc = go.GetComponent<NPCInteraction>();
        if (npc == null) return;

        DailyDialogue day = GetOrCreateDay(npc.dialoguesByDay, 13);
        day.dayTitle = "DAY 13";
        day.dayNumber = 13;

        // --- Intro ---
        day.introductionStory = new List<DialogueLine>
        {
            CreateLine("(เดินเร่งรีบเข้ามาในห้องสมุด สายตากวาดมองหาเอกสารบนโต๊ะ)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("เนีย! พอดีครูต้องการเอกสารอ้างอิงตารางธาตุเล่มเก่าของหมวดเคมีด่วนมาก ไม่รู้เก็บไว้ไหน...", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(เดินไปที่ชั้นหมวดเคมีทันที ยื่นมือหยิบหนังสือเล่มที่ต้องการออกมาส่งให้ครูวิภาในเวลาไม่ถึงสิบวินาที)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("เล่มนี้ใช่ไหมคะครูวิภา? หนูแยกหมวดอ้างอิงพิเศษไว้ตรงนี้ค่ะ", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(รับหนังสือไปมองด้วยความทึ่ง ชะงักไปครู่หนึ่งแล้วมองหน้าเนียตรงๆ)", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("หาเจอไวมาก! โทษทีนะ... ครูจำสลับตลอด แต่เธอชื่อ เนีย เลขที่ 7 ใช่ไหม?", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("ขอบใจมากนะเนีย เธอทำหน้าที่บรรณารักษ์ได้เก่งมากจริงๆ ช่วยครูได้เยอะเลย!", "Vipar", "idle", 0f, 4, SpriteAction.None),
            CreateLine("(OFC ประมวลผลคำชมอย่างตรงไปตรงมา... สัญญาณพลังบวกถูกส่งเข้าสู่สมองเต็มๆ)", "Nia", "good1", 0f, 1, SpriteAction.None),
            CreateLine("(ครูวิภาจำชื่อเราได้แล้ว... และเธอกำลังชมเราจากผลงานจริงๆ ไม่ใช่คำปลอบใจ)", "Nia", "good1", 0f, 1, SpriteAction.None)
        };

        // --- Choices ---
        day.storyChoices = new List<DialogueChoice>
        {
            CreateChoice("ขอบคุณค่ะครูวิภา! หนูเนีย เลขที่ 7 ค่ะ มีอะไรให้ช่วยในห้องสมุดบอกหนูได้ตลอดเลยนะคะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("จ้ะเนีย! ไว้ครูจะมาอุดหนุนบริการห้องสมุดบ่อยๆ นะ!", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("ยินดีค่ะครู... หนูจำตำแหน่งหนังสือได้หมดอยู่แล้ว", "Nia", "idle", -5f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("เยี่ยมมากจ้ะ ทำหน้าที่ได้ดีมากเลยนะ", "Vipar", "idle", 0f, 4, SpriteAction.None)
            }),
            CreateChoice("(พยักหน้ารับเบาๆ) ค่ะ...", "Nia", "idle", 0f, 1, SpriteAction.None, new List<DialogueLine>
            {
                CreateLine("งั้นครูขอตัวก่อนนะ ขอบใจอีกครั้ง", "Vipar", "idle", 0f, 4, SpriteAction.None)
            })
        };

        // --- Conclusion ---
        day.conclusionStory = new List<DialogueLine>
        {
            CreateLine("(รีบถือหนังสือเดินออกจากห้องสมุดไปพร้อมรอยยิ้ม)", "Vipar", "idle", 0f, 4, SpriteAction.None)
        };

        EditorUtility.SetDirty(npc);
        PrefabUtility.SavePrefabAsset(go);
        Debug.Log("[Vipar] Day 13 import complete");
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
