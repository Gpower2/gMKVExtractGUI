using System;
using System.IO;
using System.Linq;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvExtract
{
    public static class gMKVExtractExtensions
    {
        private static readonly char[] _invalidFilenameChars = Path.GetInvalidFileNameChars();
        private static readonly string _directorySeparator = Path.DirectorySeparatorChar.ToString();

        public static string ReplaceFilenamePlaceholders(
            this gMKVSegment argSeg, 
            string argMKVFile, 
            string argFilenamePattern)
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
                finalFilename = finalFilename.Replace(gMKVExtractFilenamePatterns.TrackForced, track.Forced ? "FORCED" : "");

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
            this gMKVSegment argSeg
            , string argOutputDirectory
            , string argMKVFile
            , gMKVExtractFilenamePatterns argFilenamePatterns
            , bool argOverwriteExistingFile
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
                            outputFileExtension = segTrack.GetVideoFileExtensionFromCodecID();
                            replacedFilePattern = argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.VideoTrackFilenamePattern);
                            break;
                        case MkvTrackType.audio:
                            // get the extension of the output via the CODEC_ID of the track
                            outputFileExtension = segTrack.GetAudioFileExtensionFromCodecID();
                            replacedFilePattern = argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.AudioTrackFilenamePattern);
                            break;
                        case MkvTrackType.subtitles:
                            // get the extension of the output via the CODEC_ID of the track
                            outputFileExtension = segTrack.GetSubtitleFileExtensionFromCodecID();
                            replacedFilePattern = argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.SubtitleTrackFilenamePattern);
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
                    replacedFilePattern = argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.TagsFilenamePattern);

                    outputFilename = Path.Combine(
                        outputDirectory,
                        string.Format("{0}.{1}", replacedFilePattern, "xml"));
                    break;
                case MkvExtractModes.attachments:
                    if (!(argSeg is gMKVAttachment))
                    {
                        throw new Exception("Called GetOutputFilename without attachment!");
                    }
                    outputFilename = Path.Combine(
                        outputDirectory,
                        argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.AttachmentFilenamePattern)
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
                            outputFileExtension = "txt";
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
                                argSeg.ReplaceFilenamePlaceholders(argMKVFile, argFilenamePatterns.ChapterFilenamePattern),
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

            return outputFilename.GetOutputFilename(argOverwriteExistingFile);
        }

        private static string GetVideoFileExtensionFromCodecID(this gMKVTrack argTrack)
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

        private static string GetAudioFileExtensionFromCodecID(this gMKVTrack argTrack)
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

        private static string GetSubtitleFileExtensionFromCodecID(this gMKVTrack argTrack)
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
    }
}
