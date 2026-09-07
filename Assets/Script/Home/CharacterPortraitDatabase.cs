using UnityEngine;
using System.Collections.Generic;

public static class CharacterPortraitDatabase
{
    private static readonly Dictionary<string, Sprite> cachedPortraits = new Dictionary<string, Sprite>(System.StringComparer.OrdinalIgnoreCase);

    public static Sprite GetPortrait(string characterName, string portraitEmotion = "")
    {
        if (string.IsNullOrEmpty(characterName)) return null;

        string cleanName = characterName.Trim();
        if (cleanName.StartsWith("NPC_", System.StringComparison.OrdinalIgnoreCase))
        {
            cleanName = cleanName.Substring(4).Trim();
        }

        // 1. ตรวจสอบใน Cache ก่อน
        string cacheKeyWithEmotion = $"{cleanName}_{portraitEmotion}".ToLowerInvariant();
        if (cachedPortraits.TryGetValue(cacheKeyWithEmotion, out Sprite cachedSprite) && cachedSprite != null)
        {
            return cachedSprite;
        }

        string cacheKeyDefault = $"{cleanName}_default".ToLowerInvariant();
        if (string.IsNullOrEmpty(portraitEmotion) && cachedPortraits.TryGetValue(cacheKeyDefault, out Sprite defaultCached) && defaultCached != null)
        {
            return defaultCached;
        }

        // 2. ลองโหลดจาก Resources/Portraits/ ตามชื่อและ Emotion
        if (!string.IsNullOrEmpty(portraitEmotion))
        {
            Sprite emotionSprite = Resources.Load<Sprite>($"Portraits/{cleanName}_{portraitEmotion}");
            if (emotionSprite != null)
            {
                cachedPortraits[cacheKeyWithEmotion] = emotionSprite;
                return emotionSprite;
            }
        }

        // 3. ลองโหลดจาก Resources/Portraits/ แบบ idle
        Sprite idleSprite = Resources.Load<Sprite>($"Portraits/{cleanName}_idle");
        if (idleSprite != null)
        {
            cachedPortraits[cacheKeyWithEmotion] = idleSprite;
            cachedPortraits[cacheKeyDefault] = idleSprite;
            return idleSprite;
        }

        // 4. ลองโหลดจาก Resources/Portraits/ ตามชื่อเพียวๆ
        Sprite nameSprite = Resources.Load<Sprite>($"Portraits/{cleanName}");
        if (nameSprite != null)
        {
            cachedPortraits[cacheKeyWithEmotion] = nameSprite;
            cachedPortraits[cacheKeyDefault] = nameSprite;
            return nameSprite;
        }

        // 5. ค้นหาจาก NPCInteraction ที่อยู่ในฉากปัจจุบัน
        NPCInteraction[] allNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in allNPCs)
        {
            if (npc != null && npc.npcProfiles != null)
            {
                foreach (var profile in npc.npcProfiles)
                {
                    if (profile != null && profile.npcName.Equals(cleanName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(portraitEmotion))
                        {
                            foreach (var p in profile.portraits)
                            {
                                if (p != null && p.portraitName.Equals(portraitEmotion, System.StringComparison.OrdinalIgnoreCase) && p.sprite != null)
                                {
                                    cachedPortraits[cacheKeyWithEmotion] = p.sprite;
                                    return p.sprite;
                                }
                            }
                        }
                        if (profile.portraits != null && profile.portraits.Count > 0 && profile.portraits[0] != null && profile.portraits[0].sprite != null)
                        {
                            cachedPortraits[cacheKeyWithEmotion] = profile.portraits[0].sprite;
                            cachedPortraits[cacheKeyDefault] = profile.portraits[0].sprite;
                            return profile.portraits[0].sprite;
                        }
                    }
                }
            }
        }

        return null;
    }

    public static void ClearCache()
    {
        cachedPortraits.Clear();
    }
}
