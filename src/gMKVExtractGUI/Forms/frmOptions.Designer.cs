namespace gMKVToolNix.Forms
{
    partial class frmOptions
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
            this.tlpMain = new gMKVToolNix.gTableLayoutPanel();
            this.grpTags = new gMKVToolNix.gGroupBox();
            this.tlpTags = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultTagsPlaceholder = new System.Windows.Forms.Button();
            this.txtTagsFilename = new gMKVToolNix.gTextBox();
            this.btnAddTagsPlaceholder = new System.Windows.Forms.Button();
            this.grpChapters = new gMKVToolNix.gGroupBox();
            this.tlpChapters = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultChapterPlaceholder = new System.Windows.Forms.Button();
            this.txtChaptersFilename = new gMKVToolNix.gTextBox();
            this.btnAddChapterPlaceholder = new System.Windows.Forms.Button();
            this.grpVideoTracks = new gMKVToolNix.gGroupBox();
            this.tlpVideoTracks = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultVideoTrackPlaceholder = new System.Windows.Forms.Button();
            this.txtVideoTracksFilename = new gMKVToolNix.gTextBox();
            this.btnAddVideoTrackPlaceholder = new System.Windows.Forms.Button();
            this.grpAudioTracks = new gMKVToolNix.gGroupBox();
            this.tlpAudioTracks = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultAudioTrackPlaceholder = new System.Windows.Forms.Button();
            this.txtAudioTracksFilename = new gMKVToolNix.gTextBox();
            this.btnAddAudioTrackPlaceholder = new System.Windows.Forms.Button();
            this.grpSubtitleTracks = new gMKVToolNix.gGroupBox();
            this.tlpSubtitleTracks = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultSubtitleTrackPlaceholder = new System.Windows.Forms.Button();
            this.txtSubtitleTracksFilename = new gMKVToolNix.gTextBox();
            this.btnAddSubtitleTrackPlaceholder = new System.Windows.Forms.Button();
            this.grpAttachments = new gMKVToolNix.gGroupBox();
            this.tlpAttachments = new System.Windows.Forms.TableLayoutPanel();
            this.btnDefaultAttachmentPlaceholder = new System.Windows.Forms.Button();
            this.txtAttachmentsFilename = new gMKVToolNix.gTextBox();
            this.btnAddAttachmentPlaceholder = new System.Windows.Forms.Button();
            this.grpInfo = new gMKVToolNix.gGroupBox();
            this.txtInfo = new gMKVToolNix.gRichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.grpActions = new gMKVToolNix.gGroupBox();
            this.btnDefaults = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAdvanced = new gMKVToolNix.gGroupBox();
            this.tlpAdvanced = new System.Windows.Forms.TableLayoutPanel();
            this.flpAdvancedUp = new System.Windows.Forms.FlowLayoutPanel();
            this.chkTextFilesWithoutBom = new System.Windows.Forms.CheckBox();
            this.chkRawMode = new System.Windows.Forms.CheckBox();
            this.chkFullRawMode = new System.Windows.Forms.CheckBox();
            this.flpAdvancedBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.cmbCulture = new gMKVToolNix.Controls.gComboBox();
            this.lblCulture = new System.Windows.Forms.Label();
            this.btnTranslationEditor = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.grpTags.SuspendLayout();
            this.tlpTags.SuspendLayout();
            this.grpChapters.SuspendLayout();
            this.tlpChapters.SuspendLayout();
            this.grpVideoTracks.SuspendLayout();
            this.tlpVideoTracks.SuspendLayout();
            this.grpAudioTracks.SuspendLayout();
            this.tlpAudioTracks.SuspendLayout();
            this.grpSubtitleTracks.SuspendLayout();
            this.tlpSubtitleTracks.SuspendLayout();
            this.grpAttachments.SuspendLayout();
            this.tlpAttachments.SuspendLayout();
            this.grpInfo.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpAdvanced.SuspendLayout();
            this.tlpAdvanced.SuspendLayout();
            this.flpAdvancedUp.SuspendLayout();
            this.flpAdvancedBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpTags, 0, 6);
            this.tlpMain.Controls.Add(this.grpChapters, 0, 4);
            this.tlpMain.Controls.Add(this.grpVideoTracks, 0, 1);
            this.tlpMain.Controls.Add(this.grpAudioTracks, 0, 2);
            this.tlpMain.Controls.Add(this.grpSubtitleTracks, 0, 3);
            this.tlpMain.Controls.Add(this.grpAttachments, 0, 5);
            this.tlpMain.Controls.Add(this.grpInfo, 0, 0);
            this.tlpMain.Controls.Add(this.statusStrip1, 0, 9);
            this.tlpMain.Controls.Add(this.grpActions, 0, 8);
            this.tlpMain.Controls.Add(this.grpAdvanced, 0, 7);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 10;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(616, 607);
            this.tlpMain.TabIndex = 0;
            // 
            // grpTags
            // 
            this.grpTags.Controls.Add(this.tlpTags);
            this.grpTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTags.Location = new System.Drawing.Point(3, 380);
            this.grpTags.Name = "grpTags";
            this.grpTags.Size = new System.Drawing.Size(610, 54);
            this.grpTags.TabIndex = 7;
            this.grpTags.TabStop = false;
            this.grpTags.Text = "Tags";
            // 
            // tlpTags
            // 
            this.tlpTags.ColumnCount = 3;
            this.tlpTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTags.Controls.Add(this.btnDefaultTagsPlaceholder, 2, 0);
            this.tlpTags.Controls.Add(this.txtTagsFilename, 0, 0);
            this.tlpTags.Controls.Add(this.btnAddTagsPlaceholder, 1, 0);
            this.tlpTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTags.Location = new System.Drawing.Point(3, 19);
            this.tlpTags.Name = "tlpTags";
            this.tlpTags.RowCount = 1;
            this.tlpTags.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTags.Size = new System.Drawing.Size(604, 32);
            this.tlpTags.TabIndex = 2;
            // 
            // btnDefaultTagsPlaceholder
            // 
            this.btnDefaultTagsPlaceholder.AutoSize = true;
            this.btnDefaultTagsPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultTagsPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultTagsPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultTagsPlaceholder.Name = "btnDefaultTagsPlaceholder";
            this.btnDefaultTagsPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultTagsPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultTagsPlaceholder.TabIndex = 6;
            this.btnDefaultTagsPlaceholder.Text = "Default";
            this.btnDefaultTagsPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultTagsPlaceholder.Click += new System.EventHandler(this.btnDefaultTagsPlaceholder_Click);
            // 
            // txtTagsFilename
            // 
            this.txtTagsFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTagsFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtTagsFilename.Location = new System.Drawing.Point(3, 3);
            this.txtTagsFilename.Name = "txtTagsFilename";
            this.txtTagsFilename.Size = new System.Drawing.Size(471, 23);
            this.txtTagsFilename.TabIndex = 1;
            // 
            // btnAddTagsPlaceholder
            // 
            this.btnAddTagsPlaceholder.AutoSize = true;
            this.btnAddTagsPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddTagsPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddTagsPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddTagsPlaceholder.Name = "btnAddTagsPlaceholder";
            this.btnAddTagsPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddTagsPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddTagsPlaceholder.TabIndex = 5;
            this.btnAddTagsPlaceholder.Text = "Add...";
            this.btnAddTagsPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddTagsPlaceholder.Click += new System.EventHandler(this.btnAddTagsPlaceholder_Click);
            // 
            // grpChapters
            // 
            this.grpChapters.Controls.Add(this.tlpChapters);
            this.grpChapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpChapters.Location = new System.Drawing.Point(3, 260);
            this.grpChapters.Name = "grpChapters";
            this.grpChapters.Size = new System.Drawing.Size(610, 54);
            this.grpChapters.TabIndex = 0;
            this.grpChapters.TabStop = false;
            this.grpChapters.Text = "Chapters";
            // 
            // tlpChapters
            // 
            this.tlpChapters.ColumnCount = 3;
            this.tlpChapters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpChapters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpChapters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpChapters.Controls.Add(this.btnDefaultChapterPlaceholder, 2, 0);
            this.tlpChapters.Controls.Add(this.txtChaptersFilename, 0, 0);
            this.tlpChapters.Controls.Add(this.btnAddChapterPlaceholder, 1, 0);
            this.tlpChapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpChapters.Location = new System.Drawing.Point(3, 19);
            this.tlpChapters.Name = "tlpChapters";
            this.tlpChapters.RowCount = 1;
            this.tlpChapters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpChapters.Size = new System.Drawing.Size(604, 32);
            this.tlpChapters.TabIndex = 3;
            // 
            // btnDefaultChapterPlaceholder
            // 
            this.btnDefaultChapterPlaceholder.AutoSize = true;
            this.btnDefaultChapterPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultChapterPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultChapterPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultChapterPlaceholder.Name = "btnDefaultChapterPlaceholder";
            this.btnDefaultChapterPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultChapterPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultChapterPlaceholder.TabIndex = 5;
            this.btnDefaultChapterPlaceholder.Text = "Default";
            this.btnDefaultChapterPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultChapterPlaceholder.Click += new System.EventHandler(this.btnDefaultChapterPlaceholder_Click);
            // 
            // txtChaptersFilename
            // 
            this.txtChaptersFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtChaptersFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtChaptersFilename.Location = new System.Drawing.Point(3, 3);
            this.txtChaptersFilename.Name = "txtChaptersFilename";
            this.txtChaptersFilename.Size = new System.Drawing.Size(471, 23);
            this.txtChaptersFilename.TabIndex = 1;
            // 
            // btnAddChapterPlaceholder
            // 
            this.btnAddChapterPlaceholder.AutoSize = true;
            this.btnAddChapterPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddChapterPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddChapterPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddChapterPlaceholder.Name = "btnAddChapterPlaceholder";
            this.btnAddChapterPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddChapterPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddChapterPlaceholder.TabIndex = 4;
            this.btnAddChapterPlaceholder.Text = "Add...";
            this.btnAddChapterPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddChapterPlaceholder.Click += new System.EventHandler(this.btnAddChapterPlaceholder_Click);
            // 
            // grpVideoTracks
            // 
            this.grpVideoTracks.Controls.Add(this.tlpVideoTracks);
            this.grpVideoTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpVideoTracks.Location = new System.Drawing.Point(3, 80);
            this.grpVideoTracks.Name = "grpVideoTracks";
            this.grpVideoTracks.Size = new System.Drawing.Size(610, 54);
            this.grpVideoTracks.TabIndex = 1;
            this.grpVideoTracks.TabStop = false;
            this.grpVideoTracks.Text = "Video Tracks";
            // 
            // tlpVideoTracks
            // 
            this.tlpVideoTracks.ColumnCount = 3;
            this.tlpVideoTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVideoTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpVideoTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpVideoTracks.Controls.Add(this.btnDefaultVideoTrackPlaceholder, 2, 0);
            this.tlpVideoTracks.Controls.Add(this.txtVideoTracksFilename, 0, 0);
            this.tlpVideoTracks.Controls.Add(this.btnAddVideoTrackPlaceholder, 1, 0);
            this.tlpVideoTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpVideoTracks.Location = new System.Drawing.Point(3, 19);
            this.tlpVideoTracks.Name = "tlpVideoTracks";
            this.tlpVideoTracks.RowCount = 1;
            this.tlpVideoTracks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVideoTracks.Size = new System.Drawing.Size(604, 32);
            this.tlpVideoTracks.TabIndex = 5;
            // 
            // btnDefaultVideoTrackPlaceholder
            // 
            this.btnDefaultVideoTrackPlaceholder.AutoSize = true;
            this.btnDefaultVideoTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultVideoTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultVideoTrackPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultVideoTrackPlaceholder.Name = "btnDefaultVideoTrackPlaceholder";
            this.btnDefaultVideoTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultVideoTrackPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultVideoTrackPlaceholder.TabIndex = 2;
            this.btnDefaultVideoTrackPlaceholder.Text = "Default";
            this.btnDefaultVideoTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultVideoTrackPlaceholder.Click += new System.EventHandler(this.btnDefaultVideoTrackPlaceholder_Click);
            // 
            // txtVideoTracksFilename
            // 
            this.txtVideoTracksFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVideoTracksFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtVideoTracksFilename.Location = new System.Drawing.Point(3, 3);
            this.txtVideoTracksFilename.Name = "txtVideoTracksFilename";
            this.txtVideoTracksFilename.Size = new System.Drawing.Size(471, 23);
            this.txtVideoTracksFilename.TabIndex = 0;
            // 
            // btnAddVideoTrackPlaceholder
            // 
            this.btnAddVideoTrackPlaceholder.AutoSize = true;
            this.btnAddVideoTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddVideoTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddVideoTrackPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddVideoTrackPlaceholder.Name = "btnAddVideoTrackPlaceholder";
            this.btnAddVideoTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddVideoTrackPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddVideoTrackPlaceholder.TabIndex = 1;
            this.btnAddVideoTrackPlaceholder.Text = "Add...";
            this.btnAddVideoTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddVideoTrackPlaceholder.Click += new System.EventHandler(this.btnAddVideoTrackPlaceholder_Click);
            // 
            // grpAudioTracks
            // 
            this.grpAudioTracks.Controls.Add(this.tlpAudioTracks);
            this.grpAudioTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAudioTracks.Location = new System.Drawing.Point(3, 140);
            this.grpAudioTracks.Name = "grpAudioTracks";
            this.grpAudioTracks.Size = new System.Drawing.Size(610, 54);
            this.grpAudioTracks.TabIndex = 2;
            this.grpAudioTracks.TabStop = false;
            this.grpAudioTracks.Text = "Audio Tracks";
            // 
            // tlpAudioTracks
            // 
            this.tlpAudioTracks.ColumnCount = 3;
            this.tlpAudioTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAudioTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAudioTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAudioTracks.Controls.Add(this.btnDefaultAudioTrackPlaceholder, 2, 0);
            this.tlpAudioTracks.Controls.Add(this.txtAudioTracksFilename, 0, 0);
            this.tlpAudioTracks.Controls.Add(this.btnAddAudioTrackPlaceholder, 1, 0);
            this.tlpAudioTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAudioTracks.Location = new System.Drawing.Point(3, 19);
            this.tlpAudioTracks.Name = "tlpAudioTracks";
            this.tlpAudioTracks.RowCount = 1;
            this.tlpAudioTracks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAudioTracks.Size = new System.Drawing.Size(604, 32);
            this.tlpAudioTracks.TabIndex = 4;
            // 
            // btnDefaultAudioTrackPlaceholder
            // 
            this.btnDefaultAudioTrackPlaceholder.AutoSize = true;
            this.btnDefaultAudioTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultAudioTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultAudioTrackPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultAudioTrackPlaceholder.Name = "btnDefaultAudioTrackPlaceholder";
            this.btnDefaultAudioTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultAudioTrackPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultAudioTrackPlaceholder.TabIndex = 3;
            this.btnDefaultAudioTrackPlaceholder.Text = "Default";
            this.btnDefaultAudioTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultAudioTrackPlaceholder.Click += new System.EventHandler(this.btnDefaultAudioTrackPlaceholder_Click);
            // 
            // txtAudioTracksFilename
            // 
            this.txtAudioTracksFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAudioTracksFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtAudioTracksFilename.Location = new System.Drawing.Point(3, 3);
            this.txtAudioTracksFilename.Name = "txtAudioTracksFilename";
            this.txtAudioTracksFilename.Size = new System.Drawing.Size(471, 23);
            this.txtAudioTracksFilename.TabIndex = 1;
            // 
            // btnAddAudioTrackPlaceholder
            // 
            this.btnAddAudioTrackPlaceholder.AutoSize = true;
            this.btnAddAudioTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddAudioTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddAudioTrackPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddAudioTrackPlaceholder.Name = "btnAddAudioTrackPlaceholder";
            this.btnAddAudioTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddAudioTrackPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddAudioTrackPlaceholder.TabIndex = 2;
            this.btnAddAudioTrackPlaceholder.Text = "Add...";
            this.btnAddAudioTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddAudioTrackPlaceholder.Click += new System.EventHandler(this.btnAddAudioTrackPlaceholder_Click);
            // 
            // grpSubtitleTracks
            // 
            this.grpSubtitleTracks.Controls.Add(this.tlpSubtitleTracks);
            this.grpSubtitleTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSubtitleTracks.Location = new System.Drawing.Point(3, 200);
            this.grpSubtitleTracks.Name = "grpSubtitleTracks";
            this.grpSubtitleTracks.Size = new System.Drawing.Size(610, 54);
            this.grpSubtitleTracks.TabIndex = 3;
            this.grpSubtitleTracks.TabStop = false;
            this.grpSubtitleTracks.Text = "Subtitle Tracks";
            // 
            // tlpSubtitleTracks
            // 
            this.tlpSubtitleTracks.ColumnCount = 3;
            this.tlpSubtitleTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSubtitleTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSubtitleTracks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSubtitleTracks.Controls.Add(this.btnDefaultSubtitleTrackPlaceholder, 2, 0);
            this.tlpSubtitleTracks.Controls.Add(this.txtSubtitleTracksFilename, 0, 0);
            this.tlpSubtitleTracks.Controls.Add(this.btnAddSubtitleTrackPlaceholder, 1, 0);
            this.tlpSubtitleTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSubtitleTracks.Location = new System.Drawing.Point(3, 19);
            this.tlpSubtitleTracks.Name = "tlpSubtitleTracks";
            this.tlpSubtitleTracks.RowCount = 1;
            this.tlpSubtitleTracks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSubtitleTracks.Size = new System.Drawing.Size(604, 32);
            this.tlpSubtitleTracks.TabIndex = 2;
            // 
            // btnDefaultSubtitleTrackPlaceholder
            // 
            this.btnDefaultSubtitleTrackPlaceholder.AutoSize = true;
            this.btnDefaultSubtitleTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultSubtitleTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultSubtitleTrackPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultSubtitleTrackPlaceholder.Name = "btnDefaultSubtitleTrackPlaceholder";
            this.btnDefaultSubtitleTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultSubtitleTrackPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultSubtitleTrackPlaceholder.TabIndex = 4;
            this.btnDefaultSubtitleTrackPlaceholder.Text = "Default";
            this.btnDefaultSubtitleTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultSubtitleTrackPlaceholder.Click += new System.EventHandler(this.btnDefaultSubtitleTrackPlaceholder_Click);
            // 
            // txtSubtitleTracksFilename
            // 
            this.txtSubtitleTracksFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubtitleTracksFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtSubtitleTracksFilename.Location = new System.Drawing.Point(3, 3);
            this.txtSubtitleTracksFilename.Name = "txtSubtitleTracksFilename";
            this.txtSubtitleTracksFilename.Size = new System.Drawing.Size(471, 23);
            this.txtSubtitleTracksFilename.TabIndex = 1;
            // 
            // btnAddSubtitleTrackPlaceholder
            // 
            this.btnAddSubtitleTrackPlaceholder.AutoSize = true;
            this.btnAddSubtitleTrackPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddSubtitleTrackPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddSubtitleTrackPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddSubtitleTrackPlaceholder.Name = "btnAddSubtitleTrackPlaceholder";
            this.btnAddSubtitleTrackPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddSubtitleTrackPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddSubtitleTrackPlaceholder.TabIndex = 3;
            this.btnAddSubtitleTrackPlaceholder.Text = "Add...";
            this.btnAddSubtitleTrackPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddSubtitleTrackPlaceholder.Click += new System.EventHandler(this.btnAddSubtitleTrackPlaceholder_Click);
            // 
            // grpAttachments
            // 
            this.grpAttachments.Controls.Add(this.tlpAttachments);
            this.grpAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAttachments.Location = new System.Drawing.Point(3, 320);
            this.grpAttachments.Name = "grpAttachments";
            this.grpAttachments.Size = new System.Drawing.Size(610, 54);
            this.grpAttachments.TabIndex = 4;
            this.grpAttachments.TabStop = false;
            this.grpAttachments.Text = "Attachments";
            // 
            // tlpAttachments
            // 
            this.tlpAttachments.ColumnCount = 3;
            this.tlpAttachments.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAttachments.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAttachments.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAttachments.Controls.Add(this.btnDefaultAttachmentPlaceholder, 2, 0);
            this.tlpAttachments.Controls.Add(this.txtAttachmentsFilename, 0, 0);
            this.tlpAttachments.Controls.Add(this.btnAddAttachmentPlaceholder, 1, 0);
            this.tlpAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAttachments.Location = new System.Drawing.Point(3, 19);
            this.tlpAttachments.Name = "tlpAttachments";
            this.tlpAttachments.RowCount = 1;
            this.tlpAttachments.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAttachments.Size = new System.Drawing.Size(604, 32);
            this.tlpAttachments.TabIndex = 1;
            // 
            // btnDefaultAttachmentPlaceholder
            // 
            this.btnDefaultAttachmentPlaceholder.AutoSize = true;
            this.btnDefaultAttachmentPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDefaultAttachmentPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDefaultAttachmentPlaceholder.Location = new System.Drawing.Point(540, 3);
            this.btnDefaultAttachmentPlaceholder.Name = "btnDefaultAttachmentPlaceholder";
            this.btnDefaultAttachmentPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnDefaultAttachmentPlaceholder.Size = new System.Drawing.Size(61, 26);
            this.btnDefaultAttachmentPlaceholder.TabIndex = 6;
            this.btnDefaultAttachmentPlaceholder.Text = "Default";
            this.btnDefaultAttachmentPlaceholder.UseVisualStyleBackColor = true;
            this.btnDefaultAttachmentPlaceholder.Click += new System.EventHandler(this.btnDefaultAttachmentPlaceholder_Click);
            // 
            // txtAttachmentsFilename
            // 
            this.txtAttachmentsFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAttachmentsFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtAttachmentsFilename.Location = new System.Drawing.Point(3, 3);
            this.txtAttachmentsFilename.Name = "txtAttachmentsFilename";
            this.txtAttachmentsFilename.Size = new System.Drawing.Size(471, 23);
            this.txtAttachmentsFilename.TabIndex = 1;
            // 
            // btnAddAttachmentPlaceholder
            // 
            this.btnAddAttachmentPlaceholder.AutoSize = true;
            this.btnAddAttachmentPlaceholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddAttachmentPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddAttachmentPlaceholder.Location = new System.Drawing.Point(480, 3);
            this.btnAddAttachmentPlaceholder.Name = "btnAddAttachmentPlaceholder";
            this.btnAddAttachmentPlaceholder.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddAttachmentPlaceholder.Size = new System.Drawing.Size(54, 26);
            this.btnAddAttachmentPlaceholder.TabIndex = 5;
            this.btnAddAttachmentPlaceholder.Text = "Add...";
            this.btnAddAttachmentPlaceholder.UseVisualStyleBackColor = true;
            this.btnAddAttachmentPlaceholder.Click += new System.EventHandler(this.btnAddAttachmentPlaceholder_Click);
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.txtInfo);
            this.grpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInfo.Location = new System.Drawing.Point(3, 3);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(610, 71);
            this.grpInfo.TabIndex = 5;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Information";
            // 
            // txtInfo
            // 
            this.txtInfo.DarkMode = false;
            this.txtInfo.DetectUrls = false;
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Location = new System.Drawing.Point(3, 19);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ShortcutsEnabled = false;
            this.txtInfo.Size = new System.Drawing.Size(604, 49);
            this.txtInfo.TabIndex = 0;
            this.txtInfo.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Location = new System.Drawing.Point(0, 587);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(616, 20);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnDefaults);
            this.grpActions.Controls.Add(this.btnOK);
            this.grpActions.Controls.Add(this.btnCancel);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 530);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(610, 54);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            // 
            // btnDefaults
            // 
            this.btnDefaults.Location = new System.Drawing.Point(9, 17);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(83, 30);
            this.btnDefaults.TabIndex = 6;
            this.btnDefaults.Text = "Defaults";
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(438, 17);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(524, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpAdvanced
            // 
            this.grpAdvanced.Controls.Add(this.tlpAdvanced);
            this.grpAdvanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAdvanced.Location = new System.Drawing.Point(3, 440);
            this.grpAdvanced.Name = "grpAdvanced";
            this.grpAdvanced.Size = new System.Drawing.Size(610, 84);
            this.grpAdvanced.TabIndex = 8;
            this.grpAdvanced.TabStop = false;
            this.grpAdvanced.Text = "Advanced Options";
            // 
            // tlpAdvanced
            // 
            this.tlpAdvanced.ColumnCount = 1;
            this.tlpAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAdvanced.Controls.Add(this.flpAdvancedUp, 0, 0);
            this.tlpAdvanced.Controls.Add(this.flpAdvancedBottom, 0, 1);
            this.tlpAdvanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAdvanced.Location = new System.Drawing.Point(3, 19);
            this.tlpAdvanced.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAdvanced.Name = "tlpAdvanced";
            this.tlpAdvanced.RowCount = 2;
            this.tlpAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpAdvanced.Size = new System.Drawing.Size(604, 62);
            this.tlpAdvanced.TabIndex = 9;
            // 
            // flpAdvancedUp
            // 
            this.flpAdvancedUp.Controls.Add(this.chkTextFilesWithoutBom);
            this.flpAdvancedUp.Controls.Add(this.chkRawMode);
            this.flpAdvancedUp.Controls.Add(this.chkFullRawMode);
            this.flpAdvancedUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpAdvancedUp.Location = new System.Drawing.Point(0, 0);
            this.flpAdvancedUp.Margin = new System.Windows.Forms.Padding(0);
            this.flpAdvancedUp.Name = "flpAdvancedUp";
            this.flpAdvancedUp.Size = new System.Drawing.Size(604, 24);
            this.flpAdvancedUp.TabIndex = 0;
            this.flpAdvancedUp.WrapContents = false;
            // 
            // chkTextFilesWithoutBom
            // 
            this.chkTextFilesWithoutBom.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkTextFilesWithoutBom.AutoSize = true;
            this.chkTextFilesWithoutBom.Location = new System.Drawing.Point(3, 3);
            this.chkTextFilesWithoutBom.Name = "chkTextFilesWithoutBom";
            this.chkTextFilesWithoutBom.Size = new System.Drawing.Size(200, 19);
            this.chkTextFilesWithoutBom.TabIndex = 7;
            this.chkTextFilesWithoutBom.Text = "Disable BOM to text files (v96.0+)";
            // 
            // chkRawMode
            // 
            this.chkRawMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkRawMode.AutoSize = true;
            this.chkRawMode.Location = new System.Drawing.Point(209, 3);
            this.chkRawMode.Name = "chkRawMode";
            this.chkRawMode.Size = new System.Drawing.Size(128, 19);
            this.chkRawMode.TabIndex = 6;
            this.chkRawMode.Text = "Use `raw` extraction";
            // 
            // chkFullRawMode
            // 
            this.chkFullRawMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkFullRawMode.AutoSize = true;
            this.chkFullRawMode.Location = new System.Drawing.Point(343, 3);
            this.chkFullRawMode.Name = "chkFullRawMode";
            this.chkFullRawMode.Size = new System.Drawing.Size(148, 19);
            this.chkFullRawMode.TabIndex = 5;
            this.chkFullRawMode.Text = "Use `full raw` extraction";
            // 
            // flpAdvancedBottom
            // 
            this.flpAdvancedBottom.Controls.Add(this.cmbCulture);
            this.flpAdvancedBottom.Controls.Add(this.lblCulture);
            this.flpAdvancedBottom.Controls.Add(this.btnTranslationEditor);
            this.flpAdvancedBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpAdvancedBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpAdvancedBottom.Location = new System.Drawing.Point(3, 27);
            this.flpAdvancedBottom.Name = "flpAdvancedBottom";
            this.flpAdvancedBottom.Size = new System.Drawing.Size(598, 32);
            this.flpAdvancedBottom.TabIndex = 1;
            this.flpAdvancedBottom.WrapContents = false;
            // 
            // cmbCulture
            // 
            this.cmbCulture.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmbCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCulture.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbCulture.FormattingEnabled = true;
            this.cmbCulture.Location = new System.Drawing.Point(515, 4);
            this.cmbCulture.Name = "cmbCulture";
            this.cmbCulture.Size = new System.Drawing.Size(80, 23);
            this.cmbCulture.TabIndex = 7;
            this.cmbCulture.SelectedIndexChanged += new System.EventHandler(this.CmbCulture_SelectedIndexChanged);
            // 
            // lblCulture
            // 
            this.lblCulture.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCulture.AutoSize = true;
            this.lblCulture.Location = new System.Drawing.Point(460, 8);
            this.lblCulture.Name = "lblCulture";
            this.lblCulture.Size = new System.Drawing.Size(49, 15);
            this.lblCulture.TabIndex = 3;
            this.lblCulture.Text = "Culture:";
            // 
            // btnTranslationEditor
            // 
            this.btnTranslationEditor.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTranslationEditor.AutoSize = true;
            this.btnTranslationEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTranslationEditor.Location = new System.Drawing.Point(362, 0);
            this.btnTranslationEditor.Margin = new System.Windows.Forms.Padding(0);
            this.btnTranslationEditor.Name = "btnTranslationEditor";
            this.btnTranslationEditor.Padding = new System.Windows.Forms.Padding(3);
            this.btnTranslationEditor.Size = new System.Drawing.Size(95, 31);
            this.btnTranslationEditor.TabIndex = 8;
            this.btnTranslationEditor.Text = "Translations...";
            this.btnTranslationEditor.UseVisualStyleBackColor = true;
            this.btnTranslationEditor.Click += new System.EventHandler(this.btnTranslationEditor_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(616, 607);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(390, 349);
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.grpTags.ResumeLayout(false);
            this.tlpTags.ResumeLayout(false);
            this.tlpTags.PerformLayout();
            this.grpChapters.ResumeLayout(false);
            this.tlpChapters.ResumeLayout(false);
            this.tlpChapters.PerformLayout();
            this.grpVideoTracks.ResumeLayout(false);
            this.tlpVideoTracks.ResumeLayout(false);
            this.tlpVideoTracks.PerformLayout();
            this.grpAudioTracks.ResumeLayout(false);
            this.tlpAudioTracks.ResumeLayout(false);
            this.tlpAudioTracks.PerformLayout();
            this.grpSubtitleTracks.ResumeLayout(false);
            this.tlpSubtitleTracks.ResumeLayout(false);
            this.tlpSubtitleTracks.PerformLayout();
            this.grpAttachments.ResumeLayout(false);
            this.tlpAttachments.ResumeLayout(false);
            this.tlpAttachments.PerformLayout();
            this.grpInfo.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.grpAdvanced.ResumeLayout(false);
            this.tlpAdvanced.ResumeLayout(false);
            this.flpAdvancedUp.ResumeLayout(false);
            this.flpAdvancedUp.PerformLayout();
            this.flpAdvancedBottom.ResumeLayout(false);
            this.flpAdvancedBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private gTableLayoutPanel tlpMain;
        private gGroupBox grpActions;
        private gGroupBox grpVideoTracks;
        private gGroupBox grpChapters;
        private gTextBox txtChaptersFilename;
        private gTextBox txtVideoTracksFilename;
        private gGroupBox grpAudioTracks;
        private gTextBox txtAudioTracksFilename;
        private gGroupBox grpSubtitleTracks;
        private gTextBox txtSubtitleTracksFilename;
        private gGroupBox grpAttachments;
        private gTextBox txtAttachmentsFilename;
        private System.Windows.Forms.Button btnAddVideoTrackPlaceholder;
        private System.Windows.Forms.Button btnAddChapterPlaceholder;
        private System.Windows.Forms.Button btnAddAudioTrackPlaceholder;
        private System.Windows.Forms.Button btnAddSubtitleTrackPlaceholder;
        private System.Windows.Forms.Button btnAddAttachmentPlaceholder;
        private gGroupBox grpInfo;
        private gRichTextBox txtInfo;
        private System.Windows.Forms.Button btnDefaultChapterPlaceholder;
        private System.Windows.Forms.Button btnDefaultVideoTrackPlaceholder;
        private System.Windows.Forms.Button btnDefaultAudioTrackPlaceholder;
        private System.Windows.Forms.Button btnDefaultSubtitleTrackPlaceholder;
        private System.Windows.Forms.Button btnDefaultAttachmentPlaceholder;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private gGroupBox grpTags;
        private System.Windows.Forms.Button btnDefaultTagsPlaceholder;
        private System.Windows.Forms.Button btnAddTagsPlaceholder;
        private gTextBox txtTagsFilename;
        private gGroupBox grpAdvanced;
        private System.Windows.Forms.CheckBox chkTextFilesWithoutBom;
        private System.Windows.Forms.CheckBox chkRawMode;
        private System.Windows.Forms.CheckBox chkFullRawMode;
        private System.Windows.Forms.Label lblCulture;
        private gMKVToolNix.Controls.gComboBox cmbCulture;
        private System.Windows.Forms.Button btnTranslationEditor;
        private System.Windows.Forms.TableLayoutPanel tlpAttachments;
        private System.Windows.Forms.TableLayoutPanel tlpTags;
        private System.Windows.Forms.TableLayoutPanel tlpChapters;
        private System.Windows.Forms.TableLayoutPanel tlpVideoTracks;
        private System.Windows.Forms.TableLayoutPanel tlpAudioTracks;
        private System.Windows.Forms.TableLayoutPanel tlpSubtitleTracks;
        private System.Windows.Forms.TableLayoutPanel tlpAdvanced;
        private System.Windows.Forms.FlowLayoutPanel flpAdvancedUp;
        private System.Windows.Forms.FlowLayoutPanel flpAdvancedBottom;
        private System.Windows.Forms.Button btnDefaults;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}
