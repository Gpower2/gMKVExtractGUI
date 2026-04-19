using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using gMKVToolNix.Localization;

namespace gMKVToolNix.Translator.Console.Commands
{
    public class MasterCommand
    {
        // Regex to find: GetString("key"
        // It's intentionally simple to be fast.
        private static readonly Regex GetStringRegex = new Regex(@"GetString\s*\(\s*\""([^\""]+)\""");

        public static int Execute(MasterOptions opts)
        {
            System.Console.WriteLine($"Scanning {opts.SourceDirectory} for localization keys...");
            var keysFromCode = new HashSet<string>();

            try
            {
                // 1. Scan all files and find all keys
                foreach (string file in Directory.EnumerateFiles(opts.SourceDirectory, "*.cs", SearchOption.AllDirectories))
                {
                    if (file.Contains("\\obj\\") 
                        || file.Contains("\\bin\\")
                        || file.Contains("\\gMKVToolNix.Translator.Console")
                        || file.Contains("\\gMKVToolnix.Unit.Tests")
                        || file.Contains("\\ILocalizationService.cs")
                        || file.Contains("\\JsonLocalizationService.cs") )
                    {
                        continue;
                    }

                    string content = File.ReadAllText(file);
                    foreach (Match match in GetStringRegex.Matches(content))
                    {
                        if (match.Groups.Count > 1)
                        {
                            keysFromCode.Add(match.Groups[1].Value);
                        }
                    }
                }

                System.Console.WriteLine($"Found {keysFromCode.Count} unique keys in code.");

                // 2. Load existing master file (if it exists)
                TranslationFile oldMaster = File.Exists(opts.MasterFile)
                    ? TranslationFileService.LoadFile(opts.MasterFile)
                    : new TranslationFile { Metadata = { Culture = "en", CreationDate = DateTime.UtcNow } };

                if (oldMaster == null) 
                { 
                    oldMaster = new TranslationFile { Metadata = { Culture = "en", CreationDate = DateTime.UtcNow } }; 
                }

                // 3. Create the new master file by merging
                var newMaster = new TranslationFile
                {
                    Metadata = oldMaster.Metadata
                };
                newMaster.Metadata.LastEditDate = DateTime.UtcNow;

                int newKeys = 0;
                var orphanedKeys = new List<string>(oldMaster.Entries.Keys);

                foreach (string key in keysFromCode)
                {
                    orphanedKeys.Remove(key); // This key is used, not an orphan

                    if (oldMaster.Entries.TryGetValue(key, out var oldEntry))
                    {
                        // Case 1: Key already exists. Preserve it.
                        newMaster.Entries.Add(key, oldEntry);
                    }
                    else
                    {
                        // Case 2: New key found in code. Add a placeholder.
                        newMaster.Entries.Add(key, new TranslationEntry
                        {
                            Source = $"!NEW! {key}", // Placeholder
                            Translation = $"!NEW! {key}", // Placeholder
                            IsTranslated = true, // It's "en"
                            Notes = "!NEW! PLEASE ADD CONTEXT NOTES"
                        });
                        newKeys++;
                    }
                }

                // 4. Save the new master file
                TranslationFileService.SaveFile(newMaster, opts.MasterFile);

                System.Console.WriteLine("Master file sync complete!");
                System.Console.WriteLine($"  {newKeys} new keys added to master file.");

                if (orphanedKeys.Count > 0)
                {
                    System.Console.WriteLine($"  WARNING: {orphanedKeys.Count} orphaned keys found (in {opts.MasterFile} but not in code):");
                    foreach (var key in orphanedKeys)
                    {
                        System.Console.WriteLine($"    - {key}");
                    }
                }

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
