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
            this.tlpActions = new System.Windows.Forms.TableLayoutPanel();
            this.flpActionsLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.btnShowLog = new System.Windows.Forms.Button();
            this.btnShowJobs = new System.Windows.Forms.Button();
            this.chkShowPopup = new System.Windows.Forms.CheckBox();
            this.flpActionsRight = new System.Windows.Forms.FlowLayoutPanel();
            this.lblChapterType = new System.Windows.Forms.Label();
            this.cmbChapterType = new gMKVToolNix.Controls.gComboBox();
            this.cmbExtractionMode = new gMKVToolNix.Controls.gComboBox();
            this.lblExtractionMode = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnAddJobs = new System.Windows.Forms.Button();
            this.grpOutputDirectory = new gMKVToolNix.gGroupBox();
            this.tlpOutputDirectory = new System.Windows.Forms.TableLayoutPanel();
            this.chkUseSourceDirectory = new System.Windows.Forms.CheckBox();
            this.txtOutputDirectory = new gMKVToolNix.gTextBox();
            this.contextMenuStripOutputDirectory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setAsDefaultDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBrowseOutputDirectory = new System.Windows.Forms.Button();
            this.grpConfig = new gMKVToolNix.gGroupBox();
            this.tlpConfig = new System.Windows.Forms.TableLayoutPanel();
            this.txtMKVToolnixPath = new gMKVToolNix.gTextBox();
            this.btnAutoDetectMkvToolnix = new System.Windows.Forms.Button();
            this.btnBrowseMKVToolnixPath = new System.Windows.Forms.Button();
            this.grpInputFiles = new gMKVToolNix.gGroupBox();
            this.tlpInput = new System.Windows.Forms.TableLayoutPanel();
            this.flpFileOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.chkAppendOnDragAndDrop = new System.Windows.Forms.CheckBox();
            this.chkOverwriteExistingFiles = new System.Windows.Forms.CheckBox();
            this.chkDisableTooltips = new System.Windows.Forms.CheckBox();
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.grpSelectedFileInfo = new gMKVToolNix.gGroupBox();
            this.txtSegmentInfo = new gMKVToolNix.gRichTextBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAbortAll = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.chkDarkMode = new System.Windows.Forms.CheckBox();
            this.flpFrmOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.statusStrip.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.tlpActions.SuspendLayout();
            this.flpActionsLeft.SuspendLayout();
            this.flpActionsRight.SuspendLayout();
            this.grpOutputDirectory.SuspendLayout();
            this.tlpOutputDirectory.SuspendLayout();
            this.contextMenuStripOutputDirectory.SuspendLayout();
            this.grpConfig.SuspendLayout();
            this.tlpConfig.SuspendLayout();
            this.grpInputFiles.SuspendLayout();
            this.tlpInput.SuspendLayout();
            this.flpFileOptions.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.grpSelectedFileInfo.SuspendLayout();
            this.flpFrmOptions.SuspendLayout();
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
            this.grpActions.Controls.Add(this.tlpActions);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 468);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(618, 54);
            this.grpActions.TabIndex = 8;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // tlpActions
            // 
            this.tlpActions.ColumnCount = 3;
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpActions.Controls.Add(this.flpActionsLeft, 0, 0);
            this.tlpActions.Controls.Add(this.flpActionsRight, 2, 0);
            this.tlpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActions.Location = new System.Drawing.Point(3, 19);
            this.tlpActions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpActions.Name = "tlpActions";
            this.tlpActions.RowCount = 1;
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.Size = new System.Drawing.Size(612, 32);
            this.tlpActions.TabIndex = 2;
            // 
            // flpActionsLeft
            // 
            this.flpActionsLeft.AutoSize = true;
            this.flpActionsLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpActionsLeft.Controls.Add(this.btnShowLog);
            this.flpActionsLeft.Controls.Add(this.btnShowJobs);
            this.flpActionsLeft.Controls.Add(this.chkShowPopup);
            this.flpActionsLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpActionsLeft.Location = new System.Drawing.Point(0, 0);
            this.flpActionsLeft.Margin = new System.Windows.Forms.Padding(0);
            this.flpActionsLeft.Name = "flpActionsLeft";
            this.flpActionsLeft.Padding = new System.Windows.Forms.Padding(3);
            this.flpActionsLeft.Size = new System.Drawing.Size(176, 32);
            this.flpActionsLeft.TabIndex = 0;
            this.flpActionsLeft.WrapContents = false;
            // 
            // btnShowLog
            // 
            this.btnShowLog.AutoSize = true;
            this.btnShowLog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnShowLog.Location = new System.Drawing.Point(3, 3);
            this.btnShowLog.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowLog.Name = "btnShowLog";
            this.btnShowLog.Padding = new System.Windows.Forms.Padding(2);
            this.btnShowLog.Size = new System.Drawing.Size(50, 29);
            this.btnShowLog.TabIndex = 6;
            this.btnShowLog.Text = "Log...";
            this.btnShowLog.UseVisualStyleBackColor = true;
            this.btnShowLog.Click += new System.EventHandler(this.btnShowLog_Click);
            // 
            // btnShowJobs
            // 
            this.btnShowJobs.AutoSize = true;
            this.btnShowJobs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnShowJobs.Location = new System.Drawing.Point(53, 3);
            this.btnShowJobs.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowJobs.Name = "btnShowJobs";
            this.btnShowJobs.Padding = new System.Windows.Forms.Padding(2);
            this.btnShowJobs.Size = new System.Drawing.Size(53, 29);
            this.btnShowJobs.TabIndex = 13;
            this.btnShowJobs.Text = "Jobs...";
            this.btnShowJobs.UseVisualStyleBackColor = true;
            this.btnShowJobs.Click += new System.EventHandler(this.btnShowJobs_Click);
            // 
            // chkShowPopup
            // 
            this.chkShowPopup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkShowPopup.AutoSize = true;
            this.chkShowPopup.Location = new System.Drawing.Point(109, 8);
            this.chkShowPopup.Name = "chkShowPopup";
            this.chkShowPopup.Size = new System.Drawing.Size(61, 19);
            this.chkShowPopup.TabIndex = 12;
            this.chkShowPopup.Text = "Popup";
            this.chkShowPopup.UseVisualStyleBackColor = true;
            this.chkShowPopup.CheckedChanged += new System.EventHandler(this.chkShowPopup_CheckedChanged);
            // 
            // flpActionsRight
            // 
            this.flpActionsRight.AutoSize = true;
            this.flpActionsRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpActionsRight.Controls.Add(this.lblChapterType);
            this.flpActionsRight.Controls.Add(this.cmbChapterType);
            this.flpActionsRight.Controls.Add(this.cmbExtractionMode);
            this.flpActionsRight.Controls.Add(this.lblExtractionMode);
            this.flpActionsRight.Controls.Add(this.btnExtract);
            this.flpActionsRight.Controls.Add(this.btnAddJobs);
            this.flpActionsRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpActionsRight.Location = new System.Drawing.Point(224, 0);
            this.flpActionsRight.Margin = new System.Windows.Forms.Padding(0);
            this.flpActionsRight.Name = "flpActionsRight";
            this.flpActionsRight.Padding = new System.Windows.Forms.Padding(3);
            this.flpActionsRight.Size = new System.Drawing.Size(388, 32);
            this.flpActionsRight.TabIndex = 1;
            this.flpActionsRight.WrapContents = false;
            // 
            // lblChapterType
            // 
            this.lblChapterType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblChapterType.AutoSize = true;
            this.lblChapterType.Location = new System.Drawing.Point(3, 10);
            this.lblChapterType.Margin = new System.Windows.Forms.Padding(0);
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
            this.cmbChapterType.Location = new System.Drawing.Point(55, 6);
            this.cmbChapterType.Name = "cmbChapterType";
            this.cmbChapterType.Size = new System.Drawing.Size(52, 23);
            this.cmbChapterType.TabIndex = 2;
            this.cmbChapterType.SelectedIndexChanged += new System.EventHandler(this.cmbChapterType_SelectedIndexChanged);
            // 
            // cmbExtractionMode
            // 
            this.cmbExtractionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbExtractionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtractionMode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbExtractionMode.FormattingEnabled = true;
            this.cmbExtractionMode.Location = new System.Drawing.Point(113, 6);
            this.cmbExtractionMode.Name = "cmbExtractionMode";
            this.cmbExtractionMode.Size = new System.Drawing.Size(102, 23);
            this.cmbExtractionMode.TabIndex = 8;
            // 
            // lblExtractionMode
            // 
            this.lblExtractionMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblExtractionMode.AutoSize = true;
            this.lblExtractionMode.Location = new System.Drawing.Point(218, 10);
            this.lblExtractionMode.Margin = new System.Windows.Forms.Padding(0);
            this.lblExtractionMode.Name = "lblExtractionMode";
            this.lblExtractionMode.Size = new System.Drawing.Size(42, 15);
            this.lblExtractionMode.TabIndex = 9;
            this.lblExtractionMode.Text = "Extract";
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.AutoSize = true;
            this.btnExtract.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExtract.Location = new System.Drawing.Point(260, 3);
            this.btnExtract.Margin = new System.Windows.Forms.Padding(0);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Padding = new System.Windows.Forms.Padding(2);
            this.btnExtract.Size = new System.Drawing.Size(56, 29);
            this.btnExtract.TabIndex = 10;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_btnAddJobs_Click);
            // 
            // btnAddJobs
            // 
            this.btnAddJobs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddJobs.AutoSize = true;
            this.btnAddJobs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddJobs.Location = new System.Drawing.Point(316, 3);
            this.btnAddJobs.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddJobs.Name = "btnAddJobs";
            this.btnAddJobs.Padding = new System.Windows.Forms.Padding(2);
            this.btnAddJobs.Size = new System.Drawing.Size(69, 29);
            this.btnAddJobs.TabIndex = 14;
            this.btnAddJobs.Text = "Add Jobs";
            this.btnAddJobs.UseVisualStyleBackColor = true;
            this.btnAddJobs.Click += new System.EventHandler(this.btnExtract_btnAddJobs_Click);
            // 
            // grpOutputDirectory
            // 
            this.grpOutputDirectory.Controls.Add(this.tlpOutputDirectory);
            this.grpOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOutputDirectory.Location = new System.Drawing.Point(3, 408);
            this.grpOutputDirectory.Name = "grpOutputDirectory";
            this.grpOutputDirectory.Size = new System.Drawing.Size(618, 54);
            this.grpOutputDirectory.TabIndex = 7;
            this.grpOutputDirectory.TabStop = false;
            this.grpOutputDirectory.Text = "Output Directory for Selected File (you can drag and drop the directory)";
            // 
            // tlpOutputDirectory
            // 
            this.tlpOutputDirectory.ColumnCount = 3;
            this.tlpOutputDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOutputDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOutputDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOutputDirectory.Controls.Add(this.chkUseSourceDirectory, 2, 0);
            this.tlpOutputDirectory.Controls.Add(this.txtOutputDirectory, 0, 0);
            this.tlpOutputDirectory.Controls.Add(this.btnBrowseOutputDirectory, 1, 0);
            this.tlpOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOutputDirectory.Location = new System.Drawing.Point(3, 19);
            this.tlpOutputDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.tlpOutputDirectory.Name = "tlpOutputDirectory";
            this.tlpOutputDirectory.Padding = new System.Windows.Forms.Padding(3);
            this.tlpOutputDirectory.RowCount = 1;
            this.tlpOutputDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOutputDirectory.Size = new System.Drawing.Size(612, 32);
            this.tlpOutputDirectory.TabIndex = 2;
            // 
            // chkUseSourceDirectory
            // 
            this.chkUseSourceDirectory.AutoSize = true;
            this.chkUseSourceDirectory.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseSourceDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkUseSourceDirectory.Location = new System.Drawing.Point(522, 6);
            this.chkUseSourceDirectory.Name = "chkUseSourceDirectory";
            this.chkUseSourceDirectory.Size = new System.Drawing.Size(84, 20);
            this.chkUseSourceDirectory.TabIndex = 4;
            this.chkUseSourceDirectory.Text = "Use Source";
            this.chkUseSourceDirectory.UseVisualStyleBackColor = true;
            this.chkUseSourceDirectory.CheckedChanged += new System.EventHandler(this.chkUseSourceDirectory_CheckedChanged);
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.AllowDrop = true;
            this.txtOutputDirectory.ContextMenuStrip = this.contextMenuStripOutputDirectory;
            this.txtOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutputDirectory.Location = new System.Drawing.Point(6, 6);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.ShortcutsEnabled = false;
            this.txtOutputDirectory.Size = new System.Drawing.Size(444, 23);
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
            this.contextMenuStripOutputDirectory.Size = new System.Drawing.Size(282, 48);
            this.contextMenuStripOutputDirectory.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripOutputDirectory_Opening);
            // 
            // setAsDefaultDirectoryToolStripMenuItem
            // 
            this.setAsDefaultDirectoryToolStripMenuItem.Name = "setAsDefaultDirectoryToolStripMenuItem";
            this.setAsDefaultDirectoryToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.setAsDefaultDirectoryToolStripMenuItem.Text = "Set As Default Directory";
            this.setAsDefaultDirectoryToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultDirectoryToolStripMenuItem_Click);
            // 
            // useCurrentlySetDefaultDirectoryToolStripMenuItem
            // 
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Enabled = false;
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Name = "useCurrentlySetDefaultDirectoryToolStripMenuItem";
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Text = "Use Currently Set Default Directory:";
            this.useCurrentlySetDefaultDirectoryToolStripMenuItem.Click += new System.EventHandler(this.useCurrentlySetDefaultDirectoryToolStripMenuItem_Click);
            // 
            // btnBrowseOutputDirectory
            // 
            this.btnBrowseOutputDirectory.AutoSize = true;
            this.btnBrowseOutputDirectory.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowseOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseOutputDirectory.Location = new System.Drawing.Point(453, 3);
            this.btnBrowseOutputDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowseOutputDirectory.Name = "btnBrowseOutputDirectory";
            this.btnBrowseOutputDirectory.Padding = new System.Windows.Forms.Padding(1);
            this.btnBrowseOutputDirectory.Size = new System.Drawing.Size(66, 26);
            this.btnBrowseOutputDirectory.TabIndex = 3;
            this.btnBrowseOutputDirectory.Text = "Browse...";
            this.btnBrowseOutputDirectory.UseVisualStyleBackColor = true;
            this.btnBrowseOutputDirectory.Click += new System.EventHandler(this.btnBrowseOutputDirectory_Click);
            // 
            // grpConfig
            // 
            this.grpConfig.Controls.Add(this.tlpConfig);
            this.grpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConfig.Location = new System.Drawing.Point(3, 3);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Size = new System.Drawing.Size(618, 54);
            this.grpConfig.TabIndex = 0;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "MKVToolnix Directory (you can drag and drop the directory)";
            // 
            // tlpConfig
            // 
            this.tlpConfig.ColumnCount = 3;
            this.tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpConfig.Controls.Add(this.txtMKVToolnixPath, 0, 0);
            this.tlpConfig.Controls.Add(this.btnAutoDetectMkvToolnix, 2, 0);
            this.tlpConfig.Controls.Add(this.btnBrowseMKVToolnixPath, 1, 0);
            this.tlpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpConfig.Location = new System.Drawing.Point(3, 19);
            this.tlpConfig.Margin = new System.Windows.Forms.Padding(0);
            this.tlpConfig.Name = "tlpConfig";
            this.tlpConfig.RowCount = 1;
            this.tlpConfig.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConfig.Size = new System.Drawing.Size(612, 32);
            this.tlpConfig.TabIndex = 9;
            // 
            // txtMKVToolnixPath
            // 
            this.txtMKVToolnixPath.AllowDrop = true;
            this.txtMKVToolnixPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMKVToolnixPath.Location = new System.Drawing.Point(3, 3);
            this.txtMKVToolnixPath.Name = "txtMKVToolnixPath";
            this.txtMKVToolnixPath.ReadOnly = true;
            this.txtMKVToolnixPath.ShortcutsEnabled = false;
            this.txtMKVToolnixPath.Size = new System.Drawing.Size(438, 23);
            this.txtMKVToolnixPath.TabIndex = 7;
            this.txtMKVToolnixPath.WordWrap = false;
            this.txtMKVToolnixPath.TextChanged += new System.EventHandler(this.txtMKVToolnixPath_TextChanged);
            this.txtMKVToolnixPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_DragDrop);
            this.txtMKVToolnixPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txt_DragEnter);
            // 
            // btnAutoDetectMkvToolnix
            // 
            this.btnAutoDetectMkvToolnix.AutoSize = true;
            this.btnAutoDetectMkvToolnix.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAutoDetectMkvToolnix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAutoDetectMkvToolnix.Location = new System.Drawing.Point(523, 3);
            this.btnAutoDetectMkvToolnix.Name = "btnAutoDetectMkvToolnix";
            this.btnAutoDetectMkvToolnix.Padding = new System.Windows.Forms.Padding(3);
            this.btnAutoDetectMkvToolnix.Size = new System.Drawing.Size(86, 26);
            this.btnAutoDetectMkvToolnix.TabIndex = 8;
            this.btnAutoDetectMkvToolnix.Text = "Auto Detect";
            this.btnAutoDetectMkvToolnix.UseVisualStyleBackColor = true;
            this.btnAutoDetectMkvToolnix.Click += new System.EventHandler(this.btnAutoDetectMkvToolnix_Click);
            // 
            // btnBrowseMKVToolnixPath
            // 
            this.btnBrowseMKVToolnixPath.AutoSize = true;
            this.btnBrowseMKVToolnixPath.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowseMKVToolnixPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseMKVToolnixPath.Location = new System.Drawing.Point(447, 3);
            this.btnBrowseMKVToolnixPath.Name = "btnBrowseMKVToolnixPath";
            this.btnBrowseMKVToolnixPath.Padding = new System.Windows.Forms.Padding(3);
            this.btnBrowseMKVToolnixPath.Size = new System.Drawing.Size(70, 26);
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
            this.tlpInput.ColumnCount = 2;
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpInput.Controls.Add(this.flpFileOptions, 0, 1);
            this.tlpInput.Controls.Add(this.trvInputFiles, 0, 0);
            this.tlpInput.Controls.Add(this.btnSelect, 1, 1);
            this.tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInput.Location = new System.Drawing.Point(3, 19);
            this.tlpInput.Margin = new System.Windows.Forms.Padding(0);
            this.tlpInput.Name = "tlpInput";
            this.tlpInput.RowCount = 2;
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.Size = new System.Drawing.Size(612, 217);
            this.tlpInput.TabIndex = 1;
            // 
            // flpFileOptions
            // 
            this.flpFileOptions.AutoSize = true;
            this.flpFileOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpFileOptions.Controls.Add(this.chkAppendOnDragAndDrop);
            this.flpFileOptions.Controls.Add(this.chkOverwriteExistingFiles);
            this.flpFileOptions.Controls.Add(this.chkDisableTooltips);
            this.flpFileOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFileOptions.Location = new System.Drawing.Point(0, 185);
            this.flpFileOptions.Margin = new System.Windows.Forms.Padding(0);
            this.flpFileOptions.Name = "flpFileOptions";
            this.flpFileOptions.Padding = new System.Windows.Forms.Padding(3);
            this.flpFileOptions.Size = new System.Drawing.Size(543, 32);
            this.flpFileOptions.TabIndex = 5;
            // 
            // chkAppendOnDragAndDrop
            // 
            this.chkAppendOnDragAndDrop.AutoSize = true;
            this.chkAppendOnDragAndDrop.Location = new System.Drawing.Point(6, 6);
            this.chkAppendOnDragAndDrop.Name = "chkAppendOnDragAndDrop";
            this.chkAppendOnDragAndDrop.Size = new System.Drawing.Size(194, 19);
            this.chkAppendOnDragAndDrop.TabIndex = 1;
            this.chkAppendOnDragAndDrop.Text = "Append input on drag and drop";
            this.chkAppendOnDragAndDrop.UseVisualStyleBackColor = true;
            this.chkAppendOnDragAndDrop.CheckedChanged += new System.EventHandler(this.chkAppendOnDragAndDrop_CheckedChanged);
            // 
            // chkOverwriteExistingFiles
            // 
            this.chkOverwriteExistingFiles.AutoSize = true;
            this.chkOverwriteExistingFiles.Location = new System.Drawing.Point(206, 6);
            this.chkOverwriteExistingFiles.Name = "chkOverwriteExistingFiles";
            this.chkOverwriteExistingFiles.Size = new System.Drawing.Size(144, 19);
            this.chkOverwriteExistingFiles.TabIndex = 2;
            this.chkOverwriteExistingFiles.Text = "Overwrite existing files";
            this.chkOverwriteExistingFiles.UseVisualStyleBackColor = true;
            this.chkOverwriteExistingFiles.CheckedChanged += new System.EventHandler(this.chkOverwriteExistingFiles_CheckedChanged);
            // 
            // chkDisableTooltips
            // 
            this.chkDisableTooltips.AutoSize = true;
            this.chkDisableTooltips.Location = new System.Drawing.Point(356, 6);
            this.chkDisableTooltips.Name = "chkDisableTooltips";
            this.chkDisableTooltips.Size = new System.Drawing.Size(107, 19);
            this.chkDisableTooltips.TabIndex = 3;
            this.chkDisableTooltips.Text = "Disable tooltips";
            this.chkDisableTooltips.UseVisualStyleBackColor = true;
            this.chkDisableTooltips.CheckedChanged += new System.EventHandler(this.chkDisableTooltips_CheckedChanged);
            // 
            // trvInputFiles
            // 
            this.trvInputFiles.AllowDrop = true;
            this.trvInputFiles.CheckBoxes = true;
            this.trvInputFiles.CheckOnClick = true;
            this.tlpInput.SetColumnSpan(this.trvInputFiles, 2);
            this.trvInputFiles.ContextMenuStrip = this.contextMenuStrip;
            this.trvInputFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvInputFiles.HideSelection = false;
            this.trvInputFiles.Location = new System.Drawing.Point(3, 3);
            this.trvInputFiles.Name = "trvInputFiles";
            this.trvInputFiles.SelectOnRightClick = true;
            this.trvInputFiles.ShowNodeToolTips = true;
            this.trvInputFiles.Size = new System.Drawing.Size(606, 179);
            this.trvInputFiles.TabIndex = 0;
            this.trvInputFiles.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvInputFiles_AfterCheck);
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
            this.contextMenuStrip.Size = new System.Drawing.Size(246, 458);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // addInputFileToolStripMenuItem
            // 
            this.addInputFileToolStripMenuItem.Name = "addInputFileToolStripMenuItem";
            this.addInputFileToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.addInputFileToolStripMenuItem.Text = "Add Input File(s)...";
            this.addInputFileToolStripMenuItem.Click += new System.EventHandler(this.addInputFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(242, 6);
            // 
            // checkTracksToolStripMenuItem
            // 
            this.checkTracksToolStripMenuItem.Name = "checkTracksToolStripMenuItem";
            this.checkTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkTracksToolStripMenuItem.Text = "Check All Tracks";
            this.checkTracksToolStripMenuItem.Click += new System.EventHandler(this.checkTracksToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
            // 
            // checkVideoTracksToolStripMenuItem
            // 
            this.checkVideoTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allVideoTracksToolStripMenuItem});
            this.checkVideoTracksToolStripMenuItem.Name = "checkVideoTracksToolStripMenuItem";
            this.checkVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkVideoTracksToolStripMenuItem.Text = "Check Video Tracks...";
            // 
            // allVideoTracksToolStripMenuItem
            // 
            this.allVideoTracksToolStripMenuItem.Name = "allVideoTracksToolStripMenuItem";
            this.allVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.allVideoTracksToolStripMenuItem.Text = "All Video Tracks";
            this.allVideoTracksToolStripMenuItem.Click += new System.EventHandler(this.allVideoTracksToolStripMenuItem_Click);
            // 
            // checkAudioTracksToolStripMenuItem
            // 
            this.checkAudioTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAudioTracksToolStripMenuItem});
            this.checkAudioTracksToolStripMenuItem.Name = "checkAudioTracksToolStripMenuItem";
            this.checkAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkAudioTracksToolStripMenuItem.Text = "Check Audio Tracks...";
            // 
            // allAudioTracksToolStripMenuItem
            // 
            this.allAudioTracksToolStripMenuItem.Name = "allAudioTracksToolStripMenuItem";
            this.allAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.allAudioTracksToolStripMenuItem.Text = "All Audio Tracks";
            this.allAudioTracksToolStripMenuItem.Click += new System.EventHandler(this.allAudioTracksToolStripMenuItem_Click);
            // 
            // checkSubtitleTracksToolStripMenuItem
            // 
            this.checkSubtitleTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allSubtitleTracksToolStripMenuItem});
            this.checkSubtitleTracksToolStripMenuItem.Name = "checkSubtitleTracksToolStripMenuItem";
            this.checkSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkSubtitleTracksToolStripMenuItem.Text = "Check Subtitle Tracks...";
            // 
            // allSubtitleTracksToolStripMenuItem
            // 
            this.allSubtitleTracksToolStripMenuItem.Name = "allSubtitleTracksToolStripMenuItem";
            this.allSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.allSubtitleTracksToolStripMenuItem.Text = "All Subtitle Tracks";
            this.allSubtitleTracksToolStripMenuItem.Click += new System.EventHandler(this.allSubtitleTracksToolStripMenuItem_Click);
            // 
            // checkChapterTracksToolStripMenuItem
            // 
            this.checkChapterTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allChapterTracksToolStripMenuItem});
            this.checkChapterTracksToolStripMenuItem.Name = "checkChapterTracksToolStripMenuItem";
            this.checkChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkChapterTracksToolStripMenuItem.Text = "Check Chapter Tracks...";
            // 
            // allChapterTracksToolStripMenuItem
            // 
            this.allChapterTracksToolStripMenuItem.Name = "allChapterTracksToolStripMenuItem";
            this.allChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.allChapterTracksToolStripMenuItem.Text = "All Chapter Tracks";
            this.allChapterTracksToolStripMenuItem.Click += new System.EventHandler(this.allChapterTracksToolStripMenuItem_Click);
            // 
            // checkAttachmentTracksToolStripMenuItem
            // 
            this.checkAttachmentTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAttachmentTracksToolStripMenuItem});
            this.checkAttachmentTracksToolStripMenuItem.Name = "checkAttachmentTracksToolStripMenuItem";
            this.checkAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.checkAttachmentTracksToolStripMenuItem.Text = "Check Attachment Tracks...";
            // 
            // allAttachmentTracksToolStripMenuItem
            // 
            this.allAttachmentTracksToolStripMenuItem.Name = "allAttachmentTracksToolStripMenuItem";
            this.allAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.allAttachmentTracksToolStripMenuItem.Text = "All Attachment Tracks";
            this.allAttachmentTracksToolStripMenuItem.Click += new System.EventHandler(this.allAttachmentTracksToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(242, 6);
            // 
            // uncheckTracksToolStripMenuItem
            // 
            this.uncheckTracksToolStripMenuItem.Name = "uncheckTracksToolStripMenuItem";
            this.uncheckTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckTracksToolStripMenuItem.Text = "Uncheck All Tracks";
            this.uncheckTracksToolStripMenuItem.Click += new System.EventHandler(this.uncheckTracksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(242, 6);
            // 
            // uncheckVideoTracksToolStripMenuItem
            // 
            this.uncheckVideoTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allVideoTracksToolStripMenuItem1});
            this.uncheckVideoTracksToolStripMenuItem.Name = "uncheckVideoTracksToolStripMenuItem";
            this.uncheckVideoTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckVideoTracksToolStripMenuItem.Text = "Uncheck Video Tracks...";
            // 
            // allVideoTracksToolStripMenuItem1
            // 
            this.allVideoTracksToolStripMenuItem1.Name = "allVideoTracksToolStripMenuItem1";
            this.allVideoTracksToolStripMenuItem1.Size = new System.Drawing.Size(170, 22);
            this.allVideoTracksToolStripMenuItem1.Text = "All Video Tracks";
            this.allVideoTracksToolStripMenuItem1.Click += new System.EventHandler(this.allVideoTracksToolStripMenuItem1_Click);
            // 
            // uncheckAudioTracksToolStripMenuItem
            // 
            this.uncheckAudioTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAudioTracksToolStripMenuItem1});
            this.uncheckAudioTracksToolStripMenuItem.Name = "uncheckAudioTracksToolStripMenuItem";
            this.uncheckAudioTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckAudioTracksToolStripMenuItem.Text = "Uncheck Audio Tracks...";
            // 
            // allAudioTracksToolStripMenuItem1
            // 
            this.allAudioTracksToolStripMenuItem1.Name = "allAudioTracksToolStripMenuItem1";
            this.allAudioTracksToolStripMenuItem1.Size = new System.Drawing.Size(170, 22);
            this.allAudioTracksToolStripMenuItem1.Text = "All Audio Tracks";
            this.allAudioTracksToolStripMenuItem1.Click += new System.EventHandler(this.allAudioTracksToolStripMenuItem1_Click);
            // 
            // uncheckSubtitleTracksToolStripMenuItem
            // 
            this.uncheckSubtitleTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allSubtitleTracksToolStripMenuItem1});
            this.uncheckSubtitleTracksToolStripMenuItem.Name = "uncheckSubtitleTracksToolStripMenuItem";
            this.uncheckSubtitleTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckSubtitleTracksToolStripMenuItem.Text = "Uncheck Subtitle Tracks...";
            // 
            // allSubtitleTracksToolStripMenuItem1
            // 
            this.allSubtitleTracksToolStripMenuItem1.Name = "allSubtitleTracksToolStripMenuItem1";
            this.allSubtitleTracksToolStripMenuItem1.Size = new System.Drawing.Size(179, 22);
            this.allSubtitleTracksToolStripMenuItem1.Text = "All Subtitle Tracks";
            this.allSubtitleTracksToolStripMenuItem1.Click += new System.EventHandler(this.allSubtitleTracksToolStripMenuItem1_Click);
            // 
            // uncheckChapterTracksToolStripMenuItem
            // 
            this.uncheckChapterTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allChapterTracksToolStripMenuItem1});
            this.uncheckChapterTracksToolStripMenuItem.Name = "uncheckChapterTracksToolStripMenuItem";
            this.uncheckChapterTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckChapterTracksToolStripMenuItem.Text = "Uncheck Chapter Tracks...";
            // 
            // allChapterTracksToolStripMenuItem1
            // 
            this.allChapterTracksToolStripMenuItem1.Name = "allChapterTracksToolStripMenuItem1";
            this.allChapterTracksToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.allChapterTracksToolStripMenuItem1.Text = "All Chapter Tracks";
            this.allChapterTracksToolStripMenuItem1.Click += new System.EventHandler(this.allChapterTracksToolStripMenuItem1_Click);
            // 
            // uncheckAttachmentTracksToolStripMenuItem
            // 
            this.uncheckAttachmentTracksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allAttachmentTracksToolStripMenuItem1});
            this.uncheckAttachmentTracksToolStripMenuItem.Name = "uncheckAttachmentTracksToolStripMenuItem";
            this.uncheckAttachmentTracksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.uncheckAttachmentTracksToolStripMenuItem.Text = "Uncheck Attachment Tracks...";
            // 
            // allAttachmentTracksToolStripMenuItem1
            // 
            this.allAttachmentTracksToolStripMenuItem1.Name = "allAttachmentTracksToolStripMenuItem1";
            this.allAttachmentTracksToolStripMenuItem1.Size = new System.Drawing.Size(201, 22);
            this.allAttachmentTracksToolStripMenuItem1.Text = "All Attachment Tracks";
            this.allAttachmentTracksToolStripMenuItem1.Click += new System.EventHandler(this.allAttachmentTracksToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(242, 6);
            // 
            // removeAllInputFilesToolStripMenuItem
            // 
            this.removeAllInputFilesToolStripMenuItem.Name = "removeAllInputFilesToolStripMenuItem";
            this.removeAllInputFilesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.removeAllInputFilesToolStripMenuItem.Text = "Remove All Input Files";
            this.removeAllInputFilesToolStripMenuItem.Click += new System.EventHandler(this.removeAllInputFilesToolStripMenuItem_Click);
            // 
            // removeSelectedInputFileToolStripMenuItem
            // 
            this.removeSelectedInputFileToolStripMenuItem.Name = "removeSelectedInputFileToolStripMenuItem";
            this.removeSelectedInputFileToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.removeSelectedInputFileToolStripMenuItem.Text = "Remove Selected Input File";
            this.removeSelectedInputFileToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedInputFileToolStripMenuItem_Click);
            // 
            // openSelectedFileToolStripMenuItem
            // 
            this.openSelectedFileToolStripMenuItem.Name = "openSelectedFileToolStripMenuItem";
            this.openSelectedFileToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.openSelectedFileToolStripMenuItem.Text = "Open Selected File...";
            this.openSelectedFileToolStripMenuItem.Click += new System.EventHandler(this.openSelectedFileToolStripMenuItem_Click);
            // 
            // openSelectedFileFolderToolStripMenuItem
            // 
            this.openSelectedFileFolderToolStripMenuItem.Name = "openSelectedFileFolderToolStripMenuItem";
            this.openSelectedFileFolderToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.openSelectedFileFolderToolStripMenuItem.Text = "Open Selected File Folder...";
            this.openSelectedFileFolderToolStripMenuItem.Click += new System.EventHandler(this.openSelectedFileFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(242, 6);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.AutoSize = true;
            this.btnSelect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSelect.Location = new System.Drawing.Point(546, 188);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Padding = new System.Windows.Forms.Padding(3);
            this.btnSelect.Size = new System.Drawing.Size(63, 26);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select...";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
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
            this.btnAbort.AutoSize = true;
            this.btnAbort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAbort.Location = new System.Drawing.Point(215, 3);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Padding = new System.Windows.Forms.Padding(3);
            this.btnAbort.Size = new System.Drawing.Size(53, 31);
            this.btnAbort.TabIndex = 12;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAbortAll
            // 
            this.btnAbortAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbortAll.AutoSize = true;
            this.btnAbortAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAbortAll.Location = new System.Drawing.Point(139, 3);
            this.btnAbortAll.Name = "btnAbortAll";
            this.btnAbortAll.Padding = new System.Windows.Forms.Padding(3);
            this.btnAbortAll.Size = new System.Drawing.Size(70, 31);
            this.btnAbortAll.TabIndex = 13;
            this.btnAbortAll.Text = "Abort All";
            this.btnAbortAll.UseVisualStyleBackColor = true;
            this.btnAbortAll.Click += new System.EventHandler(this.btnAbortAll_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.AutoSize = true;
            this.btnOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOptions.Location = new System.Drawing.Point(59, 3);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Padding = new System.Windows.Forms.Padding(3);
            this.btnOptions.Size = new System.Drawing.Size(74, 31);
            this.btnOptions.TabIndex = 14;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // chkDarkMode
            // 
            this.chkDarkMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkDarkMode.AutoSize = true;
            this.chkDarkMode.Location = new System.Drawing.Point(3, 9);
            this.chkDarkMode.Name = "chkDarkMode";
            this.chkDarkMode.Size = new System.Drawing.Size(50, 19);
            this.chkDarkMode.TabIndex = 15;
            this.chkDarkMode.Text = "Dark";
            this.chkDarkMode.UseVisualStyleBackColor = true;
            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);
            // 
            // flpFrmOptions
            // 
            this.flpFrmOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFrmOptions.AutoSize = true;
            this.flpFrmOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpFrmOptions.Controls.Add(this.chkDarkMode);
            this.flpFrmOptions.Controls.Add(this.btnOptions);
            this.flpFrmOptions.Controls.Add(this.btnAbortAll);
            this.flpFrmOptions.Controls.Add(this.btnAbort);
            this.flpFrmOptions.Location = new System.Drawing.Point(341, 524);
            this.flpFrmOptions.Name = "flpFrmOptions";
            this.flpFrmOptions.Size = new System.Drawing.Size(271, 37);
            this.flpFrmOptions.TabIndex = 2;
            this.flpFrmOptions.WrapContents = false;
            // 
            // frmMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 561);
            this.Controls.Add(this.flpFrmOptions);
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
            this.tlpActions.ResumeLayout(false);
            this.tlpActions.PerformLayout();
            this.flpActionsLeft.ResumeLayout(false);
            this.flpActionsLeft.PerformLayout();
            this.flpActionsRight.ResumeLayout(false);
            this.flpActionsRight.PerformLayout();
            this.grpOutputDirectory.ResumeLayout(false);
            this.tlpOutputDirectory.ResumeLayout(false);
            this.tlpOutputDirectory.PerformLayout();
            this.contextMenuStripOutputDirectory.ResumeLayout(false);
            this.grpConfig.ResumeLayout(false);
            this.tlpConfig.ResumeLayout(false);
            this.tlpConfig.PerformLayout();
            this.grpInputFiles.ResumeLayout(false);
            this.tlpInput.ResumeLayout(false);
            this.tlpInput.PerformLayout();
            this.flpFileOptions.ResumeLayout(false);
            this.flpFileOptions.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.grpSelectedFileInfo.ResumeLayout(false);
            this.flpFrmOptions.ResumeLayout(false);
            this.flpFrmOptions.PerformLayout();
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
        private System.Windows.Forms.CheckBox chkDarkMode;
        private System.Windows.Forms.Button btnAutoDetectMkvToolnix;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.FlowLayoutPanel flpFileOptions;
        private System.Windows.Forms.CheckBox chkAppendOnDragAndDrop;
        private System.Windows.Forms.CheckBox chkOverwriteExistingFiles;
        private System.Windows.Forms.CheckBox chkDisableTooltips;
        private System.Windows.Forms.TableLayoutPanel tlpConfig;
        private System.Windows.Forms.TableLayoutPanel tlpOutputDirectory;
        private System.Windows.Forms.TableLayoutPanel tlpActions;
        private System.Windows.Forms.FlowLayoutPanel flpActionsLeft;
        private System.Windows.Forms.FlowLayoutPanel flpActionsRight;
        private System.Windows.Forms.FlowLayoutPanel flpFrmOptions;
    }
}
