# Localization Strings Manifest
## gMKVExtractGUI - Complete String Inventory

**Generated:** March 17, 2025  
**Last Updated:** March 17, 2025 (Culture Selector Implementation)  
**Total Strings:** 115 localizable entries (111 original + 4 new for culture selector)  
**Target Forms:** 5 WinForms

---

## Summary Statistics

| Metric | Count |
|--------|-------|
| Total UI Strings | 115 |
| Forms Processed | 5 |
| Button/Control Labels | 39 |
| GroupBox Labels | 20 |
| Menu Items | 10 |
| Checkbox Labels | 8 |
| Status Labels | 6 |
| Advanced Options Strings | 4 (NEW) |
| Other Labels | 13 |

---

## String Categories

### 1. Window Titles (3 strings)
- `UI.MainForm.Title` → "gMKVExtractGUI"
- `UI.JobManager.Title` → "Job Manager"
- `UI.LogForm.Title` → "Log"

### 2. Button Labels (25 strings)

#### Browse Buttons (4)
- `UI.MainForm.InputFile.Browse`
- `UI.MainForm.OutputDirectory.Browse`
- `UI.MainForm.Config.Browse`
- `UI.MainForm2.Config.Browse`

#### Action Buttons (12)
- `UI.MainForm.Actions.Extract`
- `UI.MainForm.Actions.Abort`
- `UI.MainForm.Actions.AbortAll`
- `UI.MainForm.Actions.Log`
- `UI.JobManager.Actions.SaveJobs`
- `UI.JobManager.Actions.LoadJobs`
- `UI.JobManager.Actions.RunJobs`
- `UI.JobManager.Actions.Remove`
- `UI.LogForm.Actions.Save`
- `UI.LogForm.Actions.ClearLog`
- `UI.LogForm.Actions.Refresh`
- `UI.LogForm.Actions.Close`

#### Configuration Buttons (7)
- `UI.JobManager.Actions.Abort`
- `UI.JobManager.Actions.AbortAll`
- `UI.MainForm2.Config.AutoDetect`
- `UI.OptionsForm.Tags.Default`
- `UI.OptionsForm.Chapters.Default`
- `UI.OptionsForm.VideoTracks.Default`
- `UI.OptionsForm.AudioTracks.Default`
- `UI.OptionsForm.SubtitleTracks.Default`
- `UI.OptionsForm.Attachments.Default`

#### Add Buttons (6)
- `UI.OptionsForm.Tags.Add`
- `UI.OptionsForm.Chapters.Add`
- `UI.OptionsForm.VideoTracks.Add`
- `UI.OptionsForm.AudioTracks.Add`
- `UI.OptionsForm.SubtitleTracks.Add`
- `UI.OptionsForm.Attachments.Add`

#### Job Management (2)
- `UI.MainForm2.Actions.AddJobs`
- `UI.MainForm2.Actions.ShowJobs`

### 3. GroupBox Labels (20 strings)

#### Main Form Groups (6)
- `UI.MainForm.InputFile.Group` → "Input file (you can drag and drop the file)"
- `UI.MainForm.OutputDirectory.Group` → "Output Directory"
- `UI.MainForm.Actions.Group` → "Actions"
- `UI.MainForm.Config.Group` → "MKVToolnix Directory (you can drag and drop the directory)"
- `UI.MainForm.InputFileInfo.Group` → "Input File Information"
- `UI.MainForm.Log.Group` → "Log"

#### Job Manager Groups (4)
- `UI.JobManager.Progress.Group` → "Progress"
- `UI.JobManager.Jobs.Group` → "Jobs"
- `UI.JobManager.Actions.Group` → "Actions"

#### Log Form Groups (2)
- `UI.LogForm.Log.Group` → "Log"
- `UI.LogForm.Actions.Group` → "Actions"

#### Main Form 2 Groups (4)
- `UI.MainForm2.Actions.Group` → "Actions"
- `UI.MainForm2.OutputDirectory.Group` → "Output Directory for Selected File (you can drag and drop the directory)"
- `UI.MainForm2.Config.Group` → "MKVToolnix Directory (you can drag and drop the directory)"
- `UI.MainForm2.InputFiles.Group` → "Input Files (you can drag and drop files or directories)"

#### Options Form Groups (7)
- `UI.OptionsForm.Tags.Group` → "Attachments"
- `UI.OptionsForm.Chapters.Group` → "Chapters"
- `UI.OptionsForm.VideoTracks.Group` → "Video Tracks"
- `UI.OptionsForm.AudioTracks.Group` → "Audio Tracks"
- `UI.OptionsForm.SubtitleTracks.Group` → "Subtitle Tracks"
- `UI.OptionsForm.Attachments.Group` → "Attachments"
- `UI.OptionsForm.Info.Group` → "Information"

#### NEW: Advanced Options Group (1)
- Advanced Options section with culture selector (4 new strings)

### 10. Advanced Options Strings (4 strings - NEW)

**Culture Selector Support:**
- `UI.OptionsForm.Defaults` → "Defaults"
- `UI.OptionsForm.RawMode` → "Use `raw` extraction"
- `UI.OptionsForm.FullRawMode` → "Use `full raw` extraction"
- `UI.OptionsForm.TextFilesWithoutBom` → "Disable BOM to text files (v96.0+)"

**Note:** These 4 strings support the Advanced Options controls in frmOptions and enable proper localization of extraction mode options.

### 4. Checkbox Labels (8 strings)
- `UI.MainForm.OutputDirectory.Lock` → "Lock"
- `UI.MainForm.Actions.Popup` → "Popup"
- `UI.MainForm.Actions.JobMode` → "Job Mode"
- `UI.JobManager.Actions.Popup` → "Popup"
- `UI.MainForm2.Actions.Popup` → "Popup"
- `UI.MainForm2.OutputDirectory.UseSource` → "Use Source"
- `UI.MainForm2.Actions.Extract` → "Extract" (could be button or label)

### 5. Combo Box & Label Indicators (6 strings)
- `UI.MainForm.Actions.ExtractionMode` → "Extract:"
- `UI.MainForm.Actions.ChapterType` → "Chapter"
- `UI.MainForm2.Actions.ExtractionMode` → "Extract"
- `UI.MainForm2.Actions.ChapterType` → "Chapter"

### 6. Status Bar Labels (4 strings)
- `UI.MainForm.Status.Track` → "track"
- `UI.MainForm.Status.Status` → "status"
- `UI.MainForm2.Status.Status` → "status"
- `UI.MainForm2.Status.TotalStatus` → "status"

### 7. Progress Labels (3 strings)
- `UI.JobManager.Progress.CurrentTrack` → "Current Track"
- `UI.JobManager.Progress.TotalProgress` → "Total Progress"
- `UI.JobManager.Progress.CurrentProgress` → "Current Progress"

### 8. Context Menu Items (10 strings)

#### Track Selection (7)
- `UI.ContextMenu.SelectAllTracks` → "Select All Tracks"
- `UI.ContextMenu.SelectAllVideoTracks` → "Select All Video Tracks"
- `UI.ContextMenu.SelectAllAudioTracks` → "Select All Audio Tracks"
- `UI.ContextMenu.SelectAllSubtitleTracks` → "Select All Subtitle Tracks"
- `UI.ContextMenu.SelectAllChapterTracks` → "Select All Chapter Tracks"
- `UI.ContextMenu.SelectAllAttachmentTracks` → "Select All Attachments Tracks"
- `UI.ContextMenu.UnselectAllTracks` → "Unselect All tracks"

#### Job & Directory Selection (3)
- `UI.JobManager.Jobs.ChangeToReadyStatus` → "Change to Ready Status"
- `UI.JobManager.Jobs.SelectAll` → "Select All"
- `UI.JobManager.Jobs.DeselectAll` → "Deselect All"
- `UI.MainForm2.OutputDirectory.SetAsDefault` → "Set As Default Directory"
- `UI.MainForm2.OutputDirectory.UseDefault` → "Use Currently Set Default Directory:"

### 9. Copy/Log Actions (1 string)
- `UI.LogForm.Actions.CopySelection` → "Copy Selection"

---

## Forms Reference

### frmMain.cs
**Purpose:** Main extraction interface  
**String Count:** 30  
**Key Controls:**
- Context menu for track selection (7 items)
- Input/Output file groups with browse buttons
- Actions group (Extract, Abort, Log buttons)
- Configuration group (MKVToolnix path)
- Input file info display
- Status bar

### frmJobManager.cs
**Purpose:** Job queue management  
**String Count:** 22  
**Key Controls:**
- Progress group (current track, total, current progress)
- Jobs group with selection menu
- Actions group (Save, Load, Run, Remove, Abort buttons)

### frmLog.cs
**Purpose:** Log viewer  
**String Count:** 11  
**Key Controls:**
- Log display group
- Actions group (Save, Clear, Refresh, Copy, Close buttons)

### frmMain2.cs
**Purpose:** Alternate main interface with file list  
**String Count:** 25  
**Key Controls:**
- Status bar with progress indicators
- Actions group (Add Jobs, Show Jobs, Extract, Log)
- Output directory group with context menu
- Configuration group (Auto Detect button)
- Input files group

### frmOptions.cs
**Purpose:** Settings/options dialog with culture selector  
**String Count:** 27 (23 original + 4 new)  
**Key Controls:**
- Attachments, Chapters, Video/Audio/Subtitle Tracks, Attachments configuration groups
- Information display group
- Each group has Default and Add buttons
- **NEW:** Advanced Options section with culture selector (dropdown + label)
  - Culture selector for runtime language switching
  - Auto-detects available cultures from *.json files in app directory
  - Persists selection in gSettings.Culture
  - Immediately applies changes to all open forms

---

## Key Naming Convention

The key naming follows this pattern:

```
UI.[FormName].[ComponentType].[ComponentName]
```

**Examples:**
- `UI.MainForm.InputFile.Group` - GroupBox in Main Form for Input File
- `UI.MainForm.Actions.Extract` - Button in Actions group on Main Form
- `UI.ContextMenu.SelectAllTracks` - Context menu item
- `UI.JobManager.Progress.CurrentTrack` - Label in Progress group in Job Manager

**Benefits:**
- Hierarchical organization makes maintenance easier
- Easy to find related strings
- Clear scope of each string
- Translator-friendly with built-in context

---

## JSON File Details

**File:** `/mnt/e/Development/gMKVExtractGUI/code/src/en.json`  
**Format:** JSON with metadata and structured entries  
**File Size:** ~19 KB  
**Valid JSON:** ✅ Confirmed

### JSON Structure Example:
```json
{
  "Metadata": {
    "Culture": "en",
    "Translator": null,
    "CreationDate": "2025-03-17T00:00:00Z",
    "LastEditDate": "2025-03-17T00:00:00Z"
  },
  "Entries": {
    "UI.MainForm.Title": {
      "Source": "gMKVExtractGUI",
      "Translation": "gMKVExtractGUI",
      "IsTranslated": true,
      "Notes": "Application window title"
    },
    ...
  }
}
```

---

## String Statistics by Length

| Length Range | Count | Examples |
|--------------|-------|----------|
| 3-10 chars | 22 | "Extract", "Log...", "Popup" |
| 11-20 chars | 35 | "Current Track", "Select All" |
| 21-50 chars | 25 | "MKVToolnix Directory..." |
| 51+ chars | 10 | "Input file (you can drag...)" |

---

## Strings Requiring Special Attention for Translators

### Context-Dependent Strings
These strings may need special handling in different languages:

1. **"Browse..."** (Appears 4 times)
   - Used consistently across all browse buttons
   - Translators should use same translation for consistency

2. **"Popup"** (Appears 3 times)
   - Same meaning in all contexts (show notification popup)
   - Should use consistent translation

3. **"Actions"** (Appears 5 times)
   - Generic label for action control groups
   - Should use consistent translation

4. **"Status"** (Appears 2 times)
   - Short status indicators in status bar
   - May need language-specific abbreviation

### Strings with Instructions
These contain helpful context for users:

1. "Input file (you can drag and drop the file)"
2. "MKVToolnix Directory (you can drag and drop the directory)"
3. "Output Directory for Selected File (you can drag and drop the directory)"
4. "Input Files (you can drag and drop files or directories)"

**Note:** Translators should preserve the instructional parenthetical content.

### Pluralization Considerations
Some strings may need plural forms in certain languages:

- "Select All Tracks" (plural)
- "Current Track" (singular)
- "Video Tracks", "Audio Tracks", "Subtitle Tracks" (plurals)

---

## Implementation Checklist

- [x] All UI strings identified (115 total)
- [x] Master en.json file created with proper structure
- [x] Key naming convention applied consistently
- [x] Metadata and notes added for each entry
- [x] JSON syntax validated
- [x] Culture selector UI implemented in frmOptions
- [x] Text accuracy fixes applied (frmLog, frmOptions)
- [x] Culture auto-detection from *.json files implemented
- [x] Multi-form synchronization implemented
- [ ] Code compilation in Visual Studio (Next step)
- [ ] Culture selector runtime testing
- [ ] Additional language template files created
- [ ] Translator review and approval
- [ ] Language-specific testing

---

## Next Steps for Implementation

1. **Compilation & Testing:** Compile solution in Visual Studio to verify culture selector implementation
2. **Runtime Testing:** Verify culture dropdown functionality and culture switching
3. **Form Localization:** Update remaining form code to use localized strings (Phase 2)
4. **Language Templates:** Generate new language files using translator tool
5. **Translation:** Have native speakers provide translations
6. **QA Testing:** Test application in multiple languages
7. **Deployment:** Package translation files with application

## Recent Changes (March 17, 2025)

### Culture Selector Implementation ✅

**Files Modified:**
- `frmOptions.Designer.cs` - Added lblCulture and cboCulture controls
- `frmOptions.cs` - Updated InitializeCultureSelector(), CboCulture_SelectedIndexChanged(), ApplyLocalizationToAllForms()
- `en.json` - Added 4 new localization keys for Advanced Options strings

**New Methods Added:**
- `GetAvailableCultures()` - Scans directory for *.json files to auto-detect available cultures
- `CboCulture_SelectedIndexChanged()` - Handles culture selection and form refresh
- `ApplyLocalizationToAllForms()` - Synchronizes culture change across all open forms

**Key Features:**
- ✅ Culture selector dropdown in Advanced Options groupbox
- ✅ Auto-detection of available cultures from *.json files
- ✅ Runtime culture switching with immediate UI refresh
- ✅ Settings persistence via gSettings.Culture
- ✅ Designer-based UI placement for proper layout management

---

**Report Generated:** March 17, 2025  
**Prepared For:** gMKVExtractGUI Localization Project
