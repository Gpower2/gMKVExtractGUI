# Localization Strings Manifest
## Current Inventory Summary

**Last Updated:** April 19, 2026
**Authoritative Source:** `src\gMKVExtractGUI\en.json`
**Total Keys:** 289
**Locale Files Shipped:** 17
**Locale Parity:** all shipped locale files currently contain 289 entries

---

## Locale Files

| File | Culture | Entries |
|---|---|---:|
| `en.json` | `en` | 289 |
| `es.json` | `es` | 289 |
| `de.json` | `de` | 289 |
| `pt.json` | `pt` | 289 |
| `pt-br.json` | `pt-br` | 289 |
| `fr.json` | `fr` | 289 |
| `el.json` | `el` | 289 |
| `cn.json` | `cn` | 289 |
| `ja.json` | `ja` | 289 |
| `ru.json` | `ru` | 289 |
| `it.json` | `it` | 289 |
| `nl.json` | `nl` | 289 |
| `pl.json` | `pl` | 289 |
| `tr.json` | `tr` | 289 |
| `ro.json` | `ro` | 289 |
| `hi.json` | `hi` | 289 |
| `ko.json` | `ko` | 289 |

---

## Top-Level Key Families

The table below summarizes the current key distribution by the second path segment in `en.json`.

| Key Family | Count | Notes |
|---|---:|---|
| `UI.MainForm2` | 100 | Active main window, tooltips, status text, output directory menu, and large context-menu surface |
| `UI.OptionsForm` | 64 | Filename patterns, placeholder menus, advanced options, culture selector, and translation-editor launcher |
| `UI.TranslationEditor` | 34 | In-app translation editor labels, create/sync workflow, filters, tooltips, and status text |
| `UI.JobManager` | 31 | Job queue actions, grid headers, progress labels, dialogs, and context menu |
| `UI.MainForm` | 22 | Legacy main form strings kept in sync |
| `UI.Common` | 16 | Shared popup titles, generic dialog text, and common status strings |
| `UI.LogForm` | 9 | Log viewer window strings |
| `UI.ContextMenu` | 7 | Shared legacy track-selection menu items |
| `UI.Log` | 3 | Shared log-related strings |
| `UI.Controls` | 3 | Control-specific shared strings |

**Total:** 289

---

## Coverage Notes

The manifest is not limited to static form labels anymore. The current inventory also covers:

- runtime popup and confirmation text
- exception dialog titles and bodies
- file dialog titles and filters
- localized tooltips
- main-form and Job Manager context menus
- `frmOptions` placeholder insertion menus
- the in-app translation editor workflow

The main form's progress-value labels are intentionally blank at startup and are updated at runtime, so the live percentage text is not represented as a fixed UI label in the JSON files.

---

## Naming Convention

Localization keys follow hierarchical dot notation:

```text
UI.[Scope].[Section].[Name]
```

Examples:

- `UI.MainForm2.Actions.Extract`
- `UI.MainForm2.OutputDirectory.UseDefaultWithValue`
- `UI.JobManager.Jobs.ChangeToReadyStatus`
- `UI.OptionsForm.Advanced.Culture`
- `UI.Common.Dialog.AreYouSureTitle`

This structure keeps related strings grouped and makes translator context easier to preserve.

---

## Runtime Notes

- `LocalizationManager.GetString(...)` is the current-culture lookup API.
- `LocalizationManager.GetStringForCulture(...)` is the explicit culture-specific API.
- `JsonLocalizationService` loads all translation files into an in-memory runtime cache and does not reread JSON files on every lookup.
- `JsonLocalizationService.Defaults.cs` embeds the full English key set, so `en` remains available even when locale files are missing.
- `LocalizedFontResolver` lets `frmOptions` prefer script-capable fonts for locales such as `hi`, `ja`, `cn`, and `ko` without relying on Windows-only font-enumeration behavior.
- Lookup fallback order is: requested culture -> neutral culture -> English (`en`) -> `!Key!`.
- Formatting failures surface as `!BadFormat:Key!` and are logged.

---

## Maintenance Notes

1. `en.json` is the authoritative per-key inventory.
2. Use `gMKVToolNix.Translator.Console master` to refresh the master file from code.
3. Use the in-app **Translations...** editor or the console `template` and `sync` commands to create or align non-English locale files.
4. Regenerate or update `JsonLocalizationService.Defaults.cs` whenever `en.json` gains or changes keys.
5. If you add a new culture file, also add it to `src\gMKVExtractGUI\gMKVExtractGUI.csproj` so it is copied to the output directory.
6. Keep new popup, tooltip, and context-menu strings aligned with the existing key hierarchy rather than introducing ad-hoc schemas.

---

## Practical Scope Reference

For day-to-day work, use these files together:

- `src\gMKVExtractGUI\en.json` - authoritative key list
- `src\gMKVExtractGUI\Localization\JsonLocalizationService.cs` - runtime loading and fallback behavior
- `src\gMKVExtractGUI\Localization\JsonLocalizationService.Defaults.cs` - embedded English fallback map
- `src\gMKVExtractGUI\Localization\LocalizedFontResolver.cs` - script-aware UI font fallback for localized rich text
- `src\gMKVExtractGUI\Forms\frmOptions.cs` - culture discovery and reload flow
- `src\gMKVExtractGUI\Forms\frmTranslationEditor.cs` - in-app translation workflow
- `src\gMKVToolNix.Translator.Console\README.md` - maintenance workflow
