using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace gMKVToolNix.Translator.Console.Commands
{
    public class ScanCommand
    {
        private static readonly Regex StringRegex = new Regex(@"\""(.+?)\""", RegexOptions.Compiled);
        private static readonly string[] IgnoredSubstrings = 
        { 
            "GetString(", 
            ".Service.GetString", 
            "case \"", 
            "[", 
            "SELECT ", 
            "UPDATE ", 
            "INSERT ", 
            "DELETE ",
            ".Contains(",
            ".StartsWith(",
            ".EndsWith(",
            ".Equals(",
            "Replace(",
            ".IndexOf(",
            ".Substring(",
            ".SetEnvironmentVariable(",
            ".GetEnvironmentVariable(",
            "PlatformExtensions.",
            ".OpenSubKey(",
            "Path.Combine(",
            "ExtractProperty(",
            ".Parse(",
            ".ParseExact(",
            ".GetProperty(",
            "HRESULT",
            "gForm",
            "ProgressValue.Text",
            "Filter =",
            ".DefaultExt =",
            "DateTime.Now.ToString",
            "inputExtension !=",
            "#region",
            "extension !=",
            "lblTotalStatus.Text",
            "propertyFinalName ==",
            "childFinalPropertyName ==",
            "pName ==",
            "valuePropertyName ==",
            "propName ==",
            "optionList.Add(",
            "outputFileExtension =",
            "replacedFilePattern",
            "lblStatus.Text =",
            ".Name = ",
            ".Font =",
            "ConvertOptionValueListToString",
            "new OptionValue<",
            "entryString =",
            ".Milliseconds.ToString(",
            ".Seconds.ToString(",
            ".Minutes.ToString(",
            ".Hours.ToString(",
            ".Duration =",
            "myProcessInfo.Arguments",

        };
        private static readonly string[] IgnoredPrefixes = { "C:\\", "\\\\", "http:", "https:", "{0:", ".csproj" };

        public static int Execute(ScanOptions opts)
        {
            System.Console.WriteLine($"Scanning {opts.SourceDirectory}...");
            var report = new List<ScanReportEntry>();

            try
            {
                long totalEntries = 0;
                foreach (string file in Directory.EnumerateFiles(opts.SourceDirectory, "*.cs", SearchOption.AllDirectories))
                {
                    if (file.Contains("\\obj\\")
                        || file.Contains("\\bin\\")
                        || file.Contains("\\gMKVToolNix.Translator.Console")
                        || file.Contains("\\gMKVToolnix.Unit.Tests")
                        || file.Contains("\\Program.cs")
                        || file.Contains("\\gSettings.cs")
                        || file.Contains("\\AssemblyInfo.cs")
                        || file.Contains("\\gMKVExtractFilenamePatterns.cs")
                        || file.Contains("\\JsonLocalizationService.Defaults.cs")
                        || file.Contains("\\JsonLocalizationService.cs")
                        || file.Contains("\\NativeMethods.cs")
                        || file.Contains("\\gMKVVersionParser.cs")
                        || file.Contains("\\Cue.cs")
                        || file.Contains("\\CueTrack.cs")
                        || file.Contains("\\ChapterExtensions.cs")
                        || file.Contains("\\CodecPrivateDataExtensions.cs")
                        || file.Contains("\\Resources.Designer.cs"))
                    {
                        continue;
                    }

                    string currentNamespace = "Unknown";
                    string currentClass = "Unknown";
                    string[] lines = File.ReadAllLines(file);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                        // Basic context finding
                        if (line.StartsWith("namespace ")) currentNamespace = line.Split(' ')[1].Trim();
                        if (line.Contains("class ")) currentClass = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(s => !s.StartsWith(":") && s != "class");

                        // Check for ignored patterns
                        bool skipLine = false;
                        foreach (var sub in IgnoredSubstrings)
                        {
                            if (line.Contains(sub)) { skipLine = true; break; }
                        }
                        if (skipLine) continue;

                        foreach (Match match in StringRegex.Matches(line))
                        {
                            string foundString = match.Groups[1].Value;

                            // Check for ignored prefixes and length
                            if (foundString.Length < 2) continue;
                            bool skipString = false;
                            foreach (var pre in IgnoredPrefixes)
                            {
                                if (foundString.StartsWith(pre, StringComparison.OrdinalIgnoreCase)) { skipString = true; break; }
                            }
                            if (skipString) continue;

                            // Passed filters! Add to report.
                            report.Add(new ScanReportEntry
                            {
                                FilePath = file.Replace(opts.SourceDirectory, ""),
                                LineNumber = i + 1,
                                FoundString = foundString,
                                SuggestedKey = $"{currentNamespace}.{currentClass}.{SanitizeKey(foundString)}",
                                FullLine = line
                            });
                            totalEntries++;
                        }
                    }
                }

                File.WriteAllText(opts.OutputFile, JsonConvert.SerializeObject(report, Formatting.Indented));
                System.Console.WriteLine($"Scan complete. Total entries found: {totalEntries}. Report generated: {opts.OutputFile}");
                return 0; // Success
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR: {ex.Message}");
                return 1; // Failure
            }
        }

        private static string SanitizeKey(string value)
        {
            // Truncate long strings and replace invalid chars
            string key = value.Length > 30 ? value.Substring(0, 30) : value;
            return Regex.Replace(key, @"[^a-zA-Z0-9_]", "_");
        }
    }
}
