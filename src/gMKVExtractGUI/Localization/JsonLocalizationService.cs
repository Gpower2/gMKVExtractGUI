using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using gMKVToolNix.Log;
using Newtonsoft.Json;

namespace gMKVToolNix.Localization
{
    public partial class JsonLocalizationService : ILocalizationService
    {
        // This is the new "flattened" dictionary for runtime use
        // Key: cultureName (e.g., "de-DE"), Value: Dictionary<string, string>
        private readonly Dictionary<string, Dictionary<string, string>> _runtimeCache;

        // The master culture (English)
        private const string FallbackCulture = "en"; // Or "en-US"

        public JsonLocalizationService(string translationFolder)
        {
            _runtimeCache = new Dictionary<string, Dictionary<string, string>>(System.StringComparer.OrdinalIgnoreCase);
            SeedEnglishDefaults();
            LoadAllTranslations(translationFolder);
        }

        private void SeedEnglishDefaults()
        {
            _runtimeCache[FallbackCulture] = CreateEnglishDefaultsDictionary();
        }

        private static Dictionary<string, string> CreateEnglishDefaultsDictionary()
        {
            return new Dictionary<string, string>(_englishDefaults, System.StringComparer.OrdinalIgnoreCase);
        }

        private void LoadAllTranslations(string translationFolder)
        {
            if (!Directory.Exists(translationFolder))
            {
                gMKVLogger.Log(string.Format("Translation folder not found: {0}. Using embedded English defaults only.", translationFolder));
                return;
            }

            IEnumerable<string> files = TranslationPathService.EnumerateTranslationFiles(translationFolder);

            foreach (string file in files)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    var translationFile = JsonConvert.DeserializeObject<TranslationFile>(jsonContent);

                    if (translationFile == null || translationFile.Metadata == null || translationFile.Entries == null)
                    {
                        continue;
                    }

                    string culture = translationFile.Metadata.Culture;
                    if (string.IsNullOrWhiteSpace(culture))
                    {
                        // Log: "File {file} has no culture in metadata."
                        continue;
                    }

                    // --- This is the key change ---
                    // Flatten the complex object into a simple runtime dictionary
                    var flatDictionary = string.Equals(culture, FallbackCulture, System.StringComparison.OrdinalIgnoreCase)
                        ? CreateEnglishDefaultsDictionary()
                        : new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                    foreach (var entry in translationFile.Entries)
                    {
                        if (entry.Value == null)
                        {
                            continue;
                        }

                        // Use the translated value. If it's null/empty,
                        // fall back to the source text for that entry.
                        flatDictionary[entry.Key] =
                            !string.IsNullOrWhiteSpace(entry.Value.Translation)
                            ? entry.Value.Translation
                            : entry.Value.Source;
                    }

                    _runtimeCache[culture] = flatDictionary;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load {file}: {ex.Message}");
                    gMKVLogger.Log($"Failed to load {file}: {ex.Message}");
                }
            }
        }

        public List<string> GetAvailableCultures()
        {
            return _runtimeCache.Keys
                .OrderBy(culture => culture, System.StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public string ResolveCultureName(string cultureName)
        {
            string targetCulture = string.IsNullOrWhiteSpace(cultureName)
                ? FallbackCulture
                : cultureName.Trim();

            if (TryResolveCultureName(targetCulture, out string resolvedCulture))
            {
                return resolvedCulture;
            }

            if (targetCulture.Contains("-"))
            {
                string neutralCulture = targetCulture.Split('-')[0];
                if (TryResolveCultureName(neutralCulture, out resolvedCulture))
                {
                    return resolvedCulture;
                }
            }

            return FallbackCulture;
        }

        public string GetStringForCulture(string key, string cultureName)
        {
            cultureName = ResolveCultureName(cultureName);

            // 1. Try to find in the requested culture (e.g., "de-DE")
            if (_runtimeCache.TryGetValue(cultureName, out var cultureDict))
            {
                if (cultureDict.TryGetValue(key, out var value))
                {
                    return value; // Found!
                }
            }

            // 2. If not found, try to find in the neutral part (e.g., "de")
            if (cultureName.Contains("-"))
            {
                string neutralCulture = cultureName.Split('-')[0];
                if (_runtimeCache.TryGetValue(neutralCulture, out var neutralDict))
                {
                    if (neutralDict.TryGetValue(key, out var value))
                    {
                        return value; // Found neutral!
                    }
                }
            }

            // 3. If still not found, fall back to the master English dictionary
            if (_runtimeCache.TryGetValue(FallbackCulture, out var fallbackDict))
            {
                if (fallbackDict.TryGetValue(key, out var value))
                {
                    return value; // Found fallback!
                }
            }

            // 4. All failed.
            return $"!{key}!";
        }

        public string GetStringForCulture(string key, string cultureName, params object[] formatArgs)
        {
            string formatString = GetStringForCulture(key, cultureName);
            try
            {
                return string.Format(formatString, formatArgs);
            }
            catch (FormatException ex)
            {
                string arguments = formatArgs == null
                    ? "<null>"
                    : string.Join(", ", formatArgs.Select(arg => arg == null ? "<null>" : arg.ToString()).ToArray());

                gMKVLogger.Log(string.Format(
                    "Localization format error for key '{0}' in culture '{1}'. Format: '{2}'. Args: [{3}]. {4}",
                    key,
                    cultureName,
                    formatString,
                    arguments,
                    ex.Message));
                return $"!BadFormat:{key}!";
            }
        }

        string ILocalizationService.GetString(string key, string cultureName)
        {
            return GetStringForCulture(key, cultureName);
        }

        string ILocalizationService.GetString(string key, string cultureName, params object[] formatArgs)
        {
            return GetStringForCulture(key, cultureName, formatArgs);
        }

        private bool TryResolveCultureName(string cultureName, out string resolvedCulture)
        {
            resolvedCulture = _runtimeCache.Keys.FirstOrDefault(
                availableCulture => string.Equals(availableCulture, cultureName, System.StringComparison.OrdinalIgnoreCase));

            return resolvedCulture != null;
        }
    }
}
