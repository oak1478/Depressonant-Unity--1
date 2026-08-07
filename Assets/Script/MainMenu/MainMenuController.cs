using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("ลาก GameObject ของหน้าเมนูแรกสุดมาใส่ตรงนี้ (ที่มีปุ่ม START, OPTIONS, EXIT)")]
    public GameObject mainPanel;

    [Tooltip("ลาก GameObject ของหน้าสองมาใส่ตรงนี้ (ที่มีปุ่ม NEW GAME, LOAD GAME, BACK)")]
    public GameObject playOptionsPanel;

    [Tooltip("ลาก GameObject ของหน้าโหลดเกมมาใส่ตรงนี้ (ที่มีสล็อต 4 ช่อง และปุ่ม BACK)")]
    public GameObject slotSelectPanel;

    [Tooltip("ลาก GameObject ของหน้าตั้งค่าออปชั่นมาใส่ตรงนี้ (ที่มีแท็บ GRAPHIC, AUDIO, etc.)")]
    public GameObject settingsPanel;

    [Header("Save Slot Buttons (Load Game)")]
    [Tooltip("ลากปุ่ม Slot 1 ถึง Slot 4 มาใส่เรียงตามลำดับ")]
    public Button[] slotButtons = new Button[4];

    [Tooltip("ลาก TMP Text ของสล็อต 1 ถึง 4 ที่ใช้แสดงรายละเอียดมาใส่เรียงตามลำดับ")]
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[4];

    private SaveSystem saveSystem;

    void Awake()
    {
        // ⚡ [แก้ไข] สร้าง GameObject แยกเฉพาะสำหรับ SaveSystem เพื่อไม่ให้ MainMenu Canvas โดน DontDestroyOnLoad ติดไปด้วย
        GameObject saveGo = new GameObject("SaveSystem");
        saveSystem = saveGo.AddComponent<SaveSystem>();
    }

    void Start()
    {
        // เริ่มต้นแสดงผลหน้าแรกสุด และปิดหน้าอื่นๆ ทั้งหมด
        ShowMainMenu();
    }

    // --- ฟังก์ชันควบคุมการแสดงผลของหน้าจอ (Panel Triggers) ---

    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowPlayOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(true);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSlotSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // อัปเดตรายละเอียดและปุ่มของแต่ละสล็อต
        UpdateSlotDetails();
    }

    public void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (playOptionsPanel != null) playOptionsPanel.SetActive(false);
        if (slotSelectPanel != null) slotSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // --- ฟังก์ชันหลักเกี่ยวกับระบบเซฟและการเลือกสล็อต ---

    // ⚡ เริ่มเกมใหม่โดยอัตโนมัติ: ค้นหาสล็อตว่างหรือเขียนทับช่องเก่าที่สุด
    public void StartNewGame()
    {
        int targetSlot = 1;
        bool foundEmpty = false;

        // 1. ค้นหาสล็อตที่ยังไม่มีไฟล์เซฟอยู่จริง
        for (int i = 1; i <= 4; i++)
        {
            if (!saveSystem.HasSaveFile(i))
            {
                targetSlot = i;
                foundEmpty = true;
                break;
            }
        }

        // 2. หากสล็อตเต็มทุกช่อง ค้นหาช่องที่เขียนไว้นานที่สุด (Oldest Last Write Time) เพื่อบันทึกทับ
        if (!foundEmpty)
        {
            System.DateTime oldestTime = System.DateTime.MaxValue;
            for (int i = 1; i <= 4; i++)
            {
                string path = saveSystem.GetSaveFilePath(i);
                if (System.IO.File.Exists(path))
                {
                    System.DateTime writeTime = System.IO.File.GetLastWriteTime(path);
                    if (writeTime < oldestTime)
                    {
                        oldestTime = writeTime;
                        targetSlot = i;
                    }
                }
            }
        }

        Debug.Log($"[New Game Auto-Select] เริ่มเล่นใหม่ในสล็อต: {targetSlot}");

        // 3. รีเซ็ตสถานะระดับโลกเข้า GameManager
        if (GameManagerSetup.Instance != null)
        {
            GameManagerSetup.Instance.currentStress = 0f;
            GameManagerSetup.Instance.currentDay = 1;
            GameManagerSetup.Instance.consecutiveMaxStressDays = 0;
            GameManagerSetup.Instance.savedInventory.Clear();
            GameManagerSetup.Instance.pickedUpItemIDs.Clear();
            GameManagerSetup.Instance.activeSaveSlot = targetSlot;
            GameManagerSetup.Instance.playTime = 0f;
        }
        else
        {
            // บันทึกบัฟเฟอร์รอ GameManager โหลดในซีนใหม่
            SaveData initialData = new SaveData
            {
                currentDay = 1,
                currentStress = 0f,
                activeSaveSlot = targetSlot,
                playTime = 0f
            };
            SaveSystem.pendingLoadData = initialData;
        }

        // 4. บันทึกทับไฟล์เซฟตั้งต้นลงเครื่อง (ซีน index 2 คือ Bedroom_3D)
        saveSystem.SaveGameFromGlobal(targetSlot, Vector3.zero, 2);

        // 5. โหลดซีนห้องนอนเพื่อเริ่มเล่น
        SceneManager.LoadScene("Bedroom_3D");
    }

    // ⚡ ฟังก์ชันสำหรับอัปเดตตัวหนังสือแสดงข้อมูลเซฟในแต่ละสล็อต 1 - 4
    private void UpdateSlotDetails()
    {
        for (int i = 0; i < 4; i++)
        {
            int slotIdx = i + 1;
            bool hasSave = saveSystem.HasSaveFile(slotIdx);

            if (slotButtons[i] == null) continue;

            if (hasSave)
            {
                SaveData data = saveSystem.LoadGame(slotIdx);
                if (data != null)
                {
                    // จัดข้อความตามโครงสร้างในรูปภาพ:
                    // แถวแรก: ชื่อตัวละคร และชื่อสถานที่ (เช่น Nia       MY ROOM)
                    // แถวสอง: วันที่ และเวลาเล่นสะสม (เช่น DAY 3       Play Time 0:46:30)
                    string sceneName = GetSceneName(data.currentSceneIndex);
                    string formattedTime = FormatPlayTime(data.playTime);

                    if (slotTexts[i] != null)
                    {
                        // การเว้นช่องว่าง \t ช่วยดันข้อความให้ห่างกันซ้ายขวาตามภาพต้นฉบับ
                        slotTexts[i].text = $"Nia\t\t\t\t{sceneName}\nDAY {data.currentDay}\t\t\t\tPlay Time {formattedTime}";
                    }
                }
                slotButtons[i].interactable = true;
            }
            else
            {
                if (slotTexts[i] != null)
                {
                    slotTexts[i].text = $"SLOT {slotIdx}\nEMPTY";
                }
                slotButtons[i].interactable = false; // ไม่มีเซฟกดโหลดไม่ได้
            }
        }
    }

    // ⚡ ฟังก์ชันสำหรับกดโหลดไฟล์ตามสล็อต (เรียกผ่าน Unity Event หรือปุ่มสล็อต)
    public void LoadSlot(int slotIndex)
    {
        SaveData data = saveSystem.LoadGameToGlobal(slotIndex);
        if (data != null && data.currentSceneIndex > 0)
        {
            Debug.Log($"[Load Game] โหลดสล็อตที่: {slotIndex} สำเร็จ! กำลังเข้าซีนรหัส {data.currentSceneIndex}");
            SceneManager.LoadScene(data.currentSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("Bedroom_3D");
        }
    }

    // --- ฟังก์ชันแปลข้อมูลระดับเกม ---

    private string GetSceneName(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 1: return "HOME TOWN";
            case 2: return "MY ROOM";
            case 30: return "OUTSIDE";
            case 33: return "SCHOOL";
            default: return "MY ROOM";
        }
    }

    private string FormatPlayTime(float totalSeconds)
    {
        int hours = (int)(totalSeconds / 3600);
        int minutes = (int)((totalSeconds % 3600) / 60);
        int seconds = (int)(totalSeconds % 60);
        return $"{hours}:{minutes:D2}:{seconds:D2}";
    }

    public void ExitGame()
    {
        Debug.Log("ออกจากเกม!");
        Application.Quit();
    }
}