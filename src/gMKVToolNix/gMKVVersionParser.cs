using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace gMKVToolNix
{
    public static class gMKVVersionParser
    {
        private static readonly Regex _versionRegEx = new Regex(
            @"\bv(?<major>\d+)\.(?<minor>\d+)(?:\.(?<patch>\d+))?\b", 
            RegexOptions.Compiled);

        private static readonly gMKVVersion _defaultEmptyVersion = new gMKVVersion()
        {
            FileMajorPart = 0,
            FileMinorPart = 0,
            FilePrivatePart = 0
        };

        public static gMKVVersion ParseVersionOutput(List<string> mkvtoolnixOutputLines)
        {
            if (mkvtoolnixOutputLines == null || !mkvtoolnixOutputLines.Any())
            {
                return _defaultEmptyVersion;
            }

            string fileMajorVersion = null;
            string fileMinorVersion = null;
            string filePrivateVersion = null;

            foreach (string outputLine in mkvtoolnixOutputLines)
            {
                if (string.IsNullOrWhiteSpace(outputLine))
                {
                    continue; // Skip empty or whitespace lines
                }

                Match match = _versionRegEx.Match(outputLine);
                if (match.Success)
                {
                    fileMajorVersion = match.Groups["major"].Value;
                    fileMinorVersion = match.Groups["minor"].Value;

                    string patch = match.Groups["patch"].Value;
                    if (!string.IsNullOrEmpty(patch))
                    {
                        filePrivateVersion = patch;
                    }
                    break;
                }
            }

            return new gMKVVersion()
            {
                FileMajorPart = int.TryParse(fileMajorVersion, out int majorVersion) ? majorVersion : 0,
                FileMinorPart = int.TryParse(fileMinorVersion, out int minorVersion) ? minorVersion : 0,
                FilePrivatePart = int.TryParse(filePrivateVersion, out int privateVersion) ? privateVersion : 0
            };
        }
    }
}
