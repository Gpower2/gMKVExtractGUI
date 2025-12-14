using System;
using System.Collections.Generic;
using System.Linq;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvExtract
{
    public sealed class gMKVExtractSegmentsParameters
    {
        public string MKVFile { get; set; } = "";
        public List<gMKVSegment> MKVSegmentsToExtract { get; set; }
        public string OutputDirectory { get; set; } = "";
        public MkvChapterTypes ChapterType { get; set; }
        public TimecodesExtractionMode TimecodesExtractionMode { get; set; }
        public CuesExtractionMode CueExtractionMode { get; set; }
        public gMKVExtractFilenamePatterns FilenamePatterns { get; set; }
        public bool DisableBomForTextFiles { get; set; } = false;
        public bool UseRawExtractionMode { get; set; } = false;
        public bool UseFullRawExtractionMode { get; set; } = false;
        public bool OverwriteExistingFile { get; set; } = false;

        public override bool Equals(object oth)
        {
            gMKVExtractSegmentsParameters other = oth as gMKVExtractSegmentsParameters;
            if (other == null)
            {
                return false;
            }

            return
                MKVFile.Equals(other.MKVFile, StringComparison.OrdinalIgnoreCase)
                && Enumerable.SequenceEqual(
                    MKVSegmentsToExtract.Select(t => t.GetHashCode()).OrderBy(t => t),
                    other.MKVSegmentsToExtract.Select(t => t.GetHashCode()).OrderBy(t => t))
                && OutputDirectory.Equals(other.OutputDirectory, StringComparison.OrdinalIgnoreCase)
                && ChapterType == other.ChapterType
                && TimecodesExtractionMode == other.TimecodesExtractionMode
                && CueExtractionMode == other.CueExtractionMode
                && FilenamePatterns.Equals(other.FilenamePatterns)
                && DisableBomForTextFiles.Equals(other.DisableBomForTextFiles)
                && UseRawExtractionMode.Equals(other.UseRawExtractionMode)
                && UseFullRawExtractionMode.Equals(other.UseFullRawExtractionMode)
                && OverwriteExistingFile.Equals(other.OverwriteExistingFile)
            ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + MKVFile.GetHashCode();
                hash = hash * 23 + MKVSegmentsToExtract.GetHashCode();
                hash = hash * 23 + OutputDirectory.GetHashCode();
                hash = hash * 23 + ChapterType.GetHashCode();
                hash = hash * 23 + TimecodesExtractionMode.GetHashCode();
                hash = hash * 23 + CueExtractionMode.GetHashCode();
                hash = hash * 23 + FilenamePatterns.GetHashCode();
                hash = hash * 23 + DisableBomForTextFiles.GetHashCode();
                hash = hash * 23 + UseRawExtractionMode.GetHashCode();
                hash = hash * 23 + UseFullRawExtractionMode.GetHashCode();
                hash = hash * 23 + OverwriteExistingFile.GetHashCode();
                return hash;
            }
        }
    }
}