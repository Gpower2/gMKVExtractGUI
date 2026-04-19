# Copilot instructions — gMKVExtractGUI

## Build, test, and lint commands

- This is an older .NET Framework 4.0 solution with non-SDK-style projects. Prefer Visual Studio or `msbuild`; `dotnet build`/`dotnet test` is not the primary workflow here.
- NuGet packages are checked into `packages\`, so there is usually no separate restore step.
- The solution defines both `Any CPU` and `x86` configurations.

```powershell
msbuild gMKVExtractGUI.sln /p:Configuration=Debug /p:Platform="Any CPU"
msbuild src\gMKVExtractGUI\gMKVExtractGUI.csproj /p:Configuration=Debug /p:Platform="Any CPU"
msbuild src\gMKVToolNix.Translator.Console\gMKVToolNix.Translator.Console.csproj /p:Configuration=Debug /p:Platform="Any CPU"
msbuild tests\gMKVToolnix.Unit.Tests\gMKVToolnix.Unit.Tests.csproj /p:Configuration=Debug /p:Platform="Any CPU"

vstest.console.exe tests\gMKVToolnix.Unit.Tests\bin\Debug\gMKVToolNix.Unit.Tests.dll
vstest.console.exe tests\gMKVToolnix.Unit.Tests\bin\Debug\gMKVToolNix.Unit.Tests.dll /Tests:gMKVToolnix.Unit.Tests.gMkvParser_Tests.VersionOutput_ShouldBe_Parsed_Successfully
```

- Lint: no repository lint command is configured.

## High-level architecture

- The solution has four projects:
  - `src\gMKVExtractGUI` - WinForms front end. `Program.Main` initializes localization, then launches `Forms\frmMain2` (not `frmMain`).
  - `src\gMKVToolNix` - core library around `mkvmerge`, `mkvinfo`, and `mkvextract`; owns segment models, parsing, filename pattern logic, process helpers, and logging.
  - `src\gMKVToolNix.Translator.Console` - localization maintenance CLI with `scan`, `master`, `template`, and `sync` verbs.
  - `tests\gMKVToolnix.Unit.Tests` - MSTest coverage for version parsing and file naming helpers in the core library.

- File analysis in the GUI is a merged-tool pipeline. `frmMain2` calls `gMKVHelper.GetMergedMkvSegmentList`, which uses `gMKVMerge` as the primary source, fills in missing segment info / codec private data / delays with `gMKVInfo`, then turns the merged `gMKVSegment` list into the checked tree shown in the main form.

- Extraction uses the same job model for immediate runs and queued runs. `frmMain2` builds `gMKVExtractSegmentsParameters` and wraps them in `gMKVJob`; clicking **Extract** runs those jobs immediately, while **Add Jobs** sends the same objects to `frmJobManager`. `gMKVExtract` groups track parameters by extraction mode and shells out to `mkvextract`, including version-specific handling for chapters, cue sheets, tags, and raw/fullraw modes.

- Three services cut across most UI work:
  - `gSettings` persists `gMKVExtractGUI.ini` in the application directory when writable, otherwise under `Application.UserAppDataPath`.
  - `LocalizationManager` loads and caches `gmkvextract-*.json` translations from the executable directory, serves `LocalizationManager.GetString(...)` lookups for the active culture, and rebuilds the cache on `LocalizationManager.Reload(...)`. If no prefixed files exist yet, the shared path helper temporarily falls back to legacy bare `<culture>.json` names.
  - `ThemeManager` plus `WinAPI\NativeMethods` apply light/dark styling to stock controls and the custom `g*` controls. Context menus should go through `ThemeManager.ApplyContextMenuTheme(...)` rather than direct popup-window retheming.

- MKVToolNix path discovery is part of startup behavior. `frmMain2` accepts `--mkvtoolnix=...`, otherwise falls back to the saved setting, the current directory, the Windows registry, or `/usr/bin` on Linux/Mono.

## Key conventions

- The GUI project lives in `src\gMKVExtractGUI`, but most namespaces are still `gMKVToolNix`. Search by namespace as well as by folder name when tracing UI code.

- Prefer the custom controls in `src\gMKVExtractGUI\Controls` (`gForm`, `gTreeView`, `gTextBox`, `gDataGridView`, etc.) instead of introducing new one-off wrappers. Theme and dark-mode behavior is centralized in `ThemeManager`, so new forms and context menus should follow that pattern.

- Filename pattern work spans three places: placeholder constants in `gMKVExtractFilenamePatterns`, persisted defaults in `gSettings`, and the add-placeholder/default UI in `frmOptions`. Changing only one of those will leave the app inconsistent.

- Localization work is JSON-first. Runtime strings come from `src\gMKVExtractGUI\gmkvextract-en.json` and `LocalizationManager.GetString(...)`, but `src\gMKVExtractGUI\Localization\JsonLocalizationService.Defaults.cs` also carries the embedded English fallback set. Translation-file maintenance now flows through the shared helpers in `src\gMKVToolNix\Localization`, which are used by both the in-app `frmTranslationEditor` UI and `src\gMKVToolNix.Translator.Console`. If you add a new culture file, include it in `src\gMKVExtractGUI\gMKVExtractGUI.csproj` with `CopyToOutputDirectory` so the runtime loader and culture picker can see it.

- Use `LocalizationManager.GetString(...)` for current-culture lookups and `LocalizationManager.GetStringForCulture(...)` only when a caller truly needs an explicit culture. The old public `GetString(key, culture)` shape was removed because it conflicted with formatted `GetString(key, params object[])` calls.

- For popup menus, keep the built-in WinForms menu styling path and avoid calling `NativeMethods.SetWindowThemeManaged(...)` on `ToolStripDropDown` popup handles during `Opening`; that retheming path was the source of the earlier menu race/heap-corruption issue.

- Job definitions are serialized to XML. If you add job fields or new segment subtypes, keep `gMKVJob` serialization attributes (`[Serializable]`, `XmlInclude`) aligned with `frmJobManager` save/load behavior for `List<gMKVJobInfo>`.

- Segment metadata changes usually span both parsers. The UI relies on the merged result from `gMKVMerge`, `gMKVInfo`, and `gMKVHelper.GetMergedMkvSegmentList`, so parser changes often need coordinated updates in more than one class.
