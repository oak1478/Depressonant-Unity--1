using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    [Tooltip("Dialogue message (This string will display as the Element name in the Inspector list)")]
    public string text;

    [Tooltip("Speaker Name (e.g. Nia, Mom) - should match the name in NPC Profiles")]
    public string speakerName;

    [Tooltip("Name of the portrait emotion (e.g. smile, angry) matching the NPC Profile")]
    public string portraitName;

    [Tooltip("Stress change value when this line is read (+ to increase / - to decrease)")]
    public float stressChange = 0f;

    [Tooltip("Other Name to display instead (e.g., ??? or Stranger) - leaves blank to use speakerName")]
    public string otherName;

    [Tooltip("Active portrait slot index: 0 = Auto-Snap, 1 to 7 (ordered left to right)")]
    public int activeSlotIndex = 0;

    [Tooltip("Visual action/motion effect for this character portrait")]
    public SpriteAction motionEffect = SpriteAction.None;

    [Tooltip("Voice sound name to play when this line starts")]
    public string voiceSoundName;

    // ⚡ สไปรต์สำหรับใช้แสดงผลที่ประมวลผลเสร็จแล้วในช่วงรันไทม์ (ไม่เซฟลงโปรเจกต์)
    [System.NonSerialized] public Sprite resolvedPortraitSprite;
}

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("Button text for this dialogue option")]
    public string choiceButtonText; 
    
    [Tooltip("Stress change value when this choice is selected (+ to increase / - to decrease)")]
    public float stressChange = 0f;

    [Tooltip("Speaker Name who reacts or speaks this choice (e.g. Nia, Mom) - should match the name in NPC Profiles")]
    public string speakerName;

    [Tooltip("Name of the portrait emotion for this choice (e.g. smile, angry) matching the NPC Profile")]
    public string portraitName;

    [Tooltip("Other Name to display instead (e.g., ??? or Stranger) - leaves blank to use speakerName")]
    public string otherName;

    [Tooltip("Active portrait slot index for this choice: 0 = Auto-Snap, 1 to 7 (ordered left to right)")]
    public int activeSlotIndex = 0;

    [Tooltip("Visual action/motion effect for this choice portrait")]
    public SpriteAction motionEffect = SpriteAction.None;

    [Tooltip("Voice sound name to play when this choice is selected")]
    public string voiceSoundName;

    [Tooltip("Next dialogue lines to play after selecting this choice")]
    public List<DialogueLine> nextDialogueLines;

    // ⚡ สไปรต์สำหรับใช้แสดงผลที่ประมวลผลเสร็จแล้วในช่วงรันไทม์ (ไม่เซฟลงโปรเจกต์)
    [System.NonSerialized] public Sprite resolvedPortraitSprite;
}

public class DialogueManager : MonoBehaviour
{
    [Header("🖥️ UI References")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Image leftPortrait;
    public Image rightPortrait;
    public GameObject choicePanel;
    public TextMeshProUGUI[] choiceButtonsText;

    [Header("🎭 UI Multi-Portraits Slot (Left=1 to Right=7)")]
    [Tooltip("Drag 7 Image objects in order from left to right. Position 4 is the center.")]
    public Image[] characterSlots;

    [Header("⚙️ Settings")]
    public float typingSpeed = 0.03f;
    public string playerName = "Sensei";

    [Header("🎵 Audio Settings")]
    public AudioSource audioSource;
    
    [Tooltip("ลากไฟล์เสียงพิมพ์ดีดติ๊กๆ มาใส่ตรงนี้")]
    public AudioClip typingSound;
    
    [Tooltip("ความถี่ของเสียงพิมพ์ (เช่น 3 = เล่นเสียงทุกๆ 3 ตัวอักษร)")]
    [Range(1, 10)]
    public int typingSoundFrequency = 3;
    
    [Range(0f, 1f)] public float typingVolume = 0.3f;
    [Range(0f, 1f)] public float voiceVolume = 1f;
    
    [Tooltip("โยนไฟล์เสียงพากย์ทั้งหมดที่จะใช้ในเกมลงในช่องนี้")]
    public List<AudioClip> voiceClips = new List<AudioClip>();

    private string currentNPCName;
    private List<DialogueLine> currentActiveStory = new List<DialogueLine>();
    private List<DialogueChoice> currentChoicesData = new List<DialogueChoice>();
    private List<DialogueLine> conclusionStoryData = new List<DialogueLine>();
    
    private int dialogueStep = 0;
    private bool isWaitingForChoice = false;
    private bool isTyping = false;
    private string targetText = "";
    
    private enum StoryPhase { Intro, Branch, Conclusion }
    private StoryPhase currentPhase = StoryPhase.Intro;

    private Image[] activeSlots;
    private Sprite currentFallbackPlayerImg;
    private Sprite currentFallbackNpcImg;

    // ⚡ ตัวเก็บสถานะเพื่อจดจำว่าใครอยู่ที่สล็อตไหน ป้องกันตัวละครเดียวกันแสดงซ้ำซ้อนหลายตำแหน่ง
    private string[] slotSpeakers;

    void Start() { if (dialogueCanvas != null) dialogueCanvas.SetActive(false); }

    void Update()
    {
        if (dialogueCanvas != null && dialogueCanvas.activeSelf && !isWaitingForChoice)
        {
            bool advancePressed = false;
            if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)) advancePressed = true;
            if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) advancePressed = true;

            if (advancePressed)
            {
                if (isTyping) { StopAllCoroutines(); bodyText.text = targetText; isTyping = false; }
                else { AdvanceDialogue(); }
            }
        }
    }

    public void StartCustomDialogue(string npcName, Sprite playerImg, Sprite npcImg, List<DialogueLine> intro, List<DialogueChoice> choices, List<DialogueLine> conclusion)
    {
        currentNPCName = npcName;
        currentActiveStory = new List<DialogueLine>(intro);
        currentChoicesData = choices;
        conclusionStoryData = conclusion;
        
        dialogueStep = 0;
        isWaitingForChoice = false;
        currentPhase = StoryPhase.Intro;

        SetupSlots();

        currentFallbackPlayerImg = playerImg;
        currentFallbackNpcImg = npcImg;

        ResetPortraits();
        
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialogueCanvas != null) dialogueCanvas.SetActive(true);

        Time.timeScale = 0f; 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowNextLine();
    }

    void SetupSlots()
    {
        if (characterSlots != null && characterSlots.Length > 0)
        {
            activeSlots = characterSlots;
        }
        else
        {
            List<Image> fallbackList = new List<Image>();
            if (leftPortrait != null) fallbackList.Add(leftPortrait);
            if (rightPortrait != null) fallbackList.Add(rightPortrait);
            activeSlots = fallbackList.ToArray();
        }

        foreach (Image slot in activeSlots)
        {
            if (slot != null && slot.GetComponent<PortraitEffector>() == null)
            {
                slot.gameObject.AddComponent<PortraitEffector>();
            }
        }

        // รีเซ็ตขนาดตัวเก็บรายชื่อผู้พูดประจำสล็อต
        slotSpeakers = new string[activeSlots.Length];
    }

    void ResetPortraits()
    {
        if (activeSlots == null) return;
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] != null)
            {
                activeSlots[i].gameObject.SetActive(false);
            }
        }
        slotSpeakers = new string[activeSlots.Length];
    }

    void ShowNextLine()
    {
        if (dialogueStep >= currentActiveStory.Count) return;

        DialogueLine currentLine = currentActiveStory[dialogueStep];
        targetText = currentLine.text;

        ApplyStress(currentLine.stressChange);

        // 1. กำหนดชื่อผู้พูด
        string currentSpeaker = currentLine.speakerName;
        if (string.IsNullOrEmpty(currentSpeaker)) currentSpeaker = currentNPCName;

        if (!string.IsNullOrEmpty(currentLine.otherName))
        {
            nameText.text = currentLine.otherName;
        }
        else
        {
            bool isPlayer = currentSpeaker.Equals(playerName, System.StringComparison.OrdinalIgnoreCase) || 
                             currentSpeaker.Equals("Nia", System.StringComparison.OrdinalIgnoreCase);
            nameText.text = isPlayer ? playerName : currentSpeaker;
        }

        // 2. หาสล็อตพิกัดการขึ้นรูปตัวละคร (ตำแหน่ง 1-7 แปลงเป็น Index 0-6, หากเป็น 0 หรือออกนอกขอบเขตให้ดีฟอลต์สล็อต 3)
        int targetSlotIndex = 3; // 4 (ตรงกลาง) เป็นค่าเริ่มต้น
        if (currentLine.activeSlotIndex >= 1 && currentLine.activeSlotIndex <= activeSlots.Length)
        {
            targetSlotIndex = currentLine.activeSlotIndex - 1;
        }
        else
        {
            // ค่าดีฟอลต์ย้อนกลับกรณีข้อมูลไม่อยู่ใน 1-7: Player ขึ้นช่อง 0 (ซ้ายสุด), คนอื่นขึ้นขวาสุด
            bool isPlayer = currentSpeaker.Equals(playerName, System.StringComparison.OrdinalIgnoreCase) || 
                            currentSpeaker.Equals("Nia", System.StringComparison.OrdinalIgnoreCase);
            targetSlotIndex = isPlayer ? 0 : (activeSlots.Length - 1);
        }

        // 3. หาสปรายต์หน้าตัวละคร
        Sprite displaySprite = currentLine.resolvedPortraitSprite;
        if (displaySprite == null)
        {
            bool isPlayer = currentSpeaker.Equals(playerName, System.StringComparison.OrdinalIgnoreCase) || 
                            currentSpeaker.Equals("Nia", System.StringComparison.OrdinalIgnoreCase);
            displaySprite = isPlayer ? currentFallbackPlayerImg : currentFallbackNpcImg;
        }

        // 4. แสดงรูป, ไฮไลท์คนพูดปัจจุบัน และทำเอฟเฟกต์ Dimming คนอื่น
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == null) continue;

            if (i == targetSlotIndex)
            {
                if (displaySprite != null)
                {
                    activeSlots[i].sprite = displaySprite;
                    activeSlots[i].gameObject.SetActive(true);
                }

                activeSlots[i].color = Color.white;
                activeSlots[i].transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                slotSpeakers[i] = currentSpeaker; // บันทึกว่าผู้พูดคนนี้จองสล็อตนี้แล้ว

                if (currentLine.motionEffect != SpriteAction.None)
                {
                    PortraitEffector effector = activeSlots[i].GetComponent<PortraitEffector>();
                    if (effector != null)
                    {
                        effector.PlayEffect(currentLine.motionEffect);
                    }
                }
            }
            else
            {
                // หากตัวละครเดียวกันเคยอยู่ในสล็อตอื่น ให้ทำการเคลียร์และปิดตัวเก่าทิ้งทันที เพื่อแก้ปัญหารูปซ้ำซ้อน
                if (!string.IsNullOrEmpty(currentSpeaker) && slotSpeakers[i] == currentSpeaker)
                {
                    activeSlots[i].gameObject.SetActive(false);
                    slotSpeakers[i] = "";
                }
                else
                {
                    activeSlots[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    activeSlots[i].transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                }
            }
        }

        PlayVoiceSound(currentLine.voiceSoundName);
        StartCoroutine(TypeText(targetText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        bodyText.text = "";
        int charCount = 0;
        foreach (char letter in text.ToCharArray()) 
        { 
            bodyText.text += letter; 
            charCount++;
            
            // ⚡ เล่นเสียงพิมพ์ตามจังหวะที่กำหนด
            if (audioSource != null && typingSound != null && charCount % typingSoundFrequency == 0)
            {
                audioSource.PlayOneShot(typingSound, typingVolume);
            }
            
            yield return new WaitForSecondsRealtime(typingSpeed); 
        }
        isTyping = false;
    }

    void AdvanceDialogue()
    {
        dialogueStep++;
        if (dialogueStep < currentActiveStory.Count) { ShowNextLine(); }
        else { AdvanceFromPhase(); }
    }

    void AdvanceFromPhase()
    {
        if (currentPhase == StoryPhase.Intro) 
        {
            if (currentChoicesData != null && currentChoicesData.Count > 0) ShowChoices();
            else StartConclusion();
        }
        else if (currentPhase == StoryPhase.Branch) { StartConclusion(); }
        else { EndDialogue(); }
    }

    void ShowChoices()
    {
        isWaitingForChoice = true;
        if (choicePanel != null) choicePanel.SetActive(true);
        for (int i = 0; i < choiceButtonsText.Length; i++) 
        {
            if (i < currentChoicesData.Count && choiceButtonsText[i] != null) 
            {
                choiceButtonsText[i].text = currentChoicesData[i].choiceButtonText;
                choiceButtonsText[i].transform.parent.gameObject.SetActive(true);
            } 
            else if (choiceButtonsText[i] != null) 
            {
                choiceButtonsText[i].transform.parent.gameObject.SetActive(false);
            }
        }

        if (activeSlots != null)
        {
            for (int i = 0; i < activeSlots.Length; i++)
            {
                if (activeSlots[i] != null)
                {
                    activeSlots[i].color = new Color(0.4f, 0.4f, 0.4f, 1f);
                    activeSlots[i].transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                }
            }
        }

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public bool IsDialogueActive()
    {
        return dialogueCanvas != null && dialogueCanvas.activeSelf;
    }

    public void SelectChoice(int choiceIndex)
    {
        int dataIndex = choiceIndex - 1;
        if (dataIndex < 0 || dataIndex >= currentChoicesData.Count) return;

        DialogueChoice selected = currentChoicesData[dataIndex];
        
        ApplyStress(selected.stressChange);

        // อัปเดตการแสดงผลรูปตัวละครจากการกดเลือกช้อยส์ (ตำแหน่ง 1-7 แปลงเป็น Index 0-6)
        string choiceSpeaker = selected.speakerName;
        if (string.IsNullOrEmpty(choiceSpeaker)) choiceSpeaker = currentNPCName;

        if (!string.IsNullOrEmpty(choiceSpeaker))
        {
            // กำหนดชื่อคนพูดตอบสนอง
            if (!string.IsNullOrEmpty(selected.otherName))
            {
                nameText.text = selected.otherName;
            }
            else
            {
                bool isPlayer = choiceSpeaker.Equals(playerName, System.StringComparison.OrdinalIgnoreCase) || 
                                 choiceSpeaker.Equals("Nia", System.StringComparison.OrdinalIgnoreCase);
                nameText.text = isPlayer ? playerName : choiceSpeaker;
            }

            // เลือกสล็อตการขึ้นรูปตัวละคร
            int targetSlotIndex = 3; // 4 (ตรงกลาง) เป็นค่าเริ่มต้น
            if (selected.activeSlotIndex >= 1 && selected.activeSlotIndex <= activeSlots.Length)
            {
                targetSlotIndex = selected.activeSlotIndex - 1;
            }
            else
            {
                bool isPlayer = choiceSpeaker.Equals(playerName, System.StringComparison.OrdinalIgnoreCase) || 
                                 choiceSpeaker.Equals("Nia", System.StringComparison.OrdinalIgnoreCase);
                targetSlotIndex = isPlayer ? 0 : (activeSlots.Length - 1);
            }

            // โหลดสไปรต์ตัวละคร
            Sprite displaySprite = selected.resolvedPortraitSprite;

            // อัปเดตพอร์เทรตบนจอทั้งหมด
            for (int i = 0; i < activeSlots.Length; i++)
            {
                if (activeSlots[i] == null) continue;

                if (i == targetSlotIndex)
                {
                    if (displaySprite != null)
                    {
                        activeSlots[i].sprite = displaySprite;
                        activeSlots[i].gameObject.SetActive(true);
                    }
                    activeSlots[i].color = Color.white;
                    activeSlots[i].transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                    slotSpeakers[i] = choiceSpeaker; // บันทึกว่าผู้พูดคนนี้จองสล็อตนี้แล้ว

                    if (selected.motionEffect != SpriteAction.None)
                    {
                        PortraitEffector effector = activeSlots[i].GetComponent<PortraitEffector>();
                        if (effector != null)
                        {
                            effector.PlayEffect(selected.motionEffect);
                        }
                    }
                }
                else
                {
                    // ป้องกันรูปซ้ำจากการเลือกช้อยส์
                    if (!string.IsNullOrEmpty(choiceSpeaker) && slotSpeakers[i] == choiceSpeaker)
                    {
                        activeSlots[i].gameObject.SetActive(false);
                        slotSpeakers[i] = "";
                    }
                    else
                    {
                        activeSlots[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                        activeSlots[i].transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                    }
                }
            }
        }

        if (choicePanel != null) choicePanel.SetActive(false);
        isWaitingForChoice = false;

        // สร้างบรรทัดประโยคจำลองเพื่อแสดงข้อความที่เรากดเลือกช้อยส์ซ้ำขึ้นกล่องสนทนา
        DialogueLine choiceReplayLine = new DialogueLine();
        choiceReplayLine.text = selected.choiceButtonText;
        choiceReplayLine.speakerName = !string.IsNullOrEmpty(selected.speakerName) ? selected.speakerName : playerName;
        choiceReplayLine.portraitName = selected.portraitName;
        choiceReplayLine.otherName = selected.otherName;
        choiceReplayLine.activeSlotIndex = selected.activeSlotIndex;
        choiceReplayLine.motionEffect = selected.motionEffect;
        choiceReplayLine.resolvedPortraitSprite = selected.resolvedPortraitSprite;
        choiceReplayLine.stressChange = 0f; 
        choiceReplayLine.voiceSoundName = selected.voiceSoundName;

        List<DialogueLine> combinedStory = new List<DialogueLine>();
        combinedStory.Add(choiceReplayLine);

        if (selected.nextDialogueLines != null && selected.nextDialogueLines.Count > 0)
        {
            combinedStory.AddRange(selected.nextDialogueLines);
        }

        currentActiveStory = combinedStory;
        dialogueStep = 0;
        currentPhase = StoryPhase.Branch;
        ShowNextLine();
    }

    void StartConclusion()
    {
        currentPhase = StoryPhase.Conclusion;
        dialogueStep = 0;
        if (conclusionStoryData != null && conclusionStoryData.Count > 0)
        {
            currentActiveStory = new List<DialogueLine>(conclusionStoryData);
            ShowNextLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        currentFallbackPlayerImg = null;
        currentFallbackNpcImg = null;
        Time.timeScale = 1f; 

        bool is3DScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Bedroom_3D";
        Cursor.visible = !is3DScene;
        Cursor.lockState = is3DScene ? CursorLockMode.Locked : CursorLockMode.None;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnDialogueFinished();
        }
    }

    void ApplyStress(float amount)
    {
        if (amount == 0f) return;
        
        StressManager sm = Object.FindAnyObjectByType<StressManager>();
        if (sm != null)
        {
            sm.ChangeStress(amount);
        }
        else if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentStress = Mathf.Clamp(GameManagerSetup.Instance.currentStress + amount, 0f, 100f);
        }

        Debug.Log($"[Stress System] Calculated stress change: {amount}");
    }

    private void PlayVoiceSound(string clipName)
    {
        if (string.IsNullOrEmpty(clipName) || clipName == "None" || audioSource == null) return;
        
        foreach (var clip in voiceClips)
        {
            if (clip != null && clip.name == clipName)
            {
                audioSource.PlayOneShot(clip, voiceVolume);
                return;
            }
        }
    }
}