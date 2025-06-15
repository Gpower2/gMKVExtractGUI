using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using gMKVToolNix.Log;
using gMKVToolNix.MkvInfo;
using gMKVToolNix.MkvMerge;
using gMKVToolNix.Segments;
using Microsoft.Win32;

namespace gMKVToolNix
{
    public static class gMKVHelper
    {
        private static bool? _isOnLinux = null;

        /// <summary>
        /// Returns if the running Platform is Linux Or MacOSX
        /// </summary>
        public static bool IsOnLinux
        {
            get
            {
                if (_isOnLinux.HasValue)
                {
                    return _isOnLinux.Value;
                }

                PlatformID myPlatform = Environment.OSVersion.Platform;

                // 128 is Mono 1.x specific value for Linux systems, so it's there to provide compatibility
                _isOnLinux = (myPlatform == PlatformID.Unix) || (myPlatform == PlatformID.MacOSX) || ((int)myPlatform == 128);

                return _isOnLinux.Value;
            }
        }

        private static string _mkvMergeGuiFilename = null;

        /// <summary>
        /// Gets the mkvmerge GUI executable filename
        /// </summary>
        public static string MKV_MERGE_GUI_FILENAME 
        {
            get 
            { 
                if (_mkvMergeGuiFilename != null)
                {
                    return _mkvMergeGuiFilename;
                }

                _mkvMergeGuiFilename = IsOnLinux ? "mmg" : "mmg.exe";

                return _mkvMergeGuiFilename;
            }
        }

        private static string _mkvMergeNewGuiFilename = null;

        /// <summary>
        /// Gets the new mkvmerge GUI executable filename
        /// </summary>
        public static string MKV_MERGE_NEW_GUI_FILENAME
        {
            get 
            { 
                if (_mkvMergeNewGuiFilename != null)
                {
                    return _mkvMergeNewGuiFilename;
                }

                _mkvMergeNewGuiFilename = IsOnLinux ? "mkvmerge" : "mkvmerge.exe";

                return _mkvMergeNewGuiFilename;
            }
        }

        /// <summary>
        /// Unescapes string from mkvtoolnix output
        /// </summary>
        /// <param name="argString"></param>
        /// <returns></returns>
        public static string UnescapeString(string argString)
        {
            return argString.
                Replace(@"\s", " ").
                Replace(@"\2", "\"").
                Replace(@"\c", ":").
                Replace(@"\h", "#").
                Replace(@"\\", @"\").
                Replace(@"\b", "[").
                Replace(@"\B", "]");
        }

        /// <summary>
        /// Escapes string from mkvtoolnix output
        /// </summary>
        /// <param name="argString"></param>
        /// <returns></returns>
        public static string EscapeString(string argString)
        {
            return argString.
                Replace(" ", @"\s").
                Replace("\"", @"\2").
                Replace(":", @"\c").
                Replace("#", @"\h").
                Replace(@"\", @"\\").
                Replace("[", @"\b").
                Replace("]", @"\B");
        }

        /// <summary>
        /// Returns the path from MKVToolnix.
        /// It tries to find it via the registry keys.
        /// If it doesn't find it, it throws an exception.
        /// </summary>
        /// <returns></returns>
        public static string GetMKVToolnixPathViaRegistry()
        {
            // Check if we are on Linux, so we don't have to check the registry
            if (gMKVHelper.IsOnLinux)
            {
                throw new Exception("Running on Linux...");
            }

            RegistryKey regMkvToolnix = null;
            string valuePath = "";
            bool subKeyFound = false;
            bool valueFound = false;

            // First check for Installed MkvToolnix
            // First check Win32 registry
            RegistryKey regUninstall = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").
                OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Uninstall");

            foreach (string subKeyName in regUninstall.GetSubKeyNames())
            {
                if (subKeyName.Equals("MKVToolNix", StringComparison.OrdinalIgnoreCase))
                {
                    subKeyFound = true;
                    regMkvToolnix = regUninstall.OpenSubKey("MKVToolNix");
                    break;
                }
            }

            // if sub key was found, try to get the executable path
            if (subKeyFound)
            {
                foreach (string valueName in regMkvToolnix.GetValueNames())
                {
                    if (valueName.Equals("DisplayIcon", StringComparison.OrdinalIgnoreCase))
                    {
                        valueFound = true;
                        valuePath = (string)regMkvToolnix.GetValue(valueName);
                        gMKVLogger.Log($"Found MKVToolNix in registry (Win32): {valuePath}");
                        break;
                    }
                }
            }

            // if value was not found, search Win64 registry
            if (!valueFound)
            {
                subKeyFound = false;

                regUninstall = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Wow6432Node").OpenSubKey("Microsoft").
                    OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Uninstall");

                foreach (string subKeyName in regUninstall.GetSubKeyNames())
                {
                    if (subKeyName.Equals("MKVToolNix", StringComparison.OrdinalIgnoreCase))
                    {
                        subKeyFound = true;
                        regMkvToolnix = regUninstall.OpenSubKey("MKVToolNix");
                        break;
                    }
                }

                // if sub key was found, try to get the executable path
                if (subKeyFound)
                {
                    foreach (string valueName in regMkvToolnix.GetValueNames())
                    {
                        if (valueName.Equals("DisplayIcon", StringComparison.OrdinalIgnoreCase))
                        {
                            valueFound = true;
                            valuePath = (string)regMkvToolnix.GetValue(valueName);
                            gMKVLogger.Log($"Found MKVToolNix in registry (Win64): {valuePath}");
                            break;
                        }
                    }
                }
            }

            // if value was still not found, we may have portable installation
            // let's try the CURRENT_USER registry
            if (!valueFound)
            {
                RegistryKey regSoftware = Registry.CurrentUser.OpenSubKey("Software");
                subKeyFound = false;
                foreach (string subKey in regSoftware.GetSubKeyNames())
                {
                    if (subKey.Equals("mkvmergeGUI", StringComparison.OrdinalIgnoreCase))
                    {
                        subKeyFound = true;
                        regMkvToolnix = regSoftware.OpenSubKey("mkvmergeGUI");
                        break;
                    }
                }

                // if we didn't find the MkvMergeGUI key, all hope is lost
                if (!subKeyFound)
                {
                    throw new Exception($"Couldn't find MKVToolNix in your system!{Environment.NewLine}Please download and install it or provide a manual path!");
                }

                RegistryKey regGui = null;
                bool foundGuiKey = false;
                foreach (string subKey in regMkvToolnix.GetSubKeyNames())
                {
                    if (subKey.Equals("GUI", StringComparison.OrdinalIgnoreCase))
                    {
                        foundGuiKey = true;
                        regGui = regMkvToolnix.OpenSubKey("GUI");
                        break;
                    }
                }
                // if we didn't find the GUI key, all hope is lost
                if (!foundGuiKey)
                {
                    throw new Exception("Found MKVToolNix in your system but not the registry Key GUI!");
                }

                foreach (string valueName in regGui.GetValueNames())
                {
                    if (valueName.Equals("mkvmerge_executable", StringComparison.OrdinalIgnoreCase))
                    {
                        valueFound = true;
                        valuePath = (string)regGui.GetValue("mkvmerge_executable");
                        gMKVLogger.Log($"Found MKVToolNix in registry (CURRENT_USER): {valuePath}");
                        break;
                    }
                }
                // if we didn't find the mkvmerge_executable value, all hope is lost
                if (!valueFound)
                {
                    throw new Exception("Found MKVToolNix in your system but not the registry value mkvmerge_executable!");
                }
            }

            // Now that we found a value (otherwise we would not be here, an exception would have been thrown)
            // let's check if it's valid
            if (!File.Exists(valuePath))
            {
                throw new Exception($"Found a registry value ({valuePath}) for MKVToolNix in your system but it is not valid!");
            }

            // Everything is A-OK! Return the valid Directory value! :)
            return Path.GetDirectoryName(valuePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argMkvToolnixPath"></param>
        /// <param name="argInputFile"></param>
        /// <returns></returns>
        public static List<gMKVSegment> GetMergedMkvSegmentList(string argMkvToolnixPath, string argInputFile)
        {
            gMKVMerge g = new gMKVMerge(argMkvToolnixPath);
            gMKVInfo gInfo = new gMKVInfo(argMkvToolnixPath);

            List<gMKVSegment> segmentList = g.GetMKVSegments(argInputFile);

            // Check if information was found in mkvmerge output
            bool segmentInfoWasFound = false;
            foreach (gMKVSegment seg in segmentList)
            {
                if (seg is gMKVSegmentInfo)
                {
                    segmentInfoWasFound = true;
                    break;
                }
            }

            // Check if codec_private_data was found in mkvmerge output
            bool codecPrivateDataWasFound = false;
            foreach (gMKVSegment seg in segmentList)
            {
                if (seg is gMKVTrack)
                {
                    if (!string.IsNullOrWhiteSpace(((gMKVTrack)seg).CodecPrivateData))
                    {
                        codecPrivateDataWasFound = true;
                        break;
                    }                    
                }
            }

            if (!segmentInfoWasFound || !codecPrivateDataWasFound)
            {
                List<gMKVSegment> segmentListInfo = gInfo.GetMKVSegments(argInputFile);
                foreach (gMKVSegment seg in segmentListInfo)
                {
                    if (seg is gMKVSegmentInfo && !segmentInfoWasFound)
                    {
                        segmentList.Insert(0, seg);
                    }
                    else if (seg is gMKVTrack && !codecPrivateDataWasFound)
                    {
                        // Update CodecPrivate info from mkvinfo to mkvextract segments
                        foreach (gMKVSegment seg2 in segmentList)
                        {
                            if (seg2 is gMKVTrack)
                            {
                                if (((gMKVTrack)seg2).TrackID == ((gMKVTrack)seg).TrackID)
                                {
                                    if (!string.IsNullOrWhiteSpace(((gMKVTrack)seg).CodecPrivate))
                                    {
                                        ((gMKVTrack)seg2).CodecPrivate = ((gMKVTrack)seg).CodecPrivate;
                                    }
                                    if (((gMKVTrack)seg2).TrackType == MkvTrackType.video)
                                    {
                                        if (((gMKVTrack)seg2).VideoPixelWidth < ((gMKVTrack)seg).VideoPixelWidth)
                                        {
                                            ((gMKVTrack)seg2).VideoPixelWidth = ((gMKVTrack)seg).VideoPixelWidth;
                                        }
                                        if (((gMKVTrack)seg2).VideoPixelHeight < ((gMKVTrack)seg).VideoPixelHeight)
                                        {
                                            ((gMKVTrack)seg2).VideoPixelHeight = ((gMKVTrack)seg).VideoPixelHeight;
                                        }
                                        if (!string.IsNullOrWhiteSpace(((gMKVTrack)seg).ExtraInfo))
                                        {
                                            ((gMKVTrack)seg2).ExtraInfo = ((gMKVTrack)seg).ExtraInfo;
                                        }
                                    }
                                    else if (((gMKVTrack)seg2).TrackType == MkvTrackType.audio)
                                    {
                                        if (((gMKVTrack)seg2).AudioChannels < ((gMKVTrack)seg).AudioChannels)
                                        {
                                            ((gMKVTrack)seg2).AudioChannels = ((gMKVTrack)seg).AudioChannels;
                                        }
                                        if (((gMKVTrack)seg2).AudioSamplingFrequency < ((gMKVTrack)seg).AudioSamplingFrequency)
                                        {
                                            ((gMKVTrack)seg2).AudioSamplingFrequency = ((gMKVTrack)seg).AudioSamplingFrequency;
                                        }
                                        if (!string.IsNullOrWhiteSpace(((gMKVTrack)seg).ExtraInfo))
                                        {
                                            ((gMKVTrack)seg2).ExtraInfo = ((gMKVTrack)seg).ExtraInfo;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Try to determine the delays from mkvmerge info
            if (!g.FindDelays(segmentList))
            {
                // If we couldn't determine the delays from mkvmerge info, then we use mkvinfo
                gInfo.FindAndSetDelays(segmentList, argInputFile);
            }

            // Translate codec_private_data in codec_private information
            g.FindCodecPrivate(segmentList);

            return segmentList;
        }

        private static readonly FieldInfo _dataReceivedEventArgsFieldInfo = typeof(DataReceivedEventArgs)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)[0];

        /// <summary>
        /// Creates a DataReceivedEventArgs instance with the given Data.
        /// </summary>
        /// <param name="argData"></param>
        /// <returns></returns>
        public static DataReceivedEventArgs GetDataReceivedEventArgs(object argData)
        {
            DataReceivedEventArgs eventArgs = (DataReceivedEventArgs)System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(typeof(DataReceivedEventArgs));

            _dataReceivedEventArgsFieldInfo.SetValue(eventArgs, argData);

            return eventArgs;
        }

        /// <summary>
        /// Reads a Process's standard output stream character by character and calls the user defined method for each line
        /// </summary>
        /// <param name="argProcess"></param>
        /// <param name="argHandler"></param>
        public static void ReadStreamPerCharacter(Process argProcess, DataReceivedEventHandler argHandler)
        {
            StreamReader reader = argProcess.StandardOutput;
            StringBuilder line = new StringBuilder();
            while (true)
            {
                if (!reader.EndOfStream)
                {
                    char c = (char)reader.Read();
                    if (c == '\r')
                    {
                        if ((char)reader.Peek() == '\n')
                        {
                            // consume the next character
                            reader.Read();
                        }

                        argHandler(argProcess, GetDataReceivedEventArgs(line.ToString()));
                        line.Length = 0;
                    }
                    else if (c == '\n')
                    {
                        argHandler(argProcess, GetDataReceivedEventArgs(line.ToString()));
                        line.Length = 0;
                    }
                    else
                    {
                        line.Append(c);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
