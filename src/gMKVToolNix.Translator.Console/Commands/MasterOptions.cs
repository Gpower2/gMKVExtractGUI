using CommandLine;

namespace gMKVToolNix.Translator.Console.Commands
{
    [Verb("master", HelpText = "Generate or update the master (en.json) file by scanning code for GetString() calls.")]
    public class MasterOptions
    {
        [Option('s', "source", Required = true, HelpText = "The root source code directory to scan.")]
        public string SourceDirectory { get; set; }

        [Option('m', "master", Required = true, HelpText = "The path to the master (e.g., en.json) translation file to create or update.")]
        public string MasterFile { get; set; }
    }
}
