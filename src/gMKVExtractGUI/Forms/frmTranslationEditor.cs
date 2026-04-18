using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;
using gRoot = gMKVToolNix;

namespace gMKVToolNix.Forms
{
    public class frmTranslationEditor : gForm
    {
        private sealed class TranslationEditorRow
        {
            public string Key { get; set; }
            public string Source { get; set; }
            public string Translation { get; set; }
            public bool IsTranslated { get; set; }
            public string Notes { get; set; }
        }

        private readonly string _translationsDirectory;
        private readonly string _initialCulture;
        private readonly gSettings _settings;
        private readonly BindingSource _bindingSource = new BindingSource();

        private TranslationFile _masterFile;
        private TranslationFile _currentFile;
        private List<TranslationEditorRow> _allRows = new List<TranslationEditorRow>();
        private bool _isLoadingCulture;
        private bool _hasPendingChanges;
        private string _selectedCulture;

        private readonly gRoot.gTableLayoutPanel _mainLayout = new gRoot.gTableLayoutPanel();
        private readonly gRoot.gGroupBox _settingsGroup = new gRoot.gGroupBox();
        private readonly gRoot.gGroupBox _translationsGroup = new gRoot.gGroupBox();
        private readonly gRoot.gGroupBox _actionsGroup = new gRoot.gGroupBox();
        private readonly gRoot.gTableLayoutPanel _settingsLayout = new gRoot.gTableLayoutPanel();
        private readonly gRoot.gTableLayoutPanel _actionsLayout = new gRoot.gTableLayoutPanel();
        private readonly FlowLayoutPanel _settingsRow1 = new FlowLayoutPanel();
        private readonly FlowLayoutPanel _settingsRow2 = new FlowLayoutPanel();
        private readonly FlowLayoutPanel _actionsPanel = new FlowLayoutPanel();
        private readonly Label _lblTargetCulture = new Label();
        private readonly gComboBox _cmbTargetCulture = new gComboBox();
        private readonly Label _lblNewCulture = new Label();
        private readonly gRoot.gTextBox _txtNewCulture = new gRoot.gTextBox();
        private readonly Label _lblTranslator = new Label();
        private readonly gRoot.gTextBox _txtTranslator = new gRoot.gTextBox();
        private readonly Label _lblSearch = new Label();
        private readonly gRoot.gTextBox _txtSearch = new gRoot.gTextBox();
        private readonly CheckBox _chkShowOnlyUntranslated = new CheckBox();
        private readonly Label _lblSummary = new Label();
        private readonly Label _lblSaveState = new Label();
        private readonly gDataGridView _translationsGrid = new gDataGridView();
        private readonly Button _btnCreate = new Button();
        private readonly Button _btnSync = new Button();
        private readonly Button _btnSave = new Button();
        private readonly Button _btnClose = new Button();

        public bool HasSavedChanges { get; private set; }

        public frmTranslationEditor(string translationsDirectory, string initialCulture)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(translationsDirectory))
                {
                    throw new ArgumentException("A translations directory is required.", nameof(translationsDirectory));
                }

                _translationsDirectory = translationsDirectory;
                _initialCulture = string.IsNullOrWhiteSpace(initialCulture) ? "en" : initialCulture;

                _settings = new gSettings(GetCurrentDirectory());
                _settings.Reload();

                InitializeComponent();

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

                ApplyLocalization();
                LoadMasterFile();
                LoadAvailableCultures();
                SelectCulture(_initialCulture);
                InitDPI();
                ApplyResponsiveLayout();
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 161);
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterParent;
            Name = "frmTranslationEditor";

            _mainLayout.ColumnCount = 1;
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _mainLayout.RowCount = 3;
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            _mainLayout.Dock = DockStyle.Fill;
            _mainLayout.Padding = new Padding(6);

            _settingsGroup.Dock = DockStyle.Fill;
            _settingsGroup.Controls.Add(_settingsLayout);

            _settingsLayout.ColumnCount = 1;
            _settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _settingsLayout.RowCount = 2;
            _settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _settingsLayout.Dock = DockStyle.Fill;
            _settingsLayout.Padding = new Padding(6, 0, 6, 6);

            ConfigureSettingsRow(_settingsRow1);
            ConfigureSettingsRow(_settingsRow2);

            _lblTargetCulture.AutoSize = true;
            _lblTargetCulture.Margin = new Padding(6, 8, 0, 0);
            _cmbTargetCulture.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbTargetCulture.Width = 120;
            _cmbTargetCulture.Margin = new Padding(6, 3, 0, 0);
            _cmbTargetCulture.SelectedIndexChanged += cmbTargetCulture_SelectedIndexChanged;

            _lblNewCulture.AutoSize = true;
            _lblNewCulture.Margin = new Padding(12, 8, 0, 0);
            _txtNewCulture.Width = 110;
            _txtNewCulture.Margin = new Padding(6, 3, 0, 0);

            _lblTranslator.AutoSize = true;
            _lblTranslator.Margin = new Padding(12, 8, 0, 0);
            _txtTranslator.Width = 220;
            _txtTranslator.Margin = new Padding(6, 3, 0, 0);
            _txtTranslator.TextChanged += txtTranslator_TextChanged;

            _lblSearch.AutoSize = true;
            _lblSearch.Margin = new Padding(6, 8, 0, 0);
            _txtSearch.Width = 260;
            _txtSearch.Margin = new Padding(6, 3, 0, 0);
            _txtSearch.TextChanged += FilterControl_Changed;

            _chkShowOnlyUntranslated.AutoSize = true;
            _chkShowOnlyUntranslated.Margin = new Padding(12, 7, 0, 0);
            _chkShowOnlyUntranslated.CheckedChanged += FilterControl_Changed;

            _lblSummary.AutoSize = true;
            _lblSummary.Margin = new Padding(12, 8, 0, 0);

            _settingsRow1.Controls.Add(_lblTargetCulture);
            _settingsRow1.Controls.Add(_cmbTargetCulture);
            _settingsRow1.Controls.Add(_lblNewCulture);
            _settingsRow1.Controls.Add(_txtNewCulture);
            _settingsRow1.Controls.Add(_lblTranslator);
            _settingsRow1.Controls.Add(_txtTranslator);

            _settingsRow2.Controls.Add(_lblSearch);
            _settingsRow2.Controls.Add(_txtSearch);
            _settingsRow2.Controls.Add(_chkShowOnlyUntranslated);
            _settingsRow2.Controls.Add(_lblSummary);

            _settingsLayout.Controls.Add(_settingsRow1, 0, 0);
            _settingsLayout.Controls.Add(_settingsRow2, 0, 1);

            _translationsGroup.Dock = DockStyle.Fill;
            _translationsGroup.Controls.Add(_translationsGrid);

            ConfigureTranslationsGrid();

            _actionsGroup.Dock = DockStyle.Fill;
            _actionsGroup.Controls.Add(_actionsLayout);

            _actionsLayout.ColumnCount = 2;
            _actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _actionsLayout.RowCount = 1;
            _actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _actionsLayout.Dock = DockStyle.Fill;
            _actionsLayout.Padding = new Padding(6, 10, 6, 6);

            _lblSaveState.AutoSize = true;
            _lblSaveState.Anchor = AnchorStyles.Left;
            _lblSaveState.Margin = new Padding(0, 6, 12, 0);

            _actionsPanel.AutoSize = true;
            _actionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _actionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _actionsPanel.FlowDirection = FlowDirection.RightToLeft;
            _actionsPanel.WrapContents = false;
            _actionsPanel.Padding = new Padding(0);
            _actionsPanel.Margin = new Padding(0);

            ConfigureActionButton(_btnClose, btnClose_Click);
            ConfigureActionButton(_btnSave, btnSave_Click);
            ConfigureActionButton(_btnSync, btnSync_Click);
            ConfigureActionButton(_btnCreate, btnCreate_Click);

            _actionsPanel.Controls.Add(_btnClose);
            _actionsPanel.Controls.Add(_btnSave);
            _actionsPanel.Controls.Add(_btnSync);
            _actionsPanel.Controls.Add(_btnCreate);

            _actionsLayout.Controls.Add(_lblSaveState, 0, 0);
            _actionsLayout.Controls.Add(_actionsPanel, 1, 0);

            _mainLayout.Controls.Add(_settingsGroup, 0, 0);
            _mainLayout.Controls.Add(_translationsGroup, 0, 1);
            _mainLayout.Controls.Add(_actionsGroup, 0, 2);

            Controls.Add(_mainLayout);

            ResumeLayout(false);
        }

        private void ConfigureSettingsRow(FlowLayoutPanel panel)
        {
            panel.Dock = DockStyle.Fill;
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.WrapContents = true;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.Padding = new Padding(0);
            panel.Margin = new Padding(0, 0, 0, 6);
        }

        private void ConfigureTranslationsGrid()
        {
            _translationsGrid.Dock = DockStyle.Fill;
            _translationsGrid.AutoGenerateColumns = false;
            _translationsGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            _translationsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _translationsGrid.MultiSelect = false;
            _translationsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _translationsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _translationsGrid.DataSource = _bindingSource;
            _translationsGrid.CurrentCellDirtyStateChanged += grdTranslations_CurrentCellDirtyStateChanged;
            _translationsGrid.CellValueChanged += grdTranslations_CellValueChanged;

            var keyColumn = new DataGridViewTextBoxColumn
            {
                Name = "colKey",
                DataPropertyName = "Key",
                ReadOnly = true,
                FillWeight = 24F,
                MinimumWidth = 120
            };

            var sourceColumn = new DataGridViewTextBoxColumn
            {
                Name = "colSource",
                DataPropertyName = "Source",
                ReadOnly = true,
                FillWeight = 28F,
                MinimumWidth = 180,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };

            var translationColumn = new DataGridViewTextBoxColumn
            {
                Name = "colTranslation",
                DataPropertyName = "Translation",
                ReadOnly = false,
                FillWeight = 28F,
                MinimumWidth = 180,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };

            var translatedColumn = new DataGridViewCheckBoxColumn
            {
                Name = "colIsTranslated",
                DataPropertyName = "IsTranslated",
                FillWeight = 10F,
                MinimumWidth = 80
            };

            var notesColumn = new DataGridViewTextBoxColumn
            {
                Name = "colNotes",
                DataPropertyName = "Notes",
                ReadOnly = true,
                FillWeight = 20F,
                MinimumWidth = 140,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };

            _translationsGrid.Columns.AddRange(keyColumn, sourceColumn, translationColumn, translatedColumn, notesColumn);
        }

        private void ConfigureActionButton(Button button, EventHandler clickHandler)
        {
            button.Margin = new Padding(6, 0, 0, 0);
            button.Size = new Size(95, 30);
            button.UseVisualStyleBackColor = true;
            button.Click += clickHandler;
        }

        private void LoadMasterFile()
        {
            string masterFile = Path.Combine(_translationsDirectory, "en.json");
            _masterFile = TranslationFileService.LoadFile(masterFile);
        }

        private void LoadAvailableCultures()
        {
            _isLoadingCulture = true;
            try
            {
                string selectedCulture = _cmbTargetCulture.SelectedItem as string;
                var cultures = Directory
                    .EnumerateFiles(_translationsDirectory, "*.json", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileNameWithoutExtension)
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

        private void SelectCulture(string culture)
        {
            if (_cmbTargetCulture.Items.Count == 0)
            {
                return;
            }

            string selectedCulture = FindCultureItem(culture) ?? (_cmbTargetCulture.Items[0] as string);
            SetSelectedCultureItem(selectedCulture);
            LoadSelectedCulture(selectedCulture);
        }

        private void LoadSelectedCulture(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return;
            }

            _isLoadingCulture = true;
            try
            {
                string path = Path.Combine(_translationsDirectory, culture + ".json");
                _currentFile = TranslationFileService.LoadFile(path);
                _selectedCulture = culture;
                _txtTranslator.Text = _currentFile.Metadata.Translator ?? string.Empty;

                _allRows = _currentFile.Entries
                    .OrderBy(entry => entry.Key)
                    .Select(entry => new TranslationEditorRow
                    {
                        Key = entry.Key,
                        Source = entry.Value.Source,
                        Translation = entry.Value.Translation,
                        IsTranslated = entry.Value.IsTranslated,
                        Notes = entry.Value.Notes
                    })
                    .ToList();

                ApplyFilters();
                SetPendingChanges(false);
                UpdateActionStates();
                ApplyResponsiveLayout();
            }
            finally
            {
                _isLoadingCulture = false;
            }
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

            _btnSave.Enabled = hasCurrentFile && _hasPendingChanges;
            _btnSync.Enabled = hasCurrentFile && !isEnglish;
            _lblSaveState.Text = LocalizationManager.GetString(_hasPendingChanges
                ? "UI.TranslationEditor.Status.Dirty"
                : "UI.TranslationEditor.Status.Clean");
            UpdateWindowTitle();
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

            string path = Path.Combine(_translationsDirectory, _currentFile.Metadata.Culture + ".json");
            TranslationFileService.SaveFile(_currentFile, path);
            HasSavedChanges = true;
            SetPendingChanges(false);
        }

        private void CreateNewCulture()
        {
            if (!ConfirmDiscardPendingChanges())
            {
                return;
            }

            string culture = (_txtNewCulture.Text ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(culture))
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.CultureCodeRequired");
            }

            string path = Path.Combine(_translationsDirectory, culture + ".json");
            if (File.Exists(path))
            {
                throw CreateLocalizedException("UI.TranslationEditor.Errors.LocaleExists", culture);
            }

            LoadMasterFile();

            var newFile = TranslationMaintenanceService.CreateTemplate(
                _masterFile,
                culture,
                string.IsNullOrWhiteSpace(_txtTranslator.Text) ? null : _txtTranslator.Text.Trim());

            TranslationFileService.SaveFile(newFile, path);
            HasSavedChanges = true;

            LoadAvailableCultures();
            SelectCulture(culture);
            _txtNewCulture.Clear();

            ShowLocalizedSuccessMessage("UI.TranslationEditor.Success.LocaleCreated", false, culture);
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

            var target = TranslationFileService.LoadFile(Path.Combine(_translationsDirectory, _currentFile.Metadata.Culture + ".json"));
            var result = TranslationMaintenanceService.Synchronize(_masterFile, target);

            TranslationFileService.SaveFile(result.TranslationFile, Path.Combine(_translationsDirectory, result.TranslationFile.Metadata.Culture + ".json"));
            HasSavedChanges = true;

            SelectCulture(result.TranslationFile.Metadata.Culture);

            ShowLocalizedSuccessMessage(
                "UI.TranslationEditor.Success.LocaleSynced",
                false,
                result.TranslationFile.Metadata.Culture,
                result.AddedCount,
                result.UpdatedCount,
                result.RemovedCount);
        }

        private void ApplyLocalization()
        {
            _settingsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Settings.Group");
            _translationsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Grid.Group");
            _actionsGroup.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Group");

            _lblTargetCulture.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.TargetCulture");
            _lblNewCulture.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.NewCulture");
            _lblTranslator.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Translator");
            _lblSearch.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.Search");
            _chkShowOnlyUntranslated.Text = LocalizationManager.GetString("UI.TranslationEditor.Fields.ShowOnlyUntranslated");

            _btnCreate.Text = LocalizationManager.GetString("UI.TranslationEditor.Actions.Create");
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

            UpdateSummary();
            UpdateActionStates();
            ApplyResponsiveLayout();
        }

        private void FilterControl_Changed(object sender, EventArgs e)
        {
            if (!_isLoadingCulture)
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

            int availableWidth = Math.Max(320, _mainLayout.ClientSize.Width - _mainLayout.Padding.Horizontal - 24);
            _settingsRow1.MaximumSize = new Size(availableWidth, 0);
            _settingsRow2.MaximumSize = new Size(availableWidth, 0);

            int summaryWidth = Math.Max(180, availableWidth - (_lblSearch.Width + _txtSearch.Width + 72));
            _lblSummary.MaximumSize = new Size(summaryWidth, 0);

            int settingsHeight =
                _settingsRow1.GetPreferredSize(new Size(availableWidth, 0)).Height +
                _settingsRow2.GetPreferredSize(new Size(availableWidth, 0)).Height + 46;
            _mainLayout.RowStyles[0].Height = Math.Max(104, settingsHeight);

            int buttonsWidth = _actionsPanel.GetPreferredSize(Size.Empty).Width;
            int saveStateWidth = Math.Max(180, availableWidth - buttonsWidth - 24);
            _lblSaveState.MaximumSize = new Size(saveStateWidth, 0);

            int actionsHeight = Math.Max(
                _actionsPanel.GetPreferredSize(Size.Empty).Height,
                _lblSaveState.GetPreferredSize(new Size(saveStateWidth, 0)).Height) + 36;
            _mainLayout.RowStyles[2].Height = Math.Max(68, actionsHeight);
            _mainLayout.PerformLayout();
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
