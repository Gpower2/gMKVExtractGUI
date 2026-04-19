using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix.Forms
{
    public partial class frmTranslationEditor : gForm
    {
        private sealed class TranslationEditorRow
        {
            public string Key { get; set; }
            public string Source { get; set; }
            public string Translation { get; set; }
            public bool IsTranslated { get; set; }
            public string Notes { get; set; }
        }

        private sealed class TranslationCultureLoadResult
        {
            public string Culture { get; set; }
            public TranslationFile TranslationFile { get; set; }
            public List<TranslationEditorRow> Rows { get; set; }
        }

        private sealed class NewLocaleRequest
        {
            public string Culture { get; set; }
            public string Translator { get; set; }
        }

        private readonly string _translationsDirectory;
        private readonly string _initialCulture;
        private readonly gSettings _settings;
        private readonly bool _skipRuntimeInitialization;

        private TranslationFile _masterFile;
        private TranslationFile _currentFile;
        private List<TranslationEditorRow> _allRows = new List<TranslationEditorRow>();
        private bool _isLoadingCulture;
        private bool _isBusy;
        private bool _hasPendingChanges;
        private string _selectedCulture;
        private string _busyStatusKey;

        public bool HasSavedChanges { get; private set; }

        public frmTranslationEditor()
            : this("en", true)
        {
        }

        public frmTranslationEditor(string initialCulture)
            : this(initialCulture, false)
        {
        }

        private frmTranslationEditor(string initialCulture, bool skipRuntimeInitialization)
        {
            try
            {
                _skipRuntimeInitialization = skipRuntimeInitialization;
                _translationsDirectory = this.GetCurrentDirectory();
                _initialCulture = string.IsNullOrWhiteSpace(initialCulture) ? "en" : initialCulture;

                _settings = _skipRuntimeInitialization ? null : new gSettings(GetCurrentDirectory());
                if (_settings != null)
                {
                    _settings.Reload();
                }

                InitializeComponent();

                if (_skipRuntimeInitialization)
                {
                    return;
                }

                _toolTip.AutoPopDelay = 15000;
                _toolTip.InitialDelay = 300;
                _toolTip.ReshowDelay = 100;
                _toolTip.ShowAlways = true;

                Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
                ThemeManager.ApplyTheme(this, _settings.DarkMode);

                if (this.Handle != IntPtr.Zero)
                {
                    NativeMethods.SetWindowThemeManaged(this.Handle, _settings.DarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, _settings.DarkMode);
                }
                else
                {
                    this.Shown += (s, ev) =>
                    {
                        NativeMethods.SetWindowThemeManaged(this.Handle, _settings.DarkMode);
                        NativeMethods.TrySetImmersiveDarkMode(this.Handle, _settings.DarkMode);
                    };
                }

                Shown += frmTranslationEditor_Shown;
                ApplyLocalization();
                LoadMasterFile();
                LoadAvailableCultures();
                SetSelectedCultureItem(FindCultureItem(_initialCulture) ?? (_cmbTargetCulture.Items.Count > 0 ? _cmbTargetCulture.Items[0] as string : null));
                InitDPI();
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(ex.ToString());
                if (!_skipRuntimeInitialization)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void LoadMasterFile()
        {
            string masterFile = TranslationPathService.GetMasterFilePath(_translationsDirectory);
            _masterFile = TranslationFileService.LoadFile(masterFile);
        }

        private void LoadAvailableCultures()
        {
            _isLoadingCulture = true;
            try
            {
                string selectedCulture = _cmbTargetCulture.SelectedItem as string;
                var cultures = TranslationPathService
                    .EnumerateTranslationFiles(_translationsDirectory)
                    .Select(path =>
                    {
                        string culture;
                        return TranslationPathService.TryGetCultureFromPath(path, out culture)
                            ? culture
                            : null;
                    })
                    .Where(culture => !string.IsNullOrWhiteSpace(culture))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(culture => culture)
                    .ToList();

                if (!cultures.Contains("en", StringComparer.OrdinalIgnoreCase))
                {
                    cultures.Insert(0, "en");
                }

                _cmbTargetCulture.Items.Clear();
                foreach (string culture in cultures)
                {
                    _cmbTargetCulture.Items.Add(culture);
                }

                string existingCulture = FindCultureItem(selectedCulture);
                if (!string.IsNullOrWhiteSpace(existingCulture))
                {
                    _cmbTargetCulture.SelectedItem = existingCulture;
                }
            }
            finally
            {
                _isLoadingCulture = false;
            }
        }

        private void LoadSelectedCulture(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return;
            }

            _isLoadingCulture = true;
            SetBusyState(true, "UI.TranslationEditor.Status.Loading");

            Task.Factory
                .StartNew(() => LoadCultureData(culture))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (IsDisposed || Disposing)
                        {
                            return;
                        }

                        if (task.IsFaulted)
                        {
                            throw task.Exception == null
                                ? new Exception("Failed to load locale.")
                                : task.Exception.GetBaseException();
                        }

                        ApplyLoadedCulture(task.Result);
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrWhiteSpace(_selectedCulture))
                        {
                            SetSelectedCultureItem(_selectedCulture);
                        }

                        gMKVLogger.Log(ex.ToString());
                        ShowErrorMessage(ex.Message);
                    }
                    finally
                    {
                        _isLoadingCulture = false;
                        SetBusyState(false);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private TranslationCultureLoadResult LoadCultureData(string culture)
        {
            string path = TranslationPathService.GetExistingTranslationFilePath(_translationsDirectory, culture);
            TranslationFile translationFile = TranslationFileService.LoadFile(path);

            return new TranslationCultureLoadResult
            {
                Culture = culture,
                TranslationFile = translationFile,
                Rows = translationFile.Entries
                    .OrderBy(entry => entry.Key)
                    .Select(entry => new TranslationEditorRow
                    {
                        Key = entry.Key,
                        Source = entry.Value.Source,
                        Translation = entry.Value.Translation,
                        IsTranslated = entry.Value.IsTranslated,
                        Notes = entry.Value.Notes
                    })
                    .ToList()
            };
        }

        private void ApplyLoadedCulture(TranslationCultureLoadResult loadResult)
        {
            if (loadResult == null)
            {
                return;
            }

            _currentFile = loadResult.TranslationFile;
            _selectedCulture = loadResult.Culture;
            _txtTranslator.Text = _currentFile.Metadata.Translator ?? string.Empty;
            _allRows = loadResult.Rows ?? new List<TranslationEditorRow>();

            ApplyFilters();
            SetPendingChanges(false);
            UpdateActionStates();
            ApplyResponsiveLayout();
        }

        private void ApplyFilters()
        {
            IEnumerable<TranslationEditorRow> filteredRows = _allRows;
            string searchText = _txtSearch.Text == null ? string.Empty : _txtSearch.Text.Trim();

            if (_chkShowOnlyUntranslated.Checked)
            {
                filteredRows = filteredRows.Where(row => !row.IsTranslated);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredRows = filteredRows.Where(row =>
                    Contains(row.Key, searchText)
                    || Contains(row.Source, searchText)
                    || Contains(row.Translation, searchText)
                    || Contains(row.Notes, searchText));
            }

            _bindingSource.DataSource = null;
            _bindingSource.DataSource = filteredRows.ToList();
            _bindingSource.ResetBindings(false);
            _translationsGrid.ClearSelection();
            _translationsGrid.Refresh();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            int translatedCount = _allRows.Count(row => row.IsTranslated);
            _lblSummary.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Summary", translatedCount, _allRows.Count);
        }

        private void UpdateActionStates()
        {
            bool hasCurrentFile = _currentFile != null;
            bool isEnglish = hasCurrentFile && string.Equals(_currentFile.Metadata.Culture, "en", StringComparison.OrdinalIgnoreCase);

            _btnCreate.Enabled = !_isBusy;
            _btnSave.Enabled = !_isBusy && hasCurrentFile && _hasPendingChanges;
            _btnSync.Enabled = !_isBusy && hasCurrentFile && !isEnglish;
            _btnClose.Enabled = !_isBusy;
            _lblSaveState.Text = _isBusy
                ? LocalizationManager.GetString(string.IsNullOrWhiteSpace(_busyStatusKey)
                    ? "UI.TranslationEditor.Status.Loading"
                    : _busyStatusKey)
                : LocalizationManager.GetString(_hasPendingChanges
                    ? "UI.TranslationEditor.Status.Dirty"
                    : "UI.TranslationEditor.Status.Clean");
            UpdateWindowTitle();
        }

        private void SetBusyState(bool isBusy, string busyStatusKey = null)
        {
            _isBusy = isBusy;
            _busyStatusKey = isBusy ? busyStatusKey : null;
            UseWaitCursor = isBusy;
            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
            _settingsGroup.Enabled = !isBusy;
            _translationsGroup.Enabled = !isBusy;
            UpdateActionStates();
        }

        private void ApplyTooltips()
        {
            _toolTip.SetToolTip(_btnCreate, LocalizationManager.GetString("UI.TranslationEditor.Tooltips.NewLocale"));
            _toolTip.SetToolTip(_btnSync, LocalizationManager.GetString("UI.TranslationEditor.Tooltips.Sync"));
        }

        private void frmTranslationEditor_Shown(object sender, EventArgs e)
        {
            Shown -= frmTranslationEditor_Shown;
            BeginInvoke((MethodInvoker)delegate
            {
                ApplyResponsiveLayout();

                string selectedCulture = _cmbTargetCulture.SelectedItem as string;
                if (_currentFile == null && !string.IsNullOrWhiteSpace(selectedCulture))
                {
                    LoadSelectedCulture(selectedCulture);
                }
            });
        }

        private void CommitGridChanges()
        {
            Validate();
            _translationsGrid.EndEdit();
            _bindingSource.EndEdit();
        }

        private void SaveCurrentFile()
        {
            if (_currentFile == null)
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.SelectCulture");
            }

            CommitGridChanges();

            _currentFile.Metadata.Translator = string.IsNullOrWhiteSpace(_txtTranslator.Text) ? null : _txtTranslator.Text.Trim();
            _currentFile.Metadata.LastEditDate = DateTime.UtcNow;

            foreach (TranslationEditorRow row in _allRows)
            {
                _currentFile.Entries[row.Key] = new TranslationEntry
                {
                    Source = row.Source,
                    Translation = row.Translation,
                    IsTranslated = row.IsTranslated,
                    Notes = row.Notes
                };
            }

            string path = TranslationPathService.GetTranslationFilePath(_translationsDirectory, _currentFile.Metadata.Culture);
            TranslationFileService.SaveFile(_currentFile, path);
            HasSavedChanges = true;
            SetPendingChanges(false);
        }

        private NewLocaleRequest ShowNewLocaleDialog()
        {
            using (var dialog = new gForm())
            {
                var layout = new gTableLayoutPanel();
                var lblCulture = new Label();
                var txtCulture = new gTextBox();
                var lblTranslator = new Label();
                var txtTranslator = new gTextBox();
                var actionsPanel = new FlowLayoutPanel();
                var btnCreate = new Button();
                var btnCancel = new Button();

                dialog.SuspendLayout();
                dialog.Font = Font;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ShowInTaskbar = false;
                dialog.ClientSize = new Size(420, 150);
                dialog.Text = LocalizationManager.GetString("UI.TranslationEditor.Dialogs.NewLocaleTitle");
                dialog.Icon = Icon;

                layout.ColumnCount = 2;
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                layout.RowCount = 3;
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                layout.Dock = DockStyle.Fill;
                layout.Padding = new Padding(12);

                lblCulture.AutoSize = true;
                lblCulture.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.NewCulture");
                lblCulture.Margin = new Padding(0, 6, 12, 0);

                txtCulture.Dock = DockStyle.Fill;
                txtCulture.Margin = new Padding(0, 0, 0, 8);

                lblTranslator.AutoSize = true;
                lblTranslator.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Translator");
                lblTranslator.Margin = new Padding(0, 6, 12, 0);

                txtTranslator.Dock = DockStyle.Fill;
                txtTranslator.Text = string.IsNullOrWhiteSpace(_txtTranslator.Text) ? string.Empty : _txtTranslator.Text.Trim();

                actionsPanel.Dock = DockStyle.Fill;
                actionsPanel.FlowDirection = FlowDirection.RightToLeft;
                actionsPanel.WrapContents = false;
                actionsPanel.Margin = new Padding(0, 12, 0, 0);

                btnCreate.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Create");
                btnCreate.ApplyLocalizedButtonSize(95);
                btnCreate.DialogResult = DialogResult.OK;

                btnCancel.Text = LocalizationManager.GetString("UI.OptionsForm.Actions.Cancel");
                btnCancel.ApplyLocalizedButtonSize(95);
                btnCancel.DialogResult = DialogResult.Cancel;

                actionsPanel.Controls.Add(btnCancel);
                actionsPanel.Controls.Add(btnCreate);

                layout.Controls.Add(lblCulture, 0, 0);
                layout.Controls.Add(txtCulture, 1, 0);
                layout.Controls.Add(lblTranslator, 0, 1);
                layout.Controls.Add(txtTranslator, 1, 1);
                layout.Controls.Add(actionsPanel, 0, 2);
                layout.SetColumnSpan(actionsPanel, 2);

                dialog.Controls.Add(layout);
                dialog.AcceptButton = btnCreate;
                dialog.CancelButton = btnCancel;

                ThemeManager.ApplyTheme(dialog, _settings.DarkMode);
                if (dialog.Handle != IntPtr.Zero)
                {
                    NativeMethods.SetWindowThemeManaged(dialog.Handle, _settings.DarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(dialog.Handle, _settings.DarkMode);
                }

                dialog.ResumeLayout(false);
                dialog.PerformLayout();

                return dialog.ShowDialog(this) == DialogResult.OK
                    ? new NewLocaleRequest
                    {
                        Culture = (txtCulture.Text ?? string.Empty).Trim().ToLowerInvariant(),
                        Translator = string.IsNullOrWhiteSpace(txtTranslator.Text) ? null : txtTranslator.Text.Trim()
                    }
                    : null;
            }
        }

        private void CreateNewCulture()
        {
            if (!ConfirmDiscardPendingChanges())
            {
                return;
            }

            NewLocaleRequest request = ShowNewLocaleDialog();
            if (request == null)
            {
                return;
            }

            string culture = request.Culture;
            if (string.IsNullOrWhiteSpace(culture))
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.CultureCodeRequired");
            }

            string path = TranslationPathService.GetTranslationFilePath(_translationsDirectory, culture);
            if (File.Exists(path))
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.LocaleExists", culture);
            }

            LoadMasterFile();

            var newFile = TranslationMaintenanceService.CreateTemplate(
                _masterFile,
                culture,
                request.Translator);

            TranslationFileService.SaveFile(newFile, path);
            HasSavedChanges = true;

            LoadAvailableCultures();
            SetSelectedCultureItem(culture);
            ShowLocalizedSuccessMessage("UI.TranslationEditor.Success.LocaleCreated", false, culture);
            LoadSelectedCulture(culture);
        }

        private void SyncCurrentCulture()
        {
            if (_currentFile == null)
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.SelectCulture");
            }

            if (string.Equals(_currentFile.Metadata.Culture, "en", StringComparison.OrdinalIgnoreCase))
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.CannotSyncEnglish");
            }

            CommitGridChanges();
            SaveCurrentFile();
            LoadMasterFile();

            var target = TranslationFileService.LoadFile(
                TranslationPathService.GetExistingTranslationFilePath(_translationsDirectory, _currentFile.Metadata.Culture));
            var result = TranslationMaintenanceService.Synchronize(_masterFile, target);

            TranslationFileService.SaveFile(
                result.TranslationFile,
                TranslationPathService.GetTranslationFilePath(_translationsDirectory, result.TranslationFile.Metadata.Culture));
            HasSavedChanges = true;

            ShowLocalizedSuccessMessage(
                "UI.TranslationEditor.Success.LocaleSynced",
                false,
                result.TranslationFile.Metadata.Culture,
                result.AddedCount,
                result.UpdatedCount,
                result.RemovedCount);

            SetSelectedCultureItem(result.TranslationFile.Metadata.Culture);
            LoadSelectedCulture(result.TranslationFile.Metadata.Culture);
        }

        private void ApplyLocalization()
        {
            _settingsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Settings.Group");
            _translationsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Grid.Group");
            _actionsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Group");

            _lblTargetCulture.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.TargetCulture");
            _lblTranslator.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Translator");
            _lblSearch.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Search");
            _chkShowOnlyUntranslated.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.ShowOnlyUntranslated");

            _btnCreate.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.NewLocale");
            _btnSync.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Sync");
            _btnSave.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Save");
            _btnClose.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Close");

            _translationsGrid.Columns["colKey"].HeaderText = LocalizationManager.GetString("UI.TranslationEditor.Columns.Key");
            _translationsGrid.Columns["colSource"].HeaderText = LocalizationManager.GetString("UI.TranslationEditor.Columns.Source");
            _translationsGrid.Columns["colTranslation"].HeaderText = LocalizationManager.GetString("UI.TranslationEditor.Columns.Translation");
            _translationsGrid.Columns["colIsTranslated"].HeaderText = LocalizationManager.GetString("UI.TranslationEditor.Columns.IsTranslated");
            _translationsGrid.Columns["colNotes"].HeaderText = LocalizationManager.GetString("UI.TranslationEditor.Columns.Notes");

            _btnCreate.ApplyLocalizedButtonSize(95);
            _btnSync.ApplyLocalizedButtonSize(95);
            _btnSave.ApplyLocalizedButtonSize(95);
            _btnClose.ApplyLocalizedButtonSize(95);

            ApplyTooltips();
            UpdateSummary();
            UpdateActionStates();
            ApplyResponsiveLayout();
        }

        private void FilterControl_Changed(object sender, EventArgs e)
        {
            if (!_isLoadingCulture && !_isBusy)
            {
                CommitGridChanges();
            }

            ApplyFilters();
        }

        private void cmbTargetCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoadingCulture)
            {
                return;
            }

            try
            {
                string selectedCulture = _cmbTargetCulture.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(selectedCulture)
                    || string.Equals(selectedCulture, _selectedCulture, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (!ConfirmDiscardPendingChanges())
                {
                    SetSelectedCultureItem(_selectedCulture);
                    return;
                }

                LoadSelectedCulture(selectedCulture);
            }
            catch (Exception ex)
            {
                SetSelectedCultureItem(_selectedCulture);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                CreateNewCulture();
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                SyncCurrentCulture();
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCurrentFile();
                ShowLocalizedSuccessMessage("UI.TranslationEditor.Success.LocaleSaved", false, _currentFile.Metadata.Culture);
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtTranslator_TextChanged(object sender, EventArgs e)
        {
            if (_isLoadingCulture || _currentFile == null)
            {
                return;
            }

            SetPendingChanges(true);
        }

        private void grdTranslations_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_translationsGrid.IsCurrentCellDirty)
            {
                _translationsGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void grdTranslations_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isLoadingCulture || e.RowIndex < 0)
            {
                return;
            }

            var row = _translationsGrid.Rows[e.RowIndex].DataBoundItem as TranslationEditorRow;
            if (row == null)
            {
                return;
            }

            string columnName = _translationsGrid.Columns[e.ColumnIndex].Name;
            if (string.Equals(columnName, "colTranslation", StringComparison.Ordinal))
            {
                row.IsTranslated = !string.IsNullOrWhiteSpace(row.Translation);
                _translationsGrid.InvalidateRow(e.RowIndex);
            }

            UpdateSummary();
            SetPendingChanges(true);
        }

        private void SetPendingChanges(bool hasPendingChanges)
        {
            if (_hasPendingChanges == hasPendingChanges)
            {
                return;
            }

            _hasPendingChanges = hasPendingChanges;
            UpdateActionStates();
            ApplyResponsiveLayout();
        }

        private void UpdateWindowTitle()
        {
            string title = LocalizationManager.GetString("UI.TranslationEditor.Title");
            Text = string.Format(
                "gMKVExtractGUI v{0} -- {1}{2}",
                GetCurrentVersion(),
                title,
                _hasPendingChanges ? " *" : string.Empty);
        }

        private bool ConfirmDiscardPendingChanges()
        {
            CommitGridChanges();
            if (!_hasPendingChanges)
            {
                return true;
            }

            return ShowLocalizedQuestion(
                "UI.TranslationEditor.Dialogs.DiscardChanges",
                "UI.Common.Dialog.AreYouSureTitle",
                false,
                _selectedCulture ?? string.Empty) == DialogResult.Yes;
        }

        private void SetSelectedCultureItem(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return;
            }

            string existingCulture = FindCultureItem(culture);
            if (string.IsNullOrWhiteSpace(existingCulture))
            {
                return;
            }

            _isLoadingCulture = true;
            try
            {
                _cmbTargetCulture.SelectedItem = existingCulture;
            }
            finally
            {
                _isLoadingCulture = false;
            }
        }

        private string FindCultureItem(string culture)
        {
            return _cmbTargetCulture.Items
                .Cast<object>()
                .Select(item => item as string)
                .FirstOrDefault(item => string.Equals(item, culture, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyResponsiveLayout()
        {
            if (_mainLayout.RowStyles.Count < 3)
            {
                return;
            }

            int availableWidth = Math.Max(320, ClientSize.Width - _mainLayout.Padding.Horizontal - 24);
            int contentWidth = Math.Max(280, availableWidth - 18);
            _settingsRow1.MaximumSize = new Size(contentWidth, 0);
            _settingsRow2.MaximumSize = new Size(contentWidth, 0);
            _settingsLayout.MaximumSize = new Size(contentWidth, 0);
            _settingsRow1.PerformLayout();
            _settingsRow2.PerformLayout();
            _settingsLayout.PerformLayout();
            _settingsGroup.PerformLayout();

            int summaryWidth = Math.Max(180, contentWidth - (_lblSearch.Width + _txtSearch.Width + 72));
            _lblSummary.MaximumSize = new Size(summaryWidth, 0);

            int settingsHeight = Math.Max(104, _settingsLayout.GetPreferredSize(new Size(contentWidth, 0)).Height + 28);
            _mainLayout.RowStyles[0].Height = Math.Max(104, settingsHeight);

            _actionsPanel.PerformLayout();
            _actionsLayout.PerformLayout();
            _actionsGroup.PerformLayout();
            int buttonsWidth = _actionsPanel.GetPreferredSize(Size.Empty).Width;
            int saveStateWidth = Math.Max(180, contentWidth - buttonsWidth - 24);
            _lblSaveState.MaximumSize = new Size(saveStateWidth, 0);

            int actionsHeight = Math.Max(84, _actionsLayout.GetPreferredSize(new Size(contentWidth, 0)).Height + 18);
            _mainLayout.RowStyles[2].Height = actionsHeight;
            _mainLayout.PerformLayout();
            PerformLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyResponsiveLayout();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!e.Cancel && !ConfirmDiscardPendingChanges())
            {
                e.Cancel = true;
                return;
            }

            DialogResult = HasSavedChanges ? DialogResult.OK : DialogResult.Cancel;
            base.OnFormClosing(e);
        }

        private static bool Contains(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
