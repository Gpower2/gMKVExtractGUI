using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gMKVToolNix.Localization
{
    public static class TranslationPathService
    {
        public const string FilePrefix = "gmkvextract-";
        private const string FileExtension = ".json";
        private const string MasterCulture = "en";
        private static readonly Dictionary<string, string> CultureAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "cn", "zh-tw" },
            { "zh", "zh-cn" },
            { "zh-chs", "zh-cn" },
            { "zh-cn", "zh-cn" },
            { "zh-hans", "zh-cn" },
            { "zh-sg", "zh-cn" },
            { "zh-cht", "zh-tw" },
            { "zh-hant", "zh-tw" },
            { "zh-hk", "zh-tw" },
            { "zh-mo", "zh-tw" },
            { "zh-tw", "zh-tw" }
        };
        private static readonly Dictionary<string, string[]> LegacyCultureAliases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "zh-tw", new[] { "cn" } }
        };

        public static string GetTranslationFileName(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                throw new ArgumentException("A culture code is required.", nameof(culture));
            }

            return FilePrefix + culture.Trim() + FileExtension;
        }

        public static string GetTranslationFilePath(string directory, string culture)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("A translation directory is required.", nameof(directory));
            }

            return Path.Combine(directory, GetTranslationFileName(culture));
        }

        public static string GetExistingTranslationFilePath(string directory, string culture)
        {
            foreach (string candidateCulture in GetCultureLookupChain(culture))
            {
                string preferredPath = GetTranslationFilePath(directory, candidateCulture);
                if (File.Exists(preferredPath))
                {
                    return preferredPath;
                }

                string legacyPath = GetLegacyTranslationFilePath(directory, candidateCulture);
                if (File.Exists(legacyPath))
                {
                    return legacyPath;
                }
            }

            foreach (string legacyCulture in GetLegacyCultureAliases(culture))
            {
                string preferredPath = GetTranslationFilePath(directory, legacyCulture);
                if (File.Exists(preferredPath))
                {
                    return preferredPath;
                }

                string legacyPath = GetLegacyTranslationFilePath(directory, legacyCulture);
                if (File.Exists(legacyPath))
                {
                    return legacyPath;
                }
            }

            string fallbackCulture = GetCanonicalCultureCode(culture);
            if (string.IsNullOrWhiteSpace(fallbackCulture))
            {
                fallbackCulture = culture == null ? string.Empty : culture.Trim();
            }

            return GetTranslationFilePath(directory, fallbackCulture);
        }

        public static string GetMasterFilePath(string directory)
        {
            return GetExistingTranslationFilePath(directory, MasterCulture);
        }

        public static IEnumerable<string> EnumerateTranslationFiles(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return Enumerable.Empty<string>();
            }

            string[] preferredFiles = Directory.GetFiles(directory, FilePrefix + "*" + FileExtension, SearchOption.TopDirectoryOnly);
            if (preferredFiles.Length > 0)
            {
                return preferredFiles.OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
            }

            return Directory.GetFiles(directory, "*" + FileExtension, SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
        }

        public static bool TryGetCultureFromPath(string path, out string culture)
        {
            culture = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            if (fileName.StartsWith(FilePrefix, StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName.Substring(FilePrefix.Length);
            }

            culture = fileName.Trim();
            return !string.IsNullOrWhiteSpace(culture);
        }

        public static string NormalizeCultureCode(string culture)
        {
            return string.IsNullOrWhiteSpace(culture)
                ? string.Empty
                : culture.Trim().Replace('_', '-').ToLowerInvariant();
        }

        public static string GetCanonicalCultureCode(string culture)
        {
            string normalizedCulture = NormalizeCultureCode(culture);
            if (string.IsNullOrWhiteSpace(normalizedCulture))
            {
                return string.Empty;
            }

            string canonicalCulture;
            return CultureAliases.TryGetValue(normalizedCulture, out canonicalCulture)
                ? canonicalCulture
                : normalizedCulture;
        }

        public static IEnumerable<string> GetCultureLookupChain(string culture)
        {
            string normalizedCulture = NormalizeCultureCode(culture);
            if (string.IsNullOrWhiteSpace(normalizedCulture))
            {
                return Enumerable.Empty<string>();
            }

            var candidates = new List<string>();
            var seenCandidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string currentCulture = normalizedCulture;
            while (!string.IsNullOrWhiteSpace(currentCulture))
            {
                AddCultureCandidate(candidates, seenCandidates, currentCulture);
                AddCultureCandidate(candidates, seenCandidates, GetCanonicalCultureCode(currentCulture));

                int separatorIndex = currentCulture.LastIndexOf('-');
                if (separatorIndex < 0)
                {
                    break;
                }

                currentCulture = currentCulture.Substring(0, separatorIndex);
            }

            return candidates;
        }

        public static string ResolveAvailableCulture(IEnumerable<string> availableCultures, string requestedCulture)
        {
            if (availableCultures == null)
            {
                throw new ArgumentNullException(nameof(availableCultures));
            }

            string[] cultureList = availableCultures
                .Where(culture => !string.IsNullOrWhiteSpace(culture))
                .ToArray();

            foreach (string candidateCulture in GetCultureLookupChain(requestedCulture))
            {
                string match = cultureList.FirstOrDefault(
                    culture => string.Equals(culture, candidateCulture, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static string GetLegacyTranslationFilePath(string directory, string culture)
        {
            return Path.Combine(directory, culture.Trim() + FileExtension);
        }

        private static IEnumerable<string> GetLegacyCultureAliases(string culture)
        {
            string canonicalCulture = GetCanonicalCultureCode(culture);
            if (string.IsNullOrWhiteSpace(canonicalCulture))
            {
                return Enumerable.Empty<string>();
            }

            string[] aliases;
            return LegacyCultureAliases.TryGetValue(canonicalCulture, out aliases)
                ? aliases
                : Enumerable.Empty<string>();
        }

        private static void AddCultureCandidate(ICollection<string> candidates, ISet<string> seenCandidates, string culture)
        {
            if (!string.IsNullOrWhiteSpace(culture) && seenCandidates.Add(culture))
            {
                candidates.Add(culture);
            }
        }
    }
}
