namespace gMKVToolNix.MkvExtract
{
    public class TrackParameter
    {
        public MkvExtractModes ExtractMode { get; set; } = MkvExtractModes.tracks;
        public string Options { get; set; } = "";
        public string TrackOutput { get; set; } = "";
        public bool WriteOutputToFile { get; set; } = false;
        public string OutputFilename { get; set; } = "";

        public TrackParameter(
            MkvExtractModes argExtractMode,
            string argOptions,
            string argTrackOutput,
            bool argWriteOutputToFile,
            string argOutputFilename)
        {
            ExtractMode = argExtractMode;
            Options = argOptions;
            TrackOutput = argTrackOutput;
            WriteOutputToFile = argWriteOutputToFile;
            OutputFilename = argOutputFilename;
        }

        public TrackParameter() { }
    }
}
