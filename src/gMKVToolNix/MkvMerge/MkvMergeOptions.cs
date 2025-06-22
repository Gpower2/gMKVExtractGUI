namespace gMKVToolNix.MkvMerge
{
    public enum MkvMergeOptions
    {
        identify, // Will let mkvmerge(1) probe the single file and report its type, the tracks contained in the file and their track IDs. If this option is used then the only other option allowed is the filename. 
        identify_verbose, // Will let mkvmerge(1) probe the single file and report its type, the tracks contained in the file and their track IDs. If this option is used then the only other option allowed is the filename. 
        ui_language, //Forces the translations for the language code to be used 
        command_line_charset,
        output_charset,
        identification_format, // Set the identification results format ('text', 'verbose-text', 'json')
        version,
    }
}
