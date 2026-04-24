# gMKVExtractGUI

**A comprehensive C# .NET GUI for `mkvextract` and `mkvinfo` utilities.**

---

## Short Summary

`gMKVExtractGUI` is a powerful and intuitive **Graphical User Interface (GUI)** built in C# .NET 4.0 for the essential **`mkvextract`** utility, which is part of the MKVToolNix suite. It aims to provide a user-friendly wrapper that incorporates most (if not all) of the functionality of both `mkvextract` and `mkvinfo`.

---

## Full Description

Navigating the command-line interface of `mkvextract` can be daunting for many users. `gMKVExtractGUI` simplifies this process by offering a robust and responsive desktop application. Written in **C# .NET 4.0**, it ensures high compatibility across a range of Windows operating systems (from Windows XP onward) and is also designed to run smoothly on Linux through Mono (v1.6.4 and newer). While not extensively tested, it may even function on macOS.

This tool is perfect for anyone looking to easily extract tracks, timecodes, attachments, chapters, tags, or CUE sheets from Matroska (MKV) files without needing to remember complex command-line arguments. It also leverages `mkvinfo` and `mkvmerge` for rapid analysis of MKV elements.

---

## Features

`gMKVExtractGUI` is packed with features to streamline your MKV extraction workflow:

* **Complete `mkvextract` Functionality:** Access 100% of `mkvextract`'s capabilities, supporting extraction of tracks, timecodes, attachments, chapters (both XML and OGM), tags, and CUE sheets.
* **Batch Extraction:** Efficiently extract elements from multiple MKV files at once (available from v2.0.0 and above).
* **Custom Output Filename Patterns:** Define personalized naming conventions for your extracted files (available from v2.5.0 and above).
* **Fast MKV Analysis:** Utilizes `mkvinfo` and `mkvmerge` for incredibly quick analysis of MKV file elements.
* **Automatic Audio Delay Detection:** Automatically finds the audio track's delay relative to video and appends it to the extracted filename for easy synchronization.
* **Automatic MKVToolNix Detection:** Automatically detects the MKVToolNix installation directory from the Windows registry, no manual path configuration needed.
* **Standalone Executable:** The `gMKVExtractGUI` executable doesn't need to be placed inside the MKVToolNix directory.
* **Standard File Extensions:** Uses appropriate file extensions for extracted tracks based on `CODEC_ID` as defined in the official `mkvextract` documentation.
* **Responsive GUI:** Employs a separate thread for invoking `mkvextract` operations, ensuring a smooth and responsive user interface during long extractions.
* **Job Mode:** Incorporates a dedicated job mode for managing and executing batch extractions (new in v1.6).
* **Wide OS Compatibility:**
    * **Windows:** Supports all versions from Windows XP and above.
    * **Linux:** Compatible via Mono (from v1.6.4 and above).
* **High DPI Support:** Designed to work seamlessly in high-DPI environments (available from v2.2.0 and above).
* **Localization & Live Language Switching:** UI labels, tooltips, popups, and context menus are loaded from JSON translation files and can be switched at runtime from the Options dialog. The same dialog now exposes an in-app translation editor for maintaining locale files. If locale files are missing, the app falls back to its embedded English defaults instead of showing localization placeholders.
* **Dark Mode:** Includes an optional dark theme with context menus themed consistently across the main windows.

---

## Project Homepage

For more information, discussions, and support, please visit the project's original homepage:

[http://forum.doom9.org/showthread.php?t=170249](http://forum.doom9.org/showthread.php?t=170249)

---

## Getting Started

1.  **Download the Latest Release:** Head over to the [Releases](https://github.com/Gpower2/gMKVExtractGUI/releases) section and download the latest executable.
2.  **Prerequisites:** Ensure you have [MKVToolNix](https://mkvtoolnix.download/) installed on your system.
3.  **Documentation:** For detailed usage instructions, refer to the [User Manual](docs/README.md).
4.  **Optional Setup:** Open **Options** to choose the UI theme, switch the application language, or launch the in-app **Translations...** editor. The current build ships with `en`, `es`, `de`, `pt`, `pt-br`, `fr`, `el`, `zh-cn`, `zh-tw`, `ja`, `ru`, `it`, `nl`, `pl`, `tr`, `ro`, `hi`, and `ko` locale files.

---

## Contributing

We welcome contributions! If you have suggestions, bug reports, or want to contribute code, please feel free to open an issue or submit a pull request.

---

## License

This software is dedicated to the **Public Domain** under the terms of **The Unlicense**.
For more information, please see the [UNLICENSE](https://unlicense.org/) file.
