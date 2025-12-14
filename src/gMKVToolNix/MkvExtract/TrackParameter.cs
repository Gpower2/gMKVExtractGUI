namespace gMKVToolNix.MkvExtract
{
    public class TrackParameter
    {
        public MkvExtractModes ExtractMode { get; set; } = MkvExtractModes.tracks;
        public string Options { get; set; } = "";
        public string TrackOutput { get; set; } = "";
        public bool WriteOutputToFile { get; set; } = false;
        public bool DisableBomForTextFiles { get; set; } = false;
        public string OutputFilename { get; set; } = "";

        public TrackParameter(
            MkvExtractModes argExtractMode,
            string argOptions,
            string argTrackOutput,
            bool argWriteOutputToFile,
            bool argDisableBomForTextFiles,
            string argOutputFilename)
        {
            ExtractMode = argExtractMode;
            Options = argOptions;
            TrackOutput = argTrackOutput;
            WriteOutputToFile = argWriteOutputToFile;
            DisableBomForTextFiles = argDisableBomForTextFiles;
            OutputFilename = argOutputFilename;
        }

        public TrackParameter() { }
    }
}
