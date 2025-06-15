using System;

namespace gMKVToolNix.Jobs
{
    [Serializable]
    public class gMKVJobInfo
    {
        public gMKVJob Job { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public JobState State { get; set; } = JobState.Ready;

        public TimeSpan? Duration
        {
            get
            {
                if (StartTime.HasValue)
                {
                    if (EndTime.HasValue)
                    {
                        return ((TimeSpan?)(EndTime - StartTime)).Value;
                    }
                    else
                    {
                        return ((TimeSpan?)(DateTime.Now - StartTime)).Value;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public gMKVJobInfo(gMKVJob argJob)
        {
            Job = argJob;
        }

        // For serialization only!!!
        internal gMKVJobInfo() { }

        public void Reset()
        {
            EndTime = null;
            StartTime = null;
            State = JobState.Ready;
        }
    }
}
