using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace gMKVToolNix.Forms
{
    public partial class frmOptions : gMKVToolNix.gForm
    {
        private gSettings _Settings = null;
        private ContextMenuStrip _VideoTrackContextMenu = null;
        private ContextMenuStrip _AudioTrackContextMenu = null;
        private ContextMenuStrip _SubtitleTrackContextMenu = null;
        private ContextMenuStrip _ChapterContextMenu = null;
        private ContextMenuStrip _AttachmentContextMenu = null;

        private readonly static string INFO_TEXT = "Here you can specify the output filename format for each kind of track." + Environment.NewLine +
            "Pressing the \"Add...\" button you will see the list with the available placeholders and select them." + Environment.NewLine +
            "Pressing the \"Default\" button you will reset the output filename format to its default value.";

        public frmOptions()
        {
            try
            {
                InitializeComponent();

                Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
                Text = String.Format("gMKVExtractGUI v{0} -- Options", GetCurrentVersion());

                // Set the info text
                txtInfo.Text = INFO_TEXT;

                // Initialize the DPI aware scaling
                InitDPI();

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
        }

        private void UpdateSettings()
        {
            _Settings.VideoTrackFilenamePattern = txtVideoTracksFilename.Text;
            _Settings.AudioTrackFilenamePattern = txtAudioTracksFilename.Text;
            _Settings.SubtitleTrackFilenamePattern = txtSubtitleTracksFilename.Text;
            _Settings.ChapterFilenamePattern = txtChaptersFilename.Text;
            _Settings.AttachmentFilenamePattern = txtAttachmentsFilename.Text;
        }

        private ToolStripMenuItem GetToolstripMenuItem(string description, string placeholder, TextBox txtBox)
        {
            return new ToolStripMenuItem(description, null, (object s, EventArgs ea) =>
            {
                txtBox.Text = txtBox.Text.Insert(txtBox.SelectionStart, placeholder);
            });
        }

        private void InitPlaceholderContextMenus()
        {
            _VideoTrackContextMenu = new ContextMenuStrip();
            _AudioTrackContextMenu = new ContextMenuStrip();
            _SubtitleTrackContextMenu = new ContextMenuStrip();
            _ChapterContextMenu = new ContextMenuStrip();
            _AttachmentContextMenu = new ContextMenuStrip();

            // Common placeholders
            // ============================================================================================================================
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (without extension)", gMKVExtractFilenamePatterns.FilenameNoExt, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (with extension)", gMKVExtractFilenamePatterns.Filename, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Directory Separator", gMKVExtractFilenamePatterns.DirectorySeparator, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");

            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (without extension)", gMKVExtractFilenamePatterns.FilenameNoExt, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (with extension)", gMKVExtractFilenamePatterns.Filename, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Directory Separator", gMKVExtractFilenamePatterns.DirectorySeparator, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");

            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (without extension)", gMKVExtractFilenamePatterns.FilenameNoExt, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (with extension)", gMKVExtractFilenamePatterns.Filename, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add("-");
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Directory Separator", gMKVExtractFilenamePatterns.DirectorySeparator, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add("-");

            _ChapterContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (without extension)", gMKVExtractFilenamePatterns.FilenameNoExt, txtChaptersFilename));
            _ChapterContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (with extension)", gMKVExtractFilenamePatterns.Filename, txtChaptersFilename));
            _ChapterContextMenu.Items.Add("-");
            _ChapterContextMenu.Items.Add(GetToolstripMenuItem("Directory Separator", gMKVExtractFilenamePatterns.DirectorySeparator, txtChaptersFilename));            

            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (without extension)", gMKVExtractFilenamePatterns.FilenameNoExt, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Input Filename (with extension)", gMKVExtractFilenamePatterns.Filename, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add("-");
            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Directory Separator", gMKVExtractFilenamePatterns.DirectorySeparator, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add("-");
            // ============================================================================================================================

            // Common Track placeholders
            // ============================================================================================================================
            var vidTrackNumber = new ToolStripMenuItem("Track Number...", null);            
            vidTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (No format)", gMKVExtractFilenamePatterns.TrackNumber, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (1 digit)", gMKVExtractFilenamePatterns.TrackNumber_0, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (2 digits)", gMKVExtractFilenamePatterns.TrackNumber_00, txtVideoTracksFilename));
            vidTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (3 digits)", gMKVExtractFilenamePatterns.TrackNumber_000, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(vidTrackNumber);
            //_VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtVideoTracksFilename));

            var vidTrackID = new ToolStripMenuItem("Track ID...", null);            
            vidTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (No format)", gMKVExtractFilenamePatterns.TrackID, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (1 digit)", gMKVExtractFilenamePatterns.TrackID_0, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (2 digits)", gMKVExtractFilenamePatterns.TrackID_00, txtVideoTracksFilename));
            vidTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (3 digits)", gMKVExtractFilenamePatterns.TrackID_000, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(vidTrackID);
            //_VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtVideoTracksFilename));

            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Name", gMKVExtractFilenamePatterns.TrackName, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language", gMKVExtractFilenamePatterns.TrackLanguage, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language IETF", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec ID", gMKVExtractFilenamePatterns.TrackCodecID, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec Private", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Delay", gMKVExtractFilenamePatterns.TrackDelay, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Effective Delay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add("-");

            var audTrackNumber = new ToolStripMenuItem("Track Number...", null);
            audTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (No format)", gMKVExtractFilenamePatterns.TrackNumber, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (1 digit)", gMKVExtractFilenamePatterns.TrackNumber_0, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (2 digits)", gMKVExtractFilenamePatterns.TrackNumber_00, txtAudioTracksFilename));
            audTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (3 digits)", gMKVExtractFilenamePatterns.TrackNumber_000, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(audTrackNumber);
            //_AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtAudioTracksFilename));

            var audTrackID = new ToolStripMenuItem("Track ID...", null);
            audTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (No format)", gMKVExtractFilenamePatterns.TrackID, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (1 digit)", gMKVExtractFilenamePatterns.TrackID_0, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (2 digits)", gMKVExtractFilenamePatterns.TrackID_00, txtAudioTracksFilename));
            audTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (3 digits)", gMKVExtractFilenamePatterns.TrackID_000, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(audTrackID);
            //_AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtAudioTracksFilename));

            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Name", gMKVExtractFilenamePatterns.TrackName, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language", gMKVExtractFilenamePatterns.TrackLanguage, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language IETF", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec ID", gMKVExtractFilenamePatterns.TrackCodecID, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec Private", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Delay", gMKVExtractFilenamePatterns.TrackDelay, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Effective Delay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add("-");

            var subTrackNumber = new ToolStripMenuItem("Track Number...", null);
            subTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (No format)", gMKVExtractFilenamePatterns.TrackNumber, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (1 digit)", gMKVExtractFilenamePatterns.TrackNumber_0, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (2 digits)", gMKVExtractFilenamePatterns.TrackNumber_00, txtSubtitleTracksFilename));
            subTrackNumber.DropDownItems.Add(GetToolstripMenuItem("Track Number (3 digits)", gMKVExtractFilenamePatterns.TrackNumber_000, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(subTrackNumber);
            //_SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Number", gMKVExtractFilenamePatterns.TrackNumber, txtSubtitleTracksFilename));

            var subTrackID = new ToolStripMenuItem("Track ID...", null);
            subTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (No format)", gMKVExtractFilenamePatterns.TrackID, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (1 digit)", gMKVExtractFilenamePatterns.TrackID_0, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (2 digits)", gMKVExtractFilenamePatterns.TrackID_00, txtSubtitleTracksFilename));
            subTrackID.DropDownItems.Add(GetToolstripMenuItem("Track ID (3 digits)", gMKVExtractFilenamePatterns.TrackID_000, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(subTrackID);
            //_SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track ID", gMKVExtractFilenamePatterns.TrackID, txtSubtitleTracksFilename));

            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Name", gMKVExtractFilenamePatterns.TrackName, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language", gMKVExtractFilenamePatterns.TrackLanguage, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Language IETF", gMKVExtractFilenamePatterns.TrackLanguageIetf, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec ID", gMKVExtractFilenamePatterns.TrackCodecID, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Codec Private", gMKVExtractFilenamePatterns.TrackCodecPrivate, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Delay", gMKVExtractFilenamePatterns.TrackDelay, txtSubtitleTracksFilename));
            _SubtitleTrackContextMenu.Items.Add(GetToolstripMenuItem("Track Effective Delay", gMKVExtractFilenamePatterns.TrackEffectiveDelay, txtSubtitleTracksFilename));
            // ============================================================================================================================

            // Video Track placeholders
            // ============================================================================================================================
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Video Pixel Width", gMKVExtractFilenamePatterns.VideoPixelWidth, txtVideoTracksFilename));
            _VideoTrackContextMenu.Items.Add(GetToolstripMenuItem("Video Pixel Height", gMKVExtractFilenamePatterns.VideoPixelHeight, txtVideoTracksFilename));
            // ============================================================================================================================

            // Audio Track placeholders
            // ============================================================================================================================
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Audio Sampling Frequency", gMKVExtractFilenamePatterns.AudioSamplingFrequency, txtAudioTracksFilename));
            _AudioTrackContextMenu.Items.Add(GetToolstripMenuItem("Audio Channels", gMKVExtractFilenamePatterns.AudioChannels, txtAudioTracksFilename));
            // ============================================================================================================================

            // Attachment placeholders
            // ============================================================================================================================
            var attachmentID = new ToolStripMenuItem("Attachment ID...", null);
            attachmentID.DropDownItems.Add(GetToolstripMenuItem("Attachment ID (No format)", gMKVExtractFilenamePatterns.AttachmentID, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetToolstripMenuItem("Attachment ID (1 digit)", gMKVExtractFilenamePatterns.AttachmentID_0, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetToolstripMenuItem("Attachment ID (2 digits)", gMKVExtractFilenamePatterns.AttachmentID_00, txtAttachmentsFilename));
            attachmentID.DropDownItems.Add(GetToolstripMenuItem("Attachment ID (3 digits)", gMKVExtractFilenamePatterns.AttachmentID_000, txtAttachmentsFilename));

            _AttachmentContextMenu.Items.Add(attachmentID);
            //_AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Attachment ID", gMKVExtractFilenamePatterns.AttachmentID, txtAttachmentsFilename));

            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Attachment Filename", gMKVExtractFilenamePatterns.AttachmentFilename, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Attachment MIME Type", gMKVExtractFilenamePatterns.AttachmentMimeType, txtAttachmentsFilename));
            _AttachmentContextMenu.Items.Add(GetToolstripMenuItem("Attachment File Size (bytes)", gMKVExtractFilenamePatterns.AttachmentFileSize, txtAttachmentsFilename));
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

                FillFromSettings();
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
