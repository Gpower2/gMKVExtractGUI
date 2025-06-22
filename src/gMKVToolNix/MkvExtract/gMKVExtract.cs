using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using gMKVToolNix.CueSheet;
using gMKVToolNix.Log;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvExtract
{
    public delegate void MkvExtractProgressUpdatedEventHandler(int progress);
    public delegate void MkvExtractTrackUpdatedEventHandler(string filename, string trackName);

    public class gMKVExtract
    {
        private static readonly char[] _invalidFilenameChars = Path.GetInvalidFileNameChars();
        private static readonly string _directorySeparator = Path.DirectorySeparatorChar.ToString();

        private class TrackParameter
        {
            public MkvExtractModes ExtractMode { get; set; } = MkvExtractModes.tracks;
            public string Options { get; set; } = "";
            public string TrackOutput { get; set; } = "";
            public bool WriteOutputToFile { get; set; } = false;
            public string OutputFilename { get; set; } = "";

            public TrackParameter(
                MkvExtractModes argExtractMode,
                string argOptions,
                string argTrackOutput,
                bool argWriteOutputToFile,
                string argOutputFilename)
            {
                ExtractMode = argExtractMode;
                Options = argOptions;
                TrackOutput = argTrackOutput;
                WriteOutputToFile = argWriteOutputToFile;
                OutputFilename = argOutputFilename;
            }

            public TrackParameter() { }
        }

        private class OptionValue
        {
            public MkvExtractGlobalOptions Option { get; set; }

            public string Parameter { get; set; } = "";

            public OptionValue(MkvExtractGlobalOptions option, string parameter)
            {
                Option = option;
                Parameter = parameter;
            }
        }

        /// <summary>
        /// Gets the mkvextract executable filename
        /// </summary>
        public static string MKV_EXTRACT_FILENAME
        {
            get { return PlatformExtensions.IsOnLinux ? "mkvextract" : "mkvextract.exe"; }
        }

        private static readonly XmlSerializer _chaptersXmlSerializer = new XmlSerializer(typeof(Chapters));

        private readonly string _MKVToolnixPath = "";
        private readonly string _MKVExtractFilename = "";
        private readonly StringBuilder _MKVExtractOutput = new StringBuilder();
        private StreamWriter _OutputFileWriter = null;
        private readonly StringBuilder _ErrorBuilder = new StringBuilder();
        private gMKVVersion _Version = null;

        public event MkvExtractProgressUpdatedEventHandler MkvExtractProgressUpdated;
        public event MkvExtractTrackUpdatedEventHandler MkvExtractTrackUpdated;

        private Exception _ThreadedException = null;
        public Exception ThreadedException { get { return _ThreadedException; } }

        public bool Abort { get; set; }

        public bool AbortAll { get; set; }

        public gMKVExtract(string mkvToonlixPath)
        {
            _MKVToolnixPath = mkvToonlixPath;
            _MKVExtractFilename = Path.Combine(_MKVToolnixPath, MKV_EXTRACT_FILENAME);
        }

        public void ExtractMKVSegmentsThreaded(object argParameters)
        {
            _ThreadedException = null;
            try
            {
                gMKVExtractSegmentsParameters parameters = (gMKVExtractSegmentsParameters)argParameters;
                ExtractMKVSegments(
                    parameters.MKVFile,
                    parameters.MKVSegmentsToExtract,
                    parameters.OutputDirectory,
                    parameters.ChapterType,
                    parameters.TimecodesExtractionMode,
                    parameters.CueExtractionMode,
                    parameters.FilenamePatterns
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        private List<TrackParameter> GetTrackParameters(
            gMKVSegment argSeg
            , string argMKVFile
            , string argOutputDirectory
            , MkvChapterTypes argChapterType
            , TimecodesExtractionMode argTimecodesExtractionMode
            , CuesExtractionMode argCueExtractionMode
            , gMKVExtractFilenamePatterns argFilenamePatterns
        )
        {
            // create the new parameter list type
            List<TrackParameter> trackParameterList = new List<TrackParameter>();

            // check the selected segment's type
            if (argSeg is gMKVTrack track)
            {
                // if we are in a mode that requires timecodes extraction, add the parameter for the track
                if (argTimecodesExtractionMode != TimecodesExtractionMode.NoTimecodes)
                {
                    trackParameterList.Add(new TrackParameter(
                        // Since MKVToolNix v17.0 the timecode word has been replaced with timestamp
                        GetMKVExtractVersion().FileMajorPart >= 17 ? MkvExtractModes.timestamps_v2 : MkvExtractModes.timecodes_v2,
                        "",
                        string.Format("{0}:\"{1}\"",
                            track.TrackID,
                            GetOutputFilename(argSeg, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.timestamps_v2)
                        ),
                        false,
                        ""
                    ));
                }

                // if we are in a mode that requires cues extraction, add the parameter for the track
                if (argCueExtractionMode != CuesExtractionMode.NoCues)
                {
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.cues,
                        "",
                        string.Format("{0}:\"{1}\"",
                            track.TrackID,
                            GetOutputFilename(argSeg, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.cues)
                        ),
                        false,
                        ""
                    ));
                }

                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                    (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.NoCues)
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                {

                    // add the parameter for extracting the track
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.tracks,
                        "",
                        string.Format("{0}:\"{1}\"",
                            track.TrackID,
                            GetOutputFilename(argSeg, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.tracks)
                        ),
                        false,
                        ""
                    ));
                }
            }
            else if (argSeg is gMKVAttachment attachment)
            {
                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                    (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.NoCues)
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                {
                    // add the parameter for extracting the attachment
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.attachments,
                        "",
                        string.Format("{0}:\"{1}\"",
                            attachment.ID,
                            GetOutputFilename(argSeg, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.attachments)
                        ),
                        false,
                        ""
                    ));
                }
            }
            else if (argSeg is gMKVChapter)
            {
                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                    (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.NoCues)
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    || (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes &&
                    argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                {
                    string options = "";

                    // check the chapter's type to determine the output file's extension and options
                    if (argChapterType == MkvChapterTypes.OGM)
                    {
                        options = "--simple";
                    }

                    string chapterFile = GetOutputFilename(argSeg, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.chapters, argChapterType);

                    // add the parameter for extracting the chapters
                    // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.chapters,
                        options,
                        GetMKVExtractVersion().FileMajorPart >= 17 ? chapterFile : "",
                        (GetMKVExtractVersion().FileMajorPart < 17),
                        GetMKVExtractVersion().FileMajorPart >= 17 ? "" : chapterFile
                    ));
                }
            }

            return trackParameterList;
        }

        public void ExtractMKVSegments(
            string argMKVFile
            , List<gMKVSegment> argMKVSegmentsToExtract
            , string argOutputDirectory
            , MkvChapterTypes argChapterType
            , TimecodesExtractionMode argTimecodesExtractionMode
            , CuesExtractionMode argCueExtractionMode
            , gMKVExtractFilenamePatterns argFilenamePatterns
        )
        {
            Abort = false;
            AbortAll = false;
            _ErrorBuilder.Length = 0;
            _MKVExtractOutput.Length = 0;
            // Analyze the MKV segments and get the initial parameters
            List<TrackParameter> initialParameters = new List<TrackParameter>();
            foreach (gMKVSegment seg in argMKVSegmentsToExtract)
            {
                if (AbortAll)
                {
                    _ErrorBuilder.AppendLine("User aborted all the processes!");
                    break;
                }
                try
                {
                    initialParameters.AddRange(GetTrackParameters(
                        seg, argMKVFile, argOutputDirectory, argChapterType, argTimecodesExtractionMode, argCueExtractionMode, argFilenamePatterns));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    _ErrorBuilder.AppendLine($"Segment: {seg}{Environment.NewLine}Exception: {ex.Message}{Environment.NewLine}");
                }
            }

            // Group the initial parameters, in order to batch extract the mkv segments
            List<TrackParameter> finalParameters = new List<TrackParameter>();
            foreach (TrackParameter initPar in initialParameters)
            {
                TrackParameter currentPar = null;
                foreach (TrackParameter finalPar in finalParameters)
                {
                    if (finalPar.ExtractMode == initPar.ExtractMode)
                    {
                        currentPar = finalPar;
                        break;
                    }
                }

                if (currentPar != null)
                {
                    currentPar.TrackOutput = $"{currentPar.TrackOutput} {initPar.TrackOutput}";
                }
                else
                {
                    finalParameters.Add(initPar);
                }
            }

            // Time to extract the mkv segments
            foreach (TrackParameter finalPar in finalParameters)
            {
                if (AbortAll)
                {
                    _ErrorBuilder.AppendLine("User aborted all the processes!");
                    break;
                }

                try
                {
                    if (finalPar.WriteOutputToFile)
                    {
                        _OutputFileWriter = new StreamWriter(finalPar.OutputFilename, false, new UTF8Encoding(false, true));
                    }

                    OnMkvExtractTrackUpdated(argMKVFile, finalPar.ExtractMode.ToString());
                    ExtractMkvSegment(argMKVFile, finalPar);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    _ErrorBuilder.AppendLine($"Track output: {finalPar.TrackOutput}{Environment.NewLine}Exception: {ex.Message}{Environment.NewLine}");
                }
                finally
                {
                    if (_OutputFileWriter != null)
                    {
                        _OutputFileWriter.Close();
                        _OutputFileWriter = null;
                    }

                    try
                    {
                        // If we have chapters with CUE or PBF format, then we read the XML chapters and convert it to CUE or PBF format
                        if (finalPar.ExtractMode == MkvExtractModes.chapters)
                        {
                            // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                            string outputFile = GetMKVExtractVersion().FileMajorPart >= 17 ? finalPar.TrackOutput : finalPar.OutputFilename;

                            string outputExtension = Path.GetExtension(outputFile).Substring(1).ToLowerInvariant();

                            if (outputExtension.Equals("cue")
                                || outputExtension.Equals("pbf"))
                            {
                                Chapters chapters = null;
                                using (StreamReader sr = new StreamReader(outputFile))
                                {
                                    chapters = (Chapters)_chaptersXmlSerializer.Deserialize(sr);
                                }

                                if (outputExtension.Equals("cue"))
                                {
                                    Cue cue = new Cue
                                    {
                                        File = Path.GetFileName(argMKVFile),
                                        FileType = "WAVE",
                                        Title = Path.GetFileName(argMKVFile),
                                        Tracks = new List<CueTrack>()
                                    };

                                    if (chapters.EditionEntry != null
                                        && chapters.EditionEntry.Length > 0
                                        && chapters.EditionEntry[0].ChapterAtom != null
                                        && chapters.EditionEntry[0].ChapterAtom.Length > 0)
                                    {
                                        int currentChapterTrackNumber = 1;
                                        foreach (ChapterAtom atom in chapters.EditionEntry[0].ChapterAtom)
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

                                    using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
                                    {
                                        sw.Write(cueBuilder.ToString());
                                    }
                                }
                                else if (outputExtension.Equals("pbf"))
                                {
                                    // First line : [Bookmark]
                                    // Other lines: {i}={timestamp_in_ms}*{title}*{hash_data}
                                    // i starts from 0
                                    StringBuilder pbfBuilder = new StringBuilder();
                                    
                                    // Add the header for PBF format
                                    pbfBuilder.AppendLine("[Bookmark]");

                                    // Add the chapters in PBF format
                                    if (chapters.EditionEntry != null
                                        && chapters.EditionEntry.Length > 0
                                        && chapters.EditionEntry[0].ChapterAtom != null
                                        && chapters.EditionEntry[0].ChapterAtom.Length > 0)
                                    {
                                        int currentChapterTrackNumber = 0;
                                        foreach (ChapterAtom atom in chapters.EditionEntry[0].ChapterAtom)
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

                                    using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
                                    {
                                        sw.Write(pbfBuilder.ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc);
                        _ErrorBuilder.AppendLine($"Track output: {finalPar.TrackOutput}{Environment.NewLine}Exception: {exc.Message}{Environment.NewLine}");
                    }
                }
            }

            // check for errors
            if (_ErrorBuilder.Length > 0)
            {
                throw new Exception(_ErrorBuilder.ToString());
            }
        }

        public void ExtractMKVTimecodesThreaded(object argParameters)
        {
            _ThreadedException = null;
            try
            {
                gMKVExtractSegmentsParameters parameters = (gMKVExtractSegmentsParameters)argParameters;
                ExtractMKVSegments(
                    parameters.MKVFile,
                    parameters.MKVSegmentsToExtract,
                    parameters.OutputDirectory,
                    parameters.ChapterType,
                    TimecodesExtractionMode.OnlyTimecodes,
                    CuesExtractionMode.NoCues,
                    parameters.FilenamePatterns
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMKVCuesThreaded(object argParameters)
        {
            _ThreadedException = null;
            try
            {
                gMKVExtractSegmentsParameters parameters = (gMKVExtractSegmentsParameters)argParameters;
                ExtractMKVSegments(
                    parameters.MKVFile,
                    parameters.MKVSegmentsToExtract,
                    parameters.OutputDirectory,
                    parameters.ChapterType,
                    TimecodesExtractionMode.NoTimecodes,
                    CuesExtractionMode.OnlyCues,
                    parameters.FilenamePatterns
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMkvCuesheetThreaded(object argParameters)
        {
            _ThreadedException = null;
            try
            {
                gMKVExtractSegmentsParameters parameters = (gMKVExtractSegmentsParameters)argParameters;
                ExtractMkvCuesheet(
                    parameters.MKVFile,
                    parameters.OutputDirectory,
                    parameters.FilenamePatterns
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMkvCuesheet(string argMKVFile, string argOutputDirectory, gMKVExtractFilenamePatterns argFilenamePatterns)
        {
            Abort = false;
            AbortAll = false;
            _ErrorBuilder.Length = 0;
            _MKVExtractOutput.Length = 0;

            string cueFile = GetOutputFilename(null, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.cuesheet);
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Cue Sheet");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                if (GetMKVExtractVersion().FileMajorPart < 17)
                {
                    _OutputFileWriter = new StreamWriter(cueFile, false, new UTF8Encoding(false, true));
                }
                ExtractMkvSegment(
                    argMKVFile
                     , new TrackParameter(
                        MkvExtractModes.cuesheet
                        , ""
                        , GetMKVExtractVersion().FileMajorPart >= 17 ? cueFile : ""
                        , (GetMKVExtractVersion().FileMajorPart < 17)
                        , GetMKVExtractVersion().FileMajorPart >= 17 ? "" : cueFile
                    )
               );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (_OutputFileWriter != null)
                {
                    _OutputFileWriter.Close();
                    _OutputFileWriter = null;
                }
            }

            // check for errors
            if (_ErrorBuilder.Length > 0)
            {
                throw new Exception(_ErrorBuilder.ToString());
            }
        }

        public void ExtractMkvTagsThreaded(object argParameters)
        {
            _ThreadedException = null;
            try
            {
                gMKVExtractSegmentsParameters parameters = (gMKVExtractSegmentsParameters)argParameters;
                ExtractMkvTags(
                    parameters.MKVFile,
                    parameters.OutputDirectory,
                    parameters.FilenamePatterns
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMkvTags(string argMKVFile, string argOutputDirectory, gMKVExtractFilenamePatterns argFilenamePatterns)
        {
            Abort = false;
            AbortAll = false;
            _ErrorBuilder.Length = 0;
            _MKVExtractOutput.Length = 0;

            string tagsFile = GetOutputFilename(null, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.tags);
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Tags");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                if (GetMKVExtractVersion().FileMajorPart < 17)
                {
                    _OutputFileWriter = new StreamWriter(tagsFile, false, new UTF8Encoding(false, true));
                }
                ExtractMkvSegment(
                    argMKVFile
                    , new TrackParameter(
                        MkvExtractModes.tags
                        , ""
                        , GetMKVExtractVersion().FileMajorPart >= 17 ? tagsFile : ""
                        , (GetMKVExtractVersion().FileMajorPart < 17)
                        , GetMKVExtractVersion().FileMajorPart >= 17 ? "" : tagsFile
                    )
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (_OutputFileWriter != null)
                {
                    _OutputFileWriter.Close();
                    _OutputFileWriter = null;
                }
            }

            // check for errors
            if (_ErrorBuilder.Length > 0)
            {
                throw new Exception(_ErrorBuilder.ToString());
            }
        }

        protected void OnMkvExtractProgressUpdated(int progress)
        {
            MkvExtractProgressUpdated?.Invoke(progress);
        }

        protected void OnMkvExtractTrackUpdated(string filename, string trackName)
        {
            MkvExtractTrackUpdated?.Invoke(filename, trackName);
        }

        private void ExtractMkvSegment(string argMKVFile, TrackParameter argParameter)
        {
            OnMkvExtractProgressUpdated(0);

            // check for existence of MKVExtract
            if (!File.Exists(_MKVExtractFilename))
            {
                throw new Exception($"Could not find {MKV_EXTRACT_FILENAME}!{Environment.NewLine}{_MKVExtractFilename}");
            }

            DataReceivedEventHandler handler = null;

            // Check the file version of the mkvextract
            if (_Version == null)
            {
                _Version = GetMKVExtractVersion();
            }

            // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
            // are now always written to files instead.
            if (argParameter.WriteOutputToFile && _Version.FileMajorPart < 17)
            {
                handler = myProcess_OutputDataReceived_WriteToFile;
            }
            else
            {
                handler = myProcess_OutputDataReceived;
            }

            ExecuteMkvExtract(argMKVFile, argParameter, handler);
        }

        private void ExecuteMkvExtract(string argMKVFile, TrackParameter argParameter, DataReceivedEventHandler argHandler)
        {
            using (Process myProcess = new Process())
            {
                ProcessStartInfo myProcessInfo = new ProcessStartInfo
                {
                    FileName = _MKVExtractFilename
                };

                // Check the file version of the mkvextract
                if (_Version == null)
                {
                    _Version = GetMKVExtractVersion();
                }

                string parameters = "";
                string LC_ALL = "";
                string LANG = "";
                string LC_MESSAGES = "";

                // Since MKVToolNix v9.7.0, start using the --gui-mode option
                if (_Version.FileMajorPart > 9 ||
                    (_Version.FileMajorPart == 9 && _Version.FileMinorPart >= 7))
                {
                    parameters = "--gui-mode";
                }
                else
                {
                    // Before MKVToolNix 9.7.0, the safest way to ensure English output on Linux is through the EnvironmentVariables
                    if (PlatformExtensions.IsOnLinux)
                    {
                        // Get the original values
                        LC_ALL = Environment.GetEnvironmentVariable("LC_ALL", EnvironmentVariableTarget.Process);
                        LANG = Environment.GetEnvironmentVariable("LANG", EnvironmentVariableTarget.Process);
                        LC_MESSAGES = Environment.GetEnvironmentVariable("LC_MESSAGES", EnvironmentVariableTarget.Process);

                        gMKVLogger.Log($"Detected Environment Variables: LC_ALL=\"{LC_ALL}\",LANG=\"{LANG}\",LC_MESSAGES=\"{LC_MESSAGES}\"");

                        // Set the english locale
                        Environment.SetEnvironmentVariable("LC_ALL", "en_US.UTF-8", EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LANG", "en_US.UTF-8", EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LC_MESSAGES", "en_US.UTF-8", EnvironmentVariableTarget.Process);

                        gMKVLogger.Log("Setting Environment Variables: LC_ALL=LANG=LC_MESSAGES=\"en_US.UTF-8\"");
                    }
                }

                // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                string options = "";
                if (PlatformExtensions.IsOnLinux)
                {
                    options = $"{parameters} --ui-language en_US {argParameter.Options}";
                }
                else
                {
                    options = $"{parameters} --ui-language en {argParameter.Options}";
                }

                // Since MKVToolNix v17.0, the syntax has changed
                if (_Version.FileMajorPart >= 17)
                {
                    // new Syntax
                    // mkvextract {source-filename} {mode1} [options] [extraction-spec1] [mode2] [options] [extraction-spec2] […] 
                    myProcessInfo.Arguments = string.Format(" \"{0}\" {1} {2} {3} ",
                        argMKVFile,
                        argParameter.ExtractMode.ToString(),
                        options,
                        string.IsNullOrWhiteSpace(argParameter.TrackOutput)
                        || argParameter.ExtractMode == MkvExtractModes.tracks
                        || argParameter.ExtractMode == MkvExtractModes.timecodes_v2
                        || argParameter.ExtractMode == MkvExtractModes.timestamps_v2
                        || argParameter.ExtractMode == MkvExtractModes.cues
                        || argParameter.ExtractMode == MkvExtractModes.attachments 
                            ? argParameter.TrackOutput 
                            : string.Format("\"{0}\"", argParameter.TrackOutput)
                    );
                }
                else
                {
                    // old Syntax
                    // mkvextract {mode} {source-filename} [options] [extraction-spec]
                    myProcessInfo.Arguments = string.Format(" {0} \"{1}\" {2} {3}",
                        argParameter.ExtractMode.ToString(),
                        argMKVFile,
                        options,
                        argParameter.TrackOutput
                    );
                }

                myProcessInfo.UseShellExecute = false;
                myProcessInfo.RedirectStandardOutput = true;
                myProcessInfo.StandardOutputEncoding = Encoding.UTF8;
                myProcessInfo.RedirectStandardError = true;
                myProcessInfo.StandardErrorEncoding = Encoding.UTF8;
                myProcessInfo.CreateNoWindow = true;
                myProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.StartInfo = myProcessInfo;

                Debug.WriteLine(myProcessInfo.Arguments);
                gMKVLogger.Log($"\"{_MKVExtractFilename}\" {myProcessInfo.Arguments}");

                // Start the mkvinfo process
                myProcess.Start();

                // Read the Standard output character by character
                myProcess.ReadStreamPerCharacter(argHandler);

                // Wait for the process to exit
                myProcess.WaitForExit();

                // Debug write the exit code
                string exitString = $"Exit code: {myProcess.ExitCode}";
                Debug.WriteLine(exitString);
                gMKVLogger.Log(exitString);

                // Check the exit code
                // ExitCode 1 is for warnings only, so ignore it
                if (myProcess.ExitCode > 1)
                {
                    // something went wrong!
                    throw new Exception(string.Format("Mkvextract exited with error code {0}!"
                        + Environment.NewLine + Environment.NewLine + "Errors reported:" + Environment.NewLine + "{1}",
                        myProcess.ExitCode, _ErrorBuilder.ToString()));
                }
                else if (myProcess.ExitCode < 0)
                {
                    // user aborted the current procedure!
                    throw new Exception("User aborted the current process!");
                }

                // Before MKVToolNix 9.7.0, the safest way to ensure English output on Linux is through the EnvironmentVariables
                if (PlatformExtensions.IsOnLinux)
                {
                    if (_Version.FileMajorPart < 9 ||
                        (_Version.FileMajorPart == 9 && _Version.FileMinorPart < 7))
                    {
                        // Reset the environment vairables to their original values
                        Environment.SetEnvironmentVariable("LC_ALL", LC_ALL, EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LANG", LANG, EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LC_MESSAGES", LC_MESSAGES, EnvironmentVariableTarget.Process);

                        gMKVLogger.Log($"Resetting Environment Variables: LC_ALL=\"{LC_ALL}\",LANG=\"{LANG}\",LC_MESSAGES=\"{LC_MESSAGES}\"");
                    }
                }
            }
        }

        public gMKVVersion GetMKVExtractVersion()
        {
            if (_Version != null)
            {
                return _Version;
            }

            // check for existence of mkvextract
            if (!File.Exists(_MKVExtractFilename))
            {
                throw new Exception($"Could not find {MKV_EXTRACT_FILENAME}!{Environment.NewLine}{_MKVExtractFilename}");
            }

            if (PlatformExtensions.IsOnLinux)
            {
                // When on Linux, we need to run mkvextract

                // Clear the mkvextract output
                _MKVExtractOutput.Length = 0;
                // Clear the error builder
                _ErrorBuilder.Length = 0;

                // Execute mkvextract
                List<OptionValue> options = new List<OptionValue>
                {
                    new OptionValue(MkvExtractGlobalOptions.version, "")
                };

                using (Process myProcess = new Process())
                {
                    // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                    // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                    options.Add(new OptionValue(MkvExtractGlobalOptions.ui_language, "en_US"));

                    ProcessStartInfo myProcessInfo = new ProcessStartInfo
                    {
                        FileName = _MKVExtractFilename,
                        Arguments = ConvertOptionValueListToString(options),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        RedirectStandardError = true,
                        StandardErrorEncoding = Encoding.UTF8,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    myProcess.StartInfo = myProcessInfo;

                    Debug.WriteLine(myProcessInfo.Arguments);
                    gMKVLogger.Log($"\"{_MKVExtractFilename}\" {myProcessInfo.Arguments}");

                    // Start the mkvextract process
                    myProcess.Start();

                    // Read the Standard output character by character
                    myProcess.ReadStreamPerCharacter(myProcess_OutputDataReceived);

                    // Wait for the process to exit
                    myProcess.WaitForExit();

                    // Debug write the exit code
                    string exitString = $"Exit code: {myProcess.ExitCode}";
                    Debug.WriteLine(exitString);
                    gMKVLogger.Log(exitString);

                    // Check the exit code
                    // ExitCode 1 is for warnings only, so ignore it
                    if (myProcess.ExitCode > 1)
                    {
                        // something went wrong!
                        throw new Exception(string.Format("Mkvmerge exited with error code {0}!" +
                            Environment.NewLine + Environment.NewLine + "Errors reported:" + Environment.NewLine + "{1}",
                            myProcess.ExitCode, _ErrorBuilder.ToString()));
                    }
                }

                // Parse version info
                ParseVersionOutput();

                // Clear the mkvextract output
                _MKVExtractOutput.Length = 0;
            }
            else
            {
                // When on Windows, we can use FileVersionInfo.GetVersionInfo
                var version = FileVersionInfo.GetVersionInfo(_MKVExtractFilename);
                _Version = new gMKVVersion()
                {
                    FileMajorPart = version.FileMajorPart,
                    FileMinorPart = version.FileMinorPart,
                    FilePrivatePart = version.FilePrivatePart
                };
            }

            if (_Version != null)
            {
                gMKVLogger.Log(string.Format("Detected mkvextract version: {0}.{1}.{2}",
                    _Version.FileMajorPart,
                    _Version.FileMinorPart,
                    _Version.FilePrivatePart
                ));
            }

            return _Version;
        }

        void myProcess_OutputDataReceived_WriteToFile(object sender, DataReceivedEventArgs e)
        {
            // check for user abort
            if (Abort)
            {
                ((Process)sender).Kill();
                Abort = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                // add the line to the output stringbuilder
                _OutputFileWriter.WriteLine(e.Data);

                // check for progress (in gui-mode)
                if (e.Data.Contains("#GUI#progress"))
                {
                    OnMkvExtractProgressUpdated(Convert.ToInt32(e.Data.Substring(e.Data.IndexOf(" ") + 1, e.Data.IndexOf("%") - e.Data.IndexOf(" ") - 1)));
                }
                // check for progress
                else if (e.Data.Contains("Progress:"))
                {
                    OnMkvExtractProgressUpdated(Convert.ToInt32(e.Data.Substring(e.Data.IndexOf(":") + 1, e.Data.IndexOf("%") - e.Data.IndexOf(":") - 1)));
                }
                else if (e.Data.Contains("#GUI#error"))
                {
                    _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(" ") + 1).Trim());
                }
                // check for errors
                else if (e.Data.Contains("Error:"))
                {
                    _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(":") + 1).Trim());
                }

                // debug write the output line
                Debug.WriteLine(e.Data);

                // log the output
                gMKVLogger.Log(e.Data);
            }
        }

        void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // check for user abort
            if (Abort)
            {
                ((Process)sender).Kill();
                Abort = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                // add the line to the output stringbuilder
                _MKVExtractOutput.AppendLine(e.Data);

                // check for progress (in gui-mode)
                if (e.Data.Contains("#GUI#progress"))
                {
                    OnMkvExtractProgressUpdated(Convert.ToInt32(e.Data.Substring(e.Data.IndexOf(" ") + 1, e.Data.IndexOf("%") - e.Data.IndexOf(" ") - 1)));
                }
                // check for progress
                else if (e.Data.Contains("Progress:"))
                {
                    OnMkvExtractProgressUpdated(Convert.ToInt32(e.Data.Substring(e.Data.IndexOf(":") + 1, e.Data.IndexOf("%") - e.Data.IndexOf(":") - 1)));
                }
                else if (e.Data.Contains("#GUI#error"))
                {
                    _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(" ") + 1).Trim());
                }
                // check for errors
                else if (e.Data.Contains("Error:"))
                {
                    _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(":") + 1).Trim());
                }

                // debug write the output line
                Debug.WriteLine(e.Data);

                // log the output
                gMKVLogger.Log(e.Data);
            }
        }

        private void ParseVersionOutput()
        {
            _Version = gMKVVersionParser.ParseVersionOutput(_MKVExtractOutput?.ToString());
        }

        public static string ReplaceFilenamePlaceholders(gMKVSegment argSeg, string argMKVFile, string argFilenamePattern)
        {
            string mkvFilenameNoExt = Path.GetFileNameWithoutExtension(argMKVFile);
            string mkvFilename = Path.GetFileName(argMKVFile);
            string finalFilename = argFilenamePattern;

            // Common placeholders
            finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.FilenameNoExt, mkvFilenameNoExt);
            finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.Filename, mkvFilename);

            // Track placeholders
            if (argSeg is gMKVTrack track)
            {
                // Common Track placeholders
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackNumber, track.TrackNumber.ToString());
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackNumber_0, track.TrackNumber.ToString("0"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackNumber_00, track.TrackNumber.ToString("00"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackNumber_000, track.TrackNumber.ToString("000"));

                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackID, track.TrackID.ToString());
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackID_0, track.TrackID.ToString("0"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackID_00, track.TrackID.ToString("00"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackID_000, track.TrackID.ToString("000"));

                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackName, track.TrackName);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackLanguage, track.Language);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackLanguageIetf, track.LanguageIetf);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackCodecID, track.CodecID);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackCodecPrivate, track.CodecPrivate);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackDelay, track.Delay.ToString());
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackEffectiveDelay, track.EffectiveDelay.ToString());

                // Video Track placeholders
                if (track.TrackType == MkvTrackType.video)
                {
                    finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.VideoPixelWidth, track.VideoPixelWidth.ToString());
                    finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.VideoPixelHeight, track.VideoPixelHeight.ToString());
                }

                // Audio Track placeholders
                else if (track.TrackType == MkvTrackType.audio)
                {
                    finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AudioSamplingFrequency, track.AudioSamplingFrequency.ToString());
                    finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AudioChannels, track.AudioChannels.ToString());
                }
            }

            // Attachment placeholders
            else if (argSeg is gMKVAttachment attachment)
            {
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentID, attachment.ID.ToString());
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentID_0, attachment.ID.ToString("0"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentID_00, attachment.ID.ToString("00"));
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentID_000, attachment.ID.ToString("000"));

                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentFilename, attachment.Filename);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentMimeType, attachment.MimeType);
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.AttachmentFileSize, attachment.FileSize);
            }

            // Replace illegal filename characters with '_'
            if (finalFilename.AsEnumerable().Any(c => _invalidFilenameChars.Contains(c)))
            {
                finalFilename = string.Join("_", finalFilename.Split(_invalidFilenameChars));
            }

            // Replace directory separator
            finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.DirectorySeparator, _directorySeparator);

            // Final Trim to avoid having filenames like "test .mkv"
            return finalFilename.Trim();
        }

        public static string GetOutputFilename(
            gMKVSegment argSeg
            , string argOutputDirectory
            , string argMKVFile
            , gMKVExtractFilenamePatterns argFilenamePatterns
            , MkvExtractModes argMkvExtractMode
            , MkvChapterTypes argMkvChapterType = MkvChapterTypes.XML)
        {
            string outputFilename = "";
            string outputFileExtension = "";
            string argMkvFilenameNoExt = Path.GetFileNameWithoutExtension(argMKVFile);
            string replacedFilePattern = "";

            string outputDirectory = argOutputDirectory;

            switch (argMkvExtractMode)
            {
                case MkvExtractModes.tracks:
                    if (!(argSeg is gMKVTrack segTrack))
                    {
                        throw new Exception("Called GetOutputFilename without track!");
                    }

                    // check the track's type in order to get the output file's extension and the delay for audio tracks
                    switch (segTrack.TrackType)
                    {
                        case MkvTrackType.video:
                            // get the extension of the output via the CODEC_ID of the track
                            outputFileExtension = GetVideoFileExtensionFromCodecID(segTrack);
                            replacedFilePattern = ReplaceFilenamePlaceholders(argSeg, argMKVFile, argFilenamePatterns.VideoTrackFilenamePattern);
                            break;
                        case MkvTrackType.audio:
                            // get the extension of the output via the CODEC_ID of the track
                            outputFileExtension = GetAudioFileExtensionFromCodecID(segTrack);
                            replacedFilePattern = ReplaceFilenamePlaceholders(argSeg, argMKVFile, argFilenamePatterns.AudioTrackFilenamePattern);
                            break;
                        case MkvTrackType.subtitles:
                            // get the extension of the output via the CODEC_ID of the track
                            outputFileExtension = GetSubtitleFileExtensionFromCodecID(segTrack);
                            replacedFilePattern = ReplaceFilenamePlaceholders(argSeg, argMKVFile, argFilenamePatterns.SubtitleTrackFilenamePattern);
                            break;
                        default:
                            break;
                    }
                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}.{1}",
                            replacedFilePattern
                            , outputFileExtension)
                    );
                    break;
                case MkvExtractModes.tags:
                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}_tags.xml", argMkvFilenameNoExt));
                    break;
                case MkvExtractModes.attachments:
                    if (!(argSeg is gMKVAttachment))
                    {
                        throw new Exception("Called GetOutputFilename without attachment!");
                    }
                    outputFilename = Path.Combine(
                        outputDirectory,
                        ReplaceFilenamePlaceholders(argSeg, argMKVFile, argFilenamePatterns.AttachmentFilenamePattern)
                    );
                    break;
                case MkvExtractModes.chapters:
                    // check the chapter's type to determine the output file's extension and options
                    switch (argMkvChapterType)
                    {
                        case MkvChapterTypes.XML:
                            outputFileExtension = "xml";
                            break;
                        case MkvChapterTypes.OGM:
                            outputFileExtension = "ogm.txt";
                            break;
                        case MkvChapterTypes.CUE:
                            outputFileExtension = "cue";
                            break;
                        case MkvChapterTypes.PBF:
                            outputFileExtension = "pbf";
                            break;
                        default:
                            break;
                    }
                    outputFilename = Path.Combine(
                            outputDirectory,
                            string.Format("{0}.{1}",
                                ReplaceFilenamePlaceholders(argSeg, argMKVFile, argFilenamePatterns.ChapterFilenamePattern),
                                outputFileExtension)
                    );
                    break;
                case MkvExtractModes.cuesheet:
                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}_cuesheet.cue", argMkvFilenameNoExt));
                    break;
                case MkvExtractModes.timecodes_v2:
                case MkvExtractModes.timestamps_v2:
                    if (!(argSeg is gMKVTrack timeTrack))
                    {
                        throw new Exception("Called GetOutputFilename without track/timestamps!");
                    }
                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}_track{1}_[{2}].tc.txt",
                            argMkvFilenameNoExt,
                            timeTrack.TrackNumber,
                            timeTrack.Language));
                    break;
                case MkvExtractModes.cues:
                    if (!(argSeg is gMKVTrack cueTrack))
                    {
                        throw new Exception("Called GetOutputFilename without track/cues!");
                    }
                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}_track{1}_[{2}].cue",
                            argMkvFilenameNoExt,
                            cueTrack.TrackNumber,
                            cueTrack.Language));
                    break;
                default:
                    break;
            }

            return outputFilename.GetOutputFilename();
        }

        private static string GetVideoFileExtensionFromCodecID(gMKVTrack argTrack)
        {
            string outputFileExtension;
            string codecIdUpperCase = argTrack.CodecID.ToUpper();
            if (codecIdUpperCase.Contains("V_MS/VFW/FOURCC"))
            {
                outputFileExtension = "avi";
            }
            else if (codecIdUpperCase.Contains("V_UNCOMPRESSED"))
            {
                outputFileExtension = "raw";
            }
            else if (codecIdUpperCase.Contains("V_MPEG4/ISO/"))
            {
                outputFileExtension = "avc";
            }
            else if (codecIdUpperCase.Contains("V_MPEGH/ISO/HEVC"))
            {
                outputFileExtension = "hevc";
            }
            else if (codecIdUpperCase.Contains("V_AV1"))
            {
                outputFileExtension = "av1";
            }
            else if (codecIdUpperCase.Contains("V_MPEG4/MS/V3"))
            {
                outputFileExtension = "mp4";
            }
            else if (codecIdUpperCase.Contains("V_MPEG1"))
            {
                outputFileExtension = "mpg";
            }
            else if (codecIdUpperCase.Contains("V_MPEG2"))
            {
                outputFileExtension = "mpg";
            }
            else if (codecIdUpperCase.Contains("V_REAL/"))
            {
                outputFileExtension = "rm";
            }
            else if (codecIdUpperCase.Contains("V_QUICKTIME"))
            {
                outputFileExtension = "mov";
            }
            else if (codecIdUpperCase.Contains("V_THEORA"))
            {
                outputFileExtension = "ogv";
            }
            else if (codecIdUpperCase.Contains("V_PRORES"))
            {
                outputFileExtension = "mov";
            }
            else if (codecIdUpperCase.Contains("V_VP"))
            {
                outputFileExtension = "ivf";
            }
            else if (codecIdUpperCase.Contains("V_DIRAC"))
            {
                outputFileExtension = "drc";
            }
            else
            {
                outputFileExtension = "mkv";
            }
            return outputFileExtension;
        }

        private static string GetAudioFileExtensionFromCodecID(gMKVTrack argTrack)
        {
            string outputFileExtension;
            string codecIdUpperCase = argTrack.CodecID.ToUpper();
            if (codecIdUpperCase.Contains("A_MPEG/L3"))
            {
                outputFileExtension = "mp3";
            }
            else if (codecIdUpperCase.Contains("A_MPEG/L2"))
            {
                outputFileExtension = "mp2";
            }
            else if (codecIdUpperCase.Contains("A_MPEG/L1"))
            {
                outputFileExtension = "mpa";
            }
            else if (codecIdUpperCase.Contains("A_PCM"))
            {
                outputFileExtension = "wav";
            }
            else if (codecIdUpperCase.Contains("A_MPC"))
            {
                outputFileExtension = "mpc";
            }
            else if (codecIdUpperCase.Contains("A_AC3"))
            {
                outputFileExtension = "ac3";
            }
            else if (codecIdUpperCase.Contains("A_EAC3"))
            {
                outputFileExtension = "eac3";
            }
            else if (codecIdUpperCase.Contains("A_ALAC"))
            {
                outputFileExtension = "caf";
            }
            else if (codecIdUpperCase.Contains("A_DTS"))
            {
                outputFileExtension = "dts";
            }
            else if (codecIdUpperCase.Contains("A_VORBIS"))
            {
                outputFileExtension = "ogg";
            }
            else if (codecIdUpperCase.Contains("A_FLAC"))
            {
                outputFileExtension = "flac";
            }
            else if (codecIdUpperCase.Contains("A_REAL"))
            {
                outputFileExtension = "ra";
            }
            else if (codecIdUpperCase.Contains("A_MS/ACM"))
            {
                outputFileExtension = "wav";
            }
            else if (codecIdUpperCase.Contains("A_AAC"))
            {
                outputFileExtension = "aac";
            }
            else if (codecIdUpperCase.Contains("A_QUICKTIME"))
            {
                outputFileExtension = "mov";
            }
            else if (codecIdUpperCase.Contains("A_TRUEHD"))
            {
                outputFileExtension = "thd";
            }
            else if (codecIdUpperCase.Contains("A_TTA1"))
            {
                outputFileExtension = "tta";
            }
            else if (codecIdUpperCase.Contains("A_WAVPACK4"))
            {
                outputFileExtension = "wv";
            }
            else if (codecIdUpperCase.Contains("A_OPUS"))
            {
                outputFileExtension = "opus";
            }
            else if (codecIdUpperCase.Contains("A_MLP"))
            {
                outputFileExtension = "mlp";
            }
            else
            {
                outputFileExtension = "mka";
            }
            return outputFileExtension;
        }

        private static string GetSubtitleFileExtensionFromCodecID(gMKVTrack argTrack)
        {
            string outputFileExtension;
            string codecIdUpperCase = argTrack.CodecID.ToUpper();
            if (codecIdUpperCase.Contains("S_TEXT/UTF8"))
            {
                outputFileExtension = "srt";
            }
            else if (codecIdUpperCase.Contains("S_TEXT/ASCII"))
            {
                outputFileExtension = "srt";
            }
            else if (codecIdUpperCase.Contains("S_TEXT/SSA"))
            {
                outputFileExtension = "ass";
            }
            else if (codecIdUpperCase.Contains("S_TEXT/ASS"))
            {
                outputFileExtension = "ass";
            }
            else if (codecIdUpperCase.Contains("S_TEXT/USF"))
            {
                outputFileExtension = "usf";
            }
            else if (codecIdUpperCase.Contains("S_TEXT/WEBVTT"))
            {
                outputFileExtension = "webvtt";
            }
            else if (codecIdUpperCase.Contains("S_IMAGE/BMP"))
            {
                outputFileExtension = "sub";
            }
            else if (codecIdUpperCase.Contains("S_VOBSUB"))
            {
                outputFileExtension = "sub";
            }
            else if (codecIdUpperCase.Contains("S_DVBSUB"))
            {
                outputFileExtension = "dvbsub";
            }
            else if (codecIdUpperCase.Contains("S_HDMV/PGS"))
            {
                outputFileExtension = "sup";
            }
            else if (codecIdUpperCase.Contains("S_HDMV/TEXTST"))
            {
                outputFileExtension = "textst";
            }
            else if (codecIdUpperCase.Contains("S_KATE"))
            {
                outputFileExtension = "ogg";
            }
            else
            {
                outputFileExtension = "sub";
            }
            return outputFileExtension;
        }

        private static string ConvertOptionValueListToString(List<OptionValue> listOptionValue)
        {
            StringBuilder optionString = new StringBuilder();
            foreach (OptionValue optVal in listOptionValue)
            {
                optionString.AppendFormat(" {0} {1}", ConvertEnumOptionToStringOption(optVal.Option), optVal.Parameter);
            }
            return optionString.ToString();
        }

        private static string ConvertEnumOptionToStringOption(MkvExtractGlobalOptions enumOption)
        {
            return $"--{enumOption.ToString().Replace("_", "-")}";
        }
    }
}
