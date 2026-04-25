namespace gMKVToolNix
{
    partial class frmJobManager
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
            this.grpProgress = new gMKVToolNix.gGroupBox();
            this.txtCurrentTrack = new gMKVToolNix.gTextBox();
            this.lblCurrentTrack = new System.Windows.Forms.Label();
            this.lblTotalProgressValue = new System.Windows.Forms.Label();
            this.lblCurrentProgressValue = new System.Windows.Forms.Label();
            this.lblTotalProgress = new System.Windows.Forms.Label();
            this.lblCurrentProgress = new System.Windows.Forms.Label();
            this.prgBrTotal = new System.Windows.Forms.ProgressBar();
            this.prgBrCurrent = new System.Windows.Forms.ProgressBar();
            this.tlpJobs = new gMKVToolNix.gTableLayoutPanel();
            this.grpJobs = new gMKVToolNix.gGroupBox();
            this.grdJobs = new gMKVToolNix.Controls.gDataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeToReadyStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpActions = new gMKVToolNix.gGroupBox();
            this.chkShowPopup = new System.Windows.Forms.CheckBox();
            this.btnSaveJobs = new System.Windows.Forms.Button();
            this.btnLoadJobs = new System.Windows.Forms.Button();
            this.btnAbortAll = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tlpMain.SuspendLayout();
            this.grpProgress.SuspendLayout();
            this.tlpJobs.SuspendLayout();
            this.grpJobs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdJobs)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpProgress, 0, 1);
            this.tlpMain.Controls.Add(this.tlpJobs, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpMain.Size = new System.Drawing.Size(624, 431);
            this.tlpMain.TabIndex = 5;
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.txtCurrentTrack);
            this.grpProgress.Controls.Add(this.lblCurrentTrack);
            this.grpProgress.Controls.Add(this.lblTotalProgressValue);
            this.grpProgress.Controls.Add(this.lblCurrentProgressValue);
            this.grpProgress.Controls.Add(this.lblTotalProgress);
            this.grpProgress.Controls.Add(this.lblCurrentProgress);
            this.grpProgress.Controls.Add(this.prgBrTotal);
            this.grpProgress.Controls.Add(this.prgBrCurrent);
            this.grpProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProgress.Location = new System.Drawing.Point(3, 324);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(618, 104);
            this.grpProgress.TabIndex = 3;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Progress";
            // 
            // txtCurrentTrack
            // 
            this.txtCurrentTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentTrack.BackColor = System.Drawing.SystemColors.Window;
            this.txtCurrentTrack.Location = new System.Drawing.Point(107, 18);
            this.txtCurrentTrack.Name = "txtCurrentTrack";
            this.txtCurrentTrack.ReadOnly = true;
            this.txtCurrentTrack.Size = new System.Drawing.Size(460, 23);
            this.txtCurrentTrack.TabIndex = 8;
            // 
            // lblCurrentTrack
            // 
            this.lblCurrentTrack.AutoSize = true;
            this.lblCurrentTrack.Location = new System.Drawing.Point(8, 22);
            this.lblCurrentTrack.Name = "lblCurrentTrack";
            this.lblCurrentTrack.Size = new System.Drawing.Size(78, 15);
            this.lblCurrentTrack.TabIndex = 7;
            this.lblCurrentTrack.Text = "Current Track";
            // 
            // lblTotalProgressValue
            // 
            this.lblTotalProgressValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalProgressValue.AutoSize = true;
            this.lblTotalProgressValue.Location = new System.Drawing.Point(573, 79);
            this.lblTotalProgressValue.Name = "lblTotalProgressValue";
            this.lblTotalProgressValue.Size = new System.Drawing.Size(0, 15);
            this.lblTotalProgressValue.TabIndex = 6;
            // 
            // lblCurrentProgressValue
            // 
            this.lblCurrentProgressValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentProgressValue.AutoSize = true;
            this.lblCurrentProgressValue.Location = new System.Drawing.Point(573, 51);
            this.lblCurrentProgressValue.Name = "lblCurrentProgressValue";
            this.lblCurrentProgressValue.Size = new System.Drawing.Size(0, 15);
            this.lblCurrentProgressValue.TabIndex = 5;
            // 
            // lblTotalProgress
            // 
            this.lblTotalProgress.AutoSize = true;
            this.lblTotalProgress.Location = new System.Drawing.Point(8, 79);
            this.lblTotalProgress.Name = "lblTotalProgress";
            this.lblTotalProgress.Size = new System.Drawing.Size(81, 15);
            this.lblTotalProgress.TabIndex = 4;
            this.lblTotalProgress.Text = "Total Progress";
            // 
            // lblCurrentProgress
            // 
            this.lblCurrentProgress.AutoSize = true;
            this.lblCurrentProgress.Location = new System.Drawing.Point(8, 51);
            this.lblCurrentProgress.Name = "lblCurrentProgress";
            this.lblCurrentProgress.Size = new System.Drawing.Size(95, 15);
            this.lblCurrentProgress.TabIndex = 3;
            this.lblCurrentProgress.Text = "Current Progress";
            // 
            // prgBrTotal
            // 
            this.prgBrTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBrTotal.Location = new System.Drawing.Point(107, 76);
            this.prgBrTotal.Name = "prgBrTotal";
            this.prgBrTotal.Size = new System.Drawing.Size(460, 20);
            this.prgBrTotal.TabIndex = 2;
            // 
            // prgBrCurrent
            // 
            this.prgBrCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBrCurrent.Location = new System.Drawing.Point(107, 48);
            this.prgBrCurrent.Name = "prgBrCurrent";
            this.prgBrCurrent.Size = new System.Drawing.Size(460, 20);
            this.prgBrCurrent.TabIndex = 1;
            // 
            // tlpJobs
            // 
            this.tlpJobs.ColumnCount = 2;
            this.tlpJobs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpJobs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpJobs.Controls.Add(this.grpJobs, 0, 0);
            this.tlpJobs.Controls.Add(this.grpActions, 1, 0);
            this.tlpJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpJobs.Location = new System.Drawing.Point(0, 0);
            this.tlpJobs.Margin = new System.Windows.Forms.Padding(0);
            this.tlpJobs.Name = "tlpJobs";
            this.tlpJobs.RowCount = 1;
            this.tlpJobs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpJobs.Size = new System.Drawing.Size(624, 321);
            this.tlpJobs.TabIndex = 4;
            // 
            // grpJobs
            // 
            this.grpJobs.Controls.Add(this.grdJobs);
            this.grpJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpJobs.Location = new System.Drawing.Point(3, 3);
            this.grpJobs.Name = "grpJobs";
            this.grpJobs.Size = new System.Drawing.Size(508, 315);
            this.grpJobs.TabIndex = 0;
            this.grpJobs.TabStop = false;
            this.grpJobs.Text = "Jobs";
            // 
            // grdJobs
            // 
            this.grdJobs.AllowUserToAddRows = false;
            this.grdJobs.AllowUserToDeleteRows = false;
            this.grdJobs.AllowUserToResizeRows = false;
            this.grdJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdJobs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.grdJobs.BackgroundColor = System.Drawing.Color.White;
            this.grdJobs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdJobs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdJobs.ContextMenuStrip = this.contextMenuStrip;
            this.grdJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdJobs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdJobs.Location = new System.Drawing.Point(3, 19);
            this.grdJobs.Name = "grdJobs";
            this.grdJobs.ReadOnly = true;
            this.grdJobs.RowHeadersVisible = false;
            this.grdJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdJobs.Size = new System.Drawing.Size(502, 293);
            this.grdJobs.TabIndex = 1;
            this.grdJobs.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdJobs_CellContentDoubleClick);
            this.grdJobs.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.grdJobs_DataBindingComplete);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.changeToReadyStatusToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(200, 98);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // changeToReadyStatusToolStripMenuItem
            // 
            this.changeToReadyStatusToolStripMenuItem.Name = "changeToReadyStatusToolStripMenuItem";
            this.changeToReadyStatusToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.changeToReadyStatusToolStripMenuItem.Text = "Change to Ready Status";
            this.changeToReadyStatusToolStripMenuItem.Click += new System.EventHandler(this.changeToReadyStatusToolStripMenuItem_Click);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.chkShowPopup);
            this.grpActions.Controls.Add(this.btnSaveJobs);
            this.grpActions.Controls.Add(this.btnLoadJobs);
            this.grpActions.Controls.Add(this.btnAbortAll);
            this.grpActions.Controls.Add(this.btnAbort);
            this.grpActions.Controls.Add(this.btnRunAll);
            this.grpActions.Controls.Add(this.btnRemove);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(517, 3);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(104, 315);
            this.grpActions.TabIndex = 4;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // chkShowPopup
            // 
            this.chkShowPopup.AutoSize = true;
            this.chkShowPopup.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowPopup.Location = new System.Drawing.Point(20, 69);
            this.chkShowPopup.Name = "chkShowPopup";
            this.chkShowPopup.Size = new System.Drawing.Size(61, 19);
            this.chkShowPopup.TabIndex = 13;
            this.chkShowPopup.Text = "Popup";
            this.chkShowPopup.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowPopup.UseVisualStyleBackColor = true;
            this.chkShowPopup.CheckedChanged += new System.EventHandler(this.chkShowPopup_CheckedChanged);
            // 
            // btnSaveJobs
            // 
            this.btnSaveJobs.Location = new System.Drawing.Point(7, 186);
            this.btnSaveJobs.Name = "btnSaveJobs";
            this.btnSaveJobs.Size = new System.Drawing.Size(90, 30);
            this.btnSaveJobs.TabIndex = 6;
            this.btnSaveJobs.Text = "Save Jobs...";
            this.btnSaveJobs.UseVisualStyleBackColor = true;
            this.btnSaveJobs.Click += new System.EventHandler(this.btnSaveJobs_Click);
            // 
            // btnLoadJobs
            // 
            this.btnLoadJobs.Location = new System.Drawing.Point(7, 150);
            this.btnLoadJobs.Name = "btnLoadJobs";
            this.btnLoadJobs.Size = new System.Drawing.Size(90, 30);
            this.btnLoadJobs.TabIndex = 5;
            this.btnLoadJobs.Text = "Load Jobs...";
            this.btnLoadJobs.UseVisualStyleBackColor = true;
            this.btnLoadJobs.Click += new System.EventHandler(this.btnLoadJobs_Click);
            // 
            // btnAbortAll
            // 
            this.btnAbortAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAbortAll.Location = new System.Drawing.Point(7, 282);
            this.btnAbortAll.Name = "btnAbortAll";
            this.btnAbortAll.Size = new System.Drawing.Size(90, 30);
            this.btnAbortAll.TabIndex = 4;
            this.btnAbortAll.Text = "Abort All";
            this.btnAbortAll.UseVisualStyleBackColor = true;
            this.btnAbortAll.Click += new System.EventHandler(this.btnAbortAll_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAbort.Location = new System.Drawing.Point(7, 246);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(90, 30);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnRunAll
            // 
            this.btnRunAll.Location = new System.Drawing.Point(7, 93);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(90, 30);
            this.btnRunAll.TabIndex = 2;
            this.btnRunAll.Text = "Run Jobs";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(7, 18);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 30);
            this.btnRemove.TabIndex = 0;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.deselectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // frmJobManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 431);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(640, 470);
            this.Name = "frmJobManager";
            this.Text = "frmJobManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmJobManager_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.tlpJobs.ResumeLayout(false);
            this.grpJobs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdJobs)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private gGroupBox grpJobs;
        private System.Windows.Forms.ProgressBar prgBrCurrent;
        private System.Windows.Forms.ProgressBar prgBrTotal;
        private gGroupBox grpProgress;
        private System.Windows.Forms.Label lblTotalProgressValue;
        private System.Windows.Forms.Label lblCurrentProgressValue;
        private System.Windows.Forms.Label lblTotalProgress;
        private System.Windows.Forms.Label lblCurrentProgress;
        private gGroupBox grpActions;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRunAll;
        private gTableLayoutPanel tlpMain;
        private gTableLayoutPanel tlpJobs;
        private System.Windows.Forms.Button btnAbortAll;
        private System.Windows.Forms.Button btnAbort;
        private Controls.gDataGridView grdJobs;
        private System.Windows.Forms.Button btnSaveJobs;
        private System.Windows.Forms.Button btnLoadJobs;
        private gTextBox txtCurrentTrack;
        private System.Windows.Forms.Label lblCurrentTrack;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem changeToReadyStatusToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkShowPopup;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}