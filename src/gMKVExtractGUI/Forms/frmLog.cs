using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix
{
    public partial class frmLog : gForm
    {
        private readonly gSettings _Settings = null;
        private const int ActionButtonMinWidth = 95;
        private const int ActionButtonSpacing = 4;

        public frmLog()
        {
            InitializeComponent();
            InitForm();

            _Settings = new gSettings(this.GetCurrentDirectory());
            _Settings.Reload();

            ThemeManager.ApplyTheme(this, _Settings.DarkMode);

            if (this.Handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
                NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);
            }
            else
            {
                this.Shown += (s, ev) => {
                    NativeMethods.SetWindowThemeManaged(this.Handle, _Settings.DarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, _Settings.DarkMode);
                };
            }

            ApplyLocalization();

            InitDPI();
        }

        private void InitForm()
        {
            Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
            Text = string.Format("gMKVExtractGUI v{0} -- Log", GetCurrentVersion());
        }

        private void frmLog_Activated(object sender, EventArgs e)
        {
            SetLogText(gMKVLogger.LogText);
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.Select(txtLog.TextLength, 0);
            txtLog.ScrollToCaret();
            UpdateLogGroupTitle();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtLog.SelectedText))
                {
                    Clipboard.SetData(DataFormats.UnicodeText, txtLog.SelectedText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetLogText(gMKVLogger.LogText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
            this.Hide();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShowLocalizedQuestion("UI.Log.Dialogs.ClearLogQuestion", "UI.Common.Dialog.AreYouSureTitle") == DialogResult.Yes)
                {
                    gMKVLogger.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = LocalizationManager.GetString("UI.Log.Dialogs.SelectFilenameTitle");
                sfd.CheckFileExists = false; // Changed to false to allow creating new files
                sfd.DefaultExt = "txt";
                sfd.Filter = "*.txt|*.txt";
                sfd.FileName = string.Format("[{0}][{1}][gMKVExtractGUI_v{2}].txt",
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    DateTime.Now.ToString("HH-mm-ss"),
                    Assembly.GetExecutingAssembly().GetName().Version);
                if (sfd.ShowDialog() == DialogResult.OK) // ShowDialog returns OK, not Yes
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.Write(gMKVLogger.LogText);
                    }
                    ShowLocalizedSuccessMessage("UI.Log.Success.LogSaved", false, sfd.FileName);
                }
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
            this.Text = string.Format("gMKVExtractGUI v{0} -- {1}", GetCurrentVersion(), LocalizationManager.GetString("UI.LogForm.Title"));
            grpActions.Text = LocalizationManager.GetString("UI.LogForm.Actions.Group");
            btnSave.Text = LocalizationManager.GetString("UI.LogForm.Actions.Save");
            btnClear.Text = LocalizationManager.GetString("UI.LogForm.Actions.ClearLog");
            btnRefresh.Text = LocalizationManager.GetString("UI.LogForm.Actions.Refresh");
            btnCopy.Text = LocalizationManager.GetString("UI.LogForm.Actions.CopySelection");
            btnClose.Text = LocalizationManager.GetString("UI.LogForm.Actions.Close");
            SetLogText(gMKVLogger.LogText);
            ApplyResponsiveLayout();
        }

        public void UpdateTheme(bool darkMode)
        {
            ThemeManager.ApplyTheme(this, darkMode);
            if (this.IsHandleCreated) // Important check
            {
                NativeMethods.SetWindowThemeManaged(this.Handle, darkMode);
                NativeMethods.TrySetImmersiveDarkMode(this.Handle, darkMode);
            }
            else
            {
                // If handle not created yet, defer until it is.
                // This might be less critical for already shown forms but good for robustness.
                this.HandleCreated += (s, e) => {
                    NativeMethods.SetWindowThemeManaged(this.Handle, darkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, darkMode);
                };
            }
        }

        private void ApplyResponsiveLayout()
        {
            btnClear.ApplyLocalizedButtonSize(ActionButtonMinWidth);
            btnSave.ApplyLocalizedButtonSize(ActionButtonMinWidth);
            btnRefresh.ApplyLocalizedButtonSize(ActionButtonMinWidth);
            btnCopy.ApplyLocalizedButtonSize(ActionButtonMinWidth);
            btnClose.ApplyLocalizedButtonSize(ActionButtonMinWidth);

            const int buttonTop = 17;

            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnClear.Location = new Point(6, buttonTop);
            btnSave.Location = new Point(btnClear.Right + ActionButtonSpacing, buttonTop);

            int right = grpActions.ClientSize.Width - 5;
            btnClose.Location = new Point(right - btnClose.Width, buttonTop);
            right = btnClose.Left - ActionButtonSpacing;
            btnCopy.Location = new Point(right - btnCopy.Width, buttonTop);
            right = btnCopy.Left - ActionButtonSpacing;
            btnRefresh.Location = new Point(right - btnRefresh.Width, buttonTop);
        }

        private void SetLogText(string logText)
        {
            txtLog.Clear();
            txtLog.Text = logText ?? string.Empty;
        }

        private void UpdateLogGroupTitle()
        {
            grpLog.Text = LocalizationManager.GetString("UI.LogForm.Log.GroupWithCount", txtLog.Lines.LongLength);
        }
    }
}
