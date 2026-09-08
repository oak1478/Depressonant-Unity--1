using UnityEngine;

public class ItemGlowEffect : MonoBehaviour
{
    [Header("Glow Settings")]
    [Tooltip("ความเร็วในการกระพริบเรืองแสงของไอเทม")]
    public float pulseSpeed = 2.5f;

    [Tooltip("ระดับความสว่างต่ำสุด (Min Intensity/Alpha)")]
    public float minGlow = 0.3f;

    [Tooltip("ระดับความสว่างสูงสุด (Max Intensity/Alpha)")]
    public float maxGlow = 1.0f;

    [Tooltip("สีของการเรืองแสง (ค่าตั้งต้นคือสีทอง/เหลืองนวล)")]
    public Color glowColor = new Color(1f, 0.9f, 0.4f, 1f);

    private SpriteRenderer spriteRenderer;
    private Renderer meshRenderer;
    private Material instanceMaterial;
    private Color originalColor;
    private Light pointLight;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>() ?? GetComponentInChildren<MeshRenderer>();
        pointLight = GetComponentInChildren<Light>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else if (meshRenderer != null && meshRenderer.material != null)
        {
            instanceMaterial = meshRenderer.material; // สร้างอินสแตนซ์วัสดุแยกป้องกันแก้ไขไฟลต้นฉบับ
            if (instanceMaterial.HasProperty("_BaseColor"))
            {
                originalColor = instanceMaterial.GetColor("_BaseColor");
            }
            else if (instanceMaterial.HasProperty("_Color"))
            {
                originalColor = instanceMaterial.color;
            }
        }
    }

    void Update()
    {
        // คำนวณค่าการกระพริบนุ่มนวลตามคลื่น Sine Wave
        float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // ค่าอยู่ระหว่าง 0.0 - 1.0
        float currentIntensity = Mathf.Lerp(minGlow, maxGlow, wave);

        // 1. กรณีไอเทมเป็น Sprite 2D
        if (spriteRenderer != null)
        {
            Color lerpedColor = Color.Lerp(originalColor, glowColor, wave * 0.5f);
            lerpedColor.a = currentIntensity;
            spriteRenderer.color = lerpedColor;
        }

        // 2. กรณีไอเทมเป็น 3D Mesh (URP Emission/Color)
        if (instanceMaterial != null)
        {
            if (instanceMaterial.HasProperty("_EmissionColor"))
            {
                instanceMaterial.SetColor("_EmissionColor", glowColor * currentIntensity);
                instanceMaterial.EnableKeyword("_EMISSION");
            }
            else if (instanceMaterial.HasProperty("_BaseColor"))
            {
                Color lerpedColor = Color.Lerp(originalColor, glowColor, wave * 0.5f);
                instanceMaterial.SetColor("_BaseColor", lerpedColor);
            }
            else if (instanceMaterial.HasProperty("_Color"))
            {
                Color lerpedColor = Color.Lerp(originalColor, glowColor, wave * 0.5f);
                instanceMaterial.color = lerpedColor;
            }
        }

        // 3. กรณีไอเทมมี Point Light เล็กๆ ติดตัว
        if (pointLight != null)
        {
            pointLight.intensity = currentIntensity * 2f;
            pointLight.color = glowColor;
        }
    }

    void OnDisable()
    {
        // คืนค่าสีเดิมเมื่อปิดการทำงาน
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        if (instanceMaterial != null)
        {
            if (instanceMaterial.HasProperty("_BaseColor"))
            {
                instanceMaterial.SetColor("_BaseColor", originalColor);
            }
            else if (instanceMaterial.HasProperty("_Color"))
            {
                instanceMaterial.color = originalColor;
            }
        }
    }
}
