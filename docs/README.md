# gMKVExtractGUI User Manual

This manual describes the user interface and functionality of gMKVExtractGUI, organized by each main window (form) in the application. It is intended for end-users.

---

## Main Window (gMKVExtractGUI)

### Configuration Section
- **MKVToolNix Path:**
  - Text box to specify the path to the MKVToolNix tools.
  - **Browse...** button: Opens a folder dialog to select the MKVToolNix directory.
  - **Auto Detect** button: Attempts to automatically find the MKVToolNix installation.

### Input Section
- **Input Files:**
  - **Track List:** Shows all tracks (video, audio, subtitles, chapters, attachments) in the file. 
    - You can drag and drop files into this area in order to add them to the list.
    - You can check/uncheck tracks to select which to extract.    
  - **Context Menu (right-click on track list):**
    - Select/unselect all tracks or by type (video, audio, subtitle, chapter, attachment).
    - Remove selected or all input files.
    - Expand/collapse all nodes.
    - Open selected file or its folder.
    - Add input file.
  - **Append on Drag and Drop:**
    - Checkbox to append files to the list when dragging and dropping.
  - **Overwrite Existing Files:**
    - Checkbox to allow overwriting files in the output directory.
  - **Select...:**
    - Button to display the Context Menu.
- **Selected File Information:**
  - Displays information about the selected MKV file.

### Output Section
- **Output Directory:**
  - Text box to specify where extracted files will be saved.
- **Browse...**
  - Button to open a folder dialog to select the output directory.
- **Use Source:**
  - Checkbox to select the source directory of the selected file as the output directory.

### Actions Section
- **Logs:**
  - Button to open the log window.
- **Jobs:**
  - Button to open the job manager window.
- **Popup:**
  - Checkbox to enable/disable popup notifications.
- **Chapter Type:**
  - Dropdown to select the format for chapter extraction (e.g., XML, OGM, CUE, PBF).
- **Extraction Mode:**
  - Dropdown to select the extraction mode (e.g., Tracks, Cue_Sheet, Tags, Timecodes, etc.).
- **Add Jobs:**
  - Button to add the current selected tracks as jobs.
- **Extract:**
  - Button to start extracting the selected tracks.

### Status Bar
- **Progress Bar:**
  - Shows extraction progress.
- **Total Progress Bar:**
  - Shows overall progress for batch jobs.
- **Dark Mode:**
  - Checkbox to toggle dark mode for the UI.
- **Options:**
  - Button to open the options window.
- **Abort All:**
  - Button to abort all running extractions.
- **Abort:**
  - Button to abort the current extraction.

---

## Options Window

- **Information:**
  - Read-only text area with instructions about filename patterns and placeholders.

- **Filename Pattern Sections:**
  - For each type (Video Tracks, Audio Tracks, Subtitle Tracks, Chapters, Attachments, Tags):
    - Text box to edit the output filename pattern.
    - **Add...** button: Opens a menu to insert placeholders (e.g., {FilenameNoExt}, {TrackNumber}, etc.).
    - **Default** button: Resets the pattern to its default value.

- **Actions:**
  - **Defaults:** Button to reset all patterns to their defaults.
  - **OK:** Save changes and close the window.
  - **Cancel:** Discard changes and close the window.

- **Status Bar:**
  - Displays status messages.

---

## Log Window

- **Log:**
  - Read-only text area displaying the application log.

- **Actions:**
  - **Copy Selection:** Copies selected log text to the clipboard.
  - **Refresh:** Reloads the log content.
  - **Clear Log:** Clears the log.
  - **Save...:** Saves the log to a file.
  - **Close:** Closes the log window.

---

## Job Manager Window

- **Jobs List:**
  - Table showing all queued jobs with their details.
  - **Context Menu:**
    - Select All, Deselect All, Change to Ready Status for jobs.

- **Progress Section:**
  - **Current Track:** Shows the name of the track currently being processed.
  - **Current Progress:** Progress bar and label for the current job.
  - **Total Progress:** Progress bar and label for all jobs.

- **Actions:**
  - **Remove:** Removes selected jobs from the list.
  - **Run Jobs:** Starts processing all jobs in the queue.
  - **Abort:** Aborts the current job.
  - **Abort All:** Aborts all jobs.
  - **Save Jobs...:** Saves the job list to a file.
  - **Load Jobs...:** Loads jobs from a file.
  - **Popup:** Checkbox to enable/disable popup notifications for jobs.

---


This manual covers all main UI elements and their functions for end-users.
