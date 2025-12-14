using CommandLine;
using gMKVToolNix.Translator.Console.Commands;

namespace gMKVToolNix.Translator.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            // This uses the CommandLineParser library to route to the correct
            // verb (class) based on the first argument (scan, master, template, sync).
            _ = Parser.Default.ParseArguments<ScanOptions, MasterOptions, TemplateOptions, SyncOptions>(args)
                .MapResult(
                    (ScanOptions opts) => ScanCommand.Execute(opts),
                    (MasterOptions opts) => MasterCommand.Execute(opts),
                    (TemplateOptions opts) => TemplateCommand.Execute(opts),
                    (SyncOptions opts) => SyncCommand.Execute(opts),
                    errs => 1); // Return 1 on error
        }
    }
}
