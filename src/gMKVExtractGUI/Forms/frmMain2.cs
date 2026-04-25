using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Jobs;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.MkvExtract;
using gMKVToolNix.MkvInfo;
using gMKVToolNix.MkvMerge;
using gMKVToolNix.Segments;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix.Forms
{
    public enum TrackSelectionMode
    {
        video,
        audio,
        subtitle,
        chapter,
        attachment,
        all
    }

    public delegate void UpdateProgressDelegate(object val);
    public delegate void UpdateTrackLabelDelegate(object filename, object val);

    public partial class frmMain2 : gForm, IFormMain
    {
        private const int MainActionRowHeight = 90;
        private const int MainActionLeftMargin = 6;
        private const int MainActionRightMargin = 7;
        private const int MainActionBottomPadding = 6;
        private const int MainActionSingleRowSpacing = 8;
        private const int MainActionTopRowButtonTop = 18;
        private const int MainActionBottomRowButtonTop = 48;
        private const int MainActionComboTopOffset = 3;
        private const int MainActionLabelTopOffset = 8;
        private const int MainButtonSpacing = 6;
        private frmLog _LogForm = null;
        private frmJobManager _JobManagerForm = null;
        private ToolTip _ToolTip = null;

        private gMKVExtract _gMkvExtract = null;

        private readonly gSettings _Settings = null;

        private bool _FromConstructor = false;

        private bool _ExtractRunning = false;

        private int _CurrentJob = 0;
        private int _TotalJobs = 0;

        private List<string> _CmdArguments = new List<string>();
        private readonly Dictionary<Button, Size> _responsiveButtonBaseSizes = new Dictionary<Button, Size>();
        private bool _contextMenuItemsDirty = true;
        private bool _isApplyingResponsiveLayout = false;
        private int _chapterTypeComboBaseWidth;
        private int _extractionModeComboBaseWidth;
        private float _actionsRowBaseHeight;
        private int _fileOptionsPanelBaseHeight;
        private float _fileOptionsRowBaseHeight;

        public frmMain2()
        {
            try
            {
                _FromConstructor = true;

                InitializeComponent();
                CaptureResponsiveLayoutBaselines();

                // Get the command line arguments
                GetCommandLineArguments();

                // Set form icon from the executing assembly
                Icon = Icon.ExtractAssociatedIcon(this.GetExecutingAssemblyLocation());

                // Set form title
                Text = string.Format("gMKVExtractGUI v{0} -- By Gpower2", this.GetCurrentVersion());

                btnAbort.Enabled = false;
                btnAbortAll.Enabled = false;
                btnOptions.Enabled = true;

                cmbChapterType.DataSource = Enum.GetNames(typeof(MkvChapterTypes));
                cmbExtractionMode.DataSource = Enum.GetNames(typeof(FormMkvExtractionMode));

                ClearStatus();

                // Load settings
                _Settings = new gSettings(this.GetCurrentDirectory());
                _Settings.Reload();

                // Set form size and position from settings
                gMKVLogger.Log("Begin setting form size and position from settings...");
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(_Settings.WindowPosX, _Settings.WindowPosY);
                this.Size = new System.Drawing.Size(_Settings.WindowSizeWidth, _Settings.WindowSizeHeight);
                this.WindowState = _Settings.WindowState;
                gMKVLogger.Log("Finished setting form size and position from settings!");

                // Set chapter type, output directory and job mode from settings
                gMKVLogger.Log("Begin setting chapter type, output directory and job mode from settings...");
                cmbChapterType.SelectedItem = Enum.GetName(typeof(MkvChapterTypes), _Settings.ChapterType);
                chkUseSourceDirectory.Checked = _Settings.LockedOutputDirectory;
                // Only set the output directory if we don't use the source directory
                if (!chkUseSourceDirectory.Checked)
                {
                    txtOutputDirectory.Text = _Settings.OutputDirectory;
                }
                chkShowPopup.Checked = _Settings.ShowPopup;
                chkAppendOnDragAndDrop.Checked = _Settings.AppendOnDragAndDrop;
                chkOverwriteExistingFiles.Checked = _Settings.OverwriteExistingFiles;
                chkDisableTooltips.Checked = _Settings.DisableTooltips;
                chkDarkMode.Checked = _Settings.DarkMode;
                gMKVLogger.Log("Finished setting chapter type, output directory and job mode from settings!");

                _FromConstructor = false;

                ThemeManager.ApplyTheme(this, _Settings.DarkMode); // Apply theme on startup
                // Hack for DarkMode checkbox
                if (_Settings.DarkMode)
                {
                    chkDarkMode.BackColor = Color.FromArgb(55, 55, 55);
                }

                if (this.Handle != IntPtr.Zero) // Ensure handle is created
                {
                    NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);
                }
                else
                {
                    // If handle not created yet, do it in Load or Shown event
                    this.Shown += (s, ev) => {
                        NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
                        NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);
                    };
                }

                // Initialize the DPI aware scaling
                InitDPI();
                CaptureResponsiveLayoutBaselines();

                // Apply localization
                ApplyLocalization();

                // Set the tooltips for the controls
                SetTooltips(!chkDisableTooltips.Checked);

                // Check if user manually provided MKVToolNix path
                bool manualMkvToolNixPath = false;
                bool manualPathOK = true;

                if (_CmdArguments.Any()
                    && _CmdArguments.Where(c => c.StartsWith("--")).Any()
                    && _CmdArguments.Where(c => c.ToLower().StartsWith("--mkvtoolnix=")).Any()
                )
                {
                    // User provided a manual MKVToolNix path
                    manualMkvToolNixPath = true;
                    // Get the commend line argument
                    string arg = _CmdArguments.Where(c => c.ToLower().StartsWith("--mkvtoolnix=")).FirstOrDefault();
                    // Get the path
                    string manualPath = arg.Substring(13);
                    // Log the path
                    gMKVLogger.Log($"User provided a manual path for MKVToolNix: {manualPath}");

                    if (string.IsNullOrWhiteSpace(manualPath))
                    {
                        manualPathOK = false;
                        gMKVLogger.Log("The manual path for MKVToolNix was empty!");
                    }
                    else
                    {
                        if (!Directory.Exists(manualPath))
                        {
                            manualPathOK = false;
                            gMKVLogger.Log($"The manual path for MKVToolNix does not exist! ({manualPath})");
                        }
                        else
                        {
                            if (!File.Exists(Path.Combine(manualPath, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                                && !File.Exists(Path.Combine(manualPath, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME))
                            )
                            {
                                manualPathOK = false;
                                gMKVLogger.Log($"mkvmerge was not found in manual path! ({manualPath})");
                            }
                            else
                            {
                                // We set the flag to bypass the checks
                                // since it's a manual path from the arguments and we don't want to save it in the settings
                                _FromConstructor = true;
                                txtMKVToolnixPath.Text = manualPath;
                                _FromConstructor = false;
                            }
                        }
                    }
                }

                if (manualMkvToolNixPath && !manualPathOK)
                {
                    gMKVLogger.Log("Failed to set manual path! Trying to auto-detect...");
                }

                if (!manualMkvToolNixPath || (manualMkvToolNixPath && !manualPathOK))
                {
                    // Find MKVToolnix path
                    try
                    {
                        // First check the ini file
                        if (!string.IsNullOrWhiteSpace(_Settings.MkvToolnixPath)
                            && (File.Exists(Path.Combine(_Settings.MkvToolnixPath, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                            || File.Exists(Path.Combine(_Settings.MkvToolnixPath, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME))))
                        {
                            // We set the flag to bypass the checks
                            // since the path already exists in the settings
                            _FromConstructor = true;
                            txtMKVToolnixPath.Text = _Settings.MkvToolnixPath;
                            _FromConstructor = false;
                        }
                        else
                        {
                            gMKVLogger.Log($"mkvmerge was not found in ini path! ({_Settings.MkvToolnixPath})");

                            AutoDetectMkvToolnixPath();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        gMKVLogger.Log(ex.ToString());

                        // MKVToolnix could not be found anywhere
                        // Select exception message according to running OS
                        string exceptionMessage = "";
                        if (PlatformExtensions.IsOnLinux)
                        {
                            exceptionMessage = LocalizationManager.GetString("UI.MainForm2.Errors.AutoDetectLinuxNotFound");
                        }
                        else
                        {
                            exceptionMessage = LocalizationManager.GetString("UI.MainForm2.Errors.AutoDetectWindowsNotFound");
                        }
                        gMKVLogger.Log(exceptionMessage);
                        throw CreateLocalizedException("UI.MainForm2.Errors.AutoDetectManualPathHint", exceptionMessage, Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                _FromConstructor = false;
                ShowErrorMessage(ex.Message);
            }
        }

        private void UpdateInputFilesGroupTitle()
        {
            int fileCount = trvInputFiles.AllNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo);
            grpInputFiles.Text = fileCount > 0
                ? LocalizationManager.GetString("UI.MainForm2.InputFiles.GroupWithCount", fileCount)
                : LocalizationManager.GetString("UI.MainForm2.InputFiles.Group");
        }

        private string GetSelectedFileInfoTitle(string filename = null)
        {
            return string.IsNullOrWhiteSpace(filename)
                ? LocalizationManager.GetString("UI.MainForm2.SelectedFileInfo.Group")
                : LocalizationManager.GetString("UI.MainForm2.SelectedFileInfo.GroupWithFile", filename);
        }

        private void UpdateSelectedFileInfoTitle(TreeNode selectedNode = null)
        {
            TreeNode node = selectedNode ?? trvInputFiles.SelectedNode;
            if (node != null && node.Tag != null && !(node.Tag is gMKVSegmentInfo))
            {
                node = node.Parent;
            }

            if (node != null && node.Tag is gMKVSegmentInfo segInfo)
            {
                grpSelectedFileInfo.Text = GetSelectedFileInfoTitle(segInfo.Filename);
            }
            else
            {
                grpSelectedFileInfo.Text = GetSelectedFileInfoTitle();
            }
        }

        private string GetContextMenuTrackGroupLabel(TrackSelectionMode selectionMode)
        {
            switch (selectionMode)
            {
                case TrackSelectionMode.video:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroup.Video");
                case TrackSelectionMode.audio:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroup.Audio");
                case TrackSelectionMode.subtitle:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroup.Subtitle");
                case TrackSelectionMode.chapter:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroup.Chapter");
                case TrackSelectionMode.attachment:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroup.Attachment");
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionMode), selectionMode, null);
            }
        }

        private string GetContextMenuFilterLabel(NodeSelectionFilter filter, TrackSelectionMode selectionMode)
        {
            switch (filter)
            {
                case NodeSelectionFilter.Language:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.Language");
                case NodeSelectionFilter.LanguageIetf:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.LanguageIetf");
                case NodeSelectionFilter.ExtraInfo:
                    return selectionMode == TrackSelectionMode.video
                        ? LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.Resolution")
                        : LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.Channels");
                case NodeSelectionFilter.CodecId:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.Codec");
                case NodeSelectionFilter.Name:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.TrackName");
                case NodeSelectionFilter.Forced:
                    return LocalizationManager.GetString("UI.MainForm2.ContextMenu.Filter.Forced");
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        private string GetLocalizedBoolean(bool value)
        {
            return LocalizationManager.GetString(value ? "UI.Common.True" : "UI.Common.False");
        }

        private void MarkContextMenuDirty()
        {
            _contextMenuItemsDirty = true;
        }

        private void ResetDynamicContextMenuItems(ToolStripMenuItem parentItem, ToolStripItem staticItem)
        {
            for (int i = parentItem.DropDownItems.Count - 1; i >= 0; i--)
            {
                if (parentItem.DropDownItems[i] != staticItem)
                {
                    parentItem.DropDownItems.RemoveAt(i);
                }
            }

            if (!parentItem.DropDownItems.Contains(staticItem))
            {
                parentItem.DropDownItems.Insert(0, staticItem);
            }
        }

        private void AutoDetectMkvToolnixPath()
        {
            // Check the current directory
            string currentDirectory = GetCurrentDirectory();
            gMKVLogger.Log($"Checking in current Directory for mkvmerge... ({currentDirectory})");

            if (File.Exists(Path.Combine(currentDirectory, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                || File.Exists(Path.Combine(currentDirectory, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
            {
                // We don't set the flag to bypass the checks here
                // since we want the current directory to be saved in the settings
                txtMKVToolnixPath.Text = currentDirectory;
            }
            else
            {
                gMKVLogger.Log($"mkvmerge was not found in current directory! ({currentDirectory})");

                if (!PlatformExtensions.IsOnLinux)
                {
                    // When on Windows, check the registry
                    gMKVLogger.Log("Checking registry for mkvmerge...");

                    // We don't set the flag to bypass the checks here
                    // since we want the registry value to be saved in the settings
                    txtMKVToolnixPath.Text = gMKVHelper.GetMKVToolnixPathViaRegistry();
                }
                else
                {
                    // When on Linux, check the usr/bin first
                    string linuxDefaultPath = Path.Combine("/usr", "bin");
                    if (File.Exists(Path.Combine(linuxDefaultPath, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                        || File.Exists(Path.Combine(linuxDefaultPath, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
                    {
                        // We don't set the flag to bypass the checks here
                        // since we want the current directory to be saved in the settings
                        txtMKVToolnixPath.Text = linuxDefaultPath;
                    }
                    else
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.MkvmergeNotFoundInPath", linuxDefaultPath);
                    }
                }
            }
        }

        private void SetTooltips(bool argEnabled)
        {
            if (argEnabled)
            {
                ResetTooltipComponent();
                AddTooltips();
            }
            else
            {
                ClearTooltips();
            }
        }

        private ToolTip CreateToolTip()
        {
            return new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 1000,
                ReshowDelay = 100,
                IsBalloon = false
            };
        }

        private void ResetTooltipComponent()
        {
            if (_ToolTip != null)
            {
                _ToolTip.Active = false;
                _ToolTip.RemoveAll();
                _ToolTip.Dispose();
                _ToolTip = null;
            }

            _ToolTip = CreateToolTip();
        }

        private void RefreshLocalizedTooltipsAsync()
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke((MethodInvoker)delegate
            {
                if (IsDisposed || Disposing)
                {
                    return;
                }

                SetTooltips(!chkDisableTooltips.Checked);
            });
        }

        private void AddTooltips()
        {
            if (_ToolTip == null)
            {
                _ToolTip = CreateToolTip();
            }

            _ToolTip.SetToolTip(btnAutoDetectMkvToolnix,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.AutoDetect"));

            string inputTooltip = LocalizationManager.GetString("UI.MainForm2.Tooltips.InputFiles");

            _ToolTip.SetToolTip(grpInputFiles, inputTooltip);

            _ToolTip.SetToolTip(trvInputFiles, inputTooltip);

            _ToolTip.SetToolTip(chkAppendOnDragAndDrop,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.AppendOnDragAndDrop"));

            _ToolTip.SetToolTip(chkOverwriteExistingFiles,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.OverwriteExistingFiles"));

            _ToolTip.SetToolTip(chkUseSourceDirectory,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.UseSourceDirectory"));

            _ToolTip.SetToolTip(chkShowPopup,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.ShowPopup"));

            _ToolTip.SetToolTip(btnSelect,
                LocalizationManager.GetString("UI.MainForm2.Tooltips.Select"));
        }

        private void ClearTooltips()
        {
            if (_ToolTip != null)
            {
                _ToolTip.RemoveAll();
            }
        }

        private void btnAutoDetectMkvToolnix_Click(object sender, EventArgs e)
        {
            try
            {
                AutoDetectMkvToolnixPath();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void frmMain2_Shown(object sender, EventArgs e)
        {
            try
            {
                // check if user provided with a filename when executing the application
                if (_CmdArguments.Any()
                    && _CmdArguments.Where(c => !c.StartsWith("--")).Any())
                {
                    LoadFilesFromInputFileDrop(_CmdArguments.Where(c => !c.StartsWith("--")).ToArray());
                }
            }
            catch (Exception ex)
            {
                HandleInputFileDropFailure(ex);
            }
        }

        private void GetCommandLineArguments()
        {
            // check if user provided with command line arguments when executing the application
            string[] cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs.Length > 1)
            {
                // Copy the results to a list
                _CmdArguments = cmdArgs.ToList();
                // Remove the first argument (the executable)
                _CmdArguments.RemoveAt(0);

                // Log the commandline arguments
                gMKVLogger.Log(string.Format("Found command line arguments: {0}", string.Join(",", _CmdArguments)));
            }
        }

        private void txt_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // check if the drop data is actually a file or folder
                if (e != null && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // check for sender
                    if (sender == txtMKVToolnixPath)
                    {
                        // check if MKVToolnix Path is already set
                        if (!string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
                        {
                            if (ShowLocalizedQuestion("UI.MainForm2.Dialogs.ChangeMkvToolnixPathQuestion", "UI.Common.Dialog.AreYouSureTitle", false) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }
                    else if (sender == txtOutputDirectory)
                    {
                        // check if output directory is the same as the source
                        if (chkUseSourceDirectory.Checked)
                        {
                            return;
                        }
                    }

                    string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                    if (s != null && s.Length > 0)
                    {
                        ((gTextBox)sender).Text = s[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void txt_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e != null && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    if (sender == txtOutputDirectory)
                    {
                        // check if output directory is the same as the source
                        if (chkUseSourceDirectory.Checked)
                        {
                            e.Effect = DragDropEffects.None;
                        }
                        else
                        {
                            // check if it is a directory or not
                            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (s != null && s.Length > 0 && Directory.Exists(s[0]))
                            {
                                e.Effect = DragDropEffects.All;
                            }
                        }
                    }
                    else
                    {
                        e.Effect = DragDropEffects.All;
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private sealed class InputFileDropAnalysis
        {
            public bool ContainsDirectories { get; set; }

            public bool ContainsSubDirectories { get; set; }
        }

        private void LoadFilesFromInputFileDrop(string[] argFileDrop, bool argAppend = false)
        {
            try
            {
                tlpMain.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtSegmentInfo.Text = LocalizationManager.GetString("UI.Common.Status.GettingFiles");

                Task.Factory.StartNew(
                    () => AnalyzeInputFileDrop(argFileDrop),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                throw task.Exception == null
                                    ? new Exception("Failed to analyze dropped files.")
                                    : task.Exception.GetBaseException();
                            }

                            SearchOption directorySearchOption = SearchOption.TopDirectoryOnly;
                            InputFileDropAnalysis analysis = task.Result;

                            if (analysis.ContainsDirectories && analysis.ContainsSubDirectories)
                            {
                                Cursor = Cursors.Default;
                                DialogResult result = ShowLocalizedQuestion(
                                    "UI.MainForm2.Dialogs.IncludeSubDirectoriesQuestion",
                                    "UI.MainForm2.Dialogs.SubDirectoriesFoundTitle");
                                Cursor = Cursors.WaitCursor;

                                if (result == DialogResult.Cancel)
                                {
                                    throw CreateLocalizedException("UI.MainForm2.Errors.NoValidMatroskaFiles");
                                }

                                directorySearchOption = result == DialogResult.Yes
                                    ? SearchOption.AllDirectories
                                    : SearchOption.TopDirectoryOnly;
                            }

                            Task.Factory.StartNew(
                                () => BuildFileListFromInputFileDrop(argFileDrop, directorySearchOption),
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskScheduler.Default)
                                .ContinueWith(fileTask =>
                                {
                                    try
                                    {
                                        if (fileTask.IsFaulted)
                                        {
                                            throw fileTask.Exception == null
                                                ? new Exception("Failed to gather dropped files.")
                                                : fileTask.Exception.GetBaseException();
                                        }

                                        List<string> fileList = fileTask.Result;
                                        if (!fileList.Any())
                                        {
                                            throw CreateLocalizedException("UI.MainForm2.Errors.NoValidMatroskaFiles");
                                        }

                                        AddFileNodes(
                                            txtMKVToolnixPath.Text,
                                            fileList,
                                            argAppend,
                                            delegate { Cursor = Cursors.Default; });
                                    }
                                    catch (Exception ex)
                                    {
                                        HandleInputFileDropFailure(ex);
                                    }
                                }, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                        catch (Exception ex)
                        {
                            HandleInputFileDropFailure(ex);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                HandleInputFileDropFailure(ex);
            }
        }

        private InputFileDropAnalysis AnalyzeInputFileDrop(string[] argFileDrop)
        {
            List<string> directories = argFileDrop.Where(f => Directory.Exists(f)).ToList();

            return new InputFileDropAnalysis
            {
                ContainsDirectories = directories.Any(),
                ContainsSubDirectories = directories.Any(t =>
                    Directory.GetDirectories(t, "*", SearchOption.TopDirectoryOnly).Any())
            };
        }

        private List<string> BuildFileListFromInputFileDrop(string[] argFileDrop, SearchOption argDirectorySearchOption)
        {
            List<string> fileList = new List<string>();

            argFileDrop.Where(f => Directory.Exists(f))
                .ToList()
                .ForEach(t => fileList.AddRange(Directory.GetFiles(t, "*", argDirectorySearchOption).ToList()));

            fileList.AddRange(argFileDrop.Where(f => File.Exists(f)));

            // Remove all non valid matroska files
            fileList.RemoveAll(f =>
            {
                string extension = Path.GetExtension(f).ToLower();
                return
                    extension != ".mkv"
                    && extension != ".mka"
                    && extension != ".mks"
                    && extension != ".mk3d"
                    && extension != ".webm";
            });

            return fileList;
        }

        private void HandleInputFileDropFailure(Exception ex)
        {
            Debug.WriteLine(ex);
            gMKVLogger.Log(ex.ToString());
            Cursor = Cursors.Default;
            ShowErrorMessage(ex.Message);
            tlpMain.Enabled = true;
        }

        private void trvInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // check if the drop data is actually a file or folder
                if (e != null && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                    if (s != null && s.Length > 0)
                    {
                        LoadFilesFromInputFileDrop(s, chkAppendOnDragAndDrop.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleInputFileDropFailure(ex);
            }
        }

        private void trvInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e != null && e.Data != null)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        e.Effect = DragDropEffects.All;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private class NodeResults
        {
            public List<TreeNode> Nodes { get; set; }
            public List<string> InformationMessages { get; set; }
            public List<string> WarningMessages { get; set; }
            public List<string> ErrorMessages { get; set; }

            public NodeResults()
            {
                InformationMessages = new List<string>();
                WarningMessages = new List<string>();
                ErrorMessages = new List<string>();
            }
        }

        private void AddFileNodes(string argMKVToolNixPath, List<string> argFiles, bool argAppend = false, Action argOnCompleted = null)
        {
            try
            {
                tlpMain.Enabled = false;

                // empty all the controls in any case
                ClearControls();

                // Check for append file or not
                if (!argAppend)
                {
                    trvInputFiles.Nodes.Clear();
                }
                else
                {
                    // Remove files that already exist in the TreeView
                    argFiles.RemoveAll(f =>
                        trvInputFiles.AllNodes.Any(n =>
                            n != null
                            && n.Tag != null
                            && n.Tag is gMKVSegmentInfo segInfo
                            && segInfo.Path.Equals(f, StringComparison.InvariantCultureIgnoreCase)
                    ));

                    // Check if there are any new files to add
                    if (!argFiles.Any())
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.NoNewFiles");
                    }
                }

                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Indeterminate);

                Task.Factory.StartNew(
                    () => GetFileInfoNodes(argMKVToolNixPath, argFiles),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Error);
                                throw task.Exception == null
                                    ? new Exception("Failed to analyze input files.")
                                    : task.Exception.GetBaseException();
                            }

                            NodeResults results = task.Result;

                            trvInputFiles.BeginUpdate();
                            try
                            {
                                // Add the nodes to the TreeView
                                trvInputFiles.Nodes.AddRange(results.Nodes.ToArray());
                                MarkContextMenuDirty();
                                trvInputFiles.ExpandAll();
                            }
                            finally
                            {
                                trvInputFiles.EndUpdate();
                            }

                            // Remove the check box from the nodes that contain the gMKVSegmentInfo
                            trvInputFiles.AllNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo)
                                .ToList()
                                .ForEach(n => trvInputFiles.SetIsCheckBoxVisible(n, false));

                            // Check for error messages
                            if (results.ErrorMessages != null && results.ErrorMessages.Any())
                            {
                                ShowErrorMessage(string.Join(Environment.NewLine, results.ErrorMessages));
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            gMKVLogger.Log(ex.ToString());
                            ShowErrorMessage(ex.Message);
                        }
                        finally
                        {
                            prgBrStatus.Value = 0;
                            lblStatus.Text = "";

                            UpdateInputFilesGroupTitle();

                            tlpMain.Enabled = true;
                            gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);

                            if (argOnCompleted != null)
                            {
                                argOnCompleted();
                            }
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);

                prgBrStatus.Value = 0;
                lblStatus.Text = "";
                UpdateInputFilesGroupTitle();
                tlpMain.Enabled = true;
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);

                if (argOnCompleted != null)
                {
                    argOnCompleted();
                }
            }
        }

        private NodeResults GetFileInfoNodes(string argMKVToolNixPath, List<string> argFiles)
        {
            NodeResults results = new NodeResults();
            List<TreeNode> fileNodes = new List<TreeNode>();

            gMKVMerge gMerge = new gMKVMerge(argMKVToolNixPath);
            gMKVInfo gInfo = new gMKVInfo(argMKVToolNixPath);

            statusStrip.Invoke((MethodInvoker)delegate
            {
                prgBrStatus.Maximum = argFiles.Count;
            });
            int counter = 0;

            foreach (var sf in argFiles.OrderBy(f => Path.GetDirectoryName(f)).ThenBy(f => Path.GetFileName(f)))
            {
                counter++;
                txtSegmentInfo.Invoke((MethodInvoker)delegate
                {
                    txtSegmentInfo.Text = LocalizationManager.GetString("UI.Common.Status.AnalyzingFile", Path.GetFileName(sf));
                });

                statusStrip.Invoke((MethodInvoker)delegate
                {
                    prgBrStatus.Value = counter;
                    lblStatus.Text = string.Format("{0}%",
                        Convert.ToInt32((double)prgBrStatus.Value / (double)prgBrStatus.Maximum * 100.0));
                });

                try
                {
                    fileNodes.Add(GetFileNode(gMerge, gInfo, sf));
                }
                catch (Exception ex)
                {
                    results.ErrorMessages.Add(LocalizationManager.GetString("UI.MainForm2.Errors.FileProcessing", Path.GetFileName(sf), ex.Message));
                }
            }

            txtSegmentInfo.Invoke((MethodInvoker)delegate
            {
                txtSegmentInfo.Clear();
            });

            results.Nodes = fileNodes;
            return results;
        }

        private TreeNode GetFileNode(gMKVMerge gMerge, gMKVInfo gInfo, string argFilename)
        {
            // Check if filename was provided
            if (string.IsNullOrWhiteSpace(argFilename))
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.NoFilenameProvided");
            }

            // Check if file exists
            if (!File.Exists(argFilename))
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.FileDoesNotExist", argFilename);
            }

            // Check if the extension is a valid matroska file
            string inputExtension = Path.GetExtension(argFilename).ToLowerInvariant();
            if (inputExtension != ".mkv"
                && inputExtension != ".mka"
                && inputExtension != ".mks"
                && inputExtension != ".mk3d"
                && inputExtension != ".webm")
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.InvalidMatroskaInputFile", argFilename, Environment.NewLine);
            }

            // get the file information
            List<gMKVSegment> segmentList = gMKVHelper.GetMergedMkvSegmentList(gMerge, gInfo, argFilename);

            gMKVSegmentInfo segInfo = segmentList.OfType<gMKVSegmentInfo>().FirstOrDefault();

            TreeNode infoNode = new TreeNode(Path.GetFileName(argFilename))
            {
                Tag = segInfo
            };

            foreach (gMKVSegment seg in segmentList.Where(s => !(s is gMKVSegmentInfo)).ToList())
            {
                TreeNode segNode = new TreeNode(seg.ToString())
                {
                    Tag = seg
                };
                infoNode.Nodes.Add(segNode);
            }

            return infoNode;
        }

        private void trvInputFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (trvInputFiles.SelectedNode != null)
                {
                    TreeNode selNode = trvInputFiles.SelectedNode;
                    if (selNode.Tag == null)
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.SelectedNodeNullTag");
                    }

                    if (!(selNode.Tag is gMKVSegmentInfo))
                    {
                        // Get parent node
                        selNode = selNode.Parent;
                        if (selNode == null)
                        {
                            throw CreateLocalizedException("UI.MainForm2.Errors.SelectedNodeNoParent");
                        }

                        if (selNode.Tag == null)
                        {
                            throw CreateLocalizedException("UI.MainForm2.Errors.SelectedNodeNullTag");
                        }

                        if (!(selNode.Tag is gMKVSegmentInfo))
                        {
                            throw CreateLocalizedException("UI.MainForm2.Errors.SelectedNodeNoInfo");
                        }
                    }

                    gMKVSegmentInfo seg = selNode.Tag as gMKVSegmentInfo;
                    txtSegmentInfo.Text = LocalizationManager.GetString(
                        "UI.MainForm2.SelectedFileInfo.Details",
                        seg.WritingApplication,
                        seg.MuxingApplication,
                        seg.Duration,
                        seg.Date,
                        Environment.NewLine);

                    // check if output directory is the same as the source
                    if (chkUseSourceDirectory.Checked)
                    {
                        // set output directory to the source directory
                        txtOutputDirectory.Text = seg.Directory;
                    }

                    // Set the GroupBox title
                    UpdateSelectedFileInfoTitle(selNode);
                }
                else
                {
                    txtSegmentInfo.Clear();
                    UpdateSelectedFileInfoTitle();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        void g_MkvExtractTrackUpdated(string filename, string trackName)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new UpdateTrackLabelDelegate(UpdateTrackLabel), filename, trackName);
        }

        void g_MkvExtractProgressUpdated(int progress)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new UpdateProgressDelegate(UpdateProgress), progress);
        }

        public void UpdateProgress(object val)
        {
            int progressValue = Convert.ToInt32(val);

            prgBrStatus.Value = progressValue;
            prgBrTotalStatus.Value = (_CurrentJob - 1) * 100 + progressValue;
            lblStatus.Text = string.Format("{0}%", progressValue);
            lblTotalStatus.Text = string.Format("{0}%", prgBrTotalStatus.Value / _TotalJobs);

            // Update the task bar progress bar based on the total progress and not on the individual job
            gTaskbarProgress.SetValue(this, Convert.ToUInt64(prgBrTotalStatus.Value), (ulong)prgBrTotalStatus.Maximum);
            //gTaskbarProgress.SetValue(this, Convert.ToUInt64(val), (UInt64)100);
        }

        public void UpdateTrackLabel(object filename, object val)
        {
            txtSegmentInfo.Text = LocalizationManager.GetString("UI.Common.Status.ExtractingTrack", val, Path.GetFileName((string)filename));
        }

        private void CheckNeccessaryInputFields(bool checkSelectedTracks, bool checkSelectedChapterType)
        {
            if (string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.MkvToolnixPathRequired");
            }

            if (!File.Exists(Path.Combine(txtMKVToolnixPath.Text.Trim(), gMKVHelper.MKV_MERGE_GUI_FILENAME))
                && !File.Exists(Path.Combine(txtMKVToolnixPath.Text.Trim(), gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.MkvToolnixFilesMissing");
            }

            if (!chkUseSourceDirectory.Checked && string.IsNullOrWhiteSpace(txtOutputDirectory.Text))
            {
                throw CreateLocalizedException("UI.MainForm2.Errors.OutputDirectoryRequired");
            }

            // Get the checked nodes
            List<TreeNode> checkedNodes = trvInputFiles.CheckedNodes;

            if (checkSelectedTracks)
            {
                FormMkvExtractionMode selectedExtractionMode = (FormMkvExtractionMode)Enum.Parse(
                    typeof(FormMkvExtractionMode),
                    (string)cmbExtractionMode.SelectedItem);

                // Check if the checked nodes contain tracks
                if (!checkedNodes.Any(t => t.Tag != null && !(t.Tag is gMKVSegmentInfo)))
                {
                    if (selectedExtractionMode == FormMkvExtractionMode.Cue_Sheet || selectedExtractionMode == FormMkvExtractionMode.Tags)
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.TrackRequiredForMode", cmbExtractionMode.SelectedItem);
                    }
                    else
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.TrackRequired");
                    }
                }

                if (selectedExtractionMode == FormMkvExtractionMode.Timecodes ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Timecodes ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes)
                {
                    // Check if the ckecked nodes contain video, audio or subtitle track
                    if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVTrack)))
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.TimecodesTrackRequired");
                    }
                }

                if (selectedExtractionMode == FormMkvExtractionMode.Cues ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes)
                {
                    // Check if the ckecked nodes contain video, audio or subtitle track
                    if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVTrack)))
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.CuesTrackRequired");
                    }
                }
            }

            if (checkSelectedChapterType)
            {
                if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVChapter)))
                {
                    if (cmbChapterType.SelectedIndex == -1)
                    {
                        throw CreateLocalizedException("UI.MainForm2.Errors.ChapterTypeRequired");
                    }
                }
            }

            if (!chkUseSourceDirectory.Checked && !Directory.Exists(txtOutputDirectory.Text.Trim()))
            {
                // Ask the user to create the non existing output directory
                if (ShowLocalizedQuestion("UI.MainForm2.Dialogs.OutputDirectoryMissingQuestion", "UI.MainForm2.Dialogs.OutputDirectoryMissingTitle", false, txtOutputDirectory.Text.Trim(), Environment.NewLine) != DialogResult.Yes)
                {
                    throw CreateLocalizedException("UI.MainForm2.Errors.OutputDirectoryMissingCancelled", txtOutputDirectory.Text.Trim(), Environment.NewLine);
                }
                else
                {
                    // Create the non existing output directory
                    Directory.CreateDirectory(txtOutputDirectory.Text.Trim());
                }
            }
        }

        private gMKVExtractFilenamePatterns GetFilenamePatterns()
        {
            return new gMKVExtractFilenamePatterns()
            {
                AttachmentFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.AttachmentFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AttachmentFilenamePattern)) : _Settings.AttachmentFilenamePattern
                ,
                AudioTrackFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.AudioTrackFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AudioTrackFilenamePattern)) : _Settings.AudioTrackFilenamePattern
                ,
                ChapterFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.ChapterFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.ChapterFilenamePattern)) : _Settings.ChapterFilenamePattern
                ,
                SubtitleTrackFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.SubtitleTrackFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.SubtitleTrackFilenamePattern)) : _Settings.SubtitleTrackFilenamePattern
                ,
                VideoTrackFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.VideoTrackFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.VideoTrackFilenamePattern)) : _Settings.VideoTrackFilenamePattern
                ,
                TagsFilenamePattern =
                    string.IsNullOrWhiteSpace(_Settings.TagsFilenamePattern) ?
                        _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.TagsFilenamePattern)) : _Settings.TagsFilenamePattern
            };
        }

        private void btnExtract_btnAddJobs_Click(object sender, EventArgs e)
        {
            try
            {
                FormMkvExtractionMode extractionMode = (FormMkvExtractionMode)Enum.Parse(
                    typeof(FormMkvExtractionMode),
                    (string)cmbExtractionMode.SelectedItem);

                // Check for necessary input fields
                switch (extractionMode)
                {
                    case FormMkvExtractionMode.Tracks:
                        CheckNeccessaryInputFields(true, true);
                        break;
                    case FormMkvExtractionMode.Cue_Sheet:
                        CheckNeccessaryInputFields(true, false);
                        break;
                    case FormMkvExtractionMode.Tags:
                        CheckNeccessaryInputFields(true, false);
                        break;
                    case FormMkvExtractionMode.Timecodes:
                        CheckNeccessaryInputFields(true, false);
                        break;
                    case FormMkvExtractionMode.Tracks_And_Timecodes:
                        CheckNeccessaryInputFields(true, true);
                        break;
                    case FormMkvExtractionMode.Cues:
                        CheckNeccessaryInputFields(true, false);
                        break;
                    case FormMkvExtractionMode.Tracks_And_Cues:
                        CheckNeccessaryInputFields(true, false);
                        break;
                    case FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes:
                        CheckNeccessaryInputFields(true, false);
                        break;
                }

                // Get all checked nodes
                List<TreeNode> checkedNodes = trvInputFiles.CheckedNodes;
                // Filter out the parent nodes
                checkedNodes.RemoveAll(t => t.Tag != null && t.Tag is gMKVSegmentInfo);

                // Get all the distinct parent nodes that correspond to the checked nodes
                List<TreeNode> parentNodes = checkedNodes
                    .Where(t => t.Parent != null && t.Parent.Tag != null && t.Parent.Tag is gMKVSegmentInfo)
                    .Select(t => t.Parent)
                    .Distinct()
                    .ToList<TreeNode>();

                List<gMKVJob> jobs = new List<gMKVJob>();
                List<gMKVSegment> segments = null;

                // For each file, we need a separate job
                foreach (TreeNode parentNode in parentNodes)
                {
                    gMKVSegmentInfo infoSegment = parentNode.Tag as gMKVSegmentInfo;
                    segments = checkedNodes.Where(n => n.Parent == parentNode).Select(t => t.Tag as gMKVSegment).ToList();
                    string outputDirectory = txtOutputDirectory.Text;

                    // Check if the output dir is the same as the source
                    if (chkUseSourceDirectory.Checked)
                    {
                        outputDirectory = infoSegment.Directory;
                    }

                    gMKVExtractSegmentsParameters parameterList = new gMKVExtractSegmentsParameters();
                    switch (extractionMode)
                    {
                        case FormMkvExtractionMode.Tracks:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.NoTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.NoCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            parameterList.UseRawExtractionMode = _Settings.UseRawExtractionMode;
                            parameterList.UseFullRawExtractionMode = _Settings.UseFullRawExtractionMode;
                            break;
                        case FormMkvExtractionMode.Cue_Sheet:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            break;
                        case FormMkvExtractionMode.Tags:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            break;
                        case FormMkvExtractionMode.Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.OnlyTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.NoCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            break;
                        case FormMkvExtractionMode.Tracks_And_Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.WithTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.NoCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            parameterList.UseRawExtractionMode = _Settings.UseRawExtractionMode;
                            parameterList.UseFullRawExtractionMode = _Settings.UseFullRawExtractionMode;
                            break;
                        case FormMkvExtractionMode.Cues:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.NoTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.OnlyCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            break;
                        case FormMkvExtractionMode.Tracks_And_Cues:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.NoTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.WithCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            parameterList.UseRawExtractionMode = _Settings.UseRawExtractionMode;
                            parameterList.UseFullRawExtractionMode = _Settings.UseFullRawExtractionMode;
                            break;
                        case FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.WithTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.WithCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            parameterList.OverwriteExistingFile = chkOverwriteExistingFiles.Checked;
                            parameterList.DisableBomForTextFiles = _Settings.DisableBomForTextFiles;
                            parameterList.UseRawExtractionMode = _Settings.UseRawExtractionMode;
                            parameterList.UseFullRawExtractionMode = _Settings.UseFullRawExtractionMode;
                            break;
                    }
                    jobs.Add(new gMKVJob(extractionMode, txtMKVToolnixPath.Text, parameterList));
                }

                if (sender == btnAddJobs)
                {
                    if (_JobManagerForm == null)
                    {
                        _JobManagerForm = new frmJobManager(this);
                    }

                    _JobManagerForm.Show();
                    foreach (var job in jobs)
                    {
                        _JobManagerForm.AddJob(new gMKVJobInfo(job));
                    }
                }
                else
                {
                    StartExtractionJobs(jobs, sender == btnExtract);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());

                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Error);
                gTaskbarProgress.SetOverlayIcon(this, SystemIcons.Error, LocalizationManager.GetString("UI.Common.Dialog.ErrorTitle"));
                ShowErrorMessage(ex.Message);

                if (_ExtractRunning)
                {
                    FinishExtractionJobs(true, sender == btnExtract);
                }
                else
                {
                    gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
                    gTaskbarProgress.SetOverlayIcon(this, null, null);
                }
            }
        }

        private void StartExtractionJobs(List<gMKVJob> jobs, bool showExtractionCompletedText)
        {
            _CurrentJob = 0;
            _TotalJobs = jobs.Count;

            prgBrStatus.Minimum = 0;
            prgBrStatus.Maximum = 100;
            prgBrTotalStatus.Maximum = _TotalJobs * 100;
            prgBrTotalStatus.Visible = true;

            tlpMain.Enabled = false;
            _ExtractRunning = true;
            _gMkvExtract.MkvExtractProgressUpdated += g_MkvExtractProgressUpdated;
            _gMkvExtract.MkvExtractTrackUpdated += g_MkvExtractTrackUpdated;

            btnAbort.Enabled = true;
            btnAbortAll.Enabled = true;
            btnOptions.Enabled = false;
            gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Normal);
            gTaskbarProgress.SetOverlayIcon(this, SystemIcons.Shield, LocalizationManager.GetString("UI.Common.Status.Extracting"));

            StartNextExtractionJob(jobs, 0, showExtractionCompletedText);
        }

        private void StartNextExtractionJob(List<gMKVJob> jobs, int jobIndex, bool showExtractionCompletedText)
        {
            if (jobIndex >= jobs.Count)
            {
                OnExtractionJobsCompleted(null, showExtractionCompletedText);
                return;
            }

            gMKVJob job = jobs[jobIndex];
            _CurrentJob = jobIndex + 1;

            try
            {
                job.StartAsync(_gMkvExtract)
                    .ContinueWith(
                        task => HandleExtractionJobCompleted(jobs, jobIndex, showExtractionCompletedText, task),
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                OnExtractionJobsCompleted(ex, showExtractionCompletedText);
            }
        }

        private void HandleExtractionJobCompleted(List<gMKVJob> jobs, int jobIndex, bool showExtractionCompletedText, Task task)
        {
            Exception extractException = GetExtractionJobException(task);
            if (extractException != null)
            {
                OnExtractionJobsCompleted(extractException, showExtractionCompletedText);
                return;
            }

            UpdateProgress(100);
            StartNextExtractionJob(jobs, jobIndex + 1, showExtractionCompletedText);
        }

        private Exception GetExtractionJobException(Task task)
        {
            if (_gMkvExtract != null && _gMkvExtract.ThreadedException != null)
            {
                return _gMkvExtract.ThreadedException;
            }

            return task.Exception == null ? null : task.Exception.Flatten().InnerException;
        }

        private void OnExtractionJobsCompleted(Exception extractException, bool showExtractionCompletedText)
        {
            if (extractException != null)
            {
                Debug.WriteLine(extractException);
                gMKVLogger.Log(extractException.ToString());
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Error);
                gTaskbarProgress.SetOverlayIcon(this, SystemIcons.Error, LocalizationManager.GetString("UI.Common.Dialog.ErrorTitle"));
                ShowErrorMessage(extractException.Message);
                FinishExtractionJobs(true, showExtractionCompletedText);
                return;
            }

            btnAbort.Enabled = false;
            btnAbortAll.Enabled = false;

            if (chkShowPopup.Checked)
            {
                ShowLocalizedSuccessMessage("UI.MainForm2.Success.ExtractionCompleted", true);
            }
            else
            {
                SystemSounds.Asterisk.Play();
            }

            FinishExtractionJobs(false, showExtractionCompletedText);
        }

        private void FinishExtractionJobs(bool exceptionOccured, bool showExtractionCompletedText)
        {
            if (_gMkvExtract != null)
            {
                _gMkvExtract.MkvExtractProgressUpdated -= g_MkvExtractProgressUpdated;
                _gMkvExtract.MkvExtractTrackUpdated -= g_MkvExtractTrackUpdated;
            }

            trvInputFiles.SelectedNode = null;
            txtSegmentInfo.Clear();
            UpdateSelectedFileInfoTitle();

            if (chkShowPopup.Checked || exceptionOccured)
            {
                ClearStatus();
            }
            else if (showExtractionCompletedText)
            {
                txtSegmentInfo.Text = LocalizationManager.GetString("UI.Common.Status.ExtractionCompleted");
            }

            _ExtractRunning = false;
            tlpMain.Enabled = true;
            btnAbort.Enabled = false;
            btnAbortAll.Enabled = false;
            btnOptions.Enabled = true;
            gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
            gTaskbarProgress.SetOverlayIcon(this, null, null);
            Refresh();
        }

        private void ClearControls()
        {
            // check if output directory is the same as the source
            if (chkUseSourceDirectory.Checked)
            {
                txtOutputDirectory.Clear();
            }

            UpdateInputFilesGroupTitle();
            UpdateSelectedFileInfoTitle();

            txtSegmentInfo.Clear();
            ClearStatus();
        }

        private void ClearStatus()
        {
            lblStatus.Text = "";
            lblTotalStatus.Text = "";
            prgBrStatus.Value = 0;
            prgBrTotalStatus.Value = 0;
            prgBrTotalStatus.Visible = false;
        }

        private void txtMKVToolnixPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor)
                {
                    // check if the folder actually contains MKVToolnix
                    string trimmedPath = txtMKVToolnixPath.Text.Trim();
                    if (!File.Exists(Path.Combine(trimmedPath, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                        && !File.Exists(Path.Combine(trimmedPath, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
                    {
                        _FromConstructor = true;
                        txtMKVToolnixPath.Text = "";
                        _FromConstructor = false;
                        throw CreateLocalizedException("UI.MainForm2.Errors.FolderDoesNotContainMkvToolnix", trimmedPath);
                    }

                    // Write the value to the ini file
                    _Settings.MkvToolnixPath = trimmedPath;
                    gMKVLogger.Log($"Changing MkvToolnixPath to {trimmedPath}");
                    _Settings.Save();
                }

                _gMkvExtract = new gMKVExtract(txtMKVToolnixPath.Text);
            }
            catch (Exception ex)
            {
                // If we are in the constructor, we don't want to show the error message
                // because it will be handled in the constructor
                if (!_FromConstructor)
                {
                    Debug.WriteLine(ex);
                    gMKVLogger.Log(ex.ToString());
                    ShowErrorMessage(ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        private void cmbChapterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor)
                {
                    if (cmbChapterType.SelectedIndex > -1)
                    {
                        // Write the value to the ini file
                        _Settings.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                        gMKVLogger.Log("Changing ChapterType...");
                        _Settings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void txtOutputDirectory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor)
                {
                    _Settings.OutputDirectory = txtOutputDirectory.Text;
                    gMKVLogger.Log("Changing OutputDirectory...");
                    _Settings.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void chkUseSourceDirectory_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtOutputDirectory.ReadOnly = chkUseSourceDirectory.Checked;
                btnBrowseOutputDirectory.Enabled = !chkUseSourceDirectory.Checked;

                if (!_FromConstructor)
                {
                    _Settings.LockedOutputDirectory = chkUseSourceDirectory.Checked;
                    gMKVLogger.Log("Changing LockedOutputDirectory");
                    _Settings.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnBrowseOutputDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                // check if output directory is the same as the source
                if (!chkUseSourceDirectory.Checked)
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        RestoreDirectory = true,
                        CheckFileExists = false,
                        CheckPathExists = false,
                        OverwritePrompt = false,
                        FileName = LocalizationManager.GetString("UI.Common.Dialog.SelectDirectoryPlaceholder"),
                        Title = LocalizationManager.GetString("UI.MainForm2.Dialogs.SelectOutputDirectoryTitle")
                    };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        txtOutputDirectory.Text = Path.GetDirectoryName(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnBrowseMKVToolnixPath_Click(object sender, EventArgs e)
        {
            try
            {
                // check if MKVToolnix Path is already set
                if (!string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
                {
                    if (ShowLocalizedQuestion("UI.MainForm2.Dialogs.ChangeMkvToolnixPathQuestion", "UI.Common.Dialog.AreYouSureTitle", false) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                OpenFileDialog ofd = new OpenFileDialog
                {
                    RestoreDirectory = true,
                    CheckFileExists = false,
                    CheckPathExists = false,
                    FileName = LocalizationManager.GetString("UI.Common.Dialog.SelectDirectoryPlaceholder"),
                    Title = LocalizationManager.GetString("UI.MainForm2.Dialogs.SelectMkvToolnixDirectoryTitle")
                };

                if (!string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
                {
                    if (Directory.Exists(txtMKVToolnixPath.Text.Trim()))
                    {
                        ofd.InitialDirectory = txtMKVToolnixPath.Text.Trim();
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtMKVToolnixPath.Text = Path.GetDirectoryName(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnShowLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (_LogForm == null)
                {
                    _LogForm = new frmLog();
                }

                _LogForm.Show();
                _LogForm.Focus();
                _LogForm.Select();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnShowJobs_Click(object sender, EventArgs e)
        {
            try
            {
                if (_JobManagerForm == null)
                {
                    _JobManagerForm = new frmJobManager(this);
                }

                _JobManagerForm.Show();
                _JobManagerForm.Focus();
                _JobManagerForm.Select();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            try
            {
                _gMkvExtract.Abort = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAbortAll_Click(object sender, EventArgs e)
        {
            try
            {
                _gMkvExtract.AbortAll = true;
                _gMkvExtract.Abort = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        public void SetTableLayoutMainStatus(bool argStatus)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (IsHandleCreated && InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    SetTableLayoutMainStatus(argStatus);
                });
                return;
            }

            tlpMain.Enabled = argStatus;
            tlpMain.Invalidate();
        }

        #region "Form Events"

        private void frmMain_Move(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor &&
                    !(this.WindowState == FormWindowState.Minimized
                    || this.WindowState == FormWindowState.Maximized))
                {
                    _Settings.WindowPosX = this.Location.X;
                    _Settings.WindowPosY = this.Location.Y;
                    _Settings.WindowState = this.WindowState;
                    gMKVLogger.Log("Changing WindowPosX, WindowPosY, WindowState");
                    _Settings.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor)
                {
                    _Settings.WindowSizeWidth = this.Size.Width;
                    _Settings.WindowSizeHeight = this.Size.Height;
                    _Settings.WindowState = this.WindowState;
                    gMKVLogger.Log("Changing WindowSizeWidth, WindowSizeHeight, WindowState");
                    _Settings.Save();
                }

                if (this.WindowState != FormWindowState.Minimized)
                {
                    ApplyResponsiveLayout();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private void frmMain_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_FromConstructor)
                {
                    _Settings.WindowState = this.WindowState;
                    gMKVLogger.Log("Changing WindowState");
                    _Settings.Save();
                }

                if (!_FromConstructor
                    && !_isApplyingResponsiveLayout
                    && this.WindowState != FormWindowState.Minimized)
                {
                    ApplyResizeResponsiveLayout();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private void ApplyResizeResponsiveLayout()
        {
            bool hasFileOptionsLayout = pnlFileOptions != null
                && pnlFileOptions.IsHandleCreated
                && tlpInput != null
                && tlpInput.RowStyles.Count > 1;
            bool hasActionsLayout = grpActions != null && grpActions.IsHandleCreated;

            if (!hasFileOptionsLayout && !hasActionsLayout)
            {
                return;
            }

            tlpMain.SuspendLayout();

            if (hasFileOptionsLayout)
            {
                tlpInput.SuspendLayout();
                pnlFileOptions.SuspendLayout();
            }

            if (hasActionsLayout)
            {
                grpActions.SuspendLayout();
            }

            try
            {
                if (hasFileOptionsLayout)
                {
                    if (_fileOptionsPanelBaseHeight > 0)
                    {
                        pnlFileOptions.Height = _fileOptionsPanelBaseHeight;
                    }

                    if (_fileOptionsRowBaseHeight > 0F)
                    {
                        tlpInput.RowStyles[1].Height = _fileOptionsRowBaseHeight;
                    }

                    LayoutFileOptionsPanel();
                }

                if (hasActionsLayout)
                {
                    LayoutActionsGroup();
                }
            }
            finally
            {
                if (hasActionsLayout)
                {
                    grpActions.ResumeLayout(false);
                    grpActions.PerformLayout();
                }

                if (hasFileOptionsLayout)
                {
                    pnlFileOptions.ResumeLayout(false);
                    pnlFileOptions.PerformLayout();
                    tlpInput.ResumeLayout(true);
                }

                tlpMain.ResumeLayout(true);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_ExtractRunning)
                {
                    e.Cancel = true;
                    ShowLocalizedErrorMessage("UI.Common.Errors.ExtractionRunningBeforeClose");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                e.Cancel = true;
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion

        private void chkShowPopup_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor)
            {
                _Settings.ShowPopup = chkShowPopup.Checked;
                gMKVLogger.Log("Changing ShowPopup");
                _Settings.Save();
            }
        }

        private void chkAppendOnDragAndDrop_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor)
            {
                _Settings.AppendOnDragAndDrop = chkAppendOnDragAndDrop.Checked;
                gMKVLogger.Log("Changing AppendOnDragAndDrop");
                _Settings.Save();
            }
        }

        private void chkOverwriteExistingFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor)
            {
                _Settings.OverwriteExistingFiles = chkOverwriteExistingFiles.Checked;
                gMKVLogger.Log("Changing OverwriteExistingFiles");
                _Settings.Save();
            }
        }

        private void chkDisableTooltips_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor)
            {
                _Settings.DisableTooltips = chkDisableTooltips.Checked;
                SetTooltips(!chkDisableTooltips.Checked);
                gMKVLogger.Log("Changing DisableTooltips");
                _Settings.Save();
            }
        }

        #region "Context Menu"

        private void SetContextMenuText()
        {
            List<TreeNode> allNodes = trvInputFiles.AllNodes.Where(n => n != null && n.Tag != null).ToList();
            List<TreeNode> checkedNodes = trvInputFiles.CheckedNodes.Where(n => n != null && n.Tag != null).ToList();

            int allTracksCount = allNodes.Count(n => (n.Tag is gMKVTrack || n.Tag is gMKVChapter || n.Tag is gMKVAttachment));
            int videoTracksCount = allNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.video);
            int audioTracksCount = allNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.audio);
            int subtitleTracksCount = allNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.subtitles);
            int chapterTracksCount = allNodes.Count(n => n.Tag is gMKVChapter);
            int attachmentTracksCount = allNodes.Count(n => n.Tag is gMKVAttachment);

            int checkedAllTracksCount = checkedNodes.Count(n => (n.Tag is gMKVTrack || n.Tag is gMKVChapter || n.Tag is gMKVAttachment));
            int checkedVideoTracksCount = checkedNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.video);
            int checkedAudioTracksCount = checkedNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.audio);
            int checkedSubtitleTracksCount = checkedNodes.Count(n => n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.subtitles);
            int checkedChapterTracksCount = checkedNodes.Count(n => n.Tag is gMKVChapter);
            int checkedAttachmentTracksCount = checkedNodes.Count(n => n.Tag is gMKVAttachment);

            int allInputFilesCount = allNodes.Count(n => n.Tag is gMKVSegmentInfo);
            string videoTracksLabel = GetContextMenuTrackGroupLabel(TrackSelectionMode.video);
            string audioTracksLabel = GetContextMenuTrackGroupLabel(TrackSelectionMode.audio);
            string subtitleTracksLabel = GetContextMenuTrackGroupLabel(TrackSelectionMode.subtitle);
            string chapterTracksLabel = GetContextMenuTrackGroupLabel(TrackSelectionMode.chapter);
            string attachmentTracksLabel = GetContextMenuTrackGroupLabel(TrackSelectionMode.attachment);

            checkTracksToolStripMenuItem.Enabled = (allTracksCount - checkedAllTracksCount > 0);
            checkVideoTracksToolStripMenuItem.Enabled = (videoTracksCount - checkedVideoTracksCount > 0);
            checkAudioTracksToolStripMenuItem.Enabled = (audioTracksCount - checkedAudioTracksCount > 0);
            checkSubtitleTracksToolStripMenuItem.Enabled = (subtitleTracksCount - checkedSubtitleTracksCount > 0);
            checkChapterTracksToolStripMenuItem.Enabled = (chapterTracksCount - checkedChapterTracksCount > 0);
            checkAttachmentTracksToolStripMenuItem.Enabled = (attachmentTracksCount - checkedAttachmentTracksCount > 0);

            uncheckTracksToolStripMenuItem.Enabled = (checkedAllTracksCount > 0);
            uncheckVideoTracksToolStripMenuItem.Enabled = (checkedVideoTracksCount > 0);
            uncheckAudioTracksToolStripMenuItem.Enabled = (checkedAudioTracksCount > 0);
            uncheckSubtitleTracksToolStripMenuItem.Enabled = (checkedSubtitleTracksCount > 0);
            uncheckChapterTracksToolStripMenuItem.Enabled = (checkedChapterTracksCount > 0);
            uncheckAttachmentTracksToolStripMenuItem.Enabled = (checkedAttachmentTracksCount > 0);

            removeAllInputFilesToolStripMenuItem.Enabled = (allInputFilesCount > 0);
            removeSelectedInputFileToolStripMenuItem.Enabled = (trvInputFiles.SelectedNode != null && trvInputFiles.SelectedNode.Tag != null);
            openSelectedFileFolderToolStripMenuItem.Enabled = (trvInputFiles.SelectedNode != null && trvInputFiles.SelectedNode.Tag != null);
            openSelectedFileToolStripMenuItem.Enabled = (trvInputFiles.SelectedNode != null && trvInputFiles.SelectedNode.Tag != null);

            checkTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckAllTracks", checkedAllTracksCount, allTracksCount);

            checkVideoTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", videoTracksLabel, checkedVideoTracksCount, videoTracksCount);
            checkAudioTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", audioTracksLabel, checkedAudioTracksCount, audioTracksCount);
            checkSubtitleTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", subtitleTracksLabel, checkedSubtitleTracksCount, subtitleTracksCount);
            checkChapterTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", chapterTracksLabel, checkedChapterTracksCount, chapterTracksCount);
            checkAttachmentTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", attachmentTracksLabel, checkedAttachmentTracksCount, attachmentTracksCount);

            allVideoTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", videoTracksLabel, checkedVideoTracksCount, videoTracksCount);
            allAudioTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", audioTracksLabel, checkedAudioTracksCount, audioTracksCount);
            allSubtitleTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", subtitleTracksLabel, checkedSubtitleTracksCount, subtitleTracksCount);
            allChapterTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", chapterTracksLabel, checkedChapterTracksCount, chapterTracksCount);
            allAttachmentTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", attachmentTracksLabel, checkedAttachmentTracksCount, attachmentTracksCount);

            uncheckTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckAllTracks", allTracksCount - checkedAllTracksCount, allTracksCount);

            uncheckVideoTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckTrackGroup", videoTracksLabel, videoTracksCount - checkedVideoTracksCount, videoTracksCount);
            uncheckAudioTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckTrackGroup", audioTracksLabel, audioTracksCount - checkedAudioTracksCount, audioTracksCount);
            uncheckSubtitleTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckTrackGroup", subtitleTracksLabel, subtitleTracksCount - checkedSubtitleTracksCount, subtitleTracksCount);
            uncheckChapterTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckTrackGroup", chapterTracksLabel, chapterTracksCount - checkedChapterTracksCount, chapterTracksCount);
            uncheckAttachmentTracksToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.UncheckTrackGroup", attachmentTracksLabel, attachmentTracksCount - checkedAttachmentTracksCount, attachmentTracksCount);

            allVideoTracksToolStripMenuItem1.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", videoTracksLabel, videoTracksCount - checkedVideoTracksCount, videoTracksCount);
            allAudioTracksToolStripMenuItem1.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", audioTracksLabel, audioTracksCount - checkedAudioTracksCount, audioTracksCount);
            allSubtitleTracksToolStripMenuItem1.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", subtitleTracksLabel, subtitleTracksCount - checkedSubtitleTracksCount, subtitleTracksCount);
            allChapterTracksToolStripMenuItem1.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", chapterTracksLabel, chapterTracksCount - checkedChapterTracksCount, chapterTracksCount);
            allAttachmentTracksToolStripMenuItem1.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AllTrackGroup", attachmentTracksLabel, attachmentTracksCount - checkedAttachmentTracksCount, attachmentTracksCount);

            removeAllInputFilesToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.RemoveAllInputFiles", allInputFilesCount);

            removeSelectedInputFileToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.RemoveSelectedInputFile");

            if (_contextMenuItemsDirty)
            {
                ResetDynamicContextMenuItems(checkVideoTracksToolStripMenuItem, allVideoTracksToolStripMenuItem);
                ResetDynamicContextMenuItems(uncheckVideoTracksToolStripMenuItem, allVideoTracksToolStripMenuItem1);

                ResetDynamicContextMenuItems(checkAudioTracksToolStripMenuItem, allAudioTracksToolStripMenuItem);
                ResetDynamicContextMenuItems(uncheckAudioTracksToolStripMenuItem, allAudioTracksToolStripMenuItem1);

                ResetDynamicContextMenuItems(checkSubtitleTracksToolStripMenuItem, allSubtitleTracksToolStripMenuItem);
                ResetDynamicContextMenuItems(uncheckSubtitleTracksToolStripMenuItem, allSubtitleTracksToolStripMenuItem1);

                List<ToolStripItem> checkItems = null;
                List<ToolStripItem> uncheckItems = null;

                List<TreeNode> allVideoNodes = allNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).ToList();
                List<TreeNode> checkedVideoNodes = checkedNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).ToList();

                // Get all video track languages
                {
                    List<string> videoLanguages = allVideoNodes.Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
                    ToolStripMenuItem tsCheckVideoTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.video), videoLanguages.Count));
                    checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByLanguage);
                    ToolStripMenuItem tsUncheckVideoTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.video), videoLanguages.Count));
                    uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByLanguage);
                    checkItems = new List<ToolStripItem>();
                    uncheckItems = new List<ToolStripItem>();
                    foreach (string lang in videoLanguages)
                    {
                        int totalLanguages = allVideoNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                        int checkedLanguages = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                        var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.video), lang, checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                        checkItems.Add(checkItem);
                        var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.video), lang, totalLanguages - checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                        uncheckItems.Add(uncheckItem);
                    }
                    tsCheckVideoTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
                    tsUncheckVideoTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());
                }

            // Get all video track languages ietf
            {
                List<string> videoLanguagesIetf = allVideoNodes.Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
                ToolStripMenuItem tsCheckVideoTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.video), videoLanguagesIetf.Count));
                checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByLanguageIetf);
                ToolStripMenuItem tsUncheckVideoTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.video), videoLanguagesIetf.Count));
                uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByLanguageIetf);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string langIetf in videoLanguagesIetf)
                {
                    int totalLanguagesIetf = allVideoNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    int checkedLanguagesIetf = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.video), langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.video), langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckVideoTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckVideoTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all video track Codec_id
            {
                List<string> videoCodecs = allVideoNodes.Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
                ToolStripMenuItem tsCheckVideoTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.video), videoCodecs.Count));
                checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByCodec);
                ToolStripMenuItem tsUncheckVideoTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.video), videoCodecs.Count));
                uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByCodec);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string codec in videoCodecs)
                {
                    int totalLanguages = allVideoNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    int checkedLanguages = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.video), codec, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.video), codec, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckVideoTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckVideoTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all video track extra info
            {
                List<string> videoExtra = allVideoNodes.Select(n => (n.Tag as gMKVTrack).ExtraInfo).Distinct().ToList();
                ToolStripMenuItem tsCheckVideoTracksByResolution = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.video), videoExtra.Count));
                checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByResolution);
                ToolStripMenuItem tsUncheckVideoTracksByResolution = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.video), videoExtra.Count));
                uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByResolution);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string extra in videoExtra)
                {
                    int totalLanguages = allVideoNodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                    int checkedLanguages = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.video), extra, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.video), extra, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckVideoTracksByResolution.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckVideoTracksByResolution.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all video track name
            {
                List<string> videoNames = allVideoNodes.Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
                // Only show menu items if the names are less than 50
                if (videoNames.Any() && videoNames.Count < 50)
                {
                    ToolStripMenuItem tsCheckVideoTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.video), videoNames.Count));
                    checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByName);
                    ToolStripMenuItem tsUncheckVideoTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.video), videoNames.Count));
                    uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByName);
                    checkItems = new List<ToolStripItem>();
                    uncheckItems = new List<ToolStripItem>();
                    foreach (string name in videoNames)
                    {
                        int totalLanguages = allVideoNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        int checkedLanguages = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.video), name, checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                        checkItems.Add(checkItem);
                        var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.video), name, totalLanguages - checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                        uncheckItems.Add(uncheckItem);
                    }
                    tsCheckVideoTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                    tsUncheckVideoTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
                }
            }

            // Get all video track Forced
            {
                List<bool> videoForced = allVideoNodes.Select(n => (n.Tag as gMKVTrack).Forced).Distinct().ToList();
                ToolStripMenuItem tsCheckVideoTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.video), videoForced.Count));
                checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByForced);
                ToolStripMenuItem tsUncheckVideoTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", videoTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.video), videoForced.Count));
                uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByForced);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (bool forced in videoForced)
                {
                    int totalForced = allVideoNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    int checkedForced = checkedVideoNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.video), GetLocalizedBoolean(forced), checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.video), GetLocalizedBoolean(forced), totalForced - checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckVideoTracksByForced.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckVideoTracksByForced.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            List<TreeNode> allAudioNodes = allNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).ToList();
            List<TreeNode> checkedAudioNodes = checkedNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).ToList();

            // Get all audio track languages
            {
                List<string> audioLanguages = allAudioNodes.Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
                ToolStripMenuItem tsCheckAudioTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.audio), audioLanguages.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByLanguage);
                ToolStripMenuItem tsUncheckAudioTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.audio), audioLanguages.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByLanguage);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string lang in audioLanguages)
                {
                    int totalLanguages = allAudioNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                    int checkedLanguages = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.audio), lang, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.audio), lang, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all audio track languages ietf
            {
                List<string> audioLanguagesIetf = allAudioNodes.Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
                ToolStripMenuItem tsCheckAudioTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.audio), audioLanguagesIetf.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByLanguageIetf);
                ToolStripMenuItem tsUncheckAudioTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.audio), audioLanguagesIetf.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByLanguageIetf);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string langIetf in audioLanguagesIetf)
                {
                    int totalLanguagesIetf = allAudioNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    int checkedLanguagesIetf = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.audio), langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.audio), langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all audio track Codec_id
            {
                List<string> audioCodecs = allAudioNodes.Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
                ToolStripMenuItem tsCheckAudioTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.audio), audioCodecs.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByCodec);
                ToolStripMenuItem tsUncheckAudioTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.audio), audioCodecs.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByCodec);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string codec in audioCodecs)
                {
                    int totalLanguages = allAudioNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    int checkedLanguages = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.audio), codec, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.audio), codec, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all audio track extra info
            {
                List<string> audioExtraInfo = allAudioNodes.Select(n => (n.Tag as gMKVTrack).ExtraInfo).Distinct().ToList();
                ToolStripMenuItem tsCheckAudioTracksByChannels = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.audio), audioExtraInfo.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByChannels);
                ToolStripMenuItem tsUncheckAudioTracksByChannels = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.audio), audioExtraInfo.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByChannels);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string extra in audioExtraInfo)
                {
                    int totalLanguages = allAudioNodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                    int checkedLanguages = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.audio), extra, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.ExtraInfo, TrackSelectionMode.audio), extra, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByChannels.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByChannels.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all audio track name
            {
                List<string> audioNames = allAudioNodes.Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
                // Only show menu items if the names are less than 50
                if (audioNames.Any() && audioNames.Count < 50)
                {
                    ToolStripMenuItem tsCheckAudioTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.audio), audioNames.Count));
                    checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByName);
                    ToolStripMenuItem tsUncheckAudioTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.audio), audioNames.Count));
                    uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByName);
                    checkItems = new List<ToolStripItem>();
                    uncheckItems = new List<ToolStripItem>();
                    foreach (string name in audioNames)
                    {
                        int totalLanguages = allAudioNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        int checkedLanguages = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.audio), name, checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                        checkItems.Add(checkItem);
                        var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.audio), name, totalLanguages - checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                        uncheckItems.Add(uncheckItem);
                    }
                    tsCheckAudioTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                    tsUncheckAudioTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
                }
            }

            // Get all audio track Forced
            {
                List<bool> audioForced = allAudioNodes.Select(n => (n.Tag as gMKVTrack).Forced).Distinct().ToList();
                ToolStripMenuItem tsCheckAudioTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.audio), audioForced.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByForced);
                ToolStripMenuItem tsUncheckAudioTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", audioTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.audio), audioForced.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByForced);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (bool forced in audioForced)
                {
                    int totalForced = allAudioNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    int checkedForced = checkedAudioNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.audio), GetLocalizedBoolean(forced), checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.audio), GetLocalizedBoolean(forced), totalForced - checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByForced.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByForced.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            List<TreeNode> allSubtitleNodes = allNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).ToList();
            List<TreeNode> checkedSubtitleNodes = checkedNodes.Where(n => n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).ToList();

            // Get all subtitle track languages
            {
                List<string> subLanguages = allSubtitleNodes.Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
                ToolStripMenuItem tsCheckSubtitleTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.subtitle), subLanguages.Count));
                checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByLanguage);
                ToolStripMenuItem tsUncheckSubtitleTracksByLanguage = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.subtitle), subLanguages.Count));
                uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByLanguage);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string lang in subLanguages)
                {
                    int totalLanguages = allSubtitleNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                    int checkedLanguages = checkedSubtitleNodes.Where(n => (n.Tag as gMKVTrack).Language == lang).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.subtitle), lang, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Language, TrackSelectionMode.subtitle), lang, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckSubtitleTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckSubtitleTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all subtitle track languages IETF
            {
                List<string> subLanguagesIetf = allSubtitleNodes.Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
                ToolStripMenuItem tsCheckSubtitleTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.subtitle), subLanguagesIetf.Count));
                checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByLanguageIetf);
                ToolStripMenuItem tsUncheckSubtitleTracksByLanguageIetf = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.subtitle), subLanguagesIetf.Count));
                uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByLanguageIetf);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string langIetf in subLanguagesIetf)
                {
                    int totalLanguagesIetf = allSubtitleNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    int checkedLanguagesIetf = checkedSubtitleNodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.subtitle), langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.LanguageIetf, TrackSelectionMode.subtitle), langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckSubtitleTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckSubtitleTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all subtitle track codec_id
            {
                List<string> subCodecs = allSubtitleNodes.Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
                ToolStripMenuItem tsCheckSubtitleTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.subtitle), subCodecs.Count));
                checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByCodec);
                ToolStripMenuItem tsUncheckSubtitleTracksByCodec = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.subtitle), subCodecs.Count));
                uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByCodec);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string codec in subCodecs)
                {
                    int totalLanguages = allSubtitleNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    int checkedLanguages = checkedSubtitleNodes.Where(n => (n.Tag as gMKVTrack).CodecID == codec).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.subtitle), codec, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.CodecId, TrackSelectionMode.subtitle), codec, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckSubtitleTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckSubtitleTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all subtitle track names
            {
                List<string> subNames = allSubtitleNodes.Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
                // Only show menu items if the names are less than 50
                if (subNames.Any() && subNames.Count < 50)
                {
                    ToolStripMenuItem tsCheckSubtitleTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.subtitle), subNames.Count));
                    checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByName);
                    ToolStripMenuItem tsUncheckSubtitleTracksByName = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.subtitle), subNames.Count));
                    uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByName);
                    checkItems = new List<ToolStripItem>();
                    uncheckItems = new List<ToolStripItem>();
                    foreach (string name in subNames)
                    {
                        int totalLanguages = allSubtitleNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        int checkedLanguages = checkedSubtitleNodes.Where(n => (n.Tag as gMKVTrack).TrackName == name).Count();
                        var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.subtitle), name, checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                        checkItems.Add(checkItem);
                        var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Name, TrackSelectionMode.subtitle), name, totalLanguages - checkedLanguages, totalLanguages), null,
                                delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                            );
                        ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                        uncheckItems.Add(uncheckItem);
                    }
                    tsCheckSubtitleTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                    tsUncheckSubtitleTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
                }
            }

            // Get all subtitle track Forced
            {
                List<bool> subtitleForced = allSubtitleNodes.Select(n => (n.Tag as gMKVTrack).Forced).Distinct().ToList();
                ToolStripMenuItem tsCheckSubtitlesTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.subtitle), subtitleForced.Count));
                checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitlesTracksByForced);
                ToolStripMenuItem tsUncheckSubtitlesTracksByForced = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.TrackGroupByFilter", subtitleTracksLabel, GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.subtitle), subtitleForced.Count));
                uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitlesTracksByForced);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (bool forced in subtitleForced)
                {
                    int totalForced = allSubtitleNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    int checkedForced = checkedSubtitleNodes.Where(n => (n.Tag as gMKVTrack).Forced == forced).Count();
                    var checkItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.subtitle), GetLocalizedBoolean(forced), checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(LocalizationManager.GetString("UI.MainForm2.ContextMenu.FilterValueCount", GetContextMenuFilterLabel(NodeSelectionFilter.Forced, TrackSelectionMode.subtitle), GetLocalizedBoolean(forced), totalForced - checkedForced, totalForced), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.Forced, argFilter: forced.ToString()); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckSubtitlesTracksByForced.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckSubtitlesTracksByForced.DropDownItems.AddRange(uncheckItems.ToArray());
            }

                _contextMenuItemsDirty = false;
            }
        }

        private enum NodeSelectionFilter
        {
            Language,
            LanguageIetf,
            ExtraInfo,
            CodecId,
            Name,
            Forced,
        }

        private void SetCheckedTracks(TrackSelectionMode argSelectionMode, bool argCheck,
            NodeSelectionFilter? nodeSelectionFilter = null, string argFilter = null)
        {
            List<TreeNode> nodes = null;
            switch (argSelectionMode)
            {
                case TrackSelectionMode.video:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && n.Tag is gMKVTrack track
                        && track.TrackType == MkvTrackType.video).ToList();
                    if (argFilter != null)
                    {
                        switch (nodeSelectionFilter)
                        {
                            case NodeSelectionFilter.Language:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Language == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.LanguageIetf:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.ExtraInfo:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.CodecId:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).CodecID == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Name:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).TrackName == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Forced:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Forced == bool.Parse(argFilter)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case TrackSelectionMode.audio:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && n.Tag is gMKVTrack track
                        && track.TrackType == MkvTrackType.audio).ToList();
                    if (argFilter != null)
                    {
                        switch (nodeSelectionFilter)
                        {
                            case NodeSelectionFilter.Language:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Language == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.LanguageIetf:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.ExtraInfo:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.CodecId:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).CodecID == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Name:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).TrackName == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Forced:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Forced == bool.Parse(argFilter)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case TrackSelectionMode.subtitle:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && n.Tag is gMKVTrack track
                        && track.TrackType == MkvTrackType.subtitles).ToList();
                    if (argFilter != null)
                    {
                        switch (nodeSelectionFilter)
                        {
                            case NodeSelectionFilter.Language:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Language == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.LanguageIetf:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).LanguageIetf == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.ExtraInfo:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).ExtraInfo == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.CodecId:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).CodecID == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Name:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).TrackName == argFilter).ToList();
                                break;
                            case NodeSelectionFilter.Forced:
                                nodes = nodes.Where(n => (n.Tag as gMKVTrack).Forced == bool.Parse(argFilter)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case TrackSelectionMode.chapter:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && n.Tag is gMKVChapter).ToList();
                    break;
                case TrackSelectionMode.attachment:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && n.Tag is gMKVAttachment).ToList();
                    break;
                case TrackSelectionMode.all:
                    nodes = trvInputFiles.AllNodes.Where(n =>
                        n != null
                        && n.Tag != null
                        && !(n.Tag is gMKVSegmentInfo)).ToList();
                    break;
                default:

                    break;
            }

            nodes.ForEach(n => n.Checked = argCheck);
            MarkContextMenuDirty();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            SetContextMenuText();
            ThemeManager.ApplyContextMenuTheme(contextMenuStrip, _Settings.DarkMode);
        }

        private void checkTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.all, true);
        }

        private void allVideoTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.video, true);
        }

        private void allAudioTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.audio, true);
        }

        private void allSubtitleTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.subtitle, true);
        }

        private void allChapterTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.chapter, true);
        }

        private void allAttachmentTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.attachment, true);
        }

        private void uncheckTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.all, false);
        }

        private void allVideoTracksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.video, false);
        }

        private void allAudioTracksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.audio, false);
        }

        private void allSubtitleTracksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.subtitle, false);
        }

        private void allChapterTracksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.chapter, false);
        }

        private void allAttachmentTracksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetCheckedTracks(TrackSelectionMode.attachment, false);
        }

        private void removeAllInputFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trvInputFiles.Nodes.Clear();
            MarkContextMenuDirty();
            ClearControls();
        }

        private void removeSelectedInputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInputFiles.SelectedNode == null || trvInputFiles.SelectedNode.Tag == null)
            {
                return;
            }

            TreeNode node = trvInputFiles.SelectedNode;
            if (!(node.Tag is gMKVSegmentInfo))
            {
                node = node.Parent;
            }

            trvInputFiles.Nodes.Remove(node);
            MarkContextMenuDirty();
            if (trvInputFiles.Nodes.Count > 0)
            {
                UpdateInputFilesGroupTitle();
            }
            else
            {
                ClearControls();
            }
        }

        private void openSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (trvInputFiles.SelectedNode == null || trvInputFiles.SelectedNode.Tag == null)
                {
                    return;
                }

                TreeNode node = trvInputFiles.SelectedNode;
                if (!(node.Tag is gMKVSegmentInfo))
                {
                    node = node.Parent;
                }

                gMKVSegmentInfo segInfo = node.Tag as gMKVSegmentInfo;
                if (File.Exists(segInfo.Path))
                {
                    Process.Start(segInfo.Path);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void openSelectedFileFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (trvInputFiles.SelectedNode == null || trvInputFiles.SelectedNode.Tag == null)
                {
                    return;
                }

                TreeNode node = trvInputFiles.SelectedNode;
                if (!(node.Tag is gMKVSegmentInfo))
                {
                    node = node.Parent;
                }

                gMKVSegmentInfo segInfo = node.Tag as gMKVSegmentInfo;
                if (Directory.Exists(segInfo.Directory))
                {
                    if (File.Exists(segInfo.Path))
                    {
                        Process.Start("explorer.exe", string.Format("/select, \"{0}\"", segInfo.Path));
                    }
                    else
                    {
                        Process.Start("explorer.exe", segInfo.Directory);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void addInputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = LocalizationManager.GetString("UI.MainForm2.Dialogs.SelectInputMatroskaFileTitle"),
                    Filter = LocalizationManager.GetString("UI.MainForm2.Dialogs.SelectInputMatroskaFileFilter"),
                    Multiselect = true,
                    AutoUpgradeEnabled = true
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AddFileNodes(txtMKVToolnixPath.Text, new List<string>(ofd.FileNames), true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trvInputFiles.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trvInputFiles.CollapseAll();
        }

        #endregion

        private void contextMenuStripOutputDirectory_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                setAsDefaultDirectoryToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.SetAsDefault");

                // First check if we have a valid directory in the text box
                if ((!string.IsNullOrWhiteSpace(txtOutputDirectory.Text) && Directory.Exists(txtOutputDirectory.Text))
                    && (!string.IsNullOrWhiteSpace(_Settings.DefaultOutputDirectory) && Directory.Exists(_Settings.DefaultOutputDirectory))
                    && !txtOutputDirectory.Text.Trim().ToLower().Equals(_Settings.DefaultOutputDirectory.Trim().ToLower()))
                {
                    setAsDefaultDirectoryToolStripMenuItem.Enabled = true;
                }
                else
                {
                    setAsDefaultDirectoryToolStripMenuItem.Enabled = false;
                }

                // Check if we have a default directory in the settings
                if (!string.IsNullOrWhiteSpace(_Settings.DefaultOutputDirectory) && Directory.Exists(_Settings.DefaultOutputDirectory))
                {
                    // Check if we can use the default directory
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Enabled = !chkUseSourceDirectory.Checked;
                    // Set the text
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.UseDefaultWithValue", _Settings.DefaultOutputDirectory);
                }
                else
                {
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Enabled = false;
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.UseDefaultWithValue", LocalizationManager.GetString("UI.Common.NotSet"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void setAsDefaultDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Sanity check!
                if (!string.IsNullOrWhiteSpace(txtOutputDirectory.Text) && Directory.Exists(txtOutputDirectory.Text))
                {
                    // Check if we already have a default output directory
                    if (!string.IsNullOrWhiteSpace(_Settings.DefaultOutputDirectory) && Directory.Exists(_Settings.DefaultOutputDirectory))
                    {
                        if (ShowLocalizedQuestion("UI.MainForm2.Dialogs.ChangeDefaultOutputDirectoryQuestion", "UI.Common.Dialog.AreYouSureTitle", false,
                            _Settings.DefaultOutputDirectory) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    _Settings.DefaultOutputDirectory = txtOutputDirectory.Text.Trim();
                    gMKVLogger.Log("Changing Default Output Directory...");
                    _Settings.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void useCurrentlySetDefaultDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Sanity check!
                if (!chkUseSourceDirectory.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(_Settings.DefaultOutputDirectory) && Directory.Exists(_Settings.DefaultOutputDirectory))
                    {
                        txtOutputDirectory.Text = _Settings.DefaultOutputDirectory;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            try
            {
                using (frmOptions optionsForm = new frmOptions())
                {
                    if (optionsForm.ShowDialog(this) == DialogResult.OK)
                    {
                        _Settings.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor) // To prevent triggering during initial load
            {
                _Settings.DarkMode = chkDarkMode.Checked;
                _Settings.Save();
                ThemeManager.ApplyTheme(this, _Settings.DarkMode); // Apply theme on toggle

                // Hack for DarkMode checkbox
                if (_Settings.DarkMode)
                {
                    chkDarkMode.BackColor = Color.FromArgb(55, 55, 55);
                }

                NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
                NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);

                // Apply theme to context menu
                if (contextMenuStrip != null)
                {
                    ThemeManager.ApplyContextMenuTheme(contextMenuStrip, _Settings.DarkMode);
                }

                // New code to add: Iterate through open forms and update theme
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is frmLog logForm && openForm != this)
                    {
                        logForm.UpdateTheme(_Settings.DarkMode);
                    }
                    else if (openForm is frmJobManager jobManagerForm && openForm != this)
                    {
                        jobManagerForm.UpdateTheme(_Settings.DarkMode);
                    }
                    // Add other forms here if they also need dynamic theming
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                contextMenuStrip.Show(btnSelect, new Point(0, btnSelect.Height));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void trvInputFiles_AfterCheck(object sender, TreeViewEventArgs e)
        {
            MarkContextMenuDirty();
        }

        public void ApplyLocalization()
        {
            Text = string.Format("{0} v{1} -- By Gpower2", LocalizationManager.GetString("UI.MainForm2.Title"), GetCurrentVersion());
            UpdateInputFilesGroupTitle();
            UpdateSelectedFileInfoTitle();
            grpOutputDirectory.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.Group");
            chkUseSourceDirectory.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.UseSource");
            btnBrowseOutputDirectory.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.Browse");
            grpActions.Text = LocalizationManager.GetString("UI.MainForm2.Actions.Group");
            btnAddJobs.Text = LocalizationManager.GetString("UI.MainForm2.Actions.AddJobs");
            btnExtract.Text = LocalizationManager.GetString("UI.MainForm2.Actions.Extract");
            btnShowJobs.Text = LocalizationManager.GetString("UI.MainForm2.Actions.ShowJobs");
            btnShowLog.Text = LocalizationManager.GetString("UI.MainForm2.Actions.Log");
            btnAbort.Text = LocalizationManager.GetString("UI.MainForm2.Actions.Abort");
            btnAbortAll.Text = LocalizationManager.GetString("UI.MainForm2.Actions.AbortAll");
            chkShowPopup.Text = LocalizationManager.GetString("UI.MainForm2.Actions.Popup");
            lblExtractionMode.Text = LocalizationManager.GetString("UI.MainForm2.Actions.ExtractionMode");
            lblChapterType.Text = LocalizationManager.GetString("UI.MainForm2.Actions.ChapterType");
            grpConfig.Text = LocalizationManager.GetString("UI.MainForm2.Config.Group");
            btnAutoDetectMkvToolnix.Text = LocalizationManager.GetString("UI.MainForm2.Config.AutoDetect");
            btnBrowseMKVToolnixPath.Text = LocalizationManager.GetString("UI.MainForm2.Config.Browse");
            chkAppendOnDragAndDrop.Text = LocalizationManager.GetString("UI.MainForm2.FileOptions.AppendOnDragAndDrop");
            chkOverwriteExistingFiles.Text = LocalizationManager.GetString("UI.MainForm2.FileOptions.OverwriteExistingFiles");
            chkDisableTooltips.Text = LocalizationManager.GetString("UI.MainForm2.FileOptions.DisableTooltips");
            btnSelect.Text = LocalizationManager.GetString("UI.MainForm2.FileOptions.Select");
            chkDarkMode.Text = LocalizationManager.GetString("UI.MainForm2.Appearance.Dark");
            btnOptions.Text = LocalizationManager.GetString("UI.MainForm2.Appearance.Options");
            addInputFileToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.AddInputFiles");
            openSelectedFileToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.OpenSelectedFile");
            openSelectedFileFolderToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.OpenSelectedFileFolder");
            expandAllToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.ExpandAll");
            collapseAllToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CollapseAll");
            setAsDefaultDirectoryToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.SetAsDefault");
            useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.UseDefaultWithValue", LocalizationManager.GetString("UI.Common.NotSet"));
            ThemeManager.ApplyContextMenuTheme(contextMenuStrip, _Settings.DarkMode);
            RefreshLocalizedTooltipsAsync();
            MarkContextMenuDirty();
            ApplyResponsiveLayout();
        }

        private void CaptureResponsiveLayoutBaselines()
        {
            CaptureResponsiveButtonBaseSize(btnBrowseMKVToolnixPath);
            CaptureResponsiveButtonBaseSize(btnAutoDetectMkvToolnix);
            CaptureResponsiveButtonBaseSize(btnBrowseOutputDirectory);
            CaptureResponsiveButtonBaseSize(btnSelect);
            CaptureResponsiveButtonBaseSize(btnShowLog);
            CaptureResponsiveButtonBaseSize(btnShowJobs);
            CaptureResponsiveButtonBaseSize(btnAddJobs);
            CaptureResponsiveButtonBaseSize(btnExtract);
            CaptureResponsiveButtonBaseSize(btnOptions);
            CaptureResponsiveButtonBaseSize(btnAbortAll);
            CaptureResponsiveButtonBaseSize(btnAbort);

            if (tlpMain.RowStyles.Count > 4 && tlpMain.RowStyles[4].Height > 0F)
            {
                _actionsRowBaseHeight = Math.Max(_actionsRowBaseHeight, tlpMain.RowStyles[4].Height);
            }

            if (cmbChapterType != null && cmbChapterType.Width > 0)
            {
                _chapterTypeComboBaseWidth = cmbChapterType.Width;
            }

            if (cmbExtractionMode != null && cmbExtractionMode.Width > 0)
            {
                _extractionModeComboBaseWidth = cmbExtractionMode.Width;
            }

            if (pnlFileOptions != null && pnlFileOptions.Height > 0)
            {
                _fileOptionsPanelBaseHeight = Math.Max(_fileOptionsPanelBaseHeight, pnlFileOptions.Height);
            }

            if (tlpInput.RowStyles.Count > 1 && tlpInput.RowStyles[1].Height > 0F)
            {
                _fileOptionsRowBaseHeight = Math.Max(_fileOptionsRowBaseHeight, tlpInput.RowStyles[1].Height);
            }
        }

        private void CaptureResponsiveButtonBaseSize(Button button)
        {
            if (button == null)
            {
                return;
            }

            Size currentSize = button.Size;
            if (!_responsiveButtonBaseSizes.TryGetValue(button, out Size baseSize)
                || currentSize.Width > baseSize.Width
                || currentSize.Height > baseSize.Height)
            {
                _responsiveButtonBaseSizes[button] = currentSize;
            }
        }

        private Size GetResponsiveButtonBaseSize(Button button, int fallbackWidth, int fallbackHeight = 30)
        {
            return _responsiveButtonBaseSizes.TryGetValue(button, out Size baseSize)
                ? baseSize
                : new Size(fallbackWidth, fallbackHeight);
        }

        private void ResetResponsiveLayoutBaselines()
        {
            if (pnlFileOptions != null && _fileOptionsPanelBaseHeight > 0)
            {
                pnlFileOptions.Height = _fileOptionsPanelBaseHeight;
            }

            if (tlpInput.RowStyles.Count > 1 && _fileOptionsRowBaseHeight > 0F)
            {
                tlpInput.RowStyles[1].Height = _fileOptionsRowBaseHeight;
            }

            if (_chapterTypeComboBaseWidth > 0)
            {
                cmbChapterType.Width = _chapterTypeComboBaseWidth;
            }

            if (_extractionModeComboBaseWidth > 0)
            {
                cmbExtractionMode.Width = _extractionModeComboBaseWidth;
            }

            if (tlpMain.RowStyles.Count > 4 && _actionsRowBaseHeight > 0F)
            {
                tlpMain.RowStyles[4].Height = _actionsRowBaseHeight;
            }
        }

        private void ApplyResponsiveLayout()
        {
            if (_isApplyingResponsiveLayout)
            {
                return;
            }

            _isApplyingResponsiveLayout = true;

            try
            {
                SuspendLayout();
                tlpMain.SuspendLayout();
                grpConfig.SuspendLayout();
                grpOutputDirectory.SuspendLayout();
                grpActions.SuspendLayout();
                pnlFileOptions.SuspendLayout();
                ResetResponsiveLayoutBaselines();

                LayoutConfigGroup();
                LayoutOutputDirectoryGroup();
                LayoutFileOptionsPanel();
                LayoutActionsGroup();
                LayoutFooterControls();
            }
            finally
            {
                pnlFileOptions.ResumeLayout(false);
                pnlFileOptions.PerformLayout();
                grpActions.ResumeLayout(false);
                grpActions.PerformLayout();
                grpOutputDirectory.ResumeLayout(false);
                grpOutputDirectory.PerformLayout();
                grpConfig.ResumeLayout(false);
                grpConfig.PerformLayout();
                tlpMain.ResumeLayout(true);
                ResumeLayout(true);
                _isApplyingResponsiveLayout = false;
            }
        }

        private void LayoutConfigGroup()
        {
            btnBrowseMKVToolnixPath.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnBrowseMKVToolnixPath, 70));
            btnAutoDetectMkvToolnix.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnAutoDetectMkvToolnix, 80));

            const int buttonTop = 18;
            int right = grpConfig.ClientSize.Width - 7;

            btnAutoDetectMkvToolnix.Location = new Point(right - btnAutoDetectMkvToolnix.Width, buttonTop);
            btnBrowseMKVToolnixPath.Location = new Point(btnAutoDetectMkvToolnix.Left - MainButtonSpacing - btnBrowseMKVToolnixPath.Width, buttonTop);
            txtMKVToolnixPath.Width = Math.Max(150, btnBrowseMKVToolnixPath.Left - 12 - txtMKVToolnixPath.Left);
        }

        private void LayoutOutputDirectoryGroup()
        {
            btnBrowseOutputDirectory.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnBrowseOutputDirectory, 80));

            int right = grpOutputDirectory.ClientSize.Width - 7;
            btnBrowseOutputDirectory.Location = new Point(right - btnBrowseOutputDirectory.Width, 18);
            chkUseSourceDirectory.Location = new Point(btnBrowseOutputDirectory.Left - 12 - chkUseSourceDirectory.Width, 24);
            txtOutputDirectory.Width = Math.Max(120, chkUseSourceDirectory.Left - 12 - txtOutputDirectory.Left);
        }

        private void LayoutFileOptionsPanel()
        {
            btnSelect.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnSelect, 80));
            btnSelect.Location = new Point(pnlFileOptions.ClientSize.Width - btnSelect.Width - 3, 1);

            int maxRight = btnSelect.Left - 12;
            int bottom = LayoutWrappingControls(3, 6, maxRight, 6, 4, chkAppendOnDragAndDrop, chkOverwriteExistingFiles, chkDisableTooltips);
            int requiredHeight = Math.Max(btnSelect.Height + 2, bottom + 6);

            pnlFileOptions.Height = requiredHeight;
            btnSelect.Top = Math.Max(0, (requiredHeight - btnSelect.Height) / 2);

            if (tlpInput.RowStyles.Count > 1)
            {
                tlpInput.RowStyles[1].Height = requiredHeight;
            }
        }

        private void LayoutActionsGroup()
        {
            if (grpActions == null || grpActions.ClientSize.Width <= 0)
            {
                return;
            }

            btnShowLog.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnShowLog, 60));
            btnShowJobs.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnShowJobs, 60));
            btnAddJobs.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnAddJobs, 70));
            btnExtract.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnExtract, 80));

            cmbChapterType.Width = _chapterTypeComboBaseWidth > 0 ? _chapterTypeComboBaseWidth : 80;
            cmbExtractionMode.Width = _extractionModeComboBaseWidth > 0 ? _extractionModeComboBaseWidth : 120;

            Size showPopupSize = chkShowPopup.GetPreferredSize(Size.Empty);
            chkShowPopup.Size = showPopupSize;

            int leftSectionBottom = PositionActionsLeftSection(MainActionTopRowButtonTop, showPopupSize);
            int singleRowRightSectionBottom = PositionActionsRightSection(MainActionTopRowButtonTop, out int singleRowRightSectionLeft);

            bool fitsSingleRow = chkShowPopup.Right + MainActionSingleRowSpacing <= singleRowRightSectionLeft;
            int requiredContentBottom;

            if (fitsSingleRow)
            {
                requiredContentBottom = Math.Max(leftSectionBottom, singleRowRightSectionBottom);
            }
            else
            {
                int twoRowRightSectionBottom = PositionActionsRightSection(MainActionBottomRowButtonTop, out _);
                requiredContentBottom = Math.Max(leftSectionBottom, twoRowRightSectionBottom);
            }

            if (tlpMain.RowStyles.Count > 4)
            {
                float minimumHeight = fitsSingleRow
                    ? (_actionsRowBaseHeight > 0F ? _actionsRowBaseHeight : 60F)
                    : Math.Max((float)MainActionRowHeight, _actionsRowBaseHeight > 0F ? _actionsRowBaseHeight : 60F);
                tlpMain.RowStyles[4].Height = GetRequiredActionsRowHeight(requiredContentBottom, minimumHeight);
            }
        }

        private int PositionActionsLeftSection(int buttonTop, Size showPopupSize)
        {
            btnShowLog.Location = new Point(MainActionLeftMargin, buttonTop);
            btnShowJobs.Location = new Point(btnShowLog.Right + MainButtonSpacing, buttonTop);
            chkShowPopup.Location = new Point(btnShowJobs.Right + MainActionSingleRowSpacing, buttonTop + 6);
            chkShowPopup.Size = showPopupSize;

            return new[] { btnShowLog.Bottom, btnShowJobs.Bottom, chkShowPopup.Bottom }.Max();
        }

        private int PositionActionsRightSection(int buttonTop, out int leftmostControlLeft)
        {
            int right = grpActions.ClientSize.Width - MainActionRightMargin;

            btnExtract.Location = new Point(right - btnExtract.Width, buttonTop);
            right = btnExtract.Left - MainButtonSpacing;

            btnAddJobs.Location = new Point(right - btnAddJobs.Width, buttonTop);
            right = btnAddJobs.Left - 12;

            cmbExtractionMode.Location = new Point(right - cmbExtractionMode.Width, buttonTop + MainActionComboTopOffset);
            right = cmbExtractionMode.Left - MainButtonSpacing;

            lblExtractionMode.Location = new Point(right - lblExtractionMode.Width, buttonTop + MainActionLabelTopOffset);
            right = lblExtractionMode.Left - 16;

            cmbChapterType.Location = new Point(right - cmbChapterType.Width, buttonTop + MainActionComboTopOffset);
            right = cmbChapterType.Left - MainButtonSpacing;

            lblChapterType.Location = new Point(right - lblChapterType.Width, buttonTop + MainActionLabelTopOffset);
            leftmostControlLeft = lblChapterType.Left;

            return new[]
            {
                btnExtract.Bottom,
                btnAddJobs.Bottom,
                cmbExtractionMode.Bottom,
                lblExtractionMode.Bottom,
                cmbChapterType.Bottom,
                lblChapterType.Bottom
            }.Max();
        }

        private float GetRequiredActionsRowHeight(int requiredContentBottom, float minimumHeight)
        {
            int marginVertical = grpActions != null ? grpActions.Margin.Vertical : 0;
            int requiredHeight = requiredContentBottom + MainActionBottomPadding + marginVertical;
            return Math.Max(minimumHeight, requiredHeight);
        }

        private void LayoutFooterControls()
        {
            btnOptions.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnOptions, 80));
            btnAbortAll.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnAbortAll, 70));
            btnAbort.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnAbort, 72));

            int top = statusStrip.Top + 3;
            int right = ClientSize.Width - 8;

            btnAbort.Location = new Point(right - btnAbort.Width, top);
            right = btnAbort.Left - MainButtonSpacing;
            btnAbortAll.Location = new Point(right - btnAbortAll.Width, top);
            right = btnAbortAll.Left - MainButtonSpacing;
            btnOptions.Location = new Point(right - btnOptions.Width, top);
            right = btnOptions.Left - 10;
            chkDarkMode.Location = new Point(right - chkDarkMode.Width, top + 6);
        }

        private int LayoutWrappingControls(int startX, int startY, int maxRight, int horizontalSpacing, int verticalSpacing, params Control[] controls)
        {
            int x = startX;
            int y = startY;
            int rowHeight = 0;
            int bottom = startY;

            foreach (Control control in controls)
            {
                int controlWidth = control.GetPreferredWidth();
                int controlHeight = control.Height;

                if (x > startX && x + controlWidth > maxRight)
                {
                    x = startX;
                    y += rowHeight + verticalSpacing;
                    rowHeight = 0;
                }

                control.Location = new Point(x, y);
                x += controlWidth + horizontalSpacing;
                rowHeight = Math.Max(rowHeight, controlHeight);
                bottom = Math.Max(bottom, y + controlHeight);
            }

            return bottom;
        }
    }
}
