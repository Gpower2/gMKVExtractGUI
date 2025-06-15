using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using gMKVToolNix.Log;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvInfo
{
    public class gMKVInfo
    {
        // mkvinfo [options] <inname>
        private class OptionValue
        {
            public MkvInfoOptions Option { get; set; }

            public string Parameter { get; set; }

            public OptionValue(MkvInfoOptions opt, string par)
            {
                Option = opt;
                Parameter = par;
            }
        }

        /// <summary>
        /// Gets the mkvinfo executable filename
        /// </summary>
        public static string MKV_INFO_FILENAME
        {
            get { return gMKVHelper.IsOnLinux ? "mkvinfo" : "mkvinfo.exe"; }
        }

        private readonly string _MKVToolnixPath = "";
        private readonly string _MKVInfoFilename = "";
        private readonly List<gMKVSegment> _SegmentList = new List<gMKVSegment>();
        private readonly StringBuilder _MKVInfoOutput = new StringBuilder();
        private readonly StringBuilder _ErrorBuilder = new StringBuilder();
        private Process _MyProcess = null;
        private readonly List<gMKVTrack> _TrackList = new List<gMKVTrack>();
        private int _TrackDelaysFound = 0;
        private int _VideoTrackDelay = Int32.MinValue;
        private gMKVVersion _Version = null;

        public gMKVInfo(string mkvToonlixPath)
        {
            if (string.IsNullOrWhiteSpace(mkvToonlixPath))
            {
                throw new Exception("The MKVToolNix path was not provided!");
            }
            if (!Directory.Exists(mkvToonlixPath))
            {
                throw new Exception($"The MKVToolNix path {mkvToonlixPath} does not exist!");
            }
            _MKVToolnixPath = mkvToonlixPath;
            _MKVInfoFilename = Path.Combine(_MKVToolnixPath, MKV_INFO_FILENAME);
            if (!File.Exists(_MKVInfoFilename))
            {
                throw new Exception($"Could not find {MKV_INFO_FILENAME}!{Environment.NewLine}{_MKVInfoFilename}");
            }
        }

        public List<gMKVSegment> GetMKVSegments(string argMKVFile)
        {
            // Check for existence of MKVInfo
            if (!File.Exists(_MKVInfoFilename))
            {
                throw new Exception($"Could not find {MKV_INFO_FILENAME}!{Environment.NewLine}{_MKVInfoFilename}");
            }

            // First clear the segment list
            _SegmentList.Clear();
            // Clear the mkvinfo output
            _MKVInfoOutput.Length = 0;
            // Clear the error builder
            _ErrorBuilder.Length = 0;

            // Execute MKVInfo
            ExecuteMkvInfo(null, argMKVFile, myProcess_OutputDataReceived);

            // Start the parsing of the output
            ParseMkvInfoOutput();

            // Add the file properties in gMKVSegmentInfo
            if (_SegmentList.Any(s => s is gMKVSegmentInfo))
            {
                var seg = _SegmentList.FirstOrDefault(s => s is gMKVSegmentInfo);
                ((gMKVSegmentInfo)seg).Directory = Path.GetDirectoryName(argMKVFile);
                ((gMKVSegmentInfo)seg).Filename = Path.GetFileName(argMKVFile);
            }

            return _SegmentList;
        }

        public void FindAndSetDelays(List<gMKVSegment> argSegmentsList, string argMKVFile)
        {
            // check for existence of MKVInfo
            if (!File.Exists(_MKVInfoFilename))
            {
                throw new Exception($"Could not find {MKV_INFO_FILENAME}!{Environment.NewLine}{_MKVInfoFilename}");
            }

            // check for list of track numbers
            if (argSegmentsList == null || argSegmentsList.Count == 0)
            {
                throw new Exception("No mkv segments were provided!");
            }

            // Check if there are any video tracks
            if (!argSegmentsList.OfType<gMKVTrack>().Any(x => x.TrackType == MkvTrackType.video))
            {
                // No video track found, so set all the delays to 0
                foreach (gMKVTrack tr in argSegmentsList.OfType<gMKVTrack>().Where(x => x.TrackType == MkvTrackType.audio))
                {
                    tr.Delay = 0;
                    tr.EffectiveDelay = 0;
                }

                // Everything is fine, return true
                return;
            }

            // clear the track list 
            _TrackList.Clear();
            // reset the found delays counter
            _TrackDelaysFound = 0;

            // get only video and audio track in a trackList
            foreach (gMKVTrack segTrack in argSegmentsList.OfType<gMKVTrack>())
            {
                // only find delays for video and audio tracks
                if (segTrack.TrackType != MkvTrackType.subtitles)
                {
                    _TrackList.Add(segTrack);
                    // Update the number of tracks for which delays were found, in order to exit early later on
                    if (segTrack.Delay != int.MinValue)
                    {
                        _TrackDelaysFound++;
                    }
                }
            }

            // add the check_mode option for mkvinfo
            List<OptionValue> optionList = new List<OptionValue>
            {
                new OptionValue(MkvInfoOptions.check_mode, "")
            };

            // Execute MKVInfo
            try
            {
                ExecuteMkvInfo(optionList, argMKVFile, myProcess_OutputDataReceived_Delays);
                // set the effective delays for all tracks
                foreach (gMKVTrack tr in _TrackList)
                {
                    if (tr.TrackType == MkvTrackType.video)
                    {
                        if (_VideoTrackDelay == int.MinValue)
                        {
                            tr.EffectiveDelay = tr.Delay;
                        }
                        else
                        {
                            tr.EffectiveDelay = _VideoTrackDelay;
                        }
                    }
                    else
                    {
                        // check if the video track delay was found
                        if (_VideoTrackDelay == int.MinValue)
                        {
                            tr.EffectiveDelay = tr.Delay;
                        }
                        else
                        {
                            // set the effective delay
                            tr.EffectiveDelay = tr.Delay - _VideoTrackDelay;
                        }
                    }

                    // Check if no delays were detected
                    if (tr.Delay == int.MinValue)
                    {
                        if (tr.EffectiveDelay == int.MinValue)
                        {
                            tr.Delay = 0;
                            tr.EffectiveDelay = 0;
                        }
                        else
                        {
                            tr.Delay = tr.EffectiveDelay;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        public gMKVVersion GetMKVInfoVersion()
        {
            if (_Version != null)
            {
                return _Version;
            }

            // check for existence of MKVInfo
            if (!File.Exists(_MKVInfoFilename))
            {
                throw new Exception($"Could not find {MKV_INFO_FILENAME}!{Environment.NewLine}{_MKVInfoFilename}");
            }

            if (gMKVHelper.IsOnLinux)
            {
                // When on Linux, we need to run mkvinfo 

                // Clear the mkvinfo output
                _MKVInfoOutput.Length = 0;
                // Clear the error builder
                _ErrorBuilder.Length = 0;

                // Execute MKVInfo
                List<OptionValue> options = new List<OptionValue>
                {
                    new OptionValue(MkvInfoOptions.version, "")
                };

                using (Process myProcess = new Process())
                {
                    // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                    // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                    options.Add(new OptionValue(MkvInfoOptions.ui_language, "en_US"));

                    ProcessStartInfo myProcessInfo = new ProcessStartInfo
                    {
                        FileName = _MKVInfoFilename,
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
                    gMKVLogger.Log(String.Format("\"{0}\" {1}", _MKVInfoFilename, myProcessInfo.Arguments));

                    // Start the mkvinfo process
                    myProcess.Start();

                    // Get the Process
                    _MyProcess = myProcess;

                    // Read the Standard output character by character
                    gMKVHelper.ReadStreamPerCharacter(myProcess, myProcess_OutputDataReceived);

                    // Wait for the process to exit
                    myProcess.WaitForExit();

                    // Debug write the exit code
                    string exitString = $"Exit code: {myProcess.ExitCode}";
                    Debug.WriteLine(exitString);
                    gMKVLogger.Log(exitString);

                    _MyProcess = null;

                    // Check the exit code
                    // ExitCode 1 is for warnings only, so ignore it
                    if (myProcess.ExitCode > 1)
                    {
                        // something went wrong!
                        throw new Exception(string.Format("Mkvinfo exited with error code {0}!" +
                            Environment.NewLine + Environment.NewLine + "Errors reported:" + Environment.NewLine + "{1}",
                            myProcess.ExitCode, _ErrorBuilder.ToString()));
                    }
                }

                // Parse version info
                ParseVersionOutput();

                // Clear the mkvinfo output
                _MKVInfoOutput.Length = 0;
            }
            else
            {
                // When on Windows, we can use FileVersionInfo.GetVersionInfo
                var version = FileVersionInfo.GetVersionInfo(_MKVInfoFilename);
                _Version = new gMKVToolNix.gMKVVersion()
                {
                    FileMajorPart = version.FileMajorPart,
                    FileMinorPart = version.FileMinorPart,
                    FilePrivatePart = version.FilePrivatePart
                };
            }

            if (_Version != null)
            {
                gMKVLogger.Log(string.Format("Detected mkvinfo version: {0}.{1}.{2}",
                    _Version.FileMajorPart,
                    _Version.FileMinorPart,
                    _Version.FilePrivatePart
                ));
            }

            return _Version;
        }

        private void ExecuteMkvInfo(List<OptionValue> argOptionList, string argMKVFile, DataReceivedEventHandler argHandler)
        {
            using (Process myProcess = new Process())
            {
                // add the default options for running mkvinfo
                List<OptionValue> optionList = new List<OptionValue>();

                // Detect version
                if (_Version == null)
                {
                    _Version = GetMKVInfoVersion();
                }

                string LC_ALL = "";
                string LANG = "";
                string LC_MESSAGES = "";

                // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                // It appears that the safest way to ensure english output is through the environment variables
                if (gMKVHelper.IsOnLinux)
                {
                    optionList.Add(new OptionValue(MkvInfoOptions.ui_language, "en_US"));

                    // Get the original values
                    LC_ALL = Environment.GetEnvironmentVariable("LC_ALL", EnvironmentVariableTarget.Process);
                    LANG = Environment.GetEnvironmentVariable("LANG", EnvironmentVariableTarget.Process);
                    LC_MESSAGES = Environment.GetEnvironmentVariable("LC_MESSAGES", EnvironmentVariableTarget.Process);

                    gMKVLogger.Log(string.Format("Detected Environment Variables: LC_ALL=\"{0}\",LANG=\"{1}\",LC_MESSAGES=\"{2}\"",
                        LC_ALL, LANG, LC_MESSAGES));

                    // Set the english locale
                    Environment.SetEnvironmentVariable("LC_ALL", "en_US.UTF-8", EnvironmentVariableTarget.Process);
                    Environment.SetEnvironmentVariable("LANG", "en_US.UTF-8", EnvironmentVariableTarget.Process);
                    Environment.SetEnvironmentVariable("LC_MESSAGES", "en_US.UTF-8", EnvironmentVariableTarget.Process);

                    gMKVLogger.Log("Setting Environment Variables: LC_ALL=LANG=LC_MESSAGES=\"en_US.UTF-8\"");
                }
                else
                {
                    optionList.Add(new OptionValue(MkvInfoOptions.ui_language, "en"));
                }

                //optionList.Add(new OptionValue(MkvInfoOptions.command_line_charset, "\"UTF-8\""));
                //optionList.Add(new OptionValue(MkvInfoOptions.output_charset, "\"UTF-8\""));

                // Since MKVToolNix v9.0.0, in Windows and Mac OSX, the default behaviour is to show the GUI
                // In MKVToolNix v9.2.0 the default behaviour changed, so the no-gui option is not needed
                if (!gMKVHelper.IsOnLinux)
                {
                    if (_Version.FileMajorPart == 9 && _Version.FileMinorPart < 2)
                    {
                        optionList.Add(new OptionValue(MkvInfoOptions.no_gui, ""));
                    }
                }

                // check for extra options provided from the caller
                if (argOptionList != null)
                {
                    optionList.AddRange(argOptionList);
                }

                ProcessStartInfo myProcessInfo = new ProcessStartInfo
                {
                    FileName = _MKVInfoFilename,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                // if we didn't provide a filename, then we want to execute mkvinfo with other parameters
                if (!string.IsNullOrWhiteSpace(argMKVFile))
                {
                    myProcessInfo.Arguments = $"{ConvertOptionValueListToString(optionList)} \"{argMKVFile}\"";
                }
                else
                {
                    myProcessInfo.Arguments = ConvertOptionValueListToString(optionList);
                }
                myProcess.StartInfo = myProcessInfo;

                Debug.WriteLine(myProcessInfo.Arguments);
                gMKVLogger.Log($"\"{_MKVInfoFilename}\" {myProcessInfo.Arguments}");

                // Start the mkvinfo process
                myProcess.Start();

                // Get the Process
                _MyProcess = myProcess;

                // Read the Standard output character by character
                gMKVHelper.ReadStreamPerCharacter(myProcess, argHandler);

                // Wait for the process to exit
                myProcess.WaitForExit();

                // Debug write the exit code
                string exitString = $"Exit code: {myProcess.ExitCode}";
                Debug.WriteLine(exitString);
                gMKVLogger.Log(exitString);

                _MyProcess = null;

                // Check the exit code
                // ExitCode 1 is for warnings only, so ignore it
                if (myProcess.ExitCode > 1)
                {
                    // something went wrong!
                    throw new Exception(string.Format("Mkvinfo exited with error code {0}!" +
                        Environment.NewLine + Environment.NewLine + "Errors reported:" + Environment.NewLine + "{1}",
                        myProcess.ExitCode, _ErrorBuilder.ToString()));
                }

                if (gMKVHelper.IsOnLinux)
                {
                    // Reset the environment vairables to their original values
                    Environment.SetEnvironmentVariable("LC_ALL", LC_ALL, EnvironmentVariableTarget.Process);
                    Environment.SetEnvironmentVariable("LANG", LANG, EnvironmentVariableTarget.Process);
                    Environment.SetEnvironmentVariable("LC_MESSAGES", LC_MESSAGES, EnvironmentVariableTarget.Process);

                    gMKVLogger.Log($"Resetting Environment Variables: LC_ALL=\"{LC_ALL}\",LANG=\"{LANG}\",LC_MESSAGES=\"{LC_MESSAGES}\"");
                }
            }
        }

        private enum MkvInfoParseState
        {
            Searching,
            InsideSegmentInfo,
            InsideTrackInfo,
            InsideAttachentInfo,
            InsideChapterInfo
        }

        private void ParseMkvInfoOutput()
        {
            // start the loop for each line of the output
            gMKVSegment tmpSegment = null;
            MkvInfoParseState tmpState = MkvInfoParseState.Searching;
            int attachmentID = 1;
            string[] outputLines = _MKVInfoOutput.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string outputLine in outputLines)
            {
                // first determine the parse state we are in
                //if (outputLine.Contains("Segment,"))
                //{
                //    tmpState = MkvInfoParseState.InsideSegmentInfo;
                //    continue;
                //}
                if (outputLine.Contains("Segment information"))
                {
                    // if previous segment is not null, add it to the list and create a new one
                    if (tmpSegment != null)
                    {
                        _SegmentList.Add(tmpSegment);
                    }
                    tmpSegment = new gMKVSegmentInfo();
                    tmpState = MkvInfoParseState.InsideSegmentInfo;
                    continue;
                }
                else if (outputLine.Contains("Segment tracks"))
                {
                    tmpState = MkvInfoParseState.InsideTrackInfo;
                    continue;
                }
                else if (outputLine.Contains("Attachments"))
                {
                    tmpState = MkvInfoParseState.InsideAttachentInfo;
                    continue;
                }
                else if (outputLine.Contains("Chapters"))
                {
                    tmpState = MkvInfoParseState.InsideChapterInfo;
                    continue;
                }

                // now that we have determined the state, we parse the segment
                switch (tmpState)
                {
                    case MkvInfoParseState.Searching:
                        // if we are still searching for the state, just continue with next line
                        continue;
                    case MkvInfoParseState.InsideSegmentInfo:
                        //if (outputLine.Contains("Segment information"))
                        //{
                        //    // if previous segment is not null, add it to the list and create a new one
                        //    if (tmpSegment != null)
                        //    {
                        //        _SegmentList.Add(tmpSegment);
                        //    }
                        //    tmpSegment = new gMKVSegmentInfo();
                        //}
                        //else if (outputLine.Contains("Timecode scale:"))
                        if (outputLine.Contains("Timecode scale:"))
                        {
                            ((gMKVSegmentInfo)tmpSegment).TimecodeScale = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Muxing application:"))
                        {
                            ((gMKVSegmentInfo)tmpSegment).MuxingApplication = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Writing application:"))
                        {
                            ((gMKVSegmentInfo)tmpSegment).WritingApplication = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Duration:"))
                        {
                            ((gMKVSegmentInfo)tmpSegment).Duration = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Date:"))
                        {
                            ((gMKVSegmentInfo)tmpSegment).Date = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        //8 + Segment, size 320677930
                        //9 |+ Seek head (subentries will be skipped)
                        //10 |+ EbmlVoid (size: 4013)
                        //11 |+ Segment information
                        //12 | + Timecode scale: 1000000
                        //13 | + Muxing application: libebml v1.3.0 + libmatroska v1.4.1
                        //14 | + Writing application: mkvmerge v6.6.0 ('The Edge Of The In Between') built on Dec  1 2013 17:55:00
                        //15 | + Duration: 1364.905s (00:22:44.905)
                        //16 | + Date: Mon Jan 20 21:40:32 2014 UTC
                        //17 | + Segment UID: 0xa3 0x55 0x8d 0x9c 0x25 0x0f 0xba 0x16 0x94 0x09 0xf0 0xc9 0xb4 0x0f 0xc7 0x4b
                        break;
                    case MkvInfoParseState.InsideTrackInfo:
                        if (outputLine.Contains("+ A track"))
                        {
                            // if previous segment is not null, add it to the list and create a new one
                            if (tmpSegment != null)
                            {
                                _SegmentList.Add(tmpSegment);
                            }
                            tmpSegment = new gMKVTrack();
                        }
                        else if (outputLine.Contains("Track number:"))
                        {
                            if (outputLine.Contains("track ID for mkvmerge & mkvextract"))
                            {
                                // if we have version 5.x and newer
                                ((gMKVTrack)tmpSegment).TrackNumber = int.Parse(outputLine.Substring(outputLine.IndexOf(":") + 1, outputLine.IndexOf("(") - outputLine.IndexOf(":") - 1).Trim());
                                ((gMKVTrack)tmpSegment).TrackID = int.Parse(outputLine.Substring(outputLine.LastIndexOf(":") + 1, outputLine.IndexOf(")") - outputLine.LastIndexOf(":") - 1).Trim());
                            }
                            else
                            {
                                // if we have version 4.x and older
                                ((gMKVTrack)tmpSegment).TrackNumber = int.Parse(outputLine.Substring(outputLine.IndexOf(":") + 1).Trim());
                                ((gMKVTrack)tmpSegment).TrackID = ((gMKVTrack)tmpSegment).TrackNumber;
                            }
                        }
                        else if (outputLine.Contains("Track type:"))
                        {
                            ((gMKVTrack)tmpSegment).TrackType = (MkvTrackType)Enum.Parse(typeof(MkvTrackType), outputLine.Substring(outputLine.IndexOf(":") + 1).Trim());
                        }
                        else if (outputLine.Contains("Codec ID:"))
                        {
                            ((gMKVTrack)tmpSegment).CodecID = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Language:"))
                        {
                            ((gMKVTrack)tmpSegment).Language = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Name:"))
                        {
                            ((gMKVTrack)tmpSegment).TrackName = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Pixel width:"))
                        {
                            string tmp = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                            if (int.TryParse(tmp, out int tmpInt))
                            {
                                ((gMKVTrack)tmpSegment).VideoPixelWidth = tmpInt;
                            }
                            ((gMKVTrack)tmpSegment).ExtraInfo = tmp;
                        }
                        else if (outputLine.Contains("Pixel height:"))
                        {
                            string tmp = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                            if (int.TryParse(tmp, out int tmpInt))
                            {
                                ((gMKVTrack)tmpSegment).VideoPixelHeight = tmpInt;
                            }
                            ((gMKVTrack)tmpSegment).ExtraInfo += "x" + outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Sampling frequency:"))
                        {
                            string tmp = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                            if (int.TryParse(tmp, out int tmpInt))
                            {
                                ((gMKVTrack)tmpSegment).AudioSamplingFrequency = tmpInt;
                            }
                            ((gMKVTrack)tmpSegment).ExtraInfo = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Channels:"))
                        {
                            string tmp = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                            if (int.TryParse(tmp, out int tmpInt))
                            {
                                ((gMKVTrack)tmpSegment).AudioChannels = tmpInt;
                            }
                            ((gMKVTrack)tmpSegment).ExtraInfo += ", Ch:" + outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("CodecPrivate,"))
                        {
                            ((gMKVTrack)tmpSegment).CodecPrivate = outputLine.Substring(outputLine.IndexOf(",") + 1).Trim();
                        }

                        //18 |+ Segment tracks
                        //19 | + A track
                        //20 |  + Track number: 1 (track ID for mkvmerge & mkvextract: 0)
                        //21 |  + Track UID: 16103463283017343410
                        //22 |  + Track type: video
                        //23 |  + Lacing flag: 0
                        //24 |  + MinCache: 1
                        //25 |  + Codec ID: V_MPEG4/ISO/AVC
                        //26 |  + CodecPrivate, length 41 (h.264 profile: High @L4.1)
                        //27 |  + Default duration: 41.708ms (23.976 frames/fields per second for a video track)
                        //28 |  + Language: jpn
                        //29 |  + Name: Video
                        //30 |  + Video track
                        //31 |   + Pixel width: 1280
                        //32 |   + Pixel height: 720
                        //33 |   + Display width: 1280
                        //34 |   + Display height: 720
                        //35 | + A track
                        //36 |  + Track number: 2 (track ID for mkvmerge & mkvextract: 1)
                        //37 |  + Track UID: 7691413846401821864
                        //38 |  + Track type: audio
                        //39 |  + Codec ID: A_AAC
                        //40 |  + CodecPrivate, length 5
                        //41 |  + Default duration: 21.333ms (46.875 frames/fields per second for a video track)
                        //42 |  + Language: jpn
                        //43 |  + Name: Audio
                        //44 |  + Audio track
                        //45 |   + Sampling frequency: 48000
                        //46 |   + Channels: 2
                        //47 | + A track
                        //48 |  + Track number: 3 (track ID for mkvmerge & mkvextract: 2)
                        //49 |  + Track UID: 12438050378713133751
                        //50 |  + Track type: subtitles
                        //51 |  + Lacing flag: 0
                        //52 |  + Codec ID: S_TEXT/ASS
                        //53 |  + CodecPrivate, length 1530
                        //54 |  + Language: gre
                        //55 |  + Name: Subs
                        break;
                    case MkvInfoParseState.InsideAttachentInfo:
                        if (outputLine.Contains("Attached"))
                        {
                            // if previous segment is not null, add it to the list and create a new one
                            if (tmpSegment != null)
                            {
                                _SegmentList.Add(tmpSegment);
                            }
                            tmpSegment = new gMKVAttachment();
                            ((gMKVAttachment)tmpSegment).ID = attachmentID;
                            attachmentID++;
                        }
                        else if (outputLine.Contains("File name:"))
                        {
                            ((gMKVAttachment)tmpSegment).Filename = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("File data, size:"))
                        {
                            ((gMKVAttachment)tmpSegment).FileSize = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }
                        else if (outputLine.Contains("Mime type:"))
                        {
                            ((gMKVAttachment)tmpSegment).MimeType = outputLine.Substring(outputLine.IndexOf(":") + 1).Trim();
                        }

                        //57 |+ Attachments
                        //58 | + Attached
                        //59 |  + File name: CENSCBK.TTF
                        //60 |  + Mime type: application/x-truetype-font
                        //61 |  + File data, size: 51716
                        //62 |  + File UID: 2140725150727954330
                        //63 | + Attached
                        //64 |  + File name: segoeui.ttf
                        //65 |  + Mime type: application/x-truetype-font
                        //66 |  + File data, size: 516560
                        //67 |  + File UID: 16764867774861384581
                        //68 | + Attached
                        //69 |  + File name: segoeuib.ttf
                        //70 |  + Mime type: application/x-truetype-font
                        //71 |  + File data, size: 497372
                        //72 |  + File UID: 9565689537892410299
                        //73 | + Attached
                        //74 |  + File name: segoeuii.ttf
                        //75 |  + Mime type: application/x-truetype-font
                        //76 |  + File data, size: 385560
                        //77 |  + File UID: 13103126947871579286
                        //78 | + Attached
                        //79 |  + File name: segoeuil.ttf
                        //80 |  + Mime type: application/x-truetype-font
                        //81 |  + File data, size: 330908
                        //82 |  + File UID: 3235680178630447031
                        //83 | + Attached
                        //84 |  + File name: segoeuiz.ttf
                        //85 |  + Mime type: application/x-truetype-font
                        //86 |  + File data, size: 398148
                        //87 |  + File UID: 17520358819351445890
                        //88 | + Attached
                        //89 |  + File name: seguisb.ttf
                        //90 |  + Mime type: application/x-truetype-font
                        //91 |  + File data, size: 406192
                        //92 |  + File UID: 8550836732450669472

                        break;
                    case MkvInfoParseState.InsideChapterInfo:
                        if (outputLine.Contains("EditionEntry"))
                        {
                            // if previous segment is not null, add it to the list and create a new one
                            if (tmpSegment != null)
                            {
                                _SegmentList.Add(tmpSegment);
                            }
                            tmpSegment = new gMKVChapter();
                        }
                        else if (outputLine.Contains("ChapterAtom"))
                        {
                            ((gMKVChapter)tmpSegment).ChapterCount += 1;
                        }

                        //93 |+ Chapters
                        //94 | + EditionEntry
                        //95 |  + EditionFlagHidden: 0
                        //96 |  + EditionFlagDefault: 0
                        //97 |  + EditionUID: 5248481698181523363
                        //98 |  + ChapterAtom
                        //99 |   + ChapterUID: 13651813039521317265
                        //100 |   + ChapterTimeStart: 00:00:00.000000000
                        //101 |   + ChapterTimeEnd: 00:00:40.874000000
                        //102 |   + ChapterFlagHidden: 0
                        //103 |   + ChapterFlagEnabled: 1
                        //104 |   + ChapterDisplay
                        //105 |    + ChapterString: ��������
                        //106 |    + ChapterLanguage: und
                        //107 |  + ChapterAtom
                        //108 |   + ChapterUID: 9861180919652459706
                        //109 |   + ChapterTimeStart: 00:00:40.999000000
                        //110 |   + ChapterTimeEnd: 00:02:00.829000000
                        //111 |   + ChapterFlagHidden: 0
                        //112 |   + ChapterFlagEnabled: 1
                        //113 |   + ChapterDisplay
                        //114 |    + ChapterString: ������ �����
                        //115 |    + ChapterLanguage: und
                        //116 |  + ChapterAtom
                        //117 |   + ChapterUID: 18185444543032186557
                        //118 |   + ChapterTimeStart: 00:02:00.954000000
                        //119 |   + ChapterTimeEnd: 00:21:24.700000000
                        //120 |   + ChapterFlagHidden: 0
                        //121 |   + ChapterFlagEnabled: 1
                        //122 |   + ChapterDisplay
                        //123 |    + ChapterString: ������ �����
                        //124 |    + ChapterLanguage: und
                        //125 |  + ChapterAtom
                        //126 |   + ChapterUID: 12481834811641996944
                        //127 |   + ChapterTimeStart: 00:21:24.867000000
                        //128 |   + ChapterTimeEnd: 00:22:44.864000000
                        //129 |   + ChapterFlagHidden: 0
                        //130 |   + ChapterFlagEnabled: 1
                        //131 |   + ChapterDisplay
                        //132 |    + ChapterString: ������ ������
                        //133 |    + ChapterLanguage: und

                        break;
                    default:
                        break;
                }
            }

            // if the last segment was not added to the list and it is not null, add it to the list
            if (tmpSegment != null)
            {
                _SegmentList.Add(tmpSegment);
            }
        }

        private void ParseVersionOutput()
        {
            string fileMajorVersion = "0";
            string fileMinorVersion = "0";
            string filePrivateVersion = "0";
            if (_MKVInfoOutput != null && _MKVInfoOutput.Length > 0)
            {
                string[] outputLines = _MKVInfoOutput.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string outputLine in outputLines)
                {
                    if (outputLine.StartsWith("mkvinfo v"))
                    {
                        string versionString = outputLine.Substring(8);
                        versionString = versionString.Substring(1, versionString.IndexOf(" "));
                        if (versionString.Contains("."))
                        {
                            string[] parts = versionString.Split(new string[] { "." }, StringSplitOptions.None);
                            if (parts.Length >= 2)
                            {
                                fileMajorVersion = parts[0];
                                fileMinorVersion = parts[1];
                                if (parts.Length > 2)
                                {
                                    filePrivateVersion = parts[2];
                                }
                            }
                        }
                        break;
                    }
                }
            }

            gMKVVersion version = new gMKVToolNix.gMKVVersion()
            {
                FileMajorPart = int.TryParse(fileMajorVersion, out int majorVersion) ? majorVersion : 0,
                FileMinorPart = int.TryParse(fileMinorVersion, out int minorVersion) ? minorVersion : 0,
                FilePrivatePart = int.TryParse(filePrivateVersion, out int privateVersion) ? privateVersion : 0
            };

            _Version = version;
        }

        private void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Trim().Length > 0)
                {
                    // debug write the output line
                    Debug.WriteLine(e.Data);
                    // log the output
                    gMKVLogger.Log(e.Data);

                    // add the line to the output stringbuilder
                    _MKVInfoOutput.AppendLine(e.Data);
                    // check for errors
                    if (e.Data.Contains("Error:"))
                    {
                        _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(":") + 1).Trim());
                    }
                }
            }
        }

        private void myProcess_OutputDataReceived_Delays(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Trim().Length > 0)
                {
                    // debug write the output line
                    Debug.WriteLine(e.Data);
                    // log the output
                    gMKVLogger.Log(e.Data);

                    // check for errors                    
                    if (e.Data.Contains("Error:"))
                    {
                        _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(":") + 1).Trim());
                    }

                    // check if line contains the first timecode for one of the requested tracks
                    foreach (gMKVTrack tr in _TrackList)
                    {
                        // check if the delay is already found
                        if (tr.Delay == int.MinValue)
                        {
                            // try to find the delay
                            Match m = Regex.Match(e.Data, string.Format(@"track number {0}, \d+ frame\(s\), timecode (\d+\.\d+)s", tr.TrackNumber));
                            Match m2 = Regex.Match(e.Data, string.Format(@"track number {0}, \d+ frame\(s\), timestamp (\d{{2}}):(\d{{2}}):(\d{{2}}).(\d{{9}})", tr.TrackNumber));
                            if (m.Success)
                            {
                                // Parse the delay (get the seconds in decimal, multiply by 1000 to convert them to ms, and then convert to Int32
                                int delay = Convert.ToInt32(decimal.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) * 1000m);
                                // set the track delay
                                tr.Delay = delay;
                                // increase the number of track delays found
                                _TrackDelaysFound++;
                                // check if the track is a videotrack and set the VideoTrackDelay
                                if (tr.TrackType == MkvTrackType.video)
                                {
                                    // set the video track delay
                                    _VideoTrackDelay = delay;
                                }
                                break;
                            }
                            else if (m2.Success)
                            {
                                // Parse the delay (get the seconds in nanoseconds
                                int delayHours = Convert.ToInt32(long.Parse(m2.Groups[1].Value, CultureInfo.InvariantCulture));
                                int delayMinutes = Convert.ToInt32(long.Parse(m2.Groups[2].Value, CultureInfo.InvariantCulture));
                                int delaySeconds = Convert.ToInt32(long.Parse(m2.Groups[3].Value, CultureInfo.InvariantCulture));
                                int delayNanoSeconds = Convert.ToInt32(long.Parse(m2.Groups[4].Value, CultureInfo.InvariantCulture));

                                int delay = Convert.ToInt32(new TimeSpan(0, delayHours, delayMinutes, delaySeconds, delayNanoSeconds / 1000000).TotalMilliseconds);
                                // set the track delay
                                tr.Delay = delay;
                                // increase the number of track delays found
                                _TrackDelaysFound++;
                                // check if the track is a videotrack and set the VideoTrackDelay
                                if (tr.TrackType == MkvTrackType.video)
                                {
                                    // set the video track delay
                                    _VideoTrackDelay = delay;
                                }
                                break;
                            }
                        }
                    }

                    // check if first timecodes for all tracks where found
                    if (_TrackDelaysFound == _TrackList.Count)
                    {
                        if (_MyProcess != null)
                        {
                            if (!_MyProcess.HasExited)
                            {
                                _MyProcess.Kill();
                            }
                        }
                    }
                }
            }
        }

        private static string ConvertOptionValueListToString(List<OptionValue> argListOptionValue)
        {
            StringBuilder optionString = new StringBuilder();
            foreach (OptionValue optVal in argListOptionValue)
            {
                optionString.AppendFormat(" {0} {1}", ConvertEnumOptionToStringOption(optVal.Option), optVal.Parameter);
            }
            return optionString.ToString();
        }

        private static string ConvertEnumOptionToStringOption(MkvInfoOptions argEnumOption)
        {
            return $"--{argEnumOption}".Replace("_", "-");
        }
    }
}
