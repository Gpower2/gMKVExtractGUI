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
            string preferredPath = GetTranslationFilePath(directory, culture);
            if (File.Exists(preferredPath))
            {
                return preferredPath;
            }

            string legacyPath = GetLegacyTranslationFilePath(directory, culture);
            if (File.Exists(legacyPath))
            {
                return legacyPath;
            }

            return preferredPath;
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

        private static string GetLegacyTranslationFilePath(string directory, string culture)
        {
            return Path.Combine(directory, culture.Trim() + FileExtension);
        }
    }
}
