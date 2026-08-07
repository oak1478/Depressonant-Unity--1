using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("ตั้งค่าแผงควบคุมเสียง")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    [Header("หน้าต่าง UI")]
    public GameObject settingsPanel;

    void Start()
    {
        // ตอนเริ่มเกม ให้โหลดค่าเสียงเดิมที่ผู้เล่นเคยตั้งไว้
        // ถ้าไม่เคยตั้ง ให้ใช้ค่า 0 (ดังสุด)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0f);
        
        // ⚡ [แก้ไขเพิ่มเติม] เช็กความปลอดภัยป้องกัน NullReferenceException
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        else
        {
            Debug.LogWarning("[SettingsController] กรุณาลาก UI Slider ของคุณมาใส่ในช่อง Volume Slider ในหน้าต่าง Inspector");
        }

        SetVolume(savedVolume);
        
        // ซ่อนหน้าต่างตอนเริ่ม
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกทุกครั้งที่เลื่อนหลอด Slider
    public void SetVolume(float volume)
    {
        // ⚡ [แก้ไขเพิ่มเติม] เช็กความปลอดภัยป้องกัน Null Mixer
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVolume", volume);
        }

        // บันทึกค่าลงเครื่อง (ใช้ PlayerPrefs เพราะเป็นแค่ Setting เล็กๆ)
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // เปิดหน้าต่าง Settings
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // ปิดหน้าต่าง Settings
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}