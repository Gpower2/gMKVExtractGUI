using System;

namespace gMKVToolNix.Segments
{
    [Serializable]
    public class gMKVChapter : gMKVSegment
    {
        public int ChapterCount { get; set; }

        public override string ToString()
        {
            string entryString = ChapterCount > 1 ? "entries" : "entry";
            return $"Chapters {ChapterCount} {entryString}";
        }

        public override bool Equals(object oth)
        {
            gMKVChapter other = oth as gMKVChapter;
            if (other == null)
            {
                return false;
            }

            return ChapterCount == other.ChapterCount;
        }

        public override int GetHashCode()
        {
            return ChapterCount.GetHashCode();
        }
    }
}
