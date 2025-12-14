namespace gMKVToolNix.Translator.Console.Commands
{
    public class ScanReportEntry
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string FoundString { get; set; }
        public string SuggestedKey { get; set; }
        public string FullLine { get; set; }
    }
}
