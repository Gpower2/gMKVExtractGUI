using System;

namespace gMKVToolNix.Segments
{
    [Serializable]
    public class gMKVAttachment : gMKVSegment
    {
        public int ID { get; set; }

        public string Filename { get; set; } = "";

        public string MimeType { get; set; } = "";

        public string FileSize { get; set; } = "";

        public override string ToString()
        {
            return $"Attachment {ID} [{Filename}][{MimeType}][{FileSize} bytes]";
        }

        public override bool Equals(object oth)
        {
            gMKVAttachment other = oth as gMKVAttachment;
            if (other == null)
            {
                return false;
            }

            return
                Filename.Equals(other.Filename, StringComparison.OrdinalIgnoreCase)
                && FileSize.Equals(other.FileSize, StringComparison.OrdinalIgnoreCase)
                && ID == other.ID
                && MimeType.Equals(other.MimeType, StringComparison.OrdinalIgnoreCase)
            ;
        }

        public override int GetHashCode()
        {
            // https://stackoverflow.com/a/263416
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Filename.GetHashCode();
                hash = hash * 23 + FileSize.GetHashCode();
                hash = hash * 23 + ID.GetHashCode();
                hash = hash * 23 + MimeType.GetHashCode();
                return hash;
            }
        }
    }
}
