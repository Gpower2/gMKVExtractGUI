namespace gMKVToolNix.MkvInfo
{
    public enum MkvInfoOptions
    {
        gui, // Start the GUI (and open inname if it was given)
        checksum, // Calculate and display checksums of frame contents
        check_mode, // Calculate and display checksums and use verbosity level 4.
        summary, // Only show summaries of the contents, not each element
        track_info, // Show statistics for each track in verbose mode
        hexdump, // Show the first 16 bytes of each frame as a hex dump
        full_hexdump, // Show all bytes of each frame as a hex dump
        size, // Show the size of each element including its header
        verbose, // Increase verbosity
        quiet, // Suppress status output
        ui_language, // Force the translations for 'code' to be used
        command_line_charset, //  Charset for strings on the command line
        output_charset, // Output messages in this charset
        redirect_output, // Redirects all messages into this file
        help, // Show this help
        version, // Show version information
        check_for_updates, // Check online for the latest release
        gui_mode, // In this mode specially-formatted lines may be output that can tell a controlling GUI what's happening
        no_gui, // It doesn't show the GUI but the CLI
    }
}