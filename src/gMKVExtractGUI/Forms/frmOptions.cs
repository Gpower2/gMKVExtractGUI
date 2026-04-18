using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.MkvExtract;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix.Forms
{
    public partial class frmOptions : gMKVToolNix.gForm
    {
        private const int PatternButtonMinWidth = 83;
        private const int ActionButtonMinWidth = 80;
        private const int LayoutSpacing = 6;
        private readonly Button _btnTranslationEditor = new Button();
        private gSettings _Settings = null;
        private ContextMenuStrip _VideoTrackContextMenu = null;
        private ContextMenuStrip _AudioTrackContextMenu = null;
        private ContextMenuStrip _SubtitleTrackContextMenu = null;
        private ContextMenuStrip _ChapterContextMenu = null;
        private ContextMenuStrip _AttachmentContextMenu = null;
        private ContextMenuStrip _TagsContextMenu = null;

        public frmOptions()
        {
            try
            {
                InitializeComponent();

                Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
                Text = string.Format("gMKVExtractGUI v{0} -- Options", GetCurrentVersion());

                txtInfo.Text = LocalizationManager.GetString("UI.OptionsForm.Info.Text");

                // Initialize the DPI aware scaling
                InitDPI();

                InitializeTranslationEditorButton();

                // Initialize the context menus
                InitPlaceholderContextMenus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            // Set settings
            _Settings = new gSettings(this.GetCurrentDirectory());
            _Settings.Reload();

            // Fill from settings
            FillFromSettings();

            // Initialize culture selector
            InitializeCultureSelector();

            // Apply Theme
            ThemeManager.ApplyTheme(this, _Settings.DarkMode);
            // Explicitly set ForeColor for txtInfo after theming
            txtInfo.ForeColor = _Settings.DarkMode
                ? ThemeManager.DarkModeTextForeColor
                : ThemeManager.LightModeTextForeColor;
            NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
            NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);

            ApplyThemeToContextMenus(_Settings.DarkMode);

            // Apply localization
            ApplyLocalization();

            // Select the information text box
            txtInfo.Select();
            txtInfo.Focus();
        }

        private void FillFromSettings()
        {
            txtVideoTracksFilename.Text = _Settings.VideoTrackFilenamePattern;
            txtAudioTracksFilename.Text = _Settings.AudioTrackFilenamePattern;
            txtSubtitleTracksFilename.Text = _Settings.SubtitleTrackFilenamePattern;
            txtChaptersFilename.Text = _Settings.ChapterFilenamePattern;
            txtAttachmentsFilename.Text = _Settings.AttachmentFilenamePattern;
            txtTagsFilename.Text = _Settings.TagsFilenamePattern;
            chkTextFilesWithoutBom.Checked = _Settings.DisableBomForTextFiles;
            chkRawMode.Checked = _Settings.UseRawExtractionMode;
            chkFullRawMode.Checked = _Settings.UseFullRawExtractionMode;
        }

        private void InitializeTranslationEditorButton()
        {
            _btnTranslationEditor.Name = "btnTranslationEditor";
            _btnTranslationEditor.Size = new Size(95, 30);
            _btnTranslationEditor.UseVisualStyleBackColor = true;
            _btnTranslationEditor.Click += btnTranslationEditor_Click;
            grpAdvanced.Controls.Add(_btnTranslationEditor);
        }

        private void UpdateSettings()
        {
            _Settings.VideoTrackFilenamePattern = txtVideoTracksFilename.Text;
            _Settings.AudioTrackFilenamePattern = txtAudioTracksFilename.Text;
            _Settings.SubtitleTrackFilenamePattern = txtSubtitleTracksFilename.Text;
            _Settings.ChapterFilenamePattern = txtChaptersFilename.Text;
            _Settings.AttachmentFilenamePattern = txtAttachmentsFilename.Text;
            _Settings.TagsFilenamePattern = txtTagsFilename.Text;
            _Settings.DisableBomForTextFiles = chkTextFilesWithoutBom.Checked;
            _Settings.UseRawExtractionMode = chkRawMode.Checked;
            _Settings.UseFullRawExtractionMode = chkFullRawMode.Checked;
        }

        private void InitializeCultureSelector()
        {
            try
            {
                var cultures = GetAvailableCultures();
                cmbCulture.Items.Clear();
                foreach (var culture in cultures)
                {
                    cmbCulture.Items.Add(culture);
                }

                var currentCulture = _Settings.Culture;
                if (cmbCulture.Items.Contains(currentCulture))
                {
                    cmbCulture.SelectedItem = currentCulture;
                }
                else if (cmbCulture.Items.Count > 0)
                {
                    cmbCulture.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private List<string> GetAvailableCultures()
        {
            try
            {
                if (LocalizationManager.IsInitialized)
                {
                    return LocalizationManager.GetAvailableCultures().ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }

            return new List<string> { "en" };
        }

        private void CmbCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedCulture = cmbCulture.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedCulture) && selectedCulture != _Settings.Culture)
                {
                    _Settings.Culture = selectedCulture;
                    _Settings.Save();
                    LocalizationManager.Reload(selectedCulture);
                    ApplyLocalizationToAllForms();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void ApplyLocalizationToAllForms()
        {
            try
            {
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (form is frmMain mainForm)
                        mainForm.ApplyLocalization();
                    else if (form is frmMain2 mainForm2)
                        mainForm2.ApplyLocalization();
                    else if (form is frmJobManager jobManager)
                        jobManager.ApplyLocalization();
                    else if (form is frmLog logForm)
                        logForm.ApplyLocalization();
                    else if (form is frmOptions optionsForm)
                        optionsForm.ApplyLocalization();
                }

                ThemeManager.ApplyTheme(this, _Settings.DarkMode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
            }
        }

        private ToolStripMenuItem GetToolstripMenuItem(string description, string placeholder, TextBox txtBox)
        {
            return new ToolStripMenuItem(description, null, (object s, EventArgs ea) =>
            {
                txtBox.Text = txtBox.Text.Insert(txtBox.SelectionStart, placeholder);
            });
        }

        private ToolStripMenuItem GetLocalizedToolstripMenuItem(string key, string placeholder, TextBox txtBox)
        {
            return GetToolstripMenuItem(LocalizationManager.GetString(key), placeholder, txtBox);
        }

        private ToolStripMenuItem GetLocalizedPlaceholderGroup(string key)
        {
            return new ToolStripMenuItem(LocalizationManager.GetString(key), null);
        }

        private void ApplyThemeToContextMenus(bool darkMode)
        {
            if (_VideoTrackContextMenu != null) ThemeManager.ApplyTheme(_VideoTrackContextMenu, darkMode);
            if (_AudioTrackContextMenu != null) ThemeManager.ApplyTheme(_AudioTrackContextMenu, darkMode);
            if (_SubtitleTrackContextMenu != null) ThemeManager.ApplyTheme(_SubtitleTrackContextMenu, darkMode);
            if (_ChapterContextMenu != null) ThemeManager.ApplyTheme(_ChapterContextMenu, darkMode);
            if (_AttachmentContextMenu != null) ThemeManager.ApplyTheme(_AttachmentContextMenu, darkMode);
            if (_TagsContextMenu != null) ThemeManager.ApplyTheme(_TagsContextMenu, darkMode);
        }

        private void InitPlaceholderContextMenus()
        {
            _VideoTrackContextMenu = new ContextMenuStrip();
            _AudioTrackContextMenu = new ContextMenuStrip();
            _SubtitleTrackContextMenu = new ContextMenuStrip();
            _ChapterContextMenu = new ContextMenuStrip();
            _AttachmentContextMenu = new ContextMenuStrip();
            _TagsContextMenu = new ContextMenuStrip();

            // Common placeholders
            // ============================================================================================================================
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");

            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");

            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add("-");
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add("-");

            _ChapterContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtChaptersFilename));
            _ChapterContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtChaptersFilename));
            _ChapterContextMenu.Items.Add("-");
            _ChapterContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtChaptersFilename));

            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add("-");
            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add("-");

            _TagsContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithoutExtension", gMKVExtractFilenamePatterns.FilenameNoExt, txtTagsFilename));
            _TagsContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.InputFilenameWithExtension", gMKVExtractFilenamePatterns.Filename, txtTagsFilename));
            _TagsContextMenu.Items.Add("-");
            _TagsContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.DirectorySeparator", gMKVExtractFilenamePatterns.DirectorySeparator, txtTagsFilename));
            // ============================================================================================================================

            // Common Track placeholders
            // ============================================================================================================================
            var vidTrackNumber = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackNumber.Group");
            vidTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.NoFormat", gMKVExtractFilenamePatterns.TrackNumber, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.OneDigit", gMKVExtractFilenamePatterns.TrackNumber_0, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.TwoDigits", gMKVExtractFilenamePatterns.TrackNumber_00, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.ThreeDigits", gMKVExtractFilenamePatterns.TrackNumber_000, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(vidTrackNumber);
            //_VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtVideoTracksFilename));

            var vidTrackID = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackId.Group");
            vidTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.NoFormat", gMKVExtractFilenamePatterns.TrackID, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.OneDigit", gMKVExtractFilenamePatterns.TrackID_0, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.TwoDigits", gMKVExtractFilenamePatterns.TrackID_00, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.ThreeDigits", gMKVExtractFilenamePatterns.TrackID_000, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(vidTrackID);
            //_VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackName", gMKVExtractFilenamePatterns.TrackName, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguage", gMKVExtractFilenamePatterns.TrackLanguage, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguageIetf", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecId", gMKVExtractFilenamePatterns.TrackCodecID, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecPrivate", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackDelay", gMKVExtractFilenamePatterns.TrackDelay, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackEffectiveDelay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackForced", gMKVExtractFilenamePatterns.TrackForced, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");

            var audTrackNumber = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackNumber.Group");
            audTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.NoFormat", gMKVExtractFilenamePatterns.TrackNumber, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.OneDigit", gMKVExtractFilenamePatterns.TrackNumber_0, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.TwoDigits", gMKVExtractFilenamePatterns.TrackNumber_00, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.ThreeDigits", gMKVExtractFilenamePatterns.TrackNumber_000, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(audTrackNumber);
            //_AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtAudioTracksFilename));

            var audTrackID = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackId.Group");
            audTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.NoFormat", gMKVExtractFilenamePatterns.TrackID, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.OneDigit", gMKVExtractFilenamePatterns.TrackID_0, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.TwoDigits", gMKVExtractFilenamePatterns.TrackID_00, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.ThreeDigits", gMKVExtractFilenamePatterns.TrackID_000, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(audTrackID);
            //_AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackName", gMKVExtractFilenamePatterns.TrackName, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguage", gMKVExtractFilenamePatterns.TrackLanguage, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguageIetf", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecId", gMKVExtractFilenamePatterns.TrackCodecID, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecPrivate", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackDelay", gMKVExtractFilenamePatterns.TrackDelay, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackEffectiveDelay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackForced", gMKVExtractFilenamePatterns.TrackForced, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");

            var subTrackNumber = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackNumber.Group");
            subTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.NoFormat", gMKVExtractFilenamePatterns.TrackNumber, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.OneDigit", gMKVExtractFilenamePatterns.TrackNumber_0, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.TwoDigits", gMKVExtractFilenamePatterns.TrackNumber_00, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackNumber.ThreeDigits", gMKVExtractFilenamePatterns.TrackNumber_000, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(subTrackNumber);
            //_SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtSubtitleTracksFilename));

            var subTrackID = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.TrackId.Group");
            subTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.NoFormat", gMKVExtractFilenamePatterns.TrackID, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.OneDigit", gMKVExtractFilenamePatterns.TrackID_0, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.TwoDigits", gMKVExtractFilenamePatterns.TrackID_00, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackId.ThreeDigits", gMKVExtractFilenamePatterns.TrackID_000, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(subTrackID);
            //_SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackName", gMKVExtractFilenamePatterns.TrackName, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguage", gMKVExtractFilenamePatterns.TrackLanguage, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackLanguageIetf", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecId", gMKVExtractFilenamePatterns.TrackCodecID, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackCodecPrivate", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackDelay", gMKVExtractFilenamePatterns.TrackDelay, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackEffectiveDelay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.TrackForced", gMKVExtractFilenamePatterns.TrackForced, txtSubtitleTracksFilename));
            // ============================================================================================================================

            // Video Track placeholders
            // ============================================================================================================================
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.VideoPixelWidth", gMKVExtractFilenamePatterns.VideoPixelWidth, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.VideoPixelHeight", gMKVExtractFilenamePatterns.VideoPixelHeight, txtVideoTracksFilename));
            // ============================================================================================================================

            // Audio Track placeholders
            // ============================================================================================================================
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AudioSamplingFrequency", gMKVExtractFilenamePatterns.AudioSamplingFrequency, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AudioChannels", gMKVExtractFilenamePatterns.AudioChannels, txtAudioTracksFilename));
            // ============================================================================================================================

            // Attachment placeholders
            // ============================================================================================================================
            var attachmentID = GetLocalizedPlaceholderGroup("UI.OptionsForm.Placeholders.AttachmentId.Group");
            attachmentID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentId.NoFormat", gMKVExtractFilenamePatterns.AttachmentID, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentId.OneDigit", gMKVExtractFilenamePatterns.AttachmentID_0, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentId.TwoDigits", gMKVExtractFilenamePatterns.AttachmentID_00, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentId.ThreeDigits", gMKVExtractFilenamePatterns.AttachmentID_000, txtAttachmentsFilename));

            _AttachmentContextMenu.Items.Add(attachmentID);
            //_AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Attachment ID", gMKVExtractFilenamePatterns.AttachmentID, txtAttachmentsFilename));

            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentFilename", gMKVExtractFilenamePatterns.AttachmentFilename, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentMimeType", gMKVExtractFilenamePatterns.AttachmentMimeType, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetLocalizedToolstripMenuItem("UI.OptionsForm.Placeholders.AttachmentFileSizeBytes", gMKVExtractFilenamePatterns.AttachmentFileSize, txtAttachmentsFilename));
            // ============================================================================================================================
        }

        private void btnAddVideoTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _VideoTrackContextMenu.Show(btnAddVideoTrackPlaceholder, 0, btnAddVideoTrackPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAddAudioTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _AudioTrackContextMenu.Show(btnAddAudioTrackPlaceholder, 0, btnAddAudioTrackPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAddSubtitleTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _SubtitleTrackContextMenu.Show(btnAddSubtitleTrackPlaceholder, 0, btnAddSubtitleTrackPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAddChapterPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _ChapterContextMenu.Show(btnAddChapterPlaceholder, 0, btnAddChapterPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAddAttachmentPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _AttachmentContextMenu.Show(btnAddAttachmentPlaceholder, 0, btnAddAttachmentPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAddTagsPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                _TagsContextMenu.Show(btnAddTagsPlaceholder, 0, btnAddTagsPlaceholder.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSettings();
                _Settings.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultVideoTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.VideoTrackFilenamePattern));

                _Settings.VideoTrackFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultAudioTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AudioTrackFilenamePattern));

                _Settings.AudioTrackFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultSubtitleTrackPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.SubtitleTrackFilenamePattern));

                _Settings.SubtitleTrackFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultChapterPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.ChapterFilenamePattern));

                _Settings.ChapterFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultAttachmentPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AttachmentFilenamePattern));

                _Settings.AttachmentFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaultTagsPlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.TagsFilenamePattern));

                _Settings.TagsFilenamePattern = defaultValue;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnDefaults_Click(object sender, EventArgs e)
        {
            try
            {
                string defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.VideoTrackFilenamePattern));
                _Settings.VideoTrackFilenamePattern = defaultValue;

                defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AudioTrackFilenamePattern));
                _Settings.AudioTrackFilenamePattern = defaultValue;

                defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.SubtitleTrackFilenamePattern));
                _Settings.SubtitleTrackFilenamePattern = defaultValue;

                defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.ChapterFilenamePattern));
                _Settings.ChapterFilenamePattern = defaultValue;

                defaultValue = _Settings.GetPropertyDefaultValue<string>(nameof(_Settings.AttachmentFilenamePattern));
                _Settings.AttachmentFilenamePattern = defaultValue;

                chkTextFilesWithoutBom.Checked = false;
                chkRawMode.Checked = false;
                chkFullRawMode.Checked = false;

                FillFromSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        public void ApplyLocalization()
        {
            Text = string.Format("gMKVExtractGUI v{0} -- {1}", GetCurrentVersion(), LocalizationManager.GetString("UI.OptionsForm.Title"));
            grpInfo.Text = LocalizationManager.GetString("UI.OptionsForm.Info.Group");
            txtInfo.Text = LocalizationManager.GetString("UI.OptionsForm.Info.Text");
            grpVideoTracks.Text = LocalizationManager.GetString("UI.OptionsForm.VideoTracks.Group");
            btnAddVideoTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.VideoTracks.Add");
            btnDefaultVideoTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.VideoTracks.Default");
            grpAudioTracks.Text = LocalizationManager.GetString("UI.OptionsForm.AudioTracks.Group");
            btnAddAudioTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.AudioTracks.Add");
            btnDefaultAudioTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.AudioTracks.Default");
            grpSubtitleTracks.Text = LocalizationManager.GetString("UI.OptionsForm.SubtitleTracks.Group");
            btnAddSubtitleTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.SubtitleTracks.Add");
            btnDefaultSubtitleTrackPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.SubtitleTracks.Default");
            grpChapters.Text = LocalizationManager.GetString("UI.OptionsForm.Chapters.Group");
            btnAddChapterPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Chapters.Add");
            btnDefaultChapterPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Chapters.Default");
            grpAttachments.Text = LocalizationManager.GetString("UI.OptionsForm.Attachments.Group");
            btnAddAttachmentPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Attachments.Add");
            btnDefaultAttachmentPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Attachments.Default");
            grpTags.Text = LocalizationManager.GetString("UI.OptionsForm.Tags.Group");
            btnAddTagsPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Tags.Add");
            btnDefaultTagsPlaceholder.Text = LocalizationManager.GetString("UI.OptionsForm.Tags.Default");
            grpAdvanced.Text = LocalizationManager.GetString("UI.OptionsForm.Advanced.Group");
            lblCulture.Text = LocalizationManager.GetString("UI.OptionsForm.Advanced.Culture");
            _btnTranslationEditor.Text = LocalizationManager.GetString("UI.OptionsForm.Advanced.Translations");
            grpActions.Text = LocalizationManager.GetString("UI.OptionsForm.Actions.Group");
            btnDefaults.Text = LocalizationManager.GetString("UI.OptionsForm.Defaults");
            btnOK.Text = LocalizationManager.GetString("UI.OptionsForm.Actions.OK");
            btnCancel.Text = LocalizationManager.GetString("UI.OptionsForm.Actions.Cancel");
            chkTextFilesWithoutBom.Text = LocalizationManager.GetString("UI.OptionsForm.TextFilesWithoutBom");
            chkRawMode.Text = LocalizationManager.GetString("UI.OptionsForm.RawMode");
            chkFullRawMode.Text = LocalizationManager.GetString("UI.OptionsForm.FullRawMode");
            InitPlaceholderContextMenus();
            if (_Settings != null)
            {
                ApplyThemeToContextMenus(_Settings.DarkMode);
            }
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            LayoutPatternGroup(grpVideoTracks, txtVideoTracksFilename, btnAddVideoTrackPlaceholder, btnDefaultVideoTrackPlaceholder);
            LayoutPatternGroup(grpAudioTracks, txtAudioTracksFilename, btnAddAudioTrackPlaceholder, btnDefaultAudioTrackPlaceholder);
            LayoutPatternGroup(grpSubtitleTracks, txtSubtitleTracksFilename, btnAddSubtitleTrackPlaceholder, btnDefaultSubtitleTrackPlaceholder);
            LayoutPatternGroup(grpChapters, txtChaptersFilename, btnAddChapterPlaceholder, btnDefaultChapterPlaceholder);
            LayoutPatternGroup(grpAttachments, txtAttachmentsFilename, btnAddAttachmentPlaceholder, btnDefaultAttachmentPlaceholder);
            LayoutPatternGroup(grpTags, txtTagsFilename, btnAddTagsPlaceholder, btnDefaultTagsPlaceholder);
            LayoutAdvancedGroup();
            LayoutActionsGroup();
        }

        private void LayoutPatternGroup(GroupBox groupBox, TextBox textBox, Button addButton, Button defaultButton)
        {
            addButton.ApplyLocalizedButtonSize(PatternButtonMinWidth);
            defaultButton.ApplyLocalizedButtonSize(PatternButtonMinWidth);

            int top = Math.Max(16, textBox.Top - 4);
            int right = groupBox.ClientSize.Width - 6;

            defaultButton.Location = new Point(right - defaultButton.Width, top);
            addButton.Location = new Point(defaultButton.Left - LayoutSpacing - addButton.Width, top);

            int textRight = addButton.Left - LayoutSpacing;
            textBox.Width = Math.Max(180, textRight - textBox.Left);
        }

        private void LayoutActionsGroup()
        {
            btnDefaults.ApplyLocalizedButtonSize(90);
            btnOK.ApplyLocalizedButtonSize(ActionButtonMinWidth);
            btnCancel.ApplyLocalizedButtonSize(ActionButtonMinWidth);

            const int buttonTop = 17;

            btnDefaults.Location = new Point(9, buttonTop);

            int right = grpActions.ClientSize.Width - 6;
            btnCancel.Location = new Point(right - btnCancel.Width, buttonTop);
            btnOK.Location = new Point(btnCancel.Left - LayoutSpacing - btnOK.Width, buttonTop);
        }

        private void LayoutAdvancedGroup()
        {
            chkTextFilesWithoutBom.Location = new Point(9, 20);

            chkRawMode.Location = new Point(9, 46);
            chkFullRawMode.Location = new Point(chkRawMode.Right + 12, 46);

            int cultureRowTop = 46;
            int advancedRowHeight = 86;
            int comboWidth = Math.Max(80, cmbCulture.Width);
            _btnTranslationEditor.ApplyLocalizedButtonSize(95);
            int right = grpAdvanced.ClientSize.Width - 6;

            cmbCulture.Width = comboWidth;
            cmbCulture.Location = new Point(right - cmbCulture.Width, cultureRowTop - 3);
            lblCulture.Location = new Point(cmbCulture.Left - LayoutSpacing - lblCulture.Width, cultureRowTop);
            _btnTranslationEditor.Location = new Point(lblCulture.Left - 12 - _btnTranslationEditor.Width, cultureRowTop - 3);

            if (chkFullRawMode.Right + 12 > _btnTranslationEditor.Left)
            {
                cultureRowTop = 70;
                advancedRowHeight = 110;
                cmbCulture.Location = new Point(right - cmbCulture.Width, cultureRowTop - 3);
                lblCulture.Location = new Point(cmbCulture.Left - LayoutSpacing - lblCulture.Width, cultureRowTop);
                _btnTranslationEditor.Location = new Point(lblCulture.Left - 12 - _btnTranslationEditor.Width, cultureRowTop - 3);
            }

            if (chkFullRawMode.Right + 12 > _btnTranslationEditor.Left)
            {
                cultureRowTop = 94;
                advancedRowHeight = 134;
                cmbCulture.Location = new Point(right - cmbCulture.Width, cultureRowTop - 3);
                lblCulture.Location = new Point(cmbCulture.Left - LayoutSpacing - lblCulture.Width, cultureRowTop);
                _btnTranslationEditor.Location = new Point(lblCulture.Left - 12 - _btnTranslationEditor.Width, cultureRowTop - 3);
            }

            if (tlpMain.RowStyles.Count > 7)
            {
                tlpMain.RowStyles[7].Height = advancedRowHeight;
            }
        }

        private void btnTranslationEditor_Click(object sender, EventArgs e)
        {
            try
            {
                string culture = cmbCulture.SelectedItem == null
                    ? (_Settings == null ? "en" : _Settings.Culture)
                    : cmbCulture.SelectedItem.ToString();

                using (var editor = new frmTranslationEditor(GetCurrentDirectory(), culture))
                {
                    editor.ShowDialog(this);

                    if (editor.HasSavedChanges)
                    {
                        InitializeCultureSelector();
                        if (_Settings != null)
                        {
                            LocalizationManager.Reload(_Settings.Culture);
                            ApplyLocalizationToAllForms();
                        }
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
    }
}
