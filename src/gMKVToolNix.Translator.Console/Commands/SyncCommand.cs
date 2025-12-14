using System;
using System.Collections.Generic;
using System.Linq;
using gMKVToolNix.Localization;

namespace gMKVToolNix.Translator.Console.Commands
{
    public class SyncCommand
    {
        public static int Execute(SyncOptions opts)
        {
            System.Console.WriteLine($"Syncing {opts.TargetFile} with {opts.MasterFile}...");
            try
            {
                // Load both files
                var master = TranslationFileService.LoadFile(opts.MasterFile);
                var target = TranslationFileService.LoadFile(opts.TargetFile);
                if (master == null || target == null) return 1;

                var newEntries = new Dictionary<string, TranslationEntry>();
                int newStrings = 0;
                int updatedStrings = 0;
                int removedStrings = 0;

                // Loop 1: Iterate over the MASTER file to add/update entries
                foreach (var masterEntry in master.Entries)
                {
                    string key = masterEntry.Key;
                    var masterVal = masterEntry.Value;

                    if (target.Entries.TryGetValue(key, out var targetEntry))
                    {
                        // Case 1: Key exists. Check if source text changed.
                        if (targetEntry.Source != masterVal.Source)
                        {
                            // Source text changed! Invalidate old translation.
                            targetEntry.Source = masterVal.Source;
                            targetEntry.Translation = masterVal.Source; // Reset translation
                            targetEntry.IsTranslated = false;
                            targetEntry.Notes = masterVal.Notes; // Update notes
                            updatedStrings++;
                        }
                        else
                        {
                            // Source text is the same. Just update notes just in case.
                            targetEntry.Notes = masterVal.Notes;
                        }
                        newEntries.Add(key, targetEntry); // Keep the existing entry
                    }
                    else
                    {
                        // Case 2: Key is new. Add it from the master.
                        newEntries.Add(key, new TranslationEntry
                        {
                            Source = masterVal.Source,
                            Translation = masterVal.Source, // Placeholder
                            IsTranslated = false,
                            Notes = masterVal.Notes
                        });
                        newStrings++;
                    }
                }

                // Check for removed strings
                // (We can find this by comparing the old target count to the new entry count)
                removedStrings = target.Entries.Count - (newEntries.Count - newStrings);

                // Replace the old entries with the new, synced entries
                target.Entries = newEntries;
                target.Metadata.LastEditDate = DateTime.UtcNow;

                // Save the updated target file
                TranslationFileService.SaveFile(target, opts.TargetFile);
                System.Console.WriteLine("Sync complete!");
                System.Console.WriteLine($"  {newStrings} new strings added.");
                System.Console.WriteLine($"  {updatedStrings} strings updated (source changed).");
                System.Console.WriteLine($"  {removedStrings} strings removed (orphaned).");
                return 0; // Success
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR: {ex.Message}");
                return 1; // Failure
            }
        }
    }
}
