namespace gMKVToolNix.Forms
{
    partial class frmTranslationEditor
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.BindingSource _bindingSource;
        private System.Windows.Forms.ToolTip _toolTip;
        private gMKVToolNix.gTableLayoutPanel _mainLayout;
        private gMKVToolNix.gGroupBox _settingsGroup;
        private gMKVToolNix.gGroupBox _translationsGroup;
        private gMKVToolNix.gGroupBox _actionsGroup;
        private gMKVToolNix.gTableLayoutPanel _settingsLayout;
        private gMKVToolNix.gTableLayoutPanel _actionsLayout;
        private System.Windows.Forms.FlowLayoutPanel _settingsRow1;
        private System.Windows.Forms.FlowLayoutPanel _settingsRow2;
        private System.Windows.Forms.FlowLayoutPanel _actionsPanel;
        private System.Windows.Forms.Label _lblTargetCulture;
        private gMKVToolNix.Controls.gComboBox _cmbTargetCulture;
        private System.Windows.Forms.Label _lblTranslator;
        private gMKVToolNix.gTextBox _txtTranslator;
        private System.Windows.Forms.Label _lblSearch;
        private gMKVToolNix.gTextBox _txtSearch;
        private System.Windows.Forms.CheckBox _chkShowOnlyUntranslated;
        private System.Windows.Forms.Label _lblSummary;
        private System.Windows.Forms.Label _lblSaveState;
        private gMKVToolNix.Controls.gDataGridView _translationsGrid;
        private System.Windows.Forms.Button _btnCreate;
        private System.Windows.Forms.Button _btnSync;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle sourceColumnStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle translationColumnStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle notesColumnStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewTextBoxColumn colKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            System.Windows.Forms.DataGridViewTextBoxColumn colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            System.Windows.Forms.DataGridViewTextBoxColumn colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            System.Windows.Forms.DataGridViewCheckBoxColumn colIsTranslated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            System.Windows.Forms.DataGridViewTextBoxColumn colNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.components = new System.ComponentModel.Container();
            this._bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._mainLayout = new gMKVToolNix.gTableLayoutPanel();
            this._settingsGroup = new gMKVToolNix.gGroupBox();
            this._settingsLayout = new gMKVToolNix.gTableLayoutPanel();
            this._settingsRow1 = new System.Windows.Forms.FlowLayoutPanel();
            this._lblTargetCulture = new System.Windows.Forms.Label();
            this._cmbTargetCulture = new gMKVToolNix.Controls.gComboBox();
            this._lblTranslator = new System.Windows.Forms.Label();
            this._txtTranslator = new gMKVToolNix.gTextBox();
            this._settingsRow2 = new System.Windows.Forms.FlowLayoutPanel();
            this._lblSearch = new System.Windows.Forms.Label();
            this._txtSearch = new gMKVToolNix.gTextBox();
            this._chkShowOnlyUntranslated = new System.Windows.Forms.CheckBox();
            this._lblSummary = new System.Windows.Forms.Label();
            this._translationsGroup = new gMKVToolNix.gGroupBox();
            this._translationsGrid = new gMKVToolNix.Controls.gDataGridView();
            this._actionsGroup = new gMKVToolNix.gGroupBox();
            this._actionsLayout = new gMKVToolNix.gTableLayoutPanel();
            this._lblSaveState = new System.Windows.Forms.Label();
            this._actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._btnClose = new System.Windows.Forms.Button();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnSync = new System.Windows.Forms.Button();
            this._btnCreate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            this._mainLayout.SuspendLayout();
            this._settingsGroup.SuspendLayout();
            this._settingsLayout.SuspendLayout();
            this._settingsRow1.SuspendLayout();
            this._settingsRow2.SuspendLayout();
            this._translationsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._translationsGrid)).BeginInit();
            this._actionsGroup.SuspendLayout();
            this._actionsLayout.SuspendLayout();
            this._actionsPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _mainLayout
            //
            this._mainLayout.ColumnCount = 1;
            this._mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainLayout.Controls.Add(this._settingsGroup, 0, 0);
            this._mainLayout.Controls.Add(this._translationsGroup, 0, 1);
            this._mainLayout.Controls.Add(this._actionsGroup, 0, 2);
            this._mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainLayout.Location = new System.Drawing.Point(0, 0);
            this._mainLayout.Name = "_mainLayout";
            this._mainLayout.Padding = new System.Windows.Forms.Padding(6);
            this._mainLayout.RowCount = 3;
            this._mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this._mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this._mainLayout.Size = new System.Drawing.Size(1100, 700);
            this._mainLayout.TabIndex = 0;
            //
            // _settingsGroup
            //
            this._settingsGroup.Controls.Add(this._settingsLayout);
            this._settingsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsGroup.Location = new System.Drawing.Point(9, 9);
            this._settingsGroup.Name = "_settingsGroup";
            this._settingsGroup.Size = new System.Drawing.Size(1082, 98);
            this._settingsGroup.TabIndex = 0;
            this._settingsGroup.TabStop = false;
            this._settingsGroup.Text = "Translation Settings";
            //
            // _settingsLayout
            //
            this._settingsLayout.ColumnCount = 1;
            this._settingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._settingsLayout.Controls.Add(this._settingsRow1, 0, 0);
            this._settingsLayout.Controls.Add(this._settingsRow2, 0, 1);
            this._settingsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsLayout.Location = new System.Drawing.Point(3, 19);
            this._settingsLayout.Name = "_settingsLayout";
            this._settingsLayout.Padding = new System.Windows.Forms.Padding(6, 0, 6, 6);
            this._settingsLayout.RowCount = 2;
            this._settingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._settingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._settingsLayout.Size = new System.Drawing.Size(1076, 76);
            this._settingsLayout.TabIndex = 0;
            //
            // _settingsRow1
            //
            this._settingsRow1.AutoSize = true;
            this._settingsRow1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._settingsRow1.Controls.Add(this._lblTargetCulture);
            this._settingsRow1.Controls.Add(this._cmbTargetCulture);
            this._settingsRow1.Controls.Add(this._lblTranslator);
            this._settingsRow1.Controls.Add(this._txtTranslator);
            this._settingsRow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsRow1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._settingsRow1.Location = new System.Drawing.Point(6, 0);
            this._settingsRow1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this._settingsRow1.Name = "_settingsRow1";
            this._settingsRow1.Padding = new System.Windows.Forms.Padding(0);
            this._settingsRow1.Size = new System.Drawing.Size(1064, 29);
            this._settingsRow1.TabIndex = 0;
            this._settingsRow1.WrapContents = true;
            //
            // _lblTargetCulture
            //
            this._lblTargetCulture.AutoSize = true;
            this._lblTargetCulture.Location = new System.Drawing.Point(6, 8);
            this._lblTargetCulture.Margin = new System.Windows.Forms.Padding(6, 8, 0, 0);
            this._lblTargetCulture.Name = "_lblTargetCulture";
            this._lblTargetCulture.Size = new System.Drawing.Size(83, 15);
            this._lblTargetCulture.TabIndex = 0;
            this._lblTargetCulture.Text = "Target Culture";
            //
            // _cmbTargetCulture
            //
            this._cmbTargetCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbTargetCulture.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this._cmbTargetCulture.FormattingEnabled = true;
            this._cmbTargetCulture.Location = new System.Drawing.Point(95, 3);
            this._cmbTargetCulture.Margin = new System.Windows.Forms.Padding(6, 3, 0, 0);
            this._cmbTargetCulture.Name = "_cmbTargetCulture";
            this._cmbTargetCulture.Size = new System.Drawing.Size(120, 23);
            this._cmbTargetCulture.TabIndex = 1;
            this._cmbTargetCulture.SelectedIndexChanged += new System.EventHandler(this.cmbTargetCulture_SelectedIndexChanged);
            //
            // _lblTranslator
            //
            this._lblTranslator.AutoSize = true;
            this._lblTranslator.Location = new System.Drawing.Point(227, 8);
            this._lblTranslator.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this._lblTranslator.Name = "_lblTranslator";
            this._lblTranslator.Size = new System.Drawing.Size(56, 15);
            this._lblTranslator.TabIndex = 2;
            this._lblTranslator.Text = "Translator";
            //
            // _txtTranslator
            //
            this._txtTranslator.Location = new System.Drawing.Point(289, 3);
            this._txtTranslator.Margin = new System.Windows.Forms.Padding(6, 3, 0, 0);
            this._txtTranslator.Name = "_txtTranslator";
            this._txtTranslator.Size = new System.Drawing.Size(220, 23);
            this._txtTranslator.TabIndex = 3;
            this._txtTranslator.TextChanged += new System.EventHandler(this.txtTranslator_TextChanged);
            //
            // _settingsRow2
            //
            this._settingsRow2.AutoSize = true;
            this._settingsRow2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._settingsRow2.Controls.Add(this._lblSearch);
            this._settingsRow2.Controls.Add(this._txtSearch);
            this._settingsRow2.Controls.Add(this._chkShowOnlyUntranslated);
            this._settingsRow2.Controls.Add(this._lblSummary);
            this._settingsRow2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsRow2.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._settingsRow2.Location = new System.Drawing.Point(6, 35);
            this._settingsRow2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this._settingsRow2.Name = "_settingsRow2";
            this._settingsRow2.Padding = new System.Windows.Forms.Padding(0);
            this._settingsRow2.Size = new System.Drawing.Size(1064, 29);
            this._settingsRow2.TabIndex = 1;
            this._settingsRow2.WrapContents = true;
            //
            // _lblSearch
            //
            this._lblSearch.AutoSize = true;
            this._lblSearch.Location = new System.Drawing.Point(6, 8);
            this._lblSearch.Margin = new System.Windows.Forms.Padding(6, 8, 0, 0);
            this._lblSearch.Name = "_lblSearch";
            this._lblSearch.Size = new System.Drawing.Size(42, 15);
            this._lblSearch.TabIndex = 0;
            this._lblSearch.Text = "Search";
            //
            // _txtSearch
            //
            this._txtSearch.Location = new System.Drawing.Point(54, 3);
            this._txtSearch.Margin = new System.Windows.Forms.Padding(6, 3, 0, 0);
            this._txtSearch.Name = "_txtSearch";
            this._txtSearch.Size = new System.Drawing.Size(260, 23);
            this._txtSearch.TabIndex = 1;
            this._txtSearch.TextChanged += new System.EventHandler(this.FilterControl_Changed);
            //
            // _chkShowOnlyUntranslated
            //
            this._chkShowOnlyUntranslated.AutoSize = true;
            this._chkShowOnlyUntranslated.Location = new System.Drawing.Point(326, 7);
            this._chkShowOnlyUntranslated.Margin = new System.Windows.Forms.Padding(12, 7, 0, 0);
            this._chkShowOnlyUntranslated.Name = "_chkShowOnlyUntranslated";
            this._chkShowOnlyUntranslated.Size = new System.Drawing.Size(147, 19);
            this._chkShowOnlyUntranslated.TabIndex = 2;
            this._chkShowOnlyUntranslated.Text = "Show only untranslated";
            this._chkShowOnlyUntranslated.UseVisualStyleBackColor = true;
            this._chkShowOnlyUntranslated.CheckedChanged += new System.EventHandler(this.FilterControl_Changed);
            //
            // _lblSummary
            //
            this._lblSummary.AutoSize = true;
            this._lblSummary.Location = new System.Drawing.Point(485, 8);
            this._lblSummary.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this._lblSummary.Name = "_lblSummary";
            this._lblSummary.Size = new System.Drawing.Size(27, 15);
            this._lblSummary.TabIndex = 3;
            this._lblSummary.Text = "0 / 0";
            //
            // _translationsGroup
            //
            this._translationsGroup.Controls.Add(this._translationsGrid);
            this._translationsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._translationsGroup.Location = new System.Drawing.Point(9, 113);
            this._translationsGroup.Name = "_translationsGroup";
            this._translationsGroup.Size = new System.Drawing.Size(1082, 510);
            this._translationsGroup.TabIndex = 1;
            this._translationsGroup.TabStop = false;
            this._translationsGroup.Text = "Translations";
            //
            // _translationsGrid
            //
            this._translationsGrid.AutoGenerateColumns = false;
            this._translationsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._translationsGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this._translationsGrid.BackgroundColor = System.Drawing.Color.White;
            this._translationsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._translationsGrid.DataSource = this._bindingSource;
            this._translationsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._translationsGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._translationsGrid.Location = new System.Drawing.Point(3, 19);
            this._translationsGrid.MultiSelect = false;
            this._translationsGrid.Name = "_translationsGrid";
            this._translationsGrid.RowHeadersVisible = false;
            this._translationsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._translationsGrid.Size = new System.Drawing.Size(1076, 488);
            this._translationsGrid.TabIndex = 0;
            this._translationsGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.grdTranslations_CurrentCellDirtyStateChanged);
            this._translationsGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdTranslations_CellValueChanged);
            //
            // colKey
            //
            colKey.DataPropertyName = "Key";
            colKey.FillWeight = 24F;
            colKey.MinimumWidth = 120;
            colKey.Name = "colKey";
            colKey.ReadOnly = true;
            colKey.HeaderText = "Key";
            //
            // colSource
            //
            sourceColumnStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            colSource.DataPropertyName = "Source";
            colSource.DefaultCellStyle = sourceColumnStyle;
            colSource.FillWeight = 28F;
            colSource.MinimumWidth = 180;
            colSource.Name = "colSource";
            colSource.ReadOnly = true;
            colSource.HeaderText = "Source";
            //
            // colTranslation
            //
            translationColumnStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            colTranslation.DataPropertyName = "Translation";
            colTranslation.DefaultCellStyle = translationColumnStyle;
            colTranslation.FillWeight = 28F;
            colTranslation.MinimumWidth = 180;
            colTranslation.Name = "colTranslation";
            colTranslation.HeaderText = "Translation";
            //
            // colIsTranslated
            //
            colIsTranslated.DataPropertyName = "IsTranslated";
            colIsTranslated.FillWeight = 10F;
            colIsTranslated.MinimumWidth = 80;
            colIsTranslated.Name = "colIsTranslated";
            colIsTranslated.HeaderText = "Translated";
            //
            // colNotes
            //
            notesColumnStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            colNotes.DataPropertyName = "Notes";
            colNotes.DefaultCellStyle = notesColumnStyle;
            colNotes.FillWeight = 20F;
            colNotes.MinimumWidth = 140;
            colNotes.Name = "colNotes";
            colNotes.ReadOnly = true;
            colNotes.HeaderText = "Notes";
            this._translationsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            colKey,
            colSource,
            colTranslation,
            colIsTranslated,
            colNotes});
            //
            // _actionsGroup
            //
            this._actionsGroup.Controls.Add(this._actionsLayout);
            this._actionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionsGroup.Location = new System.Drawing.Point(9, 629);
            this._actionsGroup.Name = "_actionsGroup";
            this._actionsGroup.Size = new System.Drawing.Size(1082, 62);
            this._actionsGroup.TabIndex = 2;
            this._actionsGroup.TabStop = false;
            this._actionsGroup.Text = "Actions";
            //
            // _actionsLayout
            //
            this._actionsLayout.ColumnCount = 2;
            this._actionsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._actionsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._actionsLayout.Controls.Add(this._lblSaveState, 0, 0);
            this._actionsLayout.Controls.Add(this._actionsPanel, 1, 0);
            this._actionsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionsLayout.Location = new System.Drawing.Point(3, 19);
            this._actionsLayout.Name = "_actionsLayout";
            this._actionsLayout.Padding = new System.Windows.Forms.Padding(6, 10, 6, 6);
            this._actionsLayout.RowCount = 1;
            this._actionsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._actionsLayout.Size = new System.Drawing.Size(1076, 40);
            this._actionsLayout.TabIndex = 0;
            //
            // _lblSaveState
            //
            this._lblSaveState.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblSaveState.AutoSize = true;
            this._lblSaveState.Location = new System.Drawing.Point(9, 12);
            this._lblSaveState.Margin = new System.Windows.Forms.Padding(0, 6, 12, 0);
            this._lblSaveState.Name = "_lblSaveState";
            this._lblSaveState.Size = new System.Drawing.Size(0, 15);
            this._lblSaveState.TabIndex = 0;
            //
            // _actionsPanel
            //
            this._actionsPanel.AutoSize = true;
            this._actionsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._actionsPanel.Controls.Add(this._btnClose);
            this._actionsPanel.Controls.Add(this._btnSave);
            this._actionsPanel.Controls.Add(this._btnSync);
            this._actionsPanel.Controls.Add(this._btnCreate);
            this._actionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._actionsPanel.Location = new System.Drawing.Point(634, 10);
            this._actionsPanel.Margin = new System.Windows.Forms.Padding(0);
            this._actionsPanel.Name = "_actionsPanel";
            this._actionsPanel.Padding = new System.Windows.Forms.Padding(0);
            this._actionsPanel.Size = new System.Drawing.Size(436, 30);
            this._actionsPanel.TabIndex = 1;
            this._actionsPanel.WrapContents = false;
            //
            // _btnClose
            //
            this._btnClose.Location = new System.Drawing.Point(341, 0);
            this._btnClose.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(95, 30);
            this._btnClose.TabIndex = 3;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // _btnSave
            //
            this._btnSave.Location = new System.Drawing.Point(240, 0);
            this._btnSave.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(95, 30);
            this._btnSave.TabIndex = 2;
            this._btnSave.Text = "Save";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // _btnSync
            //
            this._btnSync.Location = new System.Drawing.Point(139, 0);
            this._btnSync.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._btnSync.Name = "_btnSync";
            this._btnSync.Size = new System.Drawing.Size(95, 30);
            this._btnSync.TabIndex = 1;
            this._btnSync.Text = "Sync";
            this._btnSync.UseVisualStyleBackColor = true;
            this._btnSync.Click += new System.EventHandler(this.btnSync_Click);
            //
            // _btnCreate
            //
            this._btnCreate.Location = new System.Drawing.Point(38, 0);
            this._btnCreate.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._btnCreate.Name = "_btnCreate";
            this._btnCreate.Size = new System.Drawing.Size(95, 30);
            this._btnCreate.TabIndex = 0;
            this._btnCreate.Text = "New Locale...";
            this._btnCreate.UseVisualStyleBackColor = true;
            this._btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // frmTranslationEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this._mainLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "frmTranslationEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Translation Editor";
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            this._mainLayout.ResumeLayout(false);
            this._settingsGroup.ResumeLayout(false);
            this._settingsLayout.ResumeLayout(false);
            this._settingsLayout.PerformLayout();
            this._settingsRow1.ResumeLayout(false);
            this._settingsRow1.PerformLayout();
            this._settingsRow2.ResumeLayout(false);
            this._settingsRow2.PerformLayout();
            this._translationsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._translationsGrid)).EndInit();
            this._actionsGroup.ResumeLayout(false);
            this._actionsLayout.ResumeLayout(false);
            this._actionsLayout.PerformLayout();
            this._actionsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
