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
using gMKVToolNix.Jobs;
using gMKVToolNix.Log;
using gMKVToolNix.MkvExtract;
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
        private frmLog _LogForm = null;
        private frmJobManager _JobManagerForm = null;

        private gMKVExtract _gMkvExtract = null;

        private readonly gSettings _Settings = null;

        private bool _FromConstructor = false;

        private bool _ExtractRunning = false;

        private int _CurrentJob = 0;
        private int _TotalJobs = 0;

        private List<string> _CmdArguments = new List<string>();

        public frmMain2()
        {
            try
            {
                _FromConstructor = true;

                InitializeComponent();

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
                        gMKVLogger.Log($"Checking in ini path for mkvmerge... ({_Settings.MkvToolnixPath})");

                        if (File.Exists(Path.Combine(_Settings.MkvToolnixPath, gMKVHelper.MKV_MERGE_GUI_FILENAME))
                            || File.Exists(Path.Combine(_Settings.MkvToolnixPath, gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
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
                            exceptionMessage = "Could not find MKVToolNix in /usr/bin, or in the current directory, or in the ini file!";
                        }
                        else
                        {
                            exceptionMessage = "Could not find MKVToolNix in registry, or in the current directory, or in the ini file!";
                        }
                        gMKVLogger.Log(exceptionMessage);
                        throw new Exception(exceptionMessage + Environment.NewLine + "Please download and reinstall or provide a manual path!");
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
                        throw new Exception($"mkvmerge was not found in path {linuxDefaultPath}!");
                    }
                }
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
                    tlpMain.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    txtSegmentInfo.Text = "Getting files...";

                    // Get the file list
                    List<string> fileList = GetFilesFromInputFileDrop(_CmdArguments.Where(c => !c.StartsWith("--")).ToArray());

                    // Check if any valid matroska files were provided
                    if (!fileList.Any())
                    {
                        throw new Exception("No valid matroska files were provided!");
                    }

                    // Add files to the TreeView
                    AddFileNodes(txtMKVToolnixPath.Text, fileList);

                    Cursor = Cursors.Default;
                    tlpMain.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                Cursor = Cursors.Default;
                ShowErrorMessage(ex.Message);
                tlpMain.Enabled = true;
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
                            if (ShowQuestion("Do you really want to change MKVToolnix path?", "Are you sure?", false) != DialogResult.Yes)
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

        private List<string> GetFilesFromInputFileDrop(string[] argFileDrop)
        {
            List<string> fileList = new List<string>();

            // Check if directories were provided
            bool directoryExists = false;
            using (Task ta = Task.Factory.StartNew(() =>
            {
                directoryExists = argFileDrop.Any(f => Directory.Exists(f));
            }))
            {
                while (!ta.IsCompleted) { Application.DoEvents(); }
                if (ta.Exception != null) { throw ta.Exception; }
            }

            if (directoryExists)
            {
                // Check if they contain subdirectories
                List<string> subDirList = new List<string>();

                using (Task ta = Task.Factory.StartNew(() =>
                {
                    argFileDrop.Where(f => Directory.Exists(f))
                    .ToList()
                    .ForEach(t => subDirList.AddRange(Directory.GetDirectories(t, "*", SearchOption.TopDirectoryOnly).ToList()));
                }))
                {
                    while (!ta.IsCompleted) { Application.DoEvents(); }
                    if (ta.Exception != null) { throw ta.Exception; }
                }

                if (subDirList.Any())
                {
                    Cursor = Cursors.Default;
                    var result = ShowQuestion("Do you want to include files in sub directories?", "Sub directories found!");
                    Cursor = Cursors.WaitCursor;
                    if (result == DialogResult.Yes)
                    {
                        using (Task ta = Task.Factory.StartNew(() =>
                        {
                            // Add the subdirectory files
                            argFileDrop.Where(f => Directory.Exists(f))
                            .ToList()
                            .ForEach(t => fileList.AddRange(Directory.GetFiles(t, "*", SearchOption.AllDirectories).ToList()));
                        }))
                        {
                            while (!ta.IsCompleted) { Application.DoEvents(); }
                            if (ta.Exception != null) { throw ta.Exception; }
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        using (Task ta = Task.Factory.StartNew(() =>
                        {
                            // Add the top level directory files
                            argFileDrop.Where(f => Directory.Exists(f))
                            .ToList()
                            .ForEach(t => fileList.AddRange(Directory.GetFiles(t, "*", SearchOption.TopDirectoryOnly).ToList()));
                        }))
                        {
                            while (!ta.IsCompleted) { Application.DoEvents(); }
                            if (ta.Exception != null) { throw ta.Exception; }
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        Cursor = Cursors.Default;
                        return new List<string>();
                    }
                }
                else
                {
                    using (Task ta = Task.Factory.StartNew(() =>
                    {
                        // Since there are no subdirectories, add the files from the directory
                        argFileDrop.Where(f => Directory.Exists(f))
                        .ToList()
                        .ForEach(t => fileList.AddRange(Directory.GetFiles(t, "*", SearchOption.TopDirectoryOnly).ToList()));
                    }))
                    {
                        while (!ta.IsCompleted) { Application.DoEvents(); }
                        if (ta.Exception != null) { throw ta.Exception; }
                    }
                }
            }

            using (Task ta = Task.Factory.StartNew(() =>
            {
                // Add the files provided
                argFileDrop.Where(f => File.Exists(f))
                .ToList()
                .ForEach(t => fileList.Add(t));
            }))
            {
                while (!ta.IsCompleted) { Application.DoEvents(); }
                if (ta.Exception != null) { throw ta.Exception; }
            }

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
                        tlpMain.Enabled = false;
                        Cursor = Cursors.WaitCursor;
                        txtSegmentInfo.Text = "Getting files...";

                        // Get the file list
                        List<string> fileList = GetFilesFromInputFileDrop(s);

                        // Check if any valid matroska files were provided
                        if (!fileList.Any())
                        {
                            throw new Exception("No valid matroska files were provided!");
                        }

                        // Add files to the TreeView
                        AddFileNodes(txtMKVToolnixPath.Text, fileList, chkAppendOnDragAndDrop.Checked);

                        Cursor = Cursors.Default;
                        tlpMain.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                Cursor = Cursors.Default;
                ShowErrorMessage(ex.Message);
                tlpMain.Enabled = true;
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

        private void AddFileNodes(string argMKVToolNixPath, List<string> argFiles, bool argAppend = false)
        {
            try
            {
                tlpMain.Enabled = false;
                Application.DoEvents();
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
                        throw new Exception("No new files to add!");
                    }
                }

                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Indeterminate);

                NodeResults results = null;

                Task ta = Task.Factory.StartNew(() =>
                {
                    results = GetFileInfoNodes(argMKVToolNixPath, argFiles);
                });

                while (!ta.IsCompleted)
                {
                    Application.DoEvents();
                }

                if (ta.Exception != null)
                {
                    gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Error);
                    throw ta.Exception;
                }

                // Add the nodes to the TreeView
                trvInputFiles.Nodes.AddRange(results.Nodes.ToArray());
                // Remove the check box from the nodes that contain the gMKVSegmentInfo
                trvInputFiles.AllNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo)
                    .ToList()
                    .ForEach(n => trvInputFiles.SetIsCheckBoxVisible(n, false));

                trvInputFiles.ExpandAll();

                // Check for error messages
                if (results.ErrorMessages != null && results.ErrorMessages.Any())
                {
                    ShowErrorMessage(string.Join(Environment.NewLine, results.ErrorMessages));
                }
            }
            finally
            {
                prgBrStatus.Value = 0;
                lblStatus.Text = "";

                grpInputFiles.Text = string.Format("Input Files (you can drag and drop files or directories) ({0} files)",
                    trvInputFiles.AllNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo));

                tlpMain.Enabled = true;
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
                Application.DoEvents();
            }
        }

        private NodeResults GetFileInfoNodes(string argMKVToolNixPath, List<string> argFiles)
        {
            NodeResults results = new NodeResults();
            List<TreeNode> fileNodes = new List<TreeNode>();

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
                    txtSegmentInfo.Text = string.Format("Analyzing {0}...", Path.GetFileName(sf));
                });

                statusStrip.Invoke((MethodInvoker)delegate
                {
                    prgBrStatus.Value = counter;
                    lblStatus.Text = string.Format("{0}%",
                        Convert.ToInt32((double)prgBrStatus.Value / (double)prgBrStatus.Maximum * 100.0));
                });

                try
                {
                    fileNodes.Add(GetFileNode(argMKVToolNixPath, sf));
                }
                catch (Exception ex)
                {
                    results.ErrorMessages.Add(string.Format("file: {0} error: {1}", Path.GetFileName(sf), ex.Message));
                }
            }

            txtSegmentInfo.Invoke((MethodInvoker)delegate
            {
                txtSegmentInfo.Clear();
            });

            results.Nodes = fileNodes;
            return results;
        }

        private TreeNode GetFileNode(string argMKVToolNixPath, string argFilename)
        {
            // Check if MKVToolNix path was provided
            if (string.IsNullOrWhiteSpace(argMKVToolNixPath))
            {
                throw new Exception("The MKVToolNix path was not provided!");
            }

            // Check if filename was provided
            if (string.IsNullOrWhiteSpace(argFilename))
            {
                throw new Exception("No filename was provided!");
            }

            // Check if file exists
            if (!File.Exists(argFilename))
            {
                throw new Exception(string.Format("The file {0} does not exist!", argFilename));
            }

            // Check if the extension is a valid matroska file
            string inputExtension = Path.GetExtension(argFilename).ToLower();
            if (inputExtension != ".mkv"
                && inputExtension != ".mka"
                && inputExtension != ".mks"
                && inputExtension != ".mk3d"
                && inputExtension != ".webm")
            {
                throw new Exception($"The input file {argFilename}{Environment.NewLine}{Environment.NewLine}is not a valid matroska file!");
            }

            // get the file information                    
            List<gMKVSegment> segmentList = gMKVHelper.GetMergedMkvSegmentList(argMKVToolNixPath, argFilename);

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
                        throw new Exception("Selected node has null tag!");
                    }
                    if (!(selNode.Tag is gMKVSegmentInfo))
                    {
                        // Get parent node
                        selNode = selNode.Parent;
                        if (selNode == null)
                        {
                            throw new Exception("Selected node has no parent node!");
                        }
                        if (selNode.Tag == null)
                        {
                            throw new Exception("Selected node has null tag!");
                        }
                        if (!(selNode.Tag is gMKVSegmentInfo))
                        {
                            throw new Exception("Selected node has no info!");
                        }
                    }
                    gMKVSegmentInfo seg = selNode.Tag as gMKVSegmentInfo;
                    txtSegmentInfo.Text = string.Format("Writing Application: {1}{0}Muxing Application: {2}{0}Duration: {3}{0}Date: {4}",
                        Environment.NewLine,
                        seg.WritingApplication,
                        seg.MuxingApplication,
                        seg.Duration,
                        seg.Date);

                    // check if output directory is the same as the source
                    if (chkUseSourceDirectory.Checked)
                    {
                        // set output directory to the source directory
                        txtOutputDirectory.Text = seg.Directory;
                    }

                    // Set the GroupBox title
                    grpSelectedFileInfo.Text = $"Selected File Information ({seg.Filename})";
                }
                else
                {
                    txtSegmentInfo.Clear();
                    grpSelectedFileInfo.Text = "Selected File Information";
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
            this.Invoke(new UpdateTrackLabelDelegate(UpdateTrackLabel), new object[] { filename, trackName });
        }

        void g_MkvExtractProgressUpdated(int progress)
        {
            this.Invoke(new UpdateProgressDelegate(UpdateProgress), new object[] { progress });
        }

        public void UpdateProgress(object val)
        {
            prgBrStatus.Value = Convert.ToInt32(val);
            prgBrTotalStatus.Value = (_CurrentJob - 1) * 100 + Convert.ToInt32(val);
            lblStatus.Text = string.Format("{0}%", Convert.ToInt32(val));
            lblTotalStatus.Text = string.Format("{0}%", prgBrTotalStatus.Value / _TotalJobs);
            
            // Update the task bar progress bar based on the total progress and not on the individual job
            gTaskbarProgress.SetValue(this, Convert.ToUInt64(prgBrTotalStatus.Value), (ulong)prgBrTotalStatus.Maximum);
            //gTaskbarProgress.SetValue(this, Convert.ToUInt64(val), (UInt64)100);

            Application.DoEvents();
        }

        public void UpdateTrackLabel(object filename, object val)
        {
            txtSegmentInfo.Text = string.Format("Extracting {0} from {1}...", val, Path.GetFileName((string)filename));
            Application.DoEvents();
        }

        private void CheckNeccessaryInputFields(bool checkSelectedTracks, bool checkSelectedChapterType)
        {
            if (string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
            {
                throw new Exception("You must provide with MKVToolnix path!");
            }
            if (!File.Exists(Path.Combine(txtMKVToolnixPath.Text.Trim(), gMKVHelper.MKV_MERGE_GUI_FILENAME))
                && !File.Exists(Path.Combine(txtMKVToolnixPath.Text.Trim(), gMKVHelper.MKV_MERGE_NEW_GUI_FILENAME)))
            {
                throw new Exception("The MKVToolnix path provided does not contain MKVToolnix files!");
            }
            if (!chkUseSourceDirectory.Checked && string.IsNullOrWhiteSpace(txtOutputDirectory.Text))
            {
                throw new Exception("You haven't specified an output directory!");
            }
            if (!chkUseSourceDirectory.Checked && !Directory.Exists(txtOutputDirectory.Text.Trim()))
            {
                // Ask the user to create the non existing output directory
                if (ShowQuestion(string.Format("The output directory \"{0}\" does not exist!{1}{1}Do you want to create it?", txtOutputDirectory.Text.Trim(), Environment.NewLine), "Output directory does not exist!", false) != DialogResult.Yes)
                {
                    throw new Exception(string.Format("The output directory \"{0}\" does not exist!{1}{1}Extraction was cancelled!", txtOutputDirectory.Text.Trim(), Environment.NewLine));
                }
                else
                {
                    // Create the non existing output directory
                    Directory.CreateDirectory(txtOutputDirectory.Text.Trim());
                }
            }

            // Get the checked nodes
            List<TreeNode> checkedNodes = trvInputFiles.CheckedNodes;

            if (checkSelectedTracks)
            {
                FormMkvExtractionMode selectedExtractionMode = (FormMkvExtractionMode)Enum.Parse(typeof(FormMkvExtractionMode), (string)cmbExtractionMode.SelectedItem);

                // Check if the checked nodes contain tracks
                if (!checkedNodes.Any(t => t.Tag != null && !(t.Tag is gMKVSegmentInfo)))
                {
                    if (selectedExtractionMode == FormMkvExtractionMode.Cue_Sheet || selectedExtractionMode == FormMkvExtractionMode.Tags)
                    {
                        throw new Exception($"You must select a file's track in order to extract {cmbExtractionMode.SelectedItem}!");
                    }
                    else
                    {
                        throw new Exception("You must select a track to extract!");
                    }
                }

                if (selectedExtractionMode == FormMkvExtractionMode.Timecodes ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Timecodes ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes)
                {
                    // Check if the ckecked nodes contain video, audio or subtitle track
                    if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVTrack)))
                    {
                        throw new Exception("You must select a video, audio or subtitles track to extract timecodes!");
                    }
                }

                if (selectedExtractionMode == FormMkvExtractionMode.Cues ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues ||
                    selectedExtractionMode == FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes)
                {
                    // Check if the ckecked nodes contain video, audio or subtitle track
                    if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVTrack)))
                    {
                        throw new Exception("You must select a video, audio or subtitles track to extract cues!");
                    }
                }
            }

            if (checkSelectedChapterType)
            {
                if (!checkedNodes.Any(t => t.Tag != null && (t.Tag is gMKVChapter)))
                {
                    if (cmbChapterType.SelectedIndex == -1)
                    {
                        throw new Exception("You must select a chapter type!");
                    }
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
            };
        }

        private void btnExtract_btnAddJobs_Click(object sender, EventArgs e)
        {
            bool exceptionOccured = false;
            try
            {
                tlpMain.Enabled = false;
                _ExtractRunning = true;
                Application.DoEvents();
                _gMkvExtract.MkvExtractProgressUpdated += g_MkvExtractProgressUpdated;
                _gMkvExtract.MkvExtractTrackUpdated += g_MkvExtractTrackUpdated;

                FormMkvExtractionMode extractionMode = (FormMkvExtractionMode)Enum.Parse(typeof(FormMkvExtractionMode), (string)cmbExtractionMode.SelectedItem);

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

                Thread myThread = null;
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
                            break;
                        case FormMkvExtractionMode.Cue_Sheet:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Tags:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.OnlyTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.NoCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Tracks_And_Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.WithTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.NoCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Cues:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.NoTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.OnlyCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Tracks_And_Cues:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.NoTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.WithCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
                            break;
                        case FormMkvExtractionMode.Tracks_And_Cues_And_Timecodes:
                            parameterList.MKVFile = infoSegment.Path;
                            parameterList.MKVSegmentsToExtract = segments;
                            parameterList.OutputDirectory = outputDirectory;
                            parameterList.ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), (string)cmbChapterType.SelectedItem);
                            parameterList.TimecodesExtractionMode = TimecodesExtractionMode.WithTimecodes;
                            parameterList.CueExtractionMode = CuesExtractionMode.WithCues;
                            parameterList.FilenamePatterns = GetFilenamePatterns();
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
                    _CurrentJob = 0;
                    _TotalJobs = jobs.Count;

                    prgBrStatus.Minimum = 0;
                    prgBrStatus.Maximum = 100;
                    prgBrTotalStatus.Maximum = _TotalJobs * 100;
                    prgBrTotalStatus.Visible = true;

                    foreach (var job in jobs)
                    {
                        // increate the current job index
                        _CurrentJob++;
                        // start the thread
                        myThread = new Thread(new ParameterizedThreadStart(job.ExtractMethod(_gMkvExtract)));
                        myThread.Start(job.ParametersList);

                        btnAbort.Enabled = true;
                        btnAbortAll.Enabled = true;
                        btnOptions.Enabled = false;
                        gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Normal);
                        gTaskbarProgress.SetOverlayIcon(this, SystemIcons.Shield, "Extracting...");
                        Application.DoEvents();
                        while (myThread.ThreadState != System.Threading.ThreadState.Stopped)
                        {
                            Application.DoEvents();
                        }
                        // check for exceptions
                        if (_gMkvExtract.ThreadedException != null)
                        {
                            throw _gMkvExtract.ThreadedException;
                        }
                        UpdateProgress(100);
                    }
                    btnAbort.Enabled = false;
                    btnAbortAll.Enabled = false;
                    this.Refresh();
                    Application.DoEvents();
                    if (chkShowPopup.Checked)
                    {
                        ShowSuccessMessage("The extraction was completed successfully!", true);
                    }
                    else
                    {
                        SystemSounds.Asterisk.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());

                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Error);
                gTaskbarProgress.SetOverlayIcon(this, SystemIcons.Error, "Error!");
                exceptionOccured = true;
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                if (_gMkvExtract != null)
                {
                    _gMkvExtract.MkvExtractProgressUpdated -= g_MkvExtractProgressUpdated;
                    _gMkvExtract.MkvExtractTrackUpdated -= g_MkvExtractTrackUpdated;
                }

                trvInputFiles.SelectedNode = null;
                txtSegmentInfo.Clear();
                grpSelectedFileInfo.Text = "Selected File Information";

                if (chkShowPopup.Checked || exceptionOccured)
                {
                    ClearStatus();
                }
                else
                {
                    if (sender == btnExtract)
                    {
                        txtSegmentInfo.Text = "Extraction completed!";
                    }
                }

                _ExtractRunning = false;
                tlpMain.Enabled = true;
                btnAbort.Enabled = false;
                btnAbortAll.Enabled = false;
                btnOptions.Enabled = true;
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
                gTaskbarProgress.SetOverlayIcon(this, null, null);
                this.Refresh();
                Application.DoEvents();
            }
        }

        private void ClearControls()
        {
            // check if output directory is the same as the source
            if (chkUseSourceDirectory.Checked)
            {
                txtOutputDirectory.Clear();
            }

            grpInputFiles.Text = "Input Files (you can drag and drop files or directories)";
            grpSelectedFileInfo.Text = "Selected File Information";

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
                        throw new Exception($"The folder does not contain MKVToolnix! {trimmedPath}");
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
                        FileName = "Select directory",
                        Title = "Select output directory..."
                    };
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
                    if (ShowQuestion("Do you really want to change MKVToolnix path?", "Are you sure?", false) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                OpenFileDialog ofd = new OpenFileDialog
                {
                    RestoreDirectory = true,
                    CheckFileExists = false,
                    CheckPathExists = false,
                    FileName = "Select directory",
                    Title = "Select MKVToolnix directory..."
                };
                if (!string.IsNullOrWhiteSpace(txtMKVToolnixPath.Text))
                {
                    if (Directory.Exists(txtMKVToolnixPath.Text.Trim()))
                    {
                        ofd.InitialDirectory = txtMKVToolnixPath.Text.Trim();
                    }
                }
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            tlpMain.Enabled = argStatus;
            Application.DoEvents();
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_ExtractRunning)
                {
                    e.Cancel = true;
                    ShowErrorMessage("There is an extraction process running! Please abort before closing!");
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

        #region "Context Menu"

        private void SetContextMenuText()
        {
            List<TreeNode> allNodes = trvInputFiles.AllNodes;
            List<TreeNode> checkedNodes = trvInputFiles.CheckedNodes;

            int allTracksCount = allNodes.Count(n => n != null && n.Tag != null && (n.Tag is gMKVTrack || n.Tag is gMKVChapter || n.Tag is gMKVAttachment));
            int videoTracksCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.video);
            int audioTracksCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.audio);
            int subtitleTracksCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.subtitles);
            int chapterTracksCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVChapter);
            int attachmentTracksCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVAttachment);

            int checkedAllTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && (n.Tag is gMKVTrack || n.Tag is gMKVChapter || n.Tag is gMKVAttachment));
            int checkedVideoTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.video);
            int checkedAudioTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.audio);
            int checkedSubtitleTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVTrack track && track.TrackType == MkvTrackType.subtitles);
            int checkedChapterTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVChapter);
            int checkedAttachmentTracksCount = checkedNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVAttachment);

            int allInputFilesCount = allNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo);

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

            checkTracksToolStripMenuItem.Text = string.Format("Check All Tracks ({0}/{1})", checkedAllTracksCount, allTracksCount);

            checkVideoTracksToolStripMenuItem.Text = string.Format("Check Video Tracks... ({0}/{1})", checkedVideoTracksCount, videoTracksCount);
            checkAudioTracksToolStripMenuItem.Text = string.Format("Check Audio Tracks... ({0}/{1})", checkedAudioTracksCount, audioTracksCount);
            checkSubtitleTracksToolStripMenuItem.Text = string.Format("Check Subtitle Tracks... ({0}/{1})", checkedSubtitleTracksCount, subtitleTracksCount);
            checkChapterTracksToolStripMenuItem.Text = string.Format("Check Chapter Tracks... ({0}/{1})", checkedChapterTracksCount, chapterTracksCount);
            checkAttachmentTracksToolStripMenuItem.Text = string.Format("Check Attachment Tracks... ({0}/{1})", checkedAttachmentTracksCount, attachmentTracksCount);

            allVideoTracksToolStripMenuItem.Text = string.Format("All Video Tracks ({0}/{1})", checkedVideoTracksCount, videoTracksCount);
            allAudioTracksToolStripMenuItem.Text = string.Format("All Audio Tracks ({0}/{1})", checkedAudioTracksCount, audioTracksCount);
            allSubtitleTracksToolStripMenuItem.Text = string.Format("All Subtitle Tracks ({0}/{1})", checkedSubtitleTracksCount, subtitleTracksCount);
            allChapterTracksToolStripMenuItem.Text = string.Format("All Chapter Tracks ({0}/{1})", checkedChapterTracksCount, chapterTracksCount);
            allAttachmentTracksToolStripMenuItem.Text = string.Format("All Attachment Tracks ({0}/{1})", checkedAttachmentTracksCount, attachmentTracksCount);

            uncheckTracksToolStripMenuItem.Text = string.Format("Uncheck All Tracks ({0}/{1})", (allTracksCount - checkedAllTracksCount), allTracksCount);

            uncheckVideoTracksToolStripMenuItem.Text = string.Format("Uncheck Video Tracks... ({0}/{1})", videoTracksCount - checkedVideoTracksCount, videoTracksCount);
            uncheckAudioTracksToolStripMenuItem.Text = string.Format("Uncheck Audio Tracks... ({0}/{1})", audioTracksCount - checkedAudioTracksCount, audioTracksCount);
            uncheckSubtitleTracksToolStripMenuItem.Text = string.Format("Uncheck Subtitle Tracks... ({0}/{1})", subtitleTracksCount - checkedSubtitleTracksCount, subtitleTracksCount);
            uncheckChapterTracksToolStripMenuItem.Text = string.Format("Uncheck Chapter Tracks... ({0}/{1})", chapterTracksCount - checkedChapterTracksCount, chapterTracksCount);
            uncheckAttachmentTracksToolStripMenuItem.Text = string.Format("Uncheck Attachment Tracks... ({0}/{1})", attachmentTracksCount - checkedAttachmentTracksCount, attachmentTracksCount);

            allVideoTracksToolStripMenuItem1.Text = string.Format("All Video Tracks ({0}/{1})", videoTracksCount - checkedVideoTracksCount, videoTracksCount);
            allAudioTracksToolStripMenuItem1.Text = string.Format("All Audio Tracks ({0}/{1})", audioTracksCount - checkedAudioTracksCount, audioTracksCount);
            allSubtitleTracksToolStripMenuItem1.Text = string.Format("All Subtitle Tracks ({0}/{1})", subtitleTracksCount - checkedSubtitleTracksCount, subtitleTracksCount);
            allChapterTracksToolStripMenuItem1.Text = string.Format("All Chapter Tracks ({0}/{1})", chapterTracksCount - checkedChapterTracksCount, chapterTracksCount);
            allAttachmentTracksToolStripMenuItem1.Text = string.Format("All Attachment Tracks ({0}/{1})", attachmentTracksCount - checkedAttachmentTracksCount, attachmentTracksCount);

            removeAllInputFilesToolStripMenuItem.Text = string.Format("Remove All Input Files ({0})", allInputFilesCount);

            removeSelectedInputFileToolStripMenuItem.Text = "Remove Selected Input File";

            checkVideoTracksToolStripMenuItem.DropDownItems.Clear();
            checkVideoTracksToolStripMenuItem.DropDownItems.Add(allVideoTracksToolStripMenuItem);
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Clear();
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(allVideoTracksToolStripMenuItem1);

            checkAudioTracksToolStripMenuItem.DropDownItems.Clear();
            checkAudioTracksToolStripMenuItem.DropDownItems.Add(allAudioTracksToolStripMenuItem);
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Clear();
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(allAudioTracksToolStripMenuItem1);

            checkSubtitleTracksToolStripMenuItem.DropDownItems.Clear();
            checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(allSubtitleTracksToolStripMenuItem);
            uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Clear();
            uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(allSubtitleTracksToolStripMenuItem1);

            List<ToolStripItem> checkItems = null;
            List<ToolStripItem> uncheckItems = null;

            // Get all video track languages
            List<string> videoLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).
                Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
            ToolStripMenuItem tsCheckVideoTracksByLanguage = new ToolStripMenuItem(string.Format("Video Tracks by Language ({0})...", videoLanguages.Count));
            checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByLanguage);
            ToolStripMenuItem tsUncheckVideoTracksByLanguage = new ToolStripMenuItem(string.Format("Video Tracks by Language ({0})...", videoLanguages.Count));
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByLanguage);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string lang in videoLanguages)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).Language == lang).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).Language == lang).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckVideoTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckVideoTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all video track languages ietf
            List<string> videoLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).
                Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
            ToolStripMenuItem tsCheckVideoTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Video Tracks by Language IETF ({0})...", videoLanguagesIetf.Count));
            checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByLanguageIetf);
            ToolStripMenuItem tsUncheckVideoTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Video Tracks by Language IETF ({0})...", videoLanguagesIetf.Count));
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByLanguageIetf);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string langIetf in videoLanguagesIetf)
            {
                int totalLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                int checkedLanguagesIetf = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckVideoTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckVideoTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all video track Codec_id 
            List<string> videoCodecs = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).
                Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
            ToolStripMenuItem tsCheckVideoTracksByCodec = new ToolStripMenuItem(string.Format("Video Tracks by Codec ({0})...", videoCodecs.Count));
            checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByCodec);
            ToolStripMenuItem tsUncheckVideoTracksByCodec = new ToolStripMenuItem(string.Format("Video Tracks by Codec ({0})...", videoCodecs.Count));
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByCodec);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string codec in videoCodecs)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).CodecID == codec).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).CodecID == codec).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckVideoTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckVideoTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all video track extra info
            List<string> videoExtra = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).
                Select(n => (n.Tag as gMKVTrack).ExtraInfo).Distinct().ToList();
            ToolStripMenuItem tsCheckVideoTracksByResolution = new ToolStripMenuItem(string.Format("Video Tracks by Resolution ({0})...", videoExtra.Count));
            checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByResolution);
            ToolStripMenuItem tsUncheckVideoTracksByResolution = new ToolStripMenuItem(string.Format("Video Tracks by Resolution ({0})...", videoExtra.Count));
            uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByResolution);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string extra in videoExtra)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Resolution: [{0}] ({1}/{2})", extra, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Resolution: [{0}] ({1}/{2})", extra, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckVideoTracksByResolution.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckVideoTracksByResolution.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all video track name
            List<string> videoNames = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video).
                Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
            // Only show menu items if the names are less than 50
            if (videoNames.Any() && videoNames.Count < 50)
            {
                ToolStripMenuItem tsCheckVideoTracksByName = new ToolStripMenuItem(string.Format("Video Tracks by Track Name ({0})...", videoNames.Count));
                checkVideoTracksToolStripMenuItem.DropDownItems.Add(tsCheckVideoTracksByName);
                ToolStripMenuItem tsUncheckVideoTracksByName = new ToolStripMenuItem(string.Format("Video Tracks by Track Name ({0})...", videoNames.Count));
                uncheckVideoTracksToolStripMenuItem.DropDownItems.Add(tsUncheckVideoTracksByName);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string name in videoNames)
                {
                    int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).TrackName == name).Count();
                    int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.video && (n.Tag as gMKVTrack).TrackName == name).Count();
                    var checkItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.video, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckVideoTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckVideoTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all audio track languages
            List<string> audioLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).
                Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
            ToolStripMenuItem tsCheckAudioTracksByLanguage = new ToolStripMenuItem(string.Format("Audio Tracks by Language ({0})...", audioLanguages.Count));
            checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByLanguage);
            ToolStripMenuItem tsUncheckAudioTracksByLanguage = new ToolStripMenuItem(string.Format("Audio Tracks by Language ({0})...", audioLanguages.Count));
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByLanguage);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string lang in audioLanguages)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).Language == lang).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).Language == lang).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckAudioTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckAudioTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all audio track languages ietf
            List<string> audioLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).
                Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
            ToolStripMenuItem tsCheckAudioTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Audio Tracks by Language IETF ({0})...", audioLanguagesIetf.Count));
            checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByLanguageIetf);
            ToolStripMenuItem tsUncheckAudioTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Audio Tracks by Language IETF  ({0})...", audioLanguagesIetf.Count));
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByLanguageIetf);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string langIetf in audioLanguagesIetf)
            {
                int totalLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                int checkedLanguagesIetf = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckAudioTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckAudioTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all audio track Codec_id 
            List<string> audioCodecs = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).
                Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
            ToolStripMenuItem tsCheckAudioTracksByCodec = new ToolStripMenuItem(string.Format("Audio Tracks by Codec ({0})...", audioCodecs.Count));
            checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByCodec);
            ToolStripMenuItem tsUncheckAudioTracksByCodec = new ToolStripMenuItem(string.Format("Audio Tracks by Codec ({0})...", audioCodecs.Count));
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByCodec);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string codec in audioCodecs)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).CodecID == codec).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).CodecID == codec).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckAudioTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckAudioTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all audio track extra info
            List<string> audioExtraInfo = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).
                Select(n => (n.Tag as gMKVTrack).ExtraInfo).Distinct().ToList();
            ToolStripMenuItem tsCheckAudioTracksByChannels = new ToolStripMenuItem(string.Format("Audio Tracks by Channels ({0})...", audioExtraInfo.Count));
            checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByChannels);
            ToolStripMenuItem tsUncheckAudioTracksByChannels = new ToolStripMenuItem(string.Format("Audio Tracks by Channels ({0})...", audioExtraInfo.Count));
            uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByChannels);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string extra in audioExtraInfo)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).ExtraInfo == extra).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Channels: [{0}] ({1}/{2})", extra, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Channels: [{0}] ({1}/{2})", extra, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.ExtraInfo, argFilter: extra); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckAudioTracksByChannels.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckAudioTracksByChannels.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all audio track name
            List<string> audioNames = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio).
                Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
            // Only show menu items if the names are less than 50
            if (audioNames.Any() && audioNames.Count < 50)
            {
                ToolStripMenuItem tsCheckAudioTracksByName = new ToolStripMenuItem(string.Format("Audio Tracks by Track Name ({0})...", audioNames.Count));
                checkAudioTracksToolStripMenuItem.DropDownItems.Add(tsCheckAudioTracksByName);
                ToolStripMenuItem tsUncheckAudioTracksByName = new ToolStripMenuItem(string.Format("Audio Tracks by Track Name ({0})...", audioNames.Count));
                uncheckAudioTracksToolStripMenuItem.DropDownItems.Add(tsUncheckAudioTracksByName);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string name in audioNames)
                {
                    int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).TrackName == name).Count();
                    int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.audio && (n.Tag as gMKVTrack).TrackName == name).Count();
                    var checkItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.audio, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckAudioTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckAudioTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
            }

            // Get all subtitle track languages
            List<string> subLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).
                Select(n => (n.Tag as gMKVTrack).Language).Distinct().ToList();
            ToolStripMenuItem tsCheckSubtitleTracksByLanguage = new ToolStripMenuItem(string.Format("Subtitle Tracks by Language ({0})...", subLanguages.Count));
            checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByLanguage);
            ToolStripMenuItem tsUncheckSubtitleTracksByLanguage = new ToolStripMenuItem(string.Format("Subtitle Tracks by Language ({0})...", subLanguages.Count));
            uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByLanguage);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string lang in subLanguages)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).Language == lang).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).Language == lang).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language: [{0}] ({1}/{2})", lang, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.Language, argFilter: lang); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckSubtitleTracksByLanguage.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckSubtitleTracksByLanguage.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all subtitle track languages IETF
            List<string> subLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).
                Select(n => (n.Tag as gMKVTrack).LanguageIetf).Distinct().ToList();
            ToolStripMenuItem tsCheckSubtitleTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Subtitle Tracks by Language IETF ({0})...", subLanguagesIetf.Count));
            checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByLanguageIetf);
            ToolStripMenuItem tsUncheckSubtitleTracksByLanguageIetf = new ToolStripMenuItem(string.Format("Subtitle Tracks by Language IETF ({0})...", subLanguagesIetf.Count));
            uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByLanguageIetf);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string langIetf in subLanguagesIetf)
            {
                int totalLanguagesIetf = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                int checkedLanguagesIetf = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).LanguageIetf == langIetf).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Language IETF: [{0}] ({1}/{2})", langIetf, totalLanguagesIetf - checkedLanguagesIetf, totalLanguagesIetf), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.LanguageIetf, argFilter: langIetf); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckSubtitleTracksByLanguageIetf.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckSubtitleTracksByLanguageIetf.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all subtitle track codec_id
            List<string> subCodecs = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).
                Select(n => (n.Tag as gMKVTrack).CodecID).Distinct().ToList();
            ToolStripMenuItem tsCheckSubtitleTracksByCodec = new ToolStripMenuItem(string.Format("Subtitle Tracks by Codec ({0})...", subCodecs.Count));
            checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByCodec);
            ToolStripMenuItem tsUncheckSubtitleTracksByCodec = new ToolStripMenuItem(string.Format("Subtitle Tracks by Codec ({0})...", subCodecs.Count));
            uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByCodec);
            checkItems = new List<ToolStripItem>();
            uncheckItems = new List<ToolStripItem>();
            foreach (string codec in subCodecs)
            {
                int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).CodecID == codec).Count();
                int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).CodecID == codec).Count();
                var checkItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                checkItems.Add(checkItem);
                var uncheckItem = new ToolStripMenuItem(string.Format("Codec: [{0}] ({1}/{2})", codec, totalLanguages - checkedLanguages, totalLanguages), null,
                        delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.CodecId, argFilter: codec); }
                    );
                ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                uncheckItems.Add(uncheckItem);
            }
            tsCheckSubtitleTracksByCodec.DropDownItems.AddRange(checkItems.ToArray());
            tsUncheckSubtitleTracksByCodec.DropDownItems.AddRange(uncheckItems.ToArray());

            // Get all subtitle track names
            List<string> subNames = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles).
                Select(n => (n.Tag as gMKVTrack).TrackName).Distinct().ToList();
            // Only show menu items if the names are less than 50
            if (subNames.Any() && subNames.Count < 50)
            {
                ToolStripMenuItem tsCheckSubtitleTracksByName = new ToolStripMenuItem(string.Format("Subtitle Tracks by Track Name ({0})...", subNames.Count));
                checkSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsCheckSubtitleTracksByName);
                ToolStripMenuItem tsUncheckSubtitleTracksByName = new ToolStripMenuItem(string.Format("Subtitle Tracks by Track Name ({0})...", subNames.Count));
                uncheckSubtitleTracksToolStripMenuItem.DropDownItems.Add(tsUncheckSubtitleTracksByName);
                checkItems = new List<ToolStripItem>();
                uncheckItems = new List<ToolStripItem>();
                foreach (string name in subNames)
                {
                    int totalLanguages = allNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).TrackName == name).Count();
                    int checkedLanguages = checkedNodes.Where(n => n != null && n.Tag != null && n.Tag is gMKVTrack && (n.Tag as gMKVTrack).TrackType == MkvTrackType.subtitles && (n.Tag as gMKVTrack).TrackName == name).Count();
                    var checkItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, true, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(checkItem, _Settings.DarkMode); // Apply theme
                    checkItems.Add(checkItem);
                    var uncheckItem = new ToolStripMenuItem(string.Format("Track Name: [{0}] ({1}/{2})", name, totalLanguages - checkedLanguages, totalLanguages), null,
                            delegate { SetCheckedTracks(TrackSelectionMode.subtitle, false, nodeSelectionFilter: NodeSelectionFilter.Name, argFilter: name); }
                        );
                    ThemeManager.ApplyToolStripItemTheme(uncheckItem, _Settings.DarkMode);
                    uncheckItems.Add(uncheckItem);
                }
                tsCheckSubtitleTracksByName.DropDownItems.AddRange(checkItems.ToArray());
                tsUncheckSubtitleTracksByName.DropDownItems.AddRange(uncheckItems.ToArray());
            }
        }

        private enum NodeSelectionFilter
        {
            Language,
            LanguageIetf,
            ExtraInfo,
            CodecId,
            Name,
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
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            SetContextMenuText();

            // Apply theme to context menu and its items
            if (contextMenuStrip != null)
            {
                ThemeManager.ApplyTheme(contextMenuStrip, _Settings.DarkMode);
            }
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
            if (trvInputFiles.Nodes.Count > 0)
            {
                grpInputFiles.Text = string.Format(
                    "Input Files (you can drag and drop files or directories) ({0} files)",
                    trvInputFiles.AllNodes.Count(n => n != null && n.Tag != null && n.Tag is gMKVSegmentInfo));
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
                    Title = "Select an input matroska file...",
                    Filter = "Matroska files (*.mkv;*.mka;*.mks;*.mk3d;*.webm)|*.mkv;*.mka;*.mks;*.mk3d;*.webm|Matroska video files (*.mkv)|*.mkv|Matroska audio files (*.mka)|*.mka|Matroska subtitle files (*.mks)|*.mks|Matroska 3D files (*.mk3d)|*.mk3d|Webm files (*.webm)|*.webm",
                    Multiselect = true,
                    AutoUpgradeEnabled = true
                };
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = string.Format("Use Currently Set Default Directory: ({0})", _Settings.DefaultOutputDirectory);
                }
                else
                {
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Enabled = false;
                    useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = "Use Currently Set Default Directory: (Not Set!)";
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
                        if (ShowQuestion(string.Format("Are you sure you want to change the currently set ({0}) default output directory?",
                            _Settings.DefaultOutputDirectory), "Are you sure?", false) == DialogResult.No)
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
                frmOptions optionsForm = new frmOptions();

                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    _Settings.Reload();
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
                    ThemeManager.ApplyTheme(contextMenuStrip, _Settings.DarkMode);
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
    }
}
