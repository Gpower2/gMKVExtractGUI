using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gMKVToolNix.CueSheet;

namespace gMKVToolNix.MkvExtract
{
    public static class ChapterExtensions
    {
        public static string ConvertChaptersToCueFile(
            this Chapters argChapters,
            string argMKVFile)
        {
            Cue cue = new Cue
            {
                File = Path.GetFileName(argMKVFile),
                FileType = "WAVE",
                Title = Path.GetFileName(argMKVFile),
                Tracks = new List<CueTrack>()
            };

            if (argChapters.EditionEntry != null
                && argChapters.EditionEntry.Length > 0
                && argChapters.EditionEntry[0].ChapterAtom != null
                && argChapters.EditionEntry[0].ChapterAtom.Length > 0)
            {
                int currentChapterTrackNumber = 1;
                foreach (ChapterAtom atom in argChapters.EditionEntry[0].ChapterAtom)
                {
                    CueTrack cueTrack = new CueTrack
                    {
                        Number = currentChapterTrackNumber
                    };

                    if (atom.ChapterDisplay != null
                        && atom.ChapterDisplay.Length > 0)
                    {
                        cueTrack.Title = atom.ChapterDisplay[0].ChapterString;
                    }

                    if (!string.IsNullOrWhiteSpace(atom.ChapterTimeStart)
                        && atom.ChapterTimeStart.Contains(":"))
                    {
                        string[] timeElements = atom.ChapterTimeStart.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (timeElements.Length == 3)
                        {
                            // Find cue minutes from hours and minutes
                            int hours = int.Parse(timeElements[0]);
                            int minutes = int.Parse(timeElements[1]) + 60 * hours;

                            // Convert nanoseconds to frames (each second is 75 frames)
                            long nanoSeconds = 0;
                            int frames = 0;
                            int secondsLength = timeElements[2].Length;
                            if (timeElements[2].Contains("."))
                            {
                                secondsLength = timeElements[2].IndexOf(".");
                                nanoSeconds = long.Parse(timeElements[2].Substring(timeElements[2].IndexOf(".") + 1));
                                // I take the integer part of the result action in order to get the first frame
                                frames = Convert.ToInt32(Math.Floor((double)nanoSeconds / 1000000000.0 * 75.0));
                            }
                            cueTrack.Index = string.Format("{0}:{1}:{2}",
                                minutes.ToString("#00")
                                , timeElements[2].Substring(0, secondsLength)
                                , frames.ToString("00")
                                );
                        }
                    }

                    cue.Tracks.Add(cueTrack);
                    currentChapterTrackNumber++;
                }
            }

            StringBuilder cueBuilder = new StringBuilder();

            cueBuilder.AppendFormat("REM GENRE \"\"\r\n");
            cueBuilder.AppendFormat("REM DATE \"\"\r\n");
            cueBuilder.AppendFormat("PERFORMER \"\"\r\n");
            cueBuilder.AppendFormat("TITLE \"{0}\"\r\n", cue.Title);
            cueBuilder.AppendFormat("FILE \"{0}\" {1}\r\n", cue.File, cue.FileType);

            foreach (CueTrack tr in cue.Tracks)
            {
                cueBuilder.AppendFormat("\tTRACK {0} AUDIO\r\n", tr.Number.ToString("00"));
                cueBuilder.AppendFormat("\t\tTITLE \"{0}\"\r\n", tr.Title);
                cueBuilder.AppendFormat("\t\tPERFORMER \"\"\r\n");
                cueBuilder.AppendFormat("\t\tINDEX 01 {0}\r\n", tr.Index);
            }

            return cueBuilder.ToString();
        }

        public static string ConvertChaptersToPbf(
            this Chapters argChapters,
            string argMKVFile)
        {
            // First line : [Bookmark]
            // Other lines: {i}={timestamp_in_ms}*{title}*{hash_data}
            // i starts from 0
            StringBuilder pbfBuilder = new StringBuilder();

            // Add the header for PBF format
            pbfBuilder.AppendLine("[Bookmark]");

            // Add the chapters in PBF format
            if (argChapters.EditionEntry != null
                && argChapters.EditionEntry.Length > 0
                && argChapters.EditionEntry[0].ChapterAtom != null
                && argChapters.EditionEntry[0].ChapterAtom.Length > 0)
            {
                int currentChapterTrackNumber = 0;
                foreach (ChapterAtom atom in argChapters.EditionEntry[0].ChapterAtom)
                {
                    if (!string.IsNullOrWhiteSpace(atom.ChapterTimeStart)
                        && atom.ChapterTimeStart.Contains(":"))
                    {
                        string[] timeElements = atom.ChapterTimeStart.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (timeElements.Length == 3)
                        {
                            // Find milliseconds from hours, minutes and seconds
                            int hours = int.Parse(timeElements[0]);
                            int minutes = int.Parse(timeElements[1]);
                            int secondsLength = timeElements[2].Length;

                            int milliseconds = 0;
                            if (timeElements[2].Contains("."))
                            {
                                secondsLength = timeElements[2].IndexOf(".");
                                // Convert nanoseconds to milliseconds
                                milliseconds = int.Parse(timeElements[2].Substring(secondsLength + 1)) / 1000000;
                            }
                            int seconds = int.Parse(timeElements[2].Substring(0, secondsLength));

                            long totalMilliseconds =
                                hours * 3600 * 1000L
                                + (minutes * 60 * 1000L)
                                + (seconds * 1000L)
                                + milliseconds;

                            pbfBuilder.AppendFormat("{0}={1}*{2}*{3}",
                                currentChapterTrackNumber.ToString("#0"),
                                totalMilliseconds.ToString(),
                                atom.ChapterDisplay != null
                                && atom.ChapterDisplay.Length > 0
                                    ? atom.ChapterDisplay[0].ChapterString
                                    : "",
                                "" // Empty string for hash data, as we don't have anything to add
                            );
                            pbfBuilder.AppendLine();
                        }
                    }
                    currentChapterTrackNumber++;
                }
            }

            return pbfBuilder.ToString();
        }
    }
}
