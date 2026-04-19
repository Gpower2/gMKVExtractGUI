using CommandLine;

namespace gMKVToolNix.Translator.Console.Commands
{
    [Verb("sync", HelpText = "Sync a target translation file with the master file.")]
    public class SyncOptions
    {
        [Option('m', "master", Required = true, HelpText = "The master (e.g., gmkvextract-en.json) translation file.")]
        public string MasterFile { get; set; }

        [Option('t', "target", Required = true, HelpText = "The target translation file to update (e.g., gmkvextract-de.json).")]
        public string TargetFile { get; set; }
    }
}
