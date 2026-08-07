using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class KeycapUIBinding
{
    public string keyName; // เช่น "W", "A", "S", "D", "F", "E", "TAB", "ESC" หรือ "Key_W", "Key_Tab"
    public Image keycapImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;
}

public class TutorialUI : MonoBehaviour
{
    [Header("Main UI Container")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;

    [Header("Progress Fill Bar (ขีดหลอดโหลดตามเวลา)")]
    public Image progressBarFill; // หลอด UI Image (ต้องตั้งค่า Image Type = Filled)

    [Header("Keycap Bindings (ลากไอคอนปุ่มปุ่มที่นี่)")]
    public KeycapUIBinding[] keycapBindings;

    [Header("Auto Animation Settings")]
    [Tooltip("ระยะเวลาสลับภาพอนิเมชันปุ่มบุ๋ม/นูน (วินาที) ตั้งเป็น 0.5 วินาทีตามต้องการ")]
    public float blinkInterval = 0.5f;

    void Awake()
    {
        AutoSetupReferences();
        DisableRaycastTargets();
    }

    void Start()
    {
        AutoSetupReferences();
        DisableRaycastTargets();
        SetProgress(0f);
    }

    void Update()
    {
        // ⚡ [อนิเมชันปุ่มขยับเอง] สลับภาพปุ่มปกติ <-> ปุ่มโดนกด สลับกันไปมาทุกๆ 0.5 วินาทีอัตโนมัติ
        if (keycapBindings == null) return;

        bool showPressedState = (Mathf.FloorToInt(Time.unscaledTime / blinkInterval) % 2) == 1;

        foreach (var binding in keycapBindings)
        {
            if (binding.keycapImage == null || !binding.keycapImage.gameObject.activeSelf) continue;

            if (showPressedState && binding.pressedSprite != null)
            {
                binding.keycapImage.sprite = binding.pressedSprite;
            }
            else if (!showPressedState && binding.normalSprite != null)
            {
                binding.keycapImage.sprite = binding.normalSprite;
            }
        }
    }

    // ⚡ [ปลดล็อกเมาส์] ปิด raycastTarget ขององค์ประกอบ UI บน Tutorial ทั้งหมดเพื่อไม่ให้บังเมาส์คลิกกระเป๋า/ช้อยส์
    public void DisableRaycastTargets()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                raycaster.enabled = false; // ปิด GraphicRaycaster ถาวรไม่ให้บังเลเซอร์เมาส์
            }
        }

        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            if (g != null)
            {
                g.raycastTarget = false;
            }
        }
    }

    public void AutoSetupReferences()
    {
        if (tutorialPanel == null) tutorialPanel = gameObject;

        // ⚡ 1. ออโต้ค้นหาตัวหนังสือคำสอน หากยังไม่ได้ลากวางใน Inspector
        if (instructionText == null)
        {
            instructionText = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        // ⚡ 2. ออโต้ค้นหาหลอดขีด Progress Fill หากยังไม่ได้ลากวางหรือลากใส่วัตถุผิดตัวใน Inspector
        if (progressBarFill == null || progressBarFill.gameObject.name.Contains("Background"))
        {
            Transform bg = transform.Find("ProgressBarBackground");
            if (bg != null)
            {
                Image childFill = bg.GetComponentInChildren<Image>(true);
                if (childFill != null && childFill.gameObject != bg.gameObject)
                {
                    progressBarFill = childFill;
                }
            }
            if (progressBarFill == null || progressBarFill.gameObject.name.Contains("Background"))
            {
                Image[] imgs = GetComponentsInChildren<Image>(true);
                foreach (var img in imgs)
                {
                    if (img.gameObject.name.Equals("ProgressBarFill", System.StringComparison.OrdinalIgnoreCase))
                    {
                        progressBarFill = img;
                        break;
                    }
                }
            }
        }

        // ⚡ 3. ออโต้ค้นหาไอคอนปุ่มคีย์บอร์ดทั้งหมดใต้ KeycapContainer หากยังไม่ได้ลากวางใน Inspector
        if (keycapBindings == null || keycapBindings.Length == 0)
        {
            Transform container = transform.Find("KeycapContainer");
            if (container == null) container = transform.Find("KeyCapContainer");
            
            if (container != null)
            {
                Image[] childImages = container.GetComponentsInChildren<Image>(true);
                List<KeycapUIBinding> autoList = new List<KeycapUIBinding>();
                
                foreach (Image img in childImages)
                {
                    if (img.gameObject == container.gameObject) continue;
                    
                    KeycapUIBinding binding = new KeycapUIBinding();
                    binding.keyName = img.gameObject.name;
                    binding.keycapImage = img;
                    binding.normalSprite = img.sprite;
                    autoList.Add(binding);
                }
                
                keycapBindings = autoList.ToArray();
            }
        }

        CheckAndApplyThaiFont();
    }

    private void CheckAndApplyThaiFont()
    {
        if (instructionText != null && (instructionText.font == null || instructionText.font.name.Contains("LiberationSans")))
        {
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            foreach (var f in fonts)
            {
                if (f != null && (f.name.Contains("Kanit") || f.name.Contains("Thai") || f.name.Contains("Sarabun") || f.name.Contains("Prompt")))
                {
                    instructionText.font = f;
                    break;
                }
            }
        }
    }

    private string CleanKeyName(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\"", "").Replace("'", "").Replace("Key_", "").Replace("key_", "").Trim().ToUpper();
    }

    public void ShowTutorial(string message, string[] activeKeys)
    {
        AutoSetupReferences();
        DisableRaycastTargets();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
        if (instructionText != null)
        {
            CheckAndApplyThaiFont();
            instructionText.text = message;
        }

        // ซ่อน/โชว์ปุ่มคีย์บอร์ดที่เกี่ยวข้องในสเต็ปนี้
        if (keycapBindings != null)
        {
            foreach (var binding in keycapBindings)
            {
                if (binding.keycapImage == null) continue;

                string cleanedBindingName = CleanKeyName(binding.keyName);
                if (string.IsNullOrEmpty(cleanedBindingName))
                {
                    cleanedBindingName = CleanKeyName(binding.keycapImage.gameObject.name);
                }

                bool isKeyActive = false;
                if (activeKeys != null)
                {
                    foreach (string key in activeKeys)
                    {
                        string cleanedTargetKey = CleanKeyName(key);
                        if (cleanedBindingName.Equals(cleanedTargetKey, System.StringComparison.OrdinalIgnoreCase))
                        {
                            isKeyActive = true;
                            break;
                        }
                    }
                }

                binding.keycapImage.gameObject.SetActive(isKeyActive);
                if (isKeyActive && binding.normalSprite != null)
                {
                    binding.keycapImage.sprite = binding.normalSprite;
                }
            }
        }

        SetProgress(0f);
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        SetProgress(0f);
    }

    // อัปเดตขีดหลอด Progress (0.0 ถึง 1.0)
    public void SetProgress(float progressRatio)
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = Mathf.Clamp01(progressRatio);
        }
    }
}
