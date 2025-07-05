using System;
using System.Collections;
using System.Globalization;
using System.Text;
using gMKVToolNix.Segments;

namespace gMKVToolNix.MkvMerge
{
    public static class CodecPrivateDataExtensions
    {
        public static byte[] HexStringToByteArray(this string hexString)
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

        public static string GetTrackCodecPrivate(this gMKVTrack track)
        {
            // Check if the track has CodecPrivateData
            // and it doesn't have a text representation of CodecPrivate
            if (string.IsNullOrWhiteSpace(track.CodecPrivateData)
                || string.IsNullOrWhiteSpace(track.CodecPrivate))
            {
                return null;
            }

            byte[] codecPrivateBytes = track.CodecPrivateData.HexStringToByteArray();
            if (track.TrackType == MkvTrackType.video)
            {
                if (track.CodecID == "V_MS/VFW/FOURCC")
                {
                    return string.Format("length {0} (FourCC: \"{1}\")"
                        , codecPrivateBytes.Length
                        ,
                        ((32 <= codecPrivateBytes[16]) && (127 > codecPrivateBytes[16]) 
                            ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[16] }) 
                            : "?") +
                        ((32 <= codecPrivateBytes[17]) && (127 > codecPrivateBytes[17]) 
                            ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[17] }) 
                            : "?") +
                        ((32 <= codecPrivateBytes[18]) && (127 > codecPrivateBytes[18]) 
                            ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[18] }) 
                            : "?") +
                        ((32 <= codecPrivateBytes[19]) && (127 > codecPrivateBytes[19]) 
                            ? Encoding.ASCII.GetString(new byte[] { codecPrivateBytes[19] }) 
                            : "?")
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

                    return string.Format("length {0} (h.264 profile: {1} @L{2}.{3})"
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
                        (codecPrivateBits[0] ? "1" : "0"), 
                        2);

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

                    return string.Format("length {0} (HEVC profile: {1} @L{2}.{3})"
                        , codecPrivateBytes.Length
                        , profileIdcString
                        , levelIdc / 3 / 10
                        , levelIdc / 3 % 10
                    );
                }
                else
                {
                    return $"length {codecPrivateBytes.Length}";
                }
            }
            else if (track.TrackType == MkvTrackType.audio)
            {
                if (track.CodecID == "A_MS/ACM")
                {
                    //UInt16 formatTag = BitConverter.ToUInt16(new byte[] { codecPrivateBytes[1], codecPrivateBytes[0] }, 0);
                    return string.Format("length {0} (format tag: 0x{1:x2}{2:x2})"
                        , codecPrivateBytes.Length
                        , codecPrivateBytes[0]
                        , codecPrivateBytes[1]
                    );
                }
                else
                {
                    return $"length {codecPrivateBytes.Length}";
                }
            }
            else
            {
                return $"length {codecPrivateBytes.Length}";
            }
        }
    }
}
