using gMKVToolNix.Controls;

namespace gMKVToolNix
{
    partial class frmMain
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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllVideoTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllAudioTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllSubtitleTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllChapterTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllAttachmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.unselectAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAbortAll = new System.Windows.Forms.Button();
            this.tlpMain = new gMKVToolNix.gTableLayoutPanel();
            this.grpInputFile = new gMKVToolNix.gGroupBox();
            this.txtInputFile = new gMKVToolNix.gTextBox();
            this.btnBrowseInputFile = new System.Windows.Forms.Button();
            this.grpOutputDirectory = new gMKVToolNix.gGroupBox();
            this.chkLockOutputDirectory = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutputDirectory = new System.Windows.Forms.Button();
            this.txtOutputDirectory = new gMKVToolNix.gTextBox();
            this.grpActions = new gMKVToolNix.gGroupBox();
            this.chkShowPopup = new System.Windows.Forms.CheckBox();
            this.chkJobMode = new System.Windows.Forms.CheckBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.lblExtractionMode = new System.Windows.Forms.Label();
            this.cmbExtractionMode = new gMKVToolNix.Controls.gComboBox();
            this.btnShowLog = new System.Windows.Forms.Button();
            this.lblChapterType = new System.Windows.Forms.Label();
            this.cmbChapterType = new gMKVToolNix.Controls.gComboBox();
            this.grpConfig = new gMKVToolNix.gGroupBox();
            this.btnBrowseMKVToolnixPath = new System.Windows.Forms.Button();
            this.txtMKVToolnixPath = new gMKVToolNix.gTextBox();
            this.grpInputFileInfo = new gMKVToolNix.gGroupBox();
            this.tableLayoutPanel1 = new gMKVToolNix.gTableLayoutPanel();
            this.chkLstInputFileTracks = new gMKVToolNix.gCheckedListBox();
            this.txtSegmentInfo = new gMKVToolNix.gRichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.prgBrStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.lblTrack = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpLog = new gMKVToolNix.gGroupBox();
            this.contextMenuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.grpInputFile.SuspendLayout();
            this.grpOutputDirectory.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpConfig.SuspendLayout();
            this.grpInputFileInfo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllTracksToolStripMenuItem,
            this.selectAllVideoTracksToolStripMenuItem,
            this.selectAllAudioTracksToolStripMenuItem,
            this.selectAllSubtitleTracksToolStripMenuItem,
            this.selectAllChapterTracksToolStripMenuItem,
            this.selectAllAttachmentsToolStripMenuItem,
            this.toolStripSeparator1,
            this.unselectAllTracksToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(230, 164);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // selectAllTracksToolStripMenuItem
            // 
            this.selectAllTracksToolStripMenuItem.Name = "selectAllTracksToolStripMenuItem";
            this.selectAllTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllTracksToolStripMenuItem.Text = "Select All Tracks";
            this.selectAllTracksToolStripMenuItem.Click += new System.EventHandler(this.selectAllTracksToolStripMenuItem_Click);
            // 
            // selectAllVideoTracksToolStripMenuItem
            // 
            this.selectAllVideoTracksToolStripMenuItem.Name = "selectAllVideoTracksToolStripMenuItem";
            this.selectAllVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllVideoTracksToolStripMenuItem.Text = "Select All Video Tracks";
            this.selectAllVideoTracksToolStripMenuItem.Click += new System.EventHandler(this.selectAllVideoTracksToolStripMenuItem_Click);
            // 
            // selectAllAudioTracksToolStripMenuItem
            // 
            this.selectAllAudioTracksToolStripMenuItem.Name = "selectAllAudioTracksToolStripMenuItem";
            this.selectAllAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllAudioTracksToolStripMenuItem.Text = "Select All Audio Tracks";
            this.selectAllAudioTracksToolStripMenuItem.Click += new System.EventHandler(this.selectAllAudioTracksToolStripMenuItem_Click);
            // 
            // selectAllSubtitleTracksToolStripMenuItem
            // 
            this.selectAllSubtitleTracksToolStripMenuItem.Name = "selectAllSubtitleTracksToolStripMenuItem";
            this.selectAllSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllSubtitleTracksToolStripMenuItem.Text = "Select All Subtitle Tracks";
            this.selectAllSubtitleTracksToolStripMenuItem.Click += new System.EventHandler(this.selectAllSubtitleTracksToolStripMenuItem_Click);
            // 
            // selectAllChapterTracksToolStripMenuItem
            // 
            this.selectAllChapterTracksToolStripMenuItem.Name = "selectAllChapterTracksToolStripMenuItem";
            this.selectAllChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllChapterTracksToolStripMenuItem.Text = "Select All Chapter Tracks";
            this.selectAllChapterTracksToolStripMenuItem.Click += new System.EventHandler(this.selectAllChapterTracksToolStripMenuItem_Click);
            // 
            // selectAllAttachmentsToolStripMenuItem
            // 
            this.selectAllAttachmentsToolStripMenuItem.Name = "selectAllAttachmentsToolStripMenuItem";
            this.selectAllAttachmentsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.selectAllAttachmentsToolStripMenuItem.Text = "Select All Attachments Tracks";
            this.selectAllAttachmentsToolStripMenuItem.Click += new System.EventHandler(this.selectAllAttachmentsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(226, 6);
            // 
            // unselectAllTracksToolStripMenuItem
            // 
            this.unselectAllTracksToolStripMenuItem.Name = "unselectAllTracksToolStripMenuItem";
            this.unselectAllTracksToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.unselectAllTracksToolStripMenuItem.Text = "Unselect All tracks";
            this.unselectAllTracksToolStripMenuItem.Click += new System.EventHandler(this.unselectAllTracksToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnAbort);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnAbortAll);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tlpMain);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(624, 561);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(624, 561);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.Location = new System.Drawing.Point(512, 528);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(92, 30);
            this.btnAbort.TabIndex = 10;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAbortAll
            // 
            this.btnAbortAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbortAll.Location = new System.Drawing.Point(414, 528);
            this.btnAbortAll.Name = "btnAbortAll";
            this.btnAbortAll.Size = new System.Drawing.Size(92, 30);
            this.btnAbortAll.TabIndex = 11;
            this.btnAbortAll.Text = "Abort All";
            this.btnAbortAll.UseVisualStyleBackColor = true;
            this.btnAbortAll.Click += new System.EventHandler(this.btnAbortAll_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpInputFile, 0, 0);
            this.tlpMain.Controls.Add(this.grpOutputDirectory, 0, 1);
            this.tlpMain.Controls.Add(this.grpActions, 0, 4);
            this.tlpMain.Controls.Add(this.grpConfig, 0, 2);
            this.tlpMain.Controls.Add(this.grpInputFileInfo, 0, 3);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.Size = new System.Drawing.Size(624, 525);
            this.tlpMain.TabIndex = 1;
            // 
            // grpInputFile
            // 
            this.grpInputFile.AllowDrop = true;
            this.grpInputFile.Controls.Add(this.txtInputFile);
            this.grpInputFile.Controls.Add(this.btnBrowseInputFile);
            this.grpInputFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInputFile.Location = new System.Drawing.Point(3, 3);
            this.grpInputFile.Name = "grpInputFile";
            this.grpInputFile.Size = new System.Drawing.Size(618, 54);
            this.grpInputFile.TabIndex = 3;
            this.grpInputFile.TabStop = false;
            this.grpInputFile.Text = "Input file (you can drag and drop the file)";
            this.grpInputFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpInputFile_DragDrop);
            this.grpInputFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.grpInputFile_DragEnter);
            // 
            // txtInputFile
            // 
            this.txtInputFile.AllowDrop = true;
            this.txtInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFile.Location = new System.Drawing.Point(6, 22);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.ReadOnly = true;
            this.txtInputFile.ShortcutsEnabled = false;
            this.txtInputFile.Size = new System.Drawing.Size(519, 23);
            this.txtInputFile.TabIndex = 1;
            this.txtInputFile.WordWrap = false;
            this.txtInputFile.TextChanged += new System.EventHandler(this.txtInputFile_TextChanged);
            this.txtInputFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtInputFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // btnBrowseInputFile
            // 
            this.btnBrowseInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInputFile.Location = new System.Drawing.Point(531, 17);
            this.btnBrowseInputFile.Name = "btnBrowseInputFile";
            this.btnBrowseInputFile.Size = new System.Drawing.Size(80, 30);
            this.btnBrowseInputFile.TabIndex = 2;
            this.btnBrowseInputFile.Text = "Browse...";
            this.btnBrowseInputFile.UseVisualStyleBackColor = true;
            this.btnBrowseInputFile.Click += new System.EventHandler(this.btnBrowseInputFile_Click);
            // 
            // grpOutputDirectory
            // 
            this.grpOutputDirectory.Controls.Add(this.chkLockOutputDirectory);
            this.grpOutputDirectory.Controls.Add(this.btnBrowseOutputDirectory);
            this.grpOutputDirectory.Controls.Add(this.txtOutputDirectory);
            this.grpOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOutputDirectory.Location = new System.Drawing.Point(3, 63);
            this.grpOutputDirectory.Name = "grpOutputDirectory";
            this.grpOutputDirectory.Size = new System.Drawing.Size(618, 54);
            this.grpOutputDirectory.TabIndex = 4;
            this.grpOutputDirectory.TabStop = false;
            this.grpOutputDirectory.Text = "Output Directory";
            // 
            // chkLockOutputDirectory
            // 
            this.chkLockOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLockOutputDirectory.AutoSize = true;
            this.chkLockOutputDirectory.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLockOutputDirectory.Location = new System.Drawing.Point(474, 24);
            this.chkLockOutputDirectory.Name = "chkLockOutputDirectory";
            this.chkLockOutputDirectory.Size = new System.Drawing.Size(51, 19);
            this.chkLockOutputDirectory.TabIndex = 4;
            this.chkLockOutputDirectory.Text = "Lock";
            this.chkLockOutputDirectory.UseVisualStyleBackColor = true;
            this.chkLockOutputDirectory.CheckedChanged += new System.EventHandler(this.chkLockOutputDirectory_CheckedChanged);
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
            this.txtOutputDirectory.Location = new System.Drawing.Point(6, 22);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.ShortcutsEnabled = false;
            this.txtOutputDirectory.Size = new System.Drawing.Size(462, 23);
            this.txtOutputDirectory.TabIndex = 2;
            this.txtOutputDirectory.WordWrap = false;
            this.txtOutputDirectory.TextChanged += new System.EventHandler(this.txtOutputDirectory_TextChanged);
            this.txtOutputDirectory.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtOutputDirectory.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.chkShowPopup);
            this.grpActions.Controls.Add(this.chkJobMode);
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
            this.grpActions.TabIndex = 6;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // chkShowPopup
            // 
            this.chkShowPopup.AutoSize = true;
            this.chkShowPopup.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowPopup.Location = new System.Drawing.Point(158, 24);
            this.chkShowPopup.Name = "chkShowPopup";
            this.chkShowPopup.Size = new System.Drawing.Size(61, 19);
            this.chkShowPopup.TabIndex = 12;
            this.chkShowPopup.Text = "Popup";
            this.chkShowPopup.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowPopup.UseVisualStyleBackColor = true;
            this.chkShowPopup.CheckedChanged += new System.EventHandler(this.chkShowPopup_CheckedChanged);
            // 
            // chkJobMode
            // 
            this.chkJobMode.AutoSize = true;
            this.chkJobMode.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkJobMode.Location = new System.Drawing.Point(72, 24);
            this.chkJobMode.Name = "chkJobMode";
            this.chkJobMode.Size = new System.Drawing.Size(78, 19);
            this.chkJobMode.TabIndex = 11;
            this.chkJobMode.Text = "Job Mode";
            this.chkJobMode.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkJobMode.UseVisualStyleBackColor = true;
            this.chkJobMode.CheckedChanged += new System.EventHandler(this.chkJobMode_CheckedChanged);
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(521, 18);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(90, 30);
            this.btnExtract.TabIndex = 10;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // lblExtractionMode
            // 
            this.lblExtractionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExtractionMode.AutoSize = true;
            this.lblExtractionMode.Location = new System.Drawing.Point(360, 26);
            this.lblExtractionMode.Name = "lblExtractionMode";
            this.lblExtractionMode.Size = new System.Drawing.Size(45, 15);
            this.lblExtractionMode.TabIndex = 9;
            this.lblExtractionMode.Text = "Extract:";
            // 
            // cmbExtractionMode
            // 
            this.cmbExtractionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbExtractionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtractionMode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbExtractionMode.FormattingEnabled = true;
            this.cmbExtractionMode.Location = new System.Drawing.Point(411, 22);
            this.cmbExtractionMode.Name = "cmbExtractionMode";
            this.cmbExtractionMode.Size = new System.Drawing.Size(102, 23);
            this.cmbExtractionMode.TabIndex = 8;
            // 
            // btnShowLog
            // 
            this.btnShowLog.Location = new System.Drawing.Point(6, 18);
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
            this.lblChapterType.Location = new System.Drawing.Point(228, 26);
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
            this.cmbChapterType.Location = new System.Drawing.Point(281, 22);
            this.cmbChapterType.Name = "cmbChapterType";
            this.cmbChapterType.Size = new System.Drawing.Size(52, 23);
            this.cmbChapterType.TabIndex = 2;
            this.cmbChapterType.SelectedIndexChanged += new System.EventHandler(this.cmbChapterType_SelectedIndexChanged);
            // 
            // grpConfig
            // 
            this.grpConfig.Controls.Add(this.btnBrowseMKVToolnixPath);
            this.grpConfig.Controls.Add(this.txtMKVToolnixPath);
            this.grpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConfig.Location = new System.Drawing.Point(3, 123);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Size = new System.Drawing.Size(618, 54);
            this.grpConfig.TabIndex = 7;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "MKVToolnix Directory (you can drag and drop the directory)";
            // 
            // btnBrowseMKVToolnixPath
            // 
            this.btnBrowseMKVToolnixPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMKVToolnixPath.Location = new System.Drawing.Point(531, 18);
            this.btnBrowseMKVToolnixPath.Name = "btnBrowseMKVToolnixPath";
            this.btnBrowseMKVToolnixPath.Size = new System.Drawing.Size(80, 30);
            this.btnBrowseMKVToolnixPath.TabIndex = 5;
            this.btnBrowseMKVToolnixPath.Text = "Browse...";
            this.btnBrowseMKVToolnixPath.UseVisualStyleBackColor = true;
            this.btnBrowseMKVToolnixPath.Click += new System.EventHandler(this.btnBrowseMKVToolnixPath_Click);
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
            this.txtMKVToolnixPath.Size = new System.Drawing.Size(519, 23);
            this.txtMKVToolnixPath.TabIndex = 3;
            this.txtMKVToolnixPath.WordWrap = false;
            this.txtMKVToolnixPath.TextChanged += new System.EventHandler(this.txtMKVToolnixPath_TextChanged);
            this.txtMKVToolnixPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtMKVToolnixPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // grpInputFileInfo
            // 
            this.grpInputFileInfo.AllowDrop = true;
            this.grpInputFileInfo.Controls.Add(this.tableLayoutPanel1);
            this.grpInputFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInputFileInfo.Location = new System.Drawing.Point(3, 183);
            this.grpInputFileInfo.Name = "grpInputFileInfo";
            this.grpInputFileInfo.Size = new System.Drawing.Size(618, 279);
            this.grpInputFileInfo.TabIndex = 5;
            this.grpInputFileInfo.TabStop = false;
            this.grpInputFileInfo.Text = "Input File Information";
            this.grpInputFileInfo.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpInputFileInfo_DragDrop);
            this.grpInputFileInfo.DragEnter += new System.Windows.Forms.DragEventHandler(this.grpInputFile_DragEnter);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.chkLstInputFileTracks, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtSegmentInfo, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(612, 257);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // chkLstInputFileTracks
            // 
            this.chkLstInputFileTracks.CheckOnClick = true;
            this.chkLstInputFileTracks.ContextMenuStrip = this.contextMenuStrip;
            this.chkLstInputFileTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkLstInputFileTracks.FormattingEnabled = true;
            this.chkLstInputFileTracks.HorizontalScrollbar = true;
            this.chkLstInputFileTracks.Location = new System.Drawing.Point(3, 83);
            this.chkLstInputFileTracks.Name = "chkLstInputFileTracks";
            this.chkLstInputFileTracks.Size = new System.Drawing.Size(606, 171);
            this.chkLstInputFileTracks.TabIndex = 0;
            // 
            // txtSegmentInfo
            // 
            this.txtSegmentInfo.DetectUrls = false;
            this.txtSegmentInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSegmentInfo.Location = new System.Drawing.Point(3, 3);
            this.txtSegmentInfo.Name = "txtSegmentInfo";
            this.txtSegmentInfo.ReadOnly = true;
            this.txtSegmentInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtSegmentInfo.ShortcutsEnabled = false;
            this.txtSegmentInfo.Size = new System.Drawing.Size(606, 74);
            this.txtSegmentInfo.TabIndex = 1;
            this.txtSegmentInfo.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prgBrStatus,
            this.lblTrack,
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 525);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 36);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // prgBrStatus
            // 
            this.prgBrStatus.AutoSize = false;
            this.prgBrStatus.Name = "prgBrStatus";
            this.prgBrStatus.Size = new System.Drawing.Size(200, 30);
            // 
            // lblTrack
            // 
            this.lblTrack.Name = "lblTrack";
            this.lblTrack.Size = new System.Drawing.Size(33, 31);
            this.lblTrack.Text = "track";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 31);
            this.lblStatus.Text = "status";
            // 
            // grpLog
            // 
            this.grpLog.Location = new System.Drawing.Point(14, 606);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(755, 106);
            this.grpLog.TabIndex = 8;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 561);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.grpLog);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "frmMain";
            this.Text = "gMKVExtractGUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.ClientSizeChanged += new System.EventHandler(this.frmMain_ClientSizeChanged);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.grpInputFile.ResumeLayout(false);
            this.grpInputFile.PerformLayout();
            this.grpOutputDirectory.ResumeLayout(false);
            this.grpOutputDirectory.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.grpConfig.ResumeLayout(false);
            this.grpConfig.PerformLayout();
            this.grpInputFileInfo.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private gTextBox txtInputFile;
        private System.Windows.Forms.Button btnBrowseInputFile;
        private gGroupBox grpInputFile;
        private gGroupBox grpOutputDirectory;
        private System.Windows.Forms.Button btnBrowseOutputDirectory;
        private gTextBox txtOutputDirectory;
        private gGroupBox grpInputFileInfo;
        private gCheckedListBox chkLstInputFileTracks;
        private gGroupBox grpActions;
        private gGroupBox grpConfig;
        private System.Windows.Forms.Button btnBrowseMKVToolnixPath;
        private gTextBox txtMKVToolnixPath;
        private gGroupBox grpLog;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private gTableLayoutPanel tlpMain;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar prgBrStatus;
        private gTableLayoutPanel tableLayoutPanel1;
        private gRichTextBox txtSegmentInfo;
        private System.Windows.Forms.Label lblChapterType;
        private gComboBox cmbChapterType;
        private System.Windows.Forms.ToolStripStatusLabel lblTrack;
        private System.Windows.Forms.Button btnShowLog;
        private System.Windows.Forms.CheckBox chkLockOutputDirectory;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnAbortAll;
        private System.Windows.Forms.Label lblExtractionMode;
        private gComboBox cmbExtractionMode;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem selectAllTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllVideoTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllAudioTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllSubtitleTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllChapterTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllAttachmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem unselectAllTracksToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkJobMode;
        private System.Windows.Forms.CheckBox chkShowPopup;
    }
}

