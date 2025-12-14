# **gMKVToolnix.Translator.Console**

gMKVToolnix.Translator.Console is a command-line utility designed to manage the localization workflow for the gMKVToolnix application. It provides tools to find hardcoded strings, create new language templates, and synchronize existing translation files with a master file.

This tool works with a specific JSON structure that includes metadata and detailed translation entries, making it easy to integrate with other tools, such as a translation GUI.

## **Commands**

The utility is split into three main commands (or "verbs"):

1. `scan`: (Initial Step) Scans the source code to find hardcoded strings to help with initial refactoring.  
2. `master`: (Maintenance) Scans the refactored code for `GetString()` calls and updates the master `en.json` file.  
3. `template`: (New Language) Creates a new, untranslated file (e.g., `de.json`) from the master `en.json`.  
4. `sync`: (Maintenance) Updates an existing translation file (e.g., `de.json`) with new changes from the `en.json` master.

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
gMKVToolnix.Translator.Console.exe scan --source "C:\Projects\gMKVToolnix"

# Scan a specific project and output the report to a custom file  
gMKVToolnix.Translator.Console.exe scan -s "C:\Projects\gMKVToolnix\gMKVToolnix.GUI" -o "C:\Temp\gui_strings.json"
```

### **2\. master**

This command scans the source code for all `GetString("key")` calls. It uses this list of keys to create or update the master `en.json` file. It preserves all existing source text and notes while adding placeholders for any new keys it finds.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--source | \-s | **Yes** | The root source code directory to scan recursively. |
| \--master | \-m | **Yes** | The path to the master (e.g., `en.json`) file to create or update. |

#### **Example Usage**

```
# Scan the code and update the en.json master file  
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\\gMKVToolnix" -m "C:\App\Translations\en.json"
```

### **3\. template**

This command creates a new, blank translation file for a new culture. It uses the master file (e.g., `en.json`) as a source, copying all keys, source text, and context notes. It marks all new entries as `isTranslated: false`.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--master | \-m | **Yes** | The path to the master translation file (e.g., `en.json`). |
| \--culture | \-c | **Yes** | The culture code for the new file (e.g., `de-DE` or `fr`). |
| \--output | \-o | No | The output file path. **Default:** Creates a file named after the culture (e.g., `de-DE.json`) in the same directory as the master file. |

#### **Example Usage**

```
# Create a new German (Germany) translation file  
gMKVToolnix.Translator.Console.exe template --master "C:\App\Translations\en.json" --culture "de-DE"

# Create a new French file in a specific output location  
gMKVToolnix.Translator.Console.exe template -m "C:\App\Translations\en.json" -c "fr-FR" -o "C:\Temp\new_french_file.json"
```

### **4\. sync**

This is the most important command for ongoing maintenance. It synchronizes an existing translation file (the "target") with the master file.

It performs the following actions:

* **Adds** any new strings from the master to the target.  
* **Removes** any strings from the target that no longer exist in the master.  
* **Updates** any entries where the `source` text in the master has changed. This will reset the translation and mark it as `isTranslated: false` to force a re-translation.

#### **Options**

| Option | Short | Required | Description |
| :---- | :---- | :---- | :---- |
| \--master | \-m | **Yes** | The path to the master translation file (e.g., `en.json`). |
| \--target | \-t | **Yes** | The path to the existing translation file to update (e.g., `de-DE.json`). |

#### **Example Usage**

```
# Sync the German translation file with the latest master file  
gMKVToolnix.Translator.Console.exe sync --master "C:\App\Translations\en.json" --target "C:\App\Translations\de-DE.json"

# Sync the French file  
gMKVToolnix.Translator.Console.exe sync -m "C:\App\Translations\en.json" -t "C:\App\Translations\fr-FR.json"
```

## **Typical Workflow**

Here is the end-to-end process for localizing the application:

1. **Initial Scan:** The developer runs scan to find all hardcoded strings.  
```
gMKVToolnix.Translator.Console.exe scan -s "C:\Projects\gMKVToolnix"
```

2. **Refactor Code:** The developer uses `scan_report.json` to refactor all code (e.g., changing `"Open"` to `_loc.GetString("Gui.MainMenu.Open", culture)`).  

3. **Generate Master:** The developer runs master to auto-generate the `en.json` file from the refactored code.  
```
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\gMKVToolnix" -m "C:\App\Translations\en.json"
```

4. **Edit Master:** The developer opens `en.json` and fills in the source text and notes for all entries that show \!NEW\!.  

5. **Create New Template:** The developer wants to add German. They run template.  
```
gMKVToolnix.Translator.Console.exe template -m "C:\App\Translations\en.json" -c "de-DE"
```

6. **Translation:** A translator opens `de-DE.json`, changes the translation values, and sets `isTranslated` to `true`.  

7. **New Features:** Weeks later, the developer adds new features, refactors code, and adds new `_loc.GetString(...)` calls.  

8. **Update Master:** The developer re-runs master. The tool finds the new keys and adds them as placeholders to `en.json`, preserving all old work.  
```
gMKVToolnix.Translator.Console.exe master -s "C:\Projects\gMKVToolnix" -m "C:\App\Translations\en.json"
```

9. **Edit Master:** The developer opens `en.json` and fills in the source and notes for the new keys.  

10. **Synchronize:** The developer runs sync to update `de-DE.json`.  
```    
gMKVToolnix.Translator.Console.exe sync -m "C:\App\Translations\en.json" -t "C:\App\Translations\de-DE.json"
```
The console reports that the new strings were added to `de-DE.json`.  

11. **Final Translation:** The translator opens `de-DE.json` and can now easily find and translate the new items that are marked as `isTranslated: false`.