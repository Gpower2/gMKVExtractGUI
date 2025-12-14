using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            _runtimeCache = new Dictionary<string, Dictionary<string, string>>();
            LoadAllTranslations(translationFolder);
        }

        private void LoadAllTranslations(string translationFolder)
        {
            if (!Directory.Exists(translationFolder))
            {
                // Log: "Translation folder not found."
                return;
            }

            string[] files = Directory.GetFiles(translationFolder, "*.json");

            foreach (string file in files)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    var translationFile = JsonConvert.DeserializeObject<TranslationFile>(jsonContent);

                    if (translationFile == null || translationFile.Entries == null) continue;

                    string culture = translationFile.Metadata.Culture;
                    if (string.IsNullOrWhiteSpace(culture))
                    {
                        // Log: "File {file} has no culture in metadata."
                        continue;
                    }

                    // --- This is the key change ---
                    // Flatten the complex object into a simple runtime dictionary
                    var flatDictionary = new Dictionary<string, string>();
                    foreach (var entry in translationFile.Entries)
                    {
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

        public string GetString(string key, string cultureName)
        {
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

        public string GetString(string key, string cultureName, params object[] formatArgs)
        {
            string formatString = GetString(key, cultureName);
            try
            {
                return string.Format(formatString, formatArgs);
            }
            catch (FormatException)
            {
                return $"!BadFormat:{key}!";
            }
        }
    }
}
