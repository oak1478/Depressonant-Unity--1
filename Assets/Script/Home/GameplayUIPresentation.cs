using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal static class GameplayUIPresentation
{
    public static readonly Color Cream = new Color32(244, 239, 230, 255);
    public static readonly Color Accent = new Color32(158, 142, 202, 255);
    private static readonly Color Ink = new Color32(17, 14, 24, 248);
    private static readonly Color Card = new Color32(35, 31, 46, 255);

    public static void StyleInventory(GameObject panel, Image[] slots)
    {
        if (panel == null || slots == null || slots.Length == 0 || slots[0] == null) return;
        Transform root = panel.transform;
        var oldArt = root.Find("Image")?.GetComponent<Image>();
        if (oldArt != null) oldArt.gameObject.SetActive(false);
        if (panel.TryGetComponent(out Image oldOverlay)) oldOverlay.enabled = false;

        var scrim = Rect(root, "Bag backdrop", 0f, 0f, 1f, 1f);
        scrim.SetAsFirstSibling();
        Fill(scrim, new Color(0.025f, 0.02f, 0.04f, 0.18f)).raycastTarget = true;
        var bag = Rect(scrim, "Bag surface", 0.105f, 0.085f, 0.895f, 0.915f);
        Fill(bag, Ink);
        Fill(Rect(bag, "Accent line", 0f, 0.989f, 1f, 1f), Accent);
        Label(bag, "DEPRESSONANT   /   INVENTORY", 24f, Accent, 0.045f, 0.843f, 0.80f, 0.91f);
        Label(bag, "Nia's Bag", 57f, Cream, 0.044f, 0.722f, 0.80f, 0.84f).fontStyle = FontStyles.Bold;
        var examine = Rect(bag, "Examine surface", 0.63f, 0.105f, 0.955f, 0.693f);
        Fill(examine, Card);
        Fill(Rect(examine, "Examine accent", 0f, 0.98f, 1f, 1f), Accent);
        Label(examine, "EXAMINE", 30f, Accent, 0.08f, 0.79f, 0.92f, 0.97f);
        Label(bag, "TAB   /   CLOSE BAG", 23f, Accent, 0.045f, 0.018f, 0.55f, 0.075f);

        if (slots[0].transform.parent is not RectTransform grid) return;
        if (grid.TryGetComponent(out GridLayoutGroup layout)) layout.enabled = false;
        SetBounds(grid, 0.14f, 0.172f, 0.585f, 0.66f);
        int rows = Mathf.CeilToInt(slots.Length / 3f);
        for (int i = 0; i < slots.Length; i++)
        {
            Image icon = slots[i];
            if (icon == null || icon.transform.parent != grid) continue;
            float left = (i % 3) / 3f;
            float top = 1f - (i / 3) / (float)rows;
            float right = left + 0.305f;
            float bottom = top - 1f / rows + 0.04f;
            var slot = Rect(grid, $"Slot {i + 1} card", left, bottom, right, top);
            slot.SetAsFirstSibling();
            Image surface = Fill(slot, Card);
            Label(slot, $"{i + 1:00}", 21f, Accent, 0.075f, 0.73f, 0.24f, 0.94f);
            SetBounds(icon.rectTransform, left + 0.071f, bottom + 0.034f, right - 0.025f, top - 0.065f);
            icon.preserveAspect = true;
            if (icon.TryGetComponent(out Button button))
            {
                surface.color = Color.white;
                button.targetGraphic = surface;
                button.transition = Selectable.Transition.ColorTint;
                button.colors = new ColorBlock
                {
                    normalColor = Card,
                    highlightedColor = new Color32(75, 64, 102, 255),
                    pressedColor = new Color32(54, 45, 72, 255),
                    selectedColor = new Color32(75, 64, 102, 255),
                    disabledColor = Card,
                    colorMultiplier = 1f,
                    fadeDuration = 0.12f
                };
            }
        }
    }

    public static void StyleQuest(GameObject root, GameObject title)
    {
        if (root == null || !root.TryGetComponent(out RectTransform panel)) return;
        panel.sizeDelta = new Vector2(510f, panel.sizeDelta.y);
        panel.anchoredPosition = new Vector2(32f, -30f);
        if (!root.TryGetComponent(out Image background)) background = root.AddComponent<Image>();
        background.sprite = null;
        background.type = Image.Type.Simple;
        background.color = Ink;
        background.raycastTarget = false;
        Fill(Rect(panel, "Quest accent", 0f, 0f, 0.010f, 1f), Accent);
        if (title != null && title.TryGetComponent(out TextMeshProUGUI label))
        {
            label.fontSize = 27f;
            label.color = Accent;
        }
    }

    public static void SetBounds(RectTransform rect, float left, float bottom, float right, float top)
    {
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static RectTransform Rect(Transform parent, string name, float left, float bottom, float right, float top)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetBounds(rect, left, bottom, right, top);
        return rect;
    }

    private static Image Fill(RectTransform rect, Color color)
    {
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI Label(Transform parent, string text, float maxSize, Color color,
        float left, float bottom, float right, float top)
    {
        var label = Rect(parent, text, left, bottom, right, top).gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.color = color;
        label.enableAutoSizing = true;
        label.fontSizeMin = 13f;
        label.fontSizeMax = maxSize;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.raycastTarget = false;
        return label;
    }
}
