using System.Collections.Generic;

[System.Serializable] // บรรทัดนี้สำคัญมาก! ทำให้ Unity แปลงคลาสนี้เป็น JSON ได้
public class SaveData
{
    // เก็บตำแหน่งตัวละคร
    public float playerX;
    public float playerY;
    public float playerZ;

    // เก็บของในกระเป๋า (แบบเก่า: เก็บเป็นชื่อข้อความ)
    public List<string> inventory = new List<string>();

    // [เพิ่มใหม่] เก็บของในกระเป๋าแบบละเอียด (ตำแหน่งสล็อต, ประเภทไอเทม, จำนวนชิ้น)
    public List<GlobalInventoryItem> inventoryItems = new List<GlobalInventoryItem>();

    // ลำดับฉาก (เช่น 1 = ฉากเล่นเกมหลัก)
    public int currentSceneIndex;

    // [เพิ่มใหม่] เก็บวันและค่าความเครียดสะสมข้ามฉาก
    public int currentDay;
    public float currentStress;
    public int consecutiveMaxStressDays; // [เพิ่มใหม่] จำนวนวันที่ความเครียดเต็ม 100% ติดต่อกัน

    // [เพิ่มใหม่] เก็บรหัสไอเทมที่ถูกเก็บไปแล้วในเกมนอน/ตื่น
    public List<string> pickedUpItemIDs = new List<string>();

    // [เพิ่มใหม่] เก็บรายชื่อ NPC ที่คุยจบแล้วในวันปัจจุบัน
    public List<string> talkedNPCsToday = new List<string>();

    // [เพิ่มใหม่] สล็อตเซฟเกมที่กำลังใช้งานอยู่ (1-4)
    public int activeSaveSlot;

    // [เพิ่มใหม่] เก็บเวลาเล่นรวมของเซฟนี้ (หน่วยเป็นวินาที)
    public float playTime;
}