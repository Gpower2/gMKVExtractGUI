using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    parameters.FilenamePatterns
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
                    initialParameters.AddRange(
                        GetTrackParameters(
                            seg,
                            argMKVFile, 
                            argOutputDirectory, 
                            argChapterType, 
                            argTimecodesExtractionMode, 
                            argCueExtractionMode, 
                            argFilenamePatterns,
                            _Version));
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

            string cueFile = gMKVExtractExtensions.GetOutputFilename(null, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.cuesheet);
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Cue Sheet");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
                // are now always written to files instead.
                if (_Version.FileMajorPart < 17)
                {
                    _OutputFileWriter = new StreamWriter(cueFile, false, new UTF8Encoding(false, true));
                }

                ExtractMkvSegment(
                    argMKVFile
                     , new TrackParameter(
                        MkvExtractModes.cuesheet
                        , ""
                        , _Version.FileMajorPart >= 17 ? cueFile : ""
                        , _Version.FileMajorPart < 17
                        , _Version.FileMajorPart >= 17 ? "" : cueFile
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

            string tagsFile = gMKVExtractExtensions.GetOutputFilename(null, argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.tags);
            try
            {
                OnMkvExtractTrackUpdated(argMKVFile, "Tags");
                // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets)
                // are now always written to files instead.
                if (_Version.FileMajorPart < 17)
                {
                    _OutputFileWriter = new StreamWriter(tagsFile, false, new UTF8Encoding(false, true));
                }

                ExtractMkvSegment(
                    argMKVFile
                    , new TrackParameter(
                        MkvExtractModes.tags
                        , ""
                        , _Version.FileMajorPart >= 17 ? tagsFile : ""
                        , _Version.FileMajorPart < 17
                        , _Version.FileMajorPart >= 17 ? "" : tagsFile
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

        public static List<TrackParameter> GetTrackParameters(
            gMKVSegment argSeg
            , string argMKVFile
            , string argOutputDirectory
            , MkvChapterTypes argChapterType
            , TimecodesExtractionMode argTimecodesExtractionMode
            , CuesExtractionMode argCueExtractionMode
            , gMKVExtractFilenamePatterns argFilenamePatterns
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
                            argSeg.GetOutputFilename(argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.timestamps_v2)
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
                            argSeg.GetOutputFilename(argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.cues)
                        ),
                        false,
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
                            argSeg.GetOutputFilename(argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.tracks)
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
                            argSeg.GetOutputFilename(argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.attachments)
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

                    string chapterFile = argSeg.GetOutputFilename(argOutputDirectory, argMKVFile, argFilenamePatterns, MkvExtractModes.chapters, argChapterType);

                    // add the parameter for extracting the chapters
                    // Since MKVToolNix v17.0, items that were written to the standard output (chapters, tags and cue sheets) are now always written to files instead.
                    trackParameterList.Add(new TrackParameter(
                        MkvExtractModes.chapters,
                        options,
                        version.FileMajorPart >= 17 ? chapterFile : "",
                        version.FileMajorPart < 17,
                        version.FileMajorPart >= 17 ? "" : chapterFile
                    ));
                }
            }

            return trackParameterList;
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

                string outputString = _MKVExtractOutput?.ToString();

                // Clear the mkvextract output
                _MKVExtractOutput.Length = 0;

                // Parse version info
                return gMKVVersionParser.ParseVersionOutput(outputString);
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

        private static string ConvertOptionValueListToString(List<OptionValue> listOptionValue)
        {
            StringBuilder optionString = new StringBuilder();
            foreach (OptionValue optVal in listOptionValue)
            {
                optionString.AppendFormat(" {0} {1}", 
                    ConvertEnumOptionToStringOption(optVal.Option), 
                    optVal.Parameter);
            }

            return optionString.ToString();
        }

        private static readonly Dictionary<MkvExtractGlobalOptions, string> _MkvExtractGlobalOptionsToStringMap = 
            Enum.GetValues(typeof(MkvExtractGlobalOptions))
            .Cast<MkvExtractGlobalOptions>()
            .ToDictionary(
                val => val,
                val => $"--{val.ToString().Replace("_", "-")}"
            );

        private static string ConvertEnumOptionToStringOption(MkvExtractGlobalOptions enumOption)
        {
            return _MkvExtractGlobalOptionsToStringMap[enumOption];
        }
    }
}
