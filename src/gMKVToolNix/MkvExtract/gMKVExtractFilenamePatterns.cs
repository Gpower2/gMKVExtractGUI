using System;

namespace gMKVToolNix.MkvExtract
{
    public sealed class gMKVExtractFilenamePatterns
    {
        // Common placeholders
        public static readonly string FilenameNoExt = "{FilenameNoExt}";
        public static readonly string Filename = "{Filename}";
        public static readonly string DirectorySeparator = "{DirSeparator}";

        // Common Track placeholders
        public static readonly string TrackNumber = "{TrackNumber}";
        public static readonly string TrackNumber_0 = "{TrackNumber:0}";
        public static readonly string TrackNumber_00 = "{TrackNumber:00}";
        public static readonly string TrackNumber_000 = "{TrackNumber:000}";
        public static readonly string TrackID = "{TrackID}";
        public static readonly string TrackID_0 = "{TrackID:0}";
        public static readonly string TrackID_00 = "{TrackID:00}";
        public static readonly string TrackID_000 = "{TrackID:000}";
        public static readonly string TrackName = "{TrackName}";
        public static readonly string TrackLanguage = "{Language}";
        public static readonly string TrackLanguageIetf = "{LanguageIETF}";
        public static readonly string TrackCodecID = "{CodecID}";
        public static readonly string TrackCodecPrivate = "{CodecPrivate}";
        public static readonly string TrackDelay = "{Delay}";
        public static readonly string TrackEffectiveDelay = "{EffectiveDelay}";
        public static readonly string TrackForced = "{TrackForced}";

        // Video Track placeholders
        public static readonly string VideoPixelWidth = "{PixelWidth}";
        public static readonly string VideoPixelHeight = "{PixelHeight}";

        // Audio Track placeholders
        public static readonly string AudioSamplingFrequency = "{SamplingFrequency}";
        public static readonly string AudioChannels = "{Channels}";

        // Attachment placeholders
        public static readonly string AttachmentID = "{AttachmentID}";
        public static readonly string AttachmentID_0 = "{AttachmentID:0}";
        public static readonly string AttachmentID_00 = "{AttachmentID:00}";
        public static readonly string AttachmentID_000 = "{AttachmentID:000}";
        public static readonly string AttachmentFilename = "{AttachmentFilename}";
        public static readonly string AttachmentMimeType = "{MimeType}";
        public static readonly string AttachmentFileSize = "{AttachmentFileSize}";

        public string VideoTrackFilenamePattern { get; set; } = "";
        public string AudioTrackFilenamePattern { get; set; } = "";
        public string SubtitleTrackFilenamePattern { get; set; } = "";
        public string ChapterFilenamePattern { get; set; } = "";
        public string AttachmentFilenamePattern { get; set; } = "";
        public string TagsFilenamePattern { get; set; } = "";

        public override bool Equals(object oth)
        {
            gMKVExtractFilenamePatterns other = oth as gMKVExtractFilenamePatterns;
            if (oth == null)
            {
                return false;
            }

            return
                VideoTrackFilenamePattern.Equals(other.VideoTrackFilenamePattern, StringComparison.OrdinalIgnoreCase)
                && AudioTrackFilenamePattern.Equals(other.AudioTrackFilenamePattern, StringComparison.OrdinalIgnoreCase)
                && SubtitleTrackFilenamePattern.Equals(other.SubtitleTrackFilenamePattern, StringComparison.OrdinalIgnoreCase)
                && ChapterFilenamePattern.Equals(other.ChapterFilenamePattern, StringComparison.OrdinalIgnoreCase)
                && AttachmentFilenamePattern.Equals(other.AttachmentFilenamePattern, StringComparison.OrdinalIgnoreCase)
                && TagsFilenamePattern.Equals(other.TagsFilenamePattern, StringComparison.OrdinalIgnoreCase)
            ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + VideoTrackFilenamePattern.GetHashCode();
                hash = hash * 23 + AudioTrackFilenamePattern.GetHashCode();
                hash = hash * 23 + SubtitleTrackFilenamePattern.GetHashCode();
                hash = hash * 23 + ChapterFilenamePattern.GetHashCode();
                hash = hash * 23 + AttachmentFilenamePattern.GetHashCode();
                hash = hash * 23 + TagsFilenamePattern.GetHashCode();
                return hash;
            }
        }
    }
}