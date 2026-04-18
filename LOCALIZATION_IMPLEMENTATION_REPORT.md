# gMKVExtractGUI - Localization Implementation Report

**Last Updated:** April 18, 2026
**Status:** Complete and validated
**Locale Files in Tree:** 9 (`en`, `es`, `de`, `pt`, `pt-br`, `fr`, `el`, `cn`, `ja`)
**Current Key Count:** 268 keys in every locale file
**Validation Snapshot:** Solution builds successfully and the current unit suite is at 19 passing tests

---

## Executive Summary

The localization work on this branch is no longer in a "partial rollout" phase. The application now uses a cached JSON-based localization runtime, starts with the saved culture, reloads translations when the culture changes in `frmOptions`, and ships aligned locale files for the requested languages.

This branch also now includes an in-app translation editor launched from `frmOptions`, shared translation-maintenance services used by both the GUI and the CLI, and responsive layout fixes on the localized forms that previously had fixed-size overflow issues.

In addition to the original UI labels, this branch also localized the remaining runtime surfaces that mattered in practice:

- shared popup titles and bodies
- exception dialogs
- file dialog titles and filters
- tooltips
- main-form and Job Manager context menus
- `frmOptions` placeholder insertion menus

The dark-mode context-menu work also settled on the previous built-in WinForms styling path rather than a custom renderer. The final implementation keeps the older `ManagerRenderMode` / `Professional` look while avoiding popup-window retheming on `ToolStripDropDown` handles during menu opening.

---

## Current Runtime Architecture

### LocalizationManager

`LocalizationManager` is the entry point used by the WinForms UI:

- `Initialize(culture)` creates the runtime service the first time the app starts.
- `Reload(culture)` rebuilds the runtime localization service from the translation directory and switches the active culture.
- `GetString(key)` returns the current-culture value.
- `GetString(key, params object[] args)` formats the current-culture value.
- `GetStringForCulture(key, culture)` and `GetStringForCulture(key, culture, args)` are the explicit culture-specific helpers.

The public culture-specific helper was renamed to `GetStringForCulture(...)` on this branch to avoid the overload ambiguity that previously caused `!BadFormat:...!` failures when the first format argument was a string.

### JsonLocalizationService

`JsonLocalizationService` loads translation files from the executable directory and flattens them into an in-memory runtime cache:

- source files: `*.json`
- runtime structure: `Dictionary<string, Dictionary<string, string>>`
- load timing: once during initialization, and again only when `LocalizationManager.Reload(...)` is called
- per-lookup file I/O: none
- built-in fallback: `JsonLocalizationService.Defaults.cs` embeds the full English key set so `en` is always available, even if locale files are deleted from disk

Lookup fallback order:

1. requested culture (for example `pt-br`)
2. neutral culture when applicable (for example `pt`)
3. English fallback (`en`)
4. placeholder output (`!Key!`) if all lookups fail

Formatted lookup failures are logged and returned as `!BadFormat:Key!` so broken format strings are visible rather than silently swallowed.

### Culture Selection Flow

`frmOptions` owns runtime culture switching:

1. it queries `LocalizationManager.GetAvailableCultures()`
2. it populates the culture dropdown from the discovered JSON files
3. it saves the selected culture to `gSettings.Culture`
4. it calls `LocalizationManager.Reload(selectedCulture)`
5. it reapplies localization across the open forms
6. it can launch the in-app translation editor for locale maintenance

This means the active culture is loaded from settings at startup and can be changed without restarting the application.
If the saved culture is blank or points to a locale file that is no longer available, the runtime now normalizes back to English instead of keeping an invalid culture name and surfacing `!Key!` placeholders.

---

## Locale Files Currently Shipped

| File | Culture | Entries |
|---|---|---:|
| `src\gMKVExtractGUI\en.json` | `en` | 268 |
| `src\gMKVExtractGUI\es.json` | `es` | 268 |
| `src\gMKVExtractGUI\de.json` | `de` | 268 |
| `src\gMKVExtractGUI\pt.json` | `pt` | 268 |
| `src\gMKVExtractGUI\pt-br.json` | `pt-br` | 268 |
| `src\gMKVExtractGUI\fr.json` | `fr` | 268 |
| `src\gMKVExtractGUI\el.json` | `el` | 268 |
| `src\gMKVExtractGUI\cn.json` | `cn` | 268 |
| `src\gMKVExtractGUI\ja.json` | `ja` | 268 |

All locale files are included in `src\gMKVExtractGUI\gMKVExtractGUI.csproj` so they are copied to the output directory and are visible both to the runtime loader and to the culture picker in `frmOptions`.
Those files now sit on top of the embedded English defaults rather than being the only source of fallback text.

---

## Branch Changes Captured in the Current Implementation

### 1. Startup and Reload Behavior

- the application no longer forces English at startup
- the saved culture now drives the initial localization state
- runtime language changes rebuild the localization service correctly
- locale discovery works with non-two-letter file names such as `pt-br`

### 2. Runtime Localization Coverage

This branch extended localization beyond static labels:

- popup helpers in `gForm`
- exception dialogs in `ExceptionExtensions`
- tree-view-specific error text in `gTreeView`
- file dialog titles and filters
- main-form tooltips
- main-form and Job Manager context menus
- `frmOptions` placeholder menus

### 3. Formatting Regression Fix

The previous public overload pair:

- `GetString(string key, string culture)`
- `GetString(string key, params object[] args)`

allowed some formatted string calls to be bound as culture lookups instead of format calls. The fix was to rename the explicit culture overloads to `GetStringForCulture(...)` while keeping the current-culture `GetString(...)` API intact.

### 4. Context Menu Stability and Theming

The final context-menu solution on this branch is:

- keep the previous built-in WinForms context-menu styling path
- apply menu colors and render mode through `ThemeManager.ApplyContextMenuTheme(...)`
- skip native `SetWindowThemeManaged(...)` / `TrySetImmersiveDarkMode(...)` calls for `ToolStripDropDown` popup windows

That last guard matters because retheming popup menu HWNDs during `Opening` was the risky path that led to the earlier heap-corruption crash reports. The app now preserves the original dark-mode look without using a custom context-menu renderer.

### 5. Translator Workflow

The branch now exposes translation maintenance in two aligned ways:

- `frmOptions` launches `frmTranslationEditor` for translator-friendly editing, create, sync, filter, and save operations
- `src\gMKVToolNix.Translator.Console` uses the same shared translation services for `template` and `sync`

This keeps the JSON schema and maintenance behavior consistent across manual GUI work and scripted CLI workflows.

### 6. Localized Layout Hardening

The main localized forms (`frmMain2`, `frmOptions`, `frmJobManager`, `frmLog`) now resize key buttons, labels, and rows at runtime after localization is applied. This avoids language-specific designer forks while reducing clipped text in wider locales.

---

## Validation Notes

The current state has been validated with:

- locale-file parity checks (`268` entries in every shipped locale file)
- solution build success
- the repository MSTest suite
- regression tests for localization formatting behavior
- regression tests for nested context-menu theming behavior
- regression tests for the shared translation-maintenance services

At the time of this update, the suite total is **19 passing tests**.

---

## Maintenance Guidance

### When Adding New Strings

1. add the runtime call site with `LocalizationManager.GetString(...)`
2. if you truly need an explicit culture, use `LocalizationManager.GetStringForCulture(...)`
3. update the master file with `gMKVToolNix.Translator.Console master`
4. sync the non-English locale files with the in-app **Translations...** editor or the console `template` / `sync` workflow
5. keep `JsonLocalizationService.Defaults.cs` aligned with `en.json`
6. keep the locale files aligned and copied in the GUI project file

### When Adding New Culture Files

- keep the JSON schema consistent with `en.json`
- set the correct `Metadata.Culture`
- include the new file in `gMKVExtractGUI.csproj`
- verify the file appears in `frmOptions` culture selection

### When Touching Context Menus

- theme popup menus through `ThemeManager.ApplyContextMenuTheme(...)`
- do not call `NativeMethods.SetWindowThemeManaged(...)` on `ToolStripDropDown` popup handles
- keep main-form and Job Manager context-menu behavior aligned

---

## Authoritative Sources

For the current implementation, these files are the most important references:

- `src\gMKVExtractGUI\Localization\LocalizationManager.cs`
- `src\gMKVExtractGUI\Localization\JsonLocalizationService.cs`
- `src\gMKVExtractGUI\Localization\JsonLocalizationService.Defaults.cs`
- `src\gMKVExtractGUI\Forms\frmOptions.cs`
- `src\gMKVExtractGUI\Forms\frmTranslationEditor.cs`
- `src\gMKVExtractGUI\Theming\ThemeManager.cs`
- `src\gMKVExtractGUI\en.json`
- `src\gMKVToolNix\Localization\TranslationFileService.cs`
- `src\gMKVToolNix\Localization\TranslationMaintenanceService.cs`

The string inventory itself is maintained in `LOCALIZATION_STRINGS_MANIFEST.md`, while `en.json` remains the authoritative per-key source of truth.
