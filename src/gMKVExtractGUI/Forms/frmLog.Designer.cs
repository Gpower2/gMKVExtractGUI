namespace gMKVToolNix
{
    partial class frmLog
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
            this.tlpMain = new gMKVToolNix.gTableLayoutPanel();
            this.grpLog = new gMKVToolNix.gGroupBox();
            this.txtLog = new gMKVToolNix.gRichTextBox();
            this.grpActions = new gMKVToolNix.gGroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpLog, 0, 0);
            this.tlpMain.Controls.Add(this.grpActions, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.Size = new System.Drawing.Size(604, 501);
            this.tlpMain.TabIndex = 0;
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(3, 3);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(598, 435);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtLog.Location = new System.Drawing.Point(3, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(592, 413);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnSave);
            this.grpActions.Controls.Add(this.btnClear);
            this.grpActions.Controls.Add(this.btnRefresh);
            this.grpActions.Controls.Add(this.btnCopy);
            this.grpActions.Controls.Add(this.btnClose);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 444);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(598, 54);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(105, 17);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 30);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(6, 17);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(95, 30);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear Log";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(300, 17);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(399, 17);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(95, 30);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy Selection";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(498, 17);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(95, 30);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(604, 501);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "frmLog";
            this.Text = "frmLog";
            this.Activated += new System.EventHandler(this.frmLog_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private gTableLayoutPanel tlpMain;
        private gGroupBox grpLog;
        private gRichTextBox txtLog;
        private gGroupBox grpActions;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
    }
}