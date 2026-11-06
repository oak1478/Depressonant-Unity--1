using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Styles the existing buttons without replacing their UnityEvent bindings.
internal static class MainMenuPresentation
{
    private static readonly Color Ink = new Color32(17, 14, 24, 255);
    private static readonly Color Cream = new Color32(244, 239, 230, 255);
    private static readonly Color Muted = new Color32(175, 168, 186, 255);
    private static readonly Color Accent = new Color32(158, 142, 202, 255);

    public static void Apply(GameObject main, GameObject play, GameObject slots, Button[] slotButtons, TextMeshProUGUI[] slotTexts)
    {
        Sprite originalArt = main != null ? main.transform.Find("Image")?.GetComponent<Image>()?.sprite : null;
        if (originalArt == null) return;

        StyleNavigation(main, originalArt, "DEPRESSONANT", "A story told over seventeen days.", "01   /   MAIN MENU",
            new[] { "Btn_Start", "Btn_Option", "Btn_Exit" }, new[] { "Start", "Options", "Exit" });
        StyleNavigation(play, originalArt, "Play", "Begin again or return to your story.", "02   /   PLAY",
            new[] { "Btn_Newgame", "Btn_LoadGame", "Btn_Back" }, new[] { "New Game", "Load Game", "Back" });

        if (slots == null) return;
        Transform root = slots.transform;
        AddBackdrop(root, originalArt, true);
        var oldHeading = root.Find("Load Game");
        if (oldHeading != null) oldHeading.gameObject.SetActive(false);
        AddHeading(root, "03   /   SAVE SLOTS", "Load Game", "Choose a save to continue.");
        StyleNamedButton(root, "Back", "Back", 0.19f, false, 0.31f);

        for (int i = 0; i < Mathf.Min(4, slotButtons.Length); i++)
        {
            Button button = slotButtons[i];
            if (button == null) continue;

            float x = 0.48f + (i % 2) * 0.245f;
            float y = 0.30f + (1 - i / 2) * 0.215f;
            SetBounds((RectTransform)button.transform, x, y, x + 0.225f, y + 0.185f);
            StyleButton(button, false);
            TextMeshProUGUI details = i < slotTexts.Length ? slotTexts[i] : null;
            HideOriginalLabels(button);
            Label(button.transform, $"SLOT  {i + 1:00}", 30f, Accent, 0.08f, 0.63f, 0.94f, 0.91f);
            if (details != null)
            {
                details.transform.SetParent(button.transform, false);
                details.gameObject.SetActive(true);
                SetBounds(details.rectTransform, 0.08f, 0.10f, 0.94f, 0.58f);
                StyleLabel(details, 29f, Cream);
            }
        }
    }

    private static void StyleNavigation(GameObject panel, Sprite art, string title, string description,
        string section, string[] names, string[] labels)
    {
        if (panel == null) return;
        Transform root = panel.transform;
        AddBackdrop(root, art, false);
        AddHeading(root, section, title, description);
        for (int i = 0; i < names.Length; i++)
            StyleNamedButton(root, names[i], labels[i], 0.44f - 0.102f * i, i == 0, 0.40f);
    }

    private static void AddBackdrop(Transform root, Sprite art, bool savePage)
    {
        var old = root.Find("Image");
        if (old != null) old.gameObject.SetActive(false);

        RectTransform backdrop = Rect(root, "Ink backdrop", 0f, 0f, 1f, 1f);
        backdrop.SetAsFirstSibling();
        Fill(backdrop, Ink);
        var illustration = Rect(backdrop, "Original ink illustration", 0.51f, 0.075f, 0.96f, 0.925f);
        var image = illustration.gameObject.AddComponent<RawImage>();
        image.texture = art.texture;
        image.uvRect = new Rect(0.74f, 0.17f, 0.26f, 0.60f);
        image.color = savePage ? new Color(0.23f, 0.20f, 0.30f) : new Color(0.63f, 0.57f, 0.71f);
        image.raycastTarget = false;
        Fill(Rect(backdrop, "Artwork divider", 0.505f, 0.075f, 0.507f, 0.925f), Accent);
        Label(backdrop, "DEPRESSONANT   /   STORY MODE", 23f, Muted, 0.065f, 0.062f, 0.49f, 0.108f);
    }

    private static void AddHeading(Transform root, string section, string title, string description)
    {
        Label(root, section, 28f, Accent, 0.065f, 0.825f, 0.48f, 0.89f);
        Fill(Rect(root, "Section mark", 0.065f, 0.797f, 0.115f, 0.801f), Accent);
        var heading = Label(root, title, 93f, Cream, 0.063f, 0.650f, 0.485f, 0.784f);
        heading.fontStyle = FontStyles.Bold;
        Label(root, description, 34f, Muted, 0.066f, 0.552f, 0.475f, 0.639f);
    }

    private static void StyleNamedButton(Transform root, string name, string caption, float y, bool primary, float right)
    {
        var button = root.Find(name)?.GetComponent<Button>();
        if (button == null) return;
        SetBounds((RectTransform)button.transform, 0.065f, y, right, y + 0.079f);
        StyleButton(button, primary);
        HideOriginalLabels(button);
        Label(button.transform, caption, 39f, primary ? Ink : Cream, 0.07f, 0.06f, 0.88f, 0.94f);
        Label(button.transform, ">", 34f, primary ? Ink : Accent, 0.89f, 0.06f, 0.98f, 0.94f);
    }

    private static void StyleButton(Button button, bool primary)
    {
        if (!button.TryGetComponent(out Image image)) image = button.gameObject.AddComponent<Image>();
        image.enabled = true;
        image.sprite = null;
        image.type = Image.Type.Simple;
        image.color = Color.white;
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        button.colors = new ColorBlock
        {
            normalColor = primary ? Accent : new Color32(35, 31, 46, 255),
            highlightedColor = primary ? new Color32(190, 176, 227, 255) : new Color32(61, 53, 82, 255),
            selectedColor = primary ? new Color32(202, 191, 236, 255) : new Color32(75, 64, 102, 255),
            pressedColor = primary ? new Color32(142, 125, 190, 255) : new Color32(23, 19, 32, 255),
            disabledColor = new Color32(31, 28, 40, 255),
            colorMultiplier = 1f,
            fadeDuration = 0.12f
        };
    }

    private static void HideOriginalLabels(Button button)
    {
        foreach (var label in button.GetComponentsInChildren<TextMeshProUGUI>(true))
            label.gameObject.SetActive(false);
    }

    private static RectTransform Rect(Transform parent, string name, float left, float bottom, float right, float top)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetBounds(rect, left, bottom, right, top);
        return rect;
    }

    private static void SetBounds(RectTransform rect, float left, float bottom, float right, float top)
    {
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Fill(RectTransform rect, Color color)
    {
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private static TextMeshProUGUI Label(Transform parent, string content, float size, Color color,
        float left, float bottom, float right, float top)
    {
        var label = Rect(parent, content, left, bottom, right, top).gameObject.AddComponent<TextMeshProUGUI>();
        label.text = content;
        StyleLabel(label, size, color);
        return label;
    }

    private static void StyleLabel(TextMeshProUGUI label, float size, Color color)
    {
        if (TMP_Settings.defaultFontAsset != null) label.font = TMP_Settings.defaultFontAsset;
        label.fontStyle = FontStyles.Normal;
        label.enableAutoSizing = true;
        label.fontSizeMin = 14f;
        label.fontSizeMax = size;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.margin = Vector4.zero;
        label.color = color;
        label.raycastTarget = false;
    }
}
