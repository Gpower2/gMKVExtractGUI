# **gMKVToolnix.Translator.Console**

gMKVToolnix.Translator.Console is a command-line utility designed to manage the localization workflow for the gMKVExtractGUI application. It provides tools to find hardcoded strings, create new language templates, and synchronize existing translation files with a master file.

For day-to-day translation work, translators can now use the in-app editor from **Options -> Translations...** inside the GUI. The console remains the developer and automation path for scanning code, rebuilding `gmkvextract-en.json`, and batch-maintaining locale files.

The tool works with the JSON translation files used by the GUI runtime:

- a `Metadata` section (`Culture`, `Translator`, `CreationDate`, `LastEditDate`)
- an `Entries` section keyed by localization key
- per-entry fields for `Source`, `Translation`, `IsTranslated`, and `Notes`

## **Commands**

The utility is split into four main commands (or "verbs"):

1. `scan`: (Initial Step) Scans the source code to find hardcoded strings to help with initial refactoring.
2. `master`: (Maintenance) Scans the refactored code for `GetString()` calls and updates the master `gmkvextract-en.json` file.
3. `template`: (New Language) Creates a new, untranslated file (e.g., `gmkvextract-de.json`) from the master `gmkvextract-en.json`.
4. `sync`: (Maintenance) Updates an existing translation file (e.g., `gmkvextract-de.json`) with new changes from the `gmkvextract-en.json` master.

### **1\. scan**

This command scans a source code directory for hardcoded C\# strings. Its purpose is to help the developer find strings that need to be externalized during the initial localization setup. It generates a `scan_report.json` file as a "to-do" list.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--source | \-s | **Yes** | The root source code directory to scan recursively. |
| \--output | \-o | No | The output path for the report. **Default:** `scan_report.json` |

#### **Example Usage**

```
# Scan the entire solution source tree
gMKVToolnix.Translator.Console.exe scan --source "C:\Projects\gMKVExtractGUI\src"

# Scan a specific project and output the report to a custom file
gMKVToolnix.Translator.Console.exe scan -s "C:\Projects\gMKVExtractGUI\src\gMKVExtractGUI" -o "C:\Temp\gui_strings.json"
```

### **2\. master**

This command scans the source code for literal `GetString("key")` calls. It uses this list of keys to create or update the master `gmkvextract-en.json` file. It preserves all existing source text and notes while adding placeholders for any new keys it finds.

> **Note:** The current scanner targets `GetString(...)` call sites. If you add new literal-key helper shapes in the runtime (for example a future scanner for `GetStringForCulture(...)` calls), update `MasterCommand` accordingly so new keys remain discoverable.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--source | \-s | **Yes** | The root source code directory to scan recursively. |
| \--master | \-m | **Yes** | The path to the master (e.g., `gmkvextract-en.json`) file to create or update. |

#### **Example Usage**

```
# Scan the code and update the gmkvextract-en.json master file
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\gMKVExtractGUI\src" -m "C:\App\Translations\gmkvextract-en.json"
```

### **3\. template**

This command creates a new, blank translation file for a new culture. It uses the master file (e.g., `gmkvextract-en.json`) as a source, copying all keys, source text, and context notes. It marks all new entries as `isTranslated: false`.

> **Note:** This command uses the same shared translation-maintenance logic as the GUI editor's **Create** action.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--master | \-m | **Yes** | The path to the master translation file (e.g., `gmkvextract-en.json`). |
| \--culture | \-c | **Yes** | The culture code for the new file (e.g., `de-DE`, `fr`, `zh-cn`, or `zh-tw`). |
| \--output | \-o | No | The output file path. **Default:** Creates a file named after the culture (e.g., `gmkvextract-de-DE.json`) in the same directory as the master file. |

#### **Example Usage**

```
# Create a new German (Germany) translation file
gMKVToolnix.Translator.Console.exe template --master "C:\App\Translations\gmkvextract-en.json" --culture "de-DE"

# Create a new Simplified Chinese file in a specific output location
gMKVToolnix.Translator.Console.exe template -m "C:\App\Translations\gmkvextract-en.json" -c "zh-cn" -o "C:\Temp\gmkvextract-zh-cn.json"
```

### **4\. sync**

This is the most important command for ongoing maintenance. It synchronizes an existing translation file (the "target") with the master file.

It performs the following actions:

* **Adds** any new strings from the master to the target.
* **Removes** any strings from the target that no longer exist in the master.
* **Updates** any entries where the `source` text in the master has changed. This will reset the translation and mark it as `isTranslated: false` to force a re-translation.

> **Note:** This command uses the same shared translation-maintenance logic as the GUI editor's **Sync** action.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--master | \-m | **Yes** | The path to the master translation file (e.g., `gmkvextract-en.json`). |
| \--target | \-t | **Yes** | The path to the existing translation file to update (e.g., `gmkvextract-de-DE.json`). |

#### **Example Usage**

```
# Sync the German translation file with the latest master file
gMKVToolnix.Translator.Console.exe sync --master "C:\App\Translations\gmkvextract-en.json" --target "C:\App\Translations\gmkvextract-de-DE.json"

# Sync the French file
gMKVToolnix.Translator.Console.exe sync -m "C:\App\Translations\gmkvextract-en.json" -t "C:\App\Translations\gmkvextract-fr-FR.json"
```

## **Typical Workflow**

Here is the end-to-end process for localizing the application:

1. **Initial Scan:** The developer runs scan to find all hardcoded strings.
```
gMKVToolnix.Translator.Console.exe scan -s "C:\Projects\gMKVExtractGUI\src"
```

2. **Refactor Code:** The developer uses `scan_report.json` to refactor all code (e.g., changing `"Open"` to `LocalizationManager.GetString("Gui.MainMenu.Open")`, or to `LocalizationManager.GetStringForCulture("Gui.MainMenu.Open", culture)` only when an explicit culture is truly required).

3. **Generate Master:** The developer runs master to auto-generate the `gmkvextract-en.json` file from the refactored code.
```
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\gMKVExtractGUI\src" -m "C:\App\Translations\gmkvextract-en.json"
```

4. **Edit Master:** The developer opens `gmkvextract-en.json` and fills in the source text and notes for all entries that show \!NEW\!.

5. **Create New Template:** The developer wants to add German. They run template.
```
gMKVToolnix.Translator.Console.exe template -m "C:\App\Translations\gmkvextract-en.json" -c "de-DE"
```

6. **Translation:** A translator usually opens **Options -> Translations...** in the GUI, loads `de-DE`, changes the translation values, and sets `isTranslated` to `true`. Direct JSON editing still works, but the editor is the preferred workflow.

7. **New Features:** Weeks later, the developer adds new features, refactors code, and adds new `_loc.GetString(...)` calls.

8. **Update Master:** The developer re-runs master. The tool finds the new keys and adds them as placeholders to `gmkvextract-en.json`, preserving all old work.
```
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\gMKVExtractGUI\src" -m "C:\App\Translations\gmkvextract-en.json"
```

9. **Edit Master:** The developer opens `gmkvextract-en.json` and fills in the source and notes for the new keys.

10. **Synchronize:** The developer runs sync to update `gmkvextract-de-DE.json`.
```
gMKVToolnix.Translator.Console.exe sync -m "C:\App\Translations\gmkvextract-en.json" -t "C:\App\Translations\gmkvextract-de-DE.json"
```
The console reports that the new strings were added to `gmkvextract-de-DE.json`.

11. **Final Translation:** The translator reopens `de-DE` in the GUI editor and can quickly find the new items that are marked as `isTranslated: false`.

## **Runtime Behavior**

The GUI runtime uses these files directly:

1. `JsonLocalizationService` scans the executable directory for `gmkvextract-*.json` translation files and falls back to legacy bare `<culture>.json` names only when no prefixed files exist yet.
2. Each file is parsed and flattened into an in-memory runtime cache keyed by culture and localization key.
3. Lookups do **not** reread translation files on every call.
4. The GUI also embeds a built-in English fallback map, so missing locale files still resolve to English instead of `!Key!` placeholders.
5. `LocalizationManager.Reload(culture)` rebuilds the cache when the user changes language from `frmOptions`.
6. Invalid or unavailable saved cultures are normalized back to a real available culture, typically `en`.
7. Chinese locale aliases normalize to canonical files: `zh-cn` for Simplified Chinese, `zh-tw` for Traditional Chinese, and legacy `cn` resolves to `zh-tw`.
8. Lookup fallback order is: requested culture -> neutral culture -> `en` -> `!Key!`.
9. Formatted lookup failures are logged and surfaced as `!BadFormat:Key!` rather than being silently ignored.
