# gMKVExtractGUI Translator Guide

This guide is for translators and localization maintainers who need to edit an existing locale or create a new one for gMKVExtractGUI.

## Choose the Right Workflow

- **Most translators:** use the in-app editor from **Options -> Translations...**
- **Developers or release maintainers:** use `gMKVToolNix.Translator.Console` for `scan`, `master`, `template`, and `sync`

The GUI editor and the console now use the same shared translation services, so creating or syncing a locale in either place follows the same rules.

## Before You Start

1. Keep `en.json` in the same translation directory as the locale files you want to edit.
2. Start the application from a folder that already contains the translation JSON files.
3. Use short, UI-friendly translations when possible; some controls can grow at runtime now, but concise strings still produce the best fit.
4. Do not rename localization keys or change the English source text in non-English files.
5. On Windows, Linux, or macOS, make sure the host system has script-capable fonts installed for languages such as Hindi, Japanese, Traditional Chinese, and Korean. The GUI now tries to pick common fonts automatically for those locales, but glyph coverage still depends on the local font set.

## Edit an Existing Locale

1. Start **gMKVExtractGUI**.
2. Open **Options**.
3. Click **Translations...** in the advanced options area.
4. Choose the locale you want to edit from the culture list.
5. Use the search box to find a key, or enable the untranslated filter to focus on incomplete entries.
6. Edit the **Translation** value for each row.
7. Use the read-only **Notes** column as translator context from `en.json`; it is not edited in the grid.
8. The **Translated** checkbox is set automatically when the **Translation** cell becomes non-empty, but you can still toggle it manually if needed.
9. Watch the save-state label and the `*` in the window title to know when there are pending changes.
10. Click **Save**.
11. Close the editor, return to **Options**, switch the app to that culture, and verify the UI in the running application.

## Create a New Locale

1. Open **Options -> Translations...**.
2. Click **New Locale...**.
3. In the dialog, enter the new culture code and optionally fill in the translator name.
4. Click **Create** to generate a new locale from `en.json`.
5. The editor will load the new file, copy the translator metadata into the top row, and start every entry as untranslated.
6. Translate the entries you are ready to complete.
7. Leave unfinished rows marked as not translated so they are easy to find later.
8. Click **Save** to write the new `<culture>.json` file.
9. Close the editor, return to **Options**, and confirm that the new culture now appears in the language dropdown.

## Sync an Existing Locale After English Changes

1. Open **Options -> Translations...**.
2. Select the locale you want to refresh.
3. Click **Sync**. This action is available only for non-English locales.
4. The editor saves any current changes first, then synchronizes the selected locale with `en.json`.
5. Review the summary so you know how many keys were added, removed, or reset.
6. Translate any rows that were added or marked as untranslated because the English source changed; those reset rows will have their translation set back to the English source.
7. Click **Save** if you make additional edits after the sync completes.
8. Reopen the locale in the app and spot-check the affected windows.

## What the Editor Fields Mean

- **Source:** the authoritative English text from `en.json`
- **Translation:** the current text for the selected locale
- **Translated:** whether the row is considered complete; the editor auto-checks it when the translation cell becomes non-empty
- **Notes:** read-only translator guidance or context copied from `en.json`

If a locale is missing a file or a specific entry, the application falls back to embedded English defaults instead of showing localization placeholders. That fallback is a safety net, not a substitute for completing the translation.

## Save-State and Busy-State Behavior

- The window title shows `*` when there are unsaved changes.
- The save-state label in the **Actions** area shows whether the editor is clean, dirty, or currently loading.
- While a locale is loading, controls are disabled and the editor uses a wait cursor to keep the UI responsive.

## Validation Checklist

After saving a locale:

1. Switch the application to that culture from **Options**.
2. Open the main window, Options window, Job Manager, and Log window.
3. Check context menus, tooltips, and popups in addition to static labels.
4. For script-heavy locales such as Hindi, Japanese, Traditional Chinese, and Korean, also verify that the text renders with readable glyphs on the target OS.
5. If a translation is technically correct but visually too long, shorten it and save again.

## When to Use the Console Instead

Use `src\gMKVToolNix.Translator.Console` when you need to:

- scan source code for hardcoded strings
- rebuild or refresh the master `en.json`
- create or sync locale files in automation or CI-style workflows
- batch-maintain locale files without launching the GUI

For the full command reference, see `src\gMKVToolNix.Translator.Console\README.md`.
