using System;
using System.Collections;
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

        private class OptionValue
        {
            public string Parameter { get; set; }

            public MkvMergeOptions Option { get; set; }

            public OptionValue(MkvMergeOptions opt, string par)
            {
                Option = opt;
                Parameter = par;
            }
        }

        /// <summary>
        /// Gets the mkvinfo executable filename
        /// </summary>
        public static string MKV_MERGE_FILENAME
        {
            get { return gMKVHelper.IsOnLinux ? "mkvmerge" : "mkvmerge.exe"; }
        }

        private readonly string _MKVToolnixPath = "";
        private readonly string _MKVMergeFilename = "";
        private readonly List<gMKVSegment> _SegmentList = new List<gMKVSegment>();
        private readonly StringBuilder _MKVMergeOutput = new StringBuilder();
        private readonly StringBuilder _ErrorBuilder = new StringBuilder();
        private gMKVVersion _Version = null;

        public gMKVMerge(string mkvToonlixPath)
        {
            if (string.IsNullOrWhiteSpace(mkvToonlixPath))
            {
                throw new Exception("The MKVToolNix path was not provided!");
            }
            if (!Directory.Exists(mkvToonlixPath))
            {
                throw new Exception(string.Format("The MKVToolNix path {0} does not exist!", mkvToonlixPath));
            }
            _MKVToolnixPath = mkvToonlixPath;
            _MKVMergeFilename = Path.Combine(_MKVToolnixPath, MKV_MERGE_FILENAME);
            if (!File.Exists(_MKVMergeFilename))
            {
                throw new Exception(string.Format("Could not find {0}!" + Environment.NewLine + "{1}", MKV_MERGE_FILENAME, _MKVMergeFilename));
            }
        }

        public List<gMKVSegment> GetMKVSegments(string argMKVFile)
        {
            // check for existence of MKVMerge
            if (!File.Exists(_MKVMergeFilename)) 
            { 
                throw new Exception(string.Format("Could not find {0}!" + Environment.NewLine + "{1}", MKV_MERGE_FILENAME, _MKVMergeFilename)); 
            }
            // First clear the segment list
            _SegmentList.Clear();
            // Clear the mkvmerge output
            _MKVMergeOutput.Length = 0;
            // Clear the error builder
            _ErrorBuilder.Length = 0;
            // Execute the mkvmerge
            ExecuteMkvMerge(null, argMKVFile, myProcess_OutputDataReceived);
            // Start the parsing of the output
            // Since MKVToolNix v9.6.0, start parsing the JSON identification info
            if (_Version == null)
            {
                _Version = GetMKVMergeVersion();
            }

            if (_Version.FileMajorPart > 9 ||
                (_Version.FileMajorPart == 9 && _Version.FileMinorPart >= 6))
            {
                ParseMkvMergeJsonOutput();
            }
            else
            {
                ParseMkvMergeOutput();
            }

            // Add the file properties in gMKVSegmentInfo
            gMKVSegmentInfo seg = _SegmentList.OfType<gMKVSegmentInfo>().FirstOrDefault();
            if (seg != null)
            {
                seg.Directory = Path.GetDirectoryName(argMKVFile);
                seg.Filename = Path.GetFileName(argMKVFile);
            }

            return _SegmentList;
        }

        public bool FindDelays(List<gMKVSegment> argSegmentList)
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

        private static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 == 1)
            {
                throw new ArgumentException($"The binary key cannot have an odd number of digits: {hexString}");
            }

            byte[] hexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }

        public void FindCodecPrivate(List<gMKVSegment> argSegmentList)
        {
            foreach (gMKVTrack track in argSegmentList.OfType<gMKVTrack>())
            {
                // Check if the track has CodecPrivateData
                // and it doesn't have a text representation of CodecPrivate
                if (!string.IsNullOrWhiteSpace(track.CodecPrivateData)
                    && string.IsNullOrWhiteSpace(track.CodecPrivate))
                {
                    byte[] codecPrivateBytes = HexStringToByteArray(track.CodecPrivateData);
                    if (track.TrackType == MkvTrackType.video)
                    {
                        if (track.CodecID == "V_MS/VFW/FOURCC")
                        {
                            track.CodecPrivate = string.Format("length {0} (FourCC: \"{1}\")"
                                , codecPrivateBytes.Length
                                ,
                                ((32 <= codecPrivateBytes[16]) && (127 > codecPrivateBytes[16]) ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[16] }) : "?") +
                                ((32 <= codecPrivateBytes[17]) && (127 > codecPrivateBytes[17]) ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[17] }) : "?") +
                                ((32 <= codecPrivateBytes[18]) && (127 > codecPrivateBytes[18]) ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[18] }) : "?") +
                                ((32 <= codecPrivateBytes[19]) && (127 > codecPrivateBytes[19]) ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[19] }) : "?")
                            );
                        }
                        else if (track.CodecID == "V_MPEG4/ISO/AVC")
                        {
                            int profileIdc = codecPrivateBytes[1];
                            int levelIdc = codecPrivateBytes[3];

                            string profileIdcString;

                            switch (profileIdc)
                            {
                                case 44:
                                    profileIdcString = "CAVLC 4:4:4 Intra";
                                    break;
                                case 66:
                                    profileIdcString = "Baseline";
                                    break;
                                case 77:
                                    profileIdcString = "Main";
                                    break;
                                case 83:
                                    profileIdcString = "Scalable Baseline";
                                    break;
                                case 86:
                                    profileIdcString = "Scalable High";
                                    break;
                                case 88:
                                    profileIdcString = "Extended";
                                    break;
                                case 100:
                                    profileIdcString = "High";
                                    break;
                                case 110:
                                    profileIdcString = "High 10";
                                    break;
                                case 118:
                                    profileIdcString = "Multiview High";
                                    break;
                                case 122:
                                    profileIdcString = "High 4:2:2";
                                    break;
                                case 128:
                                    profileIdcString = "Stereo High";
                                    break;
                                case 144:
                                    profileIdcString = "High 4:4:4";
                                    break;
                                case 244:
                                    profileIdcString = "High 4:4:4 Predictive";
                                    break;
                                default:
                                    profileIdcString = "Unknown";
                                    break;
                            }

                            track.CodecPrivate = string.Format("length {0} (h.264 profile: {1} @L{2}.{3})"
                                , codecPrivateBytes.Length
                                , profileIdcString
                                , levelIdc / 10
                                , levelIdc % 10
                            );
                        }
                        else if (track.CodecID == "V_MPEGH/ISO/HEVC")
                        {
                            BitArray codecPrivateBits = new BitArray(new byte[] { codecPrivateBytes[1] });

                            int profileIdc = Convert.ToInt32(
                                (codecPrivateBits[4] ? "1" : "0") +
                                (codecPrivateBits[3] ? "1" : "0") +
                                (codecPrivateBits[2] ? "1" : "0") +
                                (codecPrivateBits[1] ? "1" : "0") +
                                (codecPrivateBits[0] ? "1" : "0"), 2);

                            int levelIdc = codecPrivateBytes[12];

                            string profileIdcString;

                            switch (profileIdc)
                            {
                                case 1:
                                    profileIdcString = "Main";
                                    break;
                                case 2:
                                    profileIdcString = "Main 10";
                                    break;
                                case 3:
                                    profileIdcString = "Main Still Picture";
                                    break;
                                default:
                                    profileIdcString = "Unknown";
                                    break;
                            }

                            track.CodecPrivate = string.Format("length {0} (HEVC profile: {1} @L{2}.{3})"
                                , codecPrivateBytes.Length
                                , profileIdcString
                                , levelIdc / 3 / 10
                                , levelIdc / 3 % 10
                            );
                        }
                        else
                        {
                            track.CodecPrivate = $"length {codecPrivateBytes.Length}";
                        }
                    }
                    else if (track.TrackType == MkvTrackType.audio)
                    {
                        if (track.CodecID == "A_MS/ACM")
                        {
                            //UInt16 formatTag = BitConverter.ToUInt16(new byte[] { codecPrivateBytes[1], codecPrivateBytes[0] }, 0);
                            track.CodecPrivate = string.Format("length {0} (format tag: 0x{1:x2}{2:x2})"
                                , codecPrivateBytes.Length
                                , codecPrivateBytes[0]
                                , codecPrivateBytes[1]
                            );
                        }
                        else
                        {
                            track.CodecPrivate = $"length {codecPrivateBytes.Length}";
                        }
                    }
                    else
                    {
                        track.CodecPrivate = $"length {codecPrivateBytes.Length}";
                    }
                }
            }
        }

        public gMKVVersion GetMKVMergeVersion()
        {
            if (_Version != null)
            {
                return _Version;
            }

            // check for existence of mkvmerge
            if (!File.Exists(_MKVMergeFilename))
            {
                throw new Exception($"Could not find {MKV_MERGE_FILENAME}!{Environment.NewLine}{_MKVMergeFilename}");
            }

            if (gMKVHelper.IsOnLinux)
            {
                // When on Linux, we need to run mkvmerge

                // Clear the mkvmerge output
                _MKVMergeOutput.Length = 0;
                // Clear the error builder
                _ErrorBuilder.Length = 0;

                // Execute mkvmerge
                List<OptionValue> options = new List<OptionValue>
                {
                    new OptionValue(MkvMergeOptions.version, "")
                };

                using (Process myProcess = new Process())
                {
                    // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                    // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                    options.Add(new OptionValue(MkvMergeOptions.ui_language, "en_US"));

                    ProcessStartInfo myProcessInfo = new ProcessStartInfo
                    {
                        FileName = _MKVMergeFilename,
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
                    gMKVLogger.Log($"\"{_MKVMergeFilename}\" {myProcessInfo.Arguments}");

                    // Start the mkvmerge process
                    myProcess.Start();

                    // Read the Standard output character by character
                    gMKVHelper.ReadStreamPerCharacter(myProcess, myProcess_OutputDataReceived);

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
                            myProcess.ExitCode, _ErrorBuilder.ToString()));
                    }
                }

                // Parse version info
                ParseVersionOutput();

                // Clear the mkvmerge output
                _MKVMergeOutput.Length = 0;
            }
            else
            {
                // When on Windows, we can use FileVersionInfo.GetVersionInfo
                var version = FileVersionInfo.GetVersionInfo(_MKVMergeFilename);
                _Version = new gMKVVersion()
                {
                    FileMajorPart = version.FileMajorPart,
                    FileMinorPart = version.FileMinorPart,
                    FilePrivatePart = version.FilePrivatePart
                };
            }

            if (_Version != null)
            {
                gMKVLogger.Log(string.Format("Detected mkvmerge version: {0}.{1}.{2}",
                    _Version.FileMajorPart,
                    _Version.FileMinorPart,
                    _Version.FilePrivatePart
                ));
            }

            return _Version;
        }

        private void ExecuteMkvMerge(List<OptionValue> argOptionList, string argMKVFile, DataReceivedEventHandler argHandler)
        {
            using (Process myProcess = new Process())
            {
                List<OptionValue> optionList = new List<OptionValue>();

                // Check the file version of the mkvmerge.exe
                if (_Version == null)
                {
                    _Version = GetMKVMergeVersion();
                }

                string LC_ALL = "";
                string LANG = "";
                string LC_MESSAGES = "";

                // if on Linux, the language output must be defined from the environment variables LC_ALL, LANG, and LC_MESSAGES
                // After talking with Mosu, the language output is defined from ui-language, with different language codes for Windows and Linux
                if (gMKVHelper.IsOnLinux)
                {
                    optionList.Add(new OptionValue(MkvMergeOptions.ui_language, "en_US"));
                }
                else
                {
                    optionList.Add(new OptionValue(MkvMergeOptions.ui_language, "en"));
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
                        optionList.Add(new OptionValue(MkvMergeOptions.identify, ""));
                        optionList.Add(new OptionValue(MkvMergeOptions.identification_format, "json"));
                    }
                    else
                    {
                        // For previous mkvmerge versions, keep compatibility
                        optionList.Add(new OptionValue(MkvMergeOptions.identify_verbose, ""));

                        // Before JSON output, the safest way to ensure English output on Linux is throught the EnvironmentVariables
                        if (gMKVHelper.IsOnLinux)
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
                    myProcessInfo.Arguments = string.Format("{0} \"{1}\"", ConvertOptionValueListToString(optionList), argMKVFile);
                }
                else
                {
                    myProcessInfo.Arguments = ConvertOptionValueListToString(optionList);
                }

                myProcess.StartInfo = myProcessInfo;

                Debug.WriteLine(myProcessInfo.Arguments);
                gMKVLogger.Log(string.Format("\"{0}\" {1}", _MKVMergeFilename, myProcessInfo.Arguments));

                // Start the mkvinfo process
                myProcess.Start();

                // Read the Standard output character by character
                gMKVHelper.ReadStreamPerCharacter(myProcess, argHandler);

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

                // Before JSON output, the safest way to ensure English output on Linux is throught the EnvironmentVariables
                if (gMKVHelper.IsOnLinux)
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

        private void ParseMkvMergeJsonOutput()
        {
            // Read the JSON output data to a JObject
            JObject jsonObject = JObject.Parse(_MKVMergeOutput.ToString());

            // Create temporary Lists for the segments
            List<gMKVSegment> chapters = new List<gMKVSegment>();
            List<gMKVSegment> attachments = new List<gMKVSegment>();
            List<gMKVSegment> tracks = new List<gMKVSegment>();

            // Parse all the children tokens accordingly
            foreach (JToken token in jsonObject.Children())
            {
                if (!(token is JProperty))
                {
                    continue;
                }

                JProperty jProp = token as JProperty;
                if (jProp == null || string.IsNullOrWhiteSpace(jProp.Name) || !jProp.HasValues)
                {
                    continue;
                }

                string pName = jProp.Name.ToLower().Trim();
                if (pName == "chapters")
                {
                    foreach (JToken entry in jProp)
                    {
                        if (entry is JArray && (entry as JArray).Count > 0)
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
                                if (propertyAttachmentToken is JProperty)
                                {
                                    JProperty prop = propertyAttachmentToken as JProperty;
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
                            if (!(value is JProperty))
                            {
                                continue;
                            }

                            JProperty valueProperty = value as JProperty;
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
                                        if (!(childPreFinalProperty is JProperty))
                                        {
                                            continue;
                                        }
                                        JProperty childFinalProperty = childPreFinalProperty as JProperty;
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

                    _SegmentList.Add(tmpSegInfo);
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
                                if (!(childPreFinalProperty is JProperty))
                                {
                                    continue;
                                }
                                JProperty childFinalProperty = childPreFinalProperty as JProperty;
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
                                            if (!propertyFinalChild.HasValues || !(propertyFinalChild is JProperty))
                                            {
                                                continue;
                                            }
                                            JProperty propertyFinal = propertyFinalChild as JProperty;
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
                                        else if (!string.IsNullOrWhiteSpace(audioChannels) || !string.IsNullOrWhiteSpace(audioFrequency))
                                        {
                                            if (!string.IsNullOrWhiteSpace(audioChannels) && !string.IsNullOrWhiteSpace(audioFrequency))
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
                _SegmentList.Add(seg);
            }
            foreach (gMKVSegment seg in attachments)
            {
                _SegmentList.Add(seg);
            }
            foreach (gMKVSegment seg in chapters)
            {
                _SegmentList.Add(seg);
            }
        }

        private void ParseMkvMergeOutput()
        {
            // start the loop for each line of the output
            foreach (string outputLine in _MKVMergeOutput.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
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
                        _SegmentList.Add(tmpSegInfo);
                    }
                }
                else if (outputLine.StartsWith("Track ID "))
                {
                    int trackID = int.TryParse(outputLine.Substring(0, outputLine.IndexOf(":")).Replace("Track ID", "").Trim(), out int trackId) ? trackId : 0;
                    // Check if there is already a track with the same TrackID (workaround for a weird bug in MKVToolnix v4 when identifying files from AviDemux)
                    bool trackFound = false;
                    foreach (gMKVSegment tmpSeg in _SegmentList)
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
                        TrackType = (MkvTrackType)Enum.Parse(typeof(MkvTrackType), outputLine.Substring(outputLine.IndexOf(":") + 1, outputLine.IndexOf("(") - outputLine.IndexOf(":") - 1).Trim()),
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

                    _SegmentList.Add(tmpTrack);
                }
                else if (outputLine.StartsWith("Attachment ID "))
                {
                    gMKVAttachment tmp = new gMKVAttachment();
                    tmp.ID = int.Parse(outputLine.Substring(0, outputLine.IndexOf(":")).Replace("Attachment ID", "").Trim());
                    tmp.Filename = outputLine.Substring(outputLine.IndexOf("file name")).Replace("file name", "");
                    tmp.Filename = tmp.Filename.Substring(tmp.Filename.IndexOf("'") + 1, tmp.Filename.LastIndexOf("'") - 2).Trim();
                    tmp.FileSize = outputLine.Substring(outputLine.IndexOf("size")).Replace("size", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("bytes", "").Trim();
                    tmp.MimeType = outputLine.Substring(outputLine.IndexOf("type")).Replace("type", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("'", "").Trim();

                    _SegmentList.Add(tmp);
                }
                else if (outputLine.StartsWith("Chapters: "))
                {
                    gMKVChapter tmp = new gMKVChapter
                    {
                        ChapterCount = int.TryParse(outputLine.Replace("Chapters: ", "").Replace("entry", "").Replace("entries", "").Trim(), out int chapterCount)
                        ? chapterCount : 0
                    };

                    _SegmentList.Add(tmp);
                }
            }
        }

        private static string ExtractProperty(string line, string propertyName)
        {
            if (!line.Contains(propertyName + ":"))
            {
                return "";
            }

            string endCharacter = "";
            string propertyPart = line.Substring(line.IndexOf(propertyName + ":"));
            if (propertyPart.Contains(" "))
            {
                endCharacter = " ";
            }
            else if (propertyPart.Contains("]"))
            {
                endCharacter = "]";
            }

            return gMKVHelper.UnescapeString(
                propertyPart
                .Substring(0, string.IsNullOrEmpty(endCharacter) ? propertyPart.Length : propertyPart.IndexOf(endCharacter))
                .Replace(propertyName + ":", ""))
                .Trim();
        }

        private void ParseVersionOutput()
        {
            string fileMajorVersion = "0";
            string fileMinorVersion = "0";
            string filePrivateVersion = "0";
            if (_MKVMergeOutput != null && _MKVMergeOutput.Length > 0)
            {
                string[] outputLines = _MKVMergeOutput.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string outputLine in outputLines)
                {
                    if (outputLine.StartsWith("mkvmerge v"))
                    {
                        string versionString = outputLine.Substring(9);
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
                    // add the line to the output stringbuilder
                    _MKVMergeOutput.AppendLine(e.Data);
                    // check for errors
                    if (e.Data.Contains("Error:"))
                    {
                        _ErrorBuilder.AppendLine(e.Data.Substring(e.Data.IndexOf(":") + 1).Trim());
                    }
                    // debug write the output line
                    Debug.WriteLine(e.Data);
                    // log the output
                    gMKVLogger.Log(e.Data);
                }
            }
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

        private static string ConvertEnumOptionToStringOption(MkvMergeOptions enumOption)
        {
            return $"--{enumOption}".Replace("_", "-");
        }
    }
}
