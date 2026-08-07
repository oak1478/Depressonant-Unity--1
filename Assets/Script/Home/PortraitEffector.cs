using UnityEngine;
using System.Collections;

public enum SpriteAction { None, Jump, Shake, SlideIn, SlideOut }

public class PortraitEffector : MonoBehaviour
{
    private Vector3 originalPosition;
    private Coroutine activeCoroutine;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void PlayEffect(SpriteAction action)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            transform.localPosition = originalPosition;
        }

        switch (action)
        {
            case SpriteAction.Jump:
                activeCoroutine = StartCoroutine(JumpCoroutine());
                break;
            case SpriteAction.Shake:
                activeCoroutine = StartCoroutine(ShakeCoroutine());
                break;
            case SpriteAction.SlideIn:
                activeCoroutine = StartCoroutine(SlideInCoroutine());
                break;
            case SpriteAction.SlideOut:
                activeCoroutine = StartCoroutine(SlideOutCoroutine());
                break;
        }
    }

    private IEnumerator JumpCoroutine()
    {
        float duration = 0.15f;
        float height = 40f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float y = Mathf.Sin((elapsed / duration) * Mathf.PI) * height;
            transform.localPosition = originalPosition + new Vector3(0, y, 0);
            elapsed += Time.unscaledDeltaTime; // ใช้เวลาไม่ขึ้นกับ timescale ของเกม
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator ShakeCoroutine()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * 10f;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator SlideInCoroutine()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 startPos = originalPosition + new Vector3(300f, 0, 0);
        // หากสล็อตตั้งตำแหน่งทางซ้าย ให้สไลด์มาจากทางซ้ายแทน
        if (originalPosition.x < -100f) startPos = originalPosition - new Vector3(300f, 0, 0);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smoothstep easing
            transform.localPosition = Vector3.Lerp(startPos, originalPosition, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator SlideOutCoroutine()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 targetPos = originalPosition + new Vector3(300f, 0, 0);
        if (originalPosition.x < -100f) targetPos = originalPosition - new Vector3(300f, 0, 0);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.localPosition = Vector3.Lerp(originalPosition, targetPos, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        gameObject.SetActive(false); // ซ่อนตัวเองหลังสไลด์ออก
    }
}
