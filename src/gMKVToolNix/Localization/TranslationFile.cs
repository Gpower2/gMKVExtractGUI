using System.Collections.Generic;

namespace gMKVToolNix.Localization
{
    public class TranslationFile
    {
        public Metadata Metadata { get; set; }

        public Dictionary<string, TranslationEntry> Entries { get; set; }

        public TranslationFile()
        {
            Metadata = new Metadata();
            Entries = new Dictionary<string, TranslationEntry>();
        }
    }
}
