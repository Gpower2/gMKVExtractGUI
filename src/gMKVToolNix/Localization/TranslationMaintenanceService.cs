using System;
using System.Collections.Generic;

namespace gMKVToolNix.Localization
{
    public static class TranslationMaintenanceService
    {
        public static TranslationFile CreateTemplate(TranslationFile master, string culture, string translator = null)
        {
            if (master == null)
            {
                throw new ArgumentNullException(nameof(master));
            }

            if (string.IsNullOrWhiteSpace(culture))
            {
                throw new ArgumentException("A culture code is required.", nameof(culture));
            }

            var newFile = new TranslationFile
            {
                Metadata = new Metadata
                {
                    Culture = culture,
                    Translator = translator,
                    CreationDate = DateTime.UtcNow,
                    LastEditDate = DateTime.UtcNow
                }
            };

            foreach (var entry in master.Entries)
            {
                newFile.Entries[entry.Key] = new TranslationEntry
                {
                    Source = entry.Value.Source,
                    Translation = entry.Value.Source,
                    IsTranslated = false,
                    Notes = entry.Value.Notes
                };
            }

            return newFile;
        }

        public static TranslationSyncResult Synchronize(TranslationFile master, TranslationFile target)
        {
            if (master == null)
            {
                throw new ArgumentNullException(nameof(master));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var syncedEntries = new Dictionary<string, TranslationEntry>();
            int addedCount = 0;
            int updatedCount = 0;

            foreach (var masterEntry in master.Entries)
            {
                string key = masterEntry.Key;
                var masterValue = masterEntry.Value;

                if (target.Entries.TryGetValue(key, out var targetEntry))
                {
                    if (!string.Equals(targetEntry.Source, masterValue.Source, StringComparison.Ordinal))
                    {
                        targetEntry.Source = masterValue.Source;
                        targetEntry.Translation = masterValue.Source;
                        targetEntry.IsTranslated = false;
                        targetEntry.Notes = masterValue.Notes;
                        updatedCount++;
                    }
                    else
                    {
                        targetEntry.Notes = masterValue.Notes;
                    }

                    syncedEntries[key] = targetEntry;
                }
                else
                {
                    syncedEntries[key] = new TranslationEntry
                    {
                        Source = masterValue.Source,
                        Translation = masterValue.Source,
                        IsTranslated = false,
                        Notes = masterValue.Notes
                    };
                    addedCount++;
                }
            }

            int removedCount = 0;
            foreach (var targetEntry in target.Entries)
            {
                if (!master.Entries.ContainsKey(targetEntry.Key))
                {
                    removedCount++;
                }
            }

            target.Entries = syncedEntries;
            if (target.Metadata == null)
            {
                target.Metadata = new Metadata();
            }

            target.Metadata.LastEditDate = DateTime.UtcNow;

            return new TranslationSyncResult(target, addedCount, updatedCount, removedCount);
        }
    }
}
