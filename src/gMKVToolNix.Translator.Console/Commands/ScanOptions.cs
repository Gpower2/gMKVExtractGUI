using CommandLine;

namespace gMKVToolNix.Translator.Console.Commands
{
    [Verb("scan", HelpText = "Scan a source directory for hardcoded strings.")]
    public class ScanOptions
    {
        [Option('s', "source", Required = true, HelpText = "The root source code directory to scan.")]
        public string SourceDirectory { get; set; }

        [Option('o', "output", Required = false, Default = "scan_report.json", HelpText = "Output file for the scan report.")]
        public string OutputFile { get; set; }
    }
}
