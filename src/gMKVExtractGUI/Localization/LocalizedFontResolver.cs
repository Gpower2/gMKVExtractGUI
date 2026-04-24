using System;
using System.Collections.Generic;
using System.Drawing;

namespace gMKVToolNix.Localization
{
    public static class LocalizedFontResolver
    {
        private static readonly Dictionary<string, string[]> CultureFontFamilies = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "hi", new[] { "Nirmala UI", "Mangal", "Aparajita", "Arial Unicode MS", "Noto Sans Devanagari", "Lohit Devanagari", "Kohinoor Devanagari", "Devanagari MT", "Devanagari Sangam MN", "FreeSerif" } },
            { "ko", new[] { "Malgun Gothic", "Gulim", "Dotum", "Noto Sans CJK KR", "Noto Sans KR", "Apple SD Gothic Neo", "NanumGothic" } },
            { "ja", new[] { "Yu Gothic UI", "Meiryo", "MS UI Gothic", "Noto Sans CJK JP", "Noto Sans JP", "Hiragino Sans", "Hiragino Kaku Gothic ProN" } },
            { "zh-cn", new[] { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "SimSun", "Noto Sans CJK SC", "PingFang SC", "Heiti SC", "WenQuanYi Zen Hei" } },
            { "zh-tw", new[] { "Microsoft JhengHei UI", "Microsoft JhengHei", "PMingLiU", "Noto Sans CJK TC", "PingFang TC", "Heiti TC", "WenQuanYi Zen Hei" } }
        };

        public static Font ResolveFont(Font baseFont, string cultureName)
        {
            if (baseFont == null)
            {
                throw new ArgumentNullException("baseFont");
            }

            string[] candidateFamilies;
            if (!TryGetCandidateFamilies(cultureName, out candidateFamilies))
            {
                return baseFont;
            }

            foreach (string familyName in candidateFamilies)
            {
                FontStyle resolvedStyle = ResolveStyle(familyName, baseFont.Style);
                if (resolvedStyle == FontStyle.Regular && baseFont.Style != FontStyle.Regular && !IsStyleAvailable(familyName, FontStyle.Regular))
                {
                    continue;
                }

                try
                {
                    return new Font(
                        familyName,
                        baseFont.Size,
                        resolvedStyle,
                        baseFont.Unit);
                }
                catch (ArgumentException)
                {
                }
            }

            return baseFont;
        }

        private static bool TryGetCandidateFamilies(string cultureName, out string[] candidateFamilies)
        {
            foreach (string candidateCulture in TranslationPathService.GetCultureLookupChain(cultureName))
            {
                if (CultureFontFamilies.TryGetValue(candidateCulture, out candidateFamilies))
                {
                    return true;
                }
            }

            candidateFamilies = null;
            return false;
        }

        private static FontStyle ResolveStyle(string familyName, FontStyle preferredStyle)
        {
            if (IsStyleAvailable(familyName, preferredStyle))
            {
                return preferredStyle;
            }

            FontStyle[] fallbackStyles =
            {
                FontStyle.Regular,
                FontStyle.Bold,
                FontStyle.Italic,
                FontStyle.Underline,
                FontStyle.Strikeout
            };

            foreach (FontStyle fallbackStyle in fallbackStyles)
            {
                if (IsStyleAvailable(familyName, fallbackStyle))
                {
                    return fallbackStyle;
                }
            }

            return FontStyle.Regular;
        }

        private static bool IsStyleAvailable(string familyName, FontStyle style)
        {
            try
            {
                using (var family = new FontFamily(familyName))
                {
                    return family.IsStyleAvailable(style);
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
