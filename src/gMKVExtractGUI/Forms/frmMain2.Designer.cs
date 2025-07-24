using gMKVToolNix.Controls;

namespace gMKVToolNix.Forms
{
    partial class frmMain2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.prgBrStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.prgBrTotalStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.lblTotalStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlpMain = new gMKVToolNix.gTableLayoutPanel();
            this.grpActions = new gMKVToolNix.gGroupBox();
            this.btnAddJobs = new System.Windows.Forms.Button();
            this.btnShowJobs = new System.Windows.Forms.Button();
            this.chkShowPopup = new System.Windows.Forms.CheckBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.lblExtractionMode = new System.Windows.Forms.Label();
            this.cmbExtractionMode = new gMKVToolNix.Controls.gComboBox();
            this.btnShowLog = new System.Windows.Forms.Button();
            this.lblChapterType = new System.Windows.Forms.Label();
            this.cmbChapterType = new gMKVToolNix.Controls.gComboBox();
            this.grpOutputDirectory = new gMKVToolNix.gGroupBox();
            this.chkUseSourceDirectory = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutputDirectory = new System.Windows.Forms.Button();
            this.txtOutputDirectory = new gMKVToolNix.gTextBox();
            this.contextMenuStripOutputDirectory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setAsDefaultDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpConfig = new gMKVToolNix.gGroupBox();
            this.btnAutoDetectMkvToolnix = new System.Windows.Forms.Button();
            this.txtMKVToolnixPath = new gMKVToolNix.gTextBox();
            this.btnBrowseMKVToolnixPath = new System.Windows.Forms.Button();
            this.grpInputFiles = new gMKVToolNix.gGroupBox();
            this.tlpInput = new System.Windows.Forms.TableLayoutPanel();
            this.trvInputFiles = new gMKVToolNix.gTreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addInputFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.checkTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.checkVideoTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allVideoTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAudioTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allAudioTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSubtitleTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allSubtitleTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkChapterTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allChapterTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAttachmentTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allAttachmentTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.uncheckTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.uncheckVideoTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allVideoTracksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAudioTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allAudioTracksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckSubtitleTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allSubtitleTracksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckChapterTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allChapterTracksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAttachmentTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allAttachmentTracksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAllInputFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedInputFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSelectedFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSelectedFileFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFileOptions = new System.Windows.Forms.Panel();
            this.chkOverwriteExistingFiles = new System.Windows.Forms.CheckBox();
            this.chkAppendOnDragAndDrop = new System.Windows.Forms.CheckBox();
            this.grpSelectedFileInfo = new gMKVToolNix.gGroupBox();
            this.txtSegmentInfo = new gMKVToolNix.gRichTextBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAbortAll = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.chkDarkMode = new System.Windows.Forms.CheckBox();
            this.statusStrip.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpOutputDirectory.SuspendLayout();
            this.contextMenuStripOutputDirectory.SuspendLayout();
            this.grpConfig.SuspendLayout();
            this.grpInputFiles.SuspendLayout();
            this.tlpInput.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.pnlFileOptions.SuspendLayout();
            this.grpSelectedFileInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.AutoSize = false;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prgBrStatus,
            this.lblStatus,
            this.prgBrTotalStatus,
            this.lblTotalStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 525);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(624, 36);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // prgBrStatus
            // 
            this.prgBrStatus.AutoSize = false;
            this.prgBrStatus.Name = "prgBrStatus";
            this.prgBrStatus.Size = new System.Drawing.Size(100, 30);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(50, 31);
            this.lblStatus.Text = "status";
            // 
            // prgBrTotalStatus
            // 
            this.prgBrTotalStatus.Name = "prgBrTotalStatus";
            this.prgBrTotalStatus.Size = new System.Drawing.Size(100, 30);
            // 
            // lblTotalStatus
            // 
            this.lblTotalStatus.AutoSize = false;
            this.lblTotalStatus.Name = "lblTotalStatus";
            this.lblTotalStatus.Size = new System.Drawing.Size(50, 31);
            this.lblTotalStatus.Text = "status";
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpActions, 0, 4);
            this.tlpMain.Controls.Add(this.grpOutputDirectory, 0, 3);
            this.tlpMain.Controls.Add(this.grpConfig, 0, 0);
            this.tlpMain.Controls.Add(this.grpInputFiles, 0, 1);
            this.tlpMain.Controls.Add(this.grpSelectedFileInfo, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.Size = new System.Drawing.Size(624, 525);
            this.tlpMain.TabIndex = 1;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnAddJobs);
            this.grpActions.Controls.Add(this.btnShowJobs);
            this.grpActions.Controls.Add(this.chkShowPopup);
            this.grpActions.Controls.Add(this.btnExtract);
            this.grpActions.Controls.Add(this.lblExtractionMode);
            this.grpActions.Controls.Add(this.cmbExtractionMode);
            this.grpActions.Controls.Add(this.btnShowLog);
            this.grpActions.Controls.Add(this.lblChapterType);
            this.grpActions.Controls.Add(this.cmbChapterType);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 468);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(618, 54);
            this.grpActions.TabIndex = 8;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnAddJobs
            // 
            this.btnAddJobs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddJobs.Location = new System.Drawing.Point(458, 17);
            this.btnAddJobs.Name = "btnAddJobs";
            this.btnAddJobs.Size = new System.Drawing.Size(70, 30);
            this.btnAddJobs.TabIndex = 14;
            this.btnAddJobs.Text = "Add Jobs";
            this.btnAddJobs.UseVisualStyleBackColor = true;
            this.btnAddJobs.Click += new System.EventHandler(this.btnExtract_btnAddJobs_Click);
            // 
            // btnShowJobs
            // 
            this.btnShowJobs.Location = new System.Drawing.Point(70, 17);
            this.btnShowJobs.Name = "btnShowJobs";
            this.btnShowJobs.Size = new System.Drawing.Size(60, 30);
            this.btnShowJobs.TabIndex = 13;
            this.btnShowJobs.Text = "Jobs...";
            this.btnShowJobs.UseVisualStyleBackColor = true;
            this.btnShowJobs.Click += new System.EventHandler(this.btnShowJobs_Click);
            // 
            // chkShowPopup
            // 
            this.chkShowPopup.AutoSize = true;
            this.chkShowPopup.Location = new System.Drawing.Point(134, 23);
            this.chkShowPopup.Name = "chkShowPopup";
            this.chkShowPopup.Size = new System.Drawing.Size(61, 19);
            this.chkShowPopup.TabIndex = 12;
            this.chkShowPopup.Text = "Popup";
            this.chkShowPopup.UseVisualStyleBackColor = true;
            this.chkShowPopup.CheckedChanged += new System.EventHandler(this.chkShowPopup_CheckedChanged);
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(531, 17);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(80, 30);
            this.btnExtract.TabIndex = 10;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_btnAddJobs_Click);
            // 
            // lblExtractionMode
            // 
            this.lblExtractionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExtractionMode.AutoSize = true;
            this.lblExtractionMode.Location = new System.Drawing.Point(306, 25);
            this.lblExtractionMode.Name = "lblExtractionMode";
            this.lblExtractionMode.Size = new System.Drawing.Size(42, 15);
            this.lblExtractionMode.TabIndex = 9;
            this.lblExtractionMode.Text = "Extract";
            // 
            // cmbExtractionMode
            // 
            this.cmbExtractionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbExtractionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtractionMode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbExtractionMode.FormattingEnabled = true;
            this.cmbExtractionMode.Location = new System.Drawing.Point(352, 21);
            this.cmbExtractionMode.Name = "cmbExtractionMode";
            this.cmbExtractionMode.Size = new System.Drawing.Size(102, 23);
            this.cmbExtractionMode.TabIndex = 8;
            // 
            // btnShowLog
            // 
            this.btnShowLog.Location = new System.Drawing.Point(6, 17);
            this.btnShowLog.Name = "btnShowLog";
            this.btnShowLog.Size = new System.Drawing.Size(60, 30);
            this.btnShowLog.TabIndex = 6;
            this.btnShowLog.Text = "Log...";
            this.btnShowLog.UseVisualStyleBackColor = true;
            this.btnShowLog.Click += new System.EventHandler(this.btnShowLog_Click);
            // 
            // lblChapterType
            // 
            this.lblChapterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChapterType.AutoSize = true;
            this.lblChapterType.Location = new System.Drawing.Point(195, 25);
            this.lblChapterType.Name = "lblChapterType";
            this.lblChapterType.Size = new System.Drawing.Size(49, 15);
            this.lblChapterType.TabIndex = 3;
            this.lblChapterType.Text = "Chapter";
            // 
            // cmbChapterType
            // 
            this.cmbChapterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChapterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChapterType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbChapterType.FormattingEnabled = true;
            this.cmbChapterType.Location = new System.Drawing.Point(248, 21);
            this.cmbChapterType.Name = "cmbChapterType";
            this.cmbChapterType.Size = new System.Drawing.Size(52, 23);
            this.cmbChapterType.TabIndex = 2;
            this.cmbChapterType.SelectedIndexChanged += new System.EventHandler(this.cmbChapterType_SelectedIndexChanged);
            // 
            // grpOutputDirectory
            // 
            this.grpOutputDirectory.Controls.Add(this.chkUseSourceDirectory);
            this.grpOutputDirectory.Controls.Add(this.btnBrowseOutputDirectory);
            this.grpOutputDirectory.Controls.Add(this.txtOutputDirectory);
            this.grpOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOutputDirectory.Location = new System.Drawing.Point(3, 408);
            this.grpOutputDirectory.Name = "grpOutputDirectory";
            this.grpOutputDirectory.Size = new System.Drawing.Size(618, 54);
            this.grpOutputDirectory.TabIndex = 7;
            this.grpOutputDirectory.TabStop = false;
            this.grpOutputDirectory.Text = "Output Directory for Selected File (you can drag and drop the directory)";
            // 
            // chkUseSourceDirectory
            // 
            this.chkUseSourceDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUseSourceDirectory.AutoSize = true;
            this.chkUseSourceDirectory.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseSourceDirectory.Location = new System.Drawing.Point(441, 24);
            this.chkUseSourceDirectory.Name = "chkUseSourceDirectory";
            this.chkUseSourceDirectory.Size = new System.Drawing.Size(84, 19);
            this.chkUseSourceDirectory.TabIndex = 4;
            this.chkUseSourceDirectory.Text = "Use Source";
            this.chkUseSourceDirectory.UseVisualStyleBackColor = true;
            this.chkUseSourceDirectory.CheckedChanged += new System.EventHandler(this.chkUseSourceDirectory_CheckedChanged);
            // 
            // btnBrowseOutputDirectory
            // 
            this.btnBrowseOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutputDirectory.Location = new System.Drawing.Point(531, 18);
            this.btnBrowseOutputDirectory.Name = "btnBrowseOutputDirectory";
            this.btnBrowseOutputDirectory.Size = new System.Drawing.Size(80, 30);
            this.btnBrowseOutputDirectory.TabIndex = 3;
            this.btnBrowseOutputDirectory.Text = "Browse...";
            this.btnBrowseOutputDirectory.UseVisualStyleBackColor = true;
            this.btnBrowseOutputDirectory.Click += new System.EventHandler(this.btnBrowseOutputDirectory_Click);
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.AllowDrop = true;
            this.txtOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDirectory.ContextMenuStrip = this.contextMenuStripOutputDirectory;
            this.txtOutputDirectory.Location = new System.Drawing.Point(6, 22);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.ShortcutsEnabled = false;
            this.txtOutputDirectory.Size = new System.Drawing.Size(429, 23);
            this.txtOutputDirectory.TabIndex = 2;
            this.txtOutputDirectory.WordWrap = false;
            this.txtOutputDirectory.TextChanged += new System.EventHandler(this.txtOutputDirectory_TextChanged);
            this.txtOutputDirectory.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtOutputDirectory.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // contextMenuStripOutputDirectory
            // 
            this.contextMenuStripOutputDirectory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setAsDefaultDirectoryToolStripMenuItem,
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem});
            this.contextMenuStripOutputDirectory.Name = "contextMenuStripOutputDirectory";
            this.contextMenuStripOutputDirectory.Size = new System.Drawing.Size(260, 48);
            this.contextMenuStripOutputDirectory.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripOutputDirectory_Opening);
            // 
            // setAsDefaultDirectoryToolStripMenuItem
            // 
            this.setAsDefaultDirectoryToolStripMenuItem.Name = "setAsDefaultDirectoryToolStripMenuItem";
            this.setAsDefaultDirectoryToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.setAsDefaultDirectoryToolStripMenuItem.Text = "Set As Default Directory";
            this.setAsDefaultDirectoryToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultDirectoryToolStripMenuItem_Click);
            // 
            // useCurrentlySetDefaultDirectoryToolStripMenuItem
            // 
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Enabled = false;
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Name = "useCurrentlySetDefaultDirectoryToolStripMenuItem";
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = "Use Currently Set Default Directory:";
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Click += new System.EventHandler(this.useCurrentlySetDefaultDirectoryToolStripMenuItem_Click);
            // 
            // grpConfig
            // 
            this.grpConfig.Controls.Add(this.btnAutoDetectMkvToolnix);
            this.grpConfig.Controls.Add(this.txtMKVToolnixPath);
            this.grpConfig.Controls.Add(this.btnBrowseMKVToolnixPath);
            this.grpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConfig.Location = new System.Drawing.Point(3, 3);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Size = new System.Drawing.Size(618, 54);
            this.grpConfig.TabIndex = 0;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "MKVToolnix Directory (you can drag and drop the directory)";
            // 
            // btnAutoDetectMkvToolnix
            // 
            this.btnAutoDetectMkvToolnix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoDetectMkvToolnix.Location = new System.Drawing.Point(531, 18);
            this.btnAutoDetectMkvToolnix.Name = "btnAutoDetectMkvToolnix";
            this.btnAutoDetectMkvToolnix.Size = new System.Drawing.Size(80, 30);
            this.btnAutoDetectMkvToolnix.TabIndex = 8;
            this.btnAutoDetectMkvToolnix.Text = "Auto Detect";
            this.btnAutoDetectMkvToolnix.UseVisualStyleBackColor = true;
            this.btnAutoDetectMkvToolnix.Click += new System.EventHandler(this.btnAutoDetectMkvToolnix_Click);
            // 
            // txtMKVToolnixPath
            // 
            this.txtMKVToolnixPath.AllowDrop = true;
            this.txtMKVToolnixPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMKVToolnixPath.Location = new System.Drawing.Point(6, 22);
            this.txtMKVToolnixPath.Name = "txtMKVToolnixPath";
            this.txtMKVToolnixPath.ReadOnly = true;
            this.txtMKVToolnixPath.ShortcutsEnabled = false;
            this.txtMKVToolnixPath.Size = new System.Drawing.Size(448, 23);
            this.txtMKVToolnixPath.TabIndex = 7;
            this.txtMKVToolnixPath.WordWrap = false;
            this.txtMKVToolnixPath.TextChanged += new System.EventHandler(this.txtMKVToolnixPath_TextChanged);
            this.txtMKVToolnixPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtMKVToolnixPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // btnBrowseMKVToolnixPath
            // 
            this.btnBrowseMKVToolnixPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMKVToolnixPath.Location = new System.Drawing.Point(458, 18);
            this.btnBrowseMKVToolnixPath.Name = "btnBrowseMKVToolnixPath";
            this.btnBrowseMKVToolnixPath.Size = new System.Drawing.Size(70, 30);
            this.btnBrowseMKVToolnixPath.TabIndex = 6;
            this.btnBrowseMKVToolnixPath.Text = "Browse...";
            this.btnBrowseMKVToolnixPath.UseVisualStyleBackColor = true;
            this.btnBrowseMKVToolnixPath.Click += new System.EventHandler(this.btnBrowseMKVToolnixPath_Click);
            // 
            // grpInputFiles
            // 
            this.grpInputFiles.Controls.Add(this.tlpInput);
            this.grpInputFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInputFiles.Location = new System.Drawing.Point(3, 63);
            this.grpInputFiles.Name = "grpInputFiles";
            this.grpInputFiles.Size = new System.Drawing.Size(618, 239);
            this.grpInputFiles.TabIndex = 1;
            this.grpInputFiles.TabStop = false;
            this.grpInputFiles.Text = "Input Files (you can drag and drop files or directories)";
            // 
            // tlpInput
            // 
            this.tlpInput.ColumnCount = 1;
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.Controls.Add(this.trvInputFiles, 0, 0);
            this.tlpInput.Controls.Add(this.pnlFileOptions, 0, 1);
            this.tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInput.Location = new System.Drawing.Point(3, 19);
            this.tlpInput.Name = "tlpInput";
            this.tlpInput.RowCount = 2;
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpInput.Size = new System.Drawing.Size(612, 217);
            this.tlpInput.TabIndex = 1;
            // 
            // trvInputFiles
            // 
            this.trvInputFiles.AllowDrop = true;
            this.trvInputFiles.CheckBoxes = true;
            this.trvInputFiles.CheckOnClick = true;
            this.trvInputFiles.ContextMenuStrip = this.contextMenuStrip;
            this.trvInputFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvInputFiles.HideSelection = false;
            this.trvInputFiles.Location = new System.Drawing.Point(3, 3);
            this.trvInputFiles.Name = "trvInputFiles";
            this.trvInputFiles.SelectOnRightClick = true;
            this.trvInputFiles.ShowNodeToolTips = true;
            this.trvInputFiles.Size = new System.Drawing.Size(606, 186);
            this.trvInputFiles.TabIndex = 0;
            this.trvInputFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvInputFiles_AfterSelect);
            this.trvInputFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.trvInputFiles_DragDrop);
            this.trvInputFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.trvInputFiles_DragEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addInputFileToolStripMenuItem,
            this.toolStripSeparator5,
            this.checkTracksToolStripMenuItem,
            this.toolStripSeparator2,
            this.checkVideoTracksToolStripMenuItem,
            this.checkAudioTracksToolStripMenuItem,
            this.checkSubtitleTracksToolStripMenuItem,
            this.checkChapterTracksToolStripMenuItem,
            this.checkAttachmentTracksToolStripMenuItem,
            this.toolStripMenuItem1,
            this.uncheckTracksToolStripMenuItem,
            this.toolStripSeparator1,
            this.uncheckVideoTracksToolStripMenuItem,
            this.uncheckAudioTracksToolStripMenuItem,
            this.uncheckSubtitleTracksToolStripMenuItem,
            this.uncheckChapterTracksToolStripMenuItem,
            this.uncheckAttachmentTracksToolStripMenuItem,
            this.toolStripSeparator4,
            this.removeAllInputFilesToolStripMenuItem,
            this.removeSelectedInputFileToolStripMenuItem,
            this.openSelectedFileToolStripMenuItem,
            this.openSelectedFileFolderToolStripMenuItem,
            this.toolStripSeparator3,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(232, 458);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // addInputFileToolStripMenuItem
            // 
            this.addInputFileToolStripMenuItem.Name = "addInputFileToolStripMenuItem";
            this.addInputFileToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.addInputFileToolStripMenuItem.Text = "Add Input File(s)...";
            this.addInputFileToolStripMenuItem.Click += new System.EventHandler(this.addInputFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(228, 6);
            // 
            // checkTracksToolStripMenuItem
            // 
            this.checkTracksToolStripMenuItem.Name = "checkTracksToolStripMenuItem";
            this.checkTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkTracksToolStripMenuItem.Text = "Check All Tracks";
            this.checkTracksToolStripMenuItem.Click += new System.EventHandler(this.checkTracksToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(228, 6);
            // 
            // checkVideoTracksToolStripMenuItem
            // 
            this.checkVideoTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allVideoTracksToolStripMenuItem});
            this.checkVideoTracksToolStripMenuItem.Name = "checkVideoTracksToolStripMenuItem";
            this.checkVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkVideoTracksToolStripMenuItem.Text = "Check Video Tracks...";
            // 
            // allVideoTracksToolStripMenuItem
            // 
            this.allVideoTracksToolStripMenuItem.Name = "allVideoTracksToolStripMenuItem";
            this.allVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.allVideoTracksToolStripMenuItem.Text = "All Video Tracks";
            this.allVideoTracksToolStripMenuItem.Click += new System.EventHandler(this.allVideoTracksToolStripMenuItem_Click);
            // 
            // checkAudioTracksToolStripMenuItem
            // 
            this.checkAudioTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAudioTracksToolStripMenuItem});
            this.checkAudioTracksToolStripMenuItem.Name = "checkAudioTracksToolStripMenuItem";
            this.checkAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkAudioTracksToolStripMenuItem.Text = "Check Audio Tracks...";
            // 
            // allAudioTracksToolStripMenuItem
            // 
            this.allAudioTracksToolStripMenuItem.Name = "allAudioTracksToolStripMenuItem";
            this.allAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.allAudioTracksToolStripMenuItem.Text = "All Audio Tracks";
            this.allAudioTracksToolStripMenuItem.Click += new System.EventHandler(this.allAudioTracksToolStripMenuItem_Click);
            // 
            // checkSubtitleTracksToolStripMenuItem
            // 
            this.checkSubtitleTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allSubtitleTracksToolStripMenuItem});
            this.checkSubtitleTracksToolStripMenuItem.Name = "checkSubtitleTracksToolStripMenuItem";
            this.checkSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkSubtitleTracksToolStripMenuItem.Text = "Check Subtitle Tracks...";
            // 
            // allSubtitleTracksToolStripMenuItem
            // 
            this.allSubtitleTracksToolStripMenuItem.Name = "allSubtitleTracksToolStripMenuItem";
            this.allSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.allSubtitleTracksToolStripMenuItem.Text = "All Subtitle Tracks";
            this.allSubtitleTracksToolStripMenuItem.Click += new System.EventHandler(this.allSubtitleTracksToolStripMenuItem_Click);
            // 
            // checkChapterTracksToolStripMenuItem
            // 
            this.checkChapterTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allChapterTracksToolStripMenuItem});
            this.checkChapterTracksToolStripMenuItem.Name = "checkChapterTracksToolStripMenuItem";
            this.checkChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkChapterTracksToolStripMenuItem.Text = "Check Chapter Tracks...";
            // 
            // allChapterTracksToolStripMenuItem
            // 
            this.allChapterTracksToolStripMenuItem.Name = "allChapterTracksToolStripMenuItem";
            this.allChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.allChapterTracksToolStripMenuItem.Text = "All Chapter Tracks";
            this.allChapterTracksToolStripMenuItem.Click += new System.EventHandler(this.allChapterTracksToolStripMenuItem_Click);
            // 
            // checkAttachmentTracksToolStripMenuItem
            // 
            this.checkAttachmentTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAttachmentTracksToolStripMenuItem});
            this.checkAttachmentTracksToolStripMenuItem.Name = "checkAttachmentTracksToolStripMenuItem";
            this.checkAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.checkAttachmentTracksToolStripMenuItem.Text = "Check Attachment Tracks...";
            // 
            // allAttachmentTracksToolStripMenuItem
            // 
            this.allAttachmentTracksToolStripMenuItem.Name = "allAttachmentTracksToolStripMenuItem";
            this.allAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.allAttachmentTracksToolStripMenuItem.Text = "All Attachment Tracks";
            this.allAttachmentTracksToolStripMenuItem.Click += new System.EventHandler(this.allAttachmentTracksToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(228, 6);
            // 
            // uncheckTracksToolStripMenuItem
            // 
            this.uncheckTracksToolStripMenuItem.Name = "uncheckTracksToolStripMenuItem";
            this.uncheckTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckTracksToolStripMenuItem.Text = "Uncheck All Tracks";
            this.uncheckTracksToolStripMenuItem.Click += new System.EventHandler(this.uncheckTracksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(228, 6);
            // 
            // uncheckVideoTracksToolStripMenuItem
            // 
            this.uncheckVideoTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allVideoTracksToolStripMenuItem1});
            this.uncheckVideoTracksToolStripMenuItem.Name = "uncheckVideoTracksToolStripMenuItem";
            this.uncheckVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckVideoTracksToolStripMenuItem.Text = "Uncheck Video Tracks...";
            // 
            // allVideoTracksToolStripMenuItem1
            // 
            this.allVideoTracksToolStripMenuItem1.Name = "allVideoTracksToolStripMenuItem1";
            this.allVideoTracksToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.allVideoTracksToolStripMenuItem1.Text = "All Video Tracks";
            this.allVideoTracksToolStripMenuItem1.Click += new System.EventHandler(this.allVideoTracksToolStripMenuItem1_Click);
            // 
            // uncheckAudioTracksToolStripMenuItem
            // 
            this.uncheckAudioTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAudioTracksToolStripMenuItem1});
            this.uncheckAudioTracksToolStripMenuItem.Name = "uncheckAudioTracksToolStripMenuItem";
            this.uncheckAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckAudioTracksToolStripMenuItem.Text = "Uncheck Audio Tracks...";
            // 
            // allAudioTracksToolStripMenuItem1
            // 
            this.allAudioTracksToolStripMenuItem1.Name = "allAudioTracksToolStripMenuItem1";
            this.allAudioTracksToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.allAudioTracksToolStripMenuItem1.Text = "All Audio Tracks";
            this.allAudioTracksToolStripMenuItem1.Click += new System.EventHandler(this.allAudioTracksToolStripMenuItem1_Click);
            // 
            // uncheckSubtitleTracksToolStripMenuItem
            // 
            this.uncheckSubtitleTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allSubtitleTracksToolStripMenuItem1});
            this.uncheckSubtitleTracksToolStripMenuItem.Name = "uncheckSubtitleTracksToolStripMenuItem";
            this.uncheckSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckSubtitleTracksToolStripMenuItem.Text = "Uncheck Subtitle Tracks...";
            // 
            // allSubtitleTracksToolStripMenuItem1
            // 
            this.allSubtitleTracksToolStripMenuItem1.Name = "allSubtitleTracksToolStripMenuItem1";
            this.allSubtitleTracksToolStripMenuItem1.Size = new System.Drawing.Size(167, 22);
            this.allSubtitleTracksToolStripMenuItem1.Text = "All Subtitle Tracks";
            this.allSubtitleTracksToolStripMenuItem1.Click += new System.EventHandler(this.allSubtitleTracksToolStripMenuItem1_Click);
            // 
            // uncheckChapterTracksToolStripMenuItem
            // 
            this.uncheckChapterTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allChapterTracksToolStripMenuItem1});
            this.uncheckChapterTracksToolStripMenuItem.Name = "uncheckChapterTracksToolStripMenuItem";
            this.uncheckChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckChapterTracksToolStripMenuItem.Text = "Uncheck Chapter Tracks...";
            // 
            // allChapterTracksToolStripMenuItem1
            // 
            this.allChapterTracksToolStripMenuItem1.Name = "allChapterTracksToolStripMenuItem1";
            this.allChapterTracksToolStripMenuItem1.Size = new System.Drawing.Size(169, 22);
            this.allChapterTracksToolStripMenuItem1.Text = "All Chapter Tracks";
            this.allChapterTracksToolStripMenuItem1.Click += new System.EventHandler(this.allChapterTracksToolStripMenuItem1_Click);
            // 
            // uncheckAttachmentTracksToolStripMenuItem
            // 
            this.uncheckAttachmentTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAttachmentTracksToolStripMenuItem1});
            this.uncheckAttachmentTracksToolStripMenuItem.Name = "uncheckAttachmentTracksToolStripMenuItem";
            this.uncheckAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.uncheckAttachmentTracksToolStripMenuItem.Text = "Uncheck Attachment Tracks...";
            // 
            // allAttachmentTracksToolStripMenuItem1
            // 
            this.allAttachmentTracksToolStripMenuItem1.Name = "allAttachmentTracksToolStripMenuItem1";
            this.allAttachmentTracksToolStripMenuItem1.Size = new System.Drawing.Size(190, 22);
            this.allAttachmentTracksToolStripMenuItem1.Text = "All Attachment Tracks";
            this.allAttachmentTracksToolStripMenuItem1.Click += new System.EventHandler(this.allAttachmentTracksToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(228, 6);
            // 
            // removeAllInputFilesToolStripMenuItem
            // 
            this.removeAllInputFilesToolStripMenuItem.Name = "removeAllInputFilesToolStripMenuItem";
            this.removeAllInputFilesToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.removeAllInputFilesToolStripMenuItem.Text = "Remove All Input Files";
            this.removeAllInputFilesToolStripMenuItem.Click += new System.EventHandler(this.removeAllInputFilesToolStripMenuItem_Click);
            // 
            // removeSelectedInputFileToolStripMenuItem
            // 
            this.removeSelectedInputFileToolStripMenuItem.Name = "removeSelectedInputFileToolStripMenuItem";
            this.removeSelectedInputFileToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.removeSelectedInputFileToolStripMenuItem.Text = "Remove Selected Input File";
            this.removeSelectedInputFileToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedInputFileToolStripMenuItem_Click);
            // 
            // openSelectedFileToolStripMenuItem
            // 
            this.openSelectedFileToolStripMenuItem.Name = "openSelectedFileToolStripMenuItem";
            this.openSelectedFileToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openSelectedFileToolStripMenuItem.Text = "Open Selected File...";
            this.openSelectedFileToolStripMenuItem.Click += new System.EventHandler(this.openSelectedFileToolStripMenuItem_Click);
            // 
            // openSelectedFileFolderToolStripMenuItem
            // 
            this.openSelectedFileFolderToolStripMenuItem.Name = "openSelectedFileFolderToolStripMenuItem";
            this.openSelectedFileFolderToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openSelectedFileFolderToolStripMenuItem.Text = "Open Selected File Folder...";
            this.openSelectedFileFolderToolStripMenuItem.Click += new System.EventHandler(this.openSelectedFileFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(228, 6);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // pnlFileOptions
            // 
            this.pnlFileOptions.Controls.Add(this.chkOverwriteExistingFiles);
            this.pnlFileOptions.Controls.Add(this.chkAppendOnDragAndDrop);
            this.pnlFileOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFileOptions.Location = new System.Drawing.Point(0, 192);
            this.pnlFileOptions.Margin = new System.Windows.Forms.Padding(0);
            this.pnlFileOptions.Name = "pnlFileOptions";
            this.pnlFileOptions.Size = new System.Drawing.Size(612, 25);
            this.pnlFileOptions.TabIndex = 1;
            // 
            // chkOverwriteExistingFiles
            // 
            this.chkOverwriteExistingFiles.AutoSize = true;
            this.chkOverwriteExistingFiles.Location = new System.Drawing.Point(203, 3);
            this.chkOverwriteExistingFiles.Name = "chkOverwriteExistingFiles";
            this.chkOverwriteExistingFiles.Size = new System.Drawing.Size(144, 19);
            this.chkOverwriteExistingFiles.TabIndex = 2;
            this.chkOverwriteExistingFiles.Text = "Overwrite existing files";
            this.chkOverwriteExistingFiles.UseVisualStyleBackColor = true;
            this.chkOverwriteExistingFiles.CheckedChanged += new System.EventHandler(this.chkOverwriteExistingFiles_CheckedChanged);
            // 
            // chkAppendOnDragAndDrop
            // 
            this.chkAppendOnDragAndDrop.AutoSize = true;
            this.chkAppendOnDragAndDrop.Location = new System.Drawing.Point(3, 3);
            this.chkAppendOnDragAndDrop.Name = "chkAppendOnDragAndDrop";
            this.chkAppendOnDragAndDrop.Size = new System.Drawing.Size(194, 19);
            this.chkAppendOnDragAndDrop.TabIndex = 1;
            this.chkAppendOnDragAndDrop.Text = "Append input on drag and drop";
            this.chkAppendOnDragAndDrop.UseVisualStyleBackColor = true;
            this.chkAppendOnDragAndDrop.CheckedChanged += new System.EventHandler(this.chkAppendOnDragAndDrop_CheckedChanged);
            // 
            // grpSelectedFileInfo
            // 
            this.grpSelectedFileInfo.AllowDrop = true;
            this.grpSelectedFileInfo.Controls.Add(this.txtSegmentInfo);
            this.grpSelectedFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectedFileInfo.Location = new System.Drawing.Point(3, 308);
            this.grpSelectedFileInfo.Name = "grpSelectedFileInfo";
            this.grpSelectedFileInfo.Size = new System.Drawing.Size(618, 94);
            this.grpSelectedFileInfo.TabIndex = 6;
            this.grpSelectedFileInfo.TabStop = false;
            this.grpSelectedFileInfo.Text = "Selected File Information";
            this.grpSelectedFileInfo.DragDrop += new System.Windows.Forms.DragEventHandler(this.trvInputFiles_DragDrop);
            this.grpSelectedFileInfo.DragEnter += new System.Windows.Forms.DragEventHandler(this.trvInputFiles_DragEnter);
            // 
            // txtSegmentInfo
            // 
            this.txtSegmentInfo.DarkMode = false;
            this.txtSegmentInfo.DetectUrls = false;
            this.txtSegmentInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSegmentInfo.Location = new System.Drawing.Point(3, 19);
            this.txtSegmentInfo.Name = "txtSegmentInfo";
            this.txtSegmentInfo.ReadOnly = true;
            this.txtSegmentInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtSegmentInfo.ShortcutsEnabled = false;
            this.txtSegmentInfo.Size = new System.Drawing.Size(612, 72);
            this.txtSegmentInfo.TabIndex = 1;
            this.txtSegmentInfo.Text = "";
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.Location = new System.Drawing.Point(534, 528);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(72, 30);
            this.btnAbort.TabIndex = 12;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAbortAll
            // 
            this.btnAbortAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbortAll.Location = new System.Drawing.Point(461, 528);
            this.btnAbortAll.Name = "btnAbortAll";
            this.btnAbortAll.Size = new System.Drawing.Size(70, 30);
            this.btnAbortAll.TabIndex = 13;
            this.btnAbortAll.Text = "Abort All";
            this.btnAbortAll.UseVisualStyleBackColor = true;
            this.btnAbortAll.Click += new System.EventHandler(this.btnAbortAll_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.Location = new System.Drawing.Point(377, 528);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(80, 30);
            this.btnOptions.TabIndex = 14;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // chkDarkMode
            // 
            this.chkDarkMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDarkMode.AutoSize = true;
            this.chkDarkMode.Location = new System.Drawing.Point(321, 535);
            this.chkDarkMode.Name = "chkDarkMode";
            this.chkDarkMode.Size = new System.Drawing.Size(50, 19);
            this.chkDarkMode.TabIndex = 15;
            this.chkDarkMode.Text = "Dark";
            this.chkDarkMode.UseVisualStyleBackColor = true;
            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);
            // 
            // frmMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 561);
            this.Controls.Add(this.chkDarkMode);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnAbortAll);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "frmMain2";
            this.Text = "gMKVExtractGUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmMain2_Shown);
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.ClientSizeChanged += new System.EventHandler(this.frmMain_ClientSizeChanged);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.grpOutputDirectory.ResumeLayout(false);
            this.grpOutputDirectory.PerformLayout();
            this.contextMenuStripOutputDirectory.ResumeLayout(false);
            this.grpConfig.ResumeLayout(false);
            this.grpConfig.PerformLayout();
            this.grpInputFiles.ResumeLayout(false);
            this.tlpInput.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.pnlFileOptions.ResumeLayout(false);
            this.pnlFileOptions.PerformLayout();
            this.grpSelectedFileInfo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private gTableLayoutPanel tlpMain;
        private gGroupBox grpConfig;
        private System.Windows.Forms.Button btnBrowseMKVToolnixPath;
        private gTextBox txtMKVToolnixPath;
        private gGroupBox grpInputFiles;
        private gMKVToolNix.gTreeView trvInputFiles;
        private System.Windows.Forms.ToolStripProgressBar prgBrStatus;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnAbortAll;
        private System.Windows.Forms.Button btnOptions;
        private gGroupBox grpSelectedFileInfo;
        private gRichTextBox txtSegmentInfo;
        private gGroupBox grpOutputDirectory;
        private System.Windows.Forms.CheckBox chkUseSourceDirectory;
        private System.Windows.Forms.Button btnBrowseOutputDirectory;
        private gTextBox txtOutputDirectory;
        private gGroupBox grpActions;
        private System.Windows.Forms.CheckBox chkShowPopup;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label lblExtractionMode;
        private gComboBox cmbExtractionMode;
        private System.Windows.Forms.Button btnShowLog;
        private System.Windows.Forms.Label lblChapterType;
        private gComboBox cmbChapterType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.Button btnShowJobs;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem checkTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkVideoTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAudioTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkSubtitleTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkChapterTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAttachmentTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uncheckTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeAllInputFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedInputFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addInputFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckVideoTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAudioTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckSubtitleTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckChapterTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAttachmentTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripProgressBar prgBrTotalStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.Button btnAddJobs;
        private System.Windows.Forms.ToolStripMenuItem allVideoTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allAudioTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allSubtitleTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allChapterTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allAttachmentTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allVideoTracksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allAudioTracksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allSubtitleTracksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allChapterTracksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allAttachmentTracksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openSelectedFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSelectedFileFolderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOutputDirectory;
        private System.Windows.Forms.ToolStripMenuItem setAsDefaultDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useCurrentlySetDefaultDirectoryToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpInput;
        private System.Windows.Forms.CheckBox chkAppendOnDragAndDrop;
        private System.Windows.Forms.CheckBox chkDarkMode;
        private System.Windows.Forms.Button btnAutoDetectMkvToolnix;
        private System.Windows.Forms.Panel pnlFileOptions;
        private System.Windows.Forms.CheckBox chkOverwriteExistingFiles;
    }
}