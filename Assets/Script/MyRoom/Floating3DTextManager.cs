using UnityEngine;
using TMPro;
using System.Collections;

public class Floating3DTextManager : MonoBehaviour
{
    private static Floating3DTextManager _instance;
    public static Floating3DTextManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<Floating3DTextManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[Floating3DTextManager]");
                    _instance = go.AddComponent<Floating3DTextManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Default Font Settings")]
    public TMP_FontAsset customThaiFont;
    public float defaultFontSize = 3.5f;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        AutoFindThaiFont();
    }

    private void AutoFindThaiFont()
    {
        if (customThaiFont == null)
        {
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            foreach (var f in fonts)
            {
                if (f != null && (f.name.Contains("Kanit") || f.name.Contains("Thai") || f.name.Contains("Sarabun") || f.name.Contains("Prompt")))
                {
                    customThaiFont = f;
                    Debug.Log($"[Floating3DTextManager] ออโต้ค้นพบฟอนต์ภาษาไทย {f.name} สำเร็จ!");
                    break;
                }
            }
        }
    }

    public void SpawnFloatingText(Vector3 worldPosition, string message, float duration = 3.5f, float fontSize = 3.5f)
    {
        AutoFindThaiFont();

        // 1. สร้าง 3D TextMeshPro GameObject
        GameObject textObj = new GameObject("Floating3DText_Instance");
        textObj.transform.position = worldPosition;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = ThaiTextAdjuster.Adjust(message);
        tmp.fontSize = fontSize > 0 ? fontSize : defaultFontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        if (customThaiFont != null)
        {
            tmp.font = customThaiFont;
        }

        // 2. รันอนิเมชันลอยและ Fade Out
        StartCoroutine(AnimateFloatingText(textObj, tmp, duration));
    }

    private IEnumerator AnimateFloatingText(GameObject textObj, TextMeshPro tmp, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = textObj.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 0.8f, 0); // ลอยขึ้นด้านบน 0.8 เมตร

        Camera mainCam = Camera.main;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;

            // ⚡ Billboard Effect: หันหน้าเข้าหากล้องเสมอ
            if (mainCam == null) mainCam = Camera.main;
            if (mainCam != null)
            {
                textObj.transform.rotation = Quaternion.LookRotation(textObj.transform.position - mainCam.transform.position);
            }

            // ⚡ เคลื่อนที่ลอยขึ้นนุ่มนวล (Smooth Step)
            textObj.transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, ratio));

            // ⚡ ค่อยๆ จางลงช่วง 1 วินาทีสุดท้าย
            if (ratio > 0.7f)
            {
                float fadeRatio = (1f - ratio) / 0.3f;
                Color c = tmp.color;
                c.a = Mathf.Clamp01(fadeRatio);
                tmp.color = c;
            }

            yield return null;
        }

        Destroy(textObj);
    }
}
