using System.Collections.Generic;

namespace gMKVToolNix.CueSheet
{
    public class Cue
    {
        /*
REM GENRE "Electronica"
REM DATE "1998"
PERFORMER "Faithless"
TITLE "Live in Berlin"
FILE "Faithless - Live in Berlin.mp3" MP3
        */

        public string Genre { get; set; }
        public string Date { get; set; }
        public string Performer { get; set; }
        public string Title { get; set; }
        public string File { get; set; }
        public string FileType { get; set; }

        public List<CueTrack> Tracks { get; set; }
    }
}
