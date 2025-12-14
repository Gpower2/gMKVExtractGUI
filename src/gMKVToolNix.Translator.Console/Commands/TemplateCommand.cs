using System;
using System.IO;
using gMKVToolNix.Localization;

namespace gMKVToolNix.Translator.Console.Commands
{
    public class TemplateCommand
    {
        public static int Execute(TemplateOptions opts)
        {
            try
            {
                // Load master file
                var master = TranslationFileService.LoadFile(opts.MasterFile);
                if (master == null) return 1;

                // Create new translation file
                var newFile = new TranslationFile
                {
                    Metadata = new Metadata
                    {
                        Culture = opts.Culture,
                        Translator = "TRANSLATOR_NAME_HERE",
                        CreationDate = DateTime.UtcNow,
                        LastEditDate = DateTime.UtcNow
                    }
                };

                // Copy all entries from master, setting them as untranslated
                foreach (var entry in master.Entries)
                {
                    newFile.Entries[entry.Key] = new TranslationEntry
                    {
                        Source = entry.Value.Source, // Copy source
                        Translation = entry.Value.Source, // Use source as placeholder
                        IsTranslated = false, // Mark as untranslated
                        Notes = entry.Value.Notes // Copy notes for context
                    };
                }

                // Determine output path
                string outputPath = opts.OutputFile;
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Path.Combine(Path.GetDirectoryName(opts.MasterFile), $"{opts.Culture}.json");
                }

                // Save the new file
                TranslationFileService.SaveFile(newFile, outputPath);
                System.Console.WriteLine($"Template file created: {outputPath}");
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
