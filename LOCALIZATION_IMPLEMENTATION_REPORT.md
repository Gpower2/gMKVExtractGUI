# gMKVExtractGUI - Localization Implementation Report

**Last Updated:** April 24, 2026
**Status:** Complete and validated
**Locale Files in Tree:** 18 (`en`, `es`, `de`, `pt`, `pt-br`, `fr`, `el`, `zh-cn`, `zh-tw`, `ja`, `ru`, `it`, `nl`, `pl`, `tr`, `ro`, `hi`, `ko`)
**Current Key Count:** 289 keys in every locale file
**Validation Snapshot:** Solution builds successfully and the current unit suite is at 32 passing tests

---

## Executive Summary

The localization work on this branch is no longer in a "partial rollout" phase. The application now uses a cached JSON-based localization runtime, starts with the saved culture, reloads translations when the culture changes in `frmOptions`, and ships aligned locale files for the requested languages.

This branch also now includes an in-app translation editor launched from `frmOptions`, shared translation-maintenance services used by both the GUI and the CLI, responsive layout fixes on the localized forms that previously had fixed-size overflow issues, and script-aware font fallback for localized rich text in `frmOptions`.

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

- source files: primarily `gmkvextract-*.json`, with temporary fallback to legacy bare `<culture>.json` names only when no prefixed files exist
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
| `src\gMKVExtractGUI\gmkvextract-en.json` | `en` | 289 |
| `src\gMKVExtractGUI\gmkvextract-es.json` | `es` | 289 |
| `src\gMKVExtractGUI\gmkvextract-de.json` | `de` | 289 |
| `src\gMKVExtractGUI\gmkvextract-pt.json` | `pt` | 289 |
| `src\gMKVExtractGUI\gmkvextract-pt-br.json` | `pt-br` | 289 |
| `src\gMKVExtractGUI\gmkvextract-fr.json` | `fr` | 289 |
| `src\gMKVExtractGUI\gmkvextract-el.json` | `el` | 289 |
| `src\gMKVExtractGUI\gmkvextract-zh-cn.json` | `zh-cn` | 289 |
| `src\gMKVExtractGUI\gmkvextract-zh-tw.json` | `zh-tw` | 289 |
| `src\gMKVExtractGUI\gmkvextract-ja.json` | `ja` | 289 |
| `src\gMKVExtractGUI\gmkvextract-ru.json` | `ru` | 289 |
| `src\gMKVExtractGUI\gmkvextract-it.json` | `it` | 289 |
| `src\gMKVExtractGUI\gmkvextract-nl.json` | `nl` | 289 |
| `src\gMKVExtractGUI\gmkvextract-pl.json` | `pl` | 289 |
| `src\gMKVExtractGUI\gmkvextract-tr.json` | `tr` | 289 |
| `src\gMKVExtractGUI\gmkvextract-ro.json` | `ro` | 289 |
| `src\gMKVExtractGUI\gmkvextract-hi.json` | `hi` | 289 |
| `src\gMKVExtractGUI\gmkvextract-ko.json` | `ko` | 289 |

All locale files are included in `src\gMKVExtractGUI\gMKVExtractGUI.csproj` so they are copied to the output directory and are visible both to the runtime loader and to the culture picker in `frmOptions`.
Those files now sit on top of the embedded English defaults rather than being the only source of fallback text.

---

## Branch Changes Captured in the Current Implementation

### 1. Startup and Reload Behavior

- the application no longer forces English at startup
- the saved culture now drives the initial localization state
- runtime language changes rebuild the localization service correctly
- locale discovery works with non-two-letter file names such as `pt-br`, `zh-cn`, and `zh-tw`

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

### 7. Script-Aware Font Fallback

`frmOptions` now routes its informational `RichTextBox` text through `LocalizedFontResolver` so script-heavy locales such as Hindi and Chinese can prefer an installed script-capable UI font instead of relying on inconsistent control-level glyph fallback.

The resolver probes candidate font families directly and includes common Windows, Linux, and macOS font families for `hi`, `ja`, `zh-cn`, `zh-tw`, and `ko`. This keeps the current Windows fix in place while making the same path materially safer for Mono on Linux and macOS, subject to those fonts actually being installed on the host system.

---

## Validation Notes

The current state has been validated with:

- locale-file parity checks (`289` entries in every shipped locale file)
- solution build success
- the repository MSTest suite
- regression tests for localization formatting behavior
- regression tests for nested context-menu theming behavior
- regression tests for the shared translation-maintenance services

At the time of this update, the suite total is **32 passing tests**.

---

## Maintenance Guidance

### When Adding New Strings

1. add the runtime call site with `LocalizationManager.GetString(...)`
2. if you truly need an explicit culture, use `LocalizationManager.GetStringForCulture(...)`
3. update the master file with `gMKVToolNix.Translator.Console master`
4. sync the non-English locale files with the in-app **Translations...** editor or the console `template` / `sync` workflow
5. keep `JsonLocalizationService.Defaults.cs` aligned with `gmkvextract-en.json`
6. keep the locale files aligned and copied in the GUI project file

### When Adding New Culture Files

- keep the JSON schema consistent with `gmkvextract-en.json`
- set the correct `Metadata.Culture`
- include the new file in `gMKVExtractGUI.csproj`
- verify the file appears in `frmOptions` culture selection
- use `zh-cn` and `zh-tw` as the canonical Simplified/Traditional Chinese locale codes; legacy `cn` remains a runtime compatibility alias for `zh-tw`

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
- `src\gMKVExtractGUI\Localization\LocalizedFontResolver.cs`
- `src\gMKVExtractGUI\Forms\frmOptions.cs`
- `src\gMKVExtractGUI\Forms\frmTranslationEditor.cs`
- `src\gMKVExtractGUI\Theming\ThemeManager.cs`
- `src\gMKVExtractGUI\gmkvextract-en.json`
- `src\gMKVToolNix\Localization\TranslationFileService.cs`
- `src\gMKVToolNix\Localization\TranslationMaintenanceService.cs`

The string inventory itself is maintained in `LOCALIZATION_STRINGS_MANIFEST.md`, while `gmkvextract-en.json` remains the authoritative per-key source of truth.
