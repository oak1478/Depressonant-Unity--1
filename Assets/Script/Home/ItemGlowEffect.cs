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
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private bool hasEmissionProp = false;
    private bool hasBaseColorProp = false;
    private bool hasColorProp = false;

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
            hasEmissionProp = instanceMaterial.HasProperty(EmissionColorID);
            hasBaseColorProp = instanceMaterial.HasProperty(BaseColorID);
            hasColorProp = instanceMaterial.HasProperty(ColorID);

            if (hasEmissionProp)
            {
                instanceMaterial.EnableKeyword("_EMISSION");
            }

            if (hasBaseColorProp)
            {
                originalColor = instanceMaterial.GetColor(BaseColorID);
            }
            else if (hasColorProp)
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
            if (hasEmissionProp)
            {
                instanceMaterial.SetColor(EmissionColorID, glowColor * currentIntensity);
            }
            else if (hasBaseColorProp)
            {
                Color lerpedColor = Color.Lerp(originalColor, glowColor, wave * 0.5f);
                instanceMaterial.SetColor(BaseColorID, lerpedColor);
            }
            else if (hasColorProp)
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

    void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
            instanceMaterial = null;
        }
    }

    void OnDisable()
    {
        // คืนค่าสีเดิมเมื่อปิดการทำงาน
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        if (instanceMaterial != null)
        {
            if (hasBaseColorProp)
            {
                instanceMaterial.SetColor(BaseColorID, originalColor);
            }
            else if (hasColorProp)
            {
                instanceMaterial.color = originalColor;
            }
        }
    }
}
