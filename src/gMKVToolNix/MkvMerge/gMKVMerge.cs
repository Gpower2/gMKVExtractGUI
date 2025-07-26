using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using gMKVToolNix.Log;
using gMKVToolNix.Segments;
using Newtonsoft.Json.Linq;

namespace gMKVToolNix.MkvMerge
{
    public class gMKVMerge
    {
        private static readonly string[] _dateFormats = { 
            // Basic formats
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            // Extended formats
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            // All of the above with reduced accuracy
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            // Accuracy reduced to hours
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ",
            // 
            "yyyyMMdd",
            "yyyyMMddT",
            "yyyy-MM-dd",
            "yyyy-MM-ddT"
        };

        /// <summary>
        /// Gets the mkvinfo executable filename
        /// </summary>
        public static string MKV_MERGE_FILENAME
        {
            get { return PlatformExtensions.IsOnLinux ? "mkvmerge" : "mkvmerge.exe"; }
        }

        private readonly string _MKVToolnixPath = "";
        private readonly string _MKVMergeFilename = "";

        private readonly gMKVVersion _Version = null;

        public gMKVMerge(string mkvToolnixPath)
        {
            if (string.IsNullOrWhiteSpace(mkvToolnixPath))
            {
                throw new Exception("The MKVToolNix path was not provided!");
            }

            if (!Directory.Exists(mkvToolnixPath))
            {
                throw new Exception(string.Format("The MKVToolNix path {0} does not exist!", mkvToolnixPath));
            }

            _MKVToolnixPath = mkvToolnixPath;
            _MKVMergeFilename = Path.Combine(_MKVToolnixPath, MKV_MERGE_FILENAME);

            if (!File.Exists(_MKVMergeFilename))
            {
                throw new Exception(string.Format("Could not find {0}!" + Environment.NewLine + "{1}", MKV_MERGE_FILENAME, _MKVMergeFilename));
            }

            _Version = GetMKVMergeVersion();

            if (_Version != null)
            {
                gMKVLogger.Log(string.Format("Detected mkvmerge version: {0}.{1}.{2}",
                    _Version.FileMajorPart,
                    _Version.FileMinorPart,
                    _Version.FilePrivatePart
                ));
            }
        }

        public List<gMKVSegment> GetMKVSegments(string argMKVFile)
        {
            List<string> outputLines = new List<string>();
            List<string> errors = new List<string>();

            // Execute the mkvmerge
            ExecuteMkvMerge(null, argMKVFile, errors, CreateProcessOutputHandlerFactory(
                (string line) => outputLines.Add(line),
                (string error) => errors.Add(error)));

            // Set the segment list
            List<gMKVSegment> segmentList = new List<gMKVSegment>();

            // Start the parsing of the output
            // Since MKVToolNix v9.6.0, start parsing the JSON identification info
            if (_Version.FileMajorPart > 9 ||
                (_Version.FileMajorPart == 9 && _Version.FileMinorPart >= 6))
            {
                segmentList.AddRange(ParseMkvMergeJsonOutput(string.Join(Environment.NewLine, outputLines)));
            }
            else
            {
                segmentList.AddRange(ParseMkvMergeOutput(outputLines));
            }

            // Add the file properties in gMKVSegmentInfo
            gMKVSegmentInfo seg = segmentList.OfType<gMKVSegmentInfo>().FirstOrDefault();
            if (seg != null)
            {
                seg.Directory = Path.GetDirectoryName(argMKVFile);
                seg.Filename = Path.GetFileName(argMKVFile);
            }

            return segmentList;
        }

        public bool FindAndSetDelays(List<gMKVSegment> argSegmentList)
        {
            // Check to see if the list contains segments
            if (argSegmentList == null || argSegmentList.Count == 0)
            {
                return false;
            }

            // Check if there are any video tracks
            List<gMKVTrack> trackList = argSegmentList.OfType<gMKVTrack>().ToList();
            if (!trackList.Any(x => x.TrackType == MkvTrackType.video))
            {
                // No video track found, so set all the delays to 0
                foreach (gMKVTrack tr in trackList.Where(x => x.TrackType == MkvTrackType.audio))
                {
                    tr.Delay = 0;
                    tr.EffectiveDelay = 0;
                }

                // Everything is fine, return true
                return true;
            }

            int videoDelay = int.MinValue;

            // First, find the video delay
            foreach (gMKVTrack track in trackList)
            {
                if (track.TrackType == MkvTrackType.video)
                {
                    // Check if MinimumTimestamp property was found
                    if (track.MinimumTimestamp == long.MinValue)
                    {
                        // Could not determine the delays
                        return false;
                    }
                    // Convert to ms from ns
                    videoDelay = Convert.ToInt32(track.MinimumTimestamp / 1000000);
                    track.Delay = videoDelay;
                    track.EffectiveDelay = videoDelay;
                    break;
                }
            }

            // Check if video delay was found
            if (videoDelay == int.MinValue)
            {
                // Could not determine the delays
                return false;
            }

            // If video delay was found, then determine all the audio tracks delays
            foreach (gMKVTrack track in trackList)
            {
                if (track.TrackType == MkvTrackType.audio)
                {
                    // Check if MinimumTimestamp property was found
                    if (track.MinimumTimestamp == long.MinValue)
                    {
                        // Could not determine the delays
                        return false;
                    }
                    // Convert to ms from ns
                    track.Delay = Convert.ToInt32(track.MinimumTimestamp / 1000000);
                    track.EffectiveDelay = track.Delay - videoDelay;
                }
            }

            // If everything went all right, then we return true
            return true;
        }

        public void FindAndSetCodecPrivate(List<gMKVSegment> argSegmentList)
        {
            foreach (gMKVTrack track in argSegmentList.OfType<gMKVTrack>())
            {
                track.CodecPrivate = track.GetTrackCodecPrivate();
            }
        }

        public gMKVVersion GetMKVMergeVersion()
        {
            if (_Version != null)
            {
                return _Version;
            }

            if (PlatformExtensions.IsOnLinux)
            {
                // When on Linux, we need to run mkvmerge

                // Execute mkvmerge
                List<OptionValue<MkvMergeOptions>> options = new List<OptionValue<MkvMergeOptions>>
                {
                    new OptionValue<MkvMergeOptions>(MkvMergeOptions.version, "")
                };

                List<string> versionOutputLines = new List<string>();
                List<string> errors = new List<string>();

                using (Process myProcess = new Process())
                {
                    // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                    // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                    options.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.ui_language, "en_US"));

                    ProcessStartInfo myProcessInfo = new ProcessStartInfo
                    {
                        FileName = _MKVMergeFilename,
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
                    gMKVLogger.Log($"\"{_MKVMergeFilename}\" {myProcessInfo.Arguments}");

                    // Start the mkvmerge process
                    myProcess.Start();

                    // Read the Standard output character by character
                    myProcess.ReadStreamPerCharacter(CreateProcessOutputHandlerFactory(
                        (string line) => versionOutputLines.Add(line),
                        (string error) => errors.Add(error)));

                    // Wait for the process to exit
                    myProcess.WaitForExit();

                    // Debug write the exit code
                    string exitCodeString = $"Exit code: {myProcess.ExitCode}";
                    Debug.WriteLine(exitCodeString);
                    gMKVLogger.Log(exitCodeString);

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
                var version = FileVersionInfo.GetVersionInfo(_MKVMergeFilename);
                return new gMKVVersion()
                {
                    FileMajorPart = version.FileMajorPart,
                    FileMinorPart = version.FileMinorPart,
                    FilePrivatePart = version.FilePrivatePart
                };
            }
        }

        private void ExecuteMkvMerge(
            List<OptionValue<MkvMergeOptions>> argOptionList, 
            string argMKVFile, 
            List<string> errors, 
            Action<Process, string> argHandler)
        {
            using (Process myProcess = new Process())
            {
                List<OptionValue<MkvMergeOptions>> optionList = new List<OptionValue<MkvMergeOptions>>();

                string LC_ALL = "";
                string LANG = "";
                string LC_MESSAGES = "";

                // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                if (PlatformExtensions.IsOnLinux)
                {
                    optionList.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.ui_language, "en_US"));
                }
                else
                {
                    optionList.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.ui_language, "en"));
                }
                //optionList.Add(new OptionValue(MkvMergeOptions.command_line_charset, "\"UTF-8\""));
                //optionList.Add(new OptionValue(MkvMergeOptions.output_charset, "\"UTF-8\""));

                // if we didn't provide a filename, then we want to execute mkvmerge with other parameters
                if (!string.IsNullOrWhiteSpace(argMKVFile))
                {
                    // Since MKVToolNix v9.6.0, start parsing the JSON identification info
                    if (_Version.FileMajorPart > 9 ||
                        (_Version.FileMajorPart == 9 && _Version.FileMinorPart >= 6))
                    {
                        optionList.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.identify, ""));
                        optionList.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.identification_format, "json"));
                    }
                    else
                    {
                        // For previous mkvmerge versions, keep compatibility
                        optionList.Add(new OptionValue<MkvMergeOptions>(MkvMergeOptions.identify_verbose, ""));

                        // Before JSON output, the safest way to ensure English output on Linux is throught the EnvironmentVariables
                        if (PlatformExtensions.IsOnLinux)
                        {
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
                    }
                }

                // check for extra options provided from the caller
                if (argOptionList != null)
                {
                    optionList.AddRange(argOptionList);
                }

                ProcessStartInfo myProcessInfo = new ProcessStartInfo
                {
                    FileName = _MKVMergeFilename,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                if (!string.IsNullOrWhiteSpace(argMKVFile))
                {
                    myProcessInfo.Arguments = string.Format("{0} \"{1}\"", optionList.ConvertOptionValueListToString(), argMKVFile);
                }
                else
                {
                    myProcessInfo.Arguments = optionList.ConvertOptionValueListToString();
                }

                myProcess.StartInfo = myProcessInfo;

                Debug.WriteLine(myProcessInfo.Arguments);
                gMKVLogger.Log(string.Format("\"{0}\" {1}", _MKVMergeFilename, myProcessInfo.Arguments));

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
                    throw new Exception(string.Format("Mkvmerge exited with error code {0}!" +
                        Environment.NewLine + Environment.NewLine + "Errors reported:" + Environment.NewLine + "{1}",
                        myProcess.ExitCode, string.Join(Environment.NewLine, errors)));
                }

                // Before JSON output, the safest way to ensure English output on Linux is throught the EnvironmentVariables
                if (PlatformExtensions.IsOnLinux)
                {
                    if (_Version.FileMajorPart < 9 ||
                        (_Version.FileMajorPart == 9 && _Version.FileMinorPart < 6))
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

        private static List<gMKVSegment> ParseMkvMergeJsonOutput(string output)
        {
            // Read the JSON output data to a JObject
            JObject jsonObject = JObject.Parse(output);

            // Create temporary Lists for the segments
            List<gMKVSegment> chapters = new List<gMKVSegment>();
            List<gMKVSegment> attachments = new List<gMKVSegment>();
            List<gMKVSegment> tracks = new List<gMKVSegment>();

            List<gMKVSegment> finalList = new List<gMKVSegment>();

            // Parse all the children tokens accordingly
            foreach (JToken token in jsonObject.Children())
            {
                if (!(token is JProperty jProp))
                {
                    continue;
                }

                if (jProp == null || string.IsNullOrWhiteSpace(jProp.Name) || !jProp.HasValues)
                {
                    continue;
                }

                string pName = jProp.Name.ToLower().Trim();
                if (pName == "chapters")
                {
                    foreach (JToken entry in jProp)
                    {
                        if (entry is JArray entryArray && entryArray.Count > 0)
                        {
                            foreach (JToken entryTokens in entry)
                            {
                                if (entryTokens.HasValues)
                                {
                                    foreach (JToken chapEntry in entryTokens)
                                    {
                                        chapters.Add(
                                            new gMKVChapter
                                            {
                                                ChapterCount = chapEntry.ToObject<int>()
                                            }
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                else if (pName == "attachments")
                {
                    foreach (JToken attachmentToken in jProp)
                    {
                        foreach (JToken finalAttachmentToken in attachmentToken)
                        {
                            gMKVAttachment tmpAttachment = new gMKVAttachment();
                            foreach (JToken propertyAttachmentToken in finalAttachmentToken)
                            {
                                if (propertyAttachmentToken is JProperty prop)
                                {
                                    if (prop == null || string.IsNullOrWhiteSpace(prop.Name))
                                    {
                                        continue;
                                    }

                                    string propName = prop.Name.ToLower().Trim();
                                    if (propName == "content_type")
                                    {
                                        tmpAttachment.MimeType = prop.ToObject<string>();
                                    }
                                    else if (propName == "file_name")
                                    {
                                        tmpAttachment.Filename = prop.ToObject<string>();
                                    }
                                    else if (propName == "id")
                                    {
                                        tmpAttachment.ID = prop.ToObject<int>();
                                    }
                                    else if (propName == "size")
                                    {
                                        tmpAttachment.FileSize = prop.ToObject<string>();
                                    }
                                }
                            }
                            attachments.Add(tmpAttachment);
                        }
                    }
                }
                else if (pName == "container")
                {
                    gMKVSegmentInfo tmpSegInfo = new gMKVSegmentInfo();
                    foreach (JToken child in jProp)
                    {
                        if (!child.HasValues)
                        {
                            continue;
                        }

                        foreach (JToken value in child)
                        {
                            if (!(value is JProperty valueProperty))
                            {
                                continue;
                            }

                            if (valueProperty == null || string.IsNullOrWhiteSpace(valueProperty.Name))
                            {
                                continue;
                            }

                            string valuePropertyName = valueProperty.Name.ToLower().Trim();
                            if (valuePropertyName == "recognized")
                            {
                                if (!valueProperty.ToObject<bool>())
                                {
                                    throw new Exception("The container of the file was not recognized!");
                                }
                            }
                            else if (valuePropertyName == "supported")
                            {
                                if (!valueProperty.ToObject<bool>())
                                {
                                    throw new Exception("The container of the file is not supported!");
                                }
                            }
                            else if (valuePropertyName == "properties")
                            {
                                foreach (JToken childProperty in valueProperty)
                                {
                                    foreach (JToken childPreFinalProperty in childProperty)
                                    {
                                        if (!(childPreFinalProperty is JProperty childFinalProperty))
                                        {
                                            continue;
                                        }

                                        if (childFinalProperty == null || string.IsNullOrWhiteSpace(childFinalProperty.Name))
                                        {
                                            continue;
                                        }

                                        string childFinalPropertyName = childFinalProperty.Name.ToLower().Trim();
                                        if (childFinalPropertyName == "date_utc")
                                        {
                                            string dateValue = childFinalProperty.ToString().Replace("\"date_utc\":", "").Replace("\"", "").Trim();
                                            tmpSegInfo.Date = DateTime.ParseExact(dateValue, _dateFormats, CultureInfo.InvariantCulture,
                                                DateTimeStyles.AssumeUniversal).ToUniversalTime().
                                                ToString("ddd MMM dd HH:mm:ss yyyy UTC", CultureInfo.InvariantCulture);
                                        }
                                        else if (childFinalPropertyName == "duration")
                                        {
                                            // Duration: 5979.008s (01:39:39.008)
                                            string originalDuration = childFinalProperty.ToObject<string>();
                                            TimeSpan tmpTime = TimeSpan.FromMilliseconds(long.Parse(originalDuration) / 1000000.0);
                                            tmpSegInfo.Duration = string.Format("{0}s ({1}:{2}:{3}.{4})",
                                                (long.Parse(originalDuration) / 1000000000.0).ToString("#0.000", CultureInfo.InvariantCulture),
                                                tmpTime.Hours.ToString("00"),
                                                tmpTime.Minutes.ToString("00"),
                                                tmpTime.Seconds.ToString("00"),
                                                tmpTime.Milliseconds.ToString("000"));
                                        }
                                        else if (childFinalPropertyName == "muxing_application")
                                        {
                                            tmpSegInfo.MuxingApplication = childFinalProperty.ToObject<string>();
                                        }
                                        else if (childFinalPropertyName == "writing_application")
                                        {
                                            tmpSegInfo.WritingApplication = childFinalProperty.ToObject<string>();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    finalList.Add(tmpSegInfo);
                } // "container"
                else if (pName == "tracks")
                {
                    foreach (JToken child in jProp)
                    {
                        if (!child.HasValues)
                        {
                            continue;
                        }

                        foreach (JToken value in child)
                        {
                            if (!value.HasValues)
                            {
                                continue;
                            }

                            gMKVTrack tmpTrack = new gMKVTrack();

                            foreach (JToken childPreFinalProperty in value)
                            {
                                if (!(childPreFinalProperty is JProperty childFinalProperty))
                                {
                                    continue;
                                }

                                if (childFinalProperty == null || string.IsNullOrWhiteSpace(childFinalProperty.Name))
                                {
                                    continue;
                                }

                                string childFinalPropertyName = childFinalProperty.Name.ToLower().Trim();
                                if (childFinalPropertyName == "id")
                                {
                                    tmpTrack.TrackID = childFinalProperty.ToObject<int>();
                                }
                                else if (childFinalPropertyName == "type")
                                {
                                    tmpTrack.TrackType = (MkvTrackType)Enum.Parse(typeof(MkvTrackType), childFinalProperty.ToObject<string>());
                                }
                                else if (childFinalPropertyName == "properties")
                                {
                                    if (!childFinalProperty.HasValues)
                                    {
                                        continue;
                                    }

                                    foreach (JToken propertyChild in childFinalProperty)
                                    {
                                        if (!propertyChild.HasValues)
                                        {
                                            continue;
                                        }

                                        string audioChannels = "";
                                        string audioFrequency = "";
                                        string videoDimensions = "";
                                        foreach (JToken propertyFinalChild in propertyChild)
                                        {
                                            if (!propertyFinalChild.HasValues || !(propertyFinalChild is JProperty propertyFinal))
                                            {
                                                continue;
                                            }

                                            if (propertyFinal == null || string.IsNullOrWhiteSpace(propertyFinal.Name))
                                            {
                                                continue;
                                            }

                                            string propertyFinalName = propertyFinal.Name.ToLower().Trim();
                                            if (propertyFinalName == "codec_id")
                                            {
                                                tmpTrack.CodecID = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "codec_private_data")
                                            {
                                                tmpTrack.CodecPrivateData = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "track_name")
                                            {
                                                tmpTrack.TrackName = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "forced_track")
                                            {
                                                tmpTrack.Forced = propertyFinal.ToObject<bool>();
                                            }
                                            else if (propertyFinalName == "language")
                                            {
                                                tmpTrack.Language = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "language_ietf")
                                            {
                                                tmpTrack.LanguageIetf = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "minimum_timestamp")
                                            {
                                                tmpTrack.MinimumTimestamp = propertyFinal.ToObject<long>();
                                            }
                                            else if (propertyFinalName == "number")
                                            {
                                                tmpTrack.TrackNumber = propertyFinal.ToObject<int>();
                                            }
                                            else if (propertyFinalName == "pixel_dimensions")
                                            {
                                                videoDimensions = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "audio_channels")
                                            {
                                                audioChannels = propertyFinal.ToObject<string>();
                                            }
                                            else if (propertyFinalName == "audio_sampling_frequency")
                                            {
                                                audioFrequency = propertyFinal.ToObject<string>();
                                            }
                                            //else if (propertyFinalName == "default_duration")
                                            //{
                                            //}
                                            //else if (propertyFinalName == "display_dimensions")
                                            //{
                                            //}
                                            //else if (propertyFinalName == "uid")
                                            //{
                                            //}
                                        }
                                        if (!string.IsNullOrWhiteSpace(videoDimensions))
                                        {
                                            tmpTrack.ExtraInfo = videoDimensions;
                                            if (videoDimensions.ToLower().Contains("x"))
                                            {
                                                string w = videoDimensions.Substring(0, videoDimensions.IndexOf("x"));
                                                string h = videoDimensions.Substring(videoDimensions.IndexOf("x") + 1);

                                                if (int.TryParse(w, out int tmpW) && int.TryParse(h, out int tmpH))
                                                {
                                                    tmpTrack.VideoPixelWidth = tmpW;
                                                    tmpTrack.VideoPixelHeight = tmpH;
                                                }
                                            }
                                        }
                                        else if (!string.IsNullOrWhiteSpace(audioChannels) 
                                            || !string.IsNullOrWhiteSpace(audioFrequency))
                                        {
                                            if (!string.IsNullOrWhiteSpace(audioChannels) 
                                                && !string.IsNullOrWhiteSpace(audioFrequency))
                                            {
                                                tmpTrack.ExtraInfo = $"{audioFrequency}Hz, Ch: {audioChannels}";
                                            }

                                            if (!string.IsNullOrWhiteSpace(audioChannels))
                                            {
                                                if (int.TryParse(audioChannels, out int tmpInt))
                                                {
                                                    tmpTrack.AudioChannels = tmpInt;
                                                }
                                            }

                                            if (!string.IsNullOrWhiteSpace(audioFrequency))
                                            {
                                                if (int.TryParse(audioFrequency, out int tmpInt))
                                                {
                                                    tmpTrack.AudioSamplingFrequency = tmpInt;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            tracks.Add(tmpTrack);
                        }
                    }
                } // "tracks"
            }

            // Add the segments in the correct order
            foreach (gMKVSegment seg in tracks)
            {
                finalList.Add(seg);
            }
            foreach (gMKVSegment seg in attachments)
            {
                finalList.Add(seg);
            }
            foreach (gMKVSegment seg in chapters)
            {
                finalList.Add(seg);
            }

            return finalList;
        }

        private static List<gMKVSegment> ParseMkvMergeOutput(List<string> output)
        {
            List<gMKVSegment> finalList = new List<gMKVSegment>();

            // start the loop for each line of the output
            foreach (string outputLine in output)
            {
                if (string.IsNullOrWhiteSpace(outputLine))
                {
                    // skip empty lines
                    continue;
                }

                if (outputLine.StartsWith("File "))
                {
                    gMKVSegmentInfo tmpSegInfo = new gMKVSegmentInfo();
                    if (outputLine.Contains("muxing_application:"))
                    {
                        tmpSegInfo.MuxingApplication = ExtractProperty(outputLine, "muxing_application");
                    }

                    if (outputLine.Contains("writing_application:"))
                    {
                        tmpSegInfo.WritingApplication = ExtractProperty(outputLine, "writing_application");
                    }

                    if (outputLine.Contains("date_utc:"))
                    {
                        tmpSegInfo.Date = DateTime.ParseExact(ExtractProperty(outputLine, "date_utc"), _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).
                            ToUniversalTime().
                            ToString("ddd MMM dd HH:mm:ss yyyy UTC", CultureInfo.InvariantCulture);
                    }

                    if (outputLine.Contains("duration:"))
                    {
                        // Duration: 5979.008s (01:39:39.008)
                        string originalDuration = ExtractProperty(outputLine, "duration");
                        TimeSpan tmpTime = TimeSpan.FromMilliseconds(long.Parse(originalDuration) / 1000000.0);
                        tmpSegInfo.Duration = string.Format("{0}s ({1}:{2}:{3}.{4})",
                            (long.Parse(originalDuration) / 1000000000.0).ToString("#0.000", CultureInfo.InvariantCulture),
                            tmpTime.Hours.ToString("00"),
                            tmpTime.Minutes.ToString("00"),
                            tmpTime.Seconds.ToString("00"),
                            tmpTime.Milliseconds.ToString("000"));
                    }

                    if (!string.IsNullOrWhiteSpace(tmpSegInfo.MuxingApplication)
                        && !string.IsNullOrWhiteSpace(tmpSegInfo.WritingApplication)
                        && !string.IsNullOrWhiteSpace(tmpSegInfo.Duration))
                    {
                        finalList.Add(tmpSegInfo);
                    }
                }
                else if (outputLine.StartsWith("Track ID "))
                {
                    int trackID = int.TryParse(outputLine.Substring(0, outputLine.IndexOf(":")).Replace("Track ID", "").Trim(), out int trackId) ? trackId : 0;

                    // Check if there is already a track with the same TrackID (workaround for a weird bug in MKVToolnix v4 when identifying files from AviDemux)
                    bool trackFound = false;
                    foreach (gMKVSegment tmpSeg in finalList)
                    {
                        if (tmpSeg is gMKVTrack track && track.TrackID == trackID)
                        {
                            // If we already have a track with the same trackID, then don't bother adding it again
                            trackFound = true;
                            break;
                        }
                    }

                    // Check if track found
                    if (trackFound)
                    {
                        // If we already have a track with the same trackID, then don't bother adding it again
                        continue;
                    }

                    gMKVTrack tmpTrack = new gMKVTrack
                    {
                        TrackType = (MkvTrackType)Enum.Parse(
                            typeof(MkvTrackType),
                            outputLine.Substring(
                                outputLine.IndexOf(":") + 1,
                                outputLine.IndexOf("(") - outputLine.IndexOf(":") - 1)
                            .Trim()),
                        TrackID = trackID
                    };

                    if (outputLine.Contains("number"))
                    {
                        // if we have version 5.x and newer
                        tmpTrack.TrackNumber = int.TryParse(ExtractProperty(outputLine, "number"), out int trackNumber) ? trackNumber : 0;
                    }
                    else
                    {
                        // if we have version 4.x and older
                        tmpTrack.TrackNumber = tmpTrack.TrackID;
                    }

                    if (outputLine.Contains("codec_id"))
                    {
                        // if we have version 5.x and newer
                        tmpTrack.CodecID = ExtractProperty(outputLine, "codec_id");
                    }
                    else
                    {
                        // if we have version 4.x and older
                        tmpTrack.CodecID = outputLine.Substring(outputLine.IndexOf("(") + 1, outputLine.IndexOf(")") - outputLine.IndexOf("(") - 1);
                    }

                    if (outputLine.Contains("language:"))
                    {
                        tmpTrack.Language = ExtractProperty(outputLine, "language");
                    }

                    if (outputLine.Contains("language_ietf:"))
                    {
                        tmpTrack.LanguageIetf = ExtractProperty(outputLine, "language_ietf");
                    }

                    if (outputLine.Contains("track_name:"))
                    {
                        tmpTrack.TrackName = ExtractProperty(outputLine, "track_name");
                    }
                    
                    if (outputLine.Contains("forced_track:"))
                    {
                        string forcedProperty = ExtractProperty(outputLine, "forced_track");
                        tmpTrack.Forced = forcedProperty.Trim().Equals("1");
                    }

                    if (outputLine.Contains("codec_private_data:"))
                    {
                        tmpTrack.CodecPrivateData = ExtractProperty(outputLine, "codec_private_data");
                    }

                    switch (tmpTrack.TrackType)
                    {
                        case MkvTrackType.video:
                            if (outputLine.Contains("pixel_dimensions:"))
                            {
                                string videoDimensions = ExtractProperty(outputLine, "pixel_dimensions");
                                tmpTrack.ExtraInfo = videoDimensions;
                                if (videoDimensions.ToLower().Contains("x"))
                                {
                                    string w = videoDimensions.Substring(0, videoDimensions.IndexOf("x"));
                                    string h = videoDimensions.Substring(videoDimensions.IndexOf("x") + 1);

                                    if (int.TryParse(w, out int tmpW) && int.TryParse(h, out int tmpH))
                                    {
                                        tmpTrack.VideoPixelWidth = tmpW;
                                        tmpTrack.VideoPixelHeight = tmpH;
                                    }
                                }
                            }

                            // in versions after v9.0.1, Mosu was kind enough to provide us with the minimum_timestamp property
                            // in order to determine the current track's delay
                            if (outputLine.Contains("minimum_timestamp:"))
                            {
                                if (long.TryParse(ExtractProperty(outputLine, "minimum_timestamp"), out long tmpLong))
                                {
                                    tmpTrack.MinimumTimestamp = tmpLong;
                                }
                            }

                            break;
                        case MkvTrackType.audio:
                            if (outputLine.Contains("audio_sampling_frequency:"))
                            {
                                string audioFrequency = ExtractProperty(outputLine, "audio_sampling_frequency");
                                if (int.TryParse(audioFrequency, out int tmpInt))
                                {
                                    tmpTrack.AudioSamplingFrequency = tmpInt;
                                }

                                tmpTrack.ExtraInfo = audioFrequency;
                            }

                            if (outputLine.Contains("audio_channels:"))
                            {
                                string audioChannels = ExtractProperty(outputLine, "audio_channels");
                                if (int.TryParse(audioChannels, out int tmpInt))
                                {
                                    tmpTrack.AudioChannels = tmpInt;
                                }

                                tmpTrack.ExtraInfo += ", Ch:" + audioChannels;
                            }

                            // in versions after v9.0.1, Mosu was kind enough to provide us with the minimum_timestamp property
                            // in order to determine the current track's delay
                            if (outputLine.Contains("minimum_timestamp:"))
                            {
                                if (long.TryParse(ExtractProperty(outputLine, "minimum_timestamp"), out long tmpLong))
                                {
                                    tmpTrack.MinimumTimestamp = tmpLong;
                                }
                            }

                            break;
                        case MkvTrackType.subtitles:
                            break;
                        default:
                            break;
                    }

                    finalList.Add(tmpTrack);
                }
                else if (outputLine.StartsWith("Attachment ID "))
                {
                    gMKVAttachment tmp = new gMKVAttachment();
                    tmp.ID = int.Parse(outputLine.Substring("Attachment ID".Length, outputLine.IndexOf(":") - "Attachment ID".Length).Trim());

                    // Extract filename between single quotes after "file name"
                    int fileNameStart = outputLine.IndexOf("file name");
                    if (fileNameStart >= 0)
                    {
                        int firstQuote = outputLine.IndexOf('\'', fileNameStart);
                        int lastQuote = outputLine.IndexOf('\'', firstQuote + 1);
                        if (firstQuote >= 0 && lastQuote > firstQuote)
                        {
                            tmp.Filename = outputLine.Substring(firstQuote + 1, lastQuote - firstQuote - 1).Trim();
                        }
                    }

                    // Extract file size after "size"
                    int sizeStart = outputLine.IndexOf("size");
                    if (sizeStart >= 0)
                    {
                        int sizeEnd = outputLine.IndexOf(',', sizeStart);
                        string sizePart = sizeEnd > sizeStart
                            ? outputLine.Substring(sizeStart + 4, sizeEnd - sizeStart - 4)
                            : outputLine.Substring(sizeStart + 4);
                        tmp.FileSize = sizePart.Replace("bytes", "").Trim();
                    }

                    // Extract mime type after "type"
                    int typeStart = outputLine.IndexOf("type");
                    if (typeStart >= 0)
                    {
                        int typeEnd = outputLine.IndexOf(',', typeStart);
                        string typePart = typeEnd > typeStart
                            ? outputLine.Substring(typeStart + 4, typeEnd - typeStart - 4)
                            : outputLine.Substring(typeStart + 4);
                        tmp.MimeType = typePart.Replace("'", "").Trim();
                    }

                    finalList.Add(tmp);
                }
                else if (outputLine.StartsWith("Chapters: "))
                {
                    gMKVChapter tmp = new gMKVChapter
                    {
                        ChapterCount = int.TryParse(
                            outputLine
                                .Replace("Chapters: ", "")
                                .Replace("entry", "")
                                .Replace("entries", "")
                                .Trim(), 
                            out int chapterCount)
                        ? chapterCount : 0
                    };

                    finalList.Add(tmp);
                }
            }

            return finalList;
        }

        private static string ExtractProperty(string line, string propertyName)
        {
            string propertyNameWithSuffix = propertyName + ":";
            int startIdx = line.IndexOf(propertyNameWithSuffix, StringComparison.Ordinal);
            if (startIdx < 0)
            {
                return "";
            }

            startIdx += propertyNameWithSuffix.Length;
            int endIdx = line.IndexOf(' ', startIdx);
            int bracketIdx = line.IndexOf(']', startIdx);

            // Find the nearest end character (space or ])
            if (endIdx == -1 
                || (bracketIdx != -1 && bracketIdx < endIdx))
            {
                endIdx = bracketIdx;
            }

            string value;
            if (endIdx == -1)
            {
                value = line.Substring(startIdx);
            }
            else
            {
                value = line.Substring(startIdx, endIdx - startIdx);
            }

            return gMKVHelper.UnescapeString(value).Trim();
        }

        /// Factory that creates a process output handler with a custom output action.
        /// </summary>
        /// <param name="outputAction">The action to perform with the received line of text.</param>
        /// <returns>A new Action<Process, string> that can be used as a handler.</returns>
        public Action<Process, string> CreateProcessOutputHandlerFactory(Action<string> outputAction, Action<string> errorAction)
        {
            // Return a new lambda expression that matches the Action<Process, string> signature.
            // This lambda "closes over" the outputAction parameter.
            return (process, line) => ProcessLineReceivedHandler(process, line, outputAction, errorAction);
        }

        private void ProcessLineReceivedHandler(
            Process sender, 
            string lineReceived, 
            Action<string> outputAction, 
            Action<string> errorAction)
        {
            if (!string.IsNullOrWhiteSpace(lineReceived))
            {
                // debug write the output line
                Debug.WriteLine(lineReceived);

                // log the output
                gMKVLogger.Log(lineReceived);

                // Call the outputAction with the received line
                outputAction(lineReceived);

                // check for errors
                if (lineReceived.Contains("Error:"))
                {
                    errorAction(lineReceived.Substring(lineReceived.IndexOf(":") + 1).Trim());
                }
            }
        }
    }
}
