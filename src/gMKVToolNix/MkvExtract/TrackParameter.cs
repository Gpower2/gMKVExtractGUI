namespace gMKVToolNix.MkvExtract
{
    public class TrackParameter
    {
        public MkvExtractModes ExtractMode { get; set; } = MkvExtractModes.tracks;
        public string Options { get; set; } = "";
        public string TrackOutput { get; set; } = "";
        public bool WriteOutputToFile { get; set; } = false;
        public bool DisableBomForTextFiles { get; set; } = false;
        public bool UseRawExtractionMode { get; set; } = false;
        public bool UseFullRawExtractionMode { get; set; } = false;
        public string OutputFilename { get; set; } = "";

        public TrackParameter(
            MkvExtractModes argExtractMode,
            string argOptions,
            string argTrackOutput,
            bool argWriteOutputToFile,
            bool argDisableBomForTextFiles,
            bool argUseRawExtractionMode,
            bool argUseFullRawExtractionMode,
            string argOutputFilename)
        {
            ExtractMode = argExtractMode;
            Options = argOptions;
            TrackOutput = argTrackOutput;
            WriteOutputToFile = argWriteOutputToFile;
            DisableBomForTextFiles = argDisableBomForTextFiles;
            UseRawExtractionMode = argUseRawExtractionMode;
            UseFullRawExtractionMode = argUseFullRawExtractionMode;
            OutputFilename = argOutputFilename;
        }

        public TrackParameter() { }
    }
}
