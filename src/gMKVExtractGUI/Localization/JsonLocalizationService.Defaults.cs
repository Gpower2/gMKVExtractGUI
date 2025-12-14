using System.Collections.Generic;

namespace gMKVToolNix.Localization
{
    public partial class JsonLocalizationService
    {
        // 4. Hardcoded English default
        private static readonly Dictionary<string, string> _englishDefaults = new Dictionary<string, string>
        {
            // 2. Keys are created manually
            { "MainMenu.File.Open", "Open" },
            { "MainMenu.File.Save", "Save" },
            { "Ticket.Status.Open", "Open" },
            { "Ticket.Status.Closed", "Closed" }
            // ... all other hardcoded English strings
        };
    }
}
