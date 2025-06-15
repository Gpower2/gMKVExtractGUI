using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gMKVToolNix.Forms;
using gMKVToolNix.MkvExtract;
using gMKVToolNix.Segments;

namespace gMKVToolNix.Jobs
{
    public delegate void gMkvExtractMethod(object parameterList);

    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(List<gMKVSegment>))]
    [System.Xml.Serialization.XmlInclude(typeof(gMKVSegment))]
    [System.Xml.Serialization.XmlInclude(typeof(gMKVTrack))]
    [System.Xml.Serialization.XmlInclude(typeof(gMKVChapter))]
    [System.Xml.Serialization.XmlInclude(typeof(gMKVAttachment))]
    public class gMKVJob
    {
        public FormMkvExtractionMode ExtractionMode { get; set; }

        public string MKVToolnixPath { get; set; }

        public gMKVExtractSegmentsParameters ParametersList { get; set; }

        public gMKVJob(FormMkvExtractionMode argExtractionMode, string argMKVToolnixPath, gMKVExtractSegmentsParameters argParameters)
        {
            ExtractionMode = argExtractionMode;
            MKVToolnixPath = argMKVToolnixPath;
            ParametersList = argParameters;
        }

        public gMkvExtractMethod ExtractMethod(gMKVExtract argGmkvExtract) 
        {
            switch (ExtractionMode)
            {
                case FormMkvExtractionMode.Tracks:
                    return argGmkvExtract.ExtractMKVSegmentsThreaded;
                case FormMkvExtractionMode.Cue_Sheet:
                    return argGmkvExtract.ExtractMkvCuesheetThreaded;
                case FormMkvExtractionMode.Tags:
                    return argGmkvExtract.ExtractMkvTagsThreaded;
                case FormMkvExtractionMode.Timecodes:
                    return argGmkvExtract.ExtractMKVTimecodesThreaded;
                case FormMkvExtractionMode.Tracks_And_Timecodes:
                    return argGmkvExtract.ExtractMKVSegmentsThreaded;
                case FormMkvExtractionMode.Cues:
                    return argGmkvExtract.ExtractMKVCuesThreaded;
                case FormMkvExtractionMode.Tracks_And_Cues:
                    return argGmkvExtract.ExtractMKVSegmentsThreaded;
                case FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes:
                    return argGmkvExtract.ExtractMKVSegmentsThreaded;
                default:
                    throw new Exception("Unsupported Extraction Mode!");
            }
        }

        // For serialization only!!!
        internal gMKVJob() { }

        private string GetTracks(List<gMKVSegment> argSegmentList)
        {
            StringBuilder trackBuilder = new StringBuilder();
            foreach (gMKVSegment item in argSegmentList)
            {
                if (item is gMKVTrack track)
                {
                    trackBuilder.AppendFormat("[{0}:{1}]",
                        track.TrackType.ToString().Substring(0, 3),
                        track.TrackID);
                }
                else if (item is gMKVAttachment attachment)
                {
                    trackBuilder.AppendFormat("[Att:{0}]",                        
                        attachment.ID);
                }
                else if (item is gMKVChapter)
                {
                    trackBuilder.AppendFormat("[{0}]", "Chap");
                }
            }
            return trackBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder retValue = new StringBuilder();
            switch (ExtractionMode)
            {
                case FormMkvExtractionMode.Tracks:
                    retValue.AppendFormat("Tracks {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Cue_Sheet:
                    retValue.AppendFormat("Cue Sheet \r\n{0} \r\n{1}'",
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Tags:
                    retValue.AppendFormat("Tags \r\n{0} \r\n{1}",
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Timecodes:
                    retValue.AppendFormat("Timecodes {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Tracks_And_Timecodes:
                    retValue.AppendFormat("Tracks/Timecodes {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Cues:
                    retValue.AppendFormat("Cues {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Tracks_And_Cues:
                    retValue.AppendFormat("Tracks/Cues {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                case FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes:
                    retValue.AppendFormat("Tracks/Cues/Timecodes {0} \r\n{1} \r\n{2}",
                        GetTracks(ParametersList.MKVSegmentsToExtract),
                        Path.GetFileName(ParametersList.MKVFile), ParametersList.OutputDirectory);
                    break;
                default:
                    retValue.AppendFormat("Unknown job!!!");
                    break;
            }

            return retValue.ToString();
        }
    }
}
