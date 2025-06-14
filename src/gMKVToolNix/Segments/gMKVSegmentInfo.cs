using System;

namespace gMKVToolNix.Segments
{
    [Serializable]
    public class gMKVSegmentInfo : gMKVSegment
    {
        public string TimecodeScale { get; set; } = "";

        public string MuxingApplication { get; set; } = "";

        public string WritingApplication { get; set; } = "";

        public string Duration { get; set; } = "";

        public string Date { get; set; } = "";

        /// <summary>
        /// The segment's file filename
        /// </summary>
        public string Filename { get; set; } = "";

        /// <summary>
        /// The segment's file directory
        /// </summary>
        public string Directory { get; set; } = "";

        /// <summary>
        /// Returns the segment's full file path
        /// </summary>
        public string Path
        {
            get
            {
                return System.IO.Path.Combine(Directory ?? "", Filename ?? "");
            }
        }

        public override bool Equals(object oth)
        {
            gMKVSegmentInfo other = oth as gMKVSegmentInfo;
            if (other == null)
            {
                return false;
            }

            return
                Date.Equals(other.Date, StringComparison.OrdinalIgnoreCase)
                && Directory.Equals(other.Directory, StringComparison.OrdinalIgnoreCase)
                && Duration.Equals(other.Duration, StringComparison.OrdinalIgnoreCase)
                && Filename.Equals(other.Filename, StringComparison.OrdinalIgnoreCase)
                && MuxingApplication.Equals(other.MuxingApplication, StringComparison.OrdinalIgnoreCase)
                && TimecodeScale.Equals(other.TimecodeScale, StringComparison.OrdinalIgnoreCase)
                && WritingApplication.Equals(other.WritingApplication, StringComparison.OrdinalIgnoreCase)
            ;
        }

        public override int GetHashCode()
        {
            // https://stackoverflow.com/a/263416
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Date.GetHashCode();
                hash = hash * 23 + Directory.GetHashCode();
                hash = hash * 23 + Duration.GetHashCode();
                hash = hash * 23 + Filename.GetHashCode();
                hash = hash * 23 + MuxingApplication.GetHashCode();
                hash = hash * 23 + TimecodeScale.GetHashCode();
                hash = hash * 23 + WritingApplication.GetHashCode();
                return hash;
            }
        }
    }
}