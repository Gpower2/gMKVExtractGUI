using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using gMKVToolNix.Log;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvExtract
{
    public delegate void MkvExtractProgressUpdatedEventHandler(int progress);
    public delegate void MkvExtractTrackUpdatedEventHandler(string filename, string trackName);

    public class gMKVExtract
    {
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
        private readonly gMKVVersion _Version = null;

        public event MkvExtractProgressUpdatedEventHandler MkvExtractProgressUpdated;
        public event MkvExtractTrackUpdatedEventHandler MkvExtractTrackUpdated;

        private Exception _ThreadedException = null;
        public Exception ThreadedException { get { return _ThreadedException; } }

        public bool Abort { get; set; }

        public bool AbortAll { get; set; }

        public gMKVExtract(string mkvToolnixPath)
        {
            _MKVToolnixPath = mkvToolnixPath;
            _MKVExtractFilename = Path.Combine(_MKVToolnixPath, MKV_EXTRACT_FILENAME);

            // check for existence of mkvextract
            if (!File.Exists(_MKVExtractFilename))
            {
                throw new Exception($"Could not find {MKV_EXTRACT_FILENAME}!{Environment.NewLine}{_MKVExtractFilename}");
            }

            _Version = GetMKVExtractVersion();
            
            if (_Version != null)
            {
                gMKVLogger.Log(string.Format("Detected mkvextract version: {0}.{1}.{2}",
                    _Version.FileMajorPart,
                    _Version.FileMinorPart,
                    _Version.FilePrivatePart
                ));
            }
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
                    parameters.FilenamePatterns,
                    parameters.DisableBomForTextFiles,
                    parameters.OverwriteExistingFile,
                    parameters.UseRawExtractionMode,
                    parameters.UseFullRawExtractionMode
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMKVSegments(
            string argMKVFile
            , List<gMKVSegment> argMKVSegmentsToExtract
            , string argOutputDirectory
            , MkvChapterTypes argChapterType
            , TimecodesExtractionMode argTimecodesExtractionMode
            , CuesExtractionMode argCueExtractionMode
            , gMKVExtractFilenamePatterns argFilenamePatterns
            , bool argDisableBomForTextFiles
            , bool argOverwriteExistingFile
            , bool argUseRawExtractionMode
            , bool argUseFullRawExtractionMode
        )
        {
            Abort = false;
            AbortAll = false;
            List<string> errors = new List<string>();

            // Analyze the MKV segments and get the initial parameters
            List<TrackParameter> initialParameters = new List<TrackParameter>();
            foreach (gMKVSegment seg in argMKVSegmentsToExtract)
            {
                if (AbortAll)
                {
                    errors.Add("User aborted all the processes!");
                    break;
                }

                try
                {
                    initialParameters.AddRange(
                        GetTrackParameters(
                            seg,
                            argMKVFile, 
                            argOutputDirectory, 
                            argChapterType, 
                            argTimecodesExtractionMode, 
                            argCueExtractionMode, 
                            argFilenamePatterns,
                            argDisableBomForTextFiles,
                            argOverwriteExistingFile,
                            argUseRawExtractionMode,
                            argUseFullRawExtractionMode,
                            _Version));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    errors.Add($"Segment: {seg}{Environment.NewLine}Exception: {ex.Message}{Environment.NewLine}");
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
                    errors.Add("User aborted all the processes!");
                    break;
                }

                StreamWriter outputFileWriter = null;
                try
                {
                    if (finalPar.WriteOutputToFile)
                    {
                        outputFileWriter = new StreamWriter(finalPar.OutputFilename, false, new UTF8Encoding(false, true));
                    }

                    OnMkvExtractTrackUpdated(argMKVFile, finalPar.ExtractMode.ToString());
                    ExtractMkvSegment(argMKVFile, finalPar, errors, outputFileWriter);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    errors.Add($"Track output: {finalPar.TrackOutput}{Environment.NewLine}Exception: {ex.Message}{Environment.NewLine}");
                }
                finally
                {
                    if (outputFileWriter != null)
                    {
                        outputFileWriter.Close();
                        outputFileWriter = null;
                    }

                    try
                    {
                        // If we have chapters with CUE or PBF format, then we read the XML chapters and convert it to CUE or PBF format
                        if (finalPar.ExtractMode == MkvExtractModes.chapters)
                        {
                            // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
                            // are now always written to files instead.
                            string outputFile = _Version.FileMajorPart >= 17 
                                ? finalPar.TrackOutput 
                                : finalPar.OutputFilename;

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
                                    string cueContent = chapters.ConvertChaptersToCueFile(argMKVFile);

                                    using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
                                    {
                                        sw.Write(cueContent);
                                    }
                                }
                                else if (outputExtension.Equals("pbf"))
                                {
                                    string pbfContent = chapters.ConvertChaptersToPbf(argMKVFile);

                                    using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
                                    {
                                        sw.Write(pbfContent);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc);
                        errors.Add($"Track output: {finalPar.TrackOutput}{Environment.NewLine}Exception: {exc.Message}{Environment.NewLine}");
                    }
                }
            }

            // check for errors
            if (errors.Count > 0)
            {
                throw new Exception(string.Join(Environment.NewLine, errors));
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
                    parameters.FilenamePatterns,
                    parameters.DisableBomForTextFiles,
                    parameters.OverwriteExistingFile,
                    parameters.UseRawExtractionMode,
                    parameters.UseFullRawExtractionMode
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
                    parameters.FilenamePatterns,
                    parameters.DisableBomForTextFiles,
                    parameters.OverwriteExistingFile,
                    parameters.UseRawExtractionMode,
                    parameters.UseFullRawExtractionMode
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
                    parameters.FilenamePatterns,
                    parameters.DisableBomForTextFiles,
                    parameters.OverwriteExistingFile,
                    parameters.UseRawExtractionMode,
                    parameters.UseFullRawExtractionMode
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMkvCuesheet(
            string argMKVFile, 
            string argOutputDirectory, 
            gMKVExtractFilenamePatterns argFilenamePatterns,
            bool argDisableBomForTextFiles,
            bool argOverwriteExistingFile,
            bool argUseRawExtractionMode,
            bool argUseFullRawExtractionMode)
        {
            Abort = false;
            AbortAll = false;

            string cueFile = gMKVExtractExtensions.GetOutputFilename(
                null, 
                argOutputDirectory, 
                argMKVFile, 
                argFilenamePatterns,
                argOverwriteExistingFile,
                MkvExtractModes.cuesheet);

            List<string> errors = new List<string>();
            StreamWriter outputFileWriter = null;
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Cue Sheet");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
                // are now always written to files instead.
                if (_Version.FileMajorPart < 17)
                {
                    outputFileWriter = new StreamWriter(cueFile, false, new UTF8Encoding(false, true));
                }

                ExtractMkvSegment(
                    argMKVFile
                    , new TrackParameter(
                        MkvExtractModes.cuesheet
                        , ""
                        , _Version.FileMajorPart >= 17 ? cueFile : ""
                        , _Version.FileMajorPart < 17
                        , argDisableBomForTextFiles
                        , argUseRawExtractionMode
                        , argUseFullRawExtractionMode
                        , _Version.FileMajorPart >= 17 ? "" : cueFile
                    )
                    , errors
                    , outputFileWriter
               );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (outputFileWriter != null)
                {
                    outputFileWriter.Close();
                    outputFileWriter = null;
                }
            }

            // check for errors
            if (errors.Count > 0)
            {
                throw new Exception(string.Join(Environment.NewLine, errors));
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
                    parameters.FilenamePatterns,
                    parameters.DisableBomForTextFiles,
                    parameters.OverwriteExistingFile,
                    parameters.UseRawExtractionMode,
                    parameters.UseFullRawExtractionMode
                );
            }
            catch (Exception ex)
            {
                _ThreadedException = ex;
            }
        }

        public void ExtractMkvTags(
            string argMKVFile, 
            string argOutputDirectory, 
            gMKVExtractFilenamePatterns argFilenamePatterns,
            bool argDisableBomForTextFiles,
            bool argOverwriteExistingFile,
            bool argUseRawExtractionMode,
            bool argUseFullRawExtractionMode)
        {
            Abort = false;
            AbortAll = false;

            string tagsFile = gMKVExtractExtensions.GetOutputFilename(
                null, 
                argOutputDirectory, 
                argMKVFile, 
                argFilenamePatterns,
                argOverwriteExistingFile,
                MkvExtractModes.tags);

            List<string> errors = new List<string>();
            StreamWriter outputFileWriter = null;
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Tags");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
                // are now always written to files instead.
                if (_Version.FileMajorPart < 17)
                {
                    outputFileWriter = new StreamWriter(tagsFile, false, new UTF8Encoding(false, true));
                }

                ExtractMkvSegment(
                    argMKVFile
                    , new TrackParameter(
                        MkvExtractModes.tags
                        , ""
                        , _Version.FileMajorPart >= 17 ? tagsFile : ""
                        , _Version.FileMajorPart < 17
                        , argDisableBomForTextFiles
                        , argUseRawExtractionMode
                        , argUseFullRawExtractionMode
                        , _Version.FileMajorPart >= 17 ? "" : tagsFile
                    )
                    , errors
                    , outputFileWriter
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (outputFileWriter != null)
                {
                    outputFileWriter.Close();
                    outputFileWriter = null;
                }
            }

            // check for errors
            if (errors.Count > 0)
            {
                throw new Exception(string.Join(Environment.NewLine, errors));
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

        public static List<TrackParameter> GetTrackParameters(
            gMKVSegment argSeg
            , string argMKVFile
            , string argOutputDirectory
            , MkvChapterTypes argChapterType
            , TimecodesExtractionMode argTimecodesExtractionMode
            , CuesExtractionMode argCueExtractionMode
            , gMKVExtractFilenamePatterns argFilenamePatterns
            , bool argDisableBomForTextFiles
            , bool argOverwriteExistingFile
            , bool argUseRawExtractionMode
            , bool argUseFullRawExtractionMode
            , gMKVVersion version)
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
                        version.FileMajorPart >= 17 ? MkvExtractModes.timestamps_v2 : MkvExtractModes.timecodes_v2,
                        "",
                        string.Format("{0}:\"{1}\"",
                            track.TrackID,
                            argSeg.GetOutputFilename(
                                argOutputDirectory, 
                                argMKVFile, 
                                argFilenamePatterns,
                                argOverwriteExistingFile,
                                MkvExtractModes.timestamps_v2)
                        ),
                        false,
                        argDisableBomForTextFiles,
                        argUseRawExtractionMode,
                        argUseFullRawExtractionMode,
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
                            argSeg.GetOutputFilename(
                                argOutputDirectory, 
                                argMKVFile, 
                                argFilenamePatterns,
                                argOverwriteExistingFile,
                                MkvExtractModes.cues)
                        ),
                        false,
                        argDisableBomForTextFiles,
                        argUseRawExtractionMode,
                        argUseFullRawExtractionMode,
                        ""
                    ));
                }

                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                        (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.NoCues)
                        ||
                        (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    ||
                    (
                        argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues
                    )
                )
                {
                    // add the parameter for extracting the track
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.tracks,
                        "",
                        string.Format("{0}:\"{1}\"",
                            track.TrackID,
                            argSeg.GetOutputFilename(
                                argOutputDirectory, 
                                argMKVFile, 
                                argFilenamePatterns,
                                argOverwriteExistingFile,
                                MkvExtractModes.tracks)
                        ),
                        false,
                        argDisableBomForTextFiles,
                        argUseRawExtractionMode,
                        argUseFullRawExtractionMode,
                        ""
                    ));
                }
            }
            else if (argSeg is gMKVAttachment attachment)
            {
                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                        (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.NoCues)
                        ||
                        (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    ||
                    (
                        argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues
                    )
                )
                {
                    // add the parameter for extracting the attachment
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.attachments,
                        "",
                        string.Format("{0}:\"{1}\"",
                            attachment.ID,
                            argSeg.GetOutputFilename(
                                argOutputDirectory, 
                                argMKVFile, 
                                argFilenamePatterns,
                                argOverwriteExistingFile,
                                MkvExtractModes.attachments)
                        ),
                        false,
                        argDisableBomForTextFiles,
                        argUseRawExtractionMode,
                        argUseFullRawExtractionMode,
                        ""
                    ));
                }
            }
            else if (argSeg is gMKVChapter)
            {
                // check if the mode requires the extraction of the segment itself
                if (
                    !(
                        (argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.NoCues)
                        ||
                        (argTimecodesExtractionMode == TimecodesExtractionMode.NoTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                    ||
                    (
                        argTimecodesExtractionMode == TimecodesExtractionMode.OnlyTimecodes
                        && argCueExtractionMode == CuesExtractionMode.OnlyCues)
                    )
                {
                    string options = "";

                    // check the chapter's type to determine the output file's extension and options
                    if (argChapterType == MkvChapterTypes.OGM)
                    {
                        options = "--simple";
                    }

                    string chapterFile = argSeg.GetOutputFilename(
                        argOutputDirectory, 
                        argMKVFile, 
                        argFilenamePatterns, 
                        argOverwriteExistingFile,
                        MkvExtractModes.chapters,
                        argChapterType);

                    // add the parameter for extracting the chapters
                    // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.chapters,
                        options,
                        version.FileMajorPart >= 17 ? chapterFile : "",
                        version.FileMajorPart < 17,
                        argDisableBomForTextFiles,
                        argUseRawExtractionMode,
                        argUseFullRawExtractionMode,
                        version.FileMajorPart >= 17 ? "" : chapterFile
                    ));
                }
            }

            return trackParameterList;
        }

        private void ExtractMkvSegment(
            string argMKVFile, 
            TrackParameter argParameter, 
            List<string> errors, 
            StreamWriter argOutputFileWriter)
        {
            OnMkvExtractProgressUpdated(0);

            Action<Process, string> handler = null;

            // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
            // are now always written to files instead.
            if (argParameter.WriteOutputToFile && _Version.FileMajorPart < 17)
            {
                handler = CreateProcessOutputHandlerFactory(
                    (string s) => argOutputFileWriter.WriteLine(s),
                    (string s) => errors.Add(s));
            }
            else
            {
                handler = CreateProcessOutputHandlerFactory(
                    (string s) => { },
                    (string s) => errors.Add(s));
            }

            ExecuteMkvExtract(argMKVFile, argParameter, handler, errors);
        }

        private void ExecuteMkvExtract(
            string argMKVFile, 
            TrackParameter argParameter, 
            Action<Process, string> argHandler, 
            List<string> errors)
        {
            using (Process myProcess = new Process())
            {
                ProcessStartInfo myProcessInfo = new ProcessStartInfo
                {
                    FileName = _MKVExtractFilename
                };

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

                // Since MKVToolNix v96.0, we can disable writing the BOM for output text files
                // https://codeberg.org/mbunkus/mkvtoolnix/issues/6166
                if (_Version.FileMajorPart >= 96)
                {
                    if (argParameter.DisableBomForTextFiles)
                    {
                        parameters = $"{parameters} --no-bom";
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

                // Check for `full-raw` and `raw` extraction modes
                // This is only valid for track extraction
                // Full raw has precedence over raw
                if (argParameter.ExtractMode == MkvExtractModes.tracks)
                {
                    if (argParameter.UseFullRawExtractionMode)
                    {
                        options = $"{options} --fullraw";
                    }
                    else if (argParameter.UseRawExtractionMode)
                    {
                        options = $"{options} --raw";
                    }
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
                        myProcess.ExitCode, string.Join(Environment.NewLine, errors)));
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

            if (PlatformExtensions.IsOnLinux)
            {
                // When on Linux, we need to run mkvextract

                // Execute mkvextract
                List<OptionValue<MkvExtractGlobalOptions>> options = new List<OptionValue<MkvExtractGlobalOptions>>
                {
                    new OptionValue<MkvExtractGlobalOptions>(MkvExtractGlobalOptions.version, "")
                };

                List<string> versionOutputLines = new List<string>();
                List<string> errors = new List<string>();

                using (Process myProcess = new Process())
                {
                    // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                    // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                    options.Add(new OptionValue<MkvExtractGlobalOptions>(MkvExtractGlobalOptions.ui_language, "en_US"));

                    ProcessStartInfo myProcessInfo = new ProcessStartInfo
                    {
                        FileName = _MKVExtractFilename,
                        Arguments = options.ConvertOptionValueListToString(),
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
                    myProcess.ReadStreamPerCharacter(CreateProcessOutputHandlerFactory(
                        (string line) => versionOutputLines.Add(line),
                        (string error) => errors.Add(error)));

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
                            myProcess.ExitCode, string.Join(Environment.NewLine, errors)));
                    }
                }

                // Parse version info
                return gMKVVersionParser.ParseVersionOutput(versionOutputLines);
            }
            else
            {
                // When on Windows, we can use FileVersionInfo.GetVersionInfo
                var version = FileVersionInfo.GetVersionInfo(_MKVExtractFilename);
                return new gMKVVersion()
                {
                    FileMajorPart = version.FileMajorPart,
                    FileMinorPart = version.FileMinorPart,
                    FilePrivatePart = version.FilePrivatePart
                };
            }
        }

        /// <summary>
        /// Factory that creates a process output handler with a custom output action.
        /// </summary>
        /// <param name="outputAction">The action to perform with the received line of text.</param>
        /// <param name="errorAction">The action to perform with error lines.</param>
        /// <returns>A new Action<Process, string> that can be used as a handler.</returns>
        public Action<Process, string> CreateProcessOutputHandlerFactory(Action<string> outputAction, Action<string> errorAction)
        {
            // Return a new lambda expression that matches the Action<Process, string> signature.
            // This lambda "closes over" the outputAction parameter.
            return (process, line) => ProcessLineReceivedHandler(process, line, outputAction, errorAction);
        }

        private void ProcessLineReceivedHandler(
            Process senderProcess, 
            string lineReceived, 
            Action<string> outputAction,
            Action<string> errorAction)
        {
            // check for user abort
            if (Abort)
            {
                senderProcess.Kill();
                Abort = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(lineReceived))
            {
                return;
            }

            // debug write the output line
            Debug.WriteLine(lineReceived);

            // log the output
            gMKVLogger.Log(lineReceived);

            // Call the outputAction with the received line
            outputAction(lineReceived);

            // check for progress (in gui-mode)
            if (lineReceived.Contains("#GUI#progress"))
            {
                OnMkvExtractProgressUpdated(Convert.ToInt32(lineReceived.Substring(lineReceived.IndexOf(" ") + 1, lineReceived.IndexOf("%") - lineReceived.IndexOf(" ") - 1)));
            }
            // check for progress
            else if (lineReceived.Contains("Progress:"))
            {
                OnMkvExtractProgressUpdated(Convert.ToInt32(lineReceived.Substring(lineReceived.IndexOf(":") + 1, lineReceived.IndexOf("%") - lineReceived.IndexOf(":") - 1)));
            }
            else if (lineReceived.Contains("#GUI#error"))
            {
                errorAction(lineReceived.Substring(lineReceived.IndexOf(" ") + 1).Trim());
            }
            // check for errors
            else if (lineReceived.Contains("Error:"))
            {
                errorAction(lineReceived.Substring(lineReceived.IndexOf(":") + 1).Trim());
            }
        }
    }
}
