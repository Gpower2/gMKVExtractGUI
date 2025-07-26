using System;
using System.Text;

namespace gMKVToolNix.Segments
{
    [Serializable]
    public class gMKVTrack : gMKVSegment
    {
        public int TrackNumber { get; set; }

        public int TrackID { get; set; }

        public MkvTrackType TrackType { get; set; }

        public string CodecID { get; set; } = "";

        public string CodecPrivate { get; set; } = "";

        public string CodecPrivateData { get; set; } = "";
        
        public bool Forced { get; set; } = false;

        public string Language { get; set; } = "";

        public string LanguageIetf { get; set; } = "";

        public string TrackName { get; set; } = "";

        public string ExtraInfo { get; set; } = "";

        public int Delay { get; set; } = int.MinValue;

        public int EffectiveDelay { get; set; } = int.MinValue;

        /// <summary>
        /// In nanoseconds
        /// </summary>
        public long MinimumTimestamp { get; set; } = long.MinValue;

        public int VideoPixelWidth { get; set; }
        public int VideoPixelHeight { get; set; }

        public int AudioSamplingFrequency { get; set; }
        public int AudioChannels { get; set; }

        public override string ToString()
        {
            StringBuilder outputBuilder = new StringBuilder();

            outputBuilder.Append($"Track {TrackNumber} [TID {TrackID}][{TrackType}][{CodecID}]");

            if (!string.IsNullOrWhiteSpace(TrackName))
            {
                outputBuilder.Append($"[{TrackName}]");
            }

            if (!string.IsNullOrWhiteSpace(Language))
            {
                outputBuilder.Append($"[{Language}]");
            }

            if (!string.IsNullOrWhiteSpace(LanguageIetf))
            {
                outputBuilder.Append($"[{LanguageIetf}]");
            }

            if (Forced)
            {
                outputBuilder.Append("[FORCED]");
            }

            if (!string.IsNullOrWhiteSpace(ExtraInfo))
            {
                outputBuilder.Append($"[{ExtraInfo}]");
            }            

            if (!string.IsNullOrWhiteSpace(CodecPrivate))
            {
                outputBuilder.Append($"[{CodecPrivate}]");
            }

            if (TrackType != MkvTrackType.subtitles)
            {
                outputBuilder.Append($"[{Delay} ms][{EffectiveDelay} ms]");
            }

            return outputBuilder.ToString();
        }

        public override bool Equals(object oth)
        {
            gMKVTrack other = oth as gMKVTrack;
            if (other == null)
            {
                return false;
            }

            return
                AudioChannels == other.AudioChannels
                && AudioSamplingFrequency == other.AudioSamplingFrequency
                && CodecID.Equals( other.CodecID, StringComparison.OrdinalIgnoreCase)
                && CodecPrivate.Equals(other.CodecPrivate, StringComparison.OrdinalIgnoreCase)
                && CodecPrivateData.Equals(other.CodecPrivateData, StringComparison.OrdinalIgnoreCase)
                && Delay == other.Delay
                && EffectiveDelay == other.EffectiveDelay
                && ExtraInfo.Equals(other.ExtraInfo, StringComparison.OrdinalIgnoreCase)
                && Forced.Equals(other.Forced)
                && Language.Equals(other.Language, StringComparison.OrdinalIgnoreCase)
                && LanguageIetf.Equals(other.LanguageIetf, StringComparison.OrdinalIgnoreCase)
                && MinimumTimestamp == other.MinimumTimestamp
                && TrackID == other.TrackID
                && TrackName.Equals(other.TrackName, StringComparison.OrdinalIgnoreCase)
                && TrackNumber == other.TrackNumber
                && TrackType == other.TrackType
                && VideoPixelHeight == other.VideoPixelHeight
                && VideoPixelWidth == other.VideoPixelWidth
            ;
        }

        public override int GetHashCode()
        {
            // https://stackoverflow.com/a/263416
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + AudioChannels.GetHashCode();
                hash = hash * 23 + AudioSamplingFrequency.GetHashCode();
                hash = hash * 23 + CodecID.GetHashCode();
                hash = hash * 23 + CodecPrivate.GetHashCode();
                hash = hash * 23 + CodecPrivateData.GetHashCode();
                hash = hash * 23 + Delay.GetHashCode();
                hash = hash * 23 + EffectiveDelay.GetHashCode();
                hash = hash * 23 + ExtraInfo.GetHashCode();
                hash = hash * 23 + Forced.GetHashCode();
                hash = hash * 23 + Language.GetHashCode();
                hash = hash * 23 + LanguageIetf.GetHashCode();
                hash = hash * 23 + MinimumTimestamp.GetHashCode();
                hash = hash * 23 + TrackID.GetHashCode();
                hash = hash * 23 + TrackName.GetHashCode();
                hash = hash * 23 + TrackNumber.GetHashCode();
                hash = hash * 23 + TrackType.GetHashCode();
                hash = hash * 23 + VideoPixelHeight.GetHashCode();
                hash = hash * 23 + VideoPixelWidth.GetHashCode();
                return hash;
            }
        }
    }
}