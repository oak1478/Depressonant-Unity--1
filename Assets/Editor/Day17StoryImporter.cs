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

        // ลบข้อมูล Day 17 เดิมออกเพื่อลงชุดใหม่ทั้ง 3 ตอนจบ
        if (npc.dialoguesByDay == null) npc.dialoguesByDay = new List<DailyDialogue>();
        npc.dialoguesByDay.RemoveAll(d => d.dayNumber == 17);

        // -------------------------------------------------------------
        // 1. NORMAL ENDING (Ending_Normal)
        // -------------------------------------------------------------
        DailyDialogue normalDay = new DailyDialogue
        {
            dayNumber = 17,
            dayTitle = "Ending_Normal",
            introductionStory = new List<DialogueLine>
            {
                CreateLine("(เช้าวันเสาร์ที่เงียบสงบ ฉันเดินลงมาที่ห้องนั่งเล่น... แต่ครั้งนี้ต่างออกไป พ่อ แม่ และเรน นั่งรวมตัวกันอยู่ที่โซฟาเหมือนกำลังรอฉันอยู่)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("เนีย... มานั่งนี่สิ พ่อกับแม่มีเรื่องอยากจะคุยด้วยหน่อย", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(ขยับตัวเว้นที่ว่างให้ฉันนั่ง ท่าทางของแม่ดูประหม่าเล็กน้อย ไม่ได้วางอำนาจเหมือนปกติ)", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("หลายวันที่ผ่านมา... แม่เห็นว่าลูกดูโอเคขึ้นนะ กลับมาคุยกับเพื่อน ช่วยงานที่โรงเรียน... แม่ก็เลยลองกลับมามองดูตัวเองบ้าง", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อกับแม่คุยกันแล้วนะเนีย... พวกเรายอมรับว่าที่ผ่านมาบรรยากาศในบ้านเรามันตึงเครียด พ่อเอาความเครียดเรื่องงานมาลงที่บ้าน ส่วนแม่ก็จู้จี้กับลูกมากไปจนลูกอึดอัด", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(เอื้อมมือมาจับมือฉันหลวมๆ) พวกเราไม่ได้อยากให้พี่เนียรู้สึกว่าต้องสู้คนเดียวนะ... เราอยากเป็นครอบครัวที่คุยกันได้ทุกเรื่องจริงๆ", "Rain", "", 0f, 7, SpriteAction.None),
                CreateLine("(mPFC ประมวลผลภาพตรงหน้า... มันไม่ได้สมบูรณ์แบบ แววตาของพ่อยังมีรอยเหนื่อยล้า ท่าทางของแม่ยังดูมีความพยายามฝืนธรรมชาติอยู่บ้าง)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("(มันไม่ใช่ฉากกอดกันร้องไห้ฟูมฟายแบบในละครที่ทุกอย่างจะหายไปในพริบตา... แต่มันคือความเป็นจริง)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("(pgACC สงบนิ่ง... ไม่มีสัญญาณเตือนภัยอีกแล้ว SFG บอกฉันว่า 'ความไว้ใจ' ไม่ได้สร้างเสร็จในวันเดียว แต่ตอนนี้พวกเขากำลังวางอิฐก้อนแรกให้แล้ว)", "Nia", "good1", 0f, 1, SpriteAction.None)
            },
            storyChoices = new List<DialogueChoice>
            {
                CreateChoice("หนูเข้าใจค่ะ... มันคงต้องใช้เวลาปรับตัวกันอีกเยอะ แต่หนูก็จะพยายามไม่หนีปัญหาและเปิดใจกับทุกคนให้มากขึ้นเหมือนกันนะคะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("(ยิ้มบางๆ) พ่อก็หวังแบบนั้นแหละเนีย... ค่อยๆ เป็นค่อยๆ ไปนะลูก", "Dad", "", 0f, 5, SpriteAction.None)
                }),
                CreateChoice("ขอบคุณนะคะที่ยอมรับฟัง... แค่นี้หนูก็รู้สึกดีขึ้นมากแล้วค่ะ เรามาค่อยๆ ปรับกันไปนะคะ", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("(ถอนหายใจอย่างโล่งอก) จ้ะ... แม่ก็จะพยายามบ่นให้น้อยลงละกันนะ", "Mom", "idle", 0f, 4, SpriteAction.None)
                }),
                CreateChoice("หนูเองก็ต้องขอโทษที่เอาแต่ขังตัวเองค่ะ... ต่อจากนี้มีอะไรหนูจะพูดตรงๆ นะคะ", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("เย้! งั้นวันนี้เราไปหาอะไรอร่อยๆ กินกันทั้งบ้านเลยดีไหมคะ!", "Rain", "", 0f, 7, SpriteAction.Jump)
                })
            },
            conclusionStory = new List<DialogueLine>
            {
                CreateLine("(ฉันมองดูครอบครัวของฉัน...)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("(บาดแผลในใจไม่ได้หายไปสนิท และความไม่เข้าใจกันก็คงจะมีเกิดขึ้นอีกในอนาคต)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("(แต่ครั้งนี้ ฉันรู้แล้วว่าตัวเองมีค่าพอที่จะถูกรัก และพวกเขาก็พร้อมที่จะเรียนรู้ไปพร้อมกับฉัน...)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(นี่อาจจะไม่ใช่ตอนจบที่สมบูรณ์แบบที่สุด... แต่มันคือจุดเริ่มต้นของชีวิตที่ 'ปกติ' และมีรอยยิ้มได้อีกครั้ง)", "Nia", "good1", 0f, 1, SpriteAction.None)
            }
        };
        npc.dialoguesByDay.Add(normalDay);

        // -------------------------------------------------------------
        // 2. BAD ENDING (Ending_Bad)
        // -------------------------------------------------------------
        DailyDialogue badDay = new DailyDialogue
        {
            dayNumber = 17,
            dayTitle = "Ending_Bad",
            introductionStory = new List<DialogueLine>
            {
                CreateLine("(เช้าวันเสาร์... ฉันเดินลงมาจากห้องด้วยความรู้สึกที่อยากจะลองเปิดใจให้ครอบครัวอีกครั้ง ตามที่สัญญากับตัวเองไว้)", "Nia", "idle", 0f, 1, SpriteAction.None),
                CreateLine("(แต่ทันทีที่ก้าวเท้าลงมาถึงห้องนั่งเล่น บรรยากาศกลับเย็นเยียบ พ่อ แม่ และเรน นั่งอยู่ที่โซฟาด้วยสีหน้าที่ตึงเครียดสุดขีด)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("เนีย... มานั่งนี่ พ่อกับแม่มีเรื่องต้องคุยให้รู้เรื่องวันนี้ จะได้จบๆ ปัญหาสักที", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(กอดอก ถอนหายใจเสียงดัง) แม่ทนไม่ไหวแล้วนะเนีย แม่ไปคุยกับป้าจินมา เขาบอกว่าแกทำหน้าเหมือนคนแบกโลกทั้งใบอยู่ตลอดเวลา... แกจะเรียกร้องความสนใจไปถึงไหน?", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อทำงานหาเงินมาเลี้ยงพวกแกก็เหนื่อยแทบขาดใจแล้วนะเนีย! ทำไมแกถึงต้องสร้างเรื่องให้บ้านเราดูเป็นบ้านที่มีปัญหาด้วย? แกขาดเหลืออะไรตรงไหนฮะ?!", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(น้ำตาคลอ พยายามจับมือพ่อ) พ่อคะ... แม่คะ... อย่าดุพี่เนียเลย พี่เนียกำลังพยายามอยู่นะคะ...", "Rain", "", 0f, 7, SpriteAction.None),
                CreateLine("เรน! เงียบไปเลย! ปล่อยให้พี่เขารู้ตัวซะทีว่ากำลังทำให้คนอื่นเขาปั่นป่วนแค่ไหน!", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("(pgACC กรีดร้องลั่นสมอง... สัญญาณเตือนภัยทำงานหนักที่สุดในชีวิต ความรู้สึกปลอดภัยพังทลายลงในพริบตา)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("(mPFC ดับวูบ... กระจกสะท้อนตัวตนที่เพิ่งประกอบขึ้นมาใหม่ที่โรงเรียน ถูกพ่อกับแม่ทุบทำลายจนละเอียดไม่มีชิ้นดี)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("(SFG สั่งการให้ร่างกายชาหนึบ... พวกเขาไม่เคยฟัง พวกเขาไม่เคยพยายามจะเข้าใจ ฉันมันก็แค่ 'ตัวปัญหา' ที่ทำให้พวกเขาอับอาย)", "Nia", "bad1", 0f, 1, SpriteAction.None)
            },
            storyChoices = new List<DialogueChoice>
            {
                CreateChoice("(น้ำตาไหลพราก) พ่อกับแม่ก็ห่วงแต่หน้าตาตัวเอง! เคยถามหนูสักคำไหมว่าข้างในหนูพังแค่ไหน! หนูไม่อยากอยู่บ้านนี้แล้ว!", "Nia", "bad1", 15f, 1, SpriteAction.Shake, new List<DialogueLine>
                {
                    CreateLine("กล้าขึ้นเสียงกับพ่อเหรอเนีย?! ถ้าคิดว่าเก่งนักก็กลับขึ้นห้องไปเลย แล้วไม่ต้องลงมาให้เห็นหน้าอีกนะ!", "Dad", "", 0f, 5, SpriteAction.Shake)
                }),
                CreateChoice("(ตัวสั่น ก้มหน้ามองพื้น) หนู... หนูขอโทษค่ะ... หนูมันแย่เอง หนูมันเป็นตัวภาระ...", "Nia", "bad1", 10f, 1, SpriteAction.Shake, new List<DialogueLine>
                {
                    CreateLine("เลิกบีบน้ำตาสักทีเนีย! แม่เบื่อที่จะต้องมารู้สึกผิดเวลาแกทำท่าทางแบบนี้แล้วนะ หัดเข้มแข็งซะบ้างสิ!", "Mom", "idle", 0f, 4, SpriteAction.Shake)
                }),
                CreateChoice("(จ้องมองพ่อกับแม่ด้วยสายตาที่ว่างเปล่า... ถอยหลังช้าๆ แล้วหันหลังเดินกลับขึ้นบันไดโดยไม่พูดอะไรสักคำ)", "Nia", "bad1", 10f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("นี่พ่อพูดยังไม่จบนะเนีย! จะเดินหนีไปไหน! กลับมาเดี๋ยวนี้นะ!", "Dad", "", 0f, 5, SpriteAction.Shake)
                })
            },
            conclusionStory = new List<DialogueLine>
            {
                CreateLine("(ฉันเดินกลับเข้ามาในห้อง... ล็อกประตูอย่างแน่นหนา)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("(เสียงทุบประตูของพ่อและเสียงร้องไห้ของเรนดังอยู่ข้างนอก... แต่มันส่งไม่ถึงใจฉันอีกแล้ว)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("(Hippocampus ฉายซ้ำแต่ภาพความล้มเหลว... ไม่มีใครรักฉัน ไม่มีที่ไหนปลอดภัยสำหรับฉัน ทั้งโลกนี้มีแค่ความมืด)", "Nia", "bad1", 0f, 1, SpriteAction.None),
                CreateLine("(ฉันทิ้งตัวลงบนเตียง ดึงผ้าห่มขึ้นมาคลุมโปง... ปล่อยให้ความมืดมิดกลืนกินตัวตนของฉันไปตลอดกาล...)", "Nia", "bad1", 0f, 1, SpriteAction.None)
            }
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
                CreateLine("(เช้าวันเสาร์ที่สดใส แสงแดดอุ่นๆ ลอดผ่านหน้าต่างลงมาที่โต๊ะอาหาร... ฉันเดินลงมาจากห้องนอนด้วยหัวใจที่เบาสบายอย่างที่ไม่เคยเป็นมาก่อน)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(พ่อ แม่ และเรน นั่งรออยู่พร้อมหน้าพร้อมตา ที่โต๊ะมีอาหารเช้าของโปรดของฉันวางเตรียมไว้ทั้งหมด)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(ยิ้มให้อย่างอบอุ่นและจริงใจ ลุกขึ้นเดินมาหาฉัน) เนีย... มานั่งกินข้าวด้วยกันสิ พ่อกับแม่มีเรื่องสำคัญอยากจะคุยกับเราหน่อย", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(มองฉันด้วยสายตาที่เปี่ยมไปด้วยความรักและความอ่อนโยน เดินเข้ามาจับมือฉันไว้) เนีย... แม่ขอโทษนะลูก ที่ผ่านมาแม่เอาแต่กดดันแก เอาแกไปเปรียบเทียบกับคนอื่น แม่ลืมมองไปเลยว่าลูกของแม่พยายามหนักแค่ไหน...", "Mom", "idle", 0f, 4, SpriteAction.None),
                CreateLine("พ่อก็ขอโทษนะเนีย พ่อเอาความเครียดเรื่องงานมาลงที่บ้าน และมองข้ามความรู้สึกของลูกไป พ่อเห็นความพยายามและความเปลี่ยนแปลงของเนียตลอดสัปดาห์นี้แล้วนะ... พ่อภูมิใจในตัวลูกมากจริงๆ", "Dad", "", 0f, 5, SpriteAction.None),
                CreateLine("(โผเข้ากอดเอวฉันไว้แน่น ยิ้มทั้งน้ำตา) หนูรักพี่เนียที่สุดเลยนะ! พี่เนียไม่ต้องแบกอะไรไว้คนเดียวอีกแล้วนะ พวกเราอยู่ตรงนี้เสมอ!", "Rain", "", 0f, 7, SpriteAction.Jump),
                CreateLine("(OFC เปล่งส่องสว่างด้วยความสุขล้นทะลัก... สัญญาณความรู้สึกดีๆ ซึมลึกเข้าสู่หัวใจอย่างสมบูรณ์)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(pgACC สงบนิ่งและเต็มไปด้วยความผ่อนคลาย mPFC สะท้อนภาพตัวตนที่มีคุณค่า มีเป้าหมาย และเป็นที่รักของทุกคน)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(Hippocampus หลอมรวมความทรงจำแย่ๆ ในอดีตแล้วแทนที่ด้วยภาพความรักและอ้อมกอดอันอบอุ่นของครอบครัว... ม่านดำมืดพังทลายลงอย่างสิ้นเชิง)", "Nia", "good1", 0f, 1, SpriteAction.None)
            },
            storyChoices = new List<DialogueChoice>
            {
                CreateChoice("(ยิ้มกว้างด้วยความมั่นใจและน้ำตาแห่งความสุข) ขอบคุณนะคะพ่อ แม่ เรน... หนูขอบคุณจริงๆ ที่ทุกคนพยายามเข้าใจหนู จากนี้ไปหนูพร้อมจะก้าวไปข้างหน้าพร้อมกับทุกคนแล้วค่ะ!", "Nia", "good1", -15f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("(รวบตัวเนียและเรนเข้ามากอดแน่น) แม่รักเนียที่สุดเลยนะลูก... เราจะดูแลกันและกันนะ", "Mom", "idle", 0f, 4, SpriteAction.Jump)
                }),
                CreateChoice("(โผเข้ากอดพ่อกับแม่แน่น) หนูขอโทษเหมือนกันนะคะที่เคยปิดกั้นตัวเอง... หนูดีใจมากๆ ที่ได้เกิดมาเป็นลูกของพ่อกับแม่ค่ะ", "Nia", "good1", -10f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("(ลูบหัวเนียเบาๆ ด้วยความรัก) ไม่ต้องขอโทษแล้วลูก... บ้านเรากลับมารวมกันเป็นหนึ่งเดียวแล้วนะ", "Dad", "", 0f, 5, SpriteAction.None)
                }),
                CreateChoice("(ยิ้มและจับมือทุกคนไว้) วันนี้เป็นวันที่หนูมีความสุขที่สุดเลยค่ะ! เรามากินข้าวเช้าด้วยกันนะคะ!", "Nia", "good1", -5f, 1, SpriteAction.None, new List<DialogueLine>
                {
                    CreateLine("ฮูเร่! งั้นวันนี้กินเสร็จ เราไปเที่ยวด้วยกันทั้งบ้านเลยนะคะพ่อ!", "Rain", "", 0f, 7, SpriteAction.Jump)
                })
            },
            conclusionStory = new List<DialogueLine>
            {
                CreateLine("(ฉันมองดูพ่อ แม่ และเรน ที่กำลังหัวเราะและพูดคุยกันอย่างมีความสุขที่โต๊ะอาหาร...)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(เสียงลบๆ ในหัวหายไปจนหมดสิ้น เหลือไว้เพียงความเชื่อมั่นในตัวเองและความอบอุ่นที่ห้อมล้อมหัวใจ)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(ชีวิตไม่ได้เพอร์เฟกต์ และอาจจะมีอุปสรรคเข้ามาอีกในวันข้างหน้า...)", "Nia", "good1", 0f, 1, SpriteAction.None),
                CreateLine("(แต่ในวันนี้ ฉันรู้แล้วว่า... ฉันมีคนที่พร้อมจะก้าวเดินไปด้วยกัน และฉันมีค่าพอที่จะมีความสุขกับทุกวันของชีวิต)", "Nia", "good1", 0f, 1, SpriteAction.None)
            }
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
