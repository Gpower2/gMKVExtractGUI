using CommandLine;

namespace gMKVToolNix.Translator.Console.Commands
{
    [Verb("template", HelpText = "Create a new translation template from the master file.")]
    public class TemplateOptions
    {
        [Option('m', "master", Required = true, HelpText = "The master (e.g., en.json) translation file.")]
        public string MasterFile { get; set; }

        [Option('c', "culture", Required = true, HelpText = "The new culture code (e.g., de-DE).")]
        public string Culture { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file path. If omitted, uses culture.json in master's folder.")]
        public string OutputFile { get; set; }
    }
}
