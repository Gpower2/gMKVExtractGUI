using System;
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
                var master = TranslationFileService.LoadFile(opts.MasterFile);
                var target = TranslationFileService.LoadFile(opts.TargetFile);
                var result = TranslationMaintenanceService.Synchronize(master, target);

                TranslationFileService.SaveFile(result.TranslationFile, opts.TargetFile);
                System.Console.WriteLine("Sync complete!");
                System.Console.WriteLine($"  {result.AddedCount} new strings added.");
                System.Console.WriteLine($"  {result.UpdatedCount} strings updated (source changed).");
                System.Console.WriteLine($"  {result.RemovedCount} strings removed (orphaned).");
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
