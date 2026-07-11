using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly Dictionary<Button, Size> _responsiveButtonBaseSizes = new Dictionary<Button, Size>();
        private float _actionsRowBaseHeight;

        public frmLog()
        {
            InitializeComponent();
            CaptureResponsiveLayoutBaseline();
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
            CaptureResponsiveLayoutBaseline();
            ApplyResponsiveLayout();
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
            if (grpActions == null
                || tlpMain == null
                || btnClear == null
                || btnSave == null
                || btnRefresh == null
                || btnCopy == null
                || btnClose == null)
            {
                return;
            }

            btnClear.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnClear, ActionButtonMinWidth));
            btnSave.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnSave, ActionButtonMinWidth));
            btnRefresh.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnRefresh, ActionButtonMinWidth));
            btnCopy.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnCopy, ActionButtonMinWidth));
            btnClose.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(btnClose, ActionButtonMinWidth));

            int buttonTop = grpActions.GetGroupBoxContentTop() - 2;
            int rowHeight = new[] { btnClear.Height, btnSave.Height, btnRefresh.Height, btnCopy.Height, btnClose.Height }.Max();

            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnClear.Location = new Point(6, btnClear.GetVerticallyCenteredTop(buttonTop, rowHeight));
            btnSave.Location = new Point(btnClear.Right + ActionButtonSpacing, btnSave.GetVerticallyCenteredTop(buttonTop, rowHeight));

            int right = grpActions.ClientSize.Width - 5;
            btnClose.Location = new Point(right - btnClose.Width, btnClose.GetVerticallyCenteredTop(buttonTop, rowHeight));
            right = btnClose.Left - ActionButtonSpacing;
            btnCopy.Location = new Point(right - btnCopy.Width, btnCopy.GetVerticallyCenteredTop(buttonTop, rowHeight));
            right = btnCopy.Left - ActionButtonSpacing;
            btnRefresh.Location = new Point(right - btnRefresh.Width, btnRefresh.GetVerticallyCenteredTop(buttonTop, rowHeight));

            if (tlpMain.RowStyles.Count > 1)
            {
                int requiredHeight = new[] { btnClear.Bottom, btnSave.Bottom, btnRefresh.Bottom, btnCopy.Bottom, btnClose.Bottom }.Max()
                    + 6
                    + grpActions.Margin.Vertical;
                float minimumHeight = _actionsRowBaseHeight > 0F ? _actionsRowBaseHeight : tlpMain.RowStyles[1].Height;
                tlpMain.RowStyles[1].Height = Math.Max(minimumHeight, requiredHeight);
            }
        }

        private void CaptureResponsiveLayoutBaseline()
        {
            // Preserve the logical baseline from InitializeComponent(); capturing the
            // post-autoscale absolute sizes keeps the log action bar too tall at high DPI.
            CaptureResponsiveButtonBaseSize(btnClear);
            CaptureResponsiveButtonBaseSize(btnSave);
            CaptureResponsiveButtonBaseSize(btnRefresh);
            CaptureResponsiveButtonBaseSize(btnCopy);
            CaptureResponsiveButtonBaseSize(btnClose);

            if (_actionsRowBaseHeight <= 0F
                && tlpMain.RowStyles.Count > 1
                && tlpMain.RowStyles[1].Height > 0F)
            {
                _actionsRowBaseHeight = NormalizeResponsiveBaselineHeight(tlpMain.RowStyles[1].Height);
            }
        }

        private void CaptureResponsiveButtonBaseSize(Button button)
        {
            if (button == null)
            {
                return;
            }

            Size currentSize = button.Size;
            if (!_responsiveButtonBaseSizes.ContainsKey(button)
                && currentSize.Width > 0
                && currentSize.Height > 0)
            {
                _responsiveButtonBaseSizes[button] = NormalizeResponsiveBaselineSize(currentSize);
            }
        }

        private Size GetResponsiveButtonBaseSize(Button button, int fallbackWidth, int fallbackHeight = 30)
        {
            return _responsiveButtonBaseSizes.TryGetValue(button, out Size baseSize)
                ? baseSize
                : new Size(fallbackWidth, fallbackHeight);
        }

        private Size NormalizeResponsiveBaselineSize(Size currentSize)
        {
            float scaleFactor = GetResponsiveBaselineScaleFactor();
            if (scaleFactor <= 1F)
            {
                return currentSize;
            }

            return new Size(
                Math.Max(1, (int)Math.Round(currentSize.Width / scaleFactor)),
                Math.Max(1, (int)Math.Round(currentSize.Height / scaleFactor)));
        }

        private float NormalizeResponsiveBaselineHeight(float currentHeight)
        {
            float scaleFactor = GetResponsiveBaselineScaleFactor();
            return scaleFactor > 1F ? currentHeight / scaleFactor : currentHeight;
        }

        private float GetResponsiveBaselineScaleFactor()
        {
            float scaleFactor = 1F;

            if (AutoScaleMode == AutoScaleMode.Dpi
                && AutoScaleDimensions.Width > 0F
                && AutoScaleDimensions.Height > 0F
                && CurrentAutoScaleDimensions.Width > 0F
                && CurrentAutoScaleDimensions.Height > 0F)
            {
                scaleFactor = Math.Max(
                    CurrentAutoScaleDimensions.Width / AutoScaleDimensions.Width,
                    CurrentAutoScaleDimensions.Height / AutoScaleDimensions.Height);
            }

            if (scaleFactor <= 1F && currentDpi > 0F)
            {
                scaleFactor = currentDpi / DESIGN_TIME_DPI;
            }

            return scaleFactor > 0F ? scaleFactor : 1F;
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyResponsiveLayout();
        }

        protected override void OnDPIChanged()
        {
            base.OnDPIChanged();

            if (oldDpi == 0F
                || oldDpi == currentDpi
                || !IsHandleCreated
                || IsDisposed
                || Disposing)
            {
                return;
            }

            ApplyResponsiveLayout();
        }
    }
}
