using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using gMKVToolNix.Localization;

namespace gMKVToolNix
{
    public class gForm : Form
    {
        public static short LOWORD(int number)
        {
            return (short)number;
        }

        protected const int WM_DPICHANGED = 0x02E0;
        protected const float DESIGN_TIME_DPI = 96F;

        protected float oldDpi;
        protected float currentDpi;

        protected bool isMoving = false;
        protected bool shouldScale = false;

        /// <summary>
        /// Gets the form's border width in pixels
        /// </summary>
        public int BorderWidth
        {
            get { return Convert.ToInt32((double)(this.Width - this.ClientSize.Width) / 2.0); }
        }

        /// <summary>
        /// Gets the form's Title Bar Height in pixels
        /// </summary>
        public int TitlebarHeight
        {
            get { return this.Height - this.ClientSize.Height - 2 * BorderWidth; }
        }

        public gForm() :base()
        {
            // Apply the design-time DPI baseline in the base form so every derived form
            // gets Dpi autoscaling even if its designer file omits these properties.
            this.AutoScaleDimensions = new SizeF(DESIGN_TIME_DPI, DESIGN_TIME_DPI);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected string GetLocalizedString(string key, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                return LocalizationManager.GetString(key, args);
            }

            return LocalizationManager.GetString(key);
        }

        protected Exception CreateLocalizedException(string key, params object[] args)
        {
            return new Exception(GetLocalizedString(key, args));
        }

        protected void ShowLocalizedSuccessMessage(string messageKey, bool dialogOnTop = false, params object[] args)
        {
            ShowSuccessMessage(GetLocalizedString(messageKey, args), dialogOnTop);
        }

        protected void ShowLocalizedErrorMessage(string messageKey, bool dialogOnTop = false, params object[] args)
        {
            ShowErrorMessage(GetLocalizedString(messageKey, args), dialogOnTop);
        }

        protected DialogResult ShowLocalizedQuestion(string messageKey, string titleKey, bool showCancel = true, params object[] args)
        {
            return ShowQuestion(GetLocalizedString(messageKey, args), GetLocalizedString(titleKey), showCancel);
        }

        protected void InitDPI()
        {
            // Preserve previous DPI value
            oldDpi = currentDpi;
            float dx;
            using (Graphics g = this.CreateGraphics())
            {
                dx = g.DpiX;
            }
            currentDpi = dx;

            // On some runtimes (notably Mono) WinForms AutoScaleMode.Dpi is not applied at startup.
            // In that case we need to force the initial full scaling (sizes + fonts) by pretending
            // the previous DPI was the design-time DPI so HandleDpiChanged runs the full scaling path.
            if (oldDpi == 0F && PlatformExtensions.IsOnLinux)
            {
                oldDpi = DESIGN_TIME_DPI;
            }

            HandleDpiChanged();
            OnDPIChanged();
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);

            this.isMoving = true;
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            this.isMoving = false;
            if (shouldScale)
            {
                shouldScale = false;
                HandleDpiChanged();
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            if (this.shouldScale && CanPerformScaling())
            {
                this.shouldScale = false;
                HandleDpiChanged();
            }
        }

        protected bool CanPerformScaling()
        {
            return (Screen.FromControl(this).Bounds.Contains(this.Bounds));
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // This message is sent when the form is dragged to a different monitor i.e. when
                // the bigger part of its are is on the new monitor. Note that handling the message immediately
                // might change the size of the form so that it no longer overlaps the new monitor in its bigger part
                // which in turn will send again the WM_DPICHANGED message and this might cause misbehavior.
                // Therefore we delay the scaling if the form is being moved and we use the CanPerformScaling method to 
                // check if it is safe to perform the scaling.
                case WM_DPICHANGED:
                    oldDpi = currentDpi;
                    currentDpi = LOWORD((int)m.WParam);

                    if (oldDpi != currentDpi)
                    {
                        if (this.isMoving)
                        {
                            shouldScale = true;
                        }
                        else
                        {
                            HandleDpiChanged();
                        }

                        OnDPIChanged();
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        protected void HandleDpiChanged()
        {
            if (oldDpi != 0F)
            {
                float scaleFactor = currentDpi / oldDpi;

                // The default scaling method of the framework
                this.Scale(new SizeF(scaleFactor, scaleFactor));

                // Fonts are not scaled automatically so we need to handle this manually
                this.ScaleFonts(scaleFactor);

                // Perform any other scaling different than font or size (e.g. ItemHeight)
                this.PerformSpecialScaling(scaleFactor);
            }
            else
            {
                // The special scaling also needs to be done initially
                this.PerformSpecialScaling(currentDpi / DESIGN_TIME_DPI);
            }
        }

        protected virtual void ScaleFonts(float scaleFactor)
        {
            // Go through all controls in the control tree and set their Font property
            ScaleFontForControl(this, scaleFactor);
        }

        protected static void ScaleFontForControl(Control control, float scaleFactor)
        {
            control.Font = new Font(control.Font.FontFamily, control.Font.Size * scaleFactor, control.Font.Style);

            foreach (Control child in control.Controls)
            {
                ScaleFontForControl(child, scaleFactor);
            }
        }

        protected virtual void PerformSpecialScaling(float scaleFactor)
        {
        }

        protected virtual void OnDPIChanged()
        {
        }

        /// <summary>
        /// Returns the full path and filename of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected string GetExecutingAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Returns the current directory of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Returns the version of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        protected void ShowErrorMessage(string argMessage, bool dialogOnTop = false)
        {
            string dialogMessage = LocalizationManager.GetString("UI.Common.Dialog.ErrorMessage", Environment.NewLine, argMessage);
            string dialogTitle = LocalizationManager.GetString("UI.Common.Dialog.ErrorTitle");

            if (dialogOnTop)
            {
                // Create a dummy form that is on top of all other windows in desktop
                using (Form form = new Form { TopMost = true })
                {
                    MessageBox.Show(
                        form,
                        dialogMessage,
                        dialogTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    dialogMessage,
                    dialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Return to the original form
            this.BringToFront();
            this.Activate();
            this.Show();
        }

        protected void ShowSuccessMessage(string argMessage, bool dialogOnTop = false)
        {
            string dialogTitle = LocalizationManager.GetString("UI.Common.Dialog.SuccessTitle");

            if (dialogOnTop)
            {
                // Create a dummy form that is on top of all other windows in desktop
                using (Form form = new Form { TopMost = true })
                {
                    MessageBox.Show(
                        form,
                        argMessage,
                        dialogTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else 
            {
                MessageBox.Show(
                    this,
                    argMessage,
                    dialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            // Return to the original form
            this.BringToFront();
            this.Activate();
            this.Show();
        }

        protected DialogResult ShowQuestion(string argQuestion, string argTitle, bool argShowCancel = true)
        {
            MessageBoxButtons msgBoxBtns = MessageBoxButtons.YesNoCancel;
            if (!argShowCancel)
            {
                msgBoxBtns = MessageBoxButtons.YesNo;
            }

            return MessageBox.Show(
                this, 
                argQuestion, 
                argTitle, 
                msgBoxBtns, 
                MessageBoxIcon.Question);
        }

        protected void ToggleControls(Control argRootControl, bool argStatus)
        {
            foreach (Control ctrl in argRootControl.Controls)
            {
                if (ctrl is IContainer)
                {
                    ToggleControls(ctrl, argStatus);
                }
                else
                {
                    ctrl.Enabled = argStatus;
                }
            }
        }
    }
}
