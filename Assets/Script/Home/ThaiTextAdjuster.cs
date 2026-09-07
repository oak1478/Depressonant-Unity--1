using System.Collections.Generic;
using System.Text;

/// <summary>
/// Thai Text Adjuster for TextMeshPro
/// Fixes missing Thai tone marks and diacritics when stacked above upper vowels (Level 2).
/// In fonts like Kanit-Black without GPOS/mark-to-mark shaping or PUA glyphs,
/// the tone mark overlaps the upper vowel at Level 1 and becomes invisible.
/// This utility wraps tone marks that follow upper vowels in <voffset=0.25em>...</voffset>,
/// raising them cleanly above the upper vowel without affecting horizontal layout.
/// </summary>
public static class ThaiTextAdjuster
{
    // สระบน / เครื่องหมายชั้นบน (Level 1)
    private static readonly HashSet<char> UpperVowels = new HashSet<char>
    {
        '\u0E31', // ั (ไม้หันอากาศ)
        '\u0E34', // ิ (สระอิ)
        '\u0E35', // ี (สระอี)
        '\u0E36', // ึ (สระอึ)
        '\u0E37', // ื (สระอือ)
        '\u0E47', // ็ (ไม้ไต่คู้)
        '\u0E4D'  // ํ (นิคหิต)
    };

    // วรรณยุกต์และเครื่องหมายชั้นที่สอง (Level 2)
    private static readonly HashSet<char> ToneMarks = new HashSet<char>
    {
        '\u0E48', // ่ (ไม้เอก)
        '\u0E49', // ้ (ไม้โท)
        '\u0E4A', // ๊ (ไม้ตรี)
        '\u0E4B', // ๋ (ไม้จัตวา)
        '\u0E4C'  // ์ (ทัณฑฆาต / การันต์)
    };

    /// <summary>
    /// ปรับข้อความภาษาไทย โดยยกวรรณยุกต์ที่อยู่เหนือสระบนขึ้น 0.25em เพื่อให้แสดงผลได้ถูกต้อง
    /// </summary>
    public static string Adjust(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        StringBuilder sb = new StringBuilder(input.Length + 32);
        bool insideTag = false;
        char prevChar = '\0';

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // ข้ามการแปลงถ้าอยู่ใน rich text tag เช่น <b>, <color>, <size>, <voffset>
            if (c == '<')
            {
                insideTag = true;
                prevChar = '\0';
                sb.Append(c);
                continue;
            }
            if (insideTag)
            {
                if (c == '>') insideTag = false;
                sb.Append(c);
                continue;
            }

            // เมื่อพบวรรณยุกต์อยู่หลังสระบน ให้ยกขึ้นด้วย voffset
            if (ToneMarks.Contains(c) && UpperVowels.Contains(prevChar))
            {
                sb.Append("<voffset=0.25em>");
                sb.Append(c);
                sb.Append("</voffset>");
            }
            else
            {
                sb.Append(c);
            }

            prevChar = c;
        }

        return sb.ToString();
    }
}
