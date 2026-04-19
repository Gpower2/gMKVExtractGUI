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
                var master = TranslationFileService.LoadFile(opts.MasterFile);
                var newFile = TranslationMaintenanceService.CreateTemplate(master, opts.Culture, "TRANSLATOR_NAME_HERE");

                string outputPath = opts.OutputFile;
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Path.Combine(
                        Path.GetDirectoryName(opts.MasterFile),
                        TranslationPathService.GetTranslationFileName(opts.Culture));
                }

                TranslationFileService.SaveFile(newFile, outputPath);
                System.Console.WriteLine($"Template file created: {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR: {ex.Message}");
                return 1;
            }
        }
    }
}
